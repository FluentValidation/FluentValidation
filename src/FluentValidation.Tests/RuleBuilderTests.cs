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
// The latest version of this file can be found at https://github.com/jeremyskinner/FluentValidation
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
	
	public class RuleBuilderTests {
		RuleBuilder<Person, string> builder;

		public  RuleBuilderTests() {
			var rule = PropertyRule.Create<Person,string>(x => x.Surname);
			builder = new RuleBuilder<Person, string>(rule,null);
		}

		[Fact]
		public void Should_build_property_name() {
			builder.Rule.PropertyName.ShouldEqual("Surname");
		}

		[Fact]
		public void Should_compile_expression() {
			var person = new Person {Surname = "Foo"};
			builder.Rule.PropertyFunc(person).ShouldEqual("Foo");
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
			builder.Rule.CurrentValidator.ShouldBeTheSameAs(validator);
		}

		[Fact]
		public void Should_set_custom_property_name() {
			builder.SetValidator(new TestPropertyValidator()).WithName("Foo");
			Assert.Equal(builder.Rule.DisplayName.GetString(null), "Foo");
		}

		[Fact]
		public void Should_set_custom_error() {
			builder.SetValidator(new TestPropertyValidator()).WithMessage("Bar");
			builder.Rule.CurrentValidator.ErrorMessageSource.GetString(null).ShouldEqual("Bar");
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
			typeof(ArgumentNullException).ShouldBeThrownBy(() => builder.SetValidator(new TestPropertyValidator()).When(null));
		}

		[Fact]
		public void Should_throw_when_async_predicate_is_null() {
			typeof (ArgumentNullException).ShouldBeThrownBy(() => builder.SetValidator(new TestPropertyValidator()).WhenAsync((Func<Person, CancellationToken, Task<bool>>) null));
		}

		[Fact]
		public void Should_throw_when_inverse_predicate_is_null() {
			typeof(ArgumentNullException).ShouldBeThrownBy(() => builder.SetValidator(new TestPropertyValidator()).Unless(null));
		}

		[Fact]
		public void Should_throw_when_async_inverse_predicate_is_null() {
			typeof (ArgumentNullException).ShouldBeThrownBy(() => builder.SetValidator(new TestPropertyValidator()).UnlessAsync((Func<Person, CancellationToken, Task<bool>>) null));
		}

		[Fact]
		public void Calling_when_should_replace_current_validator_with_predicate_validator() {
			var validator = new TestPropertyValidator();
			builder.SetValidator(validator).When(x => true);
			builder.Rule.CurrentValidator.ShouldBe<DelegatingValidator>();

			var predicateValidator = (DelegatingValidator)builder.Rule.CurrentValidator;
			predicateValidator.InnerValidator.ShouldBeTheSameAs(validator);
		}

		[Fact]
		public void Calling_when_async_should_replace_current_validator_with_predicate_validator() {
			var validator = new TestPropertyValidator();
			builder.SetValidator(validator).WhenAsync(async x => true);
			builder.Rule.CurrentValidator.ShouldBe<DelegatingValidator>();

			var predicateValidator = (DelegatingValidator) builder.Rule.CurrentValidator;
			predicateValidator.InnerValidator.ShouldBeTheSameAs(validator);
		}
		[Fact]
		public void Calling_validate_should_delegate_to_underlying_validator() {
			var person = new Person {Surname = "Foo"};
			var validator = new Mock<IPropertyValidator>();
			builder.SetValidator(validator.Object);

			builder.Rule.Validate(new ValidationContext<Person>(person, new PropertyChain(), new DefaultValidatorSelector())).ToList();

			validator.Verify(x => x.Validate(It.Is<PropertyValidatorContext>(c => (string)c.PropertyValue == "Foo")));

		}
		[Fact]
		public void Calling_ValidateAsync_should_delegate_to_underlying_sync_validator() {
			var person = new Person { Surname = "Foo" };
			var validator = new Mock<IPropertyValidator>();
			builder.SetValidator(validator.Object);

			builder.Rule.ValidateAsync(new ValidationContext<Person>(person, new PropertyChain(), new DefaultValidatorSelector()), new CancellationToken()).Result.ToList();

			validator.Verify(x => x.Validate(It.Is<PropertyValidatorContext>(c => (string)c.PropertyValue == "Foo")));


		}
		[Fact]
		public void Calling_ValidateAsync_should_delegate_to_underlying_async_validator()
		{
			var person = new Person { Surname = "Foo" };
			TaskCompletionSource<IEnumerable<ValidationFailure>> tcs = new TaskCompletionSource<IEnumerable<ValidationFailure>>();
			tcs.SetResult(Enumerable.Empty<ValidationFailure>());

			var validator = new Mock<AsyncValidatorBase>(MockBehavior.Loose, ValidatorOptions.LanguageManager.GetStringForValidator<AsyncPredicateValidator>()) {CallBase = true};
			validator.Setup(v => v.ValidateAsync(It.IsAny<PropertyValidatorContext>(), It.IsAny<CancellationToken>())).Returns(tcs.Task);
			builder.SetValidator(validator.Object);

			builder.Rule.ValidateAsync(new ValidationContext<Person>(person, new PropertyChain(), new DefaultValidatorSelector()), new CancellationToken()).Result.ToList();

			validator.Verify(x => x.ValidateAsync(It.Is<PropertyValidatorContext>(c => (string)c.PropertyValue == "Foo"), It.IsAny<CancellationToken>()));

		}
       

		[Fact]
		public void PropertyDescription_should_return_property_name_split() {
			var builder = new RuleBuilder<Person, DateTime>(PropertyRule.Create<Person, DateTime>(x => x.DateOfBirth), null);
			builder.Rule.GetDisplayName().ShouldEqual("Date Of Birth");
		}

		[Fact]
		public void PropertyDescription_should_return_custom_property_name() {
			var builder = new RuleBuilder<Person, DateTime>(PropertyRule.Create<Person, DateTime>(x => x.DateOfBirth),null);
			builder.NotEqual(default(DateTime)).WithName("Foo");
			builder.Rule.GetDisplayName().ShouldEqual("Foo");
		}

		[Fact]
		public void Nullable_object_with_condition_should_not_throw()
		{
			var builder = new RuleBuilder<Person, int>(PropertyRule.Create<Person, int>(x => x.NullableInt.Value),null);
			builder.GreaterThanOrEqualTo(3).When(x => x.NullableInt != null);
			builder.Rule.Validate(new ValidationContext<Person>(new Person(), new PropertyChain(), new DefaultValidatorSelector()));
		}

		[Fact]
		public void Nullable_object_with_async_condition_should_not_throw()
		{
			var builder = new RuleBuilder<Person, int>(PropertyRule.Create<Person, int>(x => x.NullableInt.Value),null);
			builder.GreaterThanOrEqualTo(3).WhenAsync(async x => x.NullableInt != null);
			builder.Rule.Validate(new ValidationContext<Person>(new Person(), new PropertyChain(), new DefaultValidatorSelector()));
		}

		[Fact]
		public void Rule_for_a_non_memberexpression_should_not_generate_property_name() {
			var builder = new RuleBuilder<Person, int>(PropertyRule.Create<Person, int>(x => x.CalculateSalary()),null);
			builder.Rule.GetDisplayName().ShouldBeNull();
			builder.Rule.PropertyName.ShouldBeNull();
		}

		[Fact]
		public void Property_should_return_property_being_validated() {
			var property = typeof(Person).GetProperty("Surname");
			builder.Rule.Member.ShouldEqual(property);
		}

		[Fact]
		public void Property_should_return_null_when_it_is_not_a_property_being_validated() {
			builder = new RuleBuilder<Person, string>(PropertyRule.Create<Person, string>(x => "Foo"),null);
			builder.Rule.Member.ShouldBeNull();
		}

		[Fact]
		public void Result_should_use_custom_property_name_when_no_property_name_can_be_determined() {
			var builder = new RuleBuilder<Person, int>(PropertyRule.Create<Person, int>(x => x.CalculateSalary()),null);
			builder.GreaterThan(100).WithName("Foo");

			var results = builder.Rule.Validate(new ValidationContext<Person>(new Person(), new PropertyChain(), new DefaultValidatorSelector()));
			results.Single().PropertyName.ShouldEqual("Foo");
		}

		[Fact]
		public void Conditional_child_validator_should_register_with_validator_type_not_property() {
			var builder = new RuleBuilder<Person, Address>(PropertyRule.Create<Person, Address>(x => x.Address),null);
			builder.SetValidator(person => new NoopAddressValidator());

			builder.Rule.Validators.OfType<ChildValidatorAdaptor>().Single().ValidatorType.ShouldEqual(typeof(NoopAddressValidator));
		}

		class NoopAddressValidator : AbstractValidator<Address> {
		}

		class TestPropertyValidator : PropertyValidator {
			public TestPropertyValidator() : base(new LanguageStringSource(nameof(NotNullValidator))) {
				
			}

			protected override bool IsValid(PropertyValidatorContext context) {
				return true;
			}
		}
	}
}