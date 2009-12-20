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
	using Microsoft.Practices.ServiceLocation;
	using Moq;
	using NUnit.Framework;

	[TestFixture]
	public class ValidatorFactoryTester {
		private Mock<IServiceLocator> locator;
		private IValidatorFactory factory;

		[SetUp]
		public void Setup() {
			locator = new Mock<IServiceLocator>();
			factory = new DefaultValidatorFactory(locator.Object);
		}

		[Test]
		public void Should_obtain_from_validator_factory_using_generic() {
			var mockValidator = new Mock<IValidator<Person>>();
			locator.Setup(x => x.GetInstance(typeof(IValidator<Person>))).Returns(mockValidator.Object);

			var validator = factory.GetValidator<Person>();
			validator.ShouldBeTheSameAs(mockValidator.Object);
		}

		[Test]
		public void Should_obtain_from_validator_using_non_generic_type() {
			var mockValidator = new Mock<IValidator<Person>>();
			locator.Setup(x => x.GetInstance(typeof(IValidator<Person>))).Returns(mockValidator.Object);

			var validator = factory.GetValidator(typeof(Person));
			validator.ShouldBeTheSameAs(mockValidator.Object);
		}
	}
}