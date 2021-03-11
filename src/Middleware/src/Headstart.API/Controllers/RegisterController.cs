using Headstart.Models;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System.Threading.Tasks;
using Headstart.Models.Attributes;
using ordercloud.integrations.library;
using Headstart.API.Commands;
using OrderCloud.Catalyst;
using System.Collections.Generic;

namespace Headstart.Common.Controllers
{
    [DocComments("\"Register\" self-registration actions")]
    [HSSection.Integration(ListOrder = 6)]
    [Route("adminuser")]
    public class RegisterController : BaseController
    {
        private readonly IHSRegisterCommand _command;
        private readonly IOrderCloudClient _oc;
        public RegisterController(IHSRegisterCommand command, IOrderCloudClient oc)
        {
            _command = command;
            _oc = oc;
        }

        [DocName("POST Headstart Register")]
        [HttpPost, Route("register")]
        public async Task<HSRegister> Create([FromBody] HSRegister register)
        {
            return await _command.Create(register);
        }

        [DocName("PUT Headstart Admin Buyer Access Approval/Denial")]
        [HttpPut, Route("buyer-access-approval"), OrderCloudUserAuth(ApiRole.AdminUserAdmin)]
        public async Task<HSRegister> ApproveOrDenyBuyerAccess([FromBody] BuyerAccessApproval request)
        {
            return await _command.ApproveOrDenyBuyerAccess(request);
        }

        [DocName("GET Headstart Admins with Buyer Access Approvals")]
        [HttpGet, Route("buyer-access-approval"), OrderCloudUserAuth(ApiRole.AdminUserAdmin)]
        public async Task<IEnumerable<HSRegister>> Get()
        {
            return await _command.List();
        }
    }
}
