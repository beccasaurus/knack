using System;
using Owin;
using NUnit.Framework;

namespace Owin.Common.Specs {

	[TestFixture]
	public class RequestSpec {

		Request req;
		
		[SetUp]
		public void Before() { req = null; }

		[Test]
		public void Can_be_instantiated_with_no_arguments_to_build_a_new_request(){
			Assert.Fail("todo ... implement!");
		}

		[Test][Ignore] public void Can_be_instantiated_with_an_IRequest(){}
		[Test][Ignore] public void Can_get_the_raw_QueryString(){}
		[Test][Ignore] public void Can_get_the_value_of_a_QueryString(){}
		[Test][Ignore] public void Can_get_the_post_body(){}
		[Test][Ignore] public void Can_get_the_value_of_a_POST_variable(){}
		[Test][Ignore] public void Can_get_Params_from_either_a_QueryString_or_POST_variable(){}
		[Test][Ignore] public void Can_get_referer_or_referrer(){}
		[Test][Ignore] public void Can_get_host(){}
		[Test][Ignore] public void Can_get_port(){}
		[Test][Ignore] public void Can_get_scheme(){}
		[Test][Ignore] public void Has_predicate_properties_for_checking_request_method_type(){}
		[Test][Ignore] public void Can_get_IP(){}
	}
}
