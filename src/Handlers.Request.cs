using System;
using Owin;

namespace Owin.Handlers {

    // a writable request that handlers can use to make their lives a bit easier!
    //
    // we can also use this to help us make valid mock requests
    //
    // Might rename to something else ... in the scope of making a handler, this is a "Request".
    //
    // In the scope of an application, Owin.Request is what you want
    //
    public class Request : Owin.Request, IRequest {
	public Request() {

	}
    }
}
