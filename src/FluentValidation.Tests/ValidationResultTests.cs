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
	using Newtonsoft.Json;
	using Xunit;
	using Results;


	public class ValidationResultTests {

		[Fact]
		public void Should_be_valid_when_there_are_no_errors() {
			var result = new ValidationResult();
			result.IsValid.ShouldBeTrue();
		}

		[Fact]
		public void Should_not_be_valid_when_there_are_errors() {
			var result = new ValidationResult(new[] {new ValidationFailure(null, null), new ValidationFailure(null, null)});
			result.IsValid.ShouldBeFalse();
		}

		[Fact]
		public void Should_add_errors() {
			var result = new ValidationResult(new[] {new ValidationFailure(null, null), new ValidationFailure(null, null)});
			result.Errors.Count.ShouldEqual(2);
		}

		[Fact]
		public void Can_serialize_result() {
			var result = new ValidationResult(new[] { new ValidationFailure("Property", "Error"),  });
			var serialized = JsonConvert.SerializeObject(result);
			var deserialized = JsonConvert.DeserializeObject<ValidationResult>(serialized);
			deserialized.Errors.Count.ShouldEqual(1);
			deserialized.Errors[0].ErrorMessage.ShouldEqual("Error");
			deserialized.Errors[0].PropertyName.ShouldEqual("Property");
		}

		[Fact]
		public void Can_serialize_failure() {
			var failure = new ValidationFailure("Property", "Error");
			var serialized = JsonConvert.SerializeObject(failure);
			var deserialized = JsonConvert.DeserializeObject<ValidationFailure>(serialized);
			deserialized.PropertyName.ShouldEqual("Property");
			deserialized.ErrorMessage.ShouldEqual("Error");
		}

		[Fact]
		public void ToString_return_empty_string_when_there_is_no_error() {
			ValidationResult result = new ValidationResult();
			string actualResult = result.ToString();

			Assert.Empty(actualResult);
		}

		[Fact]
		public void ToString_return_error_messages_with_newline_as_separator() {
			const string errorMessage1 = "expected error message 1";
			const string errorMessage2 = "expected error message 2";

			string expectedResult = errorMessage1 + Environment.NewLine + errorMessage2;

			ValidationResult result = new ValidationResult(new[] {
				new ValidationFailure("property1", errorMessage1),
				new ValidationFailure("property2", errorMessage2)
			});

			string actualResult = result.ToString();

			Assert.Equal(expectedResult, actualResult);
		}

		[Fact]
		public void ToString_return_error_messages_with_given_separator()
		{
			const string errorMessage1 = "expected error message 1";
			const string errorMessage2 = "expected error message 2";
			const string separator = "~";
			const string expectedResult = errorMessage1 + separator + errorMessage2;

			ValidationResult result = new ValidationResult(new[] {
				new ValidationFailure("property1", errorMessage1),
				new ValidationFailure("property2", errorMessage2)
			});

			string actualResult = result.ToString(separator);

			Assert.Equal(expectedResult, actualResult);
		}
	}
}
