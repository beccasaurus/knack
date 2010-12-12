using System;
using System.Collections.Generic;
using System.Text;
using Owin;

namespace Owin.Test {

    public class MockSession {

	public MockSession() {}

	public MockSession(IApplication application) {
	    App = application;
	}

	public IApplication App          { get; set; }
	public MockRequest  LastRequest  { get; set; }
	public MockResponse LastResponse { get; set; }

	public MockResponse GetResponse(IRequest request) {
	    return GetResponse(new MockRequest(request));
	}

	public MockResponse GetResponse(MockRequest request) {
	    LastRequest  = request;
	    LastResponse = new MockResponse(Application.Invoke(App, request));
	    return LastResponse;
	}

	// TODO when we integrate Owin.Session's using cookies and whatnot, this should reset the session
	public MockSession ResetSession() {
	    LastRequest  = null;
	    LastResponse = null;
	    return this;
	}

	// Proxy calls to MockRequest which provides static helper methods for all of these common requests
	public MockResponse Get(   string uri)                                      { return GetResponse(MockRequest.Get(uri)); }
	public MockResponse Post(  string uri)                                      { return GetResponse(MockRequest.Post(uri)); }
	public MockResponse Post(  string uri, string postData)                     { return GetResponse(MockRequest.Post(uri, postData)); }
	public MockResponse Post(  string uri, IDictionary<string,string> postData) { return GetResponse(MockRequest.Post(uri, postData)); }
	public MockResponse Put(   string uri)                                      { return GetResponse(MockRequest.Put(uri)); }
	public MockResponse Put(   string uri, string postData)                     { return GetResponse(MockRequest.Put(uri, postData)); }
	public MockResponse Put(   string uri, IDictionary<string,string> postData) { return GetResponse(MockRequest.Put(uri, postData)); }
	public MockResponse Delete(string uri)                                      { return GetResponse(MockRequest.Delete(uri)); }
	public MockResponse Delete(string uri, string postData)                     { return GetResponse(MockRequest.Delete(uri, postData)); }
	public MockResponse Delete(string uri, IDictionary<string,string> postData) { return GetResponse(MockRequest.Delete(uri, postData)); }
    }

    // Why do these Mock classes exist if all they do is wrap the normal Owin classes?
    //
    // Well, eventually, it'll be useful to add functionality that's *specific* to testing 
    // that we wouldn't really want/need on the normal Owin classes.  So that's why!

    public class MockRequest : RequestWriter, IRequest {
	public MockRequest()                                       : base(){}
	public MockRequest(string uri)                             : base(uri){}
	public MockRequest(string method, string uri)              : base(method, uri){}
	public MockRequest(string method, string uri, string body) : base(method, uri, body){}
	public MockRequest(string method, string uri, byte[] body) : base(method, uri, body){}
	public MockRequest(IRequest request)                       : base(request){}

	public MockResponse GetResponse(IApplication application) {
	    return new MockResponse(Application.Invoke(application, this));
	}

	public MockRequest MarkAsUrlEncoded() {
	    return SetContentType("application/x-www-form-urlencoded") as MockRequest;
	}

	// static helper methods for getting MockRequest instances
	
	public static MockRequest Get(string uri) {
	    return new MockRequest("GET", uri);
	}
	// Get with query string
	
	public static MockRequest Post(string uri) {
	    return new MockRequest("POST", uri);
	}
	public static MockRequest Post(string uri, string postData) {
	    return new MockRequest("POST", uri).SetBody(postData) as MockRequest;
	}
	public static MockRequest Post(string uri, IDictionary<string,string> postData) {
	    return new MockRequest("POST", uri).SetBody(postData) as MockRequest;
	}
	
	public static MockRequest Put(string uri) {
	    return new MockRequest("PUT", uri);
	}
	public static MockRequest Put(string uri, string postData) {
	    return new MockRequest("PUT", uri).MarkAsUrlEncoded().SetBody(postData) as MockRequest;
	}
	public static MockRequest Put(string uri, IDictionary<string,string> postData) {
	    return new MockRequest("PUT", uri).MarkAsUrlEncoded().SetBody(postData) as MockRequest;
	}
	
	public static MockRequest Delete(string uri) {
	    return new MockRequest("DELETE", uri);
	}
	public static MockRequest Delete(string uri, string postData) {
	    return new MockRequest("DELETE", uri).MarkAsUrlEncoded().SetBody(postData) as MockRequest;
	}
	public static MockRequest Delete(string uri, IDictionary<string,string> postData) {
	    return new MockRequest("DELETE", uri).MarkAsUrlEncoded().SetBody(postData) as MockRequest;
	}
    }

    public class MockResponse : Response, IResponse {
	public MockResponse(IResponse response) : base(response){}
    }
}
