using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Owin;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http;

namespace SignalRServer
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            HttpConfiguration config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            config.EnsureInitialized();

            JsonMediaTypeFormatter jsonFormatter = new JsonMediaTypeFormatter();
            JsonSerializerSettings settings = jsonFormatter.SerializerSettings;
            IsoDateTimeConverter timeConverter = new IsoDateTimeConverter()
            {
                DateTimeFormat = "yyyy'-'MM'-'dd' 'HH':'mm':'ss"
            };
            settings.Converters.Add(timeConverter);
            config.Services.Replace(typeof(IContentNegotiator), new JsonContentNegotiator(jsonFormatter));

            GlobalHost.Configuration.MaxIncomingWebSocketMessageSize = null;
            GlobalHost.Configuration.DefaultMessageBufferSize = 32;
            GlobalHost.Configuration.KeepAlive = null;

            appBuilder.Map("/multipleClientsHub", map =>
            {
                map.UseCors(CorsOptions.AllowAll);
                HubConfiguration hubConfig = new HubConfiguration()
                {
                    EnableJavaScriptProxies = false,

                    EnableJSONP = true,

                    EnableDetailedErrors = true
                };

                map.RunSignalR(hubConfig);
                map.RunSignalR();
            });

            appBuilder.UseCors(CorsOptions.AllowAll);
            appBuilder.UseWebApi(config);
            appBuilder.MapSignalR();
        }
    }

    public class JsonContentNegotiator : IContentNegotiator
    {
        private readonly JsonMediaTypeFormatter jsonFormatter;

        public JsonContentNegotiator(JsonMediaTypeFormatter formatter)
        {
            this.jsonFormatter = formatter;
        }

        public ContentNegotiationResult Negotiate(System.Type type, HttpRequestMessage request, IEnumerable<MediaTypeFormatter> formatters)
        {
            return new ContentNegotiationResult(this.jsonFormatter, new MediaTypeHeaderValue("application/json"));
        }
    }
}