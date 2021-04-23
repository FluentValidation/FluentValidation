namespace FluentValidation.Internal {
	using System;
	using Resources;
	using Validators;

	public interface IMessageBuilderContext<T, out TProperty> {
		IRuleComponent<T, TProperty> Component { get; }
		IPropertyValidator PropertyValidator { get; }
		ValidationContext<T> ParentContext { get; }
		string PropertyName { get; }
		string DisplayName { get; }
		MessageFormatter MessageFormatter { get; }
		T InstanceToValidate { get; }
		TProperty PropertyValue { get; }
		string GetDefaultMessage();
	}

	public class MessageBuilderContext<T,TProperty> : IMessageBuilderContext<T,TProperty> {
		private ValidationContext<T> _innerContext;
		private TProperty _value;

		public MessageBuilderContext(ValidationContext<T> innerContext, TProperty value, RuleComponent<T,TProperty> component) {
			_innerContext = innerContext;
			_value = value;
			Component = component;
		}

		public RuleComponent<T,TProperty> Component { get; }

		IRuleComponent<T, TProperty> IMessageBuilderContext<T, TProperty>.Component => Component;

		public IPropertyValidator PropertyValidator
			=> Component.Validator;

		public ValidationContext<T> ParentContext => _innerContext;

		// public IValidationRule<T> Rule => _innerContext.Rule;

		public string PropertyName => _innerContext.PropertyName;

		public string DisplayName => _innerContext.DisplayName;

		public MessageFormatter MessageFormatter => _innerContext.MessageFormatter;

		public T InstanceToValidate => _innerContext.InstanceToValidate;
		public TProperty PropertyValue => _value;

		public string GetDefaultMessage() {
			return Component.GetErrorMessage(_innerContext, _value);
		}
	}
}
