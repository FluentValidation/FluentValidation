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
	using System;
	using System.Globalization;
	using System.Resources;
	using System.Threading;
	using Attributes;
	using Moq;
	using NUnit.Framework;
	using Resources;

	[TestFixture]
	public class ValidationMessageAttributeTests {

		Mock<ResourceManager> resourceManager;

		[SetUp]
		public void Setup() {
			Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
			resourceManager = new Mock<ResourceManager>();
			DefaultResourceManager.SetResourceManagerProvider(() => resourceManager.Object);
		}

		[TearDown]
		public void Teardown() {
			DefaultResourceManager.SetResourceManagerProvider(() => new DefaultResourceManager());
		}

		[Test]
		public void Throws_when_there_is_no_attribute() {
			typeof(InvalidOperationException).ShouldBeThrownBy(() => ValidationMessageAttribute.GetMessage(typeof(object)));
		}

		[Test]
		public void Should_return_keyed_message() {
			resourceManager.Setup(x => x.GetString("key", CultureInfo.CurrentCulture)).Returns("keyed message");
			var result = ValidationMessageAttribute.GetMessage(typeof(AttributedWithKey));
			result.ShouldEqual("keyed message");
		}

		[Test]
		public void Should_return_message() {
			ValidationMessageAttribute.GetMessage(typeof(AttributedWithMessage)).ShouldEqual("message");
		}

		[Test]
		public void Throws_when_there_is_no_key_or_message() {
			typeof(InvalidOperationException).ShouldBeThrownBy(() => ValidationMessageAttribute.GetMessage(typeof(AttributedWithNoKeyOrMessage)));
		}

		[Test]
		public void Throws_if_resourcemanager_returns_null() {
			typeof(InvalidOperationException).ShouldBeThrownBy(() => ValidationMessageAttribute.GetMessage(typeof(AttributedWithKey)));
		}

		[ValidationMessage(Key = "key")]
		private class AttributedWithKey {
			
		}

		[ValidationMessage(Message = "message")]
		private class AttributedWithMessage {
			
		}

		[ValidationMessage]
		private class AttributedWithNoKeyOrMessage {
			
		}
	}
}