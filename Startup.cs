using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore2
{
    public class Startup
    {
        private readonly ILifetimeScope _webHostScope;
        private ILifetimeScope _aspNetScope;

        public Startup(ILifetimeScope webHostScope, IHostingEnvironment hostingEnvironment, IConfiguration configuration)
        {
            _webHostScope = webHostScope ?? throw new ArgumentNullException(nameof(webHostScope));
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // register ASP.NET Core stuff
            services.AddMvc();

            Console.WriteLine(" ==> Creating ASP.NET Scope");
            _aspNetScope = _webHostScope.BeginLifetimeScope(builder => builder.Populate(services));

            return new AutofacServiceProvider(_aspNetScope);
        }

        public void Configure(IApplicationBuilder app, IApplicationLifetime appLifetime)
        {
            app.UseMvc();

            appLifetime.ApplicationStopped.Register(() =>
            {
                Console.WriteLine($" <== {DateTime.Now} ASP.NET Scope disposing");
                _aspNetScope.Dispose();
                Console.WriteLine($" <== {DateTime.Now} ASP.NET Scope disposed");
            });
        }
    }
}