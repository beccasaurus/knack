using System;
using System.Net;
using System.Web;
using System.Text;
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

	#region IRequest implementation which simply wraps InnerRequest
	// It makes sense for Owin.Request to implement IRequest.
	// We may eventually override some of the IRequest implementation 
	// but, for now, we simply proxy everything to the InnerRequest
	public string Method { get { return InnerRequest.Method; } }
	public string Uri { get { return InnerRequest.Uri; } }
	public IDictionary<string, IEnumerable<string>> Headers { get { return InnerRequest.Headers; } }
	public IDictionary<string, object> Items { get { return InnerRequest.Items; } }
	public IAsyncResult BeginReadBody(byte[] buffer, int offset, int count, AsyncCallback callback, object state) {
	    return InnerRequest.BeginReadBody(buffer, offset, count, callback, state);
	}
	public int EndReadBody(IAsyncResult result){ return InnerRequest.EndReadBody(result); }
	#endregion

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

	// blocks while it gets the full body
	public string Body {
	    get { return Encoding.UTF8.GetString(BodyBytes); }
	}

	// blocks while it gets the full body
	public byte[] BodyBytes {
	    get { return GetAllBodyBytes(); }
	}

	public byte[] GetAllBodyBytes() {
	    return GetAllBodyBytes(1000); // how many bytes to get per call to BeginReadBody
	}

	public byte[] GetAllBodyBytes(int batchSize) {
	    List<byte> allBytes = new List<byte>();
	    bool done           = false;
	    int offset          = 0;

	    while (! done) {
		byte[] buffer       = new byte[batchSize];
		IAsyncResult result = InnerRequest.BeginReadBody(buffer, offset, batchSize, null, null);
		int bytesRead       = InnerRequest.EndReadBody(result);

		if (bytesRead == 0)
		    done = true;
		else {
		    offset += batchSize;
		    allBytes.AddRange(buffer);
		}
	    }

	    return RemoveTrainingBytes(allBytes.ToArray());
	}

	byte[] RemoveTrainingBytes(byte[] bytes) {
	    int i = bytes.Length - 1;
	    while (bytes[i] == 0)
		i--;

	    if (i == 0)
		return bytes;
	    else {
		byte[] newBytes = new byte[i+1];
		Array.Copy(bytes, newBytes, i+1);
		return newBytes;
	    }
	}
    }
}
