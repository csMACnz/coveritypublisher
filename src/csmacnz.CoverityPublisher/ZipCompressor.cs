using System;
using System.IO;
using System.IO.Compression;

namespace csmacnz.CoverityPublisher
{
    public static class ZipCompressor
    {
        public static ActionResult Compress(CompressPayload payload)
        {
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
    }
}