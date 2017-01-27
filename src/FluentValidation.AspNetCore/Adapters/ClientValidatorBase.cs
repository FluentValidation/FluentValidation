namespace FluentValidation.AspNetCore {
	using System;
	using System.Collections.Generic;
	using Internal;
	using Validators;
	using System.Linq;
	using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

	public abstract class ClientValidatorBase : IClientModelValidator {
		public IPropertyValidator Validator { get; private set; }
		public PropertyRule Rule { get; private set; }

		public ClientValidatorBase(PropertyRule rule, IPropertyValidator validator) {
			this.Validator = validator;
			this.Rule = rule;
		}

		public abstract void AddValidation(ClientModelValidationContext context);

		protected static bool MergeAttribute(IDictionary<string, string> attributes, string key, string value)
		{
			if (attributes.ContainsKey(key))
			{
				return false;
			}

			attributes.Add(key, value);
			return true;
		}
	}
}