using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Owin;

namespace Owin.Handlers {

	public class Cgi {

		public static void Run(IApplication application) {
			string requestInput   = Console.In.ReadToEnd();
			IDictionary variables = Environment.GetEnvironmentVariables();
			CgiRequest request    = new CgiRequest(variables, requestInput);
			IResponse response    = Application.Invoke(application, request);
			string output         = GetResponseText(response);

			Console.Write(output);
		}

		public static string GetResponseText(IResponse response) {
			StringBuilder builder = new StringBuilder();

			// Status
			builder.AppendFormat("Status: {0}\n", response.Status);

			// Headers
			foreach (KeyValuePair<string,IEnumerable<string>> header in response.Headers)
				foreach (string value in header.Value)
				builder.AppendFormat("{0}: {1}\n", header.Key, value);

			builder.Append("\n");

			// Body ... just supports a string body for now ... next up: FileInfo support?
			foreach (object bodyPart in response.GetBody())
				if (bodyPart is string)
					builder.Append(bodyPart as string);
				else if (bodyPart is byte[])
					throw new NotImplementedException("TODO test CGI byte[] body output"); //builder.Append(Encoding.UTF8.GetString(bodyPart)); // assume UTF8 encoding for now ...
				else if (bodyPart is ArraySegment<byte>)
					throw new NotImplementedException("TODO test CGI ArraySegment<byte> body output");
				else if (bodyPart is FileInfo)
					throw new NotImplementedException("TODO test CGI FileInfo body output");
				else
					throw new FormatException("Unknown object returned by IResponse.GetBody(): " + bodyPart.GetType().Name);

			return builder.ToString();
		}
	}
}
