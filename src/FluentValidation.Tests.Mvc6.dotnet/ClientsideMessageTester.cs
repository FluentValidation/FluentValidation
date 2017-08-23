#region License
// Copyright (c) Jeremy Skinner (http://www.jeremyskinner.co.uk)
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
// The latest version of this file can be found at https://github.com/JeremySkinner/FluentValidation
#endregion

namespace FluentValidation.Tests.AspNetCore {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Net.Http;
	using System.Threading.Tasks;
	using System.Xml.Linq;
	using FluentValidation.AspNetCore;
	using Internal;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.Filters;
	using Microsoft.AspNetCore.Mvc.Internal;
	using Microsoft.AspNetCore.Mvc.ModelBinding;
	using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
	using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
	using Microsoft.AspNetCore.TestHost;
	using Validators;
    using Xunit;

	public class ClientsideFixture {
		private TestServer _server;
		private HttpClient _client;
		private XDocument _doc = null;

		public ClientsideFixture() {
			_server = MvcIntegrationTests.BuildTestServer<StartupWithContainer>();
			_client = _server.CreateClient();
		}

		public async Task<XDocument> GetClientsideMessages() {
			if (_doc != null) return _doc;

			var output = await GetResponse("/Clientside/Inputs");
			_doc= XDocument.Parse(output);
			return _doc;
		}

		public async Task<string> GetResponse(string url,
			string querystring = "") {
			if (!String.IsNullOrEmpty(querystring)) {
				url += "?" + querystring;
			}

			var response = await _client.GetAsync(url);
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadAsStringAsync();
		}

		public async Task<string> GetErrorMessage(string name, string attribute) {
			var doc = await GetClientsideMessages();
			var elem = doc.Root.Elements("input")
				.Where(x => x.Attribute("name").Value == name).SingleOrDefault();

			if (elem == null) {
				throw new Exception("Could not find element with name " + name);
			}

			var attr = elem.Attribute(attribute);

			if (attr == null || string.IsNullOrEmpty(attr.Value)) {
				throw new Exception("Could not find attr " + attribute);
			}

			return attr.Value;
		}


	}

	public class ClientsideMessageTester : IClassFixture<ClientsideFixture> {
		private readonly ClientsideFixture _fixture;

		public ClientsideMessageTester(ClientsideFixture fixture) {
			this._fixture = fixture;
			CultureScope.SetDefaultCulture();

			
		}

		[Fact]
		public async Task NotEmpty_uses_simplified_message_for_clientside_validation() {
			var msg = await _fixture.GetErrorMessage("Required", "data-val-required");
			msg.ShouldEqual("'Required' should not be empty.");
		}

		[Fact]
		public async Task NotNull_uses_simplified_message_for_clientside_validation() {
			var msg = await _fixture.GetErrorMessage("Required2", "data-val-required");
			msg.ShouldEqual("'Required2' should not be empty.");
		}

		[Fact]
		public async Task RegexValidator_uses_simplified_message_for_clientside_validation() {
			var msg = await _fixture.GetErrorMessage("RegEx", "data-val-regex");
			msg.ShouldEqual("'Reg Ex' is not in the correct format.");
		}

		[Fact]
		public async Task EmailValidator_uses_simplified_message_for_clientside_validation() {
			var msg = await _fixture.GetErrorMessage("Email", "data-val-email");
			msg.ShouldEqual("'Email' is not a valid email address.");
		}

		[Fact]
		public async Task LengthValidator_uses_simplified_message_for_clientside_validatation() {
			var msg = await _fixture.GetErrorMessage("Length", "data-val-length");
			msg.ShouldEqual("'Length' must be between 1 and 4 characters.");
		}

		[Fact]
		public async Task MinLengthValidator_uses_simplified_message_for_clientside_validatation() {
			var msg = await _fixture.GetErrorMessage("MinLength", "data-val-minlength");
			msg.ShouldEqual("'Min Length' must be more than 1 characters.");
		}

		[Fact]
		public async Task MaxengthValidator_uses_simplified_message_for_clientside_validatation() {
			var msg = await _fixture.GetErrorMessage("MaxLength", "data-val-maxlength");
			msg.ShouldEqual("'Max Length' must be less than 2 characters.");
		}

		[Fact]
		public async Task ExactLengthValidator_uses_simplified_message_for_clientside_validation() {

			var msg = await _fixture.GetErrorMessage("ExactLength", "data-val-length");
			msg.ShouldEqual("'Exact Length' must be 4 characters in length.");
		}


		[Fact]
		public async Task InclusiveBetween_validator_uses_simplified_message_for_clientside_validation() {
			var msg = await _fixture.GetErrorMessage("Range", "data-val-range");
			msg.ShouldEqual("'Range' must be between 1 and 5.");
		}

        [Fact]
        public async Task GreaterThanOrEqualTo_validator_uses_simplified_message_for_clientside_validation() {
	        var msg = await _fixture.GetErrorMessage("GreaterThanOrEqual", "data-val-range");
	        msg.ShouldEqual("'Greater Than Or Equal' must be greater than or equal to '1'.");
        }

        [Fact]
        public async Task LessThanOrEqualTo_validator_uses_simplified_message_for_clientside_validation() {
			var msg = await _fixture.GetErrorMessage("LessThanOrEqual", "data-val-range");
	        msg.ShouldEqual("'Less Than Or Equal' must be less than or equal to '10'.");
        }

		[Fact]
		public async Task EqualValidator_with_property_uses_simplified_message_for_clientside_validation() {
			var msg = await _fixture.GetErrorMessage("EqualTo", "data-val-equalto");
			msg.ShouldEqual("'Equal To' should be equal to 'Required'.");
		}

		[Fact]
		public async Task Should_not_munge_custom_message() {
			var msg = await _fixture.GetErrorMessage("LengthWithMessage", "data-val-length");
			msg.ShouldEqual("Foo");
		}

		[Fact]
		public async Task Custom_validation_message_with_placeholders() {
			var msg = await _fixture.GetErrorMessage("CustomPlaceholder", "data-val-required");
			msg.ShouldEqual("Custom Placeholder is null.");
		}

		[Fact]
		public async Task Custom_validation_message_for_length() {
			var msg = await _fixture.GetErrorMessage("LengthCustomPlaceholders", "data-val-length");
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
			var msg = await _fixture.GetErrorMessage("CreditCard", "data-val-creditcard");
			msg.ShouldEqual("'Credit Card' is not a valid credit card number.");
		}

		[Fact]
		public async Task Overrides_property_name_for_clientside_rule() {
			var msg = await _fixture.GetErrorMessage("CustomName", "data-val-required");

			msg.ShouldEqual("'Foo' must not be empty.");

		}

		[Fact]
		public async Task Overrides_property_name_for_clientside_rule_using_localized_name() {
			var msg = await _fixture.GetErrorMessage("LocalizedName", "data-val-required");

			msg.ShouldEqual("'Localised Error' must not be empty.");
		}

		[Fact]
		public async Task Overrides_property_name_for_non_nullable_value_type() {
			var msg = await _fixture.GetErrorMessage("CustomNameValueType", "data-val-required");

			msg.ShouldEqual("'Foo' must not be empty.");
		
		}

		[Fact]
		public async Task Falls_back_to_default_message_when_no_context_available_to_custom_message_format() {
			var msg = await _fixture.GetErrorMessage("MessageWithContext", "data-val-required");
			msg.ShouldEqual("'Message With Context' should not be empty.");
		}

//		[Fact]
//	    public async Task Should_only_use_rules_from_Default_ruleset() {
//	        validator.RuleSet("Foo", () => {
//				validator.RuleFor(x => x.Name).NotNull().WithMessage("first");
//	        });
//			validator.RuleFor(x => x.Name).NotNull().WithMessage("second");
//
//			// Client-side rules are only generated from the default ruleset
//			// unless explicitly specified.
//			// so in this case, only the second NotNull should make it through
//
//			var rules = GetClientRules(x => x.Name);
//			rules.Count().ShouldEqual(1);
//			rules.Single().ErrorMessage.ShouldEqual("second");
//	    }
		//
		//		[Fact]
		//		public async Task Should_use_rules_from_specified_ruleset() {
		//			validator.RuleSet("Foo", () => {
		//				validator.RuleFor(x => x.Name).NotNull().WithMessage("first");
		//			});
		//			validator.RuleFor(x => x.Name).NotNull().WithMessage("second");
		//
		//			var filter = new RuleSetForClientSideMessagesAttribute("Foo");
		//			filter.OnActionExecuting(new ActionExecutingContext { HttpContext = controllerContext.HttpContext });
		//
		//			var rules = GetClientRules(x => x.Name);
		//			rules.Count().ShouldEqual(1);
		//			rules.Single().ErrorMessage.ShouldEqual("first");
		//		}
		//
		//		[Fact]
		//		public async Task Should_use_rules_from_multiple_rulesets() {
		//			validator.RuleSet("Foo", () => {
		//				validator.RuleFor(x => x.Name).NotNull().WithMessage("first");
		//			});
		//
		//			validator.RuleSet("Bar", () => {
		//				validator.RuleFor(x => x.Name).NotNull().WithMessage("second");
		//			});
		//
		//			validator.RuleFor(x => x.Name).NotNull().WithMessage("third");
		//
		//			var filter = new RuleSetForClientSideMessagesAttribute("Foo", "Bar");
		//			filter.OnActionExecuting(new ActionExecutingContext {HttpContext = controllerContext.HttpContext});
		//
		//			var rules = GetClientRules(x => x.Name);
		//			rules.Count().ShouldEqual(2);
		//		}

		//		[Fact]
		//		public async Task Should_use_rules_from_default_ruleset_and_specified_ruleset() {
		//			validator.RuleSet("Foo", () => {
		//				validator.RuleFor(x => x.Name).NotNull().WithMessage("first");
		//			});
		//
		//			validator.RuleSet("Bar", () => {
		//				validator.RuleFor(x => x.Name).NotNull().WithMessage("second");
		//			});
		//
		//			validator.RuleFor(x => x.Name).NotNull().WithMessage("third");
		//
		//			var filter = new RuleSetForClientSideMessagesAttribute("Foo", "default");
		//			filter.OnActionExecuting(new ActionExecutingContext { HttpContext = controllerContext.HttpContext });
		//
		//			var rules = GetClientRules(x => x.Name);
		//			rules.Count().ShouldEqual(2);
		//		}



	}

	public class ClientsideModel {
		public string CreditCard { get; set; }
		public string Email { get; set; }
		public string EqualTo { get; set; }
		public string MaxLength { get; set; }
		public string MinLength { get; set; }
		public int Range { get; set; }
		public string RegEx { get; set; }
		public string Required { get; set; }
		public string Length { get; set; }
		public string Required2 { get; set; }
		public string ExactLength { get; set; }
		public int GreaterThan { get; set; }
		public int GreaterThanOrEqual { get; set; }
		public int LessThan { get; set; }
		public int LessThanOrEqual { get; set; }
		public string LengthWithMessage { get; set; }
		public string CustomPlaceholder { get; set; }
		public string LengthCustomPlaceholders { get; set; }
		public string CustomName { get; set; }
		public string MessageWithContext { get; set; }
		public int CustomNameValueType { get; set; }
		public string LocalizedName { get; set; }
	}

	public class ClientsideModelValidator : AbstractValidator<ClientsideModel> {
		public ClientsideModelValidator() {
			RuleFor(x => x.CreditCard).CreditCard();
			RuleFor(x => x.Email).EmailAddress();
			RuleFor(x => x.EqualTo).Equal(x => x.Required);
			RuleFor(x => x.MaxLength).MaximumLength(2);
			RuleFor(x => x.MinLength).MinimumLength(1);
			RuleFor(x => x.Range).InclusiveBetween(1, 5);
			RuleFor(x => x.RegEx).Matches("[0-9]");
			RuleFor(x => x.Required).NotEmpty();
			RuleFor(x => x.Required2).NotEmpty();

			RuleFor(x => x.Length).Length(1, 4);
			RuleFor(x => x.ExactLength).Length(4);
			RuleFor(x => x.LessThan).LessThan(10);
			RuleFor(x => x.LessThanOrEqual).LessThanOrEqualTo(10);
			RuleFor(x => x.GreaterThan).GreaterThan(1);
			RuleFor(x => x.GreaterThanOrEqual).GreaterThanOrEqualTo(1);

			RuleFor(x => x.LengthWithMessage).Length(1, 10).WithMessage("Foo");
			RuleFor(x => x.CustomPlaceholder).NotNull().WithMessage("{PropertyName} is null.");
			RuleFor(x => x.LengthCustomPlaceholders).Length(1, 5).WithMessage("Must be between {MinLength} and {MaxLength}.");

			RuleFor(x => x.CustomName).NotNull().WithName("Foo");
			RuleFor(x => x.LocalizedName).NotNull().WithLocalizedName(() => TestMessages.notnull_error);
			RuleFor(x => x.CustomNameValueType).NotNull().WithName("Foo");
			RuleFor(x => x.MessageWithContext).NotNull().WithMessage(x => $"Foo {x.Required}");

		}
	}
}