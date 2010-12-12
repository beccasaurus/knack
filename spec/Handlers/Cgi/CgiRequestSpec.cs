using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Owin;
using Owin.Handlers;
using NUnit.Framework;

namespace Owin.Handlers.Specs {

    // Instead of writing integration specs that actually go over HTTP, we test some of the 
    // low level CGI stuff to make sure it generates a valid IRequest to pass to the application 
    // and it properly renders the IResponse
    [TestFixture]
    public class CgiRequestSpec {

        // TODO we might want to move the example IDictionary and its helper methods into a base class
        //      it's really conventient to have it right here, however, while writing tests, so you don't 
        //      have to check a different file to see what the default variables are

        // Default variables are the result of doing a GET to / where the name of the cgi script is cgi-runner.cgi
        IDictionary GetEnvironmentVariables() {
            IDictionary vars = new Hashtable();
            vars.Add("REMOTE_HOST", "localhost");
            vars.Add("QUERY_STRING", "");
            vars.Add("SCRIPT_NAME", "/cgi-runner.cgi");
            vars.Add("REQUEST_METHOD", "GET");
            vars.Add("REQUEST_URI", "http://localhost:3000/cgi-runner.cgi/");
            vars.Add("SERVER_NAME", "localhost");
            vars.Add("SERVER_PORT", "3000");
            vars.Add("HTTP_USER_AGENT", "curl/7.21.0 (i686-pc-linux-gnu) libcurl/7.21.0 OpenSSL/0.9.8o zlib/1.2.3.4 libidn/1.18");
            vars.Add("REMOTE_ADDR", "127.0.0.1");
            vars.Add("PWD", "/home/person/Desktop/Cgi-testing");
            vars.Add("SERVER_SOFTWARE", "WEBrick/1.3.1 (Ruby/1.8.7/2010-06-23)");
            vars.Add("HTTP_HOST", "localhost:3000");
            vars.Add("GATEWAY_INTERFACE", "CGI/1.1");
            vars.Add("PATH_INFO", "/");
            vars.Add("_", "./Testing.exe");
            vars.Add("SERVER_PROTOCOL", "HTTP/1.1");
            vars.Add("SCRIPT_FILENAME", "/home/person/Desktop/Cgi-testing/cgi-runner.cgi");
            vars.Add("HTTP_ACCEPT", "*/*");
            vars.Add("SHLVL", "1");
            return vars;
        }

        IDictionary GetEnvironmentVariables(IDictionary<string, string> overrides) {
            IDictionary vars = GetEnvironmentVariables();
            foreach (KeyValuePair<string, string> item in overrides)
                vars[item.Key] = item.Value;
            return vars;
        }

        IDictionary ENV_Simple { get { return GetEnvironmentVariables(); } }

        IDictionary ENV_with_QueryString {
            get {
                return GetEnvironmentVariables(new Dictionary<string, string>{ 
		    {"PATH_INFO",    "/foo/bar"}, 
		    {"REQUEST_URI",  "http://localhost:3000/cgi-runner.cgi/foo/bar?hi=there"},
		    {"QUERY_STRING", "hi=there"}
		});
            }
        }

        CgiRequest request;

        [SetUp]
        public void Before() { request = null; }

        [TearDown]
        public void After() {
            if (request != null)
                Owin.Lint.Validate(request);
        }

        [TestFixture]
        public class Uri_Method_and_QueryStrings : CgiRequestSpec {

            [Test]
            public void Method_gets_set_using_REQUEST_METHOD() {
                request = new CgiRequest(ENV_Simple);
                Assert.That(request.Method, Is.EqualTo("GET"));

                request = new CgiRequest(GetEnvironmentVariables(new Dictionary<string, string> { { "REQUEST_METHOD", "POST" } }));
                Assert.That(request.Method, Is.EqualTo("POST"));
            }

            [Test]
            public void Uri_gets_set_using_PATH_INFO() {
                request = new CgiRequest(ENV_Simple);
                Assert.That(request.Uri, Is.EqualTo("/"));
            }

            [Test]
            public void Uri_includes_query_strings_from_QUERY_STRING() {
                request = new CgiRequest(ENV_with_QueryString);
                Assert.That(request.Uri, Is.EqualTo("/foo/bar?hi=there"));
            }

            [Test]
            public void Uri_gets_QueryString_from_QUERY_STRING() {
                request = new CgiRequest(ENV_Simple);
                Assert.That(request.QueryString, Is.EqualTo(""));

                request = new CgiRequest(ENV_with_QueryString);
                Assert.That(request.QueryString, Is.EqualTo("hi=there"));
            }
        }

        [TestFixture]
        public class Items : CgiRequestSpec {

            [Test]
            public void owin_base_path_gets_set_to_SCRIPT_NAME() {
                request = new CgiRequest(ENV_Simple);
                Assert.That(request.BasePath, Is.EqualTo("/cgi-runner.cgi"));

                request = new CgiRequest(GetEnvironmentVariables(new Dictionary<string, string> { { "SCRIPT_NAME", "/hi/there.cgi" } }));
                Assert.That(request.BasePath, Is.EqualTo("/hi/there.cgi"));
            }

            [Test]
            public void owin_server_name_gets_set_to_SERVER_NAME() {
                request = new CgiRequest(ENV_Simple);
                Assert.That(request.Host, Is.EqualTo("localhost"));

                request = new CgiRequest(GetEnvironmentVariables(new Dictionary<string, string> { { "SERVER_NAME", "google.com" } }));
                Assert.That(request.Host, Is.EqualTo("google.com"));
            }

            [Test]
            public void owin_server_port_gets_set_to_SERVER_PORT() {
                request = new CgiRequest(ENV_Simple);
                Assert.That(request.Port, Is.EqualTo(3000));

                request = new CgiRequest(GetEnvironmentVariables(new Dictionary<string, string> { { "SERVER_PORT", "1234" } }));
                Assert.That(request.Port, Is.EqualTo(1234));
            }

            [Test]
            public void owin_request_protocol_gets_set_to_SERVER_PROTOCOL() {
                request = new CgiRequest(ENV_Simple);
                Assert.That(request.Protocol, Is.EqualTo("HTTP/1.1"));

                request = new CgiRequest(GetEnvironmentVariables(new Dictionary<string, string> { { "SERVER_PROTOCOL", "HTTP/1.0" } }));
                Assert.That(request.Protocol, Is.EqualTo("HTTP/1.0"));
            }

            [Test]
            public void owin_url_scheme_gets_set_using_scheme_seen_in_REQUEST_URI() {
                request = new CgiRequest(ENV_Simple);
                Assert.That(request.Scheme, Is.EqualTo("http"));

                request = new CgiRequest(GetEnvironmentVariables(new Dictionary<string, string> { { "REQUEST_URI", "https://localhost/foo.cgi/" } }));
                Assert.That(request.Scheme, Is.EqualTo("https"));
            }

            [Test]
            public void owin_remote_endpoint_gets_set_using_REMOTE_ADDR_and_SERVER_PORT() {
                request = new CgiRequest(ENV_Simple);
                Assert.That(request.IPEndPoint.ToString(), Is.EqualTo("127.0.0.1:3000"));

                request = new CgiRequest(GetEnvironmentVariables(new Dictionary<string, string> { { "REMOTE_ADDR", "192.168.0.1" }, { "SERVER_PORT", "443" } }));
                Assert.That(request.IPEndPoint.ToString(), Is.EqualTo("192.168.0.1:443"));
            }
        }

        [TestFixture]
        public class Headers : CgiRequestSpec {

            [Test]
            public void ContentType_gets_set_by_CONTENT_TYPE_header() {
                request = new CgiRequest(ENV_Simple);
                Assert.That(request.ContentType, Is.Null);

                request = new CgiRequest(GetEnvironmentVariables(new Dictionary<string, string> { { "CONTENT_TYPE", "application/json" } }));
                Assert.That(request.ContentType, Is.EqualTo("application/json"));
            }

            [Test]
            public void All_environment_variables_become_headers() {
                request = new CgiRequest(ENV_Simple);

                Assert.That(request.GetHeader("REMOTE_HOST"), Is.EqualTo("localhost"));
                Assert.That(request.GetHeader("QUERY_STRING"), Is.EqualTo(""));
                Assert.That(request.GetHeader("SCRIPT_NAME"), Is.EqualTo("/cgi-runner.cgi"));
                Assert.That(request.GetHeader("REQUEST_METHOD"), Is.EqualTo("GET"));
                Assert.That(request.GetHeader("REQUEST_URI"), Is.EqualTo("http://localhost:3000/cgi-runner.cgi/"));
                Assert.That(request.GetHeader("SERVER_NAME"), Is.EqualTo("localhost"));
                Assert.That(request.GetHeader("SERVER_PORT"), Is.EqualTo("3000"));
                Assert.That(request.GetHeader("HTTP_USER_AGENT"), Is.EqualTo("curl/7.21.0 (i686-pc-linux-gnu) libcurl/7.21.0 OpenSSL/0.9.8o zlib/1.2.3.4 libidn/1.18"));
            }
        }

        [TestFixture]
        public class Body : CgiRequestSpec {

            [Test]
            public void Can_initialize_CgiRequest_with_body_content() {
                request = new CgiRequest(ENV_Simple, "hi");
                Assert.That(request.Body, Is.EqualTo("hi"));
            }

            [Test]
            public void Can_get_and_set_body() {
                request = new CgiRequest(ENV_Simple);
                request.SetBody("hello world!");

                Assert.That(request.Body, Is.EqualTo("hello world!"));
            }

            [Test]
            public void x_www_form_urlencoded_POST_variables_are_parsed_properly() {
                request = new CgiRequest(GetEnvironmentVariables(new Dictionary<string, string>{
				{"REQUEST_METHOD","POST"},
				{"CONTENT_TYPE","application/x-www-form-urlencoded"}
				}), "name=Rover&breed=GoldenRetriever");

                Assert.That(request.POST["name"], Is.EqualTo("Rover"));
                Assert.That(request.POST["breed"], Is.EqualTo("GoldenRetriever"));
                Assert.That(request.POST["didntPostThis"], Is.Null);
            }

            [Test]
            [Ignore]
            public void multipart_formdata_POST_variables_are_parsed_properly() {
            }
        }
    }
}