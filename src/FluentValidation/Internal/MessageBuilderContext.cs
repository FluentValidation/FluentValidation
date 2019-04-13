namespace FluentValidation.Internal {
	using Resources;
	using Validators;

	public class MessageBuilderContext : IValidationContext {
		private PropertyValidatorContext _innerContext;

		public MessageBuilderContext(PropertyValidatorContext innerContext, IStringSource errorSource, IPropertyValidator propertyValidator) {
			_innerContext = innerContext;
			ErrorSource = errorSource;
			PropertyValidator = propertyValidator;
		}

		public IPropertyValidator PropertyValidator { get; }

		public IStringSource ErrorSource { get; }

		public ValidationContext ParentContext => _innerContext.ParentContext;

		public PropertyRule Rule => _innerContext.Rule;

		public string PropertyName => _innerContext.PropertyName;

		public string DisplayName => _innerContext.DisplayName;

		public MessageFormatter MessageFormatter => _innerContext.MessageFormatter;

		public object InstanceToValidate => _innerContext.InstanceToValidate;
		public object PropertyValue => _innerContext.PropertyValue;
		
		IValidationContext IValidationContext.ParentContext => ParentContext;

		public string GetDefaultMessage() {
			return MessageFormatter.BuildMessage(ErrorSource.GetString(_innerContext));
		}

		public static implicit operator PropertyValidatorContext(MessageBuilderContext ctx) {
			return ctx._innerContext;
		}

	}
}