#region License
// Copyright (c) .NET Foundation and contributors.
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
// The latest version of this file can be found at https://github.com/FluentValidation/FluentValidation
#endregion

namespace FluentValidation.Tests {
	using Xunit;

	public class TransformTests {
		[Fact]
		public void Transforms_property_value() {
			var validator = new InlineValidator<Person>();
			validator.RuleFor(x => x.Surname).Transform(name => "foo" + name).Equal("foobar");

			var result = validator.Validate(new Person {Surname = "bar"});
			result.IsValid.ShouldBeTrue();
		}

		[Fact]
		public void Transforms_property_value_to_another_type() {
			var validator = new InlineValidator<Person>();
			validator.RuleFor(x => x.Surname).Transform(name => 1).GreaterThan(10);

			var result = validator.Validate(new Person {Surname = "bar"});
			result.IsValid.ShouldBeFalse();
			result.Errors[0].ErrorCode.ShouldEqual("GreaterThanValidator");
		}

	}
}
