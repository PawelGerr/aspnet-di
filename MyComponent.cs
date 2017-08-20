using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore2
{
    public class MyComponent : IDisposable
    {
        public DateTime CreationDate { get; }

        public MyComponent()
        {
            CreationDate = DateTime.Now;
            Console.WriteLine($" ==> {nameof(MyComponent)} created");
        }

        public void Dispose()
        {
            Console.WriteLine($" <== {DateTime.Now} Disposing {nameof(MyComponent)}");
            Thread.Sleep(TimeSpan.FromSeconds(2));
            Console.WriteLine($" <== {DateTime.Now} Disposed {nameof(MyComponent)}");
        }
    }
}