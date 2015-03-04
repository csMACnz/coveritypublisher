using System;
using System.IO.Compression;

namespace csmacnz.CoverityPublisher
{
    public static class ZipCompressor
    {
        public static ActionResult Compress(CompressPayload payload)
        {
            try
            {
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
                    Message = ex.Message
                };
            }
            return new ActionResult
            {
                Successful=true,
                Message="Compression completed successfully."
            };
        }
    }
}