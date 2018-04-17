using System;
using System.Collections.Generic;
using System.Text;

namespace FluentValidation
{
	public class ValidatorInjector<TValidator> : IValidatorInjector<TValidator>
		where TValidator : IValidator, new()
	{
		private TValidator validator;

		public TValidator Validator => validator;

		public ValidatorInjector()
		{
			validator = new TValidator();
		}
	}
}
