using System;
using System.Collections.Generic;
using Owin;
using NUnit.Framework;

namespace Owin.Common.Specs {

	// *Simple* struct-like IRequest implementation for RequestSpec
	class Req : IRequest {
		public Req() {
		    Items = new Dictionary<string, object>();
		}
		public string Method { get; set; }
		public string Uri { get; set; }
		public IDictionary<string, IEnumerable<string>> Headers { get; set; }
		public IDictionary<string, object> Items { get; set; }
		public IAsyncResult BeginReadBody(byte[] buffer, int offset, int count, AsyncCallback callback, object state){ return null; }
		public int EndReadBody(IAsyncResult result){ return 0; }
	}

	[TestFixture]
	public class RequestSpec {

		// make spec for request validations?
		// Items must be set
		// required items must be present and have valid values

		Request req;

		// Helper method for getting a new Owin.Request, given an IRequest to instantiate it with
		Request R(IRequest innerRequest) {
		    return new Request(innerRequest);
		}
		
		[SetUp] public void Before() { req = null; }

		[Test][Ignore] public void Can_be_instantiated_with_an_IRequest() {
			Assert.That(R(new Req { Method = "GET"  }).Method, Is.EqualTo("GET"));
			Assert.That(R(new Req { Method = "POST" }).Method, Is.EqualTo("POST"));
			Assert.That(R(new Req { Uri    = "/foo" }).Uri,    Is.EqualTo("/foo"));
			Assert.That(R(new Req { Uri    = "/bar" }).Uri,    Is.EqualTo("/bar"));
		}

		[Test][Ignore] public void Can_get_the_raw_QueryString_from_Uri() {
		    Assert.That(R(new Req { Uri = "/"                 }).QueryString, Is.Null);
		    Assert.That(R(new Req { Uri = "/foo"              }).QueryString, Is.Null);
		    Assert.That(R(new Req { Uri = "/foo?a=5"          }).QueryString, Is.EqualTo("a=5"));
		    Assert.That(R(new Req { Uri = "/foo?a=5&hi=there" }).QueryString, Is.EqualTo("a=5&hi=there"));
		    Assert.That(R(new Req { Uri = "/?a=5&hi=there"    }).QueryString, Is.EqualTo("a=5&hi=there"));
		    Assert.That(R(new Req { Uri = "?a=5&hi=there"     }).QueryString, Is.EqualTo("a=5&hi=there"));
		}

		[Test][Ignore] public void Can_get_the_value_of_a_QueryString() {
		    Assert.That(R(new Req { Uri = "/"                 }).GET, Is.Empty);
		    Assert.That(R(new Req { Uri = "/foo"              }).GET, Is.Empty);

		    Assert.That(R(new Req { Uri = "/foo?a=5"          }).GET, Is.Not.Empty);
		    Assert.That(R(new Req { Uri = "/foo?a=5"          }).GET["a"],  Is.EqualTo("5"));
		    Assert.That(R(new Req { Uri = "/foo?a=5"          }).GET["hi"], Is.Null);

		    Assert.That(R(new Req { Uri = "/foo?a=5&hi=there" }).GET["a"],  Is.EqualTo("5"));
		    Assert.That(R(new Req { Uri = "/foo?a=5&hi=there" }).GET["hi"], Is.EqualTo("there"));
		}

		[Test][Ignore] public void Can_get_the_raw_QueryString_from_header() {}
		[Test][Ignore] public void Can_get_the_post_body() {}
		[Test][Ignore] public void Can_get_the_value_of_a_POST_variable() {}
		[Test][Ignore] public void Can_get_Params_from_either_a_QueryString_or_POST_variable() {}
		[Test][Ignore] public void Can_get_referer_or_referrer() {}
		[Test][Ignore] public void Can_get_host() {}
		[Test][Ignore] public void Can_get_port() {}
		[Test][Ignore] public void Can_get_scheme() {}
		[Test][Ignore] public void Has_predicate_properties_for_checking_request_method_type() {}
		[Test][Ignore] public void Can_get_IP() {}
		[Test][Ignore] public void Can_be_instantiated_with_no_arguments_to_build_a_new_request() {}
	}
}
