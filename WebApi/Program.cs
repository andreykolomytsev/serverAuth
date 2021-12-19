using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Infrastructure.Identity.Contexts;
using Microsoft.EntityFrameworkCore;

namespace WebApi
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            //�������� ������������ �� appSettings
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            //�������������� Logger
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(config).CreateLogger();

            var host = CreateHostBuilder(args).Build();

            using var scope = host.Services.CreateScope();

            var services = scope.ServiceProvider;
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();

            try
            {
                var context = services.GetRequiredService<ApplicationDbContext>();

                context.Database.Migrate();

                await Infrastructure.Identity.Seeds.DefaultTenant.SeedAsync(context);
                await Infrastructure.Identity.Seeds.DefaultSuperAdmin.SeedAsync(context);
                Log.Information("������ ������� ���������");

                Log.Information("���������� ��������");

                host.Run();
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "��������� ������ ��� ������� ����������");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseSerilog() //Uses Serilog instead of default .NET Logger
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
