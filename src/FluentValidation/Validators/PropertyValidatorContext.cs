#region License
// Copyright (c) .NET Foundation and contributors.
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
// The latest version of this file can be found at https://github.com/FluentValidation/FluentValidation
#endregion

namespace FluentValidation.Validators {
	using System;
	using System.Collections.Generic;
	using Internal;
	using Results;

	public class PropertyValidatorContext<T, TProperty> {
		private TProperty _propertyValue;
		private Lazy<TProperty> _propertyValueAccessor;

		public ValidationContext<T> ParentContext { get; }

		internal IValidationRule<T, TProperty> Rule { get; }
		public string PropertyName { get; }

		public string DisplayName => Rule.GetDisplayName(ParentContext);

		public T InstanceToValidate => ParentContext.InstanceToValidate;
		public MessageFormatter MessageFormatter => ParentContext.Formatter;

		//Lazily load the property value
		//to allow the delegating validator to cancel validation before value is obtained
		public TProperty PropertyValue
			=> _propertyValueAccessor != null ? _propertyValueAccessor.Value : _propertyValue;

		public PropertyValidatorContext(ValidationContext<T> parentContext, IValidationRule<T, TProperty> rule, string propertyName, TProperty propertyValue) {
			ParentContext = parentContext;
			Rule = rule;
			PropertyName = propertyName;
			_propertyValue = propertyValue;
		}

		public PropertyValidatorContext(ValidationContext<T> parentContext, IValidationRule<T, TProperty> rule, string propertyName, Lazy<TProperty> propertyValueAccessor) {
			ParentContext = parentContext;
			Rule = rule;
			PropertyName = propertyName;
			_propertyValueAccessor = propertyValueAccessor;
		}

		/// <summary>
		/// Adds a new validation failure.
		/// </summary>
		/// <param name="failure">The failure to add.</param>
		/// <exception cref="ArgumentNullException"></exception>
		public void AddFailure(ValidationFailure failure) {
			if (failure == null) throw new ArgumentNullException(nameof(failure), "A failure must be specified when calling AddFailure");
			ParentContext.Failures.Add(failure);
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
			AddFailure(PropertyName, errorMessage);
		}
	}
}
