using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ProgLibrary.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls("http://localhost:5000", "http://localhost:44336");
                    webBuilder.UseStartup<Startup>();
                    webBuilder.ConfigureKestrel(options =>
                    {
                       
                        options.ConfigureHttpsDefaults(option => {
                            option.SslProtocols = System.Security.Authentication.SslProtocols.Tls13;
                        }
                        );
                    });
                });
       
    }
}
