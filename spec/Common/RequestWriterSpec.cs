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

            [Test]
            [Ignore]
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

            [Test]
            public void Can_make_new_with_method_and_uri() {
                request = new RequestWriter("POST", "/dogs");
                Assert.That(request.Method, Is.EqualTo("POST"));
                Assert.That(request.Uri, Is.EqualTo("/dogs"));
            }

            [Test]
            public void Can_make_new_with_just_uri_which_defaults_to_get() {
                request = new RequestWriter("/cats");
                Assert.That(request.Method, Is.EqualTo("GET"));
                Assert.That(request.Uri, Is.EqualTo("/cats"));
            }

            [Test]
            public void Can_make_new_with_method_and_uri_and_body_as_string() {
                request = new RequestWriter("POST", "/dogs", "name=Rover&breed=GoldenRetriever");
                Assert.That(request.Method, Is.EqualTo("POST"));
                Assert.That(request.Uri, Is.EqualTo("/dogs"));
                Assert.That(request.Body, Is.EqualTo("name=Rover&breed=GoldenRetriever"));
            }

            [Test]
            public void Can_make_new_with_method_and_uri_and_body_as_bytes() {
                byte[] bytes = Encoding.UTF8.GetBytes("Hello there!");
                request = new RequestWriter("POST", "/dogs", bytes);
                Assert.That(request.Method, Is.EqualTo("POST"));
                Assert.That(request.Uri, Is.EqualTo("/dogs"));
                Assert.That(request.BodyBytes, Is.EqualTo(bytes));
                Assert.That(request.Body, Is.EqualTo("Hello there!"));
            }

            [Test]
            [Ignore]
            public void Can_make_new_with_uri_and_headers() {
            }

            [Test]
            [Ignore]
            public void Can_make_new_with_method_and_uri_and_headers() {
            }
        }

        [TestFixture]
        public class Modifying : RequestWriterSpec {

            [Test]
            [Ignore]
            public void Can_change_the_default_encoding_used_for_body() { } // ? Maybe ?

            [TestFixture]
            public class Fluent : RequestWriterSpec {

                [Test]
                public void Can_SetMethod() {
                    request = new RequestWriter().SetMethod("PUT");
                    Assert.That(request.Method, Is.EqualTo("PUT"));
                }

                [Test]
                public void Can_SetUri() {
                    request = new RequestWriter().SetMethod("PUT");
                    Assert.That(request.Method, Is.EqualTo("PUT"));
                }

                [Test]
                public void Can_SetHeader_with_string() {
                    request = new RequestWriter().SetHeader("content-type", "application/json");
                    Assert.That(request.Headers["content-type"], Is.EqualTo(new string[] { "application/json" }));
                }

                [Test]
                public void Can_SetHeader_with_IEnumerable_string() {
                    request = new RequestWriter().SetHeader("content-type", new string[] { "application/xml" });
                    Assert.That(request.Headers["content-type"], Is.EqualTo(new string[] { "application/xml" }));
                }

                [Test]
                public void Can_AddHeader_which_adds_to_the_IEnumerable_with_string() {
                    request = new RequestWriter().SetHeader("content-type", "application/json").SetHeader("content-type", "application/second");
                    Assert.That(request.Headers["content-type"], Is.EqualTo(new string[] { "application/second" }));

                    // Now use AddHeader, and both values should be present
                    request = new RequestWriter().SetHeader("content-type", "application/json").AddHeader("content-type", "application/second");
                    Assert.That(request.Headers["content-type"], Is.EqualTo(new string[] { "application/json", "application/second" }));
                }

                [Test]
                public void Can_AddHeader_which_adds_to_the_IEnumerable_with_IEnumerable_string() {
                    request = new RequestWriter().SetHeader("content-type", new string[] { "application/json" }).SetHeader("content-type", new string[] { "application/second" });
                    Assert.That(request.Headers["content-type"], Is.EqualTo(new string[] { "application/second" }));

                    // Now use AddHeader, and both values should be present
                    request = new RequestWriter().SetHeader("content-type", new string[] { "application/json" }).AddHeader("content-type", new string[] { "application/second" });
                    Assert.That(request.Headers["content-type"], Is.EqualTo(new string[] { "application/json", "application/second" }));
                }

                [Test]
                public void Can_SetItem() {
                    request = new RequestWriter().SetItem("owin.num", 5).SetItem("owin.decimal", 12.34).SetItem("owin.string", "hello!");
                    Assert.That(request.Items["owin.num"], Is.EqualTo(5));
                    Assert.That(request.Items["owin.decimal"], Is.EqualTo(12.34));
                    Assert.That(request.Items["owin.string"], Is.EqualTo("hello!"));
                }

                [Test]
                public void Can_SetBody_with_string() {
                    request = new RequestWriter().SetBody("Hi there");
                    Assert.That(request.Body, Is.EqualTo("Hi there"));
                    Assert.That(request.BodyBytes, Is.EqualTo(Encoding.UTF8.GetBytes("Hi there")));
                }

                [Test]
                public void Can_AddToBody_with_string() {
                    request = new RequestWriter().SetBody("Hi there").SetBody("override");
                    Assert.That(request.Body, Is.EqualTo("override"));
                    Assert.That(request.BodyBytes, Is.EqualTo(Encoding.UTF8.GetBytes("override")));

                    request = new RequestWriter().SetBody("Hi there").AddToBody("override");
                    Assert.That(request.Body, Is.EqualTo("Hi thereoverride"));
                    Assert.That(request.BodyBytes, Is.EqualTo(Encoding.UTF8.GetBytes("Hi thereoverride")));
                }

                [Test]
                public void Can_SetBody_with_bytes() {
                    request = new RequestWriter().SetBody(Encoding.UTF8.GetBytes("Hi there"));
                    Assert.That(request.Body, Is.EqualTo("Hi there"));
                    Assert.That(request.BodyBytes, Is.EqualTo(Encoding.UTF8.GetBytes("Hi there")));
                }

                [Test]
                public void Can_AddToBody_with_bytes() {
                    byte[] first = Encoding.UTF8.GetBytes("first");
                    byte[] second = Encoding.UTF8.GetBytes("second");
                    byte[] firstAndSecond = Encoding.UTF8.GetBytes("firstsecond");

                    request = new RequestWriter().SetBody(first).SetBody(second);
                    Assert.That(request.Body, Is.EqualTo("second"));
                    Assert.That(request.BodyBytes, Is.EqualTo(second));

                    request = new RequestWriter().SetBody(first).AddToBody(second);
                    Assert.That(request.Body, Is.EqualTo("firstsecond"));
                    Assert.That(request.BodyBytes, Is.EqualTo(firstAndSecond));
                }
            }
        }

        [TestFixture]
        public class Reading : RequestWriterSpec {

        }
    }
}