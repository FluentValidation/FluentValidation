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
		FluentValidationModelValidatorProvider provider;
		DefaultModelBinder binder;

		[SetUp]
		public void Setup() {
			provider = new FluentValidationModelValidatorProvider(new AttributedValidatorFactory());
			ModelValidatorProviders.Providers.Add(provider);
			DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;
			binder = new DefaultModelBinder();
		}

		[TearDown]
		public void Teardown() {
			//Cleanup
			ModelValidatorProviders.Providers.Remove(provider);
		}

		protected ModelMetadata CreateMetaData(Type type) {
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

		public class TestModelValidator3 : AbstractValidator<TestModel3> {
			public TestModelValidator3() {
				RuleFor(x => x.Id).NotNull().WithMessage("Validation failed");
			}
		}

		public class TestModelWithoutValidator {
			public int Id { get; set; }
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

		[Test]
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

			bindingContext.ModelState["Id"].Errors.Single().ErrorMessage.ShouldEqual("Validation failed");
		}

		[Test]
		public void Should_not_add_default_message_to_modelstate_when_there_is_no_required_validator_explicitly_specified() {
			var form = new FormCollection {
				{ "Id", "" }
			};

			var bindingContext = new ModelBindingContext {
				ModelName = "test",
				ModelMetadata = CreateMetaData(typeof(TestModelWithoutValidator)),
				ModelState = new ModelStateDictionary(),
				FallbackToEmptyPrefix = true,
				ValueProvider = form.ToValueProvider()
			};

			binder.BindModel(new ControllerContext(), bindingContext);

			//TODO: Localise test.
			bindingContext.ModelState["Id"].Errors.Single().ErrorMessage.ShouldEqual("'Id' must not be empty.");
		}


		[Test]
		public void WorksAlongsideDataAnnotationsProvider() {
			ModelValidatorProviders.Providers.Insert(0, new DataAnnotationsModelValidatorProvider());
			Should_not_add_default_message_to_modelstate();
		}

		[Test]
		public void Should_add_default_message_to_modelstate_when_both_fv_and_DataAnnotations_have_implicit_required_validation_disabled() {
			DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;
			FluentValidationModelValidatorProvider.AddImplicitRequiredValidator = false;

			var form = new FormCollection {
				{ "Id", "" }
			};

			var bindingContext = new ModelBindingContext {
				ModelName = "test",
				ModelMetadata = CreateMetaData(typeof(TestModelWithoutValidator)),
				ModelState = new ModelStateDictionary(),
				FallbackToEmptyPrefix = true,
				ValueProvider = form.ToValueProvider()
			};

			binder.BindModel(new ControllerContext(), bindingContext);

			bindingContext.ModelState["Id"].Errors.Single().ErrorMessage.ShouldEqual("A value is required.");


			FluentValidationModelValidatorProvider.AddImplicitRequiredValidator = true;
			DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = true;
		}
	}
}