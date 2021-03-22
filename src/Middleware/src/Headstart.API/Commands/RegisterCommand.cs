using AzureStorageUtilities.Default;
using AzureStorageUtilities.Interfaces;
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
        Task ApproveOrDenyBuyerAccess(BuyerAccessApproval request);
        Task<IEnumerable<HSRegister>> List();
    }
    public class HSRegisterCommand : IHSRegisterCommand
    {
        private readonly IOrderCloudClient _oc;
        private readonly AppSettings _settings;
        private readonly ITable _table;
        //var token = ocClient.TokenResponse.AccessToken;
        public HSRegisterCommand(AppSettings settings, IOrderCloudClient oc)
        {
            _settings = settings;
            _oc = oc;
            var storage = new AzureStorage(_settings.BlobSettings.ConnectionString);
            _table = storage.GetTable("BuyerAccessRequests");
        }

        public async Task<HSRegister> Create(HSRegister register)
        {
            var users = await _oc.AdminUsers.ListAsync<HSRegister>(search: register.Username.Trim());
            var existingRegister = users.Items.FirstOrDefault(x => x.Username.ToLower().Trim() == register.Username.ToLower().Trim());

            register.BuyerAccessRequests.ToList().ForEach(x => x.Approved = null); // makes sure nothing can be sent in approved

            if (existingRegister != null)
            {
                try
                {
                    var tempToken = await _oc.AuthenticateAsync("078B4032-EA12-431F-B32B-4DAF790A152F", register.Username, register.Password, new[] { ApiRole.BuyerAdmin });
                    if (string.IsNullOrWhiteSpace(tempToken.AccessToken))
                        throw new AccessViolationException("invalid password supplied");
                }
                catch(Exception ex)
                {
                    // TODO: figure out what to do here depending on error message
                    throw;
                }

                var existingRequests = await _table.GetAsync<BuyerAccessRequest>(existingRegister.ID);
                var requestsToAdd = register.BuyerAccessRequests
                    .Where(r => !existingRegister.BuyerAccessRequests
                        .Where(er => er.Approved.HasValue)
                        .Select(x => x.BuyerId).Contains(r.BuyerId))
                    .ToList();
                requestsToAdd.ForEach(bar => bar.UserId = existingRegister.ID);
                await _table.AddAsync(requestsToAdd);

                return existingRegister;
            }
            else
            {
                var user = await _oc.AdminUsers.CreateAsync<HSRegister>(register);
                register.BuyerAccessRequests.ToList().ForEach(bar => bar.UserId = user.ID);
                await _table.AddAsync(register.BuyerAccessRequests);
                return user;
            }
        }

        public async Task<IEnumerable<HSRegister>> List()
        {
            var users = (await _oc.AdminUsers.ListAsync<HSRegister>()).Items;
            foreach(var user in users)
            {
                var requests = await _table.GetAsync<BuyerAccessRequest>(user.ID);
                if (requests.Any(x => x.Approved == null))
                    user.BuyerAccessRequests = requests.Where(x => x.Approved == null).ToList();
            }

            return users.Where(x => x.BuyerAccessRequests.Any());
        }

        public async Task ApproveOrDenyBuyerAccess(BuyerAccessApproval buyerAccessApproval)
        {
            var user = await _oc.AdminUsers.GetAsync<HSRegister>(buyerAccessApproval.UserId);
            var requests = await _table.GetAsync<BuyerAccessRequest>(user.ID);
            foreach(var request in requests)
            {
                if (request.BuyerId.Equals(buyerAccessApproval.BuyerId, StringComparison.InvariantCultureIgnoreCase))
                {
                    request.Approved = buyerAccessApproval.Approved;
                    await ActivateBuyerAccess(buyerAccessApproval);
                    await _table.SaveAsync(request);
                }
            }
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
