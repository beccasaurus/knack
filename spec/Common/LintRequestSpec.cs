using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Owin;
using NUnit.Framework;

namespace Owin.Common.Specs {

	// we will probably take this class and use it or parts of it for Owin.Request or, if we make it, Owin.Handlers.Request (?)
	public class RequestForLint : IRequest {
		public RequestForLint() {
			// Make the Request valid
			Method = "GET";
			Uri = "";
			Items = new Dictionary<string, object>();
			Items["owin.base_path"] = "";
			Items["owin.server_name"] = "localhost";
			Items["owin.server_port"] = 80;
			Items["owin.request_protocol"] = "HTTP/1.1";
			Items["owin.url_scheme"] = "http";
			Items["owin.remote_endpoint"] = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 80);
			Headers = new Dictionary<string, IEnumerable<string>>();
		}
		public string Method { get; set; }
		public string Uri { get; set; }
		public IDictionary<string, IEnumerable<string>> Headers { get; set; }
		public IDictionary<string, object> Items { get; set; }
		public IAsyncResult BeginReadBody(byte[] buffer, int offset, int count, AsyncCallback callback, object state) { return null; }
		public int EndReadBody(IAsyncResult result) { return 0; }
	}

	// TODO refactor all helper methods to use -request- and all examples to use it to simplify things
	[TestFixture]
	public class LintRequestSpec {

		public class Req : RequestForLint { }

		string[] ErrorMessages;
		IRequest request;

		[SetUp]
		public void BeforeEach() {
			request = new Req();
			ErrorMessages = null;
		}

		// "Meta" spec to make sure that a new Req() is always valid;
		[Test]
		public void __new_Req_is_valid() {
			AssertValid();
		}

		[Test]
		[ExpectedException(typeof(LintException), ExpectedMessage = "Request was not valid: Method cannot be null")]
		public void Can_raise_exception_if_validation_fails() {
			request = new Req { Method = null };
			Owin.Lint.Validate(request);
		}

		[Test]
		[ExpectedException(typeof(LintException), ExpectedMessage = "Request was not valid: Method cannot be null, Uri cannot be null")]
		public void Can_raise_exception_with_many_messages_if_validation_fails() {
			request = new Req { Method = null, Uri = null };
			Owin.Lint.Validate(request);
		}

		[Test]
		public void Method_cannot_be_null_or_blank() {
			request = new Req { Method = null };
			AssertErrorMessage("Method cannot be null");

			request = new Req { Method = " " };
			AssertErrorMessage("Method cannot be blank");

			request = new Req { Method = "GET" };
			AssertValid();
		}

		[Test]
		public void Uri_cannot_be_null() {
			request = new Req { Uri = null };
			AssertErrorMessage("Uri cannot be null");

			request = new Req { Uri = "" };
			AssertValid();
		}

		[Test]
		[Ignore]
		public void Items_cannot_be_null_or_empty() { }

		[Test]
		public void Items_owin_base_path_cannot_be_null() {
			AssertItemCannotBeNull("owin.base_path");
		}

		[Test]
		public void Items_owin_server_name_cannot_be_null_or_blank() {
			AssertItemCannotBeNullOrBlank("owin.server_name");
		}

		[Test]
		public void Items_owin_server_port_cannot_be_null_or_blank() {
			AssertItemCannotBeNullOrBlank("owin.server_port");
		}

		[Test]
		public void Items_owin_server_port_must_be_convertable_to_an_int() {
			request.Items["owin.server_port"] = "hi";
			AssertErrorMessage("Items[\"owin.server_port\"] must be an integer");

			request.Items["owin.server_port"] = "123hi";
			AssertErrorMessage("Items[\"owin.server_port\"] must be an integer");

			request.Items["owin.server_port"] = "123";
			AssertValid();

			request.Items["owin.server_port"] = 123;
			AssertValid();
		}

		[Test]
		public void Items_owin_request_protocol_cannot_be_null_or_blank() {
			AssertItemCannotBeNullOrBlank("owin.request_protocol");
		}

		[Test]
		public void Items_owin_request_protocol_must_be_valid() {
			request.Items["owin.request_protocol"] = "HTTP5";
			AssertErrorMessage("Items[\"owin.request_protocol\"] protocol is unknown: HTTP5.  Must be HTTP/1.0 or HTTP/1.1");

			request.Items["owin.request_protocol"] = "HTTP/1.0";
			AssertValid();

			request.Items["owin.request_protocol"] = "HTTP/1.1";
			AssertValid();
		}

		[Test]
		public void Items_owin_url_scheme_must_be_http_or_https() {
			request.Items["owin.url_scheme"] = "ftp";
			AssertErrorMessage("Items[\"owin.url_scheme\"] scheme is unknown: ftp.  Must be http or https");

			request.Items["owin.url_scheme"] = "http";
			AssertValid();

			request.Items["owin.url_scheme"] = "https";
			AssertValid();
		}

		[Test]
		public void Items_owin_remote_endpoint_cannot_be_null() {
			AssertItemCannotBeNull("owin.remote_endpoint");
		}

		[Test]
		public void Items_owin_remote_endpoint_must_be_a_IPEndPoint() {
			request.Items["owin.remote_endpoint"] = "127.0.0.1:80";
			AssertErrorMessage("Items[\"owin.remote_endpoint\"] must be a System.Net.IPEndPoint");

			request.Items["owin.remote_endpoint"] = IPAddress.Parse("127.0.0.1");
			AssertErrorMessage("Items[\"owin.remote_endpoint\"] must be a System.Net.IPEndPoint");

			request.Items["owin.remote_endpoint"] = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 80);
			AssertValid();
		}

		[Test]
		public void Headers_cannot_be_null() {
			request = new Req { Headers = null };
			AssertErrorMessage("Headers cannot be null");
		}

		[Test]
			public void Headers_keys_must_be_lowercase() {
				request.Headers["Content-Type"] = new string[] { "text/html" };
				AssertErrorMessage("Header keys must be lower-cased: Content-Type");

				request = new Req();
				request.Headers["content-type"] = new string[] { "text/html" };
				AssertValid();
			}

		[Test]
		public void Headers_keys_cannot_have_colon() {
			request.Headers["content-type:"] = new string[] { "text/html" };
			AssertErrorMessage("Header keys cannot contain a colon: content-type:");
		}

		[Test]
		public void Headers_keys_cannot_have_whitespace() {
			request.Headers["content-type "] = new string[] { "text/html" };
			AssertErrorMessage("Header keys cannot contain whitespace: content-type ");
		}

		[Test]
		[Ignore]
		public void Items_owin_base_url_should_be_used_for_something() { }

		[Test]
		[Ignore]
		public void Items_owin_version_should_be_present() { } // This is not in the spec, but I think it would be smart to include it

		// Helper / Assertion methods

		void AssertItemCannotBeNull(string itemName) {
			request.Items.Remove(itemName);
			AssertErrorMessage("Items missing required key: " + itemName);

			request.Items[itemName] = null;
			AssertErrorMessage(string.Format("Items[\"{0}\"] cannot be null", itemName));
		}

		void AssertItemCannotBeNullOrBlank(string itemName) {
			AssertItemCannotBeNull(itemName);

			request.Items[itemName] = " ";
			AssertErrorMessage(string.Format("Items[\"{0}\"] cannot be blank", itemName));
		}

		void __validateRequest() {
			ErrorMessages = Owin.Lint.ErrorMessagesFor(request);
		}

		void AssertValid() {
			__validateRequest();
			Assert.That(new List<string>(ErrorMessages), Is.Empty);
		}

		void AssertErrorMessage(string message) {
			__validateRequest();
			Assert.That(ErrorMessages, Has.Member(message));
		}
	}
}
