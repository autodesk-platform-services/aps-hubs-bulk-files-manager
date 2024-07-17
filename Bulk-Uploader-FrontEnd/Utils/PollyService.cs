using Flurl.Http.Configuration;
using Polly;
using Polly.Retry;
using Serilog;
using System.Net;

namespace Bulk_Uploader_Electron.Utils
{
    public static class Policies
    {
        public static AsyncRetryPolicy<HttpResponseMessage> RetryPolicy
        {
            get
            {
                return Policy
                    .HandleResult<HttpResponseMessage>(r =>
                    {
                        if (r.StatusCode == HttpStatusCode.TooManyRequests)
                        {
                            return true; // Return true to handle wait and retries
                        }
                        if (r.IsSuccessStatusCode)
                        {
                            return false; // Return false for no retries
                        }
                        if (r.StatusCode == HttpStatusCode.Conflict ||
                            r.StatusCode == HttpStatusCode.BadRequest)
                        {
                            return false; // Return false for no retries
                        }
                        Log.Debug($"HttpResponseMessage StatusCode={r.StatusCode}");
                        return true; // Return true to handle wait and retries
                    })
                    .Or<Exception>()
                    .WaitAndRetryAsync(
                        retryCount: 4,
                        sleepDurationProvider: (retryCount, r, _) =>
                        {
                            if (r.Result != null && r.Result.Headers.TryGetValues("Retry-After", out var values))
                            {
                                string retryAfter = values.First();
                                var tooManyRequestsDelay = Int32.Parse(retryAfter);

                                Log.Information($"Retry of HTTP call due to 429 (retryCount={retryCount} delay set for {tooManyRequestsDelay}s)");
                                return new TimeSpan(0, 0, tooManyRequestsDelay);
                            }
                            else
                            {
                                Log.Debug($"Retry of HTTP call retryCount={retryCount}");
                                return new TimeSpan(0, 0, 3 * retryCount);
                            }
                        },
                        onRetryAsync: async (_, _, _, _) =>
                        {
                            await System.Threading.Tasks.Task.CompletedTask;
                        }
                    );
            }
        }
    }

    public class PollyHttpClientFactory : DefaultHttpClientFactory
    {
        public override HttpMessageHandler CreateMessageHandler()
        {
            return new PolicyHandler
            {
                InnerHandler = base.CreateMessageHandler()
            };
        }
    }

    public class PolicyHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Policies
                .RetryPolicy
                .ExecuteAsync(ct => base.SendAsync(request, ct), cancellationToken);
        }
    }
}