using System;
using System.IO;
using System.Net.Http;
using BCLExtensions;
using Newtonsoft.Json;

namespace csmacnz.CoverityPublisher
{
    public static class CoveritySubmitter
    {
        public static ActionResult Submit(PublishPayload payload)
        {
            using (var fs = new FileStream(payload.FileName, FileMode.Open, FileAccess.Read))
            {
                using (MultipartFormDataContent form = BuildForm(payload, fs))
                {
                    var url = new Uri("http://scan5.coverity.com/cgi-bin/upload.py");

                    ActionResult results = new ActionResult
                    {
                        Successful = true,
                    };
                    if (payload.SubmitToCoverity)
                    {
                        try
                        {
                            var response = Client.Post(url, form);
                            if (response.IsSuccessStatusCode)
                            {
                                results.Message = "Request Submitted Successfully";
                            }
                            else
                            {
                                results.Successful = false;
                                string content = response.Content.ReadAsStringAsync().Result;
                                string message = TryGetJsonMessageFromResponse(content);
                                if (message.IsNotNull())
                                {
                                    if (message.StartsWith("The build submission quota for this project has been reached."))
                                    {
                                        results.Successful = true;
                                        results.Message = message;
                                    }
                                    else
                                    {
                                        results.Message = "There was an error submitting your report: \n" +
                                                          response.ReasonPhrase + "; " + message;
                                    }
                                }
                                else
                                {
                                    results.Message = "There was an error submitting your report: \n" +
                                                          response.ReasonPhrase + "; " + content;
                                }
                            }
                        }
                        catch (AggregateException exception)
                        {
                            var ex = exception.InnerException;
                            results.Successful = false;
                            results.Message = "There was an error submitting your report: \n" + ex;
                        }
                    }
                    else
                    {
                        results.Message = "Dry run Successful";
                    }
                    return results;
                }
            }
        }

        private static MultipartFormDataContent BuildForm(PublishPayload payload, FileStream fs)
        {
            return new MultipartFormDataContent
                {
                    CreateStringContent("token", payload.Token),
                    CreateStringContent("email", payload.Email),
                    CreateFileContent(fs, "file", payload.FileName),
                    CreateStringContent("version", payload.Version),
                    CreateStringContent("description", payload.Description),
                    CreateStringContent("project", payload.RepositoryName)
                };
        }

        private static HttpContent CreateFileContent(FileStream stream, string name, string fileName)
        {
            var streamContent = new StreamContent(stream);
            streamContent.Headers.Add("Content-Type", "application/x-zip-compressed");
            streamContent.Headers.Add("Content-Disposition", "form-data; name=\"" + name + "\"; filename=\"" + fileName + "\"");
            return streamContent;
        }

        private static HttpContent CreateStringContent(string name, string value)
        {
            var stringContent = new StringContent(value);
            stringContent.Headers.Clear();
            stringContent.Headers.Add("Content-Disposition", "form-data; name=\"" + name + "\"");
            return stringContent;
        }

        private static string TryGetJsonMessageFromResponse(string content)
        {
            try
            {
                dynamic result = JsonConvert.DeserializeObject(content);
                return result.message;
            }
            catch
            {
                return null;
            }
        }

    }
}