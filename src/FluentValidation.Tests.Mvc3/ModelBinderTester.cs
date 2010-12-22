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
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
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
			var meta = new DataAnnotationsModelMetadataProvider();
			return meta.GetMetadataForType(null, type);
			//return new ModelMetadata(new EmptyModelMetadataProvider(), null, null, type, null);
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

		[Validator(typeof(TestModel4Validator))]
		public class TestModel4 {
			public string Surname { get; set; }
			public string Forename { get; set; }
			public string Email { get; set; }
			public DateTime DateOfBirth { get; set; }
			public string Address1 { get; set; }
		}

		

		public class TestModel4Validator : AbstractValidator<TestModel4> {
			public TestModel4Validator() {
				RuleFor(x => x.Surname).NotEqual(x => x.Forename);

				RuleFor(x => x.Email)
					.EmailAddress();

				RuleFor(x => x.Address1).NotEmpty();
			}
		}

		[Test]
		public void Should_add_all_errors_in_one_go() {
			var form = new FormCollection {
				{ "Email", "foo" },
				{ "Surname", "foo" },
				{ "Forename", "foo" },
				{ "DateOfBirth", null },
				{ "Address1", null }
			};

			var context = new ModelBindingContext {
				ModelName = "test",
				ModelMetadata =  CreateMetaData(typeof(TestModel4)),
				ModelState = new ModelStateDictionary(),
				FallbackToEmptyPrefix = true,
				ValueProvider = form.ToValueProvider(),
			};

			binder.BindModel(new ControllerContext(), context);

			context.ModelState.IsValidField("Email").ShouldBeFalse(); //Email validation failed
			context.ModelState.IsValidField("DateOfBirth").ShouldBeFalse(); //Date of Birth not specified (implicit required error)
			context.ModelState.IsValidField("Surname").ShouldBeFalse(); //cross-property
		}

		[Validator(typeof(TestModel5Validator))]
		public class TestModel5 {
			public int Id { get; set; }
			public bool SomeBool { get; set; }
		}

		public class TestModel5Validator : AbstractValidator<TestModel5> {
			public TestModel5Validator() {
				//force a complex rule
				RuleFor(x => x.SomeBool).Must(x => x == true);
				RuleFor(x => x.Id).NotEmpty();
			}
		}

		[Test]
		public void Should_add_all_erorrs_in_one_go_when_NotEmpty_rule_specified_for_non_nullable_value_type() {
			var form = new FormCollection {
				{ "SomeBool", "False" },
				{ "Id", "0" }
			};

			var context = new ModelBindingContext {
				ModelName = "test",
				ModelMetadata = CreateMetaData(typeof(TestModel5)),
				ModelState = new ModelStateDictionary(),
				FallbackToEmptyPrefix = true,
				ValueProvider = form.ToValueProvider(),
			};

			binder.BindModel(new ControllerContext(), context);

			context.ModelState.IsValidField("SomeBool").ShouldBeFalse(); //Complex rule
			context.ModelState.IsValidField("Id").ShouldBeFalse(); //NotEmpty for non-nullable value type
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
		public void Should_add_default_message_to_modelstate_when_both_fv_and_DataAnnotations_have_implicit_required_validation_disabled() {
			DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;
			provider.AddImplicitRequiredValidator = false;

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


			provider.AddImplicitRequiredValidator = true;
			DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = true;
		}
	}
}