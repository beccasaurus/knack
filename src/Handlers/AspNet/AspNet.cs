using System;
using System.Web;
using System.Collections;
using System.Collections.Generic;
using Owin;

namespace Owin.Handlers {

	public class AspNet {

		public static void WriteResponse(HttpResponse aspNetResponse, IResponse owinResponse) {
			Response response = new Response(owinResponse);

			aspNetResponse.Status = response.Status;
			aspNetResponse.Write(response.BodyText);

			// if you try to access Headers directly, you'll get:
			//    PlatformNotSupportedException: This operation requires IIS integrated pipeline mode.
			//
			// you *can* call AddHeader() however
			//
			foreach (KeyValuePair<string,IEnumerable<string>> header in response.Headers)
				foreach (string value in header.Value)
					aspNetResponse.AddHeader(header.Key, value);
		}
	}
}
