using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AspNetCore2
{
    class Program
    {
        static void Main(string[] args)
        {
            StartWebAppAsync().GetAwaiter().GetResult();
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

        private static Task StartWebServerAsync(ILifetimeScope scope)
        {
            Console.WriteLine(" ==> Creating Web Host Scope");
            var webHostScope = scope.BeginLifetimeScope(builder => builder.RegisterType<Startup>().AsSelf());

            return WebHost
                .CreateDefaultBuilder()
                .UseStartup<Startup>()
                .ConfigureLogging(builder => builder.SetMinimumLevel(LogLevel.Warning))
                .ConfigureServices(services => services.AddTransient(provider => webHostScope.Resolve<Startup>()))
                .Build()
                .RunAsync()
                .ContinueWith(t =>
                {
                    Console.WriteLine(" <== Disposing of Web Host Scope");
                    webHostScope.Dispose();
                });
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
