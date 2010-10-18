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
// The latest version of this file can be found at http://www.codeplex.com/FluentValidation
#endregion

namespace FluentValidation.Tests {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Internal;
	using Moq;
	using NUnit.Framework;
	using Resources;
	using Results;
	using Validators;

	[TestFixture]
	public class RuleBuilderTests {
		RuleBuilder<Person, string> builder;

		[SetUp]
		public void Setup() {
			var rule = PropertyRule<Person>.Create(x => x.Surname);
			builder = new RuleBuilder<Person, string>(rule);
		}

		[Test]
		public void Should_build_property_name() {
			builder.Rule.PropertyName.ShouldEqual("Surname");
		}

		[Test]
		public void Should_compile_expression() {
			var person = new Person {Surname = "Foo"};
			builder.Rule.PropertyFunc(person).ShouldEqual("Foo");
		}

		[Test]
		public void Adding_a_validator_should_return_builder() {
			var builderWithOptions = builder.SetValidator(new TestPropertyValidator());
			builderWithOptions.ShouldBeTheSameAs(builder);
		}

		[Test]
		public void Adding_a_validator_should_store_validator() {
			var validator = new TestPropertyValidator();
			builder.SetValidator(validator);
			builder.Rule.CurrentValidator.ShouldBeTheSameAs(validator);
		}

		[Test]
		public void Should_set_cutom_property_name() {
			builder.SetValidator(new TestPropertyValidator()).WithName("Foo");
			Assert.That(builder.Rule.CustomPropertyName, Is.EqualTo("Foo"));
		}

		[Test]
		public void Should_set_custom_error() {
			builder.SetValidator(new TestPropertyValidator()).WithMessage("Bar");
			builder.Rule.CurrentValidator.ErrorMessageSource.BuildErrorMessage().ShouldEqual("Bar");
		}

		[Test]
		public void Should_throw_if_validator_is_null() {
			typeof(ArgumentNullException).ShouldBeThrownBy(() => builder.SetValidator((IPropertyValidator)null));
		}

		[Test]
		public void Should_throw_if_overriding_validator_is_null() {
			typeof(ArgumentNullException).ShouldBeThrownBy(() => builder.SetValidator((IValidator<string>)null));
		}

		[Test]
		public void Should_throw_if_message_is_null() {
			typeof(ArgumentNullException).ShouldBeThrownBy(() => builder.SetValidator(new TestPropertyValidator()).WithMessage(null));
		}

		[Test]
		public void Should_throw_if_property_name_is_null() {
			typeof(ArgumentNullException).ShouldBeThrownBy(() => builder.SetValidator(new TestPropertyValidator()).WithName(null));
		}

		[Test]
		public void Should_throw_when_predicate_is_null() {
			typeof(ArgumentNullException).ShouldBeThrownBy(() => builder.SetValidator(new TestPropertyValidator()).When(null));
		}

		[Test]
		public void Should_throw_when_inverse_predicate_is_null() {
			typeof(ArgumentNullException).ShouldBeThrownBy(() => builder.SetValidator(new TestPropertyValidator()).Unless(null));
		}

		[Test]
		public void Calling_when_should_replace_current_validator_with_predicate_validator() {
			var validator = new TestPropertyValidator();
			builder.SetValidator(validator).When(x => true);
			builder.Rule.CurrentValidator.ShouldBe<DelegatingValidator>();

			var predicateValidator = (DelegatingValidator)builder.Rule.CurrentValidator;
			predicateValidator.InnerValidator.ShouldBeTheSameAs(validator);
		}

		[Test]
		public void Calling_validate_should_delegate_to_underlying_validator() {
			var person = new Person {Surname = "Foo"};
			var validator = new Mock<IPropertyValidator>();
			builder.SetValidator(validator.Object);

			builder.Rule.Validate(new ValidationContext<Person>(person, new PropertyChain(), new DefaultValidatorSelector())).ToList();

			validator.Verify(x => x.Validate(It.Is<PropertyValidatorContext>(c => c.PropertyValue == "Foo")));
		}

		[Test]
		public void PropertyDescription_should_return_property_name_split() {
			var builder = new RuleBuilder<Person, DateTime>(PropertyRule<Person>.Create(x => x.DateOfBirth));
			builder.Rule.PropertyDescription.ShouldEqual("Date Of Birth");
		}

		[Test]
		public void PropertyDescription_should_return_custom_property_name() {
			var builder = new RuleBuilder<Person, DateTime>(PropertyRule<Person>.Create(x => x.DateOfBirth));
			builder.NotEqual(default(DateTime)).WithName("Foo");
			builder.Rule.PropertyDescription.ShouldEqual("Foo");
		}

		[Test]
		public void Nullable_object_with_condition_should_not_throw() {
			var builder = new RuleBuilder<Person, int>(PropertyRule<Person>.Create(x => x.NullableInt.Value));
			builder.GreaterThanOrEqualTo(3).When(x => x.NullableInt != null);
			builder.Rule.Validate(new ValidationContext<Person>(new Person(), new PropertyChain(), new DefaultValidatorSelector()));
		}

		[Test]
		public void Rule_for_a_non_memberexpression_should_not_generate_property_name() {
			var builder = new RuleBuilder<Person, int>(PropertyRule<Person>.Create(x => x.CalculateSalary()));
			builder.Rule.PropertyDescription.ShouldBeNull();
			builder.Rule.PropertyName.ShouldBeNull();
		}

		[Test]
		public void Should_throw_when_property_name_is_null() {
			var builder = new RuleBuilder<Person, int>(PropertyRule<Person>.Create(x => x.CalculateSalary()));
			builder.GreaterThan(4);

			var ex = typeof(InvalidOperationException).ShouldBeThrownBy(() => builder.Rule.Validate(new ValidationContext<Person>(new Person(), new PropertyChain(), new DefaultValidatorSelector())).ToList());
			ex.Message.ShouldEqual("Property name could not be automatically determined for expression x => x.CalculateSalary(). Please specify either a custom property name by calling 'WithName'.");
		}

		[Test]
		public void Property_should_return_property_being_validated() {
			var property = typeof(Person).GetProperty("Surname");
			builder.Rule.Member.ShouldEqual(property);
		}

		[Test]
		public void Property_should_return_null_when_it_is_not_a_property_being_validated() {
			builder = new RuleBuilder<Person, string>(PropertyRule<Person>.Create(x => "Foo"));
			builder.Rule.Member.ShouldBeNull();
		}

		[Test]
		public void Result_should_use_custom_property_name_when_no_property_name_can_be_determined() {
			var builder = new RuleBuilder<Person, int>(PropertyRule<Person>.Create(x => x.CalculateSalary()));
			builder.GreaterThan(100).WithName("Foo");

			var results = builder.Rule.Validate(new ValidationContext<Person>(new Person(), new PropertyChain(), new DefaultValidatorSelector()));
			results.Single().PropertyName.ShouldEqual("Foo");
		}

		class TestPropertyValidator : PropertyValidator {
			public TestPropertyValidator() : base(() => Messages.notnull_error) {
				
			}

			protected override bool IsValid(PropertyValidatorContext context) {
				return true;
			}
		}
	}
}