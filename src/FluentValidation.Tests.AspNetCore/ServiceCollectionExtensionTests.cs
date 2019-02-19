using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentValidation;

namespace FluentValidation.Tests
{

	public class ServiceCollectionExtensionTests
	{
		public class TestClass { }
		public class TestValidator : AbstractValidator<TestClass> { }

		[Fact]
		public void ServiceCollectionExtensionTests_should_pass_if_validator_is_resolved()
		{
			var serviceProvider = new ServiceCollection()
				.AddValidatorsFromAssemblyContaining<TestValidator>()
				.BuildServiceProvider();

			serviceProvider.GetService<IValidator<TestClass>>().ShouldNotBeNull();
			serviceProvider.GetService<TestValidator>().ShouldNotBeNull();
		}
	}
}