using System;
using System.Net;
using System.Web;
using System.Text;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Owin {

    /// <summary>Helps you create valid IRequest objects</summary>
    public class RequestWriter : Request, IRequest {

        #region Constructors
        public RequestWriter() {
            // All of Request's helper methods that wrap InnerRequest will use our instance as the InnerRequest
            InnerRequest = this;

            // Set up the delegate, configuring the ReadTheBody method as the method that *actually* reads the body, 
            // but it's wrapped up in a delegate (ReadTheBodyDelegate) to allow for asynchronous calls
            ActuallyReadTheBody = new ReadTheBodyDelegate(ReadTheBody);

            SetValidDefaults();
        }

        public RequestWriter(string uri)
            : this() {
            Uri = uri;
        }

        public RequestWriter(string method, string uri)
            : this() {
            Method = method;
            Uri = uri;
        }

        public RequestWriter(string method, string uri, string body)
            : this() {
            Method = method;
            Uri = uri;
            TheRealBody = body;
        }

        public RequestWriter(string method, string uri, byte[] body)
            : this() {
            Method = method;
            Uri = uri;
            TheRealBodyBytes = body;
        }
        #endregion

        #region Method
        public new string Method { get; set; }

        public RequestWriter SetMethod(string method) {
            Method = method;
            return this;
        }
        #endregion

        #region Uri
        public new string Uri { get; set; }

        public RequestWriter SetUri(string uri) {
            Uri = uri;
            return this;
        }
        #endregion

        #region Headers
        public new IDictionary<string, IEnumerable<string>> Headers { get; set; }

        /// <summary>Set header with a string, overriding any other values this header may have</summary>
        public RequestWriter SetHeader(string key, string value) {
            Headers[key] = new string[] { value };
            return this;
        }

        /// <summary>Set header, overriding any other values this header may have</summary>
        public RequestWriter SetHeader(string key, IEnumerable<string> value) {
            Headers[key] = value;
            return this;
        }

        /// <summary>Set header with a string, adding to any other values this header may have</summary>
        public RequestWriter AddHeader(string key, string value) {
            if (Headers.ContainsKey(key)) {
                List<string> listOfValues = new List<string>(Headers[key]);
                listOfValues.Add(value);
                SetHeader(key, listOfValues.ToArray());
            } else
                SetHeader(key, value);
            return this;
        }

        /// <summary>Set header, adding to any other values this header may have</summary>
        public RequestWriter AddHeader(string key, IEnumerable<string> value) {
            if (Headers.ContainsKey(key)) {
                List<string> listOfValues = new List<string>(Headers[key]);
                listOfValues.AddRange(value);
                SetHeader(key, listOfValues.ToArray());
            } else
                SetHeader(key, value);
            return this;
        }

        // TODO test
        // public string ContentType {
        //     get { return HeaderOrNull("content-type"); }
        //     set { SetHeader("content-type", value); }
        // }
        #endregion

        #region Items
        public new IDictionary<string, object> Items { get; set; }

        public RequestWriter SetItem(string key, object value) {
            Items[key] = value;
            return this;
        }
        #endregion

        #region Body
        byte[] TheRealBodyBytes { get; set; }
        string TheRealBody {
            get { return Encoding.UTF8.GetString(TheRealBodyBytes); } // TODO should be able to change the encoding used (?)
            set { TheRealBodyBytes = Encoding.UTF8.GetBytes(value); }
        }

        public RequestWriter SetBody(string body) {
            TheRealBody = body;
            return this;
        }

        public RequestWriter SetBody(byte[] body) {
            TheRealBodyBytes = body;
            return this;
        }

        public RequestWriter AddToBody(string bodyPart) {
            TheRealBody += bodyPart;
            return this;
        }

        public RequestWriter AddToBody(byte[] bodyPart) {
            // this should probably use something like System.Buffer.BlockCopy to make it speedy if there are lots of bytes
            List<byte> newBytes = new List<byte>(TheRealBodyBytes);
            newBytes.AddRange(bodyPart);
            TheRealBodyBytes = newBytes.ToArray();
            return this;
        }

        public delegate int ReadTheBodyDelegate(byte[] buffer, int offset, int count);

        ReadTheBodyDelegate ActuallyReadTheBody { get; set; }

        // Method that *actually* reads the body
        int ReadTheBody(byte[] buffer, int offset, int count) {
            int bytesRead = 0;
            for (int i = 0; i < count; i++) {
                int index = offset + i;
                if (TheRealBodyBytes == null || index >= TheRealBodyBytes.Length)
                    break; // the TheRealBodyBytes doesn't have this index
                else {
                    bytesRead++;
                    buffer[i] = TheRealBodyBytes[index];
                }
            }
            return bytesRead;
        }

        public override IAsyncResult BeginReadBody(byte[] buffer, int offset, int count, AsyncCallback callback, object state) {
            return ActuallyReadTheBody.BeginInvoke(buffer, offset, count, callback, state);
        }
        public override int EndReadBody(IAsyncResult result) {
            return ActuallyReadTheBody.EndInvoke(result);
        }
        #endregion

        #region Private
        void SetValidDefaults() {
            Method = "GET";
            Uri = "";
            Headers = new Dictionary<string, IEnumerable<string>>();
            Items = new Dictionary<string, object>();
            Items["owin.base_path"] = "";
            Items["owin.server_name"] = "localhost";
            Items["owin.server_port"] = 80;
            Items["owin.request_protocol"] = "HTTP/1.1";
            Items["owin.url_scheme"] = "http";
            Items["owin.remote_endpoint"] = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 80);
        }
        #endregion
    }
}