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

namespace AspNetCore2_Autofac460
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

			// doesn't work with Autofac 4.6.0 and child scopes.
			// An exception is thrown on startup: Cannot resolve parameter 'IOptionsFactory<KestrelServerOptions>'
			_aspNetScope = _webHostScope.BeginLifetimeScope(builder => builder.Populate(services));

			// Workaround: all singleton are "rebased" to "aspNetScopeScopeTag"
			//const string aspNetScopeScopeTag = "AspNetScope";
			//_aspNetScope = _webHostScope.BeginLifetimeScope(aspNetScopeScopeTag, builder => builder.Populate(services, aspNetScopeScopeTag));

			return new AutofacServiceProvider(_aspNetScope);
		}

		public void Configure(IApplicationBuilder app, IApplicationLifetime appLifetime)
		{
			app.UseMvc();

			appLifetime.ApplicationStopped.Register(() => _aspNetScope.Dispose());
		}
	}
}
