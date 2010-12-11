using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Owin {

    public class LintException : Exception {
        public LintException(string message) : base(message) { }
    }

    public class Lint {

        static readonly List<string> RequestItemsThatCannotBeNull = new List<string> { "owin.base_path", "owin.server_name", "owin.server_port", "owin.request_protocol", "owin.remote_endpoint" };
        static readonly List<string> RequestItemsThatCannotBeBlank = new List<string> { "owin.server_name", "owin.server_port", "owin.request_protocol" };
        static readonly List<string> ValidRequestProtocols = new List<string> { "HTTP/1.0", "HTTP/1.1" };
        static readonly List<string> ValidUrlSchemes = new List<string> { "http", "https" };
        static readonly List<Type> SupportedResponseBodyTypes = new List<Type> { typeof(string), typeof(byte[]), typeof(ArraySegment<byte>), typeof(FileInfo) };

        public static void Validate(IRequest request) {
            string[] requestErrors = ErrorMessagesFor(request);
            if (requestErrors.Length > 0)
                throw new LintException("Request was not valid: " + string.Join(", ", requestErrors));
        }

        public static void Validate(IResponse response) {
            string[] responseErrors = ErrorMessagesFor(response);
            if (responseErrors.Length > 0)
                throw new LintException("Response was not valid: " + string.Join(", ", responseErrors));
        }

        public static string[] ErrorMessagesFor(IRequest request) {
            List<string> errors = new List<string>();

            // Method
            if (request.Method.IsNull()) errors.Add("Method cannot be null");
            else if (request.Method.IsWhiteSpace()) errors.Add("Method cannot be blank");

            // Uri
            if (request.Uri.IsNull()) errors.Add("Uri cannot be null");

            // Items
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
                foreach (KeyValuePair<string, object> item in request.Items) {
                    string key = item.Key;
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
                                if (!ValidRequestProtocols.Contains(value.ToString()))
                                    errors.Add(string.Format("Items[\"owin.request_protocol\"] protocol is unknown: {0}.  Must be HTTP/1.0 or HTTP/1.1", value));
                                break;
                            case "owin.url_scheme":
                                if (!ValidUrlSchemes.Contains(value.ToString()))
                                    errors.Add(string.Format("Items[\"owin.url_scheme\"] scheme is unknown: {0}.  Must be http or https", value));
                                break;
                            case "owin.remote_endpoint":
                                if (!(value is System.Net.IPEndPoint))
                                    errors.Add("Items[\"owin.remote_endpoint\"] must be a System.Net.IPEndPoint");
                                break;
                        }
                    }
                }
            }

            // Headers
            if (request.Headers.IsNull()) errors.Add("Headers cannot be null");
            else {
                foreach (KeyValuePair<string, IEnumerable<string>> header in request.Headers)
                    if (!header.Key.IsLowercase()) errors.Add("Header keys must be lower-cased: " + header.Key);
                    else if (header.Key.Contains(":")) errors.Add("Header keys cannot contain a colon: " + header.Key);
                    else if (header.Key.Contains(" ")) errors.Add("Header keys cannot contain whitespace: " + header.Key);
            }

            return errors.ToArray();
        }

        public static string[] ErrorMessagesFor(IResponse response) {
            List<string> errors = new List<string>();

            // Status
            if (response.Status.IsNull()) errors.Add("Status cannot be null");
            else if (response.Status.IsWhiteSpace()) errors.Add("Status cannot be blank");
            else if (response.Status.ContainsNonASCII()) errors.Add("Status cannot contain non-ASCII characters: " + response.Status);
            else if (response.Status.Contains("\n")) errors.Add("Status cannot include a newline: " + response.Status);
            else if (!Regex.IsMatch(response.Status, @"^\d{3} [\w ]+$"))
                errors.Add("Status must include integer status followed by a space and a reason phrase: " + response.Status);

            // Headers
            if (response.Headers.IsNull()) errors.Add("Headers cannot be null");
            else {
                foreach (KeyValuePair<string, IEnumerable<string>> header in response.Headers) {
                    string key = header.Key;
                    if (key.Contains(" ")) errors.Add("Header keys cannot contain whitespace: " + key);
                    else if (key.Contains(".")) errors.Add("Header keys cannot contain periods: " + key);
                    else if (key.Contains("\n")) errors.Add("Header keys cannot contain newlines: " + key);
                    else if (key.ContainsNonASCII()) errors.Add("Header keys cannot contain non-ASCII characters: " + key);

                    foreach (string value in header.Value)
                        if (value.Contains("\n")) errors.Add("Header values cannot contain newlines: " + key + ": " + value);
                        else if (value.ContainsNonASCII()) errors.Add("Header values cannot contain non-ASCII characters: " + key + ": " + value);
                }
            }

            // Body
            foreach (object bodyPart in response.GetBody())
                if (!SupportedResponseBodyTypes.Contains(bodyPart.GetType()))
                    errors.Add("GetBody() has unsupported type: " + bodyPart.GetType().Name + ".  Supported types: string, byte[], ArraySegment<byte>, FileInfo");
            /*
            foreach (object bodyPart in response.GetBody()) {
            bool supportedType = false;
            //if (! SupportedResponseBodyTypes.Contains(bodyPart.GetType()))
            //		    errors.Add("GetBody() has unsupported type: " + bodyPart.GetType().Name + ".  Supported types: string, byte[], ArraySegment<byte>, FileInfo");
            foreach (Type type in SupportedResponseBodyTypes) {
                if (bodyPart is type) {
                supportedType = true;
                break;
                }
            }
            if (! supportedType)
                errors.Add("GetBody() has unsupported type: " + bodyPart.GetType().Name + ".  Supported types: string, byte[], ArraySegment<byte>, FileInfo");
            }
            */

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
            return !value.ContainsKey(key);
        }
        public static bool IsAnInteger(this object value) {
            if (value == null) return false;
            if (value is int) return true;

            try {
                int.Parse(value.ToString());
                return true;
            } catch (FormatException) {
                return false;
            }
        }
        public static bool IsNotAnInteger(this object value) {
            return !value.IsAnInteger();
        }
        public static bool ContainsNonASCII(this string value) {
            return value != Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(value));
        }
        public static bool IsLowercase(this string value) {
            return value == value.ToLower();
        }
    }
}