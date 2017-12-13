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

namespace FluentValidation.Tests.Mvc5 {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Web;
	using System.Web.Mvc;
	using Internal;
	using Moq;
	using Mvc;
	using Validators;
    using Xunit;

	public class ClientsideMessageTester {
		InlineValidator<TestModel> validator;
		ControllerContext controllerContext;

		public ClientsideMessageTester() {
			System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-us");

            validator = new InlineValidator<TestModel>();
			controllerContext = CreateControllerContext();
		}

		[Fact]
		public void NotNull_uses_simplified_message_for_clientside_validation() {
			validator.RuleFor(x => x.Name).NotNull();
			var clientRule = GetClientRule(x => x.Name);
			clientRule.ErrorMessage.ShouldEqual("'Name' must not be empty.");
		}

		[Fact]
		public void NotEmpty_uses_simplified_message_for_clientside_validation() {
			validator.RuleFor(x => x.Name).NotEmpty();
			var clientRule = GetClientRule(x => x.Name);
			clientRule.ErrorMessage.ShouldEqual("'Name' should not be empty.");
		}

		[Fact]
		public void RegexValidator_uses_simplified_message_for_clientside_validation() {
			validator.RuleFor(x => x.Name).Matches("\\d");
			var clientRule = GetClientRule(x => x.Name);
			clientRule.ErrorMessage.ShouldEqual("'Name' is not in the correct format.");
		}

		[Fact]
		public void EmailValidator_uses_simplified_message_for_clientside_validation() {
			validator.RuleFor(x => x.Name).EmailAddress();
			var clientRule = GetClientRule(x => x.Name);
			clientRule.ErrorMessage.ShouldEqual("'Name' is not a valid email address.");
		}

		[Fact]
		public void LengthValidator_uses_simplified_message_for_clientside_validatation() {
			validator.RuleFor(x => x.Name).Length(1, 10);
			var clientRule = GetClientRule(x => x.Name);
			clientRule.ErrorMessage.ShouldEqual("'Name' must be between 1 and 10 characters.");
		}

		[Fact]
		public void MinLengthValidator_uses_simplified_message_for_clientside_validatation() {
			validator.RuleFor(x => x.Name).MinimumLength(1);
			var clientRule = GetClientRule(x => x.Name);
			clientRule.ErrorMessage.ShouldEqual("The length of 'Name' must be at least 1 characters.");
		}

		[Fact]
		public void MaxengthValidator_uses_simplified_message_for_clientside_validatation() {
			validator.RuleFor(x => x.Name).MaximumLength(10);
			var clientRule = GetClientRule(x => x.Name);
			clientRule.ErrorMessage.ShouldEqual("The length of 'Name' must 10 characters or fewer.");
		}

		[Fact]
		public void ExactLengthValidator_uses_simplified_message_for_clientside_validatation() {
			validator.RuleFor(x => x.Name).Length(10);
			var clientRule = GetClientRule(x => x.Name);
			clientRule.ErrorMessage.ShouldEqual("'Name' must be 10 characters in length.");
		}


		[Fact]
		public void InclusiveBetween_validator_uses_simplified_message_for_clientside_validation() {
			validator.RuleFor(x => x.Id).InclusiveBetween(1, 10);
			var clientRules = GetClientRules(x => x.Id);
			clientRules.Any(x => x.ErrorMessage == "'Id' must be between 1 and 10.").ShouldBeTrue();
		}

        [Fact]
        public void GreaterThanOrEqualTo_validator_uses_simplified_message_for_clientside_validation() {
            validator.RuleFor(x => x.Id).GreaterThanOrEqualTo(5);
            var clientRules = GetClientRules(x => x.Id);
            clientRules.Any(x => x.ErrorMessage == "'Id' must be greater than or equal to '5'.").ShouldBeTrue();
        }

        [Fact]
        public void LessThanOrEqualTo_validator_uses_simplified_message_for_clientside_validation() {
            validator.RuleFor(x => x.Id).LessThanOrEqualTo(50);
            var clientRules = GetClientRules(x => x.Id);
            clientRules.Any(x => x.ErrorMessage == "'Id' must be less than or equal to '50'.").ShouldBeTrue();
        }

		[Fact]
		public void EqualValidator_with_property_uses_simplified_message_for_clientside_validation() {
			validator.RuleFor(x => x.Name).Equal(x => x.Name2);
			var clientRule = GetClientRule(x => x.Name);
			clientRule.ErrorMessage.ShouldEqual("'Name' should be equal to 'Name2'.");
		}

		[Fact]
		public void Should_not_munge_custom_message() {
			validator.RuleFor(x => x.Name).Length(1, 10).WithMessage("Foo");
			var clientRule = GetClientRule(x => x.Name);
			clientRule.ErrorMessage.ShouldEqual("Foo");
		}

		[Fact]
		public void ExactLengthValidator_uses_simplified_message_for_clientside_validation() {
			validator.RuleFor(x => x.Name).Length(5);
			var clientRule = GetClientRule(x => x.Name);
			clientRule.ErrorMessage.ShouldEqual("'Name' must be 5 characters in length.");
		}

		[Fact]
		public void Custom_validation_message_with_placeholders() {
			validator.RuleFor(x => x.Name).NotNull().WithMessage("{PropertyName} is null.");
			var clientRule = GetClientRule(x => x.Name);
			clientRule.ErrorMessage.ShouldEqual("Name is null.");
		}

		[Fact]
		public void Custom_validation_message_for_length() {
			validator.RuleFor(x => x.Name).Length(1, 5).WithMessage("Must be between {MinLength} and {MaxLength}.");
			var clientRule = GetClientRule(x => x.Name);
			clientRule.ErrorMessage.ShouldEqual("Must be between 1 and 5.");
		}

		[Fact]
		public void Supports_custom_clientside_rules_with_IClientValidatable() {
			validator.RuleFor(x => x.Name).SetValidator(new TestPropertyValidator());
			var clientRule = GetClientRule(x => x.Name);
			clientRule.ErrorMessage.ShouldEqual("foo");
		}

		[Fact]
		public void CreditCard_creates_clientside_message() {
			validator.RuleFor(x => x.Name).CreditCard();
			var clientrule = GetClientRule(x => x.Name);
			clientrule.ErrorMessage.ShouldEqual("'Name' is not a valid credit card number.");
		}

		[Fact]
		public void Overrides_property_name_for_clientside_rule() {
			validator.RuleFor(x => x.Name).NotNull().WithName("Foo");
			var clientRule = GetClientRule(x => x.Name);
			clientRule.ErrorMessage.ShouldEqual("'Foo' must not be empty.");

		}

		[Fact]
		public void Overrides_property_name_for_clientside_rule_using_localized_name() {
			validator.RuleFor(x => x.Name).NotNull().WithLocalizedName(() => TestMessages.notnull_error);
			var clientRule = GetClientRule(x => x.Name);
			clientRule.ErrorMessage.ShouldEqual("'Localised Error' must not be empty.");
		}

		[Fact]
		public void Overrides_property_name_for_non_nullable_value_type() {
			validator.RuleFor(x => x.Id).NotNull().WithName("Foo");
			var clientRule = GetClientRule(x => x.Id);
			clientRule.ErrorMessage.ShouldEqual("'Foo' must not be empty.");
		
		}

		[Fact]
		public void Falls_back_to_default_message_when_no_context_available_to_custom_message_format() {
			validator.RuleFor(x => x.Name).NotNull().WithMessage(x => $"Foo {x.Name}");
			var clientRule = GetClientRule(x => x.Name);
			clientRule.ErrorMessage.ShouldEqual("'Name' should not be empty.");
		}

		[Fact]
	    public void Should_only_use_rules_from_Default_ruleset() {
	        validator.RuleSet("Foo", () => {
				validator.RuleFor(x => x.Name).NotNull().WithMessage("first");
	        });
			validator.RuleFor(x => x.Name).NotNull().WithMessage("second");

			// Client-side rules are only generated from the default ruleset
			// unless explicitly specified.
			// so in this case, only the second NotNull should make it through

			var rules = GetClientRules(x => x.Name);
			rules.Count().ShouldEqual(1);
			rules.Single().ErrorMessage.ShouldEqual("second");
	    }

		[Fact]
		public void Should_use_rules_from_specified_ruleset() {
			validator.RuleSet("Foo", () => {
				validator.RuleFor(x => x.Name).NotNull().WithMessage("first");
			});
			validator.RuleFor(x => x.Name).NotNull().WithMessage("second");

			var filter = new RuleSetForClientSideMessagesAttribute("Foo");
			filter.OnActionExecuting(new ActionExecutingContext { HttpContext = controllerContext.HttpContext });

			var rules = GetClientRules(x => x.Name);
			rules.Count().ShouldEqual(1);
			rules.Single().ErrorMessage.ShouldEqual("first");
		}

		[Fact]
		public void Should_use_rules_from_multiple_rulesets() {
			validator.RuleSet("Foo", () => {
				validator.RuleFor(x => x.Name).NotNull().WithMessage("first");
			});

			validator.RuleSet("Bar", () => {
				validator.RuleFor(x => x.Name).NotNull().WithMessage("second");
			});

			validator.RuleFor(x => x.Name).NotNull().WithMessage("third");

			var filter = new RuleSetForClientSideMessagesAttribute("Foo", "Bar");
			filter.OnActionExecuting(new ActionExecutingContext {HttpContext = controllerContext.HttpContext});

			var rules = GetClientRules(x => x.Name);
			rules.Count().ShouldEqual(2);
		}

		[Fact]
		public void Should_use_rules_from_default_ruleset_and_specified_ruleset() {
			validator.RuleSet("Foo", () => {
				validator.RuleFor(x => x.Name).NotNull().WithMessage("first");
			});

			validator.RuleSet("Bar", () => {
				validator.RuleFor(x => x.Name).NotNull().WithMessage("second");
			});

			validator.RuleFor(x => x.Name).NotNull().WithMessage("third");

			var filter = new RuleSetForClientSideMessagesAttribute("Foo", "default");
			filter.OnActionExecuting(new ActionExecutingContext { HttpContext = controllerContext.HttpContext });

			var rules = GetClientRules(x => x.Name);
			rules.Count().ShouldEqual(2);
		}

		private ModelClientValidationRule GetClientRule(Expression<Func<TestModel, object>> expression) {
			var propertyName = expression.GetMember().Name;
			var metadata = new DataAnnotationsModelMetadataProvider().GetMetadataForProperty(null, typeof(TestModel), propertyName);

			var factory = new Mock<IValidatorFactory>();
			factory.Setup(x => x.GetValidator(typeof(TestModel))).Returns(validator);

			var provider = new FluentValidationModelValidatorProvider(factory.Object);
			var propertyValidator = provider.GetValidators(metadata, controllerContext).Single();

			var clientRule = propertyValidator.GetClientValidationRules().Single();
			return clientRule;
		}

		private IEnumerable<ModelClientValidationRule> GetClientRules(Expression<Func<TestModel, object>> expression ) {
			var propertyName = expression.GetMember().Name;
			var metadata = new DataAnnotationsModelMetadataProvider().GetMetadataForProperty(null, typeof(TestModel), propertyName);

			var factory = new Mock<IValidatorFactory>();
			factory.Setup(x => x.GetValidator(typeof(TestModel))).Returns(validator);

			var provider = new FluentValidationModelValidatorProvider(factory.Object);
			var propertyValidators = provider.GetValidators(metadata, controllerContext);

			return (propertyValidators.SelectMany(x => x.GetClientValidationRules())).ToList();
		}

		private ControllerContext CreateControllerContext() {
			var httpContext = new Mock<HttpContextBase>();
			httpContext.Setup(x => x.Items).Returns(new Hashtable());
			return new ControllerContext { HttpContext = httpContext.Object };
		}

		private class TestModel {
			public string Name { get; set; }
			public string Name2 { get; set; }
			public int Id { get; set; }

		}

		private class TestPropertyValidator : PropertyValidator, IClientValidatable {
			public TestPropertyValidator()
				: base("foo") {
				
			}

			protected override bool IsValid(PropertyValidatorContext context) {
				return true;
			}

			public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context) {
				yield return new ModelClientValidationRule { ErrorMessage = this.ErrorMessageSource.GetString(null) };
			}
		}
	}
}