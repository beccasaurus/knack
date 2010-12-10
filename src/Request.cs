using System;
using System.Net;
using System.Web;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Owin {
    
	public class InvalidRequestException : Exception {
	    public InvalidRequestException(string message) : base(message) {}
	}

	// ...
	public class ParamsDictionary<TKey,TValue> : Dictionary<TKey,TValue>, IDictionary<TKey,TValue> {
	    public ParamsDictionary() : base() {}

	    public TValue this[TKey key] {
		get {
		    try {
			return base[key];
		    } catch (KeyNotFoundException) {
			return default (TValue);
		    }
		}
	    }
	}

	public class Request : IRequest {

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

		public string Url {
		    get {
			string url = Scheme + "://" + Host;

			// If the port is non-standard, include it in the Url
			if ((Scheme == "http" && Port != 80) || (Scheme == "https" && Port != 443))
			    url += ":" + Port.ToString();

			url += BasePath + Uri;
			return url;
		    }
		}

		// this is a crappy name ... what is this, really?  we might want to rename this property
		public string BasePath {
		    get { return InnerRequest.Items["owin.base_path"].ToString(); }
		}

		public string Scheme {
		    get { return InnerRequest.Items["owin.url_scheme"].ToString(); }
		}

		public string Host {
		    get { return InnerRequest.Items["owin.server_name"].ToString(); } // TODO return Host: header if definted?
		}

		public int Port {
		    get { return int.Parse(InnerRequest.Items["owin.server_port"].ToString()); }
		}

		/// <summary>Get the raw value of the QueryString</summary>
		/// <remarks>
		/// In OWIN, we include the QueryString in the Uri if it was provided.
		///
		/// This grabs the QueryString from the Uri unless the server provides 
		/// us with a QUERY_STRING.
		/// </remarks>
		public string QueryString {
		    get { return new Uri(Url).Query.Replace("?", ""); }
		}

		public IDictionary<string, string> GET {
		    get {
			IDictionary<string, string> get = new ParamsDictionary<string, string>();
			NameValueCollection queryStrings = HttpUtility.ParseQueryString(QueryString);
			foreach (string key in queryStrings)
			    get.Add(key, queryStrings[key]);
			return get;
		    }
		}

		public IDictionary<string, IEnumerable<string>> Headers { get; set; }
		public IDictionary<string, object> Items { get; set; }
		public IAsyncResult BeginReadBody(byte[] buffer, int offset, int count, AsyncCallback callback, object state){ return null; }
		public int EndReadBody(IAsyncResult result){ return 0; }
	}
}
