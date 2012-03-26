using System;
using Nancy.Hosting.Self;

namespace Nancy.OData.Test
{
    public class Class1
    {
        public static void Main()
        {
            var host = new NancyHost(new Uri("http://localhost:5150"));
            host.Start();
            Console.ReadLine();
            host.Stop();
        }
    }
}
