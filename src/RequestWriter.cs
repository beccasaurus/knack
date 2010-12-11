using System;
using System.Net;
using System.Web;
using System.Text;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Owin {

    /// <summary>Helps you create valid IRequest objects</summary>
    public class RequestWriter : Request, IRequest {

	public RequestWriter() {
	    InnerRequest = this; // All of Request's helper methods that wrap InnerRequest will use our instance as the InnerRequest
	    SetValidDefaults();
	}

	void SetValidDefaults() {
	    Method  = "GET";
	    Uri     = "";
	    Headers = new Dictionary<string, IEnumerable<string>>();
	    Items   = new Dictionary<string, object>();
	    Items["owin.base_path"]        = "";
	    Items["owin.server_name"]      = "localhost";
	    Items["owin.server_port"]      = 80;
	    Items["owin.request_protocol"] = "HTTP/1.1";
	    Items["owin.url_scheme"]       = "http";
	    Items["owin.remote_endpoint"]  = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 80);
	}

	#region IRequest implementation
	public string Method { get; set; }
	public string Uri { get; set; }
	public IDictionary<string, IEnumerable<string>> Headers { get; set; }
	public IDictionary<string, object> Items { get; set; }
	public IAsyncResult BeginReadBody(byte[] buffer, int offset, int count, AsyncCallback callback, object state) {
	    return null;
	}
	public int EndReadBody(IAsyncResult result) {
	    return 0;
	}
	#endregion
    }
}
