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

	public abstract class PropertyValidator : IPropertyValidator, IHasMetadata {
		public ValidatorMetadata Metadata { get; } = new ValidatorMetadata();

		protected PropertyValidator(IStringSource errorMessageSource) {
			if(errorMessageSource == null) errorMessageSource = new StaticStringSource("No default error message has been specified.");
			Metadata.ErrorMessageSource = errorMessageSource;
		}

		protected PropertyValidator(string errorMessageResourceName, Type errorMessageResourceType) {
			errorMessageResourceName.Guard("errorMessageResourceName must be specified.", nameof(errorMessageResourceName));
			errorMessageResourceType.Guard("errorMessageResourceType must be specified.", nameof(errorMessageResourceType));

			Metadata.ErrorMessageSource = new LocalizedStringSource(errorMessageResourceType, errorMessageResourceName);
		}

		protected PropertyValidator(string errorMessage) {
			Metadata.ErrorMessageSource = new StaticStringSource(errorMessage);
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
			var messageBuilderContext = new MessageBuilderContext(context, Metadata.ErrorMessageSource, this);

			var error = context.Rule.MessageBuilder != null 
				? context.Rule.MessageBuilder(messageBuilderContext) 
				: messageBuilderContext.GetDefaultMessage();

			var failure = new ValidationFailure(context.PropertyName, error, context.PropertyValue);
			failure.FormattedMessageArguments = context.MessageFormatter.AdditionalArguments;
			failure.FormattedMessagePlaceholderValues = context.MessageFormatter.PlaceholderValues;
			failure.ResourceName = Metadata.ErrorMessageSource.ResourceName;
			failure.ErrorCode = (Metadata.ErrorCodeSource != null)
				? Metadata.ErrorCodeSource.GetString(context)
				: ValidatorOptions.ErrorCodeResolver(this);

			if (Metadata.CustomStateProvider != null) {
				failure.CustomState = Metadata.CustomStateProvider(context);
			}

			failure.Severity = Metadata.Severity;
			return failure;
		}

		[Obsolete("Use IShouldValidateAsync.ShouldValidatAsync(context) instead")]
		public virtual bool IsAsync {
			get { return false; }
		}

		[Obsolete("Use Metadata.CustomStateProvider")]
		public Func<PropertyValidatorContext, object> CustomStateProvider {
			get => Metadata.CustomStateProvider;
			set => Metadata.CustomStateProvider = value;
		}

		[Obsolete("Use Metadata.Severity")]
		public Severity Severity {
			get => Metadata.Severity;
			set => Metadata.Severity = value;
		}

		[Obsolete("Use Metadata.ErrorCodeSource")]
		public IStringSource ErrorCodeSource {
			get => Metadata.ErrorCodeSource;
			set => Metadata.ErrorCodeSource = value;
		}

		[Obsolete("Use Metadata.ErrorMessageSource")]
		public IStringSource ErrorMessageSource {
			get => Metadata.ErrorMessageSource;
			set => Metadata.ErrorMessageSource = value;
		}
	}
}