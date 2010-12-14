using System;

namespace Owin {

	/// <summary>A IMiddleware is simply an IApplication that has an InnerApplication IApplication</summary>
	public interface IMiddleware : IApplication {
		IApplication InnerApplication { get; set; }
	}
}
