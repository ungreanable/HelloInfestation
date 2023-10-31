using HelloInfestation.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace HelloInfestation
{
    internal class Program
    {
        public static Task Main(string[] args)
        {
            using var host = CreateHostBuilder(args).Build();

            Task task = RunAsync(host.Services);

            return task;
        }

        public static async Task RunAsync(IServiceProvider services)
        {
            using var serviceScope = services.CreateScope();
            var provider = serviceScope.ServiceProvider;
            var fileService = provider.GetRequiredService<IFileService>();
            IDictionary<string, string> requiredFilesDict = new Dictionary<string, string>()
            {
                { "FacMH.dll", "https://drive.google.com/uc?export=download&id=1OvdkjJb2DyXzxA6HUnYbV15twF6AvVxI" },
                { "version.dll", @"File\version.dll" },
            };
            bool exited = false;
            do
            {
                Console.Write("Choose your choice:\n\t1: Enable\n\t2: Disable\n\t3: Exit\nType: ");
                string? choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        foreach(var file in requiredFilesDict)
                        {
                            Console.WriteLine($"Downloading: {file.Key}");
                            if (file.Key == "FacMH.dll")
                            {
                                await fileService.DownloadFile(file.Value, "C:\\Windows\\System\\" + file.Key);
                            }
                            else if(file.Key == "version.dll")
                            {
                                var infestationPath = RegistryHelper.RegRead("SOFTWARE\\Topfair Development\\Infestation SEA", "Path");
                                if (!string.IsNullOrEmpty(infestationPath))
                                {
                                    fileService.CopyFile(file.Value, infestationPath + file.Key);
                                }
                                else
                                    Console.WriteLine("Cannot find path of Infestation SEA Installed");
                            }
                        };
                        break;
                    case "2":
                        foreach (var file in requiredFilesDict)
                        {
                            Console.WriteLine($"Deleting: {file.Key}");
                            if (file.Key == "FacMH.dll")
                            {
                                fileService.DeleteFile("C:\\Windows\\System\\" + file.Key);
                            }
                            else if (file.Key == "version.dll")
                            {
                                var infestationPath = RegistryHelper.RegRead("SOFTWARE\\Topfair Development\\Infestation SEA", "Path");
                                fileService.DeleteFile(infestationPath + file.Key);
                            }
                        };
                        break;
                    case "3":
                        exited = true;
                        break;
                    default:
                        exited = true;
                        break;
                }
                Console.WriteLine("Press any key to continue");
                Console.ReadKey();
                Console.Clear();
            }
            while (!exited);
            Environment.Exit(0);
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((_, services) =>
                    services.AddSingleton<IFileService, FileService>()
                    .AddHttpClient()
                    .RemoveAll<IHttpMessageHandlerBuilderFilter>()
                    .AddLogging(c => c.ClearProviders()));
        }
    }
}