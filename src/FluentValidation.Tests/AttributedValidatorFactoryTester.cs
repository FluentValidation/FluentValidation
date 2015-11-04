namespace FluentValidation.Tests
{
	using Attributes;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using Xunit;

	public class AttributedValidatorFactoryTester
	{
		private readonly AttributedValidatorFactory factory;

		public AttributedValidatorFactoryTester()
		{
			factory = new AttributedValidatorFactory();
		}

		[Fact]
		public void Should_instantiate_validator()
		{
			var validator = factory.GetValidator<AttributedPerson>();
			validator.ShouldBe<TestValidator>();
		}

		[Fact]
		public void Should_instantiate_validator_non_generic()
		{
			var validator = factory.GetValidator(typeof(AttributedPerson));
			validator.ShouldBe<TestValidator>();
		}

		[Fact]
		public void Should_return_null_when_null_is_passed_to_GetValidator()
		{
			factory.GetValidator((Type)null).ShouldBeNull();
		}

		[Fact]
		public void Should_return_null_when_type_has_no_attribute()
		{
			factory.GetValidator<NonAttributedPerson>().ShouldBeNull();
		}

		[Fact]
		public void Should_return_null_when_attribute_has_no_type()
		{
			factory.GetValidator<AttributedPersonWithNoType>().ShouldBeNull();
		}

		[Fact]
		public void Should_instantiate_parameter_validator()
		{
			var parameter = GetTestParameters().First(p => p.Name == "attributedArgument");
			var validator = factory.GetValidator(parameter);
			validator.ShouldBe<TestValidator>();
		}

		[Fact]
		public void Should_return_null_when_parameter_null_is_passed_to_GetValidator()
		{
			factory.GetValidator((ParameterInfo)null).ShouldBeNull();
		}

		[Fact]
		public void Should_return_null_when_parameter_has_no_attribute()
		{
			var parameter = GetTestParameters().First(p => p.Name == "nonAttributedArgument");
			factory.GetValidator(parameter).ShouldBeNull();
		}

		[Fact]
		public void Should_return_null_when_parameter_attribute_has_no_type()
		{
			var parameter = GetTestParameters().First(p => p.Name == "attributedArgumentWithNoType");
			factory.GetValidator(parameter).ShouldBeNull();
		}

		private static IList<ParameterInfo> GetTestParameters()
		{
			return typeof(SomeService).GetMethod("SomeMethod").GetParameters();
		}

		[Validator(typeof(TestValidator))]
		private class AttributedPerson
		{
		}

		private class NonAttributedPerson
		{
		}

		[Validator(null)]
		private class AttributedPersonWithNoType
		{
		}

		private class TestValidator : AbstractValidator<AttributedPerson>
		{
		}

		private class SomeService
		{
			public void SomeMethod(
				[Validator(typeof(TestValidator))] string attributedArgument,
				[Validator(null)] string attributedArgumentWithNoType,
				string nonAttributedArgument
			)
			{
			}
		}
	}
}