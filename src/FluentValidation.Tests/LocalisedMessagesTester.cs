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
	using System.Diagnostics;
	using System.Globalization;
	using System.Linq;
	using System.Threading;
	using Internal;
	using NUnit.Framework;
	using Resources;
	using Validators;

	[TestFixture]
	public class LocalisedMessagesTester {

		[TearDown]
		public void Teardown() {
			// ensure the resource provider is reset after any tests that use it.
			ValidatorOptions.ResourceProviderType = null;
		}

		[Test]
		public void Correctly_assigns_default_localized_error_message() {
			var originalCulture = Thread.CurrentThread.CurrentUICulture;
			try {
				var validator = new TestValidator(v => v.RuleFor(x => x.Surname).NotEmpty());

				foreach (var culture in new[] { "en", "de", "fr", "es", "de", "it", "nl", "pl", "pt", "ru", "sv" }) {
					Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);
					var message = Messages.ResourceManager.GetString("notempty_error");
					var errorMessage = new MessageFormatter().AppendPropertyName("Surname").BuildMessage(message);
					Debug.WriteLine(errorMessage);
					var result = validator.Validate(new Person{Surname = null});
					result.Errors.Single().ErrorMessage.ShouldEqual(errorMessage);
				}
			}
			finally {
				// Always reset the culture.
				Thread.CurrentThread.CurrentUICulture = originalCulture;
			}
		}

		[Test]
		public void ResourceProviderType_overrides_default_messagesnote() {
			ValidatorOptions.ResourceProviderType = typeof(MyResources);

			var validator = new TestValidator() {
				v => v.RuleFor(x => x.Surname).NotEmpty()
			};

			var result = validator.Validate(new Person());
			result.Errors.Single().ErrorMessage.ShouldEqual("foo");
		}

		[Test]
		public void Sets_localised_message_via_expression() {
			var validator = new TestValidator();
			validator.RuleFor(x => x.Surname).NotEmpty().WithLocalizedMessage(() => MyResources.notempty_error);
			var result = validator.Validate(new Person());
			result.Errors.Single().ErrorMessage.ShouldEqual("foo");
		}

		[Test]
		public void When_using_explicitly_localized_message_does_not_fall_back_to_ResourceProvider() {
			ValidatorOptions.ResourceProviderType = typeof(MyResources);

			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotEmpty().WithLocalizedMessage(() => MyOverridenResources.notempty_error)
			};

			var results = validator.Validate(new Person());
			results.Errors.Single().ErrorMessage.ShouldEqual("bar");
		}

		[Test]
		public void Custom_property_validators_should_respect_ResourceProvider() {
			ValidatorOptions.ResourceProviderType = typeof(MyResources);
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).SetValidator(new MyPropertyValidator())
			};

			var results = validator.Validate(new Person());
			results.Errors.Single().ErrorMessage.ShouldEqual("foo");
		}


		[Test]
		public void When_using_explicitly_localized_message_with_custom_validator_does_not_fall_back_to_ResourceProvider() {
			ValidatorOptions.ResourceProviderType = typeof(MyResources);

			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).SetValidator(new MyPropertyValidator())
					.WithLocalizedMessage(() => MyOverridenResources.notempty_error)
			};

			var results = validator.Validate(new Person());
			results.Errors.Single().ErrorMessage.ShouldEqual("bar");
		}

		[Test]
		public void Can_use_placeholders_with_localized_messages() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotNull().WithLocalizedMessage(() => TestMessages.PlaceholderMessage, 1)
			};

			var result = validator.Validate(new Person());
			result.Errors.Single().ErrorMessage.ShouldEqual("Test 1");
		}

		[Test]
		public void Can_use_placeholders_with_localized_messages_using_expressions() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotNull().WithLocalizedMessage(() => TestMessages.PlaceholderMessage, x => 1)
			};

			var result = validator.Validate(new Person());
			result.Errors.Single().ErrorMessage.ShouldEqual("Test 1");
		}

		private class MyResources {
			public static string notempty_error {
				get { return "foo"; }
			}
		}

		private class MyOverridenResources {
			public static string notempty_error {
				get { return "bar"; }
			}
		}

		private class MyPropertyValidator : PropertyValidator {
			public MyPropertyValidator() : base(() => MyOverridenResources.notempty_error) {
				
			}

			protected override bool IsValid(PropertyValidatorContext context) {
				return false;
			}
		}
	}
}