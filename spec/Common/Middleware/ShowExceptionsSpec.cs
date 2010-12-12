using System;
using Owin;
using Owin.Test;
using NUnit.Framework;

namespace Owin.Common.Middleware.Specs {

	[TestFixture]
	public class ShowExceptionsSpec : MockSession {

		class AppThatExplodes : Application, IApplication {
			public override IResponse Call(IRequest request) {
				throw new Exception("Boom! My codez are fail!");
			}
		}

		[Test]
		[ExpectedException(typeof(Exception), ExpectedMessage = "Boom! My codez are fail!")]
		public void Exception_is_raised_when_not_using_ShowExceptions() {
			App = new AppThatExplodes();
			Get("/");
		}

		[Test]
		public void Exception_is_not_raised_but_instead_displayed_if_using_ShowExceptions() {
			App = new Builder().Use(new ShowExceptions()).Run(new AppThatExplodes()).ToApp();
			Get("/");
			Assert.That(LastResponse.BodyText, Is.StringContaining("Boom! My codez are fail!"));
		}
	}
}
