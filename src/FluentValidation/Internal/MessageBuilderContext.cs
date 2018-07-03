namespace FluentValidation.Internal {
	using Resources;
	using Validators;

	public class MessageBuilderContext {
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

		public object Instance => _innerContext.Instance;

		public MessageFormatter MessageFormatter => _innerContext.MessageFormatter;

		public object PropertyValue => _innerContext.PropertyValue;

		public string GetDefaultMessage() {
			return MessageFormatter.BuildMessage(ErrorSource.GetString(_innerContext));
		}

		public static implicit operator PropertyValidatorContext(MessageBuilderContext ctx) {
			return ctx._innerContext;
		}

	}
}