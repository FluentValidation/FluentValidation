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
// The latest version of this file can be found at https://github.com/JeremySkinner/FluentValidation
#endregion

namespace FluentValidation.Tests {
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using Xunit;
    using System.Linq;
    using Resources;
    using System;

    public class LocalisedNameTester : IDisposable {

	    public LocalisedNameTester() {
		    CultureScope.SetDefaultCulture();
	    }

		public void Dispose() {
			CultureScope.SetDefaultCulture();
		}

		[Fact]
		public void Uses_localized_name() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotNull().WithLocalizedName(typeof(MyResources), nameof(MyResources.CustomProperty))
			};

			var result = validator.Validate(new Person());
			result.Errors.Single().ErrorMessage.ShouldEqual("'foo' must not be empty.");
		}

		[Fact]
		public void Uses_localized_name_expression() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotNull().WithLocalizedName(() => MyResources.CustomProperty)
			};

			var result = validator.Validate(new Person());
			result.Errors.Single().ErrorMessage.ShouldEqual("'foo' must not be empty.");
		}

		[Fact]
		public void Does_not_overwrite_resource_when_using_custom_ResourceProvider() {
			ValidatorOptions.ResourceProviderType = typeof(OverrideResources);
			
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotNull().WithLocalizedName(() => MyResources.CustomProperty)
			};

			var result = validator.Validate(new Person());
			result.Errors.Single().ErrorMessage.ShouldEqual("'foo' must not be empty.");
		}

		[Fact]
		public void Uses_localized_name_from_display_attribute() {
			using (new CultureScope("en-us")) {
				var validator = new InlineValidator<Person2> {
					v => v.RuleFor(x => x.Name).NotNull().WithMessage("{PropertyName}")
				};

				var result = validator.Validate(new Person2());
				result.Errors[0].ErrorMessage.ShouldEqual("foo");

				using (new CultureScope("fr-FR")) {
					result = validator.Validate(new Person2());
					result.Errors[0].ErrorMessage.ShouldEqual("bar");
				}
			}
		}

		public class Person2 {
			[Display(ResourceType = typeof(TestMessages), Name = "PropertyName")]
			public string Name { get; set; }
			 
		}

		public static class MyResources {
			public static string CustomProperty {
				get { return "foo"; }
			}
		}

		public static class OverrideResources {
			public static string CustomProperty {
				get { return "bar"; }
			}
		}
	}
}