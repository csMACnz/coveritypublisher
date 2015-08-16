using System;
using System.Net.Http;

namespace csmacnz.CoverityPublisher
{
    public static class Client
    {
        public static HttpResponseMessage Post(Uri url, MultipartFormDataContent form)
        {
            using (var client = new HttpClient {Timeout = TimeSpan.FromMinutes(20)})
            {
                var task = client.PostAsync(url, form);
                task.Wait();
                return task.Result;
            }
        }
    }
}