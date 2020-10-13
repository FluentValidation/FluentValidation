#region License

// Copyright (c) .NET Foundation and contributors.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// The latest version of this file can be found at https://github.com/FluentValidation/FluentValidation

#endregion

namespace FluentValidation.Tests.AspNetCore {
	using System.Net.Http;
	using System.Threading.Tasks;
	using Xunit;

	public class ClientsideMessageTester : IClassFixture<WebAppFixture> {
		private readonly HttpClient _client;

		public ClientsideMessageTester(WebAppFixture webApp) {
			_client = webApp.WithContainer(enableLocalization:true).CreateClient();
			CultureScope.SetDefaultCulture();
		}

		[Fact]
		public async Task NotEmpty_uses_simplified_message_for_clientside_validation() {
			var msg = await _client.GetClientsideMessage("Required", "data-val-required");
			msg.ShouldEqual("'Required' must not be empty.");
		}

		[Fact]
		public async Task NotNull_uses_simplified_message_for_clientside_validation() {
			var msg = await _client.GetClientsideMessage("Required2", "data-val-required");
			msg.ShouldEqual("'Required2' must not be empty.");
		}

		[Fact]
		public async Task RegexValidator_uses_simplified_message_for_clientside_validation() {
			var msg = await _client.GetClientsideMessage("RegEx", "data-val-regex");
			msg.ShouldEqual("'Reg Ex' is not in the correct format.");
		}

		[Fact]
		public async Task EmailValidator_uses_simplified_message_for_clientside_validation() {
			var msg = await _client.GetClientsideMessage("Email", "data-val-email");
			msg.ShouldEqual("'Email' is not a valid email address.");
		}

		[Fact]
		public async Task LengthValidator_uses_simplified_message_for_clientside_validatation() {
			var msg = await _client.GetClientsideMessage("Length", "data-val-length");
			msg.ShouldEqual("'Length' must be between 1 and 4 characters.");
		}

		[Fact]
		public async Task MinLengthValidator_uses_simplified_message_for_clientside_validatation() {
			var msg = await _client.GetClientsideMessage("MinLength", "data-val-minlength");
			msg.ShouldEqual("The length of 'Min Length' must be at least 1 characters.");
		}

		[Fact]
		public async Task MaxengthValidator_uses_simplified_message_for_clientside_validatation() {
			var msg = await _client.GetClientsideMessage("MaxLength", "data-val-maxlength");
			msg.ShouldEqual("The length of 'Max Length' must be 2 characters or fewer.");
		}

		[Fact]
		public async Task ExactLengthValidator_uses_simplified_message_for_clientside_validation() {
			var msg = await _client.GetClientsideMessage("ExactLength", "data-val-length");
			msg.ShouldEqual("'Exact Length' must be 4 characters in length.");
		}


		[Fact]
		public async Task InclusiveBetween_validator_uses_simplified_message_for_clientside_validation() {
			var msg = await _client.GetClientsideMessage("Range", "data-val-range");
			msg.ShouldEqual("'Range' must be between 1 and 5.");
		}

		[Fact]
		public async Task GreaterThanOrEqualTo_validator_uses_simplified_message_for_clientside_validation() {
			var msg = await _client.GetClientsideMessage("GreaterThanOrEqual", "data-val-range");
			msg.ShouldEqual("'Greater Than Or Equal' must be greater than or equal to '1'.");
		}

		[Fact]
		public async Task LessThanOrEqualTo_validator_uses_simplified_message_for_clientside_validation() {
			var msg = await _client.GetClientsideMessage("LessThanOrEqual", "data-val-range");
			msg.ShouldEqual("'Less Than Or Equal' must be less than or equal to '10'.");
		}

		[Fact]
		public async Task EqualValidator_with_property_uses_simplified_message_for_clientside_validation() {
			var msg = await _client.GetClientsideMessage("EqualTo", "data-val-equalto");
			msg.ShouldEqual("'Equal To' must be equal to 'Required'.");
		}

		[Fact]
		public async Task Should_not_munge_custom_message() {
			var msg = await _client.GetClientsideMessage("LengthWithMessage", "data-val-length");
			msg.ShouldEqual("Foo");
		}

		[Fact]
		public async Task Custom_validation_message_with_placeholders() {
			var msg = await _client.GetClientsideMessage("CustomPlaceholder", "data-val-required");
			msg.ShouldEqual("Custom Placeholder is null.");
		}

		[Fact]
		public async Task Custom_validation_message_for_length() {
			var msg = await _client.GetClientsideMessage("LengthCustomPlaceholders", "data-val-length");
			msg.ShouldEqual("Must be between 1 and 5.");
		}

		//TODO: Is there an IClientValidatable equivalent?
//		[Fact]
//		public async Task Supports_custom_clientside_rules_with_IClientValidatable() {
//			validator.RuleFor(x => x.Name).SetValidator(new TestPropertyValidator());
//			msg.ShouldEqual("foo");
//		}

		[Fact]
		public async Task CreditCard_creates_clientside_message() {
			var msg = await _client.GetClientsideMessage("CreditCard", "data-val-creditcard");
			msg.ShouldEqual("'Credit Card' is not a valid credit card number.");
		}

		[Fact]
		public async Task Overrides_property_name_for_clientside_rule() {
			var msg = await _client.GetClientsideMessage("CustomName", "data-val-required");

			msg.ShouldEqual("'Foo' must not be empty.");
		}

		[Fact]
		public async Task Overrides_property_name_for_clientside_rule_using_localized_name() {
			var msg = await _client.GetClientsideMessage("LocalizedName", "data-val-required");

			msg.ShouldEqual("'Localised Error' must not be empty.");
		}

		[Fact]
		public async Task Overrides_property_name_for_non_nullable_value_type() {
			var msg = await _client.GetClientsideMessage("CustomNameValueType", "data-val-required");

			msg.ShouldEqual("'Foo' must not be empty.");
		}

		[Fact]
		public async Task Falls_back_to_default_message_when_no_context_available_to_custom_message_format() {
			var msg = await _client.GetClientsideMessage("MessageWithContext", "data-val-required");
			msg.ShouldEqual("'Message With Context' must not be empty.");
		}

		[Fact]
		public async Task Can_use_localizer() {
			var msg = await _client.GetClientsideMessage("LocalizedMessage", "data-val-required");
			msg.ShouldEqual("from localizer");
		}

		[Fact]
		public async Task Should_only_use_rules_from_Default_ruleset() {
			var msg = await _client.RunRulesetAction("/ClientSide/DefaultRuleset");
			msg.Length.ShouldEqual(1);
			msg[0].ShouldEqual("third");
		}

		[Fact]
		public async Task Should_use_rules_from_specified_ruleset() {
			var msg = await _client.RunRulesetAction("/ClientSide/SpecifiedRuleset");
			msg.Length.ShouldEqual(1);
			msg[0].ShouldEqual("first");
		}

		[Fact]
		public async Task Should_use_rules_from_multiple_rulesets() {
			var msgs = await _client.RunRulesetAction("/ClientSide/MultipleRulesets");
			msgs.Length.ShouldEqual(2);
			msgs[0].ShouldEqual("first");
			msgs[1].ShouldEqual("second");
		}

		[Fact]
		public async Task Should_use_rules_from_default_ruleset_and_specified_ruleset() {
			var msgs = await _client.RunRulesetAction("/ClientSide/DefaultAndSpecified");
			msgs.Length.ShouldEqual(2);
			msgs[0].ShouldEqual("first");
			msgs[1].ShouldEqual("third");
		}

		[Fact]
		public async Task Renders_attributes_inside_partial() {
			var msg = await _client.GetClientsideMessage("RequiredInsidePartial", "data-val-required");
			msg.ShouldEqual("'Required Inside Partial' must not be empty.");
		}

		[Fact]
		public async Task Instantiates_validator_for_clientside_usage_only_once() {
			//static property - make sure it's reinitialized at the start of this test as other tests affect it too
			ClientsideModelValidator.TimesInstantiated = 0;
			var results = await _client.GetClientsideMessages();
			ClientsideModelValidator.TimesInstantiated.ShouldEqual(1);
		}
	}


	public class RazorPagesClientsideMessageTester : IClassFixture<WebAppFixture> {
		private readonly HttpClient _client;

		public RazorPagesClientsideMessageTester(WebAppFixture webApp) {
			_client = webApp.WithContainer(enableLocalization: true).CreateClient();
			CultureScope.SetDefaultCulture();
		}

		[Fact]
		public async Task Should_only_use_rules_from_default_ruleset() {
			var msg = await _client.RunRulesetAction("/Rulesets/DefaultRuleSet", "Test");
			msg.Length.ShouldEqual(1);
			msg[0].ShouldEqual("third");
		}

		[Fact]
		public async Task Should_use_rules_from_specified_ruleset() {
			var msg = await _client.RunRulesetAction("/Rulesets/SpecifiedRuleSet", "Test");
			msg.Length.ShouldEqual(1);
			msg[0].ShouldEqual("first");
		}

		[Fact]
		public async Task Should_use_rules_from_multiple_rulesets() {
			var msgs = await _client.RunRulesetAction("/Rulesets/MultipleRuleSets", "Test");
			msgs.Length.ShouldEqual(2);
			msgs[0].ShouldEqual("first");
			msgs[1].ShouldEqual("second");
		}

		[Fact]
		public async Task Should_use_rules_from_default_ruleset_and_specified_ruleset() {
			var msgs = await _client.RunRulesetAction("/Rulesets/DefaultAndSpecifiedRuleSet", "Test");
			msgs.Length.ShouldEqual(2);
			msgs[0].ShouldEqual("first");
			msgs[1].ShouldEqual("third");
		}

#if NETCOREAPP3_1 || NET5_0
		[Fact]
		public async Task Should_only_use_rules_from_default_ruleset_extension() {
			var msg = await _client.RunRulesetAction("/Rulesets/RuleSetForHandlers?handler=default", "Test");
			msg.Length.ShouldEqual(1);
			msg[0].ShouldEqual("third");
		}

		[Fact]
		public async Task Should_use_rules_from_specified_ruleset_extension() {
			var msg = await _client.RunRulesetAction("/Rulesets/RuleSetForHandlers?handler=specified", "Test");
			msg.Length.ShouldEqual(1);
			msg[0].ShouldEqual("first");
		}

		[Fact]
		public async Task Should_use_rules_from_multiple_rulesets_extension() {
			var msgs = await _client.RunRulesetAction("/Rulesets/RuleSetForHandlers?handler=multiple", "Test");
			msgs.Length.ShouldEqual(2);
			msgs[0].ShouldEqual("first");
			msgs[1].ShouldEqual("second");
		}

		[Fact]
		public async Task Should_use_rules_from_default_ruleset_and_specified_ruleset_extension() {
			var msgs = await _client.RunRulesetAction("/Rulesets/RuleSetForHandlers?handler=defaultAndSpecified", "Test");
			msgs.Length.ShouldEqual(2);
			msgs[0].ShouldEqual("first");
			msgs[1].ShouldEqual("third");
		}
#endif
	}
}
