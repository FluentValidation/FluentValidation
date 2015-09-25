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

namespace FluentValidation.Tests
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using Xunit;
    using Validators;

    
    public class ScalePrecisionValidatorTests
    {
        public ScalePrecisionValidatorTests()
        {
           CultureScope.SetDefaultCulture();
        }

        [Fact]
        public void Scale_and_precision_should_work()
        {
            var validator = new TestValidator(v => v.RuleFor(x => x.Discount).SetValidator(new ScalePrecisionValidator(2, 4)));

            var result = validator.Validate(new Person {Discount = 123.456778m});
            Assert.False(result.IsValid);

            result = validator.Validate(new Person {Discount = 12.34M});
            Assert.True(result.IsValid);

            result = validator.Validate(new Person {Discount = 12.3414M});
            result.IsValid.ShouldBeFalse();

            result = validator.Validate(new Person {Discount = 1.344M});
            result.IsValid.ShouldBeFalse();

            result = validator.Validate(new Person {Discount = 156.3M});
            result.IsValid.ShouldBeTrue();

            result = validator.Validate(new Person {Discount = 1565.0M}); // fail as it counts zeros
            result.IsValid.ShouldBeFalse();

            validator = new TestValidator(v => 
                v.RuleFor(x => x.Discount)
                    .SetValidator(new ScalePrecisionValidator(2, 4) {IgnoreTrailingZeros = true}));

            result = validator.Validate(new Person {Discount = 1565.0M}); // ignores zeros now
            result.IsValid.ShouldBeTrue();

            result = validator.Validate(new Person {Discount = 15655.0M});
            result.IsValid.ShouldBeFalse();

            result = validator.Validate(new Person {Discount = 155.0000000000000000000000000M});
            result.IsValid.ShouldBeTrue();

            result = validator.Validate(new Person {Discount = 155.0000000000000000000000001M});
            result.IsValid.ShouldBeFalse();

            result = validator.Validate(new Person {Discount = 00000000000000000000155.0000000000000000000000000M});
            result.IsValid.ShouldBeTrue();
        }
    }
}