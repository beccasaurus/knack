using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Owin {

    public class Response : IResponse {

#region Constructors

	public Response() {
	    SetValidDefaults();
	}

	public Response(string bodyText, string status) : this(bodyText) {
	    Status = status;
	}

	public Response(string bodyText) : this() {
	    BodyText = bodyText;
	}
	
	public Response(string bodyText, int statusCode) : this() {
	    BodyText = bodyText;
	    SetStatus(statusCode);
	}

	public Response(string bodyText, IDictionary<string,string> headers)              : this(0, bodyText, headers)       {}
	public Response(string bodyText, IDictionary<string,IEnumerable<string>> headers) : this(0, bodyText, headers)       {}
	public Response(int statusCode, IDictionary<string,string> headers)               : this(statusCode, null, headers)  {}
	public Response(int statusCode, IDictionary<string,IEnumerable<string>> headers)  : this(statusCode, null, headers)  {}

	public Response(int statusCode, string bodyText, IDictionary<string,string> headers) : this() {
	    if (statusCode != 0)    SetStatus(statusCode);
	    if (bodyText   != null) BodyText = bodyText;
	    if (headers    != null) AddHeaders(headers);
	}

	public Response(int statusCode, string bodyText, IDictionary<string,IEnumerable<string>> headers) : this() {
	    if (statusCode != 0)    SetStatus(statusCode);
	    if (bodyText   != null) BodyText = bodyText;
	    if (headers    != null) AddHeaders(headers);
	}

#endregion

#region Status

        public string Status { get; set; }

	public int StatusCode {
	    get { return int.Parse(Status.Substring(0, Status.IndexOf(" "))); }
	}

	public string StatusMessage {
	    get { return Status.Substring(Status.IndexOf(" ") + 1); }
	}

	public Response SetStatus(int statusCode) {
	    string statusMessage = ((HttpStatusCode) statusCode).ToString();
	    Status = string.Format("{0} {1}", statusCode, statusMessage);
	    return this;
	}

	public Response SetStatus(string status) {
	    Status = status;
	    return this;
	}

#endregion

#region Body

	// Allowed object types: string, byte[], ArraySegment<byte>, FileInfo
        public IEnumerable<object> GetBody() {
	    return Body;
	}

	/// <summary>Set the body to one or many objects, overriding any other values the body may have</summary>
	public Response SetBody(params object[] objects) {
	    if (objects.Length == 1 && objects[0] is IEnumerable<object>)
		Body = objects[0] as IEnumerable<object>;
	    else
		Body = objects;
	    return this;
	}

	/// <summary>Set the body to one or many objects, adding to any other values the body may have</summary>
	public Response AddToBody(params object[] objects) {
	    IEnumerable<object> stuffToAdd = objects;
	    if (objects.Length == 1 && objects[0] is IEnumerable<object>)
		stuffToAdd = objects[0] as IEnumerable<object>;

	    List<object> allObjects = new List<object>(Body);
	    allObjects.AddRange(stuffToAdd);
	    Body = allObjects;
	    return this;
	}

        public IEnumerable<object> Body { get; set; }

	public string BodyText {
	    get {
		string text = "";
		foreach (object bodyPart in Body) {
		    if (bodyPart is string)
			text += bodyPart.ToString();
		    else
			throw new FormatException("Cannot get BodyText unless Body only contains strings.  Body contains: " + bodyPart.GetType().Name);
		}
		return text;
	    }
	    set {
		Body = new object[] { value };
		ContentLength = value.Length;
	    }
	}

	// might swap this out with a string builder ... 
	// it's tough because we should be able to write bytes as well!
	public void Write(string writeToBody) {
	    BodyText += writeToBody;
	}

	public void Clear() {
	    BodyText = "";
	}

#endregion

#region Headers

        public IDictionary<string, IEnumerable<string>> Headers { get; set; }

	/// <summary>Set header with a string, overriding any other values this header may have</summary>
	public Response SetHeader(string key, string value) {
	    Headers[key] = new string[] { value };
	    return this;
	}

	/// <summary>Set header, overriding any other values this header may have</summary>
	public Response SetHeader(string key, IEnumerable<string> value) {
	    Headers[key] = value;
	    return this;
	}

	/// <summary>Set header with a string, adding to any other values this header may have</summary>
	public Response AddHeader(string key, string value) {
	    if (Headers.ContainsKey(key)) {
		List<string> listOfValues = new List<string>(Headers[key]);
		listOfValues.Add(value);
		SetHeader(key, listOfValues.ToArray());
	    } else
		SetHeader(key, value);
	    return this;
	}

	/// <summary>Set header, adding to any other values this header may have</summary>
	public Response AddHeader(string key, IEnumerable<string> value) {
	    if (Headers.ContainsKey(key)) {
		List<string> listOfValues = new List<string>(Headers[key]);
		listOfValues.AddRange(value);
		SetHeader(key, listOfValues.ToArray());
	    } else
		SetHeader(key, value);
	    return this;
	}

	public string ContentType {
	    get { return HeaderOrNull("content-type"); }
	    set { SetHeader("content-type", value); }
	}

	public int ContentLength {
	    get {
		string length = HeaderOrNull("content-length");
		return (length == null) ? 0 : int.Parse(length);
	    }
	    set { SetHeader("content-length", value.ToString()); }
	}

#endregion

#region Private

	void SetValidDefaults() {
	    Status      = "200 OK";
	    Headers     = new Dictionary<string, IEnumerable<string>>();
	    Body        = new object[] { "" };
	    ContentType = "text/html";
	}

	void AddHeaders(IDictionary<string,string> headers) {
	    foreach (KeyValuePair<string,string> header in headers)
		Headers[header.Key] = new string[] { header.Value };
	}

	void AddHeaders(IDictionary<string,IEnumerable<string>> headers) {
	    foreach (KeyValuePair<string, IEnumerable<string>> header in headers)
		Headers[header.Key] = header.Value;
	}
	
	// If this header has multiple values, we return the first
	string HeaderOrNull(string key) {
	    if (Headers.ContainsKey(key)) {
		string value = null;
		foreach (string headerValue in Headers[key]) {
		    value = headerValue;
		    break;
		}
		return value;
	    } else
		return null;
	}
#endregion
    }
}
