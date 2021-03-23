using Headstart.API.Commands;
using Headstart.Models.Attributes;
using Microsoft.AspNetCore.Mvc;
using ordercloud.integrations.library;
using OrderCloud.Catalyst;
using OrderCloud.SDK;
using System.Threading.Tasks;

namespace Headstart.Common.Controllers
{
    [DocComments("\"Branding\" branding resources")]
    [HSSection.Integration(ListOrder = 7)]
    [Route("branding")]
    public class BrandingController : BaseController
    {
        private readonly IHSBrandingCommand _command;
        private readonly IOrderCloudClient _oc;
        public BrandingController(IHSBrandingCommand command, IOrderCloudClient oc)
        {
            _command = command;
            _oc = oc;
        }

        [HttpGet, Route("{brand}")]
        public async Task<IActionResult> Stylesheet(string brand)
        {
            var stream = await _command.GetStylesheetForBrand(brand);

            if (stream == null)
                return NotFound();

            return File(stream, "text/css");
        }
    }
}
