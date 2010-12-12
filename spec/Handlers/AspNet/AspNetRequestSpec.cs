using System;
using System.Web;
using NUnit.Framework;

namespace Owin.Handlers.Specs {

    [TestFixture]
    public class AspNetRequestSpec {

        AspNetRequest request;

        HttpRequest GetRequest() {
            return GetRequest("/");
        }
        HttpRequest GetRequest(string uri) {
            return GetRequest("GET", uri);
        }
        HttpRequest GetRequest(string method, string uri) {
            return GetRequest(method, uri, "");
        }
        HttpRequest GetRequest(string method, string uri, string queryStrings) {
            string url = uri;
            if (uri.StartsWith("/"))
                url = "http://localhost" + uri;
            if (queryStrings != "")
                url += "?" + queryStrings;
            HttpRequest request = new HttpRequest("", url, queryStrings);
            request.RequestType = method;
            return request;
        }

        [SetUp]
        public void Before() { request = null; }

        [TearDown]
        public void After() {
            if (request != null)
                Owin.Lint.Validate(request);
        }

        [TestFixture]
        public class Uri_Method_and_QueryStrings : AspNetRequestSpec {

            [Test]
            public void Method_gets_set_using_Request_RequestType() {
                request = new AspNetRequest(GetRequest());
                Assert.That(request.Method, Is.EqualTo("GET"));

                request = new AspNetRequest(GetRequest("POST", "/"));
                Assert.That(request.Method, Is.EqualTo("POST"));
            }

            [Test]
            public void Uri_gets_set_using_Request_Path() {
                request = new AspNetRequest(GetRequest("/"));
                Assert.That(request.Uri, Is.EqualTo("/"));

                request = new AspNetRequest(GetRequest("/foo/bar"));
                Assert.That(request.Uri, Is.EqualTo("/foo/bar"));
            }

            [Test]
            public void Uri_includes_query_strings_from_QueryString() {
                request = new AspNetRequest(GetRequest("GET", "/", ""));
                Assert.That(request.Uri, Is.EqualTo("/"));

                request = new AspNetRequest(GetRequest("GET", "/", "foo=bar"));
                Assert.That(request.Uri, Is.EqualTo("/?foo=bar"));
            }

            [Test]
            public void Uri_gets_QueryString_from_QueryString() {
                request = new AspNetRequest(GetRequest("GET", "/", ""));
                Assert.That(request.QueryString, Is.EqualTo(""));

                request = new AspNetRequest(GetRequest("GET", "/", "foo=bar"));
                Assert.That(request.QueryString, Is.EqualTo("foo=bar"));
            }
        }

        [TestFixture]
        public class Items : AspNetRequestSpec {

            [Test]
            public void owin_base_path_gets_set_to_Request_ApplicationPath() {
                HttpRequest native = GetRequest();
                Assert.That(native.ApplicationPath, Is.Null); // it returns null and we can't change it

                request = new AspNetRequest(native);
                Assert.That(request.BasePath, Is.EqualTo(""));
            }

            [Test]
            public void owin_server_name_gets_set_to_Request_Url_Host() {
                request = new AspNetRequest(GetRequest("http://localhost/"));
                Assert.That(request.Host, Is.EqualTo("localhost"));

                request = new AspNetRequest(GetRequest("http://google.com/"));
                Assert.That(request.Host, Is.EqualTo("google.com"));
            }

            [Test]
            public void owin_server_port_gets_set_to_Request_Url_Port() {
                request = new AspNetRequest(GetRequest("http://localhost/"));
                Assert.That(request.Port, Is.EqualTo(80));

                request = new AspNetRequest(GetRequest("http://localhost:1234/"));
                Assert.That(request.Port, Is.EqualTo(1234));
            }

            [Test]
            public void owin_request_protocol_gets_set_to_HTTP_1_1() {
                request = new AspNetRequest(GetRequest());
                Assert.That(request.Protocol, Is.EqualTo("HTTP/1.1")); // not sure if we can detect this - just use 1.1
            }

            [Test]
            public void owin_url_scheme_gets_set_using_scheme_seen_in_Request_Url_Scheme() {
                request = new AspNetRequest(GetRequest("http://localhost/"));
                Assert.That(request.Scheme, Is.EqualTo("http"));

                request = new AspNetRequest(GetRequest("https://localhost/"));
                Assert.That(request.Scheme, Is.EqualTo("https"));
            }

            [Test]
            public void owin_remote_endpoint_gets_set_using_Request_UserHostAddress_and_Port() {
                HttpRequest native = GetRequest(); // unfortunately, we can't set this  :/
                Assert.That(native.UserHostAddress, Is.Null);

                // if it's null, we'll do 0.0.0.0:80 ... cause, why not?
                request = new AspNetRequest(native);
                Assert.That(request.IPEndPoint.ToString(), Is.EqualTo("0.0.0.0:80"));

                // if we change the port, it should change the port in the end point
                request = new AspNetRequest(GetRequest("http://localhost:1234/foo"));
                Assert.That(request.IPEndPoint.ToString(), Is.EqualTo("0.0.0.0:1234"));
            }
        }

        [TestFixture]
        public class Headers : AspNetRequestSpec {

            [Test][Ignore("If you try to modify HttpRequest.Headers it throws a PlatformNotSupportedException")]
            public void All_environment_variables_become_headers() {
            }
        }

        [TestFixture]
        public class Body : AspNetRequestSpec {

            [Test][Ignore("If you try to modify HttpRequest.Form it throws NotSupportedException : Collection is read-only")]
            public void Can_get_posted_params() {
            }
        }
    }
}