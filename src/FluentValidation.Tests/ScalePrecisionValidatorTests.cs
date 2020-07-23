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
	using System.Globalization;
	using System.Linq;
	using System.Threading;
	using Xunit;
	using Validators;

	public class ScalePrecisionValidatorTests {
		public ScalePrecisionValidatorTests() {
			CultureScope.SetDefaultCulture();
		}

		[Fact]
		public void Scale_precision_should_be_valid() {
			var validator = new TestValidator(v => v.RuleFor(x => x.Discount).ScalePrecision(2, 4));

			var result = validator.Validate(new Person { Discount = 12.34M });
			result.IsValid.ShouldBeTrue();

			result = validator.Validate(new Person { Discount = 2.34M });
			result.IsValid.ShouldBeTrue();

			result = validator.Validate(new Person { Discount = -2.34M });
			result.IsValid.ShouldBeTrue();

			result = validator.Validate(new Person { Discount = 0.34M });
			result.IsValid.ShouldBeTrue();
		}

		[Fact]
		public void Scale_precision_should_not_be_valid() {
			var validator = new TestValidator(v => v.RuleFor(x => x.Discount).ScalePrecision(2, 4));

			var result = validator.Validate(new Person { Discount = 123.456778m });
			result.IsValid.ShouldBeFalse();
			result.Errors[0].ErrorMessage.ShouldEqual("'Discount' must not be more than 4 digits in total, with allowance for 2 decimals. 3 digits and 6 decimals were found.");

			result = validator.Validate(new Person { Discount = 12.3414M });
			result.IsValid.ShouldBeFalse();
			result.Errors[0].ErrorMessage.ShouldEqual("'Discount' must not be more than 4 digits in total, with allowance for 2 decimals. 2 digits and 4 decimals were found.");

			result = validator.Validate(new Person { Discount = 1.344M });
			result.IsValid.ShouldBeFalse();
			result.Errors[0].ErrorMessage.ShouldEqual("'Discount' must not be more than 4 digits in total, with allowance for 2 decimals. 1 digits and 3 decimals were found.");

			result = validator.Validate(new Person { Discount = 156.3M });
			result.IsValid.ShouldBeFalse();
			result.Errors[0].ErrorMessage.ShouldEqual("'Discount' must not be more than 4 digits in total, with allowance for 2 decimals. 3 digits and 1 decimals were found.");

			result = validator.Validate(new Person { Discount = 65.430M });
			result.IsValid.ShouldBeFalse();
			result.Errors[0].ErrorMessage.ShouldEqual("'Discount' must not be more than 4 digits in total, with allowance for 2 decimals. 2 digits and 3 decimals were found.");
		}

		[Fact]
		public void Scale_precision_should_be_valid_when_they_are_equal() {
			var validator = new TestValidator(v => v.RuleFor(x => x.Discount).ScalePrecision(2, 2));

			var result = validator.Validate(new Person { Discount = 0.34M });
			result.IsValid.ShouldBeTrue();

			result = validator.Validate(new Person { Discount = 0.3M });
			result.IsValid.ShouldBeTrue();

			result = validator.Validate(new Person { Discount = 0M });
			result.IsValid.ShouldBeTrue();

			result = validator.Validate(new Person { Discount = -0.34M });
			result.IsValid.ShouldBeTrue();
		}

		[Fact]
		public void Scale_precision_should_not_be_valid_when_they_are_equal() {
			var validator = new TestValidator(v => v.RuleFor(x => x.Discount).ScalePrecision(2, 2));

			var result = validator.Validate(new Person { Discount = 123.456778m });
			result.IsValid.ShouldBeFalse();
			result.Errors[0].ErrorMessage.ShouldEqual("'Discount' must not be more than 2 digits in total, with allowance for 2 decimals. 3 digits and 6 decimals were found.");

			result = validator.Validate(new Person { Discount = 0.341M });
			result.IsValid.ShouldBeFalse();
			result.Errors[0].ErrorMessage.ShouldEqual("'Discount' must not be more than 2 digits in total, with allowance for 2 decimals. 0 digits and 3 decimals were found.");

			result = validator.Validate(new Person { Discount = 1.34M });
			result.IsValid.ShouldBeFalse();
			result.Errors[0].ErrorMessage.ShouldEqual("'Discount' must not be more than 2 digits in total, with allowance for 2 decimals. 1 digits and 2 decimals were found.");

			result = validator.Validate(new Person { Discount = 1M });
			result.IsValid.ShouldBeFalse();
			result.Errors[0].ErrorMessage.ShouldEqual("'Discount' must not be more than 2 digits in total, with allowance for 2 decimals. 1 digits and 0 decimals were found.");
		}

		[Fact]
		public void Scale_precision_should_be_valid_when_ignoring_trailing_zeroes() {
			var validator = new TestValidator(v => v.RuleFor(x => x.Discount).ScalePrecision(2, 4, true));

			var result = validator.Validate(new Person { Discount = 15.0000000000000000000000000M });
			result.IsValid.ShouldBeTrue();

			result = validator.Validate(new Person { Discount = 0000000000000000000015.0000000000000000000000000M });
			result.IsValid.ShouldBeTrue();

			result = validator.Validate(new Person { Discount = 65.430M });
			result.IsValid.ShouldBeTrue();
		}

    [Fact]
    public void Scale_precision_should_not_be_valid_when_ignoring_trailing_zeroes() {
      var validator = new TestValidator(v => v.RuleFor(x => x.Discount).ScalePrecision(2, 4, true));

      var result = validator.Validate(new Person { Discount = 1565.0M });
      result.IsValid.ShouldBeFalse();
      result.Errors[0].ErrorMessage.ShouldEqual("'Discount' must not be more than 4 digits in total, with allowance for 2 decimals. 4 digits and 0 decimals were found.");

      result = validator.Validate(new Person { Discount = 15.0000000000000000000000001M });
      result.IsValid.ShouldBeFalse();
      result.Errors[0].ErrorMessage.ShouldEqual("'Discount' must not be more than 4 digits in total, with allowance for 2 decimals. 2 digits and 25 decimals were found.");
    }
  }
}
