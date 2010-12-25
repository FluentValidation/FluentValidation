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
// The latest version of this file can be found at http://fluentvalidation.codeplex.com
#endregion

namespace FluentValidation.Tests {
	using System;
	using System.Globalization;
	using System.Linq.Expressions;
	using System.Web.Mvc;
	using Attributes;
	using Moq;
	using Mvc;
	using NUnit.Framework;
	using Internal;
	using System.Linq;

	[TestFixture]
	public class ClientsideMessageTester {
		InlineValidator<TestModel> validator;

		[SetUp]
		public void Setup() {
			System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");
			validator = new InlineValidator<TestModel>();
		}

		[Test]
		public void NotNull_uses_simplified_message_for_clientside_validation() {
			validator.RuleFor(x => x.Name).NotNull();
			var clientRule = GetClientRule(x => x.Name);
			clientRule.ErrorMessage.ShouldEqual("'Name' must not be empty.");
		}

		[Test]
		public void NotEmpty_uses_simplified_message_for_clientside_validation() {
			validator.RuleFor(x => x.Name).NotEmpty();
			var clientRule = GetClientRule(x => x.Name);
			clientRule.ErrorMessage.ShouldEqual("'Name' should not be empty.");
		}

		[Test]
		public void RegexValidator_uses_simplified_message_for_clientside_validation() {
			validator.RuleFor(x => x.Name).Matches("\\d");
			var clientRule = GetClientRule(x => x.Name);
			clientRule.ErrorMessage.ShouldEqual("'Name' is not in the correct format.");
		}

		[Test]
		public void EmailValidator_uses_simplified_message_for_clientside_validation() {
			validator.RuleFor(x => x.Name).EmailAddress();
			var clientRule = GetClientRule(x => x.Name);
			clientRule.ErrorMessage.ShouldEqual("'Name' is not a valid email address.");
		}

		[Test]
		public void LengthValidator_uses_simplified_message_for_clientside_validatation() {
			validator.RuleFor(x => x.Name).Length(1, 10);
			var clientRule = GetClientRule(x => x.Name);
			clientRule.ErrorMessage.ShouldEqual("'Name' must be between 1 and 10 characters.");
		}

		[Test]
		public void Should_not_munge_custom_message() {
			validator.RuleFor(x => x.Name).Length(1, 10).WithMessage("Foo");
			var clientRule = GetClientRule(x => x.Name);
			clientRule.ErrorMessage.ShouldEqual("Foo");
		}

		[Test]
		public void ExactLengthValidator_uses_simplified_message_for_clientside_validation() {
			validator.RuleFor(x => x.Name).Length(5);
			var clientRule = GetClientRule(x => x.Name);
			clientRule.ErrorMessage.ShouldEqual("'Name' must be 5 characters in length.");
		}

		private ModelClientValidationRule GetClientRule(Expression<Func<TestModel, object>> expression) {
			var propertyName = expression.GetMember().Name;
			var metadata = new DataAnnotationsModelMetadataProvider().GetMetadataForProperty(null, typeof(TestModel), propertyName);

			var factory = new Mock<IValidatorFactory>();
			factory.Setup(x => x.GetValidator(typeof(TestModel))).Returns(validator);

			var provider = new FluentValidationModelValidatorProvider(factory.Object);
			var propertyValidator = provider.GetValidators(metadata, new ControllerContext()).Single();

			var clientRule = propertyValidator.GetClientValidationRules().Single();
			return clientRule;
		}

		private class TestModel {
			public string Name { get; set; }
		}
	}
}