using System;
using System.Collections.Generic;

namespace Owin {
	public class Request : IRequest {
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
	}
}
