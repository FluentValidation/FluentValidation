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

namespace FluentValidation.Tests {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using Internal;
	using Moq;
	using Xunit;
	using Resources;
	using Results;
	using Validators;
	using System.Reflection;
	using Microsoft.Win32;

	public class RuleBuilderTests {
		IRuleBuilderInitial<Person, string> builder;
		private InlineValidator<Person> _validator;
		private PropertyRule _rule;


		public  RuleBuilderTests() {
			_validator = new InlineValidator<Person>();
			builder = _validator.RuleFor(x => x.Surname);
			builder.Configure(rule => _rule = rule);
		}

		[Fact]
		public void Should_build_property_name() {
			_rule.PropertyName.ShouldEqual("Surname");
		}

		[Fact]
		public void Should_compile_expression() {
			var person = new Person {Surname = "Foo"};
			_rule.PropertyFunc(person).ShouldEqual("Foo");
		}

		[Fact]
		public void Adding_a_validator_should_return_builder() {
			var builderWithOptions = builder.SetValidator(new TestPropertyValidator());
			builderWithOptions.ShouldBeTheSameAs(builder);
		}

		[Fact]
		public void Adding_a_validator_should_store_validator() {
			var validator = new TestPropertyValidator();
			builder.SetValidator(validator);
			_rule.CurrentValidator.ShouldBeTheSameAs(validator);
		}

		[Fact]
		public void Should_set_custom_property_name() {
			builder.SetValidator(new TestPropertyValidator()).WithName("Foo");
			Assert.Equal(_rule.GetDisplayName(null), "Foo");
		}

		[Fact]
		public void Should_set_custom_error() {
			builder.SetValidator(new TestPropertyValidator()).WithMessage("Bar");
			_rule.CurrentValidator.Options.GetErrorMessage(null).ShouldEqual("Bar");
		}

		[Fact]
		public void Should_throw_if_validator_is_null() {
			typeof(ArgumentNullException).ShouldBeThrownBy(() => builder.SetValidator((IPropertyValidator)null));
		}

		[Fact]
		public void Should_throw_if_overriding_validator_is_null() {
			typeof(ArgumentNullException).ShouldBeThrownBy(() => builder.SetValidator((IValidator<string>)null));
		}

		[Fact]
		public void Should_throw_if_overriding_validator_provider_is_null() {
			typeof (ArgumentNullException).ShouldBeThrownBy(() => builder.SetValidator((Func<Person, IValidator<string>>) null));
		}

		[Fact]
		public void Should_throw_if_message_is_null() {
			typeof(ArgumentNullException).ShouldBeThrownBy(() => builder.SetValidator(new TestPropertyValidator()).WithMessage((string)null));
		}

		[Fact]
		public void Should_throw_if_property_name_is_null() {
			typeof(ArgumentNullException).ShouldBeThrownBy(() => builder.SetValidator(new TestPropertyValidator()).WithName((string)null));
		}

		[Fact]
		public void Should_throw_when_predicate_is_null() {
			typeof(ArgumentNullException).ShouldBeThrownBy(() => builder.SetValidator(new TestPropertyValidator()).When((Func<Person, bool>)null));
		}

		[Fact]
		public void Should_throw_when_context_predicate_is_null() {
			typeof(ArgumentNullException).ShouldBeThrownBy(() => builder.SetValidator(new TestPropertyValidator()).When((Func<Person, ValidationContext<Person>, bool>)null));
		}

		[Fact]
		public void Should_throw_when_async_predicate_is_null() {
			typeof (ArgumentNullException).ShouldBeThrownBy(() => builder.SetValidator(new TestPropertyValidator()).WhenAsync((Func<Person, CancellationToken, Task<bool>>) null));
		}

		[Fact]
		public void Should_throw_when_inverse_context_predicate_is_null() {
			typeof(ArgumentNullException).ShouldBeThrownBy(() => builder.SetValidator(new TestPropertyValidator()).Unless((Func<Person, ValidationContext<Person>, bool>)null));
		}

		[Fact]
		public void Should_throw_when_inverse_predicate_is_null() {
			typeof(ArgumentNullException).ShouldBeThrownBy(() => builder.SetValidator(new TestPropertyValidator()).Unless((Func<Person, bool>)null));
		}

		[Fact]
		public void Should_throw_when_async_inverse_predicate_is_null() {
			typeof (ArgumentNullException).ShouldBeThrownBy(() => builder.SetValidator(new TestPropertyValidator()).UnlessAsync((Func<Person, CancellationToken, Task<bool>>) null));
		}

		[Fact]
		public void Calling_validate_should_delegate_to_underlying_validator() {
			var person = new Person {Surname = "Foo"};
			var validator = new Mock<IPropertyValidator>();
			validator.Setup(x => x.Options).Returns(new PropertyValidatorOptions());
			builder.SetValidator(validator.Object);

			_rule.Validate(new ValidationContext<Person>(person, new PropertyChain(), new DefaultValidatorSelector())).ToList();

			validator.Verify(x => x.Validate(It.Is<PropertyValidatorContext>(c => (string)c.PropertyValue == "Foo")));

		}
		[Fact]
		public async Task Calling_ValidateAsync_should_delegate_to_underlying_sync_validator() {
			var person = new Person { Surname = "Foo" };
			var validator = new Mock<IPropertyValidator>();
			validator.Setup(x => x.Options).Returns(new PropertyValidatorOptions());
			builder.SetValidator(validator.Object);

			await _rule.ValidateAsync(new ValidationContext<Person>(person, new PropertyChain(), new DefaultValidatorSelector()), new CancellationToken());

			validator.Verify(x => x.Validate(It.Is<PropertyValidatorContext>(c => (string)c.PropertyValue == "Foo")));


		}
		[Fact]
		public async Task Calling_ValidateAsync_should_delegate_to_underlying_async_validator() {
			var person = new Person { Surname = "Foo" };
			TaskCompletionSource<IEnumerable<ValidationFailure>> tcs = new TaskCompletionSource<IEnumerable<ValidationFailure>>();
			tcs.SetResult(Enumerable.Empty<ValidationFailure>());

			var validator = new Mock<PropertyValidator>(MockBehavior.Loose, ValidatorOptions.Global.LanguageManager.GetStringForValidator<AsyncPredicateValidator>()) {CallBase = true};
			validator.Setup(x => x.ShouldValidateAsynchronously(It.IsAny<IValidationContext>())).Returns(true);
			validator.Setup(v => v.ValidateAsync(It.IsAny<PropertyValidatorContext>(), It.IsAny<CancellationToken>())).Returns(tcs.Task);
			builder.SetValidator(validator.Object);

			await _rule.ValidateAsync(new ValidationContext<Person>(person, new PropertyChain(), new DefaultValidatorSelector()), new CancellationToken());

			validator.Verify(x => x.ValidateAsync(It.Is<PropertyValidatorContext>(c => (string)c.PropertyValue == "Foo"), It.IsAny<CancellationToken>()));

		}


		[Fact]
		public void PropertyDescription_should_return_property_name_split() {
			var builder = _validator.RuleFor(x => x.DateOfBirth);
			PropertyRule rule = null;
			builder.Configure(r => rule = r);
			rule.GetDisplayName(null).ShouldEqual("Date Of Birth");
		}

		[Fact]
		public void PropertyDescription_should_return_custom_property_name() {
			var builder = _validator.RuleFor(x => x.DateOfBirth);
			PropertyRule rule = null;
			builder.Configure(r => rule = r);
			builder.NotEqual(default(DateTime)).WithName("Foo");
			rule.GetDisplayName(null).ShouldEqual("Foo");
		}

		[Fact]
		public void Nullable_object_with_condition_should_not_throw() {
			var builder = _validator.RuleFor(x => x.NullableInt.Value);
			PropertyRule rule = null;
			builder.Configure(r => rule = r);

			builder.GreaterThanOrEqualTo(3).When(x => x.NullableInt != null);
			rule.Validate(new ValidationContext<Person>(new Person(), new PropertyChain(), new DefaultValidatorSelector()));
		}

		[Fact]
		public void Nullable_object_with_async_condition_should_not_throw() {
			var builder = _validator.RuleFor(x => x.NullableInt.Value);
			PropertyRule rule = null;
			builder.Configure(r => rule = r);

			builder.GreaterThanOrEqualTo(3).WhenAsync((x,c) => Task.FromResult(x.NullableInt != null));
			rule.Validate(new ValidationContext<Person>(new Person(), new PropertyChain(), new DefaultValidatorSelector()));
		}

		[Fact]
		public void Rule_for_a_non_memberexpression_should_not_generate_property_name() {
			var builder = _validator.RuleFor(x => x.CalculateSalary());
			PropertyRule rule = null;
			builder.Configure(r => rule = r);
			rule.GetDisplayName(null).ShouldBeNull();
			rule.PropertyName.ShouldBeNull();
		}

		[Fact]
		public void Property_should_return_property_being_validated() {
			var property = typeof(Person).GetProperty("Surname");
			_rule.Member.ShouldEqual(property);
		}

		[Fact]
		public void Property_should_return_null_when_it_is_not_a_property_being_validated() {
			builder = _validator.RuleFor(x => "Foo");
			PropertyRule rule = null;
			builder.Configure(r => rule = r);
			rule.Member.ShouldBeNull();
		}

		[Fact]
		public void Result_should_use_custom_property_name_when_no_property_name_can_be_determined() {
			var builder = _validator.RuleFor(x => x.CalculateSalary());
			builder.GreaterThan(100).WithName("Foo");
			PropertyRule rule = null;
			builder.Configure(r => rule = r);
			var results = rule.Validate(new ValidationContext<Person>(new Person(), new PropertyChain(), new DefaultValidatorSelector()));
			results.Single().PropertyName.ShouldEqual("Foo");
		}

		[Fact]
		public void Conditional_child_validator_should_register_with_validator_type_not_property() {
			var builder = _validator.RuleFor(x => x.Address);
			builder.SetValidator((Person person) => new NoopAddressValidator());
			PropertyRule rule = null;
			builder.Configure(r => rule = r);

			rule.Validators.OfType<IChildValidatorAdaptor>().Single().ValidatorType.ShouldEqual(typeof(NoopAddressValidator));
		}

		class NoopAddressValidator : AbstractValidator<Address> {
		}

		class TestPropertyValidator : PropertyValidator {

			protected override bool IsValid(PropertyValidatorContext context) {
				return true;
			}

			protected override string GetDefaultMessageTemplate() {
				return Localized(nameof(NotNullValidator));
			}
		}
	}
}
