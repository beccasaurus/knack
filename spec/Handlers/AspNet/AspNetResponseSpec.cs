using System;
using System.IO;
using System.Web;
using System.Text;
using System.Collections.Generic;
using NUnit.Framework;

namespace Owin.Handlers.Specs {

    [TestFixture]
    public class AspNetResponseSpec {

        [Test][Ignore("Trying to get HttpResponse.Headers blows up with PlatformNotSupportedException : This operation requires IIS integrated pipeline mode")]
        public void Can_convert_IResponse_into_HttpResponse_given_an_HttpResponse_reference() {
            TextWriter   body     = new StringWriter();
            HttpResponse response = new HttpResponse(body);

            // check Response defaults
            Assert.That(response.Status, Is.EqualTo("200 OK"));
            Assert.That(body.ToString(), Is.EqualTo(""));
            Assert.That(response.Headers.Count, Is.EqualTo(0));

            AspNet.WriteResponse(response, new Response(404, "Oops, not found!", new Dictionary<string,string>{{"content-type","text/plains"}}));

            // check that WriteResponse() updated the response
            Assert.That(response.Status, Is.EqualTo("404 NotFound"));
            Assert.That(body.ToString(), Is.EqualTo("Oops, not found!"));
            Assert.That(response.Headers.Count, Is.EqualTo(1));
            Assert.That(response.Headers["content-type"], Is.EqualTo("text/plain"));
        }
    }
}