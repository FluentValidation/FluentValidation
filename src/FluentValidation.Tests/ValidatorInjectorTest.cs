using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FluentValidation.Tests
{
    public class ValidatorInjectorTest
    {
		
		[Fact]
		public void Should_create_new_instance_of_ValidatorInjectorObjectValidator()
		{
			var validatorInjector = new ValidatorInjector<ValidatorInjectorObjectValidator>();
			var model = new ValidatorInjectorObject
			{
				Id = 1,
				Description = "Description"
			};

			Assert.NotNull(validatorInjector.Validator);

			var validationResult = validatorInjector.Validator.Validate(model);
			Assert.NotNull(validationResult);
			Assert.True(validationResult.IsValid);
		}

	}
	public interface IValidatorInjectorObject
	{
		int Id { get; set; }
		string Description { get; set; }
	}
	public class ValidatorInjectorObject : IValidatorInjectorObject
	{
		public int Id { get; set; }
		public string Description { get; set; }
	}

	public class ValidatorInjectorObjectValidator : AbstractValidator<IValidatorInjectorObject>
	{
		public ValidatorInjectorObjectValidator()
		{
			RuleFor(x => x.Id).GreaterThan(0);
			RuleFor(x => x.Description).NotEmpty();
		}
	}
}
