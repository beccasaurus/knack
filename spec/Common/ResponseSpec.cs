using System;
using System.IO;
using System.Net;
using System.Text;
using System.Reflection;
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
				Assert.That(response.Status, Is.EqualTo("404 Not Found"));
			}

			[Test]
			public void Can_make_new_with_body_and_status_code() {
				response = new Response("Oh noes, we don't know that", 404);
				Assert.That(response.BodyText, Is.EqualTo("Oh noes, we don't know that"));
				Assert.That(response.Status, Is.EqualTo("404 NotFound"));
			}

			[Test]
			public void Can_make_new_with_body_and_headers() {
				response = new Response("some json", new Dictionary<string, string> { { "content-type", "application/json" } });
				Assert.That(response.BodyText, Is.EqualTo("some json"));
				Assert.That(response.Headers["content-type"], Is.EqualTo(new string[] { "application/json" }));
			}

			[Test]
			public void Can_make_new_with_status_code_and_headers_as_string_string_dictionary() {
				response = new Response(301, new Dictionary<string, string> { { "location", "/new/path" } });
				Assert.That(response.Status, Is.StringContaining("301 Moved")); // MS.NET: MovedPermanently, Mono: Moved
				Assert.That(response.Headers["location"], Is.EqualTo(new string[] { "/new/path" }));
			}

			[Test]
			public void Can_make_new_with_status_code_and_headers() {
				response = new Response(301, new Dictionary<string, IEnumerable<string>> { { "location", new string[] { "/new/path" } } });
				Assert.That(response.Status, Is.StringContaining("301 Moved"));
				Assert.That(response.Headers["location"], Is.EqualTo(new string[] { "/new/path" }));
			}

			[Test]
			public void Can_make_new_with_body_status_and_headers() {
				response = new Response(404, "We couldn't find that", new Dictionary<string, IEnumerable<string>> { { "content-type", new string[] { "text/plain" } } });
				Assert.That(response.BodyText, Is.EqualTo("We couldn't find that"));
				Assert.That(response.Status, Is.EqualTo("404 NotFound"));
				Assert.That(response.Headers["content-type"], Is.EqualTo(new string[] { "text/plain" }));
			}

			[Test]
			public void Can_make_new_with_body_status_and_headers_as_string_string_dictionary() {
				response = new Response(404, "We couldn't find that", new Dictionary<string, string> { { "content-type", "text/plain" } });
				Assert.That(response.BodyText, Is.EqualTo("We couldn't find that"));
				Assert.That(response.Status, Is.EqualTo("404 NotFound"));
				Assert.That(response.Headers["content-type"], Is.EqualTo(new string[] { "text/plain" }));
			}

			[Test]
			public void Can_take_an_existing_IResponse() {
				IResponse resp = new Response(404, "We couldn't find that", new Dictionary<string, string> { { "content-type", "text/plain" } });

				response = new Response(resp);

				Assert.That(response.BodyText, Is.EqualTo("We couldn't find that"));
				Assert.That(response.Status, Is.EqualTo("404 NotFound"));
				Assert.That(response.Headers["content-type"], Is.EqualTo(new string[] { "text/plain" }));
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
			public void Can_write_to_body_passing_object_to_interpolate() {
				response.Write("Hi {0} There {1} Crazy {2} Person {3}", 1, "neat", 12.34, '!');
				Assert.That(response.BodyText, Is.EqualTo("Hi 1 There neat Crazy 12.34 Person !"));
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

			[TestFixture]
			public class Fluent : ResponseSpec {

				[Test][Ignore]
				public void Can_set_cookie(){}

				[Test]
				public void Can_SetHeader_with_string() {
					response = new Response().SetHeader("content-type", "application/json");
					Assert.That(response.Headers["content-type"], Is.EqualTo(new string[] { "application/json" }));
				}

				[Test]
				public void Can_redirect_to_a_path_using_302_by_default() {
					response = new Response().Redirect("/go/here");
					Assert.That(response.StatusCode,            Is.EqualTo(302));
					Assert.That(response.GetHeader("location"), Is.EqualTo("/go/here"));
				}

				[Test]
				public void Can_redirect_to_a_path_specifying_the_status_code() {
					response = new Response().Redirect(301, "/here");
					Assert.That(response.StatusCode,            Is.EqualTo(301));
					Assert.That(response.GetHeader("location"), Is.EqualTo("/here"));
				}

				[Test][Ignore]
				public void Can_redirect_to_a_path_relative_to_the_BasePath() {}

				[Test][Ignore]
				public void Can_easily_get_a_path_relative_to_the_BasePath() {}

				[Test]
				public void Can_SetHeader_with_IEnumerable_string() {
					response = new Response().SetHeader("content-type", new string[] { "application/xml" });
					Assert.That(response.Headers["content-type"], Is.EqualTo(new string[] { "application/xml" }));
				}

				[Test]
				public void Can_AddHeader_which_adds_to_the_IEnumerable_with_string() {
					response = new Response().SetHeader("content-type", "application/json").SetHeader("content-type", "application/second");
					Assert.That(response.Headers["content-type"], Is.EqualTo(new string[] { "application/second" }));

					// Now use AddHeader, and both values should be present
					response = new Response().SetHeader("content-type", "application/json").AddHeader("content-type", "application/second");
					Assert.That(response.Headers["content-type"], Is.EqualTo(new string[] { "application/json", "application/second" }));
				}

				[Test]
				public void Can_AddHeader_which_adds_to_the_IEnumerable_with_IEnumerable_string() {
					response = new Response().SetHeader("content-type", new string[] { "application/json" }).SetHeader("content-type", new string[] { "application/second" });
					Assert.That(response.Headers["content-type"], Is.EqualTo(new string[] { "application/second" }));

					// Now use AddHeader, and both values should be present
					response = new Response().SetHeader("content-type", new string[] { "application/json" }).AddHeader("content-type", new string[] { "application/second" });
					Assert.That(response.Headers["content-type"], Is.EqualTo(new string[] { "application/json", "application/second" }));
				}

				[Test]
				public void Can_SetStatus_with_an_int() {
					response = new Response().SetStatus(200);
					Assert.That(response.Status, Is.EqualTo("200 OK"));

					response = new Response().SetStatus(404);
					Assert.That(response.Status, Is.EqualTo("404 NotFound"));
				}

				[Test]
				public void Can_SetStatus_with_a_string() {
					response = new Response().SetStatus("200 Totally Friggin OK");
					Assert.That(response.Status, Is.EqualTo("200 Totally Friggin OK"));
				}

				[Test]
				public void Can_SetBody_with_a_string() {
					response = new Response().SetBody("Hello World");
					List<object> body = response.GetBody().AsList();
					Assert.That(body.Count, Is.EqualTo(1));
					Assert.That(body[0], Is.EqualTo("Hello World"));
				}

				[Test]
				public void Can_SetBody_with_a_byte_array() {
					response = new Response().SetBody(new byte[] { (byte)1, (byte)2 });
					List<object> body = response.GetBody().AsList();
					Assert.That(body.Count, Is.EqualTo(1));
					Assert.That(body[0], Is.EqualTo(new byte[] { (byte)1, (byte)2 }));
				}

				[Test]
				public void Can_SetBody_with_an_ArraySegment_of_bytes() {
					byte[] bytes = new byte[10];
					ArraySegment<byte> segment = new ArraySegment<byte>(bytes, 0, 5);
					response = new Response().SetBody(segment);
					List<object> body = response.GetBody().AsList();
					Assert.That(body.Count, Is.EqualTo(1));
					Assert.That(body[0], Is.EqualTo(segment));
				}

				[Test]
				public void Can_SetBody_with_a_FileInfo() {
					FileInfo info = new FileInfo(Assembly.GetExecutingAssembly().Location);
					response = new Response().SetBody(info);
					List<object> body = response.GetBody().AsList();
					Assert.That(body.Count, Is.EqualTo(1));
					Assert.That(body[0], Is.EqualTo(info));
				}

				[Test]
				public void Can_SetBody_with_any_number_of_objects() {
					response = new Response().SetBody("first", "second");
					List<object> body = response.GetBody().AsList();
					Assert.That(body.Count, Is.EqualTo(2));
					Assert.That(body[0], Is.EqualTo("first"));
					Assert.That(body[1], Is.EqualTo("second"));

					FileInfo info = new FileInfo(Assembly.GetExecutingAssembly().Location);
					response = new Response().SetBody("first", info, "another string");
					body = response.GetBody().AsList();
					Assert.That(body.Count, Is.EqualTo(3));
					Assert.That(body[0], Is.EqualTo("first"));
					Assert.That(body[1], Is.EqualTo(info));
					Assert.That(body[2], Is.EqualTo("another string"));
				}

				[Test]
				public void Can_SetBody_with_an_IEnumerable_of_objects() {
					FileInfo info = new FileInfo(Assembly.GetExecutingAssembly().Location);
					IEnumerable<object> objects = new object[] { "a string", info };
					response = new Response().SetBody(objects);
					List<object> body = response.GetBody().AsList();
					Assert.That(body.Count, Is.EqualTo(2));
					Assert.That(body[0], Is.EqualTo("a string"));
					Assert.That(body[1], Is.EqualTo(info));

					// Trying to break it by passing a byte[] as the first argument, which might be an IEnumerable<object>?
					//
					// If you only pass one argument and it's an IEnumerable<object>, we use that as the body
					byte[] bytes = new byte[] { (byte)1, (byte)2 };
					response = new Response().SetBody(bytes);
					body = response.GetBody().AsList();
					Assert.That(body.Count, Is.EqualTo(1));
					Assert.That(body[0], Is.EqualTo(bytes));
				}

				[Test]
				public void Can_AddToBody_with_an_object() {
					response = new Response().SetBody("first").SetBody("second");
					List<object> body = response.GetBody().AsList();
					Assert.That(body.Count, Is.EqualTo(1));
					Assert.That(body[0], Is.EqualTo("second"));

					response = new Response().SetBody("first").AddToBody("second");
					body = response.GetBody().AsList();
					Assert.That(body.Count, Is.EqualTo(2));
					Assert.That(body[0], Is.EqualTo("first"));
					Assert.That(body[1], Is.EqualTo("second"));
				}

				[Test]
				public void Can_AddToBody_with_objects() {
					FileInfo info = new FileInfo(Assembly.GetExecutingAssembly().Location);
					response = new Response().SetBody("first").AddToBody("second").AddToBody(info, "another string");
					List<object> body = response.GetBody().AsList();
					Assert.That(body.Count, Is.EqualTo(4));
					Assert.That(body[0], Is.EqualTo("first"));
					Assert.That(body[1], Is.EqualTo("second"));
					Assert.That(body[2], Is.EqualTo(info));
					Assert.That(body[3], Is.EqualTo("another string"));
				}

				[Test]
				public void Can_AddToBody_with_an_IEnumerable_of_objects() {
					FileInfo info = new FileInfo(Assembly.GetExecutingAssembly().Location);
					IEnumerable<object> objects = new object[] { "a string", info };
					response = new Response().SetBody("first").AddToBody(objects).AddToBody("last");
					List<object> body = response.GetBody().AsList();
					Assert.That(body.Count, Is.EqualTo(4));
					Assert.That(body[0], Is.EqualTo("first"));
					Assert.That(body[1], Is.EqualTo("a string"));
					Assert.That(body[2], Is.EqualTo(info));
					Assert.That(body[3], Is.EqualTo("last"));
				}
			}
		}

		[TestFixture]
		public class Reading : ResponseSpec { // TODO add examples of helpers that Owin.Response has for reading things like headers, body, etc

		}
	}

	static class ResponseSpecExtensions {
		public static List<object> AsList(this IEnumerable<object> enumerable) {
			return enumerable.AsList<object>();
		}
		public static List<T> AsList<T>(this IEnumerable<T> enumerable) {
			return new List<T>(enumerable);
		}
	}
}
