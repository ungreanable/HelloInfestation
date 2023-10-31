using System.Diagnostics;
using System.Net;
using System.Net.Http;

namespace HelloInfestation.Service
{
    public interface IFileService
    {
        Task DownloadFile(string fileUrl, string fileName);
        void CopyFile(string sourceFile, string destinationFile);
        void DeleteFile(string filePath);
    }
    public class FileService : IFileService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public FileService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task DownloadFile(string fileUrl, string fileName)
        {
            try
            {
                using var client = _httpClientFactory.CreateClient();
                using var s = await client.GetStreamAsync(fileUrl);
                if (File.Exists(fileName))
                {
                    using var fs = new FileStream(fileName, FileMode.Truncate, FileAccess.ReadWrite);
                    await s.CopyToAsync(fs);
                }
                else
                {
                    using var fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    await s.CopyToAsync(fs);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void CopyFile(string sourceFile, string destinationFile)
        {
            try
            {
                if (File.Exists(sourceFile))
                {
                    File.Copy(sourceFile, destinationFile, true);
                }
            }
            catch (IOException iox)
            {
                Console.WriteLine(iox.Message);
            }
        }

        public void DeleteFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (IOException iox)
            {
                Console.WriteLine(iox.Message);
            }
        }
    }
}
