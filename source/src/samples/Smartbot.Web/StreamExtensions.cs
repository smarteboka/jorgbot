using System.IO;
using System.Threading.Tasks;

namespace Smartbot.Web
{
    public static class StreamExtensions
    {
        public static Task<string> ReadAsStringAsync(this Stream stream)
        {
            using (var mem = new MemoryStream())
            using (var reader = new StreamReader(mem))
            {
                stream.CopyToAsync(mem);
                mem.Seek(0, SeekOrigin.Begin);
                return reader.ReadToEndAsync();
            }
        }
    }
}