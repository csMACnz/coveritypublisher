using System;
using System.IO;
using System.Net.Http;
using System.Reflection;

namespace csmacnz.CoverityPublisher
{
    public class Program
    {
        public static void Main(string[] argv)
        {
            var args = new MainArgs(argv, exit: true, version: Assembly.GetEntryAssembly().GetName().Version);

            string coverityFileName = args.OptZip;
            string repoName = args.OptToken;
            string coverityToken = args.OptToken;
            string description = args.OptDescription;
            string email = args.OptEmail;
            string version = args.OptCodeversion;

            var client = new HttpClient();
            client.Timeout = TimeSpan.FromMinutes(20);
            var form = new MultipartFormDataContent();
            form.Add(new StringContent(coverityToken), "token");
            form.Add(new StringContent(email), "email");

            var fs = new FileStream(coverityFileName, FileMode.Open, FileAccess.Read);
            var fileField = new StreamContent(fs);
            form.Add(fileField, "file", coverityFileName);
            form.Add(new StringContent(version), "version");
            form.Add(new StringContent(description), "description");

            var url = string.Format("https://scan.coverity.com/builds?project={0}", repoName);
            var task = client.PostAsync(url, form);
            try
            {
                task.Wait();
            }
            catch (AggregateException exception)
            {
                throw exception.InnerException;
            }
            Console.WriteLine(task.Result);
            fs.Close();
        }
    }
}
