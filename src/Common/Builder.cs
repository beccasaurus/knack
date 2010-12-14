using System;
using System.Collections.Generic;

namespace Owin {

	// We might want Builder to actually be an IApplication and then we could:
	//     app = new Builder(middleware, middleware ...)
	public class Builder {

		List<IMiddleware> Middlewares = new List<IMiddleware>();
		IApplication      Application;

		public Builder() {}

		public Builder(IApplication application) {
			Application = application;
		}

		public Builder(IApplication application, params IMiddleware[] middlewares) {
			Application = application;
			Middlewares.AddRange(middlewares);
		}

		// When you Use() middleware, you want the LAST middleware to be CLOSEST to the application
		//
		//  new Builder().
		//  	Use(OUTER).
		//  	Use(INNER).
		//  	Run(APP).ToApp()
		//
		// We store the middleware stack with the innermost first.
		//
		// So when you Use() a middleware, instead of adding it to the end, we "unshift" it to the top
		//
		public Builder Use(params IMiddleware[] middlewares) {
			foreach (IMiddleware middleware in middlewares)
				Middlewares.Insert(0, middleware);
			return this;
		}

		public Builder Run(IApplication application) {
			Application = application;
			return this;
		}

		public IApplication ToApp() {
			return ToApplication();
		}

		public IApplication ToApplication() {
			if (Middlewares.Count == 0)
				return Application;
			else {
				IApplication app = Application;
				foreach (IMiddleware middleware in Middlewares) {
					middleware.InnerApplication = app;
					app = middleware;
				}
				return app;
			}
		}

		public static IApplication Build(IApplication theApp, params IMiddleware[] middlewares) {
			return new Builder(theApp, middlewares).ToApplication();
		}
	}
}
