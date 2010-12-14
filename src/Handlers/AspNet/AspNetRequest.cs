using System;
using System.Net;
using System.Web;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Owin;

namespace Owin.Handlers {

	public class AspNetRequest : RequestWriter, IRequest {

		HttpRequest NativeRequest;

		public AspNetRequest(HttpRequest nativeRequest) : base () {
			NativeRequest = nativeRequest;
			SetItems();
			SetHeaders();
		}

		public override string Method {
			get { return NativeRequest.RequestType; }
		}

		public override string Uri {
			get { return NativeUri.PathAndQuery; }
		}

		public override string Url {
			get { return NativeUri.ToString(); }
		}

		public override IDictionary<string, string> POST {
			get {
				IDictionary<string, string> post = new ParamsDictionary<string, string>();
				foreach (string key in NativeRequest.Form)
					post.Add(key, NativeRequest.Form[key]);
				return post;
			}
		}

#region Private
		Uri NativeUri {
			get { return NativeRequest.Url; }
		}

		void SetItems() {
			Items["owin.base_path"]        = NativeRequest.ApplicationPath ?? "";
			Items["owin.server_name"]      = NativeUri.Host;
			Items["owin.server_port"]      = NativeUri.Port;
			Items["owin.request_protocol"] = "HTTP/1.1";
			Items["owin.url_scheme"]       = NativeUri.Scheme;

			if (NativeRequest.UserHostAddress != null)
				Items["owin.remote_endpoint"]  = new IPEndPoint(IPAddress.Parse(NativeRequest.UserHostAddress), Port);
			else
				Items["owin.remote_endpoint"]  = new IPEndPoint(IPAddress.Parse("0.0.0.0"), Port);
		}

		void SetHeaders() {
			foreach (string key in NativeRequest.Headers.AllKeys)
				SetHeader(key, NativeRequest.Headers[key]);
		}
#endregion
	}
}
