using AzureStorageUtilities.Default;
using AzureStorageUtilities.Interfaces;
using Headstart.Common;
using OrderCloud.SDK;
using System.IO;
using System.Threading.Tasks;

namespace Headstart.API.Commands
{
    public interface IHSBrandingCommand
    {
        Task<Stream> GetStylesheetForBrand(string brand);
    }
    public class HSBrandingCommand : IHSBrandingCommand
    {
        private readonly IOrderCloudClient _oc;
        private readonly AppSettings _settings;
        private readonly IBlob _blob;

        public HSBrandingCommand(AppSettings settings, IOrderCloudClient oc)
        {
            _settings = settings;
            _oc = oc;
            var storage = new AzureStorage(_settings.BlobSettings.ConnectionString);
           _blob = storage.GetBlobContainer("branding-css");
        }

        public async Task<Stream> GetStylesheetForBrand(string brand)
        {
            return await _blob.GetStreamWithFallbackAsync($"{Path.GetFileNameWithoutExtension(brand).ToLower()}.css", "_default.css");
        }
    }
}
