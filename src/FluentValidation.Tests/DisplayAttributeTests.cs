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
	using System.ComponentModel;
	using System.ComponentModel.DataAnnotations;
	using System.Globalization;
	using System.Threading;
	using NUnit.Framework;
	using System.Linq;

	[TestFixture]
	public class DisplayAttributeTests {

		[SetUp]
		public void Setup() {
			Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
		}

		[Test]
		public void Infers_display_name_from_DisplayAttribute() {
			var validator = new InlineValidator<DisplayNameTestModel> {
				v => v.RuleFor(x => x.Name1).NotNull()
			};

			var result = validator.Validate(new DisplayNameTestModel());
			result.Errors.Single().ErrorMessage.ShouldEqual("'Foo' must not be empty.");
		}

		[Test]
		public void Infers_display_name_from_DisplayNameAttribute() {
			var validator = new InlineValidator<DisplayNameTestModel> {
				v => v.RuleFor(x => x.Name2).NotNull()
			};

			var result = validator.Validate(new DisplayNameTestModel());
			result.Errors.Single().ErrorMessage.ShouldEqual("'Bar' must not be empty.");
		}

		public class DisplayNameTestModel {
			[Display(Name = "Foo")]
			public string Name1 { get; set; }

			[DisplayName("Bar")]
			public string Name2 { get; set; }
		}
	}
}