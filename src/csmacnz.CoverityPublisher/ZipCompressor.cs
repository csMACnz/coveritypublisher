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
            if (!File.Exists(Path.Combine(payload.Directory, "BUILD.metrics.xml")))
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
            XDocument doc = XDocument.Load(Path.Combine(directory, "BUILD.metrics.xml"));
            var failureCount = doc.Root.Element(@"metrics")
                .Elements(@"metric")
                .Where(e => e.Element(@"name").Value == "failures")
                .Select(e => e.Element(@"value").Value)
                .FirstOrDefault();
            
            return int.Parse(failureCount) > 0;
        }
    }
}