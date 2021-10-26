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
                    webBuilder.UseUrls("https://localhost:5000", "https://localhost:44336");
                    webBuilder.UseStartup<Startup>();
                    webBuilder.ConfigureKestrel(options =>
                    {

                        options.ConfigureHttpsDefaults(option => {
                            //option.SslProtocols = System.Security.Authentication.SslProtocols.Tls12 | System.Security.Authentication.SslProtocols.Tls;
                            //option.AllowAnyClientCertificate();


                        });
                        options.ConfigureEndpointDefaults(option =>
                        {
                            //option.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2;
                            //option.UseHttps();
                        });
                        
                    });
                });
       
    }
}
