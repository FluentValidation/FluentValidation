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
	}
}