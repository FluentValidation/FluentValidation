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
		private List<Func<object, object, object>> customFormatArgs;
		private IStringSource errorSource;
		private IStringSource originalErrorSource;
		private IStringSource errorCodeSource;
		private IStringSource originalErrorCodeSource;

		public virtual bool IsAsync {
			get { return false; }
		}

		public Func<object, object> CustomStateProvider { get; set; }

		public Severity Severity { get; set; }

		public ICollection<Func<object, object, object>> CustomMessageFormatArguments {
			get { return customFormatArgs ?? (customFormatArgs = new List<Func<object, object, object>>()); }
		}

		protected PropertyValidator(IStringSource errorMessageSource) {
			originalErrorSource = errorSource = errorMessageSource;
		}

		protected PropertyValidator(string errorMessageResourceName, Type errorMessageResourceType) {
			originalErrorSource = errorSource = new LocalizedStringSource(errorMessageResourceType, errorMessageResourceName);
		}

		protected PropertyValidator(string errorMessage) {
			originalErrorSource = errorSource = new StaticStringSource(errorMessage);
		}

		[Obsolete("Use the constructor that takes a Type resourceType and string resourceName")]
		protected PropertyValidator(Expression<Func<string>> errorMessageResourceSelector) {
			originalErrorSource = errorSource = OverridableLocalizedStringSource.CreateFromExpression(errorMessageResourceSelector);
		}

		public IStringSource ErrorMessageSource {
			get { return errorSource; }
			set {
				if (value == null) {
					throw new ArgumentNullException("value");
				}

				errorSource = value;

				if (value is LanguageStringSource) {
					originalErrorSource = value;
				}
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

		protected virtual async Task<bool> IsValidAsync(PropertyValidatorContext context, CancellationToken cancellation) {
			return IsValid(context);
		}

		/// <summary>
		/// Prepares the <see cref="MessageFormatter"/> of <paramref name="context"/> for an upcoming <see cref="ValidationFailure"/>.
		/// </summary>
		/// <param name="context">The validator context</param>
		protected virtual void PrepareMessageFormatterForValidationError(PropertyValidatorContext context) {
			context.MessageFormatter.AppendPropertyName(context.PropertyDescription);
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
				failure.CustomState = CustomStateProvider(context.Instance);
			}

			failure.Severity = Severity;
			return failure;
		}

		string BuildErrorMessage(PropertyValidatorContext context) {
			// Performance: If we got no args we can skip adding nothing to the MessageFormatter.
			if (this.customFormatArgs != null &&
				this.customFormatArgs.Count > 0) {
				var additionalArguments = customFormatArgs.Select(func => func(context.Instance, context.PropertyValue)).ToArray();
				context.MessageFormatter.AppendAdditionalArguments(additionalArguments);
			}

			string error = context.MessageFormatter.BuildMessage(errorSource.GetString(context.Instance));
			return error;
		}

		public IStringSource ErrorCodeSource {
			get { return errorCodeSource; }
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}

				errorCodeSource = value;
			}
		}
	}
}