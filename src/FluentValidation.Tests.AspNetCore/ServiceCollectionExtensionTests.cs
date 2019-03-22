namespace FluentValidation.Tests {
	using Microsoft.Extensions.DependencyInjection;
	using Xunit;

	public class ServiceCollectionExtensionTests {
		public class TestClass { }
		public class TestValidator : AbstractValidator<TestClass> { }
		public class TestValidator2 : AbstractValidator<TestClass> { }

		[Fact]
		public void Should_resolve_validator_auto_registered_from_assembly_as_self() {
			var serviceProvider = new ServiceCollection()
				.AddValidatorsFromAssemblyContaining<ServiceCollectionExtensionTests>()
				.BuildServiceProvider();

			serviceProvider.GetService<TestValidator>().ShouldNotBeNull();
		}

		[Fact]
		public void Should_resolve_validator_auto_registered_from_assembly_as_interface() {
			var serviceProvider = new ServiceCollection()
				.AddValidatorsFromAssemblyContaining<ServiceCollectionExtensionTests>()
				.BuildServiceProvider();

			serviceProvider.GetService<IValidator<TestClass>>().ShouldNotBeNull();
		}
	}
}