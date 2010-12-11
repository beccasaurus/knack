using System;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Collections.Specialized;
using Owin;
using NUnit.Framework;

namespace Owin.Common.Specs {

    [TestFixture]
    public class ApplicationSpec {

	// Note: .NET 3.5 and 4.0 should be able to easily pass a lambda to a new Application(req => return a response);

	[Test][Ignore("We need to be able to easily create Request objects before we can make mock requests to give an IApplication ...")]
	public void Can_create_a_new_application_using_an_external_method() {
	    //Application app = new Application(new ApplicationMethod(AppImplementation));
	    //Assert.That(app.GetResponse(new Request("/foo/bar")).Body, Is.EqualTo("You called us with Uri: /foo/bar"));
	}

	//public IResponse AppImplementation(IRequest request) {
	//    return new Response("You called us with Uri: " + request.Uri);
	//}

	[Test][Ignore]
	public void Can_make_an_application_class_easily_that_uses_an_internal_method() {}
    }
}
