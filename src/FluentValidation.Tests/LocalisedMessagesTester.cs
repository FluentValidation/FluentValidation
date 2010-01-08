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
	using System.Diagnostics;
	using System.Globalization;
	using System.Threading;
	using Internal;
	using NUnit.Framework;
	using Resources;
	using Validators;

	[TestFixture]
	public class LocalisedMessagesTester {

		[Test]
		public void Correctly_assigns_default_localized_error_message() {
			var originalCulture = Thread.CurrentThread.CurrentUICulture;
			try {
				var validator = new NotEmptyValidator(null);

				foreach (var culture in new[] {"en", "de", "fr"}) {
					Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);
					var message = Messages.ResourceManager.GetString("notempty_error");
					var errorMessage = new MessageFormatter().AppendPropertyName("name").BuildMessage(message);
					Debug.WriteLine(errorMessage);
					var result = validator.Validate(new PropertyValidatorContext("name", null, x => null));
					result.Error.ShouldEqual(errorMessage);
				}
			}
			finally {
				// Always reset the culture.
				Thread.CurrentThread.CurrentUICulture = originalCulture;
			}
		}

		[Test]
		public void Uses_custom_resouces() {
			ValidatorOptions.ResourceProviderType = typeof(MyResources);

			var validator = new NotEmptyValidator(null);
			var result = validator.Validate(new PropertyValidatorContext("name", null, x => null));
			result.Error.ShouldEqual("foo");

			ValidatorOptions.ResourceProviderType = null;
		}

		private class MyResources {
			public static string notempty_error {
				get { return "foo"; }
			}
		}
	}
}