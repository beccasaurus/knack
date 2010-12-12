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
	#endregion
    }
}
