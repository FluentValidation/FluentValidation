namespace FluentValidation.Internal {
	using System;
	using System.Linq.Expressions;
	using System.Reflection;

	public class PropertyModel<T, TProperty> {
		string propertyName;

		public MemberInfo Member { get; private set; }
		public Func<T, TProperty> PropertyFunc { get; private set; }
		public Expression Expression { get; private set; }
		public string CustomPropertyName { get; set; }
		public Action<T> OnFailure { get; set; }

		public PropertyModel(MemberInfo member, Func<T, TProperty> propertyFunc, Expression expression) {
			Member = member;
			PropertyFunc = propertyFunc;
			Expression = expression;
			OnFailure = x => { };

			PropertyName = ValidatorOptions.PropertyNameResolver(typeof(T), member);
		}

		/// <summary>
		/// Returns the property name for the property being validated.
		/// Returns null if it is not a property being validated (eg a method call)
		/// </summary>
		public string PropertyName { get; set; }

		public string PropertyDescription {
			get { return CustomPropertyName ?? PropertyName.SplitPascalCase(); }
		}

	}
}