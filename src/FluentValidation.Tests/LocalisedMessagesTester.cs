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

namespace FluentValidation.Tests {
	using System;
	using System.Diagnostics;
	using System.Globalization;
	using System.Linq;
	using System.Threading;
	using Internal;
	using Xunit;
	using Resources;
	using Validators;

	
	public class LocalisedMessagesTester : IDisposable {

		public LocalisedMessagesTester() {
			// ensure the resource provider is reset after any tests that use it.
			CultureScope.SetDefaultCulture();
		}

        public void Dispose()
        {
			CultureScope.SetDefaultCulture();
		}

		[Fact]
		public void Correctly_assigns_default_localized_error_message() {

			var originalCulture = Thread.CurrentThread.CurrentUICulture;
			try {
				var validator = new TestValidator(v => v.RuleFor(x => x.Surname).NotEmpty());

				foreach (var culture in new[] { "en", "de", "fr", "es", "de", "it", "nl", "pl", "pt", "ru", "sv", "ar" }) {
					Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);
					var message = ValidatorOptions.LanguageManager.GetStringForValidator<NotEmptyValidator>();
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

		[Fact]
		public void Sets_localised_message_via_type_name() {
			var validator = new TestValidator();
			validator.RuleFor(x => x.Surname).NotEmpty().WithLocalizedMessage(typeof(MyResources), nameof(MyResources.notempty_error));
			var result = validator.Validate(new Person());

			result.Errors.Single().ErrorMessage.ShouldEqual("foo");
		}

		[Fact]
		public void Uses_func_to_get_message() {
			var validator = new TestValidator();
			validator.RuleFor(x => x.Forename).NotNull().WithMessage(x => "el foo");

			var result = validator.Validate(new Person());
			result.Errors[0].ErrorMessage.ShouldEqual("el foo");
		}

		[Fact]
		public void Formats_string_with_placeholders() {
			var validator = new TestValidator();
			validator.RuleFor(x => x.Forename).NotNull().WithMessage(x => string.Format("{{PropertyName}} {0}", x.AnotherInt));
			var result = validator.Validate(new Person());
			result.Errors[0].ErrorMessage.ShouldEqual("Forename 0");
		}


		[Fact]
		public void Formats_string_with_placeholders_when_you_cant_edit_the_string() {
			var validator = new TestValidator();
			validator.RuleFor(x => x.Forename).NotNull().WithMessage(x => string.Format("{{PropertyName}} {0}", x.AnotherInt));
			var result = validator.Validate(new Person());
			result.Errors[0].ErrorMessage.ShouldEqual("Forename 0");
		}

		[Fact]
		public void Uses_string_format_with_property_value() {
			var validator = new TestValidator();
			validator.RuleFor(x => x.Forename).Equal("Foo").WithMessage((x, forename) => $"Hello {forename}");
			var result = validator.Validate(new Person {Forename = "Jeremy"});
			result.Errors[0].ErrorMessage.ShouldEqual("Hello Jeremy");
		}

		[Fact]
		public void Does_not_throw_InvalidCastException_when_using_RuleForEach() {
			var validator = new InlineValidator<Person>();
			validator.RuleForEach(x => x.NickNames)
				.Must(x => false)
				.WithMessage((parent, name) => "x");

			var result = validator.Validate(new Person() {NickNames = new[] {"What"}});
			result.Errors.Single().ErrorMessage.ShouldEqual("x");
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
			public MyPropertyValidator() : base(nameof(MyOverridenResources.notempty_error), typeof(MyOverridenResources)) {
				
			}

			protected override bool IsValid(PropertyValidatorContext context) {
				return false;
			}
		}
	}
}