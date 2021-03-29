using Headstart.Models;
using Microsoft.AspNetCore.Mvc;
using OrderCloud.SDK;
using System.Threading.Tasks;
using Headstart.Models.Attributes;
using ordercloud.integrations.library;
using Headstart.API.Commands;
using OrderCloud.Catalyst;
using System.Collections.Generic;
using System.Linq;

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

        [DocName("GET All Headstart Buyers filtered")]
        [HttpGet("my")]
        public async Task<ListPage<Buyer>> GetAllFiltered()
        {
            var me = await _oc.Me.GetAsync();
            var fullAccess = me.AvailableRoles.Contains(ApiRole.FullAccess.ToString()) || me.AvailableRoles.Contains(ApiRole.BuyerAdmin.ToString());
            var impersonateAccess = me.AvailableRoles.Contains(ApiRole.BuyerImpersonation.ToString());
            if (!(fullAccess || impersonateAccess))
                throw new System.Exception("unable to list filtered buyers");

            var buyers = await _oc.Buyers.ListAllAsync();
            var listPage = new ListPage<Buyer>()
            {
                Items = buyers,
                Meta = new ListPageMeta() { Page = 1, PageSize = 1000, TotalCount = buyers.Count, TotalPages = 1 }
            };

            if (!fullAccess)
            {
                var impersonationBuyerIds = (await _oc.ImpersonationConfigs.ListAsync(search: me.ID, searchOn: "ImpersonationUserID"))
                    .Items.Select(x => x.BuyerID);
                listPage.Items = buyers.Where(x => impersonationBuyerIds.Contains(x.ID)).ToList();
                listPage.Meta.TotalCount = listPage.Items.Count;
            }

            return listPage;
        }
    }
}
