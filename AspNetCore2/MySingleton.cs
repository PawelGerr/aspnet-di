using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore2
{
	public class MySingleton : IDisposable
	{
		public DateTime CreationDate { get; }

		public MySingleton()
		{
			CreationDate = DateTime.Now;
			Console.WriteLine($" ==> {nameof(MySingleton)} created");
		}

		public void Dispose()
		{
			Console.WriteLine($" <== {nameof(MySingleton)} has been disposed");
		}
	}
}
