using System;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using Owin;

namespace Owin.Handlers {

    // might nest this underneath Cgi, so it's Owin.Handlers.Cgi.CgiRequest ...
    public class CgiRequest : RequestWriter, IRequest {

	IDictionary<string,string> ENV { get; set; }

	public CgiRequest(IDictionary environmentVariables) {
	    ENV = new Dictionary<string,string>();
	    foreach (DictionaryEntry entry in environmentVariables)
		ENV[entry.Key.ToString()] = entry.Value.ToString();

	    SetItemsFromENV();
	    ImportHeadersFromENV();
	}

	public override string Uri {
	    get { return RawUri.PathAndQuery.Replace(BasePath, ""); }
	}

	public override string Method {
	    get { return ENV["REQUEST_METHOD"]; }
	}

	public override string Url {
	    get { return ENV["REQUEST_URI"]; }
	}

	// TODO by default, Headers/Items in Request and RequestWriter should be a Dictionary that returns null when you ask for a non-existent key
	//      this behavior makes sense in web application, IMHO.
	public override string ContentType {
	    get {
		// TODO Request.GetHeader() for getting a single value
		if (Headers.ContainsKey("content_type")) {
		    string value = null;
		    foreach (string headerValue in Headers["content_type"]) {
			value = headerValue;
			break;
		    }
		    return value;
		} else
		    return null;
	    }
	}

	#region Private
	Uri RawUri {
	    get { return new Uri(Url); }
	}

	void SetItemsFromENV() {
	    Items["owin.base_path"]        = ENV["SCRIPT_NAME"];
	    Items["owin.server_name"]      = ENV["SERVER_NAME"];
	    Items["owin.server_port"]      = ENV["SERVER_PORT"];
	    Items["owin.request_protocol"] = ENV["SERVER_PROTOCOL"];
	    Items["owin.url_scheme"]       = RawUri.Scheme;
	    Items["owin.remote_endpoint"]  = new IPEndPoint(IPAddress.Parse(ENV["REMOTE_ADDR"]), Port);
	}

	void ImportHeadersFromENV() {
	    foreach (KeyValuePair<string,string> variable in ENV)
		SetHeader(variable.Key, variable.Value);
	}
	#endregion
    }
}
