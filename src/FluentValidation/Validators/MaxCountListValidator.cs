namespace FluentValidation.Validators {
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class MaxCountListValidator : PropertyValidator {
		private readonly int _countLimit;

		public MaxCountListValidator(int countLimit) {
			_countLimit = countLimit;
		}

		protected override bool IsValid(PropertyValidatorContext context) {
			var list = context.PropertyValue as IList<Object>;
			if (list == null)
				return true;

			var valid = list.Count() <= _countLimit;
			return valid;
		}

		protected override string GetDefaultMessageTemplate() {
			return Localized(nameof(MaxCountListValidator));
		}

		public int ValueToCompare => _countLimit;
	}
}
