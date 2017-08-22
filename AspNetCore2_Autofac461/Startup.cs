using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore2_Autofac461
{
	public class Startup
	{
		private readonly ILifetimeScope _webHostScope;
		private ILifetimeScope _aspNetScope;

		public Startup(ILifetimeScope webHostScope)
		{
			_webHostScope = webHostScope ?? throw new ArgumentNullException(nameof(webHostScope));
		}

		public IServiceProvider ConfigureServices(IServiceCollection services)
		{
			services.AddMvc();

			// just works with Autofac 4.6.1
			_aspNetScope = _webHostScope.BeginLifetimeScope(builder => builder.Populate(services));

			return new AutofacServiceProvider(_aspNetScope);
		}

		public void Configure(IApplicationBuilder app, IApplicationLifetime appLifetime)
		{
			app.UseMvc();

			appLifetime.ApplicationStopped.Register(() => _aspNetScope.Dispose());
		}
	}
}
