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
	using System.Linq;
	using Xunit;

	
	public class CustomMessageFormatTester {
		private TestValidator validator;

		public CustomMessageFormatTester() {
			validator = new TestValidator();
		}

		[Fact]
		public void Should_format_custom_message() {
			const string expected = "Surname";
			validator.RuleFor(x => x.Surname).NotNull().WithMessage("{PropertyName}");
			string error = validator.Validate(new Person()).Errors.Single().ErrorMessage;
			error.ShouldEqual(expected);
        }

		[Fact]
		public void Should_format_validation_message_with_custom_args() {
			const string expected = "Property Name: Surname Custom: One, Custom: Two";
			validator.RuleFor(x => x.Surname).NotNull().WithMessage("Property Name: {PropertyName} Custom: {0}, Custom: {1}", "One", "Two");
			string error = validator.Validate(new Person()).Errors.Single().ErrorMessage;
			error.ShouldEqual(expected);
		}

		[Fact]
		public void Should_format_validation_with_property_values() {
			const string expected = "Property Name: Surname Custom: Foo";
			validator.RuleFor(x => x.Surname).NotNull().WithMessage("Property Name: {PropertyName} Custom: {0}", x => x.Forename);
			var person = new Person { Forename = "Foo" };

			string error = validator.Validate(person).Errors.Single().ErrorMessage;
			error.ShouldEqual(expected);
		}

		[Fact]
		public void Uses_custom_delegate_for_building_message() {
			validator.RuleFor(x => x.Surname).NotNull().Configure(cfg => {
				cfg.MessageBuilder = context => "Test " + ((Person)context.Instance).Id;
			});

			var error = validator.Validate(new Person()).Errors.Single().ErrorMessage;
			error.ShouldEqual("Test 0");
		}

		[Fact]
		public void Uses_property_value_in_message() {
			validator.RuleFor(x => x.Surname).NotEqual("foo").WithMessage("was {0}", (person, name) => name);
			var error = validator.Validate(new Person { Surname = "foo"}).Errors.Single().ErrorMessage;
			error.ShouldEqual("was foo");
		}

		[Fact]
		public void Replaces_propertyvalue_placeholder() {
			validator.RuleFor(x => x.Email).EmailAddress().WithMessage("Was '{PropertyValue}'");
			var result = validator.Validate(new Person() {Email = "foo"});
			result.Errors.Single().ErrorMessage.ShouldEqual("Was 'foo'");
		}

		[Fact]
		public void Replaces_propertyvalue_with_empty_string_when_null() {
			validator.RuleFor(x => x.Surname).NotNull().WithMessage("Was '{PropertyValue}'");
			var result = validator.Validate(new Person());
			result.Errors.Single().ErrorMessage.ShouldEqual("Was ''");
		}
		
	}
}