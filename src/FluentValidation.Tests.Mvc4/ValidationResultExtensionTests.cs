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
	using System.Web.Mvc;
	using Mvc;
	using NUnit.Framework;
	using Results;

	[TestFixture]
	public class ValidationResultExtensionTests {
		private ValidationResult result;

		[SetUp]
		public void Setup() {
			result = new ValidationResult(new[] {
				new ValidationFailure("foo", "A foo error occurred", "x"),
				new ValidationFailure("bar", "A bar error occurred", "y"),
			});
		}

		[Test]
		public void Should_persist_to_modelstate() {
			var modelstate = new ModelStateDictionary();
			result.AddToModelState(modelstate, null);

			modelstate.IsValid.ShouldBeFalse();
			modelstate["foo"].Errors[0].ErrorMessage.ShouldEqual("A foo error occurred");
			modelstate["bar"].Errors[0].ErrorMessage.ShouldEqual("A bar error occurred");

			modelstate["foo"].Value.AttemptedValue.ShouldEqual("x");
			modelstate["bar"].Value.AttemptedValue.ShouldEqual("y");
		}

		[Test]
		public void Should_persist_modelstate_with_empty_prefix() {
			var modelstate = new ModelStateDictionary();
			result.AddToModelState(modelstate, "");
			modelstate["foo"].Errors[0].ErrorMessage.ShouldEqual("A foo error occurred");
		}

		[Test]
		public void Should_persist_to_modelstate_with_prefix() {
			var modelstate = new ModelStateDictionary();
			result.AddToModelState(modelstate, "baz");

			modelstate.IsValid.ShouldBeFalse();
			modelstate["baz.foo"].Errors[0].ErrorMessage.ShouldEqual("A foo error occurred");
			modelstate["baz.bar"].Errors[0].ErrorMessage.ShouldEqual("A bar error occurred");
		}

		[Test]
		public void Should_do_nothing_if_result_is_valid() {
			var modelState = new ModelStateDictionary();
			new ValidationResult().AddToModelState(modelState, null);
			modelState.IsValid.ShouldBeTrue();
		}
	}
}