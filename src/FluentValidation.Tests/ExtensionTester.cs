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
	using System;
	using System.Collections.Generic;
	using System.Linq.Expressions;
	using Xunit;
	using Internal;

	public class ExtensionTester {
		[Fact]
		public void Should_extract_member_from_member_expression() {
			Expression<Func<Person, string>> expression = person => person.Surname;
			var member = expression.GetMember();
			member.Name.ShouldEqual("Surname");
		}

		[Fact]
		public void Should_return_null_for_non_member_expressions() {
			Expression<Func<Person, string>> expression = person => "Foo";
			expression.GetMember().ShouldBeNull();
		}

		[Fact]
		public void Should_split_pascal_cased_member_name() {
			var cases = new Dictionary<string, string> {
				{"DateOfBirth", "Date Of Birth"},
				{"DATEOFBIRTH", "DATEOFBIRTH"},
				{"dateOfBirth", "date Of Birth"},
				{"dateofbirth", "dateofbirth"},
				{"Date_Of_Birth", "Date_ Of_ Birth"},
				{"Name2", "Name2"},
				{"ProductID", "Product ID"},
				{"MyTVRemote", "My TV Remote"},
				{"TVRemote", "TV Remote"},
				{"XCopy", "X Copy"},
				{"ThisXCopy", "This X Copy"},
				{"Address.Line1", "Address Line1"},
				{"Address..Line1", "Address. Line1"},
				{"address.Line1", "address Line1"},
				{"addressLine1", "address Line1"},
				{"address.line1", "address.line1"},
				{"address.line1.", "address.line1."}
			};

			foreach (var @case in cases) {
				string name = @case.Key.SplitPascalCase();
				name.ShouldEqual(@case.Value);
			}
		}

		[Fact]
		public void SplitPascalCase_should_return_null_when_input_is_null() {
			ExtensionsInternal.SplitPascalCase(null).ShouldBeNull();
		}
	}
}
