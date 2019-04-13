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
	public class CustomValidator<T> : PropertyValidator {
		private readonly Action<T, CustomContext> _action;
		private Func<T, CustomContext, CancellationToken, Task> _asyncAction;
		private readonly bool _isAsync;

		/// <summary>
		/// Creates a new instance of the CustomValidator
		/// </summary>
		/// <param name="action"></param>
		public CustomValidator(Action<T, CustomContext> action) : base(string.Empty) {
			_isAsync = false;
			_action = action;

			_asyncAction = (x, ctx, cancel) => Task.Run(() => action(x, ctx), cancel);
		}

		/// <summary>
		/// Creates a new instance of the CustomValidator.
		/// </summary>
		/// <param name="asyncAction"></param>
		public CustomValidator(Func<T, CustomContext, CancellationToken, Task> asyncAction) : base(string.Empty) {
			_isAsync = true;
			_asyncAction = asyncAction;
			//TODO: For FV 9, throw an exception by default if async validator is being executed synchronously.
			_action = (x, ctx) => Task.Run(() => _asyncAction(x, ctx, new CancellationToken())).GetAwaiter().GetResult();
		}

		public override IEnumerable<ValidationFailure> Validate(PropertyValidatorContext context) {
			var customContext = new CustomContext(context);
			_action((T) context.PropertyValue, customContext);
			return customContext.Failures;
		}

		public override async Task<IEnumerable<ValidationFailure>> ValidateAsync(PropertyValidatorContext context, CancellationToken cancellation) {
			var customContext = new CustomContext(context);
			await _asyncAction((T)context.PropertyValue, customContext, cancellation);
			return customContext.Failures;
		}

		protected override bool IsValid(PropertyValidatorContext context) {
			throw new NotImplementedException();
		}

		public override bool ShouldValidateAsync(ValidationContext context) {
			return _isAsync && context.IsAsync();
		}
	}

	/// <summary>
	/// Custom validation context
	/// </summary>
	public class CustomContext : IValidationContext {
		private PropertyValidatorContext _context;
		private List<ValidationFailure> _failures = new List<ValidationFailure>();

		/// <summary>
		/// Creates a new CustomContext
		/// </summary>
		/// <param name="context">The parent PropertyValidatorContext that represents this execution</param>
		public CustomContext(PropertyValidatorContext context) {
			_context = context;
		}

		/// <summary>
		/// Adds a new validation failure.
		/// </summary>
		/// <param name="propertyName">The property name</param>
		/// <param name="errorMessage">The error message</param>
		public void AddFailure(string propertyName, string errorMessage) {
			errorMessage.Guard("An error message must be specified when calling AddFailure.", nameof(errorMessage));
			AddFailure(new ValidationFailure(propertyName ?? string.Empty, errorMessage));
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
			failure.Guard("A failure must be specified when calling AddFailure.", nameof(failure));
			_failures.Add(failure);
		}

		internal IEnumerable<ValidationFailure> Failures => _failures;

		public PropertyRule Rule => _context.Rule;
		public string PropertyName => _context.PropertyName;
		public string DisplayName => _context.DisplayName;
		public MessageFormatter MessageFormatter => _context.MessageFormatter;
		public object InstanceToValidate => _context.InstanceToValidate;
		public object PropertyValue => _context.PropertyValue;
		IValidationContext IValidationContext.ParentContext => ParentContext;
		public ValidationContext ParentContext => _context.ParentContext;
	}
}