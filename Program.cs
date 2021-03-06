using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace ProgLibrary.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                  .WriteTo.Console()
                  .WriteTo.File("logs/log.txt", restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                  rollingInterval: RollingInterval.Day)
                  .WriteTo.File("logs/errorlog.txt", restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning)
                  .CreateLogger();
            try
            {
                Log.Information("Staring Api");
                CreateHostBuilder(args)
                    .Build()
                    .Run();
            }
            catch (System.Exception ex)
            {

                Log.Fatal(ex, "Exception in application");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls("https://localhost:5000", "https://localhost:44336");
                    webBuilder.UseStartup<Startup>();
                    webBuilder.ConfigureKestrel(options =>
                    {                       
                    });
                });
       
    }
}
