using System;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Collections.Specialized;
using Owin;
using NUnit.Framework;

// TODO omg, we need to refactor this spec, it's bad!  they should be more like the Lint specs, each of them with a request.  they should also validate each request!
//
// This was the first spec we write for Owin.Common and we hadn't come up with any clean patterns yet.
//
// We'll clean this spec up asap ...
//
namespace Owin.Common.Specs {

    // *Simple* struct-like IRequest implementation for RequestSpec
    // we will probably take this class and use it or parts of it for Owin.Request or, if we make it, Owin.Handlers.Request (?)
    public class Req : IRequest {
        public delegate void DoWork();

        public Req() {
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
            ActuallyReadTheBody = new ReadTheBodyDelegate(ReadTheBody);
        }
        public string Method { get; set; }
        public string Uri { get; set; }
        public IDictionary<string, IEnumerable<string>> Headers { get; set; }
        public IDictionary<string, object> Items { get; set; }

        public string BodyString { get; set; }
        public byte[] BodyBytes {
            get { return Encoding.UTF8.GetBytes(BodyString); }
        }

        public delegate int ReadTheBodyDelegate(byte[] buffer, int offset, int count);
        ReadTheBodyDelegate ActuallyReadTheBody { get; set; }
        int ReadTheBody(byte[] buffer, int offset, int count) {
            int bytesRead = 0;
            for (int i = 0; i < count; i++) {
                int index = offset + i;
                if (index >= BodyBytes.Length)
                    break; // the BodyBytes doesn't have this index
                else {
                    bytesRead++;
                    buffer[i] = BodyBytes[index];
                }
            }
            return bytesRead;
        }

        public IAsyncResult BeginReadBody(byte[] buffer, int offset, int count, AsyncCallback callback, object state) {
            return ActuallyReadTheBody.BeginInvoke(buffer, offset, count, callback, state);
        }
        public int EndReadBody(IAsyncResult result) {
            return ActuallyReadTheBody.EndInvoke(result);
        }
    }

    [TestFixture]
    public class RequestSpec {

        Request req;

        // Helper method for getting a new Owin.Request, given an IRequest to instantiate it with
        Request R(IRequest innerRequest) {
            return new Request(innerRequest);
        }

        [SetUp]
        public void Before() { req = null; }

        [Test]
        public void Can_be_instantiated_with_an_IRequest() {
            Assert.That(R(new Req { Method = "GET" }).Method, Is.EqualTo("GET"));
            Assert.That(R(new Req { Method = "POST" }).Method, Is.EqualTo("POST"));
            Assert.That(R(new Req { Uri = "/foo" }).Uri, Is.EqualTo("/foo"));
            Assert.That(R(new Req { Uri = "/bar" }).Uri, Is.EqualTo("/bar"));
        }

        [Test]
        public void Can_get_the_full_Url() {
            Req request = new Req { Uri = "/foo/bar?hi=there" };
            request.Items["owin.base_path"] = "/my/app";
            request.Items["owin.server_name"] = "localhost";
            request.Items["owin.server_port"] = 80;
            request.Items["owin.url_scheme"] = "http";
            Assert.That(R(request).Url, Is.EqualTo("http://localhost/my/app/foo/bar?hi=there"));

            request.Items["owin.base_path"] = "/my/root";
            Assert.That(R(request).Url, Is.EqualTo("http://localhost/my/root/foo/bar?hi=there"));

            request.Uri = "/neat";
            Assert.That(R(request).Url, Is.EqualTo("http://localhost/my/root/neat"));

            request.Items["owin.server_name"] = "code.com";
            Assert.That(R(request).Url, Is.EqualTo("http://code.com/my/root/neat"));

            request.Items["owin.url_scheme"] = "https";
            request.Items["owin.server_port"] = 443;
            Assert.That(R(request).Url, Is.EqualTo("https://code.com/my/root/neat"));

            request.Items["owin.server_port"] = 123;
            Assert.That(R(request).Url, Is.EqualTo("https://code.com:123/my/root/neat"));

            request.Items["owin.url_scheme"] = "http";
            Assert.That(R(request).Url, Is.EqualTo("http://code.com:123/my/root/neat"));
        }

	[Test]
	public void Can_get_the_path_info_which_is_the_Uri_without_querystrings() {
	    Req request = new Req { Uri = "/foo/bar?hello=there" };
	    request.Items["owin.base_path"] = "/foo.cgi/";
	    Assert.That(R(request).PathInfo, Is.EqualTo("/foo/bar"));
	}

	[Test]
	public void Can_get_the_path_which_includes_the_base_name_and_path_without_querystrings( ){
	    Req request = new Req { Uri = "/foo/bar?hello=there" };
	    Assert.That(R(request).Path, Is.EqualTo("/foo/bar"));

	    // includes base path
	    request.Items["owin.base_path"] = "/foo.cgi";
	    Assert.That(R(request).Path, Is.EqualTo("/foo.cgi/foo/bar"));
	}

        [Test]
        public void Can_get_the_raw_QueryString_from_Uri() {
            Assert.That(R(new Req { Uri = "/" }).QueryString, Is.EqualTo(""));
            Assert.That(R(new Req { Uri = "/foo" }).QueryString, Is.EqualTo(""));
            Assert.That(R(new Req { Uri = "/foo?a=5" }).QueryString, Is.EqualTo("a=5"));
            Assert.That(R(new Req { Uri = "/foo?a=5&hi=there" }).QueryString, Is.EqualTo("a=5&hi=there"));
            Assert.That(R(new Req { Uri = "/?a=5&hi=there" }).QueryString, Is.EqualTo("a=5&hi=there"));
            Assert.That(R(new Req { Uri = "?a=5&hi=there" }).QueryString, Is.EqualTo("a=5&hi=there"));
        }

        [Test]
        public void Can_get_the_value_of_a_QueryString() {
            Assert.That(R(new Req { Uri = "/" }).GET, Is.Empty);
            Assert.That(R(new Req { Uri = "/foo" }).GET, Is.Empty);

            Assert.That(R(new Req { Uri = "/foo?a=5" }).GET, Is.Not.Empty);
            Assert.That(R(new Req { Uri = "/foo?a=5" }).GET["a"], Is.EqualTo("5"));
            Assert.That(R(new Req { Uri = "/foo?a=5" }).GET["a"], Is.EqualTo("5"));
            Assert.That(R(new Req { Uri = "/foo?a=5" }).GET["hi"], Is.Null);

            Assert.That(R(new Req { Uri = "/foo?a=5&hi=there" }).GET["a"], Is.EqualTo("5"));
            Assert.That(R(new Req { Uri = "/foo?a=5&hi=there" }).GET["hi"], Is.EqualTo("there"));
        }

        [Test]
        [Ignore]
        public void Can_get_the_raw_QueryString_from_header() { }

        // public IAsyncResult BeginReadBody(byte[] buffer, int offset, int count, AsyncCallback callback, object state){ return null; }
        // public int EndReadBody(IAsyncResult result){ return 0; }
        [Test]
        public void Can_read_body_manually() {
            IRequest r = new Req { BodyString = "I am the posted body" };

            // grab some bytes ...
            byte[] bytes = new byte[5];
            IAsyncResult result = r.BeginReadBody(bytes, 0, 5, null, null); // no callback or state, we want to do this synchronously
            int bytesRead = r.EndReadBody(result); // this should block!  per: http://msdn.microsoft.com/en-us/library/ms228967(v=VS.80).aspx
            Assert.That(Encoding.UTF8.GetString(bytes), Is.EqualTo("I am "));

            // grab the rest
            byte[] moreBytes = new byte[1000];
            result = r.BeginReadBody(moreBytes, 5, 1000, null, null); // no callback or state, we want to do this synchronously
            bytesRead = r.EndReadBody(result); // this should block!  per: http://msdn.microsoft.com/en-us/library/ms228967(v=VS.80).aspx
            Assert.That(Encoding.UTF8.GetString(WithoutTrailingBytes(moreBytes)), Is.EqualTo("the posted body"));
        }

        [Test]
        public void Can_get_all_bytes_from_getbody() {
            Request r = R(new Req { BodyString = "Hello world, how goes it? ™½ <--- Non-ASCII!" });
            byte[] bytes = r.BodyBytes;
            Assert.That(bytes.Length, Is.EqualTo(47));
            Assert.That(Encoding.UTF8.GetString(bytes), Is.EqualTo("Hello world, how goes it? ™½ <--- Non-ASCII!"));
        }

        [Test]
        public void Can_read_body_as_string() {
            Request r = R(new Req { BodyString = "Hello world, how goes it? ™½ <--- Non-ASCII!" });
            Assert.That(r.Body, Is.EqualTo("Hello world, how goes it? ™½ <--- Non-ASCII!"));
        }

        [Test]
        public void Can_get_the_value_of_a_POST_variable() {
            Req r = new Req();
            r.Headers["content-type"] = new string[] { "application/x-www-form-urlencoded" };

            r.BodyString = "";
            Assert.That(R(r).POST, Is.Empty);

            r.BodyString = "a=5";
            Assert.That(R(r).POST, Is.Not.Empty);
            Assert.That(R(r).POST["a"], Is.EqualTo("5"));
            Assert.That(R(r).POST["a"], Is.EqualTo("5"));
            Assert.That(R(r).POST["hi"], Is.Null);

            r.BodyString = "a=5&hi=there";
            Assert.That(R(r).POST["a"], Is.EqualTo("5"));
            Assert.That(R(r).POST["hi"], Is.EqualTo("there"));
        }

        [Test]
        public void Can_get_the_value_of_a_POST_or_GET_variable_with_Params() {
            Req r = new Req { Uri = "/?foo=bar", Method = "POST" };
	    r.BodyString = "a=1&b=2&hi=there";
	    Request request = R(r);

	    Assert.That(request.GET.Count,  Is.EqualTo(1));
	    Assert.That(request.POST.Count, Is.EqualTo(3));
	    Assert.That(request.Params.Count, Is.EqualTo(4));

	    Assert.That(request.Params["foo"],     Is.EqualTo("bar"));
	    Assert.That(request.Params["a"],       Is.EqualTo("1"));
	    Assert.That(request.Params["b"],       Is.EqualTo("2"));
	    Assert.That(request.Params["hi"],      Is.EqualTo("there"));
	    Assert.That(request.Params["noExist"], Is.Null);
	}

        [Test]
        public void Can_get_the_value_of_a_POST_or_GET_variable_via_indexer_which_is_an_alias_to_Params() {
            Req r = new Req { Uri = "/?foo=bar", Method = "POST" };
	    r.BodyString = "a=1&b=2&hi=there";
	    Request request = R(r);

	    Assert.That(request.GET.Count,  Is.EqualTo(1));
	    Assert.That(request.POST.Count, Is.EqualTo(3));
	    Assert.That(request.Params.Count, Is.EqualTo(4));

	    Assert.That(request["foo"],     Is.EqualTo("bar"));
	    Assert.That(request["a"],       Is.EqualTo("1"));
	    Assert.That(request["b"],       Is.EqualTo("2"));
	    Assert.That(request["hi"],      Is.EqualTo("there"));
	    Assert.That(request["noExist"], Is.Null);
	}

        [Test]
        public void Can_get_the_request_content_type() {
            Req req = new Req();
            req.Headers["content-type"] = new string[] { "text/html" };
            Assert.That(R(req).ContentType, Is.EqualTo("text/html"));

            req.Headers["content-type"] = new string[] { "text/plain" };
            Assert.That(R(req).ContentType, Is.EqualTo("text/plain"));

            req.Headers.Remove("content-type");
            Assert.Null(R(req).ContentType);
        }

        [Test]
        public void Can_get_whether_or_not_the_request_has_form_data() {
            Req req = new Req { Method = "GET" };

            // if application/x-www-form-urlencoded
            req.Headers["content-type"] = new string[] { "application/x-www-form-urlencoded" };
            Assert.True(R(req).HasFormData);

            // if multipart/form-data
            req.Headers["content-type"] = new string[] { "multipart/form-data" };
            Assert.True(R(req).HasFormData);

            // if POST and no ContentType provided
            req.Method = "POST";
            req.Headers.Remove("content-type");
            Assert.True(R(req).HasFormData);

            // NO if a GET with no ContentType
            req.Method = "GET";
            Assert.False(R(req).HasFormData);

            // NO if a POST with a different Contenttype
            req.Method = "POST";
            req.Headers["content-type"] = new string[] { "text/html" };
            Assert.False(R(req).HasFormData);
        }

	// we shouldn't have to *always* enumerate thru values
	[Test]
	public void Can_easily_get_the_string_value_for_a_Header_if_you_know_a_header_will_only_have_1_value() {
            Req r = new Req();
            r.Headers["content-type"] = new string[] { "text/plain" };

	    Assert.That(R(r).Headers["content-type"], Is.EqualTo(new string[] { "text/plain" }));
	    Assert.That(R(r).GetHeader("content-type"), Is.EqualTo("text/plain"));
	}

        [Test]
        [Ignore]
        public void Can_get_referer_or_referrer() { }

        [Test]
        public void Can_get_host() {
            Req r = new Req();

            r.Items.Remove("owin.server_name");
            Assert.Null(R(r).Host);

            r.Items["owin.server_name"] = "localhost";
            Assert.That(R(r).Host, Is.EqualTo("localhost"));

            r.Items["owin.server_name"] = "google.com";
            Assert.That(R(r).Host, Is.EqualTo("google.com"));
        }

        [Test]
        [Ignore]
        public void Can_get_host_from_header() { }

        [Test]
        public void Can_get_port() {
            Req r = new Req();

            r.Items.Remove("owin.server_port");
            Assert.That(R(r).Port, Is.EqualTo(0));

            r.Items["owin.server_port"] = "8080";
            Assert.That(R(r).Port, Is.EqualTo(8080));

            r.Items["owin.server_port"] = 1234;
            Assert.That(R(r).Port, Is.EqualTo(1234));
        }

        [Test]
        [Ignore]
        public void Can_get_port_from_header() { }

        [Test]
        public void Can_get_protocol() {
            Req r = new Req();

            r.Items["owin.request_protocol"] = "HTTP/1.1";
            Assert.That(R(r).Protocol, Is.EqualTo("HTTP/1.1"));

            r.Items["owin.request_protocol"] = "HTTP/1.0";
            Assert.That(R(r).Protocol, Is.EqualTo("HTTP/1.0"));
	}

        [Test]
        public void Can_get_scheme() {
            Req r = new Req();

            r.Items["owin.url_scheme"] = "http";
            Assert.That(R(r).Scheme, Is.EqualTo("http"));

            r.Items["owin.url_scheme"] = "https";
            Assert.That(R(r).Scheme, Is.EqualTo("https"));
	}

        [Test]
        public void Has_predicate_properties_for_checking_request_method_type() {
            Req r = new Req();

            r.Method = "GET";
            Assert.True(R(r).IsGet); Assert.False(R(r).IsPost); Assert.False(R(r).IsPut); Assert.False(R(r).IsDelete); Assert.False(R(r).IsHead);

            r.Method = "POST";
            Assert.False(R(r).IsGet); Assert.True(R(r).IsPost); Assert.False(R(r).IsPut); Assert.False(R(r).IsDelete); Assert.False(R(r).IsHead);

            r.Method = "PUT";
            Assert.False(R(r).IsGet); Assert.False(R(r).IsPost); Assert.True(R(r).IsPut); Assert.False(R(r).IsDelete); Assert.False(R(r).IsHead);

            r.Method = "DELETE";
            Assert.False(R(r).IsGet); Assert.False(R(r).IsPost); Assert.False(R(r).IsPut); Assert.True(R(r).IsDelete); Assert.False(R(r).IsHead);

            r.Method = "HEAD";
            Assert.False(R(r).IsGet); Assert.False(R(r).IsPost); Assert.False(R(r).IsPut); Assert.False(R(r).IsDelete); Assert.True(R(r).IsHead);
        }

	[Test]
	public void Can_get_IPAddress() {
            Req r = new Req();

            r.Items["owin.remote_endpoint"] = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 80);
            Assert.That(R(r).IPAddress.ToString(), Is.EqualTo("127.0.0.1"));

            r.Items["owin.remote_endpoint"] = new IPEndPoint(IPAddress.Parse("192.168.0.1"), 443);
            Assert.That(R(r).IPAddress.ToString(), Is.EqualTo("192.168.0.1"));
	}

        [Test]
        public void Can_get_IPEndPoint() {
            Req r = new Req();

            r.Items["owin.remote_endpoint"] = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 80);
            Assert.That(R(r).IPEndPoint.ToString(), Is.EqualTo("127.0.0.1:80"));

            r.Items["owin.remote_endpoint"] = new IPEndPoint(IPAddress.Parse("192.168.0.1"), 443);
            Assert.That(R(r).IPEndPoint.ToString(), Is.EqualTo("192.168.0.1:443"));
	}

        [Test]
        [Ignore]
        public void Can_be_instantiated_with_no_arguments_to_build_a_new_request() { } // <---- this is what we really need now ... Handlers.Request?

        byte[] WithoutTrailingBytes(byte[] bytes) {
            int i = bytes.Length - 1;
            while (bytes[i] == 0)
                i--;

            if (i == 0)
                return bytes;
            else {
                byte[] newBytes = new byte[i + 1];
                Array.Copy(bytes, newBytes, i + 1);
                return newBytes;
            }
        }
    }
}
