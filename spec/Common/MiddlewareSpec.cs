using System;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Collections.Specialized;
using Owin;
using NUnit.Framework;

// TODO write example to make sure that both the constructor and Use/Run syntax make it visually obvious
//      where the middleware are placed in the 'stack'
//
//      new Builder(app, innermost, next, outtermost)
//      Builder().Use(outermost).Use(next).Use(innermost).Run(app)  <--- innermost should always be "next to" the app
//
namespace Owin.Common.Specs {

    [TestFixture]
    public class MiddlewareSpec {

        class TheApp : Application, IApplication {
            public override IResponse Call(IRequest request) {
                return new Response("Hello from the app");
            }
        }

	class MiddlewareThatWrapsBody : Application, IApplication, IMiddleware {
	    public MiddlewareThatWrapsBody(IApplication app) : base(app) {}

	    public MiddlewareThatWrapsBody(string prefixAndSuffix) : this(prefixAndSuffix + ":", ":" + prefixAndSuffix) {}

	    public MiddlewareThatWrapsBody(string prefix, string suffix) {
		Prefix = prefix;
		Suffix = suffix;
	    }

	    public string Prefix = "WRAPPED ";
	    public string Suffix = " By Middleware!";

            public override IResponse Call(IRequest request) {
		Response response = new Response(Application.Invoke(InnerApplication, request));
		response.BodyText = Prefix + response.BodyText + Suffix;
		return response;
	    }
	}

	[Test]
	public void Middleware_can_wrap_the_response_of_an_application() {
	    IApplication appWithoutMiddleware = new TheApp();
	    Assert.That(Application.GetResponse(appWithoutMiddleware, new Request()).BodyText, Is.EqualTo("Hello from the app"));

	    IApplication appWithMiddleware = new MiddlewareThatWrapsBody(new TheApp());
	    Assert.That(Application.GetResponse(appWithMiddleware, new Request()).BodyText, Is.Not.EqualTo("Hello from the app"));
	    Assert.That(Application.GetResponse(appWithMiddleware, new Request()).BodyText, Is.EqualTo("WRAPPED Hello from the app By Middleware!"));

	    // another, passing args to constructor because we'll probably do something like this with Owin.Builder ...
	    IMiddleware anotherApp      = new MiddlewareThatWrapsBody("BEFORE:", ":AFTER");
	    anotherApp.InnerApplication = new TheApp();
	    Assert.That(Application.GetResponse(anotherApp, new Request()).BodyText, Is.EqualTo("BEFORE:Hello from the app:AFTER"));
	}

	[Test]
	public void Middleware_can_easily_be_configured_using_Owin_Builder_build_method () {
	    IApplication app = Builder.Build(
					    new TheApp(), 
					    new MiddlewareThatWrapsBody("innermost"),
					    new MiddlewareThatWrapsBody("middle"),
					    new MiddlewareThatWrapsBody("outermost") 
	    );
	    Response response = Application.GetResponse(app, new Request());
	    Assert.That(response.BodyText, Is.EqualTo("outermost:middle:innermost:Hello from the app:innermost:middle:outermost"));
	}

	[Test]
	public void Middleware_can_easily_be_configured_using_Owin_Builder_constructor() {
	    IApplication app = new Builder( new TheApp(), 
					    new MiddlewareThatWrapsBody("innermost*"),
					    new MiddlewareThatWrapsBody("middle*"),
					    new MiddlewareThatWrapsBody("outermost*")
	    ).ToApplication();
	    Response response = Application.GetResponse(app, new Request());
	    Assert.That(response.BodyText, Is.EqualTo("outermost*:middle*:innermost*:Hello from the app:innermost*:middle*:outermost*"));
	}

	[Test]
	public void Middleware_can_easily_be_configured_using_Owin_Builder_fluent_syntax() {
	    IApplication app = new Builder().
					    Use(new MiddlewareThatWrapsBody("InnerMost")).
					    Use(new MiddlewareThatWrapsBody("Middle")).
					    Use(new MiddlewareThatWrapsBody("OuterMost")).
					    Run(new TheApp()).
					    ToApplication();

	    Response response = Application.GetResponse(app, new Request());
	    Assert.That(response.BodyText, Is.EqualTo("OuterMost:Middle:InnerMost:Hello from the app:InnerMost:Middle:OuterMost"));
	}

    }
}
