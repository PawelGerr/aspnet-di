using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore2.Controllers
{
    [Route("/")]
    public class DemoController : Controller
    {
        private readonly MySingleton _mySingleton;
        private readonly IApplicationLifetime _appLifetime;

        public DemoController(IApplicationLifetime appLifetime, MySingleton mySingleton)
        {
            _mySingleton = mySingleton ?? throw new ArgumentNullException(nameof(mySingleton));
            _appLifetime = appLifetime ?? throw new ArgumentNullException(nameof(appLifetime));
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(_mySingleton);
        }

        [HttpPost]
        public IActionResult Restart()
        {
            RestartDelayed();
            return RedirectToAction("Index");
        }
        
        private async void RestartDelayed()
        {
            await Task.Delay(300);
            _appLifetime.StopApplication();
        }
    }
}
