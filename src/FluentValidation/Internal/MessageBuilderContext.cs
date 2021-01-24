namespace FluentValidation.Internal {
	using System;
	using Resources;
	using Validators;

	public class MessageBuilderContext<T,TProperty> {
		private ValidationContext<T> _innerContext;
		private TProperty _value;

		public MessageBuilderContext(ValidationContext<T> innerContext, TProperty value, PropertyValidator<T,TProperty> propertyValidator) {
			_innerContext = innerContext;
			_value = value;
			PropertyValidator = propertyValidator;
		}

		public PropertyValidator<T,TProperty> PropertyValidator { get; }

		public ValidationContext<T> ParentContext => _innerContext;

		// public IValidationRule<T> Rule => _innerContext.Rule;

		public string PropertyName => _innerContext.PropertyName;

		public string DisplayName => _innerContext.DisplayName;

		public MessageFormatter MessageFormatter => _innerContext.MessageFormatter;

		public T InstanceToValidate => _innerContext.InstanceToValidate;
		public TProperty PropertyValue => _value;

		public string GetDefaultMessage() {
			return PropertyValidator.GetErrorMessage(_innerContext, _value);
		}
	}
}
