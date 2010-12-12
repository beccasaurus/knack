using System;
using System.Web;
using System.Collections.Generic;

namespace Owin {

    // Common Utility-type methods for Owin.  Things like query parsing.
    // Instead of directly using things like HttpUtility, it's recommended to 
    // use Owin.Util so we can easily refactor if necessary, isntead of having 
    // to re-write lots of calls to HttpUtility
    public class Util {

	public static string ToQueryString(IDictionary<string,string> data) {
	    string query = "";
	    foreach (KeyValuePair<string,string> item in data)
		query += HttpUtility.UrlEncode(item.Key) + "=" + HttpUtility.UrlEncode(item.Value) + "&";
	    if (query.EndsWith("&"))
		query = query.Substring(0, query.Length - 1);
	    return query;
	}
    }
}
