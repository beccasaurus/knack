using System;
using Owin;

namespace Owin.Handlers {

    public class Cgi {

        public static void Run(IApplication application) {
            Console.WriteLine("Status: HTTP/1.1 200 OK");
            Console.WriteLine("");
            Console.WriteLine("Hello world");
        }
    }
}