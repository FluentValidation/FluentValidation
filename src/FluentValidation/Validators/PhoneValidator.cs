using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using FluentValidation.Resources;
using System.Text.RegularExpressions;

namespace FluentValidation.Validators
{
    public class PhoneValidator : PropertyValidator, IRegularExpressionValidator
	{
		private readonly Regex regex;
		private string expression = "^\\(?([0-9]{3})\\)?[-.\\s]?([0-9]{3})[-.\\s]?([0-9]{4})$";

		public PhoneValidator() : base(new LanguageStringSource(nameof(PhoneValidator))) {
			regex = new Regex(expression, RegexOptions.IgnoreCase);
		}

		public string Expression
		{
			get
			{
				return expression;
			}
			//set { expression = value; }
		}

		protected override bool IsValid(PropertyValidatorContext context)
		{
			if (context.PropertyValue == null) return true;

			if (!regex.IsMatch((string)context.PropertyValue))
			{
				return false;
			}

			return true;
		}
	}
}
