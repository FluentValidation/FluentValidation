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
	using System.Resources;
	using System.Threading;
	using NUnit.Framework;
	using Resources;
	using Validators;

	[TestFixture]
	public class LocalisedMessagesTester {
		private ResourceManager resourceManager;

		[SetUp]
		public void Setup() {
			Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
			resourceManager = new ResourceManager("FluentValidation.Tests.TestMessages", typeof(LocalisedMessagesTester).Assembly);
			DefaultResourceManager.SetResourceManagerProvider(() => resourceManager);
		}

		[TearDown]
		public void Teardown() {
			Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
			DefaultResourceManager.SetResourceManagerProvider(() => new DefaultResourceManager());
		}

		[Test]
		public void Should_use_custom_resource_manager() {
			var result = new NotNullValidator<Person, string>().Validate(new PropertyValidatorContext<Person, string>(null, null, x => null, null, null));
			result.Error.ShouldEqual("Localised Error");
		}

		[Test]
		public void Should_use_localised_resources() {
			Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
			var result = new NotNullValidator<Person, string>().Validate(new PropertyValidatorContext<Person, string>(null, null, x => null, null, null));
			result.Error.ShouldEqual("Localised Error (FR)");
		}

		[Test]
		public void Should_fall_back_to_default_resources() {
			Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU");
			var result = new NotNullValidator<Person, string>().Validate(new PropertyValidatorContext<Person, string>(null, null, x => null, null, null));
			result.Error.ShouldEqual("Localised Error");
		}
	}
}