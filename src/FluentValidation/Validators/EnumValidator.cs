namespace FluentValidation.Validators {
	using System;
	using FluentValidation.Internal;

	public class EnumValidator : PropertyValidator {
		private readonly Type _enumType;

		public EnumValidator(Type enumType) : base("Property {PropertyName} it not a valid enum value.") {
			this._enumType = enumType;
		}

		protected override bool IsValid(PropertyValidatorContext context) {
			if (!_enumType.IsEnum()) return false;
			return Enum.IsDefined(_enumType, context.PropertyValue);
		}
	}
}