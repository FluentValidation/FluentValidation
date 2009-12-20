#region License
// Copyright 2008-2009 Jeremy Skinner (http://www.jeremyskinner.co.uk)
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
	using Results;
	using Validators;

	[TestFixture]
	public class RuleBuilderTests {
		RuleBuilder<Person, string> builder;

		[SetUp]
		public void Setup() {
			builder = new RuleBuilder<Person, string>(x => x.Surname);
		}

		[Test]
		public void Should_build_property_name() {
			builder.Model.PropertyName.ShouldEqual("Surname");
		}

		[Test]
		public void Should_compile_expression() {
			var person = new Person {Surname = "Foo"};
			builder.Model.PropertyFunc(person).ShouldEqual("Foo");
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
			builder.Cast<ISimplePropertyRule<Person>>().Single().Validator.ShouldBeTheSameAs(validator);
		}

		[Test]
		public void Should_set_cutom_property_name() {
			builder.SetValidator(new TestPropertyValidator()).WithName("Foo");
			Assert.That(builder.Model.CustomPropertyName, Is.EqualTo("Foo"));
		}

		[Test]
		public void Should_set_custom_error() {
			builder.SetValidator(new TestPropertyValidator()).WithMessage("Bar");
			builder.Cast<PropertyRule<Person, string>>().Single().CustomValidationMessage.ShouldEqual("Bar");
		}

		[Test]
		public void Should_throw_if_validator_is_null() {
			typeof(ArgumentNullException).ShouldBeThrownBy(() => builder.SetValidator((IPropertyValidator<Person, string>)null));
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
			builder.Cast<ISimplePropertyRule<Person>>().Single().Validator.ShouldBe<DelegatingValidator<Person, string>>();

			var predicateValidator = (DelegatingValidator<Person, string>)builder.Cast<ISimplePropertyRule<Person>>().Single().Validator;
			predicateValidator.InnerValidator.ShouldBeTheSameAs(validator);
		}

		[Test]
		public void Calling_validate_should_delegate_to_underlying_validator() {
			var person = new Person {Surname = "Foo"};
			var validator = new Mock<IPropertyValidator<Person, string>>();
			builder.SetValidator(validator.Object);

			builder.Single().Validate(new ValidationContext<Person>(person, new PropertyChain(), new DefaultValidatorSelector())).ToList();

			validator.Verify(x => x.Validate(It.Is<PropertyValidatorContext<Person, string>>(c => c.PropertyValue == "Foo")));
		}

		[Test]
		public void PropertyDescription_should_return_property_name_split() {
			var builder = new RuleBuilder<Person, DateTime>(x => x.DateOfBirth);
			builder.Model.PropertyDescription.ShouldEqual("Date Of Birth");
		}

		[Test]
		public void PropertyDescription_should_return_custom_property_name() {
			var builder = new RuleBuilder<Person, DateTime>(x => x.DateOfBirth);
			builder.NotEqual(default(DateTime)).WithName("Foo");
			builder.Model.PropertyDescription.ShouldEqual("Foo");
		}

		[Test]
		public void Nullable_object_with_condition_should_not_throw() {
			var builder = new RuleBuilder<Person, int>(x => x.NullableInt.Value);
			builder.GreaterThanOrEqualTo(3).When(x => x.NullableInt != null);
			builder.Single().Validate(new ValidationContext<Person>(new Person(), new PropertyChain(), new DefaultValidatorSelector()));
		}

		[Test]
		public void Rule_for_a_non_memberexpression_should_not_generate_property_name() {
			var builder = new RuleBuilder<Person, int>(x => x.CalculateSalary());
			builder.Model.PropertyDescription.ShouldBeNull();
			builder.Model.PropertyName.ShouldBeNull();
		}

		[Test]
		public void Should_throw_when_no_validator_specified() {
			typeof(InvalidOperationException).ShouldBeThrownBy(() => builder.Single().Validate(new ValidationContext<Person>(new Person(), new PropertyChain(), new DefaultValidatorSelector())));
		}

		[Test]
		public void Should_throw_when_property_name_is_null() {
			var builder = new RuleBuilder<Person, int>(x => x.CalculateSalary());
			builder.GreaterThan(4);

			var ex = typeof(InvalidOperationException).ShouldBeThrownBy(() => builder.Single().Validate(new ValidationContext<Person>(new Person(), new PropertyChain(), new DefaultValidatorSelector())).ToList());
			ex.Message.ShouldEqual("Property name could not be automatically determined for expression x => x.CalculateSalary(). Please specify either a custom property name or a custom error message (with a call to WithName or WithMessage).");
		}

		[Test]
		public void Property_should_return_property_being_validated() {
			var property = typeof(Person).GetProperty("Surname");
			builder.Model.Member.ShouldEqual(property);
		}

		[Test]
		public void Property_should_return_null_when_it_is_not_a_property_being_validated() {
			builder = new RuleBuilder<Person, string>(x => "Foo");
			builder.Model.Member.ShouldBeNull();
		}

		[Test]
		public void Result_should_use_custom_property_name_when_no_property_name_can_be_determined() {
			var builder = new RuleBuilder<Person, int>(x => x.CalculateSalary());
			builder.GreaterThan(100).WithName("Foo");

			var results = builder.Single().Validate(new ValidationContext<Person>(new Person(), new PropertyChain(), new DefaultValidatorSelector()));
			results.Single().PropertyName.ShouldEqual("Foo");
		}

		class TestPropertyValidator : IPropertyValidator<Person, string> {
			public PropertyValidatorResult Validate(PropertyValidatorContext<Person, string> context) {
				return null;
			}
		}
	}
}