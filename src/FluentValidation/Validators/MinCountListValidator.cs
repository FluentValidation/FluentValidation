namespace FluentValidation.Validators {
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class MinCountListValidator : PropertyValidator {
		private readonly int _countMin;

		public MinCountListValidator(int countMin) {
			_countMin = countMin;
		}

		protected override bool IsValid(PropertyValidatorContext context) {
			var list = context.PropertyValue as IList<Object>;
			if (list == null)
				return true;

			var valid = list.Count() >= _countMin;
			if (!valid)
				context.MessageFormatter.AppendArgument("ValueToCompare", ValueToCompare);

			return valid;
		}
		protected override string GetDefaultMessageTemplate() {
			return Localized(nameof(MinCountListValidator));
		}

		public int ValueToCompare => _countMin;
	}
}
