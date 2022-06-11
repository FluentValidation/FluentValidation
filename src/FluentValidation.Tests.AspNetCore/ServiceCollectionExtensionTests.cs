namespace FluentValidation.Tests;

using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

public class ServiceCollectionExtensionTests {
	public class TestClass { }
	public class TestValidator : AbstractValidator<TestClass> { }
	public class TestValidator2 : AbstractValidator<TestClass> { }
	internal class TestValidatorInternal : AbstractValidator<TestClass> { }

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

	[Fact]
	public void AddValidatorsFromAssemblyContaining_T_When_Instructed_Should_Add_Internal_Validators() {
		var serviceCollection = new ServiceCollection()
			.AddValidatorsFromAssemblyContaining<ServiceCollectionExtensionTests>(includeInternalTypes: true);

		Assert.Contains(serviceCollection, o => o.ImplementationType == typeof(TestValidatorInternal));
	}

	[Fact]
	public void AddValidatorsFromAssemblyContaining_T_By_Default_Should_Not_Add_Internal_Validators() {
		var serviceCollection = new ServiceCollection()
			.AddValidatorsFromAssemblyContaining<ServiceCollectionExtensionTests>();

		Assert.DoesNotContain(serviceCollection, o => o.ImplementationType == typeof(TestValidatorInternal));
	}

	[Fact]
	public void AddValidatorsFromAssemblyContaining_When_Instructed_Should_Add_Internal_Validators() {
		var serviceCollection = new ServiceCollection()
			.AddValidatorsFromAssemblyContaining(typeof(ServiceCollectionExtensionTests), includeInternalTypes: true);

		Assert.Contains(serviceCollection, o => o.ImplementationType == typeof(TestValidatorInternal));
	}

	[Fact]
	public void AddValidatorsFromAssemblyContaining_By_Default_Should_Not_Add_Internal_Validators() {
		var serviceCollection = new ServiceCollection()
			.AddValidatorsFromAssemblyContaining(typeof(ServiceCollectionExtensionTests));

		Assert.DoesNotContain(serviceCollection, o => o.ImplementationType == typeof(TestValidatorInternal));
	}

	[Fact]
	public void AddValidatorsFromAssembly_When_Instructed_Should_Add_Internal_Validators() {
		var serviceCollection = new ServiceCollection()
			.AddValidatorsFromAssembly(typeof(ServiceCollectionExtensionTests).Assembly, includeInternalTypes: true);

		Assert.Contains(serviceCollection, o => o.ImplementationType == typeof(TestValidatorInternal));
	}

	[Fact]
	public void AddValidatorsFromAssembly_By_Default_Should_Not_Add_Internal_Validators() {
		var serviceCollection = new ServiceCollection()
			.AddValidatorsFromAssembly(typeof(ServiceCollectionExtensionTests).Assembly);

		Assert.DoesNotContain(serviceCollection, o => o.ImplementationType == typeof(TestValidatorInternal));
	}

	[Fact]
	public void AddValidatorsFromAssemblies_When_Instructed_Should_Add_Internal_Validators() {
		var serviceCollection = new ServiceCollection()
			.AddValidatorsFromAssemblies(new[] { typeof(ServiceCollectionExtensionTests).Assembly }, includeInternalTypes: true);

		Assert.Contains(serviceCollection, o => o.ImplementationType == typeof(TestValidatorInternal));
	}

	[Fact]
	public void AddValidatorsFromAssemblies_By_Default_Should_Not_Add_Internal_Validators() {
		var serviceCollection = new ServiceCollection()
			.AddValidatorsFromAssemblies(new[] { typeof(ServiceCollectionExtensionTests).Assembly });

		Assert.DoesNotContain(serviceCollection, o => o.ImplementationType == typeof(TestValidatorInternal));
	}
}
