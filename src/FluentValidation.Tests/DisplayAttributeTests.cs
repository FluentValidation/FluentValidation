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
	using System.ComponentModel;
	using System.ComponentModel.DataAnnotations;
	using System.Globalization;
	using System.Threading;
	using Xunit;
	using System.Linq;

	
	public class DisplayAttributeTests {

		public DisplayAttributeTests() {
           CultureScope.SetDefaultCulture();
        }
#if !PORTABLE40
		[Fact]
		public void Infers_display_name_from_DisplayAttribute() {
			var validator = new InlineValidator<DisplayNameTestModel> {
				v => v.RuleFor(x => x.Name1).NotNull()
			};

			var result = validator.Validate(new DisplayNameTestModel());
			result.Errors.Single().ErrorMessage.ShouldEqual("'Foo' must not be empty.");
		}
#endif

#if !CoreCLR
		[Fact]
		public void Infers_display_name_from_DisplayNameAttribute() {
			var validator = new InlineValidator<DisplayNameTestModel> {
				v => v.RuleFor(x => x.Name2).NotNull()
			};

			var result = validator.Validate(new DisplayNameTestModel());
			result.Errors.Single().ErrorMessage.ShouldEqual("'Bar' must not be empty.");
		}
#endif

        public class DisplayNameTestModel {
#if !PORTABLE40
			[Display(Name = "Foo")]
			public string Name1 { get; set; }
#endif
#if !CoreCLR
			[DisplayName("Bar")]
#endif
			public string Name2 { get; set; }
		}
	}
}