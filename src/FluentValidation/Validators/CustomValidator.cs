namespace FluentValidation.Validators {
	using System;
	using System.Collections.Generic;
	using System.Linq;
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

		public override bool IsAsync { get; }

		/// <summary>
		/// Creates a new instance of the CustomValidator
		/// </summary>
		/// <param name="action"></param>
		public CustomValidator(Action<T, CustomContext> action) : base(string.Empty) {
			IsAsync = false;
			_action = action;
			_asyncAction = (x, ctx, cancel) => TaskHelpers.RunSynchronously(() =>_action(x, ctx), cancel);
		}

		/// <summary>
		/// Creates a new isntance of the CutomValidator.
		/// </summary>
		/// <param name="asyncAction"></param>
		public CustomValidator(Func<T, CustomContext, CancellationToken, Task> asyncAction) : base(string.Empty) {
			IsAsync = true;
			_asyncAction = asyncAction;
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
	}

	/// <summary>
	/// Custom validation context
	/// </summary>
	public class CustomContext {
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
		/// <param name="errorMessage">The error mesage</param>
		public void AddFailure(string propertyName, string errorMessage) {
			errorMessage.Guard("An error message must be specified when calling AddFailure.");
			AddFailure(new ValidationFailure(propertyName ?? string.Empty, errorMessage));
		}

		/// <summary>
		/// Adds a new validation failure (the property name is inferred) 
		/// </summary>
		/// <param name="errorMessage">The error message</param>
		public void AddFailure(string errorMessage) {
			errorMessage.Guard("An error message must be specified when calling AddFailure.");
			AddFailure(_context.PropertyName, errorMessage);
		}

		/// <summary>
		/// Adss a new validation failure
		/// </summary>
		/// <param name="failure">The failure to add</param>
		public void AddFailure(ValidationFailure failure) {
			failure.Guard("A failure must be specified when calling AddFailure.");
			_failures.Add(failure);
		}

		internal IEnumerable<ValidationFailure> Failures => _failures;

		public PropertyRule Rule => _context.Rule;
		public string PropertyName => _context.PropertyName;
		public string DisplayName => _context.DisplayName;
		public MessageFormatter MessageFormatter => _context.MessageFormatter;
		public ValidationContext ParentContext => _context.ParentContext;
	}
}