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
    [DocComments("\"Buyers\" represents Buyers for Headstart")]
    [HSSection.Headstart(ListOrder = 1)]
    [Route("buyer")]
    public class BuyerController : BaseController
    {
        private readonly IHSBuyerCommand _command;
        private readonly IOrderCloudClient _oc;
        public BuyerController(IHSBuyerCommand command, IOrderCloudClient oc)
        {
            _command = command;
            _oc = oc;
        }

        [DocName("POST Headstart Buyer")]
        [HttpPost, OrderCloudUserAuth(ApiRole.BuyerAdmin)]
        public async Task<SuperHSBuyer> Create([FromBody] SuperHSBuyer buyer)
        {
            return await _command.Create(buyer, UserContext.AccessToken);
        }

        [DocName("PUT Headstart Buyer")]
        [HttpPut, Route("{buyerID}"), OrderCloudUserAuth(ApiRole.BuyerAdmin)]
        public async Task<SuperHSBuyer> Put([FromBody] SuperHSBuyer superBuyer, string buyerID)
        {
            return await _command.Update(buyerID, superBuyer, UserContext.AccessToken);
        }

        [DocName("GET Headstart Buyer")]
        [HttpGet, Route("{buyerID}"), OrderCloudUserAuth(ApiRole.BuyerAdmin)]
        public async Task<SuperHSBuyer> Get(string buyerID)
        {
            return await _command.Get(buyerID, UserContext.AccessToken);
        }

        [DocName("GET All Headstart Buyers")]
        [HttpGet]
        public async Task<List<Buyer>> GetAll()
        {
            return await _oc.Buyers.ListAllAsync();
        }

        [DocName("GET Headstart Buyers filtered to what you can see")]
        [HttpGet("my"), OrderCloudUserAuth]
        public async Task<ListPage<Buyer>> MyBuyers()
        {
            return await _command.MyBuyers(UserContext.AccessToken);
        }

        [DocName("GET All Headstart Buyer Users you have access to")]
        [HttpGet("users"), OrderCloudUserAuth]
        public async Task<ListPage<User>> BuyerUsers()
        {
            return await _command.MyUsers(UserContext.AccessToken);
        }
    }
}
