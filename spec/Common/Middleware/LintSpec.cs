using System;
using Owin;
using Owin.Test;
using NUnit.Framework;

namespace Owin.Common.Middleware.Specs {

	[TestFixture]
	public class LintSpec : MockSession {

		class LintApp : Application, IApplication {
			public override IResponse Call(IRequest request) {
			    Response response = new Response();

			    switch (request.Uri) {
				case "/badResponse":
				    response.SetBody(5); // add unkown object type into body
				    break;
				default:
				    response.Write("You requested {0} {1}", request.Method, request.Uri);
				    break;
			    }

			    return response;
			}
		}

		[SetUp]
		public void Before() {
		    App = new Builder().Use(new Lint(), new ShowExceptions()).Run(new LintApp()).ToApp();
		}

		[Test]
		public void Validates_incoming_Request() {
		    IRequest goodRequest = new RequestWriter("GET", "/");
		    GetResponse(goodRequest);
		    Assert.That(LastResponse.BodyText, Is.EqualTo("You requested GET /"));
		    Assert.That(LastResponse.BodyText, Is.Not.StringContaining("LintException"));

		    IRequest badRequest = new RequestWriter().SetMethod(""); // blank Method
		    GetResponse(badRequest);
		    Assert.That(LastResponse.BodyText, Is.StringContaining("Owin.LintException: Request was not valid: Method cannot be blank"));
		}

		[Test]
		public void Validates_outgoing_Response() {
		    Get("/");
		    Assert.That(LastResponse.BodyText, Is.EqualTo("You requested GET /"));
		    Assert.That(LastResponse.BodyText, Is.Not.StringContaining("LintException"));

		    Get("/badResponse");
		    Assert.That(LastResponse.BodyText, Is.StringContaining("Owin.LintException: Response was not valid: GetBody() has unsupported type: Int32"));
		}
	}
}
