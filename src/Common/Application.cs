using System;
using System.Net;
using System.Web;
using System.Text;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Owin {

    public delegate IResponse ApplicationResponder(IRequest request);

    public class Application : IApplication, IMiddleware {

        public Application() { }

        public Application(IApplication innerApplication) {
            InnerApplication = innerApplication;
        }

        public Application(IApplication innerApplication, ApplicationResponder responder) {
            InnerApplication = innerApplication;
            Responder = responder;
        }

        public Application(ApplicationResponder responder) {
            Responder = responder;
        }

        public IApplication InnerApplication { get; set; }

        /// <summary>If you override Call in your Application subclass, it will be used for invoking your application if Responder is not set</summary>
        public virtual IResponse Call(IRequest request) {
            throw new NotImplementedException("You need to override Call in your Application subclass or set Application.Responder.");
        }

        ApplicationResponder _responder;

        /// <summary>The delegate use to actually invoke your application.  If not set manually, the Call method is used.</summary>
        public ApplicationResponder Responder {
            get {
                if (_responder == null)
                    _responder = new ApplicationResponder(Call);
                return _responder;
            }
            set { _responder = value; }
        }

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
