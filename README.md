Owin.Common
===========

Standard library for OWIN (*Open Web Interface for .NET*)
---------------------------------------------------------

Documentation: [http://remi.github.com/Owin.Common][docs]

Screencast coming soon.

We'll add README documentation shortly!

### NUnit Output

Not willing to wait for screencast or documentation?  Here's our spec output:

    ***** Owin.Common.Middleware.Specs.LintSpec.Validates_incoming_Request
    ***** Owin.Common.Middleware.Specs.LintSpec.Validates_outgoing_Response
    ***** Owin.Common.Middleware.Specs.ShowExceptionsSpec.Exception_is_not_raised_but_instead_displayed_if_using_ShowExceptions
    ***** Owin.Common.Middleware.Specs.ShowExceptionsSpec.Exception_is_raised_when_not_using_ShowExceptions
    ***** Owin.Common.Specs.ApplicationSpec.Can_create_a_new_application_using_an_external_method
    ***** Owin.Common.Specs.ApplicationSpec.Can_make_an_application_class_easily_that_uses_an_internal_method
    ***** Owin.Common.Specs.LintRequestSpec.__new_Req_is_valid
    ***** Owin.Common.Specs.LintRequestSpec.Can_raise_exception_if_validation_fails
    ***** Owin.Common.Specs.LintRequestSpec.Can_raise_exception_with_many_messages_if_validation_fails
    ***** Owin.Common.Specs.LintRequestSpec.Headers_cannot_be_null
    ***** Owin.Common.Specs.LintRequestSpec.Headers_keys_cannot_have_colon
    ***** Owin.Common.Specs.LintRequestSpec.Headers_keys_cannot_have_whitespace
    ***** Owin.Common.Specs.LintRequestSpec.Headers_keys_must_be_lowercase
    ***** Owin.Common.Specs.LintRequestSpec.Items_cannot_be_null_or_empty
    ***** Owin.Common.Specs.LintRequestSpec.Items_owin_base_path_cannot_be_null
    ***** Owin.Common.Specs.LintRequestSpec.Items_owin_base_url_should_be_used_for_something
    ***** Owin.Common.Specs.LintRequestSpec.Items_owin_remote_endpoint_cannot_be_null
    ***** Owin.Common.Specs.LintRequestSpec.Items_owin_remote_endpoint_must_be_a_IPEndPoint
    ***** Owin.Common.Specs.LintRequestSpec.Items_owin_request_protocol_cannot_be_null_or_blank
    ***** Owin.Common.Specs.LintRequestSpec.Items_owin_request_protocol_must_be_valid
    ***** Owin.Common.Specs.LintRequestSpec.Items_owin_server_name_cannot_be_null_or_blank
    ***** Owin.Common.Specs.LintRequestSpec.Items_owin_server_port_cannot_be_null_or_blank
    ***** Owin.Common.Specs.LintRequestSpec.Items_owin_server_port_must_be_convertable_to_an_int
    ***** Owin.Common.Specs.LintRequestSpec.Items_owin_url_scheme_must_be_http_or_https
    ***** Owin.Common.Specs.LintRequestSpec.Items_owin_version_should_be_present
    ***** Owin.Common.Specs.LintRequestSpec.Method_cannot_be_null_or_blank
    ***** Owin.Common.Specs.LintRequestSpec.Uri_cannot_be_null
    ***** Owin.Common.Specs.LintResponseSpec.__new_Resp_is_valid
    ***** Owin.Common.Specs.LintResponseSpec.Can_raise_exception_if_validation_fails
    ***** Owin.Common.Specs.LintResponseSpec.Can_raise_exception_with_many_messaegs_if_validation_fails
    ***** Owin.Common.Specs.LintResponseSpec.GetBody_enumerable_must_return_valid_type
    ***** Owin.Common.Specs.LintResponseSpec.GetBody_must_be_UTF8_or_raw_or_FileInfo
    ***** Owin.Common.Specs.LintResponseSpec.Headers_cannot_be_null
    ***** Owin.Common.Specs.LintResponseSpec.Headers_keys_can_only_contain_ASCII_characters
    ***** Owin.Common.Specs.LintResponseSpec.Headers_keys_cannot_contain_newlines
    ***** Owin.Common.Specs.LintResponseSpec.Headers_keys_cannot_contain_periods
    ***** Owin.Common.Specs.LintResponseSpec.Headers_keys_cannot_contain_whitespace
    ***** Owin.Common.Specs.LintResponseSpec.Headers_requires_content_type
    ***** Owin.Common.Specs.LintResponseSpec.Headers_values_can_only_contain_ASCII_characters
    ***** Owin.Common.Specs.LintResponseSpec.Headers_values_cannot_contain_newlines
    ***** Owin.Common.Specs.LintResponseSpec.Status_can_only_contain_ASCII_characters
    ***** Owin.Common.Specs.LintResponseSpec.Status_cannot_be_null_or_blank
    ***** Owin.Common.Specs.LintResponseSpec.Status_cannot_contain_a_newline
    ***** Owin.Common.Specs.LintResponseSpec.Status_must_start_with_integer_status_followed_by_a_space_and_a_reason_phrase
    ***** Owin.Common.Specs.MiddlewareSpec.Middleware_can_easily_be_configured_using_Owin_Builder_build_method
    ***** Owin.Common.Specs.MiddlewareSpec.Middleware_can_easily_be_configured_using_Owin_Builder_constructor
    ***** Owin.Common.Specs.MiddlewareSpec.Middleware_can_easily_be_configured_using_Owin_Builder_fluent_syntax
    ***** Owin.Common.Specs.MiddlewareSpec.Middleware_can_wrap_the_response_of_an_application
    ***** Owin.Common.Specs.RequestSpec.Can_be_instantiated_with_an_IRequest
    ***** Owin.Common.Specs.RequestSpec.Can_be_instantiated_with_no_arguments_to_build_a_new_request
    ***** Owin.Common.Specs.RequestSpec.Can_easily_get_the_string_value_for_a_Header_if_you_know_a_header_will_only_have_1_value
    ***** Owin.Common.Specs.RequestSpec.Can_get_all_bytes_from_getbody
    ***** Owin.Common.Specs.RequestSpec.Can_get_host
    ***** Owin.Common.Specs.RequestSpec.Can_get_host_from_header
    ***** Owin.Common.Specs.RequestSpec.Can_get_IPAddress
    ***** Owin.Common.Specs.RequestSpec.Can_get_IPEndPoint
    ***** Owin.Common.Specs.RequestSpec.Can_get_port
    ***** Owin.Common.Specs.RequestSpec.Can_get_port_from_header
    ***** Owin.Common.Specs.RequestSpec.Can_get_protocol
    ***** Owin.Common.Specs.RequestSpec.Can_get_referer_or_referrer
    ***** Owin.Common.Specs.RequestSpec.Can_get_scheme
    ***** Owin.Common.Specs.RequestSpec.Can_get_the_full_Url
    ***** Owin.Common.Specs.RequestSpec.Can_get_the_path_info_which_is_the_Uri_without_querystrings
    ***** Owin.Common.Specs.RequestSpec.Can_get_the_path_which_includes_the_base_name_and_path_without_querystrings
    ***** Owin.Common.Specs.RequestSpec.Can_get_the_raw_QueryString_from_header
    ***** Owin.Common.Specs.RequestSpec.Can_get_the_raw_QueryString_from_Uri
    ***** Owin.Common.Specs.RequestSpec.Can_get_the_request_content_type
    ***** Owin.Common.Specs.RequestSpec.Can_get_the_value_of_a_POST_or_GET_variable_via_indexer_which_is_an_alias_to_Params
    ***** Owin.Common.Specs.RequestSpec.Can_get_the_value_of_a_POST_or_GET_variable_with_Params
    ***** Owin.Common.Specs.RequestSpec.Can_get_the_value_of_a_POST_variable
    ***** Owin.Common.Specs.RequestSpec.Can_get_the_value_of_a_QueryString
    ***** Owin.Common.Specs.RequestSpec.Can_get_whether_or_not_the_request_has_form_data
    ***** Owin.Common.Specs.RequestSpec.Can_read_body_as_string
    ***** Owin.Common.Specs.RequestSpec.Can_read_body_manually
    ***** Owin.Common.Specs.RequestSpec.Has_predicate_properties_for_checking_request_method_type
    ***** Owin.Common.Specs.RequestWriterSpec+Constructors.Can_make_new_with_just_uri_which_defaults_to_get
    ***** Owin.Common.Specs.RequestWriterSpec+Constructors.Can_make_new_with_method_and_uri
    ***** Owin.Common.Specs.RequestWriterSpec+Constructors.Can_make_new_with_method_and_uri_and_body_as_bytes
    ***** Owin.Common.Specs.RequestWriterSpec+Constructors.Can_make_new_with_method_and_uri_and_body_as_string
    ***** Owin.Common.Specs.RequestWriterSpec+Constructors.Can_make_new_with_method_and_uri_and_headers
    ***** Owin.Common.Specs.RequestWriterSpec+Constructors.Can_make_new_with_uri_and_headers
    ***** Owin.Common.Specs.RequestWriterSpec+Defaults.Default_method_is_get
    ***** Owin.Common.Specs.RequestWriterSpec+Defaults.Default_uri_is_blank
    ***** Owin.Common.Specs.RequestWriterSpec+Defaults.Items_owin_base_path_is_blank_by_default
    ***** Owin.Common.Specs.RequestWriterSpec+Defaults.Items_owin_remote_endpoint_is_127_0_0_1_over_port_80_by_default
    ***** Owin.Common.Specs.RequestWriterSpec+Defaults.Items_owin_request_protocol_is_http_1_1_by_default
    ***** Owin.Common.Specs.RequestWriterSpec+Defaults.Items_owin_server_name_is_localhost_by_default
    ***** Owin.Common.Specs.RequestWriterSpec+Defaults.Items_owin_server_port_is_80_by_default
    ***** Owin.Common.Specs.RequestWriterSpec+Defaults.Items_owin_url_scheme_is_http_by_default
    ***** Owin.Common.Specs.RequestWriterSpec+Defaults.Request_body_is_empty_by_default
    ***** Owin.Common.Specs.RequestWriterSpec+Defaults.Request_headers_are_empty_by_default
    ***** Owin.Common.Specs.RequestWriterSpec+Modifying.Can_change_the_default_encoding_used_for_body
    ***** Owin.Common.Specs.RequestWriterSpec+Modifying+Fluent.Can_AddHeader_which_adds_to_the_IEnumerable_with_IEnumerable_string
    ***** Owin.Common.Specs.RequestWriterSpec+Modifying+Fluent.Can_AddHeader_which_adds_to_the_IEnumerable_with_string
    ***** Owin.Common.Specs.RequestWriterSpec+Modifying+Fluent.Can_AddToBody_with_bytes
    ***** Owin.Common.Specs.RequestWriterSpec+Modifying+Fluent.Can_AddToBody_with_string
    ***** Owin.Common.Specs.RequestWriterSpec+Modifying+Fluent.Can_SetBody_with_bytes
    ***** Owin.Common.Specs.RequestWriterSpec+Modifying+Fluent.Can_SetBody_with_string
    ***** Owin.Common.Specs.RequestWriterSpec+Modifying+Fluent.Can_SetHeader_with_IEnumerable_string
    ***** Owin.Common.Specs.RequestWriterSpec+Modifying+Fluent.Can_SetHeader_with_string
    ***** Owin.Common.Specs.RequestWriterSpec+Modifying+Fluent.Can_SetItem
    ***** Owin.Common.Specs.RequestWriterSpec+Modifying+Fluent.Can_SetMethod
    ***** Owin.Common.Specs.RequestWriterSpec+Modifying+Fluent.Can_SetUri
    ***** Owin.Common.Specs.RequestWriterSpec+Modifying+Fluent.SetHeader_automatically_lowercases_header_keys_as_required_by_the_owin_spec
    ***** Owin.Common.Specs.ResponseSpec+Constructors.Can_make_new_with_body_and_headers
    ***** Owin.Common.Specs.ResponseSpec+Constructors.Can_make_new_with_body_and_status_code
    ***** Owin.Common.Specs.ResponseSpec+Constructors.Can_make_new_with_body_and_status_string
    ***** Owin.Common.Specs.ResponseSpec+Constructors.Can_make_new_with_body_status_and_headers
    ***** Owin.Common.Specs.ResponseSpec+Constructors.Can_make_new_with_body_status_and_headers_as_string_string_dictionary
    ***** Owin.Common.Specs.ResponseSpec+Constructors.Can_make_new_with_just_a_string_body
    ***** Owin.Common.Specs.ResponseSpec+Constructors.Can_make_new_with_status_code_and_headers
    ***** Owin.Common.Specs.ResponseSpec+Constructors.Can_make_new_with_status_code_and_headers_as_string_string_dictionary
    ***** Owin.Common.Specs.ResponseSpec+Constructors.Can_take_an_existing_IResponse
    ***** Owin.Common.Specs.ResponseSpec+Defaults.Default_body_is_blank
    ***** Owin.Common.Specs.ResponseSpec+Defaults.Default_content_type_is_text_html
    ***** Owin.Common.Specs.ResponseSpec+Defaults.Default_status_is_200
    ***** Owin.Common.Specs.ResponseSpec+Modifying.Can_write_to_body
    ***** Owin.Common.Specs.ResponseSpec+Modifying.Can_write_to_body_passing_object_to_interpolate
    ***** Owin.Common.Specs.ResponseSpec+Modifying.Setting_body_directly_updates_content_length
    ***** Owin.Common.Specs.ResponseSpec+Modifying.Writing_to_body_updates_content_length
    ***** Owin.Common.Specs.ResponseSpec+Modifying+Fluent.Can_AddHeader_which_adds_to_the_IEnumerable_with_IEnumerable_string
    ***** Owin.Common.Specs.ResponseSpec+Modifying+Fluent.Can_AddHeader_which_adds_to_the_IEnumerable_with_string
    ***** Owin.Common.Specs.ResponseSpec+Modifying+Fluent.Can_AddToBody_with_an_IEnumerable_of_objects
    ***** Owin.Common.Specs.ResponseSpec+Modifying+Fluent.Can_AddToBody_with_an_object
    ***** Owin.Common.Specs.ResponseSpec+Modifying+Fluent.Can_AddToBody_with_objects
    ***** Owin.Common.Specs.ResponseSpec+Modifying+Fluent.Can_easily_get_a_path_relative_to_the_BasePath
    ***** Owin.Common.Specs.ResponseSpec+Modifying+Fluent.Can_redirect_to_a_path_relative_to_the_BasePath
    ***** Owin.Common.Specs.ResponseSpec+Modifying+Fluent.Can_redirect_to_a_path_specifying_the_status_code
    ***** Owin.Common.Specs.ResponseSpec+Modifying+Fluent.Can_redirect_to_a_path_using_302_by_default
    ***** Owin.Common.Specs.ResponseSpec+Modifying+Fluent.Can_set_cookie
    ***** Owin.Common.Specs.ResponseSpec+Modifying+Fluent.Can_SetBody_with_a_byte_array
    ***** Owin.Common.Specs.ResponseSpec+Modifying+Fluent.Can_SetBody_with_a_FileInfo
    ***** Owin.Common.Specs.ResponseSpec+Modifying+Fluent.Can_SetBody_with_a_string
    ***** Owin.Common.Specs.ResponseSpec+Modifying+Fluent.Can_SetBody_with_an_ArraySegment_of_bytes
    ***** Owin.Common.Specs.ResponseSpec+Modifying+Fluent.Can_SetBody_with_an_IEnumerable_of_objects
    ***** Owin.Common.Specs.ResponseSpec+Modifying+Fluent.Can_SetBody_with_any_number_of_objects
    ***** Owin.Common.Specs.ResponseSpec+Modifying+Fluent.Can_SetHeader_with_IEnumerable_string
    ***** Owin.Common.Specs.ResponseSpec+Modifying+Fluent.Can_SetHeader_with_string
    ***** Owin.Common.Specs.ResponseSpec+Modifying+Fluent.Can_SetStatus_with_a_string
    ***** Owin.Common.Specs.ResponseSpec+Modifying+Fluent.Can_SetStatus_with_an_int
    ***** Owin.Handlers.Specs.CgiRequestSpec+Body.Can_get_and_set_body
    ***** Owin.Handlers.Specs.CgiRequestSpec+Body.Can_initialize_CgiRequest_with_body_content
    ***** Owin.Handlers.Specs.CgiRequestSpec+Body.multipart_formdata_POST_variables_are_parsed_properly
    ***** Owin.Handlers.Specs.CgiRequestSpec+Body.x_www_form_urlencoded_POST_variables_are_parsed_properly
    ***** Owin.Handlers.Specs.CgiRequestSpec+Headers.All_environment_variables_become_headers
    ***** Owin.Handlers.Specs.CgiRequestSpec+Headers.ContentType_gets_set_by_CONTENT_TYPE_header
    ***** Owin.Handlers.Specs.CgiRequestSpec+Items.owin_base_path_gets_set_to_SCRIPT_NAME
    ***** Owin.Handlers.Specs.CgiRequestSpec+Items.owin_remote_endpoint_gets_set_using_REMOTE_ADDR_and_SERVER_PORT
    ***** Owin.Handlers.Specs.CgiRequestSpec+Items.owin_request_protocol_gets_set_to_SERVER_PROTOCOL
    ***** Owin.Handlers.Specs.CgiRequestSpec+Items.owin_server_name_gets_set_to_SERVER_NAME
    ***** Owin.Handlers.Specs.CgiRequestSpec+Items.owin_server_port_gets_set_to_SERVER_PORT
    ***** Owin.Handlers.Specs.CgiRequestSpec+Items.owin_url_scheme_gets_set_using_scheme_seen_in_REQUEST_URI
    ***** Owin.Handlers.Specs.CgiRequestSpec+Uri_Method_and_QueryStrings.Method_gets_set_using_REQUEST_METHOD
    ***** Owin.Handlers.Specs.CgiRequestSpec+Uri_Method_and_QueryStrings.Uri_gets_QueryString_from_QUERY_STRING
    ***** Owin.Handlers.Specs.CgiRequestSpec+Uri_Method_and_QueryStrings.Uri_gets_set_using_PATH_INFO
    ***** Owin.Handlers.Specs.CgiRequestSpec+Uri_Method_and_QueryStrings.Uri_includes_query_strings_from_QUERY_STRING
    ***** Owin.Handlers.Specs.CgiResponseSpec.ShouldCreateResponseOK
    ***** Owin.Test.Specs.ManuallyTestingSpec.Can_manually_make_a_MockRequest_and_get_a_MockResponse
    ***** Owin.Test.Specs.MockSessionSpec.Can_easily_do_DELETE_requests
    ***** Owin.Test.Specs.MockSessionSpec.Can_easily_do_GET_requests
    ***** Owin.Test.Specs.MockSessionSpec.Can_easily_do_POST_requests
    ***** Owin.Test.Specs.MockSessionSpec.Can_easily_do_PUT_requests
    ***** Owin.Test.Specs.MockSessionSpec.Can_get_LastRequest
    ***** Owin.Test.Specs.MockSessionSpec.Can_get_LastResponse
    ***** Owin.Test.Specs.MockSessionSpec.Can_manually_request_an_IRequest
    ***** Owin.Test.Specs.MockSessionSpec+InheritingFromMockSessionSpec.Can_inherit_from_MockSession_to_get_a_nice_DSL__get_example
    ***** Owin.Test.Specs.MockSessionSpec+InheritingFromMockSessionSpec.Can_inherit_from_MockSession_to_get_a_nice_DSL__post_example
    
    Tests run: 149, Errors: 0, Failures: 0, Inconclusive: 0, Time: 0.458861 seconds
      Not run: 18, Invalid: 0, Ignored: 18, Skipped: 0
    
    Tests Not Run:
    1) Owin.Common.Specs.LintRequestSpec.Items_cannot_be_null_or_empty : Pending (Not Yet Implemented)
    2) Owin.Common.Specs.LintRequestSpec.Items_owin_base_url_should_be_used_for_something : Pending (Not Yet Implemented)
    3) Owin.Common.Specs.LintRequestSpec.Items_owin_version_should_be_present : Pending (Not Yet Implemented)
    4) Owin.Common.Specs.LintResponseSpec.GetBody_must_be_UTF8_or_raw_or_FileInfo : Pending (Not Yet Implemented)
    5) Owin.Common.Specs.LintResponseSpec.Headers_requires_content_type : Pending (Not Yet Implemented)
    6) Owin.Common.Specs.RequestSpec.Can_be_instantiated_with_no_arguments_to_build_a_new_request : Pending (Not Yet Implemented)
    7) Owin.Common.Specs.RequestSpec.Can_get_host_from_header : Pending (Not Yet Implemented)
    8) Owin.Common.Specs.RequestSpec.Can_get_port_from_header : Pending (Not Yet Implemented)
    9) Owin.Common.Specs.RequestSpec.Can_get_referer_or_referrer : Pending (Not Yet Implemented)
    10) Owin.Common.Specs.RequestSpec.Can_get_the_raw_QueryString_from_header : Pending (Not Yet Implemented)
    11) Owin.Common.Specs.RequestWriterSpec+Constructors.Can_make_new_with_method_and_uri_and_headers : Pending (Not Yet Implemented)
    12) Owin.Common.Specs.RequestWriterSpec+Constructors.Can_make_new_with_uri_and_headers : Pending (Not Yet Implemented)
    13) Owin.Common.Specs.RequestWriterSpec+Defaults.Request_body_is_empty_by_default : Pending (Not Yet Implemented)
    14) Owin.Common.Specs.RequestWriterSpec+Modifying.Can_change_the_default_encoding_used_for_body : Pending (Not Yet Implemented)
    15) Owin.Common.Specs.ResponseSpec+Modifying+Fluent.Can_easily_get_a_path_relative_to_the_BasePath : Pending (Not Yet Implemented)
    16) Owin.Common.Specs.ResponseSpec+Modifying+Fluent.Can_redirect_to_a_path_relative_to_the_BasePath : Pending (Not Yet Implemented)
    17) Owin.Common.Specs.ResponseSpec+Modifying+Fluent.Can_set_cookie : Pending (Not Yet Implemented)
    18) Owin.Handlers.Specs.CgiRequestSpec+Body.multipart_formdata_POST_variables_are_parsed_properly : Pending (Not Yet Implemented)

[docs]: http://remi.github.com/Owin.Common
