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
	using System.Collections;
	using System.Globalization;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Threading;
	using Internal;
	using Xunit;
	using Validators;
	using System.Reflection;
	
	public class NotEqualValidatorTests {
		public  NotEqualValidatorTests() {
          CultureScope.SetDefaultCulture();
        }

		[Fact]
		public void When_the_objects_are_equal_then_the_validator_should_fail() {
			var validator = new TestValidator(v => v.RuleFor(x => x.Forename).NotEqual("Foo"));
			var result = validator.Validate(new Person { Forename = "Foo" });
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void When_the_objects_are_not_equal_then_the_validator_should_pass() {
			var validator = new TestValidator(v => v.RuleFor(x => x.Forename).NotEqual("Bar"));
			var result = validator.Validate(new Person { Forename = "Foo" });
			result.IsValid.ShouldBeTrue();
		}

		[Fact]
		public void When_the_validator_fails_the_error_message_should_be_set() {
			var validator = new TestValidator(v => v.RuleFor(x => x.Forename).NotEqual("Foo"));
			var result = validator.Validate(new Person { Forename = "Foo" });
			result.Errors.Single().ErrorMessage.ShouldEqual("'Forename' should not be equal to 'Foo'.");
		}

		[Fact]
		public void Validates_across_properties() {
			var validator = new TestValidator(
				v => v.RuleFor(x => x.Forename).NotEqual(x => x.Surname)
			);

			var result = validator.Validate(new Person { Surname = "foo", Forename = "foo" });
			result.IsValid.ShouldBeFalse();
		}


		[Fact]
		public void Should_store_property_to_compare() {
			var validator = new TestValidator(v => v.RuleFor(x => x.Forename).NotEqual(x => x.Surname));
			var propertyValidator = validator.CreateDescriptor()
				.GetValidatorsForMember("Forename")
				.OfType<NotEqualValidator>()
				.Single();

			propertyValidator.MemberToCompare.ShouldEqual(typeof(Person).GetProperty("Surname"));
		}

		[Fact]
		public void Should_store_comparison_type() {
			var validator = new TestValidator(v => v.RuleFor(x => x.Forename).NotEqual(x => x.Surname));
			var propertyValidator = validator.CreateDescriptor()
				.GetValidatorsForMember("Forename")
				.OfType<NotEqualValidator>()
				.Single();
			propertyValidator.Comparison.ShouldEqual(Comparison.NotEqual);
		}

		[Fact]
		public void Should_not_be_valid_for_case_insensitve_comparison() {
			var validator = new TestValidator(v => v.RuleFor(x => x.Forename).NotEqual("FOO", StringComparer.OrdinalIgnoreCase));
			var result = validator.Validate(new Person{Forename = "foo"});
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void Should_not_be_valid_for_case_insensitve_comparison_with_expression() {
			var validator = new TestValidator(v => v.RuleFor(x => x.Forename).NotEqual(x => x.Surname, StringComparer.OrdinalIgnoreCase));
			var result = validator.Validate(new Person { Forename = "foo", Surname = "FOO"});
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void Should_handle_custom_value_types_correctly() {
			var myType = new MyType();
			var myTypeValidator = new MyTypeValidator();

			var validationResult = myTypeValidator.Validate(myType);
			validationResult.IsValid.ShouldEqual(false);
		}

		public class MyType
		{
			public MyValueType Value { get; set; }
		}

		public class MyTypeValidator : AbstractValidator<MyType>
		{
			public MyTypeValidator()
			{
				RuleFor(myType => myType.Value).NotEqual(MyValueType.None);
			}
		}

		public struct MyValueType
		{
			public static readonly MyValueType None = default(MyValueType);

			public MyValueType(int value)
			{
				_value = value;
			}

			public int Value
			{
				get { return _value ?? -1; }
			}

			private readonly int? _value;

			public override int GetHashCode() {
				return _value == null ? 0 : _value.Value.GetHashCode();	
			}

			public override string ToString() {
				return _value == null ? null : _value.Value.ToString();
			}

			public override bool Equals(object obj)
			{
				if (obj == null || obj.GetType() != typeof(MyValueType))
					return false;

				var otherValueType = (MyValueType)obj;
				return Equals(otherValueType);
			}

			public bool Equals(MyValueType other) {
				return _value == other._value;
			}

			public static bool operator ==(MyValueType first, MyValueType second) {
				return first.Equals(second);
			}

			public static bool operator !=(MyValueType first, MyValueType second) {
				return !(first == second);	
			}
		}
	}
}