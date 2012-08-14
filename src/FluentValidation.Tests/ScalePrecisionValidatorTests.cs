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
	using System.Globalization;
	using System.Linq;
	using System.Threading;
	using NUnit.Framework;
	using Validators;

	[TestFixture]
    public class ScalePrecisionValidatorTests
    {
		[SetUp]
		public void Setup() {
			Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
		}

		[Test]
		public void Scale_and_precision_should_work() {
		    var validator = new TestValidator(v => v.RuleFor(x => x.Discount).SetValidator(new ScalePrecisionValidator(2, 4)));

            var result = validator.Validate(new Person { Discount = 123.456778m });
            Assert.IsFalse(result.IsValid);

            result = validator.Validate(new Person { Discount = 12.34M });
		    Assert.IsTrue(result.IsValid);
		}
	}
}