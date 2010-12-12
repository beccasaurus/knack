using System;
using System.Web;
using System.Collections.Generic;
using Owin;

namespace AspNetAppSample {
	public class MyApp : AspNetApplication, IHttpHandler {
		public override IResponse Call(IRequest rawRequest) {
			Request  request  = new Request(rawRequest);
			Response response = new Response();
		
			response.ContentType = "text/plain";
			response.Write("{0} {1}\n\n", request.Method, request.Uri);
			
			foreach (KeyValuePair<string,string> param in request.Params)
				response.Write("Param {0} = {1}\n", param.Key, param.Value);
				
			foreach (KeyValuePair<string,IEnumerable<string>> header in request.Headers)
				foreach (string value in header.Value)
					response.Write("Header {0} = {1}\n", header.Key, value);
		
			return response;
		}
	}
}