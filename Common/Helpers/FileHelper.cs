using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace GlumOrigins.Common.Helpers
{
    public class FileHelper
    {
        public static async Task WriteTextAsync(string filePath, string text)
        {
            byte[] encodedText = Encoding.Unicode.GetBytes(text);

            using (FileStream sourceStream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.None, 4096, true))
            {
                await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
            }
        }

        public static async Task<string> ReadTextAsync(string filePath)
        {
            using (FileStream sourceStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
            {
                StringBuilder stringBuilder = new StringBuilder();

                byte[] buffer = new byte[0x1000];

                int count;
                while ((count = await sourceStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    string text = Encoding.Unicode.GetString(buffer, 0, count);
                    stringBuilder.Append(text);
                }

                return stringBuilder.ToString();
            }
        }
    }
}
