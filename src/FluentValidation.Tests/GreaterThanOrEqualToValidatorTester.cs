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
	using System.Globalization;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Threading;
	using Internal;
	using NUnit.Framework;
	using Validators;

	[TestFixture]
	public class GreaterThanOrEqualToValidatorTester {
		private TestValidator validator;
		private const int value = 1;

		[SetUp]
		public void Setup() {
			Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
			validator = new TestValidator(v => v.RuleFor(x => x.Id).GreaterThanOrEqualTo(value));
		}

		[Test]
		public void Should_fail_when_less_than_input() {
			var result = validator.Validate(new Person{Id=0});
			result.IsValid.ShouldBeFalse();
		}

		[Test]
		public void Should_succeed_when_greater_than_input() {
			var result = validator.Validate(new Person{Id=2});
			result.IsValid.ShouldBeTrue();
		}

		[Test]
		public void Should_succeed_when_equal_to_input() {
			var result = validator.Validate(new Person{Id=value});
			result.IsValid.ShouldBeTrue();
		}

		[Test]
		public void Should_set_default_error_when_validation_fails() {
			var result = validator.Validate(new Person{Id=0});
			result.Errors.Single().ErrorMessage.ShouldEqual("'Id' must be greater than or equal to '1'.");
		}

		[Test]
		public void Validates_with_property() {
			var validator = new TestValidator(v => v.RuleFor(x => x.Id).GreaterThanOrEqualTo(x => x.AnotherInt));
			var result = validator.Validate(new Person { Id = 0, AnotherInt = 1 });
			result.IsValid.ShouldBeFalse();
		}

		[Test]
		public void Comparison_type() {
			var propertyValidator = validator.CreateDescriptor()
				.GetValidatorsForMember("Id").Cast<GreaterThanOrEqualValidator>().Single();

			propertyValidator.Comparison.ShouldEqual(Comparison.GreaterThanOrEqual);
		}

		[Test]
		public void Validates_with_nullable_when_property_is_null() {
			validator = new TestValidator(v => v.RuleFor(x => x.NullableInt).GreaterThanOrEqualTo(5));
			var result = validator.Validate(new Person());
			result.IsValid.ShouldBeTrue();
		}

		[Test]
		public void Validates_with_nullable_when_property_not_null() {
			validator = new TestValidator(v => v.RuleFor(x => x.NullableInt).GreaterThanOrEqualTo(5));
			var result = validator.Validate(new Person { NullableInt = 1 });
			result.IsValid.ShouldBeFalse();
		}

    [Test]
    public void Validates_with_nullable_when_property_is_null_cross_property()
    {
        validator = new TestValidator(v => v.RuleFor(x => x.NullableInt).GreaterThanOrEqualTo(x => x.Id));
        var result = validator.Validate(new Person { Id = 5 });
        result.IsValid.ShouldBeTrue();
    }

    [Test]
    public void Validates_with_nullable_when_property_not_null_cross_property()
    {
        validator = new TestValidator(v => v.RuleFor(x => x.NullableInt).GreaterThanOrEqualTo(x=>x.Id));
        var result = validator.Validate(new Person { NullableInt = 1, Id = 5 });
        result.IsValid.ShouldBeFalse();
    }
	}
}