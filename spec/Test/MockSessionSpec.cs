using System;
using System.Collections.Generic;
using System.Text;
using Owin;
using Owin.Test;
using NUnit.Framework;

namespace Owin.Test.Specs {

	[TestFixture]
	public class MockSessionSpec {

		[TestFixture]
		public class InheritingFromMockSessionSpec : MockSession {

				[SetUp] public void Before() { App = new TestApp(); ResetSession(); }

				[Test]
				public void Can_inherit_from_MockSession_to_get_a_nice_DSL__get_example() {
					Assert.Null(LastResponse); Assert.Null(LastRequest); // both test should start with the LastResponse & LastRequest cleared out

					Get("/foo/bar");
					Assert.That(LastResponse.BodyText, Is.EqualTo("You requested GET /foo/bar"));
				}

				[Test]
				public void Can_inherit_from_MockSession_to_get_a_nice_DSL__post_example() {
					Assert.Null(LastResponse); Assert.Null(LastRequest); // both test should start with the LastResponse & LastRequest cleared out

					Post("/foo/bar", "name=Lander");
					Assert.That(LastResponse.BodyText, Is.EqualTo("You requested POST /foo/bar POSTed Name: Lander"));
				}
		}

		class TestApp : Application, IApplication {
			public override IResponse Call(IRequest rawRequest) {
				Request  request  = new Request(rawRequest);
				Response response = new Response();

				response.Write("You requested {0} {1}", request.Method, request.Uri);

				if (! request.IsGet)
					response.Write(" {0}ed Name: {1}", request.Method, request["name"]);

				return response;
			}
		}

		MockSession session;

	[SetUp]
		public void Before() { session = new MockSession(new TestApp()); }

		[Test]
		public void Can_manually_request_an_IRequest() {
			IRequest request      = new RequestWriter("GET", "/foo");
			MockResponse response = session.GetResponse(request);
			Assert.That(response.BodyText, Is.EqualTo("You requested GET /foo"));
		}

		[Test]
		public void Can_get_LastRequest() {
			Assert.Null(session.LastRequest);

			IRequest request = new RequestWriter("GET", "/bar");
			session.GetResponse(request);

			Assert.NotNull(session.LastRequest);
			Assert.That(session.LastRequest.Method, Is.EqualTo("GET"));
			Assert.That(session.LastRequest.Uri,    Is.EqualTo("/bar"));
		}

		[Test]
		public void Can_get_LastResponse() {
			Assert.Null(session.LastResponse);

			session.GetResponse(new RequestWriter("GET", "/hi"));

			Assert.NotNull(session.LastResponse);
			Assert.That(session.LastResponse.BodyText, Is.EqualTo("You requested GET /hi"));
		}

		[Test]
		public void Can_easily_do_GET_requests() {
			session.Get("/foo/bar?hi=there");
			Assert.That(session.LastResponse.BodyText, Is.EqualTo("You requested GET /foo/bar?hi=there"));
		}

		[Test]
		public void Can_easily_do_POST_requests() {
			session.Post("/post-stuff");
			Assert.That(session.LastResponse.BodyText, Is.EqualTo("You requested POST /post-stuff POSTed Name: "));

			session.Post("/post-stuff", "name=Rover");
			Assert.That(session.LastResponse.BodyText, Is.EqualTo("You requested POST /post-stuff POSTed Name: Rover"));

			session.Post("/post-stuff", new Dictionary<string,string>{{"name","Snoopy"}});
			Assert.That(session.LastResponse.BodyText, Is.EqualTo("You requested POST /post-stuff POSTed Name: Snoopy"));
		}

		[Test]
		public void Can_easily_do_PUT_requests() {
			session.Put("/post-stuff");
			Assert.That(session.LastResponse.BodyText, Is.EqualTo("You requested PUT /post-stuff PUTed Name: "));

			session.Put("/post-stuff", "name=Rover");
			Assert.That(session.LastResponse.BodyText, Is.EqualTo("You requested PUT /post-stuff PUTed Name: Rover"));

			session.Put("/post-stuff", new Dictionary<string,string>{{"name","Snoopy"}});
			Assert.That(session.LastResponse.BodyText, Is.EqualTo("You requested PUT /post-stuff PUTed Name: Snoopy"));
		}

		[Test]
		public void Can_easily_do_DELETE_requests() {
			session.Delete("/post-stuff");
			Assert.That(session.LastResponse.BodyText, Is.EqualTo("You requested DELETE /post-stuff DELETEed Name: "));

			session.Delete("/post-stuff", "name=Rover");
			Assert.That(session.LastResponse.BodyText, Is.EqualTo("You requested DELETE /post-stuff DELETEed Name: Rover"));

			session.Delete("/post-stuff", new Dictionary<string,string>{{"name","Snoopy"}});
			Assert.That(session.LastResponse.BodyText, Is.EqualTo("You requested DELETE /post-stuff DELETEed Name: Snoopy"));
		}
	}
}
