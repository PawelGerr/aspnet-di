using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
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

		[HttpGet("CancelOnServerShutdown")]
		public async Task<IActionResult> CancelOnServerShutdown(TimeSpan time)
		{
			return await DelayExecution(time, _appLifetime.ApplicationStopping);
		}

		[HttpGet("CancelOnRequestAborted")]
		public async Task<IActionResult> CancelOnRequestAborted(TimeSpan time)
		{
			return await DelayExecution(time, HttpContext.RequestAborted);
		}

		[HttpGet("CancelByCancellationToken")]
		public async Task<IActionResult> CancelByCancellationToken(TimeSpan time, CancellationToken cancellationToken)
		{
			return await DelayExecution(time, cancellationToken);
		}

		private async Task<IActionResult> DelayExecution(TimeSpan time, CancellationToken cancellationToken)
		{
			var sw = Stopwatch.StartNew();
			try
			{
				await Task.Delay(time, cancellationToken);
			}
			catch (OperationCanceledException)
			{
				Console.WriteLine($@"Operation cancelled after {sw.Elapsed}.
    ApplicationStopping = {_appLifetime.ApplicationStopping.IsCancellationRequested}
    RequestAborted= {HttpContext.RequestAborted.IsCancellationRequested}
");
			}

			return Ok($"Delayed execution for {sw.Elapsed}");
		}

		private async void RestartDelayed()
		{
			_appLifetime.ApplicationStopping.Register(() =>
			{
				Console.WriteLine($" <== {DateTime.Now} Disposing {nameof(DemoController)}");
				Thread.Sleep(TimeSpan.FromSeconds(2)); // delaying the server shutdown
				Console.WriteLine($" <== {DateTime.Now} Disposed {nameof(DemoController)}");
			});

			await Task.Delay(300);
			_appLifetime.StopApplication();
		}

	}
}
