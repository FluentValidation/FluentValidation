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
// The latest version of this file can be found at http://www.codeplex.com/FluentValidation
#endregion

namespace FluentValidation.Validators {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using Resources;
	using Results;

	public abstract class PropertyValidator : IPropertyValidator {
		private readonly List<Func<object, object, object>> customFormatArgs = new List<Func<object, object, object>>();
		private IStringSource errorSource;

		public Func<object, object> CustomStateProvider { get; set; }

		public ICollection<Func<object, object, object>> CustomMessageFormatArguments {
			get { return customFormatArgs; }
		}

		protected PropertyValidator(string errorMessageResourceName, Type errorMessageResourceType) {
			errorSource = new LocalizedStringSource(errorMessageResourceType, errorMessageResourceName, new FallbackAwareResourceAccessorBuilder());
		}

		protected PropertyValidator(string errorMessage) {
			errorSource = new StaticStringSource(errorMessage);
		}

		protected PropertyValidator(Expression<Func<string>> errorMessageResourceSelector) {
			errorSource = LocalizedStringSource.CreateFromExpression(errorMessageResourceSelector, new FallbackAwareResourceAccessorBuilder());
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
			context.MessageFormatter.AppendPropertyName(context.PropertyDescription);
			context.MessageFormatter.AppendArgument("PropertyValue", context.PropertyValue);

			if (!IsValid(context)) {
				return new[] { CreateValidationError(context) };
			}

			return Enumerable.Empty<ValidationFailure>();
		}

		protected abstract bool IsValid(PropertyValidatorContext context);

		/// <summary>
		/// Creates an error validation result for this validator.
		/// </summary>
		/// <param name="context">The validator context</param>
		/// <returns>Returns an error validation result.</returns>
		protected virtual ValidationFailure CreateValidationError(PropertyValidatorContext context) {
			Func<PropertyValidatorContext, string> errorBuilder = context.Rule.MessageBuilder ?? BuildErrorMessage;
			var error = errorBuilder(context);

			var failure = new ValidationFailure(context.PropertyName, error, context.PropertyValue);

			if (CustomStateProvider != null) {
				failure.CustomState = CustomStateProvider(context.Instance);
			}

			return failure;
		}

		string BuildErrorMessage(PropertyValidatorContext context) {
			context.MessageFormatter.AppendAdditionalArguments(
				customFormatArgs.Select(func => func(context.Instance, context.PropertyValue)).ToArray()
				);

			string error = context.MessageFormatter.BuildMessage(errorSource.GetString());
			return error;
		}
	}
}