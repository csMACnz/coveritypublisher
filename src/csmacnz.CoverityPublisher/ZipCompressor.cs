using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;
using BCLExtensions;

namespace csmacnz.CoverityPublisher
{
    public static class ZipCompressor
    {
        private static readonly string BuildMetricsXml = "BUILD.metrics.xml";

        public static ActionResult Compress(CompressPayload payload)
        {
            var validationResult = Validate(payload);
            if (!validationResult.Successful)
            {
                return validationResult;
            }
            if (payload.ProduceZipFile)
            {
                try
                {
                    if (File.Exists(payload.Output))
                    {
                        File.Delete(payload.Output);
                    }
                    ZipFile.CreateFromDirectory(
                        payload.Directory,
                        payload.Output,
                        CompressionLevel.Optimal,
                        true,
                        new PortableFileNameEncoder());
                }
                catch (Exception ex)
                {
                    return new ActionResult
                    {
                        Successful = false,
                        Message = "Compression Error: " + ex.Message
                    };
                }
            }
            return new ActionResult
            {
                Successful=true,
                Message="Compression completed successfully."
            };
        }

        private static ActionResult Validate(CompressPayload payload)
        {
            if (!Directory.Exists(payload.Directory))
            {
                return new ActionResult
                {
                    Successful = false,
                    Message = "Input folder '{0}' cannot be found.".FormatWith(payload.Directory)
                };
            }
            if (File.Exists(payload.Output))
            {
                if (payload.OverwriteExistingFile)
                {
                    //TODO: Handle Console as a Dependency
                    Console.WriteLine("Overwriting file '{0}' with new compression data.", payload.Output);
                }
                else
                {
                    return new ActionResult
                    {
                        Successful = false,
                        Message = "Output file '{0}' already exists.".FormatWith(payload.Output)
                    };
                }
            }
            if (!File.Exists(Path.Combine(payload.Directory, BuildMetricsXml)))
            {
                return new ActionResult
                {
                    Successful = false,
                    Message = "Input folder '{0}' is not recognised as Coverity Scan results.".FormatWith(payload.Directory)
                };
            }

            if (payload.AbortOnFailures && CoverityResultsHaveFailures(payload.Directory))
            {
                return new ActionResult
                {
                    Successful = false,
                    Message = "Input folder '{0}' has recorded failures.".FormatWith(payload.Directory),
                };
            }
            return new ActionResult {Successful = true};
        }

        private static bool CoverityResultsHaveFailures(string directory)
        {
            var buildMetricsFilePath = Path.Combine(directory, BuildMetricsXml);
            if (File.Exists(buildMetricsFilePath))
            {
                var failuresValue = GetFailureCountFromFile(buildMetricsFilePath);

                int failureCount;
                return int.TryParse(failuresValue, out failureCount) && failureCount > 0;
            }
            return false;
        }

        private static string GetFailureCountFromFile(string buildMetricsFilePath)
        {
            XDocument doc = XDocument.Load(buildMetricsFilePath);
            if (doc.Root != null)
            {
                var metrics = doc.Root.Element(@"metrics");
                if (metrics != null)
                {
                    var failuresElement = metrics
                        .Elements(@"metric")
                        .FirstOrDefault(e =>
                        {
                            var nameElement = e.Element(@"name");
                            return nameElement != null && nameElement.Value == "failures";
                        });

                    if (failuresElement != null)
                    {
                        var valueElement = failuresElement.Element(@"value");
                        if (valueElement != null)
                        {
                            return valueElement.Value;
                        }
                    }
                }
            }
            return null;
        }
    }
}