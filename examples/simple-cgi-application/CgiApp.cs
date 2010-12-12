using System;
using System.IO;
using Owin;

public class CgiApp : Application, IApplication {
	public override IResponse Call(IRequest rawRequest) {
	    Request request = new Request(rawRequest);

	    if (request.Uri == "")
		return new Response().Redirect(request.BasePath + "/");

	    string htmlFileName  = (request.Uri == "/") ? "/index.html" : request.Uri + ".html";
	    string viewDirectory = Path.Combine(Directory.GetCurrentDirectory(), "views");
	    htmlFileName         = Path.Combine(viewDirectory, htmlFileName.Replace("/", ""));

	    if (File.Exists(htmlFileName)) {
		string html = null;
		using (StreamReader reader = new StreamReader(htmlFileName))
		    html = reader.ReadToEnd();
		return new Response(html);
	    } else if (request.IsPost) {
		return new Response().Write("You posted name: {0} and breed: {1}", request["name"], request["breed"]);
	    } else {
		return new Response().SetStatus(404).SetBody("File not found: " + htmlFileName);
	    }
	}
}

public class Program {
	public static void Main(string[] args) {
		Owin.Handlers.Cgi.Run(new CgiApp());
	}
}
