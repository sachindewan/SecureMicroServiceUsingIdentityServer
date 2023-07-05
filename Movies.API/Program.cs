using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Movies.API.Data;

namespace Movies.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            SeedDataBase(host);
            host.Run();
        }

        private static void SeedDataBase(IHost host)
        {
            using var scope = host.Services.CreateScope();
            var provider = scope.ServiceProvider;
            var services = provider.GetRequiredService<MoviesAPIContext>();
            MoviesContextSeed.SeedAsync(services);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
