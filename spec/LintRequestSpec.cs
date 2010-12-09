using System;
using System.Collections.Generic;
using Owin;
using NUnit.Framework;

namespace Owin.Common.Specs {

    [TestFixture]
    public class LintRequestSpec {

	[Test][Ignore] public void Method_cannot_be_null_or_blank() {}
	[Test][Ignore] public void Uri_cannot_be_null() {}
	[Test][Ignore] public void Items_owin_base_path_cannot_be_null() {}
	[Test][Ignore] public void Items_owin_server_name_cannot_be_null_or_blank() {}
	[Test][Ignore] public void Items_owin_server_port_cannot_be_null_or_blank() {}
	[Test][Ignore] public void Items_owin_server_port_must_be_convertable_to_an_int() {}
	[Test][Ignore] public void Items_owin_request_protocol_must_be_valid() {}
	[Test][Ignore] public void Items_owin_url_scheme_must_be_http_or_https() {}
	[Test][Ignore] public void Items_owin_remote_endpoint_cannot_be_null() {}
	[Test][Ignore] public void Items_owin_remote_endpoint_must_be_a_IPEndPoint() {}
	[Test][Ignore] public void Items_owin_version_should_be_present() {}
    }
}
