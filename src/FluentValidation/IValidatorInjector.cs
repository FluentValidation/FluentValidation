using System;
using System.Collections.Generic;
using System.Text;

namespace FluentValidation
{
	public interface IValidatorInjector<TValidator> where TValidator : IValidator, new()
	{
		TValidator Validator { get; }
	}
}
