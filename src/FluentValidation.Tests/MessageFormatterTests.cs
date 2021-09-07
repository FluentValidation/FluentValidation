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
	using System.Globalization;
	using Internal;
	using Xunit;

	public class MessageFormatterTests {
		MessageFormatter formatter;

		public MessageFormatterTests() {
			CultureScope.SetDefaultCulture();
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
		public void Format_property_value() {
			string result = formatter
				.AppendPropertyValue(123.45)
				.BuildMessage("{PropertyValue:#.#}");

			result.ShouldEqual("123.5");
		}

		[Fact]
		public void Understands_numeric_formats() {
			string result;
			using (new CultureScope(CultureInfo.InvariantCulture)) {
				result = formatter
					.AppendArgument("d", 123)
					.AppendArgument("e", 1052.0329112756)
					.AppendArgument("c", -1234.56789)
					.AppendArgument("f", 1234.567)
					.AppendArgument("p", 0.912)
					.AppendArgument("i", 1234.567)
					.BuildMessage("{c:c3} {c:c4} {f:f} {p:p} {d:d} {e:e} {e:e2} {d:0000} {i}");
			}

			result.ShouldEqual("(¤1,234.568) (¤1,234.5679) 1234.57 91.20 % 123 1.052033e+003 1.05e+003 0123 1234.567");
		}

		[Fact]
		public void Understands_date_formats() {
			string result;
			var now = new DateTimeOffset(2015, 5, 16, 5, 50, 06, 719, TimeSpan.FromHours(-4));
			using (new CultureScope(CultureInfo.InvariantCulture)) {
				result = formatter
					.AppendArgument("now", now)
					.BuildMessage("{now:g} {now:MM-dd-yy} {now:f} {now}");
			}

			result.ShouldEqual("05/16/2015 05:50 05-16-15 Saturday, 16 May 2015 05:50 05/16/2015 05:50:06 -04:00");
		}

		[Fact]
		public void Should_ignore_unknown_parameters() {
			string result;
			var now = new DateTimeOffset(2015, 5, 16, 5, 50, 06, 719, TimeSpan.FromHours(-4));
			using (new CultureScope(CultureInfo.InvariantCulture)) {
				result = formatter
					.AppendArgument("foo", now)
					.BuildMessage("{foo:g} {unknown} {unknown:format}");
			}

			result.ShouldEqual("05/16/2015 05:50 {unknown} {unknown:format}");
		}

		[Fact]
		public void Should_ignore_unknown_numbered_parameters() {
			var now = new DateTime(2018, 2, 1);
			string result = formatter
				.AppendArgument("foo", now)
				.BuildMessage("{foo:yyyy-MM-dd} {0}");

			result.ShouldEqual("2018-02-01 {0}");
		}
	}
}
