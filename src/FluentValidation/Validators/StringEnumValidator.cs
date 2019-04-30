namespace FluentValidation.Validators
{
	using System;
	using System.Linq;
	using System.Reflection;
	using FluentValidation.Internal;
	using Resources;

	public class StringEnumValidator : PropertyValidator
	{
		private readonly Type _enumType;

		private readonly bool _caseSensitive;

		public StringEnumValidator(Type enumType) : this(enumType, false)
		{
		}

		public StringEnumValidator(Type enumType, bool caseSensitive) : base(new LanguageStringSource(nameof(StringEnumValidator)))
		{
			_enumType = enumType;
			_caseSensitive = caseSensitive;
		}

		protected override bool IsValid(PropertyValidatorContext context)
		{
			if (context.PropertyValue == null) return true;

			string value = context.PropertyValue.ToString();
			var comparison = _caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

			return Enum.GetNames(_enumType).Any(n => n.Equals(value, comparison));
		}
	}
}
