using System;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AspNetCore2_Autofac460
{
	class Program
	{
		static async Task Main(string[] args)
		{
			var builder = new ContainerBuilder();

			using (var container = builder.Build())
			{
				await StartWebServerAsync(container);
			}
		}

		private static async Task StartWebServerAsync(ILifetimeScope scope)
		{
			using (var webHostScope = scope.BeginLifetimeScope(builder => builder.RegisterType<Startup>().AsSelf()))
			{
				await WebHost
					.CreateDefaultBuilder()
					.UseStartup<Startup>()
					.ConfigureServices(services => services.AddTransient(provider => webHostScope.Resolve<Startup>()))
					.Build()
					.RunAsync();
			}
		}
	}
}
