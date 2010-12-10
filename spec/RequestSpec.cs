using System;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Collections.Specialized;
using Owin;
using NUnit.Framework;

namespace Owin.Common.Specs {

    // *Simple* struct-like IRequest implementation for RequestSpec
    // we will probably take this class and use it or parts of it for Owin.Request or, if we make it, Owin.Handlers.Request (?)
    public class Req : IRequest {
	public delegate void DoWork();

	public Req() {
	    // Make the Request valid
	    Method = "GET";
	    Uri    = "";
	    Items  = new Dictionary<string, object>();
	    Items["owin.base_path"]        = "";
	    Items["owin.server_name"]      = "localhost";
	    Items["owin.server_port"]      = 80;
	    Items["owin.request_protocol"] = "HTTP/1.1";
	    Items["owin.url_scheme"]       = "http";
	    Items["owin.remote_endpoint"]  = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 80);
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

	    // make spec for request validations?
	    // Items must be set
	    // required items must be present and have valid values

	    Request req;

	    // Helper method for getting a new Owin.Request, given an IRequest to instantiate it with
	    Request R(IRequest innerRequest) {
		return new Request(innerRequest);
	    }
	    
	    [SetUp] public void Before() { req = null; }

	    [Test]
	    public void Can_be_instantiated_with_an_IRequest() {
		Assert.That(R(new Req { Method = "GET"  }).Method, Is.EqualTo("GET"));
		Assert.That(R(new Req { Method = "POST" }).Method, Is.EqualTo("POST"));
		Assert.That(R(new Req { Uri    = "/foo" }).Uri,    Is.EqualTo("/foo"));
		Assert.That(R(new Req { Uri    = "/bar" }).Uri,    Is.EqualTo("/bar"));
	    }

	    [Test]
	    public void Can_get_the_full_Url() {
		Req request = new Req { Uri = "/foo/bar?hi=there" };
		request.Items["owin.base_path"]   = "/my/app";
		request.Items["owin.server_name"] = "localhost";
		request.Items["owin.server_port"] = 80;
		request.Items["owin.url_scheme"]  = "http";
		Assert.That(R(request).Url, Is.EqualTo("http://localhost/my/app/foo/bar?hi=there"));

		request.Items["owin.base_path"]   = "/my/root";
		Assert.That(R(request).Url, Is.EqualTo("http://localhost/my/root/foo/bar?hi=there"));

		request.Uri = "/neat";
		Assert.That(R(request).Url, Is.EqualTo("http://localhost/my/root/neat"));

		request.Items["owin.server_name"] = "code.com";
		Assert.That(R(request).Url, Is.EqualTo("http://code.com/my/root/neat"));

		request.Items["owin.url_scheme"]  = "https";
		request.Items["owin.server_port"] = 443;
		Assert.That(R(request).Url, Is.EqualTo("https://code.com/my/root/neat"));

		request.Items["owin.server_port"] = 123;
		Assert.That(R(request).Url, Is.EqualTo("https://code.com:123/my/root/neat"));

		request.Items["owin.url_scheme"]  = "http";
		Assert.That(R(request).Url, Is.EqualTo("http://code.com:123/my/root/neat"));
	    }
	    
	    [Test]
	    public void Can_get_the_raw_QueryString_from_Uri() {
		Assert.That(R(new Req { Uri = "/"                 }).QueryString, Is.EqualTo(""));
		Assert.That(R(new Req { Uri = "/foo"              }).QueryString, Is.EqualTo(""));
		Assert.That(R(new Req { Uri = "/foo?a=5"          }).QueryString, Is.EqualTo("a=5"));
		Assert.That(R(new Req { Uri = "/foo?a=5&hi=there" }).QueryString, Is.EqualTo("a=5&hi=there"));
		Assert.That(R(new Req { Uri = "/?a=5&hi=there"    }).QueryString, Is.EqualTo("a=5&hi=there"));
		Assert.That(R(new Req { Uri = "?a=5&hi=there"     }).QueryString, Is.EqualTo("a=5&hi=there"));
	    }

	    [Test]
	    public void Can_get_the_value_of_a_QueryString() {
		Assert.That(R(new Req { Uri = "/"                 }).GET, Is.Empty);
		Assert.That(R(new Req { Uri = "/foo"              }).GET, Is.Empty);

		Assert.That(R(new Req { Uri = "/foo?a=5"          }).GET, Is.Not.Empty);
		Assert.That(R(new Req { Uri = "/foo?a=5"          }).GET["a"],  Is.EqualTo("5"));
		Assert.That(R(new Req { Uri = "/foo?a=5"          }).GET["a"],  Is.EqualTo("5"));
		Assert.That(R(new Req { Uri = "/foo?a=5"          }).GET["hi"], Is.Null);

		Assert.That(R(new Req { Uri = "/foo?a=5&hi=there" }).GET["a"],  Is.EqualTo("5"));
		Assert.That(R(new Req { Uri = "/foo?a=5&hi=there" }).GET["hi"], Is.EqualTo("there"));
	    }

	    // public IAsyncResult BeginReadBody(byte[] buffer, int offset, int count, AsyncCallback callback, object state){ return null; }
	    // public int EndReadBody(IAsyncResult result){ return 0; }
	    [Test]
	    public void Can_read_body_manually() {
		IRequest r = new Req { BodyString = "I am the posted body" };
		
		// grab some bytes ...
		byte[] bytes        = new byte[5];
		IAsyncResult result = r.BeginReadBody(bytes, 0, 5, null, null); // no callback or state, we want to do this synchronously
		int bytesRead       = r.EndReadBody(result); // this should block!  per: http://msdn.microsoft.com/en-us/library/ms228967(v=VS.80).aspx
		Assert.That(Encoding.UTF8.GetString(bytes), Is.EqualTo("I am "));

		// grab the rest
		byte[] moreBytes = new byte[1000];
		result           = r.BeginReadBody(moreBytes, 5, 1000, null, null); // no callback or state, we want to do this synchronously
		bytesRead        = r.EndReadBody(result); // this should block!  per: http://msdn.microsoft.com/en-us/library/ms228967(v=VS.80).aspx
		Assert.That(Encoding.UTF8.GetString(WithoutTrailingBytes(moreBytes)), Is.EqualTo("the posted body"));
	    }

	    byte[] WithoutTrailingBytes(byte[] bytes) {
		int i = bytes.Length - 1;
		while (bytes[i] == 0)
		    i--;

		if (i == 0)
		    return bytes;
		else {
		    byte[] newBytes = new byte[i+1];
		    Array.Copy(bytes, newBytes, i+1);
		    return newBytes;
		}
	    }

	    [Test][Ignore]
	    public void Can_read_body() {
		//Request r = R(new Req { BodyString = "I am the posted body" });
		//Assert.That(r.Body, Is.EqualTo("I am the posted body")); // <--- blocks until it gets the full body
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
