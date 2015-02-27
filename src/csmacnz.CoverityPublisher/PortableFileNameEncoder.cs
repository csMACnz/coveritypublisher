using System.Text;

namespace csmacnz.CoverityPublisher
{
    public class PortableFileNameEncoder : UTF8Encoding
    {
        public override byte[] GetBytes(string entry)
        {
            return base.GetBytes(entry.Replace("\\", "/"));
        }
    }
}
