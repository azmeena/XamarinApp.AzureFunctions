using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using ImHere.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace ImHere.Functions
{
    public static class SendLocation
    {
        [FunctionName("SendLocation")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous,
                "get", "post",
                Route = null)]HttpRequest req,
            [TwilioSms(AccountSidSetting = "TwilioAccountSid",
                AuthTokenSetting = "TwilioAuthToken",
                From = "<TwilioNumberPlaceholder>")] ICollector<CreateMessageOptions> messages,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            PostData data = JsonConvert.DeserializeObject<PostData>(requestBody);
            string url = $"https://www.google.com/maps/search/?api=1&query={data.Latitude},{data.Longitude}";
            log.LogInformation($"URL created - {url}");

            foreach (string toNo in data.ToNumbers)
            {
                PhoneNumber number = new PhoneNumber(toNo);
                CreateMessageOptions message = new CreateMessageOptions(number)
                {
                    Body = $"I'm here! {url}"
                };
                log.LogInformation($"Creating SMS message to {message.To}, message is '{message.Body}'.");
                messages.Add(message);
            }

            return new OkResult();
        }
    }
}