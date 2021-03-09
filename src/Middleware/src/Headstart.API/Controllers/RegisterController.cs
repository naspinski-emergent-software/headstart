using Headstart.Models;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System.Threading.Tasks;
using Headstart.Models.Attributes;
using ordercloud.integrations.library;
using Headstart.API.Commands;
using OrderCloud.Catalyst;

namespace Headstart.Common.Controllers
{
    [DocComments("\"Register\" self-registration actions")]
    [HSSection.Integration(ListOrder = 6)]
    [Route("register")]
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
        [HttpPost]
        public async Task<HSRegister> Create([FromBody] HSRegister register)
        {
            return await _command.Create(register);
        }

        [DocName("PUT Headstart Admin Buyer Access Approval/Denial")]
        [Route("buyer-access")]
        [HttpPut]
        public async Task<HSRegister> DenyBuyerAccess([FromBody] BuyerAccessApproval request)
        {
            return await _command.ApproveOrDenyBuyerAccess(request);
        }

        //[DocName("PUT Headstart Buyer")]
        //[HttpPut, Route("{buyerID}"), OrderCloudUserAuth(ApiRole.BuyerAdmin)]
        //public async Task<SuperHSBuyer> Put([FromBody] SuperHSBuyer superBuyer, string buyerID)
        //{
        //    return await _command.Update(buyerID, superBuyer, UserContext.AccessToken);
        //}

        //[DocName("GET Headstart Buyer")]
        //[HttpGet, Route("{buyerID}"), OrderCloudUserAuth(ApiRole.BuyerAdmin)]
        //public async Task<SuperHSBuyer> Get(string buyerID)
        //{
        //    return await _command.Get(buyerID, UserContext.AccessToken);
        //}
    }
}
