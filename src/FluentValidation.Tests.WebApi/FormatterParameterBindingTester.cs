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
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Net.Http;
	using System.Net.Http.Formatting;
	using System.Text;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Web;
	using System.Web.Http;
	using System.Web.Http.Controllers;
	using System.Web.Http.Metadata;
	using System.Web.Http.Metadata.Providers;
	using System.Web.Http.ModelBinding;
	using System.Web.Http.Validation;

	using Attributes;

	using FluentValidation.WebApi;
	using FluentValidation.Results;
    using Xunit;
	using Moq;


     //TODO: Remove these tests and make sure that WebApiIntegrationTests have replaced all the tests in here
	public class FormatterParameterBindingTester : IDisposable {
		FluentValidationModelValidatorProvider provider;
		HttpActionContext actionContext;
		ModelMetadataProvider modelMetadataProvider;

		public FormatterParameterBindingTester() {
			Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            provider = new FluentValidationModelValidatorProvider(new AttributedValidatorFactory());
			GlobalConfiguration.Configuration.Services.Clear(typeof(ModelValidatorProvider));
			GlobalConfiguration.Configuration.Services.Add(typeof(ModelValidatorProvider), provider);

			modelMetadataProvider = (ModelMetadataProvider)GlobalConfiguration.Configuration.Services.GetService(typeof(ModelMetadataProvider));

			actionContext = new HttpActionContext {
				ControllerContext = new HttpControllerContext {
					Request = new HttpRequestMessage(),
					Configuration = GlobalConfiguration.Configuration
				}
			};
		}

		public void Dispose() {
			//Cleanup
			GlobalConfiguration.Configuration.Services.Remove(typeof(ModelValidatorProvider), provider);
		}

		protected ModelMetadata CreateMetaData(Type type) {
			var meta = new DataAnnotationsModelMetadataProvider();
			return meta.GetMetadataForType(null, type);
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
			public int AnIntProperty { get; set; }
			public int CustomProperty { get; set; }
		}

		public class TestModel7Validator : AbstractValidator<TestModel7> {
			public TestModel7Validator() {
				//This ctor is intentionally blank.
				RuleFor(x => x.AnIntProperty).GreaterThan(10);
				Custom(
					model => {
						if (model.CustomProperty == 14) {
							return new ValidationFailure("CustomProperty", "Cannot be 14");
						}
						return null;
					});
			}
		}

		class MockContent : HttpContent {
			MockContent()
				: base() {

			}

			protected override Task SerializeToStreamAsync(Stream stream, TransportContext context) {
				throw new NotImplementedException();
			}

			protected override bool TryComputeLength(out long length) {
				length = -1L;
				return false;
			}
		}

		[Fact]
		public void Should_add_all_errors_in_one_go() {
			actionContext.Request.Content = JsonContent(@"{
				Email:'foo',
				Surname:'foo',
				Forename:'foo',
				DateOfBirth:null,
				Address1:null}");

			var binder = CreateParameterBinder("testModel4", typeof(TestModel4));
			binder.ExecuteBindingAsync(modelMetadataProvider, actionContext, new CancellationToken()).Wait();

			actionContext.ModelState.IsValidField("testModel4.Email").ShouldBeFalse(); //Email validation failed
			actionContext.ModelState.IsValidField("testModel4.DateOfBirth").ShouldBeFalse(); //Date of Birth not specified (implicit required error)
			actionContext.ModelState.IsValidField("testModel4.Surname").ShouldBeFalse(); //cross-property
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

		[Fact]
		public void Should_add_all_erorrs_in_one_go_when_NotEmpty_rule_specified_for_non_nullable_value_type() {
			actionContext.Request.Content = JsonContent(@"{
				SomeBool:'false',
				Id:0}");

			var binder = CreateParameterBinder("testModel5", typeof(TestModel5));
			binder.ExecuteBindingAsync(modelMetadataProvider, actionContext, new CancellationToken()).Wait();

			actionContext.ModelState.IsValidField("testModel5.SomeBool").ShouldBeFalse(); //Complex rule
			actionContext.ModelState.IsValidField("testModel5.Id").ShouldBeFalse(); //NotEmpty for non-nullable value type
		}

		[Fact]
		public void When_a_validation_error_occurs_the_error_should_be_added_to_modelstate() {
			actionContext.Request.Content = JsonContent(@"{
				Name:null}");

			var binder = CreateParameterBinder("testModel", typeof(TestModel));
			binder.ExecuteBindingAsync(modelMetadataProvider, actionContext, new CancellationToken()).Wait();

			actionContext.ModelState["testModel.Name"].Errors.Single().ErrorMessage.ShouldEqual("Validation Failed");
		}

		//web api errors seems to be always be added with prefix (I tried with the DataAnnotationsModelValidatorProvider and that's the behaviour that I observed
		//[Fact]
		//public void When_a_validation_error_occurs_the_error_should_be_added_to_Modelstate_without_prefix()
		//{
		//	var metadataProvider = (ModelMetadataProvider)GlobalConfiguration.Configuration.Services.GetService(typeof(ModelMetadataProvider));
		//	var validator = new DefaultBodyModelValidator();

		//	validator.Validate(new TestModel { Name = null }, typeof(TestModel), metadataProvider, actionContext, "request");

		//	actionContext.ModelState["request"].Errors.Count().ShouldEqual(1);
		//}

		[Fact]
		public void Should_not_fail_when_no_validator_can_be_found() {
			actionContext.Request.Content = JsonContent(@"{}");

			var binder = CreateParameterBinder("testModel2", typeof(TestModel2));
			binder.ExecuteBindingAsync(modelMetadataProvider, actionContext, new CancellationToken()).Wait();

			actionContext.ActionArguments["testModel2"].ShouldNotBeNull();
		}

		//for parse errors (trying to parse null to a value type int, datetime, etc) the formatter (json formatter in this case) takes care of them
		// and I didn't find a way to override that behaviour
		[Fact]
		public void Should_add_default_message_to_modelstate() {
			actionContext.Request.Content = JsonContent(@"{Id:''}");

			var binder = CreateParameterBinder("testModel3", typeof(TestModel3));
			binder.ExecuteBindingAsync(modelMetadataProvider, actionContext, new CancellationToken()).Wait();

			actionContext.ModelState["testModel3.Id"].Errors.Single().Exception.ShouldNotBeNull();
		}

		[Fact]
		public void Should_add_default_message_to_modelstate_when_there_is_no_required_validator_explicitly_specified() {
			actionContext.Request.Content = JsonContent(@"{Id:''}");

			var binder = CreateParameterBinder("testModel6", typeof(TestModel6));
			binder.ExecuteBindingAsync(modelMetadataProvider, actionContext, new CancellationToken()).Wait();

			actionContext.ModelState["testModel6.Id"].Errors.Single().Exception.ShouldNotBeNull();
		}

		[Fact]
		public void Should_validate_greater_than() {
			actionContext.Request.Content = JsonContent(@"{AnIntProperty:'5'}");

			var binder = CreateParameterBinder("testModel7", typeof(TestModel7));
			binder.ExecuteBindingAsync(modelMetadataProvider, actionContext, new CancellationToken()).Wait();

			actionContext.ModelState.IsValidField("testModel7.AnIntProperty").ShouldBeFalse();
		}

		[Fact]
		public void Should_validate_custom_after_property_errors() {
			actionContext.Request.Content = JsonContent(@"{AnIntProperty:'7',CustomProperty:'14'}");

			var binder = CreateParameterBinder("testModel7", typeof(TestModel7));
			binder.ExecuteBindingAsync(modelMetadataProvider, actionContext, new CancellationToken()).Wait();

			actionContext.ModelState.IsValidField("testModel7.CustomProperty").ShouldBeFalse();
		}

		private static FormatterParameterBinding CreateParameterBinder(string parameterName, Type parameterType) {
			return new FormatterParameterBinding(
				MockParameterDescriptor.Create(parameterName, parameterType),
				new List<MediaTypeFormatter> { new JsonMediaTypeFormatter() },
				new FluentValidationBodyModelValidator());
		}

		private static HttpContent JsonContent(string json) {
			return new StringContent(json, Encoding.UTF8, "application/json");
		}

		public class MockHttpContext : Mock<HttpContextBase> {
			public MockHttpContext() {
				Setup(x => x.Items).Returns(new Hashtable());
			}

			public static HttpContextBase Create() {
				return new MockHttpContext().Object;
			}
		}

		public class MockParameterDescriptor : Mock<HttpParameterDescriptor> {
			public MockParameterDescriptor(string parameterName, Type parameterType) {
				Setup(x => x.ParameterName).Returns(parameterName);
				Setup(x => x.ParameterType).Returns(parameterType);
			}

			public static HttpParameterDescriptor Create(string parameterName, Type parameterType) {
				return new MockParameterDescriptor(parameterName, parameterType).Object;
			}
		}
	}
}