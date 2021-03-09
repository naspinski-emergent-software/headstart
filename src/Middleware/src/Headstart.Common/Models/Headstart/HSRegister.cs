using ordercloud.integrations.library;
using OrderCloud.SDK;
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
    public class HSRegister : User<RegisterXp>, IHSObject
    {

    }

    [SwaggerModel]
    public class RegisterXp
    {
        // temporary field while waiting on content docs
        public IEnumerable<BuyerAccessRequest> BuyerAccessRequests { get; set; }
    }

    public class BuyerAccessRequest
    {
        public string BuyerId { get; set; }
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
