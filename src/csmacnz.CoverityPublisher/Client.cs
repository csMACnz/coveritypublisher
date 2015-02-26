using System;
using System.Net.Http;

namespace csmacnz.CoverityPublisher
{
    public class Client
    {
        public static HttpResponseMessage Post(string url, MultipartFormDataContent form)
        {
            var client = new HttpClient {Timeout = TimeSpan.FromMinutes(20)};
            var task = client.PostAsync(url, form);
            task.Wait();
            return task.Result;
        }
    }
}