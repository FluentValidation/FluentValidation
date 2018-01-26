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

namespace FluentValidation.Tests.WebApi {
	using System.Globalization;
	using System.Linq;
	using Xunit;
    using Results;
    using System.Web.Http.ModelBinding;
	using System.Web.Http.ValueProviders;
	using FluentValidation.WebApi;

    public class ValidationResultExtensionTests {
		private ValidationResult result;

		public ValidationResultExtensionTests() {
			result = new ValidationResult(new[] {
				new ValidationFailure("foo", "A foo error occurred", "x"),
				new ValidationFailure("bar", "A bar error occurred", "y"),
			});
		}

		[Fact]
		public void Should_persist_to_modelstate() {
			var modelstate = new ModelStateDictionary();
			result.AddToModelState(modelstate, null);

			modelstate.IsValid.ShouldBeFalse();
			modelstate["foo"].Errors[0].ErrorMessage.ShouldEqual("A foo error occurred");
			modelstate["bar"].Errors[0].ErrorMessage.ShouldEqual("A bar error occurred");

			modelstate["foo"].Value.AttemptedValue.ShouldEqual("x");
			modelstate["bar"].Value.AttemptedValue.ShouldEqual("y");
		}

		[Fact]
		public void Should_persist_modelstate_with_empty_prefix() {
			var modelstate = new ModelStateDictionary();
			result.AddToModelState(modelstate, "");
			modelstate["foo"].Errors[0].ErrorMessage.ShouldEqual("A foo error occurred");
		}

		[Fact]
		public void Should_persist_to_modelstate_with_prefix() {
			var modelstate = new ModelStateDictionary();
			result.AddToModelState(modelstate, "baz");

			modelstate.IsValid.ShouldBeFalse();
			modelstate["baz.foo"].Errors[0].ErrorMessage.ShouldEqual("A foo error occurred");
			modelstate["baz.bar"].Errors[0].ErrorMessage.ShouldEqual("A bar error occurred");
		}

		[Fact]
		public void Should_do_nothing_if_result_is_valid() {
			var modelState = new ModelStateDictionary();
			new ValidationResult().AddToModelState(modelState, null);
			modelState.IsValid.ShouldBeTrue();
		}

	    [Fact]
	    public void Does_not_overwrite_existing_values()
	    {
		    var modelstate = new ModelStateDictionary();
		    modelstate.AddModelError("model.Foo", "Foo");

		    result.AddToModelState(modelstate, "model");

		    modelstate["model.Foo"].Errors.Count.ShouldEqual(2);
	    }

	    [Fact]
	    public void Does_not_overwrite_existing_values_with_valid_value()
	    {
		    var modelstate = new ModelStateDictionary();
		    var state = new ModelState();

		    state.Value = new ValueProviderResult(
			    "value",
			    "value",
			    CultureInfo.CurrentCulture);

		    modelstate.Add("model.Foo", state);

		    result.AddToModelState(modelstate, "model");

		    modelstate["model.Foo"].Value.AttemptedValue.ShouldEqual("value");
		    modelstate["model.Foo"].Errors.Single().ErrorMessage.ShouldEqual("A foo error occurred");

	    }

	}
}