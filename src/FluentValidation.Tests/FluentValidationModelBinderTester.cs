namespace FluentValidation.Tests {
	using System;
	using System.Web.Mvc;
	using Attributes;
	using Mvc;
	using NUnit.Framework;
	using System.Linq;

	[TestFixture]
	public class FluentValidationModelBinderTester {
		FluentValidationModelBinder binder;

		[SetUp]
		public void Setup() {
			binder = new FluentValidationModelBinder(new AttributedValidatorFactory()); 
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
				RuleFor(x => x.Id).NotEmpty().WithMessage("Validation failed");
			}
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

			TestExtensions.ShouldEqual(bindingContext.ModelState["test.Name"].Errors.Single().ErrorMessage, "Validation Failed");
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
			TestExtensions.ShouldEqual(bindingContext.ModelState["Name"].Errors.Count(), 1);
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

			TestExtensions.ShouldEqual(bindingContext.ModelState["Id"].Errors.Single().ErrorMessage, "Validation failed");
		}
	}
}