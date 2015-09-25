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
	using System.Globalization;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Threading;
	using Internal;
	using Xunit;
	using Validators;

	
	public class GreaterThanOrEqualToValidatorTester {
		private TestValidator validator;
		private const int value = 1;
        public GreaterThanOrEqualToValidatorTester() {
            CultureScope.SetDefaultCulture();
            validator = new TestValidator(v => v.RuleFor(x => x.Id).GreaterThanOrEqualTo(value));
		}

		[Fact]
		public void Should_fail_when_less_than_input() {
			var result = validator.Validate(new Person{Id=0});
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void Should_succeed_when_greater_than_input() {
			var result = validator.Validate(new Person{Id=2});
			result.IsValid.ShouldBeTrue();
		}

		[Fact]
		public void Should_succeed_when_equal_to_input() {
			var result = validator.Validate(new Person{Id=value});
			result.IsValid.ShouldBeTrue();
		}

		[Fact]
		public void Should_set_default_error_when_validation_fails() {
			var result = validator.Validate(new Person{Id=0});
			result.Errors.Single().ErrorMessage.ShouldEqual("'Id' must be greater than or equal to '1'.");
		}

		[Fact]
		public void Validates_with_property() {
			var validator = new TestValidator(v => v.RuleFor(x => x.Id).GreaterThanOrEqualTo(x => x.AnotherInt));
			var result = validator.Validate(new Person { Id = 0, AnotherInt = 1 });
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void Validates_with_nullable_property() {
			validator = new TestValidator(v => v.RuleFor(x => x.Id).GreaterThanOrEqualTo(x => x.NullableInt));

			var resultNull = validator.Validate(new Person { Id = 0, NullableInt = null });
			var resultLess = validator.Validate(new Person { Id = 0, NullableInt = -1 });
			var resultEqual = validator.Validate(new Person { Id = 0, NullableInt = 0 });
			var resultMore = validator.Validate(new Person { Id = 0, NullableInt = 1 });

			resultNull.IsValid.ShouldBeFalse();
			resultLess.IsValid.ShouldBeTrue();
			resultEqual.IsValid.ShouldBeTrue();
			resultMore.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void Validates_nullable_with_nullable_property() {
			validator = new TestValidator(v => v.RuleFor(x => x.NullableInt).GreaterThanOrEqualTo(x => x.OtherNullableInt));

			var resultNull = validator.Validate(new Person { NullableInt = 0, OtherNullableInt = null });
			var resultLess = validator.Validate(new Person { NullableInt = 0, OtherNullableInt = -1 });
			var resultEqual = validator.Validate(new Person { NullableInt = 0, OtherNullableInt = 0 });
			var resultMore = validator.Validate(new Person { NullableInt = 0, OtherNullableInt = 1 });

			resultNull.IsValid.ShouldBeFalse();
			resultLess.IsValid.ShouldBeTrue();
			resultEqual.IsValid.ShouldBeTrue();
			resultMore.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void Comparison_type() {
			var propertyValidator = validator.CreateDescriptor()
				.GetValidatorsForMember("Id").Cast<GreaterThanOrEqualValidator>().Single();

			propertyValidator.Comparison.ShouldEqual(Comparison.GreaterThanOrEqual);
		}

		[Fact]
		public void Validates_with_nullable_when_property_is_null() {
			validator = new TestValidator(v => v.RuleFor(x => x.NullableInt).GreaterThanOrEqualTo(5));
			var result = validator.Validate(new Person());
			result.IsValid.ShouldBeTrue();
		}

		[Fact]
		public void Validates_with_nullable_when_property_not_null() {
			validator = new TestValidator(v => v.RuleFor(x => x.NullableInt).GreaterThanOrEqualTo(5));
			var result = validator.Validate(new Person { NullableInt = 1 });
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void Should_localize_value() {
			using (new CultureScope("fr-fr")) {
				var orderValidator = new InlineValidator<Order>();
				orderValidator.RuleFor(x => x.Amount).GreaterThanOrEqualTo(1.2M).WithMessage("{ComparisonValue}");
				var result = orderValidator.Validate(new Order());
				var msg = result.Errors[0].ErrorMessage;
				msg.ShouldEqual("1,2");
			}
		}

    [Fact]
    public void Validates_with_nullable_when_property_is_null_cross_property()
    {
        validator = new TestValidator(v => v.RuleFor(x => x.NullableInt).GreaterThanOrEqualTo(x => x.Id));
        var result = validator.Validate(new Person { Id = 5 });
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Validates_with_nullable_when_property_not_null_cross_property()
    {
        validator = new TestValidator(v => v.RuleFor(x => x.NullableInt).GreaterThanOrEqualTo(x=>x.Id));
        var result = validator.Validate(new Person { NullableInt = 1, Id = 5 });
        result.IsValid.ShouldBeFalse();
    }
	}
}