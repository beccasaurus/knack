using System;
using System.Web;
using Owin;
using Owin.Handlers;

namespace Owin {

	// This is an IApplication *and* an IHttpHandler, so you can use it with ASP.NET applications
	//
	// We plan on implementing the Async HttpHandler, which fits really well with the IApplication interface
	//
	public class AspNetApplication : Application, IApplication, IMiddleware, IHttpHandler {

		public void ProcessRequest(HttpContext context) {
			IRequest  request  = new AspNetRequest(context.Request);
			IResponse response = Invoke(request);
			AspNet.WriteResponse(context.Response, response);
		}

		public bool IsReusable {
			get { return true; }
		}
	}
}
