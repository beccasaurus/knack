using System;
using System.Collections.Generic;
using Owin;
using NUnit.Framework;

namespace Owin.Common.Specs {

    [TestFixture]
    public class LintResponseSpec {

	[Test][Ignore] public void Status_cannot_be_null_or_blank() {}
	[Test][Ignore] public void Status_can_only_contain_ASCII_characters() {}
	[Test][Ignore] public void Status_must_start_with_integer_status_followed_by_a_space_and_a_reason_phrase() {}
	[Test][Ignore] public void Status_cannot_contain_a_newline() {}
	[Test][Ignore] public void Headers_keys_cannot_contain_whitespace() {}
	[Test][Ignore] public void Headers_keys_cannot_contain_periods() {}
	[Test][Ignore] public void Headers_values_cannot_contain_newlines() {}
	[Test][Ignore] public void Headers_keys_can_only_contain_ASCII_characters() {}
	[Test][Ignore] public void Headers_values_can_only_contain_ASCII_characters() {}
	[Test][Ignore] public void GetBody_enumerable_must_return_valid_type() {}

	[Test][Ignore] public void GetBody_must_be_UTF8_or_raw_or_FileInfo() {} // TODO later
    }
}
