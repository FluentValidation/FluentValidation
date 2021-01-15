namespace FluentValidation.Internal {
	using System;
	using Resources;
	using Validators;

	public class MessageBuilderContext<T,TProperty> {
		private PropertyValidatorContext<T,TProperty> _innerContext;

		public MessageBuilderContext(PropertyValidatorContext<T,TProperty> innerContext, PropertyValidator<T,TProperty> propertyValidator) {
			_innerContext = innerContext;
			PropertyValidator = propertyValidator;
		}

		public PropertyValidator<T,TProperty> PropertyValidator { get; }

		public ValidationContext<T> ParentContext => _innerContext.ParentContext;

		public IValidationRule<T> Rule => _innerContext.Rule;

		public string PropertyName => _innerContext.PropertyName;

		public string DisplayName => _innerContext.DisplayName;

		public MessageFormatter MessageFormatter => _innerContext.MessageFormatter;

		public object InstanceToValidate => _innerContext.InstanceToValidate;
		public object PropertyValue => _innerContext.PropertyValue;

		public string GetDefaultMessage() {
			return PropertyValidator.GetErrorMessage(_innerContext);
		}
		public static implicit operator PropertyValidatorContext<T,TProperty>(MessageBuilderContext<T,TProperty> ctx) {
			return ctx._innerContext;
		}
	}
}
