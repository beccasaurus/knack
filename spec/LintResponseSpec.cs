using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using Owin;
using NUnit.Framework;

namespace Owin.Common.Specs {

    public class ResponseForLint : IResponse {
	public ResponseForLint() {
	    // Make the Response valid
	    Status  = "200 OK";
	    Headers = new Dictionary<string, IEnumerable<string>>();
	    Body    = new object[] {};
	}
        public string Status { get; set; }
        public IEnumerable<object> Body { get; set; }
        public IDictionary<string, IEnumerable<string>> Headers { get; set; }
        public IEnumerable<object> GetBody() {
	    return Body;
	}
    }

    [TestFixture]
    public class LintResponseSpec {

	public class Resp : ResponseForLint {}

	string[] ErrorMessages;
	Resp response;

	[SetUp]
	public void BeforeEach() {
	    response       = new Resp();
	    ErrorMessages = null;
	}

	// "Meta" spec to make sure that a new Req() is always valid;
	[Test]
	public void __new_Resp_is_valid() {
	    AssertValid();
	}

	[Test]
	[ExpectedException(typeof(LintException), ExpectedMessage = "Response was not valid: Status cannot be null")]
	public void Can_raise_exception_if_validation_fails() {
	    response = new Resp { Status = null };
	    Owin.Lint.Validate(response);
	}

	[Test]
	[ExpectedException(typeof(LintException), ExpectedMessage = "Response was not valid: Status cannot be null, Headers cannot be null")]
	public void Can_raise_exception_with_many_messaegs_if_validation_fails() {
	    response = new Resp { Status = null, Headers = null };
	    Owin.Lint.Validate(response);
	}

	[Test]
	public void Status_cannot_be_null_or_blank() {
	    response = new Resp { Status = null };
	    AssertErrorMessage("Status cannot be null");

	    response = new Resp { Status = " " };
	    AssertErrorMessage("Status cannot be blank");

	    response = new Resp { Status = "200 OK" };
	    AssertValid();
	}

	[Test]
	public void Status_can_only_contain_ASCII_characters() {
	    response = new Resp { Status = "200 OK™½Neat" };
	    AssertErrorMessage("Status cannot contain non-ASCII characters: 200 OK™½Neat");
	}

	[Test]
	public void Status_must_start_with_integer_status_followed_by_a_space_and_a_reason_phrase() {
	    response = new Resp { Status = "200" };
	    AssertErrorMessage("Status must include integer status followed by a space and a reason phrase: 200");

	    response = new Resp { Status = "200 " };
	    AssertErrorMessage("Status must include integer status followed by a space and a reason phrase: 200 ");

	    response = new Resp { Status = " Hi" };
	    AssertErrorMessage("Status must include integer status followed by a space and a reason phrase:  Hi");

	    response = new Resp { Status = "200 Foo" };
	    AssertValid();

	    response = new Resp { Status = "200 Foo Bar" };
	    AssertValid();
	}

	[Test]
	public void Status_cannot_contain_a_newline() {
	    response = new Resp { Status = "\n200 OK" };
	    AssertErrorMessage("Status cannot include a newline: \n200 OK");

	    response = new Resp { Status = "200 OK\n" };
	    AssertErrorMessage("Status cannot include a newline: 200 OK\n");
	}

	[Test]
	public void Headers_cannot_be_null() {
	    response = new Resp { Headers = null };
	    AssertErrorMessage("Headers cannot be null");
	}

	[Test]
	public void Headers_keys_cannot_contain_whitespace() {
	    response.Headers["content-type "] = new string[] { "text/html" };
	    AssertErrorMessage("Header keys cannot contain whitespace: content-type ");
	}

	[Test]
	public void Headers_keys_cannot_contain_periods() {
	    response.Headers["content.type"] = new string[] { "text/html" };
	    AssertErrorMessage("Header keys cannot contain periods: content.type");
	}

	[Test]
	public void Headers_keys_cannot_contain_newlines() {
	    response.Headers["content-type\n"] = new string[] { "text/html" };
	    AssertErrorMessage("Header keys cannot contain newlines: content-type\n");
	}

	[Test]
	public void Headers_keys_can_only_contain_ASCII_characters() {
	    response.Headers["content-type™½"] = new string[] { "text/html" };
	    AssertErrorMessage("Header keys cannot contain non-ASCII characters: content-type™½");
	}

	[Test]
	public void Headers_values_cannot_contain_newlines() {
	    response.Headers["content-type"] = new string[] { "text/html\n" };
	    AssertErrorMessage("Header values cannot contain newlines: content-type: text/html\n");
	}

	[Test]
	public void Headers_values_can_only_contain_ASCII_characters() {
	    response.Headers["content-type"] = new string[] { "text/html™½" };
	    AssertErrorMessage("Header values cannot contain non-ASCII characters: content-type: text/html™½");
	}

	[Test]
	public void GetBody_enumerable_must_return_valid_type() {
	    //string
	    response.Body = new object[] { "hi" };
	    AssertValid();

	    // byte[]
	    response.Body = new object[] { new byte[] { (byte) 0x12, (byte) 0x0F } };
	    AssertValid();
	    
	    // ArraySegment<byte>
	    byte[] bytes = new byte[10];
	    response.Body = new object[] { new ArraySegment<byte>(bytes, 0, 5) };
	    AssertValid();

	    // FileInfo
	    response.Body = new object[] { new FileInfo(Assembly.GetExecutingAssembly().Location) };
	    AssertValid();

	    // Now try some other types ...

	    response.Body = new object[] { new string[] { "no", "dogs", "allowed" } };
	    AssertErrorMessage("GetBody() has unsupported type: String[].  Supported types: string, byte[], ArraySegment<byte>, FileInfo");

	    response.Body = new object[] { 1, 2, 3, 4, 5 };
	    AssertErrorMessage("GetBody() has unsupported type: Int32.  Supported types: string, byte[], ArraySegment<byte>, FileInfo");
	}

	[Test][Ignore] public void GetBody_must_be_UTF8_or_raw_or_FileInfo() {} // TODO later

	[Test][Ignore] public void Headers_requires_content_type() {} // <--- not part of the spec but should be! ... enable a Strict mode for these?  or fork?

	// Helper / Assertion methods
	
	void __validateResponse() {
	    ErrorMessages = Owin.Lint.ErrorMessagesFor(response);    
	}

	void AssertValid() {
	    __validateResponse();
	    Assert.That(ErrorMessages.Length, Is.EqualTo(0));
	}

	void AssertErrorMessage(string message) {
	    __validateResponse();
	    Assert.That(ErrorMessages, Has.Member(message));
	}
    }
}
