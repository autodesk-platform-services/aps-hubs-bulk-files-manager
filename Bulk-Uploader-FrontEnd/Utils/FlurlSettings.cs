using Flurl.Http;
using Flurl.Http.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Bulk_Uploader_Electron.Utils
{
    public class APSSettings
    {
        public static APSSettings DefaultSettings { get; private set; } = new APSSettings();

        public static void SetFlurSettings(bool logCalls, bool logErrors, bool enableHttpRetries)
        {
            FlurlHttp.Configure(settings =>
            {
                if (logCalls)
                {
                    settings.BeforeCall += APSSettings.BeforeCall;
                    settings.AfterCall += APSSettings.AfterCall;
                }
                if (logErrors)
                {
                    settings.OnErrorAsync += APSSettings.OnError;
                }
                if (enableHttpRetries)
                {
                    settings.HttpClientFactory = new PollyHttpClientFactory();
                }
                var jsonSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };
                settings.JsonSerializer = new NewtonsoftJsonSerializer(jsonSettings);
            });
        }

        private static void AfterCall(FlurlCall callObject)
        {
            if (callObject.Response == null)
            {
                Serilog.Log.Verbose($"{callObject.Request.Url} did not complete");
            }
            else
            {
                Serilog.Log.Verbose($"{callObject.Request.Url} Complete ({callObject?.Response?.StatusCode} {callObject?.Duration}) | {callObject?.Response?.StatusCode.ToString() ?? "NO RESPONSE"}");
            }
        }

        private static void BeforeCall(FlurlCall callObject)
        {
            Serilog.Log.Verbose($"{callObject.Request.Url} Start");
        }

        private static async Task OnError(FlurlCall callObject)
        {
            var request = callObject.Request;
            var text = "";
            try
            {
                if (callObject.Response.StatusCode == 403)
                {
                    System.Diagnostics.Debug.WriteLine(callObject.Request.Headers[0].Value);
                }
                var contentString = await callObject.Response.GetStringAsync();//DEBUG
                text = contentString;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error($"{request.Url} Error\n message: {ex.Message}");
            }
            finally
            {
                Serilog.Log.Information($"{request.Url} {callObject?.Response?.StatusCode} Error\n message: {text} \nresponse:\n{callObject?.Response?.ResponseMessage}");
            }
        }
    }
}