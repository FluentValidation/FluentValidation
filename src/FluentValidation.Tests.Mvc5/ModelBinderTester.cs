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

namespace FluentValidation.Tests.Mvc5 {
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.ComponentModel.DataAnnotations;
	using System.Globalization;
	using System.Linq;
	using System.Threading;
	using System.Web;
	using System.Web.Mvc;
	using Attributes;
	using Internal;
	using Moq;
	using Mvc;
	using Xunit;
	using ValidationContext = FluentValidation.ValidationContext;
	using ValidationResult = Results.ValidationResult;

	public class ModelBinderTester : IDisposable {
		FluentValidationModelValidatorProvider provider;
		DefaultModelBinder binder;
		ControllerContext controllerContext;

		public ModelBinderTester() {
			Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            provider = new FluentValidationModelValidatorProvider(new AttributedValidatorFactory());
			ModelValidatorProviders.Providers.Add(provider);
			DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;
			binder = new DefaultModelBinder();
			controllerContext = new ControllerContext { HttpContext = MockHttpContext.Create() };
		}

		public void Dispose() {
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

		[Validator(typeof(TestModel6Validator))]
		public class TestModel6 {
			public int Id { get; set; }
		}

		public class TestModel6Validator : AbstractValidator<TestModel6> {
			public TestModel6Validator() {
				//This ctor is intentionally blank.
			}
		}


		[Validator(typeof(TestModel7Validator))]
		public class TestModel7 {
			[Display(ResourceType = typeof(TestMessages), Name="PropertyName")]
			public int Id { get; set; }
		}

		public class TestModel7Validator : AbstractValidator<TestModel7> {
			public TestModel7Validator() {
				//This ctor is intentionally blank.
			}
		}

		/*[Fact, Ignore("MVC5 changed validation behaviour sutbley causing this to fail. Investigate for a future release. ")]
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
				ModelMetadata = CreateMetaData(typeof(TestModel4)),
				ModelState = new ModelStateDictionary(),
				FallbackToEmptyPrefix = true,
				ValueProvider = form.ToValueProvider(),
			};

			binder.BindModel(controllerContext, context);

			context.ModelState.IsValidField("Email").ShouldBeFalse(); //Email validation failed
			context.ModelState.IsValidField("DateOfBirth").ShouldBeFalse(); //Date of Birth not specified (implicit required error)
			context.ModelState.IsValidField("Surname").ShouldBeFalse(); //cross-property
		}*/

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

		[Fact]
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

			binder.BindModel(controllerContext, context);

			context.ModelState.IsValidField("SomeBool").ShouldBeFalse(); //Complex rule
			context.ModelState.IsValidField("Id").ShouldBeFalse(); //NotEmpty for non-nullable value type
		}

		[Fact]
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

			binder.BindModel(controllerContext, bindingContext);

			bindingContext.ModelState["test.Name"].Errors.Single().ErrorMessage.ShouldEqual("Validation Failed");
		}

		[Fact]
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

			binder.BindModel(controllerContext, bindingContext);
			bindingContext.ModelState["Name"].Errors.Count().ShouldEqual(1);
		}

		[Fact]
		public void Should_not_fail_when_no_validator_can_be_found() {
			var bindingContext = new ModelBindingContext {
				ModelName = "test",
				ModelMetadata = CreateMetaData(typeof(TestModel2)),

				ModelState = new ModelStateDictionary(),
				FallbackToEmptyPrefix = true,
				ValueProvider = new FormCollection().ToValueProvider()
			};

			binder.BindModel(controllerContext, bindingContext).ShouldNotBeNull();
		}

		[Fact]
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

			binder.BindModel(controllerContext, bindingContext);

			bindingContext.ModelState["Id"].Errors.Single().ErrorMessage.ShouldEqual("Validation failed");
		}

		[Fact]
		public void Should_not_add_default_message_to_modelstate_when_there_is_no_required_validator_explicitly_specified() {
			var form = new FormCollection {
				{ "Id", "" }
			};

			var bindingContext = new ModelBindingContext {
				ModelName = "test",
				ModelMetadata = CreateMetaData(typeof(TestModel6)),
				ModelState = new ModelStateDictionary(),
				FallbackToEmptyPrefix = true,
				ValueProvider = form.ToValueProvider()
			};

			binder.BindModel(controllerContext, bindingContext);

			bindingContext.ModelState["Id"].Errors.Single().ErrorMessage.ShouldEqual("'Id' must not be empty.");
		}

		[Fact]
		public void Should_add_Default_message_to_modelstate_when_no_validator_specified() {
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

			binder.BindModel(controllerContext, bindingContext);

			bindingContext.ModelState["Id"].Errors.Single().ErrorMessage.ShouldEqual("A value is required.");
		}


		[Fact]
		public void Allows_override_of_required_message_for_non_nullable_value_types() {
			var form = new FormCollection {
				{ "Id", "" }
			};

			var bindingContext = new ModelBindingContext {
				ModelName = "test",
				ModelMetadata = CreateMetaData(typeof(TestModelWithOverridenMessageValueType)),
				ModelState = new ModelStateDictionary(),
				FallbackToEmptyPrefix = true,
				ValueProvider = form.ToValueProvider()
			};

			binder.BindModel(controllerContext, bindingContext);

			//TODO: Localise test.
			bindingContext.ModelState["Id"].Errors.Single().ErrorMessage.ShouldEqual("Foo");
		}

		[Fact]
		public void Allows_override_of_required_property_name_for_non_nullable_value_types() {
			var form = new FormCollection {
				{ "Id", "" }
			};

			var bindingContext = new ModelBindingContext {
				ModelName = "test",
				ModelMetadata = CreateMetaData(typeof(TestModelWithOverridenPropertyNameValueType)),
				ModelState = new ModelStateDictionary(),
				FallbackToEmptyPrefix = true,
				ValueProvider = form.ToValueProvider()
			};

			binder.BindModel(controllerContext, bindingContext);

			//TODO: Localise test.
			bindingContext.ModelState["Id"].Errors.Single().ErrorMessage.ShouldEqual("'Foo' must not be empty."
);
		}



		[Fact]
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

			binder.BindModel(controllerContext, bindingContext);

			bindingContext.ModelState["Id"].Errors.Single().ErrorMessage.ShouldEqual("A value is required.");


			provider.AddImplicitRequiredValidator = true;
			DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = true;
		}

		[Fact]
		public void Should_use_name_from_Dataannotations_if_no_rule_is_found_and_displayattribute_is_present() {
			DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;
			provider.AddImplicitRequiredValidator = true;

			var form = new FormCollection {
				{ "Id", "" }
			};

			var bindingContext = new ModelBindingContext {
				ModelName = "test",
				ModelMetadata = CreateMetaData(typeof(TestModel7)),
				ModelState = new ModelStateDictionary(),
				FallbackToEmptyPrefix = true,
				ValueProvider = form.ToValueProvider()
			};

			binder.BindModel(controllerContext, bindingContext);

			bindingContext.ModelState["Id"].Errors.Single().ErrorMessage.ShouldEqual("'foo' must not be empty.");


			DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = true;
		}


		[Fact]
		public void Should_only_validate_specified_ruleset() {
			var form = new FormCollection {
				{ "Email", "foo" },
				{ "Surname", "foo" },
				{ "Forename", "foo" },
			};

			var context = new ModelBindingContext {
				ModelName = "test",
				ModelMetadata = CreateMetaData(typeof(RulesetTestModel)),
				ModelState = new ModelStateDictionary(),
				FallbackToEmptyPrefix = true,
				ValueProvider = form.ToValueProvider(),
			};

			var binder = new CustomizeValidatorAttribute { RuleSet = "Names" };
			binder.BindModel(controllerContext, context);

			context.ModelState.IsValidField("Forename").ShouldBeFalse();
			context.ModelState.IsValidField("Surname").ShouldBeFalse();
			context.ModelState.IsValidField("Email").ShouldBeTrue();
		}

		[Fact]
		public void Should_only_validate_specified_properties() {
			var form = new FormCollection {
				{ "Email", "foo" },
				{ "Surname", "foo" },
				{ "Forename", "foo" },
			};

			var context = new ModelBindingContext {
				ModelName = "test",
				ModelMetadata = CreateMetaData(typeof(PropertiesTestModel)),
				ModelState = new ModelStateDictionary(),
				FallbackToEmptyPrefix = true,
				ValueProvider = form.ToValueProvider(),
			};

			var binder = new CustomizeValidatorAttribute { Properties = "Surname,Forename" };
			binder.BindModel(controllerContext, context);

			context.ModelState.IsValidField("Forename").ShouldBeFalse();
			context.ModelState.IsValidField("Surname").ShouldBeFalse();
			context.ModelState.IsValidField("Email").ShouldBeTrue();

		}

		[Fact]
		public void When_interceptor_specified_Intercepts_validation() {
			var form = new FormCollection {
				{ "Email", "foo" },
				{ "Surname", "foo" },
				{ "Forename", "foo" },
			};

			var context = new ModelBindingContext {
				ModelName = "test",
				ModelMetadata = CreateMetaData(typeof(PropertiesTestModel)),
				ModelState = new ModelStateDictionary(),
				FallbackToEmptyPrefix = true,
				ValueProvider = form.ToValueProvider(),
			};

			var binder = new CustomizeValidatorAttribute { Interceptor = typeof(SimplePropertyInterceptor) };
			binder.BindModel(controllerContext, context);

			context.ModelState.IsValidField("Forename").ShouldBeFalse();
			context.ModelState.IsValidField("Surname").ShouldBeFalse();
			context.ModelState.IsValidField("Email").ShouldBeTrue();

		}

		[Fact]
		public void When_interceptor_specified_Intercepts_validation_provides_custom_errors() {
			var form = new FormCollection {
				{ "Email", "foo" },
				{ "Surname", "foo" },
				{ "Forename", "foo" },
			};

			var context = new ModelBindingContext {
				ModelName = "test",
				ModelMetadata = CreateMetaData(typeof(PropertiesTestModel)),
				ModelState = new ModelStateDictionary(),
				FallbackToEmptyPrefix = true,
				ValueProvider = form.ToValueProvider(),
			};
			var binder = new CustomizeValidatorAttribute { Interceptor = typeof(ClearErrorsInterceptor) };
			binder.BindModel(controllerContext, context);

			context.ModelState.IsValid.ShouldBeTrue();
		}

		[Fact]
		public void When_validator_implements_IValidatorInterceptor_directly_interceptor_invoked() {
			var form = new FormCollection {
				{ "Email", "foo" },
				{ "Surname", "foo" },
				{ "Forename", "foo" },
			};

			var context = new ModelBindingContext {
				ModelName = "test",
				ModelMetadata = CreateMetaData(typeof(PropertiesTestModel2)),
				ModelState = new ModelStateDictionary(),
				FallbackToEmptyPrefix = true,
				ValueProvider = form.ToValueProvider(),
			};

			binder.BindModel(controllerContext, context);

			context.ModelState.IsValid.ShouldBeTrue();
		}

		[Fact]
		public void Validator_customizations_should_only_apply_to_single_parameter() {
			var form = new FormCollection {
				{ "first.Email", "foo" },
				{ "first.Surname", "foo" },
				{ "first.Forename", "foo" },
				{ "second.Email", "foo" },
				{ "second.Surname", "foo" },
				{ "second.Forename", "foo" }
			};

			var modelstate = new ModelStateDictionary();

			var firstContext = new ModelBindingContext {
				ModelName = "first",
				ModelMetadata = CreateMetaData(typeof(RulesetTestModel)),
				ModelState = modelstate,
				FallbackToEmptyPrefix = true,
				ValueProvider = form.ToValueProvider(),
			};

			var secondContext = new ModelBindingContext {
				ModelName = "second",
				ModelMetadata =  CreateMetaData(typeof(RulesetTestModel)),
				ModelState = modelstate,
				FallbackToEmptyPrefix = true,
				ValueProvider = form.ToValueProvider()
			};

			// Use the customizations for the first 
			var binder = new CustomizeValidatorAttribute { RuleSet = "Names" };
			binder.BindModel(controllerContext, firstContext);
		
			// ...but not for the second.
			this.binder.BindModel(controllerContext, secondContext);

			//customizations should only apply to the first validator 
			modelstate.IsValidField("first.Forename").ShouldBeFalse();
			modelstate.IsValidField("first.Surname").ShouldBeFalse();
			modelstate.IsValidField("second.Forename").ShouldBeTrue();
			modelstate.IsValidField("second.Surname").ShouldBeTrue();
		}


		private class SimplePropertyInterceptor : IValidatorInterceptor {
			readonly string[] properties = new[] { "Surname", "Forename" };

			public ValidationContext BeforeMvcValidation(ControllerContext cc, ValidationContext context) {
				var newContext = context.Clone(selector: new MemberNameValidatorSelector(properties));
				return newContext;
			}

			public ValidationResult AfterMvcValidation(ControllerContext cc, ValidationContext context, ValidationResult result) {
				return result;
			}
		}

		private class ClearErrorsInterceptor : IValidatorInterceptor {
			public ValidationContext BeforeMvcValidation(ControllerContext cc, ValidationContext context) {
				return null;
			}

			public ValidationResult AfterMvcValidation(ControllerContext cc, ValidationContext context, ValidationResult result) {
				return new ValidationResult();
			}
		}

		[Validator(typeof(PropertiesValidator2))]
		private class PropertiesTestModel2 {
			public string Email { get; set; }
			public string Surname { get; set; }
			public string Forename { get; set; }
		}

		private class PropertiesValidator2 : AbstractValidator<PropertiesTestModel2>, IValidatorInterceptor {
			public PropertiesValidator2() {
				RuleFor(x => x.Email).NotEqual("foo");
				RuleFor(x => x.Surname).NotEqual("foo");
				RuleFor(x => x.Forename).NotEqual("foo");
			}

			public ValidationContext BeforeMvcValidation(ControllerContext controllerContext, ValidationContext validationContext) {
				return validationContext;
			}

			public ValidationResult AfterMvcValidation(ControllerContext controllerContext, ValidationContext validationContext, ValidationResult result) {
				return new ValidationResult(); //empty errors
			}
		}


		[Validator(typeof(PropertiesValidator))]
		private class PropertiesTestModel {
			public string Email { get; set; }
			public string Surname { get; set; }
			public string Forename { get; set; }
		}


		private class PropertiesValidator : AbstractValidator<PropertiesTestModel> {
			public PropertiesValidator() {
				RuleFor(x => x.Email).NotEqual("foo");
				RuleFor(x => x.Surname).NotEqual("foo");
				RuleFor(x => x.Forename).NotEqual("foo");
			}
		}

		[Validator(typeof(RulesetTestValidator))]
		private class RulesetTestModel {
			public string Email { get; set; }
			public string Surname { get; set; }
			public string Forename { get; set; }
		}

		private class RulesetTestValidator : AbstractValidator<RulesetTestModel> {
			public RulesetTestValidator() {
				RuleFor(x => x.Email).NotEqual("foo");

				RuleSet("Names", () => {
					RuleFor(x => x.Surname).NotEqual("foo");
					RuleFor(x => x.Forename).NotEqual("foo");
				});
			}
		}

		[Validator(typeof(TestModelWithOverridenMessageValueTypeValidator))]
		private class TestModelWithOverridenMessageValueType {
			public int Id { get; set; }
		}

		[Validator(typeof(TestModelWithOverridenPropertyNameValidator))]
		private class TestModelWithOverridenPropertyNameValueType {
			public int Id { get; set; }
		}

		private class TestModelWithOverridenMessageValueTypeValidator : AbstractValidator<TestModelWithOverridenMessageValueType> {
			public TestModelWithOverridenMessageValueTypeValidator() {
				RuleFor(x => x.Id).NotNull().WithMessage("Foo");
			}
		}

		private class TestModelWithOverridenPropertyNameValidator : AbstractValidator<TestModelWithOverridenPropertyNameValueType> {
			public TestModelWithOverridenPropertyNameValidator() {
				RuleFor(x => x.Id).NotNull().WithName("Foo");

			}
		}

		public class MockHttpContext : Mock<HttpContextBase> {
			public MockHttpContext() {
				Setup(x => x.Items).Returns(new Hashtable());
			}

			public static HttpContextBase Create() {
				return new MockHttpContext().Object;
			}
		}
	}
}