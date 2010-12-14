using System;
using System.Collections.Generic;
using System.Text;
using Owin;
using Owin.Test;
using NUnit.Framework;

namespace Owin.Test.Specs {

	[TestFixture]
	public class ManuallyTestingSpec {

		class App : Application, IApplication {
			public override IResponse Call(IRequest request) {
				return new Response().Write("You requested {0} {1}", request.Method, request.Uri);
			}
		}

		[Test]
		public void Can_manually_make_a_MockRequest_and_get_a_MockResponse() {
			MockRequest request   = new MockRequest("GET", "/dogs");
			IApplication app      = new App();
			MockResponse response = request.GetResponse(app); // a MockRequest can get a MockResponse, given an IApplication
			Assert.That(response.BodyText, Is.EqualTo("You requested GET /dogs"));

			response = new MockRequest("POST", "/foo/bar").GetResponse(app);
			Assert.That(response.BodyText, Is.EqualTo("You requested POST /foo/bar"));
		}
	}
}
