using System;
using Owin;

public class Program {
	public static void Main(string[] args) {
		Owin.Handlers.Kayak.Run(new Application(new ApplicationResponder(MyAppLogic)));
		
		// If you're using a newer version of C# with lambda support, this will work:
		//Owin.Handlers.Kayak.Run(new Application(req => new Response("Hi there!")));
	}
	
	public static IResponse MyAppLogic(IRequest rawRequest) {
		var request  = new Request(rawRequest);
		var response = new Response().SetHeader("content-type", "text/plain");
		var errors   = Owin.Lint.ErrorMessagesFor(rawRequest);
		
		response.Write("{0} {1}\n\n", request.Method, request.Uri);
		
		if (errors.Length == 0)
			response.Write("The IRequest is valid!");
		else
			foreach (string message in errors)
				response.Write("Error: {0}\n", message);
		
		return response;
	}
}