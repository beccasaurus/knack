using System;
using Owin;
using Kayak;

namespace Owin.Handlers {

	public class Kayak {

		// TODO this should take a Port number
		public static void Run(IApplication application) {
			KayakServer server = new KayakServer();
			IDisposable pipe   = server.Invoke(application) as IDisposable;
			Console.WriteLine("KayakServer is running at http://localhost:8080/");
			Console.WriteLine("Press any key to exit");
			Console.Read();
			pipe.Dispose();
		}
	}
}
