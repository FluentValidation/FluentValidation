namespace FluentValidation.Tests {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Resources;
	using Attributes;
	using NUnit.Framework;
	using Resources;
	using Validators;

	[TestFixture]
	public class DefaultResouceManagerTester {
		[Test]
		public void Gets_default_resouce_manager() {
			DefaultResourceManager.Current.ShouldBe<DefaultResourceManager>();
		}

		[Test]
		public void Gets_custom_resource_manager() {
			DefaultResourceManager.SetResourceManagerProvider(() => new CustomResourceManager());
			DefaultResourceManager.Current.ShouldBe<CustomResourceManager>();
		}

		[Test]
		public void All_validators_should_declare_a_ValidationMessageAttribute_with_a_valid_resourcekey() {
			foreach (var type in PropertyValidatorTypes) {
				string message = ValidationMessageAttribute.GetMessage(type);
				Assert.IsNotNullOrEmpty(message);
			}
		}

		private IEnumerable<Type> PropertyValidatorTypes {
			get {
				return from type in typeof(NotNullValidator<,>).Assembly.GetExportedTypes()
					   where typeof(IPropertyValidator).IsAssignableFrom(type)
					   where !type.IsAbstract
					   where !type.IsInterface
					   where type != typeof(DelegatingValidator<,>) //DelegatingValidator does not create a validation message.
					   select type;
			}
		}

		[TearDown]
		public void Teardown() {
			DefaultResourceManager.SetResourceManagerProvider(() => new DefaultResourceManager());
		}

		private class CustomResourceManager : ResourceManager {
			
		}
	}
}