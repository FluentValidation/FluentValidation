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
		IModelBinder binder;
		FluentValidationModelValidatorProvider provider;

		[SetUp]
		public void Setup() {
			provider = new FluentValidationModelValidatorProvider(new AttributedValidatorFactory());
			binder = new DefaultModelBinder();

			ModelValidatorProviders.Providers.Add(provider);
		}

		[TearDown]
		public void Teardown() {
			//Cleanup
			ModelValidatorProviders.Providers.Remove(provider);
		}

		[Test]
		public void When_a_validation_error_occurs_the_error_should_be_added_to_modelstate() {
			var form = new FormCollection {
                { "test.Name", null }
			};
			var bindingContext = new ModelBindingContext {
				ModelName = "test",
				ModelMetadata = CreateMetaData(typeof(TestModel)),
				ModelState = new ModelStateDictionary(),
				FallbackToEmptyPrefix = true,
				ValueProvider = form.ToValueProvider()
			};

			binder.BindModel(new ControllerContext(), bindingContext);

			bindingContext.ModelState["test.Name"].Errors.Single().ErrorMessage.ShouldEqual("Validation Failed");
		}

		[Test]
		public void When_a_validation_error_occurs_the_error_should_be_added_to_Modelstate_without_prefix() {
			var form = new FormCollection {
				{ "Name", null }
			};

			var bindingContext = new ModelBindingContext {
				ModelName = "foo",
				ModelMetadata = CreateMetaData(typeof(TestModel)),
				ModelState = new ModelStateDictionary(),
				FallbackToEmptyPrefix = true,
				ValueProvider = form.ToValueProvider()
			};

			binder.BindModel(new ControllerContext(), bindingContext);
			bindingContext.ModelState["Name"].Errors.Count().ShouldEqual(1);
		}

		[Test]
		public void Should_not_fail_when_no_validator_can_be_found() {
			var bindingContext = new ModelBindingContext {
				ModelName = "test",
				ModelMetadata = CreateMetaData(typeof(TestModel2)),

				ModelState = new ModelStateDictionary(),
				FallbackToEmptyPrefix = true,
				ValueProvider = new FormCollection().ToValueProvider()
			};

			binder.BindModel(new ControllerContext(), bindingContext).ShouldNotBeNull();
		}

		[Test, Ignore("This test will always fail as the DefaultModelBinder does not allow for the default 'required' validation to be disabled. One day I'm hoping this will be changed...")]
		public void Should_not_add_default_message_to_modelstate() {
			var form = new FormCollection {
				{ "Id", "" }
			};

			var bindingContext = new ModelBindingContext {
				ModelName = "test",
				ModelMetadata = CreateMetaData(typeof(TestModel3)),
				ModelState = new ModelStateDictionary(),
				FallbackToEmptyPrefix = true,
				ValueProvider = form.ToValueProvider()
			};

			binder.BindModel(new ControllerContext(), bindingContext);

			bindingContext.ModelState["Id"].Errors[0].ErrorMessage.ShouldEqual("Validation failed");
		}

		private ModelMetadata CreateMetaData(Type type) {
			return new ModelMetadata(new EmptyModelMetadataProvider(), null, null, type, null);
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

		[Validator(typeof(TestModelValidator3))]
		public class TestModel3 {
			public int Id { get; set; }
		}

		public class TestModelValidator3:AbstractValidator<TestModel3> {
			public TestModelValidator3() {
				RuleFor(x => x.Id).NotEmpty().WithMessage("Validation failed");
			}
		}
	}
}