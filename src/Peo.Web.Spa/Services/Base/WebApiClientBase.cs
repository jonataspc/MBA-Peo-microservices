using System.Text.Json;

namespace Peo.Web.Spa.Services.Base
{
    public abstract class WebApiClientBase
    {
        protected static void UpdateJsonSerializerSettings(JsonSerializerOptions settings)
        {
            settings.PropertyNameCaseInsensitive = true;
        }
    }
}
