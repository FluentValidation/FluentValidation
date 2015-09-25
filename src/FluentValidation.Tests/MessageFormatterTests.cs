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
	}
}