namespace FluentValidation.Internal {
	using System;
	using Resources;
	using Validators;

	public class MessageBuilderContext : ICommonContext {
		private PropertyValidatorContext _innerContext;

		[Obsolete("This constructor will be removed in FluentValidation 10. Use the other one instead.")]
		public MessageBuilderContext(PropertyValidatorContext innerContext, IStringSource errorSource, IPropertyValidator propertyValidator) {
			_innerContext = innerContext;
			ErrorSource = errorSource;
			PropertyValidator = propertyValidator;
		}

		public MessageBuilderContext(PropertyValidatorContext innerContext, IPropertyValidator propertyValidator) {
			_innerContext = innerContext;
			PropertyValidator = propertyValidator;
			// TODO: For backwards compatibility (remove in FV10).
#pragma warning disable 618
			ErrorSource = PropertyValidator.Options.ErrorMessageSource;
#pragma warning restore 618
		}


		public IPropertyValidator PropertyValidator { get; }

		[Obsolete("This property is deprecated and will be removed in FluentValidation 10.")]
		public IStringSource ErrorSource { get; }

		public IValidationContext ParentContext => _innerContext.ParentContext;

		public PropertyRule Rule => _innerContext.Rule;

		public string PropertyName => _innerContext.PropertyName;

		public string DisplayName => _innerContext.DisplayName;

		public MessageFormatter MessageFormatter => _innerContext.MessageFormatter;

		public object InstanceToValidate => _innerContext.InstanceToValidate;
		public object PropertyValue => _innerContext.PropertyValue;

		ICommonContext ICommonContext.ParentContext => ParentContext;

		public string GetDefaultMessage() {
			return PropertyValidator.Options.GetErrorMessageTemplate(_innerContext);
		}

		public static implicit operator PropertyValidatorContext(MessageBuilderContext ctx) {
			return ctx._innerContext;
		}

	}
}
