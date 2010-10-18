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
	using System.Threading;
	using NUnit.Framework;
	using Validators;

	[TestFixture]
	public class RegularExpressionValidatorTests {
		[SetUp]
		public void Setup() {
			Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
		}

		[Test]
		public void When_the_text_matches_the_regular_expression_then_the_validator_should_pass() {
			string input = "S3";
			var validator = new RegularExpressionValidator(@"^\w\d$");
			var result = validator.Validate(new PropertyValidatorContext(null, new object(), x => input));
			result.IsValid().ShouldBeTrue();
		}

		[Test]
		public void When_the_text_does_not_match_the_regular_expression_then_the_validator_should_fail() {
			string input = "S33";
			var validator = new RegularExpressionValidator(@"^\w\d$");
			var result = validator.Validate(new PropertyValidatorContext(null, new object(), x => input));
			result.IsValid().ShouldBeFalse();

			input = " 5";
			validator = new RegularExpressionValidator(@"^\w\d$");
			result = validator.Validate(new PropertyValidatorContext(null, new object(), x => input));
			result.IsValid().ShouldBeFalse();
		}

		[Test]
		public void When_the_text_is_empty_then_the_validator_should_fail() {
			string input = "";
			var validator = new RegularExpressionValidator(@"^\w\d$");
			var result = validator.Validate(new PropertyValidatorContext(null, new object(), x => input));
			result.IsValid().ShouldBeFalse();
		}

		[Test]
		public void When_the_text_is_null_then_the_validator_should_pass() {
			string input = null;
			var validator = new RegularExpressionValidator(@"^\w\d$");
			var result = validator.Validate(new PropertyValidatorContext(null, new object(), x => input));
			result.IsValid().ShouldBeTrue();
		}

		[Test]
		public void When_validation_fails_the_default_error_should_be_set() {
			string input = "S33";
			var validator = new RegularExpressionValidator(@"^\w\d$");
			var result = validator.Validate(new PropertyValidatorContext("Name", new object(), x => input));
			result.Single().ErrorMessage.ShouldEqual("'Name' is not in the correct format.");
		}
	}
}