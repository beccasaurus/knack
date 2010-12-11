using System;
using System.Net;
using System.Web;
using System.Text;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Owin {

    public delegate IResponse ApplicationResponder(IRequest request);

    public class Application : IApplication {

        public Application() {
            Responder = new ApplicationResponder(Call);
        }

        public Application(ApplicationResponder responder) {
            Responder = responder;
        }

        public virtual IResponse Call(IRequest request) {
            throw new NotImplementedException("You need to override Call in your Application subclass or set Application.Responder.");
        }

        public ApplicationResponder Responder { get; set; }

        public IAsyncResult BeginInvoke(IRequest request, AsyncCallback callback, object state) {
            return Responder.BeginInvoke(request, callback, state);
        }

        public IResponse EndInvoke(IAsyncResult result) {
            return Responder.EndInvoke(result);
        }

        public IResponse Invoke(IRequest request) {
            return Application.Invoke(this, request);
        }

        public static IResponse Invoke(IApplication app, IRequest request) {
            IAsyncResult result = app.BeginInvoke(request, null, null);
            return app.EndInvoke(result);
        }

        public Response GetResponse(IRequest request) {
            return Application.GetResponse(this, request);
        }

        public static Response GetResponse(IApplication app, IRequest request) {
            IResponse response = Invoke(app, request);
            return new Response(response);
        }
    }
}