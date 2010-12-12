using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Owin;
using Owin.Handlers;
using NUnit.Framework;

namespace Owin.Handlers.Specs {

    // spec for how Cgi prints out an IResponse
    [TestFixture]
    public class CgiResponseSpec {

	[Test]
	public void ShouldCreateResponseOK() {
	    string output = Cgi.GetResponseText(new Response(200, "Hello world", new Dictionary<string,string>{{"content-type","text/plain"}}));
	    Assert.That(output, Is.EqualTo("Status: 200 OK\ncontent-type: text/plain\ncontent-length: 11\n\nHello world"));
	}
    }
}
