using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.Utils
{
    public static class FileUtils
    {
        public static async Task<string> ReadAllTextAsync(string filePath)
        {
            try
            {
                var stringBuilder = new StringBuilder();
                FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                await using (fileStream)
                using (var streamReader = new StreamReader(fileStream))
                {
                    string? line = await streamReader.ReadLineAsync();
                    while (line != null)
                    {
                        stringBuilder.AppendLine(line);
                        line = await streamReader.ReadLineAsync();
                    }

                    return stringBuilder.ToString();
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return string.Empty;
        }


        public static async Task<bool> WriteAllTextAsync(string strContent, string filePath)
        {
            try
            {
                FileStream fileStream = File.Create(filePath);
                await using (fileStream)
                await using (var streamReader = new StreamWriter(fileStream))
                {
                    await streamReader.WriteAsync(strContent);
                    return true;
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return false;
        }
    }
}