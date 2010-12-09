using System;
using System.Net;
using System.Web;
using System.Collections.Generic;

namespace Owin {
    
	public class InvalidRequestException : Exception {
	    public InvalidRequestException(string message) : base(message) {}
	}

	public class Request : IRequest {

		//readonly string[] requiredItems = new string[] { "owin.base_path", "owin.server_name", "owin.server_port", 
		//					      "owin.request_protocol", "owin.url_scheme", "owin.remote_endpoint" };

		public IRequest InnerRequest;

		public Request(IRequest innerRequest) {
			InnerRequest = innerRequest;
		}

		public string Method {
			get { return InnerRequest.Method; }
		}

		public string Uri {
			get { return InnerRequest.Uri; }
		}

		/// <summary>Get the raw value of the QueryString</summary>
		/// <remarks>
		/// In OWIN, we include the QueryString in the Uri if it was provided.
		///
		/// This grabs the QueryString from the Uri unless the server provides 
		/// us with a QUERY_STRING.
		/// </remarks>
		public string QueryString {
		    get {
			return null;
		    }
		}

		public IDictionary<string, string> GET {
		    get {
			IDictionary<string, string> get = new Dictionary<string, string>();

			if (QueryString == null)
			    return get;

			//foreach (string part in QueryString.Split("&"))

			return get;
		    }
		}

		public IDictionary<string, IEnumerable<string>> Headers { get; set; }
		public IDictionary<string, object> Items { get; set; }
		public IAsyncResult BeginReadBody(byte[] buffer, int offset, int count, AsyncCallback callback, object state){ return null; }
		public int EndReadBody(IAsyncResult result){ return 0; }
	}
}
