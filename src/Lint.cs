using System;
using System.Collections.Generic;

namespace Owin {

    public class LintException : Exception {
	public LintException(string message) : base(message) {}
    }

    public class Lint {

	public static void Validate(IRequest request) {
	    string[] requestErrors = ErrorMessagesFor(request);
	    if (requestErrors.Length > 0)
		throw new LintException("Request was not valid: " + string.Join(", ", requestErrors));
	}

	static readonly List<string> RequestItemsThatCannotBeNull = new List<string> {
	    "owin.base_path", "owin.server_name", "owin.server_port", "owin.request_protocol", "owin.remote_endpoint"
	};

	static readonly List<string> RequestItemsThatCannotBeBlank = new List<string> {
	    "owin.server_name", "owin.server_port", "owin.request_protocol"
	};

	static readonly List<string> ValidRequestProtocols = new List<string> { "HTTP/1.0", "HTTP/1.1" };

	static readonly List<string> ValidUrlSchemes = new List<string> { "http", "https" };

	public static string[] ErrorMessagesFor(IRequest request) {
	    List<string> errors = new List<string>();

	    if      (request.Method.IsNull())       errors.Add("Method cannot be null");
	    else if (request.Method.IsWhiteSpace()) errors.Add("Method cannot be blank");

	    if (request.Uri.IsNull()) errors.Add("Uri cannot be null");

	    if (request.Items == null)
		errors.Add("Items cannot be null"); // TODO TEST
	    else if (request.Items.Count == 0)
		errors.Add("Items cannot be empty"); // TODO TEST
	    else {
		// Check for missing items
		foreach (string itemName in RequestItemsThatCannotBeNull)
		    if (request.Items.DoesNotHaveKey(itemName))
			errors.Add("Items missing required key: " + itemName);

		// Validate existing items
		foreach (KeyValuePair<string,object> item in request.Items) {
		    string key   = item.Key;
		    object value = item.Value;

		    if (RequestItemsThatCannotBeNull.Contains(key) && request.Items[key].IsNull())
			errors.Add(string.Format("Items[\"{0}\"] cannot be null", key));
		    else if (RequestItemsThatCannotBeBlank.Contains(key) && request.Items[key].IsNullOrWhiteSpace())
			errors.Add(string.Format("Items[\"{0}\"] cannot be blank", key));
		    else {
			switch (key) {
			    case "owin.server_port":
				if (value.IsNotAnInteger())
				    errors.Add("Items[\"owin.server_port\"] must be an integer");
				break;
			    case "owin.request_protocol":
				if (! ValidRequestProtocols.Contains(value.ToString()))
				    errors.Add(string.Format("Items[\"owin.request_protocol\"] protocol is unknown: {0}.  Must be HTTP/1.0 or HTTP/1.1", value));
				break;
			    case "owin.url_scheme":
				if (! ValidUrlSchemes.Contains(value.ToString()))
				    errors.Add(string.Format("Items[\"owin.url_scheme\"] scheme is unknown: {0}.  Must be http or https", value));
				break;
			    case "owin.remote_endpoint":
				if (! (value is System.Net.IPEndPoint))
				    errors.Add("Items[\"owin.remote_endpoint\"] must be a System.Net.IPEndPoint");
				    break;
			}
		    }			
		}
	    }

	    // Status header if (! Regex.IsMatch(request.Items["owin.server_port"].ToString(), @"^\d{3} [\w ]+$"))

	    return errors.ToArray();
	}
    }

    static class LintValidationHelperExtensions {
	public static bool IsNull(this object value) {
	    return value == null;
	}
	public static bool IsNullOrWhiteSpace(this object value) {
	    if (value == null)
		return true;
	    else
		return value.ToString().IsNullOrWhiteSpace();
	}
	public static bool IsNullOrWhiteSpace(this string value) {
	    return string.IsNullOrEmpty(value) || value.IsWhiteSpace();
	}
	public static bool IsWhiteSpace(this string value) {
	    return value.Trim().Length == 0;
	}
	public static bool DoesNotHaveKey(this IDictionary<string, object> value, string key) {
	    return ! value.ContainsKey(key);
	}
	public static bool IsAnInteger(this object value) {
	    if (value == null) return false;
	    if (value is int)  return true;

	    try {
		int.Parse(value.ToString());
		return true;
	    } catch (FormatException) {
		return false;
	    }
	}
	public static bool IsNotAnInteger(this object value) {
	    return ! value.IsAnInteger();
	}
    }
}
