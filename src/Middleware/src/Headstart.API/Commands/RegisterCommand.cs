using Headstart.Common;
using Headstart.Models;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Headstart.API.Commands
{
    public interface IHSRegisterCommand
    {
        Task<HSRegister> Create(HSRegister register);
        Task<HSRegister> ApproveOrDenyBuyerAccess(BuyerAccessApproval request);
        Task<IEnumerable<HSRegister>> List();
    }
    public class HSRegisterCommand : IHSRegisterCommand
    {
        private readonly IOrderCloudClient _oc;
        private readonly AppSettings _settings;

        public HSRegisterCommand(AppSettings settings, IOrderCloudClient oc)
        {
            _settings = settings;
            _oc = oc;
        }

        public async Task<HSRegister> Create(HSRegister register)
        {
            var users = await _oc.AdminUsers.ListAsync<HSRegister>(search: register.Username);
            var existingRegister = users.Items.FirstOrDefault(x => x.Username.ToLower().Trim() == register.Username.ToLower().Trim());

            register.xp.BuyerAccessRequests.ToList().ForEach(x => x.Approved = null); // makes sure nothing can be sent in approved

            if (existingRegister != null)
            {
                try
                {
                    var tempToken = await _oc.AuthenticateAsync(_settings.OrderCloudSettings.MiddlewareClientID, register.Username, register.Password, new[] { ApiRole.BuyerAdmin });
                    if (string.IsNullOrWhiteSpace(tempToken.AccessToken))
                        throw new AccessViolationException("invalid password supplied");
                }
                catch(Exception ex)
                {
                    // TODO: figure out what to do here depending on error message
                    throw;
                }

                var requestsToAdd = register.xp.BuyerAccessRequests
                    .Where(r => !existingRegister.xp.BuyerAccessRequests
                        .Where(er => er.Approved.HasValue)
                        .Select(x => x.BuyerId).Contains(r.BuyerId));

                var partialUser = new PartialUser<RegisterXp>()
                {
                    xp = new {
                        BuyerAccessRequests = existingRegister.xp.BuyerAccessRequests.Concat(requestsToAdd)
                    },
                };
                return await _oc.AdminUsers.PatchAsync<HSRegister>(existingRegister.ID, partialUser);
            }
            else
            {
                return await _oc.AdminUsers.CreateAsync<HSRegister>(register);
            }
        }

        public async Task<IEnumerable<HSRegister>> List()
        {
            var users = (await _oc.AdminUsers.ListAsync<HSRegister>()).Items;
            return users.Where(x => x.xp != null && x.xp.BuyerAccessRequests != null && x.xp.BuyerAccessRequests.Any(x => !x.Approved.HasValue));
        }

        public async Task<HSRegister> ApproveOrDenyBuyerAccess(BuyerAccessApproval buyerAccessApproval)
        {
            var user = await _oc.AdminUsers.GetAsync<HSRegister>(buyerAccessApproval.UserId);
            var requests = user.xp.BuyerAccessRequests.ToList();
            foreach(var request in requests)
            {
                if (request.BuyerId.Equals(buyerAccessApproval.BuyerId, StringComparison.InvariantCultureIgnoreCase))
                    request.Approved = buyerAccessApproval.Approved;
            }

            var partialUser = new PartialUser<RegisterXp>()
            {
                xp = new { BuyerAccessRequests = requests },
                Active = buyerAccessApproval.Approved || user.Active
            };
            await ActivateBuyerAccess(buyerAccessApproval);

            return await _oc.AdminUsers.PatchAsync<HSRegister>(user.ID, partialUser);
        }

        private async Task ActivateBuyerAccess(BuyerAccessApproval buyerAccessApproval)
        {
            if (buyerAccessApproval.Approved)
            {
                // add to the user group (silent if they are already in it)
                await _oc.AdminUserGroups.SaveUserAssignmentAsync(new UserGroupAssignment()
                {
                     UserGroupID = "Admin_BuyerImpersonator",
                     UserID = buyerAccessApproval.UserId
                });
                await _oc.ImpersonationConfigs.CreateAsync(new ImpersonationConfig()
                {
                    ImpersonationUserID = buyerAccessApproval.UserId,
                    BuyerID = buyerAccessApproval.BuyerId,
                    SecurityProfileID = "HSBuyerAdmin",
                    ClientID = _settings.OrderCloudSettings.MiddlewareClientID
                });
            }
        }
    }
}
