using System;

namespace FluentValidation.Validators
{
    public class EnumValidator<T> : PropertyValidator
    {
        public EnumValidator() : base("Property {PropertyName} it not a valid enum value.") { }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            if (!typeof(T).IsEnum) return false;
            return Enum.IsDefined(typeof(T), context.PropertyValue);
        }
    }
}
