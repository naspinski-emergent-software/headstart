using Microsoft.WindowsAzure.Storage.Table;
using ordercloud.integrations.library;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;

namespace Headstart.Models
{
    //[SwaggerModel]
    //public class SuperHSRegister :
    //{
    //    public HSRegister Buyer { get; set; }
    //    public BuyerMarkup Markup { get; set; }
    //}

    [SwaggerModel]
    public class HSRegister : User, IHSObject
    {
        public IEnumerable<BuyerAccessRequest> BuyerAccessRequests { get; set; } = new List<BuyerAccessRequest>();
        public string BuyerClientId { get; set; }
    }

    //[SwaggerModel]
    //public class RegisterXp
    //{
    //    // temporary field while waiting on content docs
    //    public IEnumerable<BuyerAccessRequest> BuyerAccessRequests { get; set; }
    //}

    public class BuyerAccessRequest : TableEntity, ITableEntity
    {
        public BuyerAccessRequest() { }
        public BuyerAccessRequest(string userId, string buyerId)
        {
            PartitionKey = userId;
            RowKey = buyerId;
        }

        public string BuyerId { get => RowKey; set => RowKey = value; }
        public string UserId { get => PartitionKey; set => PartitionKey = value; }
        public string BuyerName { get; set; }
        public bool? Approved { get; set; }
    }

    public class BuyerAccessApproval
    {
        public string UserId { get; set; }
        public string BuyerId { get; set; }
        public bool Approved { get; set; }
    }
}
