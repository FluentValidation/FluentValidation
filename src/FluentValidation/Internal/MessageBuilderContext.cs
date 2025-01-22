namespace FluentValidation.Internal;

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

public class MessageBuilderContext<T, TProperty>(ValidationContext<T> innerContext, TProperty value, RuleComponent<T, TProperty> component)
	: IMessageBuilderContext<T, TProperty> {
	public RuleComponent<T,TProperty> Component { get; } = component;

	IRuleComponent<T, TProperty> IMessageBuilderContext<T, TProperty>.Component => Component;

	public IPropertyValidator PropertyValidator
		=> Component.Validator;

	public ValidationContext<T> ParentContext => innerContext;

	// public IValidationRule<T> Rule => _innerContext.Rule;

	public string PropertyName => innerContext.PropertyPath;

	public string DisplayName => innerContext.DisplayName;

	public MessageFormatter MessageFormatter => innerContext.MessageFormatter;

	public T InstanceToValidate => innerContext.InstanceToValidate;
	public TProperty PropertyValue => value;

	public string GetDefaultMessage() {
		return Component.GetErrorMessage(innerContext, value);
	}
}
