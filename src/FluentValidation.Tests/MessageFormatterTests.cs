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
	using Internal;
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Threading;
	using Xunit;


	public class MessageFormatterTests {
		MessageFormatter formatter;

		public MessageFormatterTests() {
			formatter = new MessageFormatter();
		}

		[Fact]
		public void Adds_value_to_message() {
			string result = formatter
				.AppendArgument("foo", "bar")
				.BuildMessage("{foo}");

			result.ShouldEqual("bar");
		}

		[Fact]
		public void Adds_PropertyName_to_message() {
			string result = formatter
				.AppendPropertyName("foo")
				.BuildMessage("{PropertyName}");

			result.ShouldEqual("foo");
		}

		[Fact]
		public void Adds_argument_and_custom_arguments() {
			string result = formatter
				.AppendArgument("foo", "bar")
				.AppendAdditionalArguments("baz")
				.BuildMessage("{foo} {0}");

			result.ShouldEqual("bar baz");
		}

		[Fact]
		public void Format_property_value()	{
			using (new CultureScope("en-US"))
			{
				string result = formatter
				.AppendPropertyValue(123.45)
				.BuildMessage("{PropertyValue:#.#}");

				result.ShouldEqual("123.5");
			}
		}

		[Fact]
		public void Understands_numeric_formats() {
			using (new CultureScope("en-US"))
			{
				string result = formatter
					.AppendArgument("d", 123)
					.AppendArgument("e", 1052.0329112756)
					.AppendArgument("c", -123.456789)
					.AppendArgument("f", 1234.567)
					.AppendArgument("p", 0.912)
					.BuildMessage("{c:c3} {c:c4} {f:f} {p:p} {d:d} {e:e} {e:e2} {d:0000}");

				result.ShouldEqual($"{-123.456789:c3} {-123.456789:c4} {1234.567:f} {0.912:p} {123:d} {1052.0329112756:e} {1052.0329112756:e2} {123:0000}");
			}
		}

		[Fact]
		public void Adds_formatted_argument_and_custom_arguments() {
			using (new CultureScope("en-US"))
			{
				string result = formatter
				.AppendArgument("foo", 123.43)
				.AppendAdditionalArguments("baz")
				.BuildMessage("{foo:#.#} {0}");

				result.ShouldEqual("123.4 baz");
			}
		}

		[Fact]
		public void Adds_formatted_argument_and_formatted_custom_arguments() {
			using (new CultureScope("en-US"))
			{
				string result = formatter
				.AppendArgument("foo", 123.43)
				.AppendAdditionalArguments(.6789)
				.BuildMessage("{foo:#.#} {0:p1}");

				result.ShouldEqual("123.4 67.9%");
			}
		}

		[Fact]
		public void Understands_date_formats() {
			var now = DateTime.Now;
			using (new CultureScope("en-US"))
			{
				string result = formatter
					.AppendArgument("now", now)
					.BuildMessage("{now:g} {now:MM-dd-yy} {now:f}");

				result.ShouldEqual($"{now:g} {now:MM-dd-yy} {now:f}");
			}
		}

		[Fact]
		public void Should_ignore_unknown_parameters() {
			var now = DateTime.Now;
			string result = formatter
				.AppendArgument("foo", now)
				.BuildMessage("{foo:g} {unknown} {unknown:format}");

			result.ShouldEqual($"{(now.ToString("g"))} {{unknown}} {{unknown:format}}");
		}

		[Fact]
		public void Should_not_call_formatted_arguments_when_nonexistant() {
			var mock = new FormatterMock();

			string result = mock
				.AppendPropertyName("foo")
				.BuildMessage("{PropertyName}");

			result.ShouldEqual("foo");
			mock.ReplacePlaceholderWithValueCalled.ShouldBeTrue();
			mock.ReplacePlaceholdersWithValuesCalled.ShouldBeFalse();
		}

		[Fact]
		public void Should_call_formatted_arguments_when_existant()	{
			var mock = new FormatterMock();

			string result = mock
				.AppendPropertyValue(123)
				.BuildMessage("{PropertyValue:d}");

			result.ShouldEqual("123");
			mock.ReplacePlaceholderWithValueCalled.ShouldBeFalse();
			mock.ReplacePlaceholdersWithValuesCalled.ShouldBeTrue();
		}

		private class FormatterMock : MessageFormatter {
			public bool ReplacePlaceholdersWithValuesCalled { get; set; }
			public bool ReplacePlaceholderWithValueCalled { get; set; }
			protected override string ReplacePlaceholdersWithValues(string template, IDictionary<string, object> values)
			{
				ReplacePlaceholdersWithValuesCalled = true;
				return base.ReplacePlaceholdersWithValues(template, values);
			}

			protected override string ReplacePlaceholderWithValue(string template, string key, object value)
			{
				ReplacePlaceholderWithValueCalled = true;
				return base.ReplacePlaceholderWithValue(template, key, value);
			}

		}
	}
}