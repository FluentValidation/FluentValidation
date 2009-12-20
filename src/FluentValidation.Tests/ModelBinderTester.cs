#region License
// Copyright 2008-2009 Jeremy Skinner (http://www.jeremyskinner.co.uk)
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
	using System.Linq;
	using System.Web.Mvc;
	using Attributes;
	using Mvc;
	using NUnit.Framework;

	[TestFixture]
	public class ModelBinderTester {
		FluentValidationModelBinder binder;

		[SetUp]
		public void Setup() {
			binder = new FluentValidationModelBinder(new AttributedValidatorFactory());
		}

		[Test]
		public void When_a_validation_error_occurs_the_error_should_be_added_to_modelstate() {
			var form = new FormCollection {
                { "test.Name", null }
			};
			var bindingContext = new ModelBindingContext {
				ModelName = "test",
				ModelType = typeof(TestModel),
				ModelState = new ModelStateDictionary(),
				FallbackToEmptyPrefix = true,
				ValueProvider = form.ToValueProvider()
			};

			binder.BindModel(null, bindingContext);

			bindingContext.ModelState["test.Name"].Errors.Single().ErrorMessage.ShouldEqual("Validation Failed");
		}

		[Test]
		public void When_a_validation_error_occurs_the_error_should_be_added_to_Modelstate_without_prefix() {
			var form = new FormCollection {
				{ "Name", null }
			};

			var bindingContext = new ModelBindingContext {
				ModelName = "foo",
				ModelType = typeof(TestModel),
				ModelState = new ModelStateDictionary(),
				FallbackToEmptyPrefix = true,
				ValueProvider = form.ToValueProvider()
			};

			binder.BindModel(null, bindingContext);
			bindingContext.ModelState["Name"].Errors.Count().ShouldEqual(1);
		}

		[Test]
		public void Should_not_fail_when_no_validator_can_be_found() {
			var bindingContext = new ModelBindingContext {
				ModelName = "test",
				ModelType = typeof(TestModel2),
				ModelState = new ModelStateDictionary(),
				FallbackToEmptyPrefix = true,
				ValueProvider = new FormCollection().ToValueProvider()
			};

			binder.BindModel(null, bindingContext).ShouldNotBeNull();
		}

		public class TestModel2 {
		}

		[Validator(typeof(TestModelValidator))]
		public class TestModel {
			public string Name { get; set; }
		}

		public class TestModelValidator : AbstractValidator<TestModel> {
			public TestModelValidator() {
				RuleFor(x => x.Name).NotNull().WithMessage("Validation Failed");
			}
		}
	}
}