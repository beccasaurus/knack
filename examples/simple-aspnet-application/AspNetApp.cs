using System;
using System.Web;
using System.Collections.Generic;
using Owin;

namespace AspNetAppSample {

	// Wrap our real application with middleware, etc.
	//
	// We could normally do this with code, but we don't have a good way to execute any configuration 
	// code with AspNet applications.  Maybe if we made a Global.asax.cs?
	//
	public class OwinApplication : AspNetApplication, IHttpHandler {
	    static IApplication app = new Builder().Use(new ShowExceptions()).Run(new MyApp()).ToApp();

	    public override IResponse Call(IRequest request) {
		return Application.Invoke(app, request);
	    }
	}

	public class MyApp : Application, IApplication {
		public override IResponse Call(IRequest rawRequest) {
			Request  request  = new Request(rawRequest);
			Response response = new Response();

			if (request.Uri == "/boom") throw new Exception("BOOM!");
		
			response.ContentType = "text/plain";
			response.Write("{0} {1}\n\n", request.Method, request.Uri);
			
			foreach (KeyValuePair<string,string> param in request.Params)
				response.Write("Param {0} = {1}\n", param.Key, param.Value);
				
			foreach (KeyValuePair<string,IEnumerable<string>> header in request.Headers)
				foreach (string value in header.Value)
					response.Write("Header {0} = {1}\n", header.Key, value);
		
			return response;
		}
	}
}
