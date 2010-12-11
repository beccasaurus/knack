using System;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Collections.Specialized;
using Owin;
using NUnit.Framework;

namespace Owin.Common.Specs {

    [TestFixture]
    public class RequestWriterSpec {

	RequestWriter request;

	[SetUp]
	public void Before() { request = new Owin.RequestWriter(); }

	[TearDown]
	public void After() {
	    if (request != null)
		Owin.Lint.Validate(request);
	}

	[TestFixture]
	public class Defaults : RequestWriterSpec {
	    
	    [Test]
	    public void Default_method_is_get() {
		Assert.That(request.Method, Is.EqualTo("GET"));
	    }

	    [Test]
	    public void Default_uri_is_blank() {
		Assert.That(request.Uri, Is.EqualTo(""));
	    }

	    [Test]
	    public void Request_headers_are_empty_by_default() {
		Assert.That(request.Headers, Is.Empty);
	    }

	    [Test][Ignore]
	    public void Request_body_is_empty_by_default() {
	    }

	    [Test]
	    public void Items_owin_base_path_is_blank_by_default() {
		Assert.That(request.Items["owin.base_path"], Is.EqualTo(""));
	    }

	    [Test]
	    public void Items_owin_server_name_is_localhost_by_default() {
		Assert.That(request.Items["owin.server_name"], Is.EqualTo("localhost"));
	    }

	    [Test]
	    public void Items_owin_server_port_is_80_by_default() {
		Assert.That(request.Items["owin.server_port"], Is.EqualTo(80));
	    }

	    [Test]
	    public void Items_owin_request_protocol_is_http_1_1_by_default() {
		Assert.That(request.Items["owin.request_protocol"], Is.EqualTo("HTTP/1.1"));
	    }

	    [Test]
	    public void Items_owin_url_scheme_is_http_by_default() {
		Assert.That(request.Items["owin.url_scheme"], Is.EqualTo("http"));
	    }

	    [Test]
	    public void Items_owin_remote_endpoint_is_127_0_0_1_over_port_80_by_default() {
		Assert.That(request.Items["owin.remote_endpoint"].ToString(), Is.EqualTo("127.0.0.1:80"));
	    }
	}

	[TestFixture]
	public class Constructors : RequestWriterSpec {

	    // TODO *THIS* is where i left off ... after doing the constructors, was gonna do modifying and 
	    //      then focus on setting/getting the request body and making sure that stuff works
	    [Test][Ignore]
	    public void Can_make_new_with_method_and_uri() { // make sure all owin. properties get set properly!!!!
	    }

	    [Test][Ignore]
	    public void Can_make_new_with_just_uri() {
	    }

	    [Test][Ignore]
	    public void Can_make_new_with_uri_and_headers() {
	    }

	    [Test][Ignore]
	    public void Can_make_new_with_method_and_uri_and_headers() {
	    }
	}

	[TestFixture]
	public class Modifying : RequestWriterSpec {

	}

	[TestFixture]
	public class Reading : RequestWriterSpec {

	}
    }
}
