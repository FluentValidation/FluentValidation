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
	using System;
	using System.Linq;
	using Xunit;

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
				v => v.RuleFor(x => x.Surname).NotNull().WithName(x => MyResources.CustomProperty)
			};

			var result = validator.Validate(new Person());
			result.Errors.Single().ErrorMessage.ShouldEqual("'foo' must not be empty.");
		}

		[Fact]
		public void Uses_localized_name_expression() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotNull().WithName(x => MyResources.CustomProperty)
			};

			var result = validator.Validate(new Person());
			result.Errors.Single().ErrorMessage.ShouldEqual("'foo' must not be empty.");
		}

		public static class MyResources {
			public static string CustomProperty {
				get { return "foo"; }
			}
		}
	}
}
