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
		public void Format_property_value()
		{
			using (new TemporalCultureExchange())
			{
				string result = formatter
				.AppendPropertyValue(123.45)
				.BuildMessage("{PropertyValue:#.#}");

				result.ShouldEqual("123.5");
			}
		}

		[Fact]
		public void Understands_numeric_formats()
		{
			using (new TemporalCultureExchange())
			{
				string result = formatter
					.AppendArgument("d", 123)
					.AppendArgument("e", 1052.0329112756)
					.AppendArgument("c", -123.456789)
					.AppendArgument("f", 1234.567)
					.AppendArgument("p", 0.912)
					.BuildMessage("{c:c3} {c:c4} {f:f} {p:p} {d:d} {e:e} {e:0.00} {d:0000}");

				result.ShouldEqual("($123.457) ($123.4568) 1234.57 91.20% 123 1.052033e+003 1052.03 0123");
			}
		}

		[Fact]
		public void Understands_date_formats()
		{
			var now = DateTime.Now;
			using (new TemporalCultureExchange())
			{
				string result = formatter
					.AppendArgument("now", now)
					.BuildMessage("{now:g} {now:MM-dd-yy} {now:f}");

				result.ShouldEqual($"{now.ToString("g")} {now.ToString("MM-dd-yy")} {now.ToString("f")}");
			}
		}

		[Fact]
		public void Should_ignore_unknown_parameters()
		{
			var now = DateTime.Now;
			string result = formatter
				.AppendArgument("foo", now)
				.BuildMessage("{foo:g} {unknown} {unknown:format}");

			result.ShouldEqual($"{(now.ToString("g"))} {{unknown}} {{unknown:format}}");
		}

		internal class TemporalCultureExchange : IDisposable
		{
			private bool disposedValue = false;
			private CultureInfo _culture;

			internal TemporalCultureExchange(string culture = "en-US")
			{
				_culture = Thread.CurrentThread.CurrentCulture;

				Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(culture);
			}

			protected virtual void Dispose(bool disposing)
			{
				if (!disposedValue)
				{
					if (disposing)
						Thread.CurrentThread.CurrentUICulture = _culture;

					disposedValue = true;
				}
			}

			public void Dispose()
			{
				Dispose(true);
			}
		}
	}
}