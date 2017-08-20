using System;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AspNetCore2
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await StartWebAppAsync();
        }

        private static async Task StartWebAppAsync()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<MySingleton>().AsSelf().SingleInstance();
            // builder configuration ...

            using (var container = builder.Build())
            {
                StartOtherStuffAsync(container);

                while (true)
                {
                    await StartWebServerAsync(container);
                    DoCleanUp(container);
                }
            }
        }

        private static async Task StartWebServerAsync(ILifetimeScope scope)
        {
            Console.WriteLine(" ==> Creating Web Host Scope");

            using (var webHostScope = scope.BeginLifetimeScope(builder =>
            {
                builder.RegisterType<Startup>().AsSelf();
                builder.RegisterType<MyComponent>().AsSelf().SingleInstance();
            }))
            {
                var component = webHostScope.Resolve<MyComponent>();

                await WebHost
                    .CreateDefaultBuilder()
                    .UseStartup<Startup>()
                    .ConfigureLogging(builder => builder.SetMinimumLevel(LogLevel.Warning))
                    .ConfigureServices(services => services.AddTransient(provider =>
                    {
                        var hostingEnv = provider.GetRequiredService<IHostingEnvironment>();
                        var config = provider.GetRequiredService<IConfiguration>();
                        var factory = webHostScope.Resolve<Func<IHostingEnvironment, IConfiguration, Startup>>();

                        return factory(hostingEnv, config);
                    }))
                    .Build()
                    .RunAsync();

                Console.WriteLine($" <== {DateTime.Now} Web Host Scope disposing");
            }
            Console.WriteLine($" <== {DateTime.Now} Web Host Scope disposed");
        }

        private static void StartOtherStuffAsync(ILifetimeScope scope)
        {
            using (var otherStuffScope = scope.BeginLifetimeScope(builder =>
            {
                /* register dependencies */
            }))
            {
                // do stuff
            }
        }

        private static void DoCleanUp(ILifetimeScope container)
        {
            // clean up
        }

    }
}
