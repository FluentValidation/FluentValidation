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
	using System.Globalization;
	using System.Threading;
	using NUnit.Framework;

	[TestFixture]
	public class ValidatorDescriptorTester {
		TestValidator validator;

		[SetUp]
		public void Setup() {
			Thread.CurrentThread.CurrentCulture = new CultureInfo("en-us");
			validator = new TestValidator();
		}

		[Test]
		public void Should_retrieve_name_given_to_it() {
			validator.RuleFor(x => x.Forename).NotNull().WithName("First Name");
			var descriptor = validator.CreateDescriptor();
			var name = descriptor.GetName(x => x.Forename);
			name.ShouldEqual("First Name");
		}

		[Test]
		public void Should_retrieve_name_given_to_it_pass_property_as_string() {
			validator.RuleFor(x => x.Forename).NotNull().WithName("First Name");
			var descriptor = validator.CreateDescriptor();
			var name = descriptor.GetName("Forename");
			name.ShouldEqual("First Name");
		}
	}
}