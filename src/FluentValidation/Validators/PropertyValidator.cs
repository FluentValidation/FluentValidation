#region License
// Copyright (c) Jeremy Skinner (http://www.jeremyskinner.co.uk)
// 
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at 
// 
// http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and 
// limitations under the License.
// 
// The latest version of this file can be found at https://github.com/jeremyskinner/FluentValidation
#endregion

using System.Threading;

namespace FluentValidation.Validators {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Threading.Tasks;
	using FluentValidation.Internal;
	using Resources;
	using Results;

	public abstract class PropertyValidator : IPropertyValidator {
		private IStringSource errorSource;
		private IStringSource errorCodeSource;

		public virtual bool IsAsync {
			get { return false; }
		}

		public Func<PropertyValidatorContext, object> CustomStateProvider { get; set; }

		public Severity Severity { get; set; }

		protected PropertyValidator(IStringSource errorMessageSource) {
			if(errorMessageSource == null) errorMessageSource = new StaticStringSource("No default error message has been specified.");
			errorSource = errorMessageSource;
		}

		protected PropertyValidator(string errorMessageResourceName, Type errorMessageResourceType) {
			errorMessageResourceName.Guard("errorMessageResourceName must be specified.");
			errorMessageResourceType.Guard("errorMessageResourceType must be specified.");

			errorSource = new LocalizedStringSource(errorMessageResourceType, errorMessageResourceName);
		}

		protected PropertyValidator(string errorMessage) {
			errorSource = new StaticStringSource(errorMessage);
		}

		[Obsolete("Use the constructor that takes a Type resourceType and string resourceName")]
		protected PropertyValidator(Expression<Func<string>> errorMessageResourceSelector) {
			errorSource = OverridableLocalizedStringSource.CreateFromExpression(errorMessageResourceSelector);
		}

		public IStringSource ErrorMessageSource {
			get { return errorSource; }
			set {
				if (value == null) {
					throw new ArgumentNullException("value");
				}

				errorSource = value;

				
			}
		}

		public virtual IEnumerable<ValidationFailure> Validate(PropertyValidatorContext context) {
			if (!IsValid(context)) {
				PrepareMessageFormatterForValidationError(context);
				return new[] { CreateValidationError(context) };
			}

			return Enumerable.Empty<ValidationFailure>();
		}

		public virtual Task<IEnumerable<ValidationFailure>> ValidateAsync(PropertyValidatorContext context, CancellationToken cancellation) {
			return
				IsValidAsync(context, cancellation)
				.Then(valid => {
					    if (valid) {
						    return Enumerable.Empty<ValidationFailure>();
					    }

						PrepareMessageFormatterForValidationError(context);
						return new[] { CreateValidationError(context) }.AsEnumerable();
				      },
					runSynchronously: true
				);
		}

		protected abstract bool IsValid(PropertyValidatorContext context);

#pragma warning disable 1998
		protected virtual async Task<bool> IsValidAsync(PropertyValidatorContext context, CancellationToken cancellation) {
			return IsValid(context);
		}
#pragma warning restore 1998

		/// <summary>
		/// Prepares the <see cref="MessageFormatter"/> of <paramref name="context"/> for an upcoming <see cref="ValidationFailure"/>.
		/// </summary>
		/// <param name="context">The validator context</param>
		protected virtual void PrepareMessageFormatterForValidationError(PropertyValidatorContext context) {
			context.MessageFormatter.AppendPropertyName(context.DisplayName);
			context.MessageFormatter.AppendPropertyValue(context.PropertyValue);
		}

		/// <summary>
		/// Creates an error validation result for this validator.
		/// </summary>
		/// <param name="context">The validator context</param>
		/// <returns>Returns an error validation result.</returns>
		protected virtual ValidationFailure CreateValidationError(PropertyValidatorContext context) {
			Func<PropertyValidatorContext, string> errorBuilder = context.Rule.MessageBuilder ?? BuildErrorMessage;
			var error = errorBuilder(context);

			var failure = new ValidationFailure(context.PropertyName, error, context.PropertyValue);
			failure.FormattedMessageArguments = context.MessageFormatter.AdditionalArguments;
			failure.FormattedMessagePlaceholderValues = context.MessageFormatter.PlaceholderValues;
			failure.ResourceName = errorSource.ResourceName;
			failure.ErrorCode = (errorCodeSource != null)
				? errorCodeSource.GetString(context.Instance)
				: GetType().Name;

			if (CustomStateProvider != null) {
				failure.CustomState = CustomStateProvider(context);
			}

			failure.Severity = Severity;
			return failure;
		}

		string BuildErrorMessage(PropertyValidatorContext context) {
			// For backwards compatibility, only pass in the PropertyValidatorContext if the string source implements IContextAwareStringSource
			// otherwise fall back to old behaviour of passing the instance. 
			object stringSourceContext = errorSource is IContextAwareStringSource ? context : context.Instance;

			string error = context.MessageFormatter.BuildMessage(errorSource.GetString(stringSourceContext));
			return error;
		}

		public IStringSource ErrorCodeSource {
			get => errorCodeSource;
			set => errorCodeSource = value ?? throw new ArgumentNullException(nameof(value));
		}
	}
}