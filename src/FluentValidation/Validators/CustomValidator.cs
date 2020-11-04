namespace FluentValidation.Validators {
	using System;
	using System.Collections.Generic;
	using System.Threading;
	using System.Threading.Tasks;
	using Internal;
	using Results;
	/// <summary>
	/// Custom validator that allows for manual/direct creation of ValidationFailure instances.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="TProperty"></typeparam>
	public class CustomValidator<T, TProperty> : PropertyValidator {
		private readonly Action<TProperty, CustomContext<T, TProperty>> _action;
		private Func<TProperty, CustomContext<T, TProperty>, CancellationToken, Task> _asyncAction;
		private readonly bool _isAsync;

		/// <summary>
		/// Creates a new instance of the CustomValidator
		/// </summary>
		/// <param name="action"></param>
		public CustomValidator(Action<TProperty, CustomContext<T, TProperty>> action) {
			_isAsync = false;
			_action = action;

			_asyncAction = (x, ctx, cancel) => Task.Run(() => action(x, ctx), cancel);
		}

		/// <summary>
		/// Creates a new instance of the CustomValidator.
		/// </summary>
		/// <param name="asyncAction"></param>
		public CustomValidator(Func<TProperty, CustomContext<T, TProperty>, CancellationToken, Task> asyncAction) {
			_isAsync = true;
			_asyncAction = asyncAction;
			//TODO: For FV 9, throw an exception by default if async validator is being executed synchronously.
			_action = (x, ctx) => Task.Run(() => _asyncAction(x, ctx, new CancellationToken())).GetAwaiter().GetResult();
		}

		public override void Validate(PropertyValidatorContext context) {
			var customContext = new CustomContext<T, TProperty>(context, this);
			_action((TProperty) context.PropertyValue, customContext);
		}

		public override async Task ValidateAsync(PropertyValidatorContext context, CancellationToken cancellation) {
			var customContext = new CustomContext<T, TProperty>(context, this);
			await _asyncAction((TProperty)context.PropertyValue, customContext, cancellation);
		}

		protected override bool IsValid(PropertyValidatorContext context) {
			throw new NotImplementedException();
		}

		public override bool ShouldValidateAsynchronously(IValidationContext context) {
			return _isAsync && context.IsAsync();
		}

		internal new ValidationFailure CreateValidationError(PropertyValidatorContext context, string propertyName, string errorMessage) {
			return base.CreateValidationError(context, propertyName, errorMessage);
		}
	}

	/// <summary>
	/// Custom validation context
	/// </summary>
	public class CustomContext<T, TProperty> {
		private PropertyValidatorContext _context;
		private CustomValidator<T, TProperty> _validator;

		/// <summary>
		/// Creates a new CustomContext
		/// </summary>
		/// <param name="context">The parent PropertyValidatorContext that represents this execution</param>
		/// <param name="validator">The validator being executed</param>
		public CustomContext(PropertyValidatorContext context, CustomValidator<T, TProperty> validator) {
			_context = context;
			_validator = validator;
		}

		/// <summary>
		/// Adds a new validation failure.
		/// </summary>
		/// <param name="propertyName">The property name</param>
		/// <param name="errorMessage">The error message</param>
		public void AddFailure(string propertyName, string errorMessage) {
			errorMessage.Guard("An error message must be specified when calling AddFailure.", nameof(errorMessage));
			errorMessage = _validator.GetErrorMessage(_context, errorMessage);
			var failure = _validator.CreateValidationError(_context, propertyName ?? string.Empty, errorMessage);
			AddFailure(failure);
		}

		/// <summary>
		/// Adds a new validation failure (the property name is inferred)
		/// </summary>
		/// <param name="errorMessage">The error message</param>
		public void AddFailure(string errorMessage) {
			errorMessage.Guard("An error message must be specified when calling AddFailure.", nameof(errorMessage));
			AddFailure(_context.PropertyName, errorMessage);
		}

		/// <summary>
		/// Adds a new validation failure
		/// </summary>
		/// <param name="failure">The failure to add</param>
		public void AddFailure(ValidationFailure failure) {
			_context.AddFailure(failure);
		}

		public PropertyRule Rule => _context.Rule;
		public string PropertyName => _context.PropertyName;
		public string DisplayName => _context.DisplayName;
		public MessageFormatter MessageFormatter => _context.MessageFormatter;
		public T InstanceToValidate => (T)_context.InstanceToValidate;
		public TProperty PropertyValue => (TProperty)_context.PropertyValue;
		public IValidationContext ParentContext => _context.ParentContext;
	}
}
