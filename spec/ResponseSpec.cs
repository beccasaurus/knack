using System;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Collections.Specialized;
using Owin;
using NUnit.Framework;

namespace Owin.Common.Specs {

    [TestFixture]
    public class ResponseSpec {

	Response response;

	[SetUp]
	public void Before() { response = new Owin.Response(); }

	[TearDown]
	public void After() {
	    if (response != null)
		Owin.Lint.Validate(response);
	}

	[TestFixture]
	public class Defaults : ResponseSpec {

	    [Test]
	    public void Default_status_is_200() {
	        Assert.That(response.Status, Is.EqualTo("200 OK"));
	        Assert.That(response.StatusCode, Is.EqualTo(200));
	        Assert.That(response.StatusMessage, Is.EqualTo("OK"));
	    }

	    [Test]
	    public void Default_body_is_blank() {
	        Assert.That(response.BodyText, Is.EqualTo(""));
	    }

	    [Test]
	    public void Default_content_type_is_text_html() {
	        Assert.That(response.ContentType, Is.EqualTo("text/html"));
	    }
	}

	[TestFixture]
	public class Constructors : ResponseSpec {

	    [Test]
	    public void Can_make_new_with_just_a_string_body() {
		response = new Response("Hello World!");
		Assert.That(response.BodyText, Is.EqualTo("Hello World!"));
	    }

	    [Test]
	    public void Can_make_new_with_body_and_status_string() {
		response = new Response("Oh noes, we don't know that", "404 Not Found");
		Assert.That(response.BodyText, Is.EqualTo("Oh noes, we don't know that"));
		Assert.That(response.Status,   Is.EqualTo("404 Not Found"));
	    }

	    [Test]
	    public void Can_make_new_with_body_and_status_code() {
		response = new Response("Oh noes, we don't know that", 404);
		Assert.That(response.BodyText, Is.EqualTo("Oh noes, we don't know that"));
		Assert.That(response.Status,   Is.EqualTo("404 NotFound"));
	    }

	    [Test]
	    public void Can_make_new_with_body_and_headers() {
		response = new Response("some json", new Dictionary<string,string>{{"content-type","application/json"}});
		Assert.That(response.BodyText, Is.EqualTo("some json"));
		Assert.That(response.Headers["content-type"], Is.EqualTo(new string[]{ "application/json" }));
	    }

	    [Test]
	    public void Can_make_new_with_status_code_and_headers_as_string_string_dictionary() {
		response = new Response(302, new Dictionary<string,string>{{"location", "/new/path"}});
		Assert.That(response.Status, Is.EqualTo("302 Redirect"));
		Assert.That(response.Headers["location"], Is.EqualTo(new string[]{ "/new/path" }));
	    }

	    [Test]
	    public void Can_make_new_with_status_code_and_headers() {
		response = new Response(302, new Dictionary<string,IEnumerable<string>>{{"location", new string[]{ "/new/path" }}});
		Assert.That(response.Status, Is.EqualTo("302 Redirect"));
		Assert.That(response.Headers["location"], Is.EqualTo(new string[]{ "/new/path" }));
	    }

	    [Test]
	    public void Can_make_new_with_body_status_and_headers() {
		response = new Response(404, "We couldn't find that", new Dictionary<string,IEnumerable<string>>{{"content-type", new string[] {"text/plain"}}});
		Assert.That(response.BodyText, Is.EqualTo("We couldn't find that"));
		Assert.That(response.Status, Is.EqualTo("404 NotFound"));
		Assert.That(response.Headers["content-type"], Is.EqualTo(new string[]{ "text/plain" }));
	    }

	    [Test]
	    public void Can_make_new_with_body_status_and_headers_as_string_string_dictionary() {
		response = new Response(404, "We couldn't find that", new Dictionary<string,string>{{"content-type", "text/plain"}});
		Assert.That(response.BodyText, Is.EqualTo("We couldn't find that"));
		Assert.That(response.Status, Is.EqualTo("404 NotFound"));
		Assert.That(response.Headers["content-type"], Is.EqualTo(new string[]{ "text/plain" }));
	    }
	}

	[TestFixture]
	public class Modifying : ResponseSpec {

	    // Headers should make sure that you can't accidentally set Headers["location"] and Headers["Location"] ... maybe?

	    [Test]
	    public void Can_write_to_body() {
		Assert.That(response.BodyText, Is.EqualTo(""));
		
		response.Write("Hello there");
		Assert.That(response.BodyText, Is.EqualTo("Hello there"));
		
		response.Write("  How goes it?");
		Assert.That(response.BodyText, Is.EqualTo("Hello there  How goes it?"));

		response.Clear();
		Assert.That(response.BodyText, Is.EqualTo(""));
	    }

	    [Test]
	    public void Writing_to_body_updates_content_length() {
		Assert.That(response.BodyText, Is.EqualTo(""));
		Assert.That(response.ContentLength, Is.EqualTo(0));
		
		response.Write("Hello there");
		Assert.That(response.BodyText, Is.EqualTo("Hello there"));
		Assert.That(response.ContentLength, Is.EqualTo(11));

		response.Write("6 more");
		Assert.That(response.ContentLength, Is.EqualTo(17));
	    }

	    [Test]
	    public void Setting_body_directly_updates_content_length() {
		response.BodyText = "123"; 
		Assert.That(response.ContentLength, Is.EqualTo(3));

		response.BodyText += "456"; 
		Assert.That(response.ContentLength, Is.EqualTo(6));

		response.BodyText = "1"; 
		Assert.That(response.ContentLength, Is.EqualTo(1));
	    }
	}

	[TestFixture]
	public class Reading : ResponseSpec { // TODO add examples of helpers that Owin.Response has for reading things like headers, body, etc

	}
    }
}
