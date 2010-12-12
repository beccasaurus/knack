using System;

namespace Owin {

    public class ShowExceptions : Application, IApplication, IMiddleware {
	public override IResponse Call(IRequest request) {
	    try {
		return Application.Invoke(InnerApplication, request);
	    } catch (Exception ex) {
		return new Response(PrettyExceptionHtmlFor(ex));
	    }
	}

	// TODO yank the WSGI/Rack template, as it's kind of conventional  :)
	public string PrettyExceptionHtmlFor(Exception ex) {
	    string html = @"
<html>
    <head>
	<title>{0}</title>
	<style type='text/css'>

	</style>
    </head>
    <body>
	<h1>{0}</h1>
	<pre>
{1}
	</pre>
    </body>
</html>
";
	    return string.Format(html, ex, ex.StackTrace);
	}
    }
}
