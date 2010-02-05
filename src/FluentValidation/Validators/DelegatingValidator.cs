#region License
// Copyright 2008-2009 Jeremy Skinner (http://www.jeremyskinner.co.uk)
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
	using Results;

	public class DelegatingValidator : IPropertyValidator, IDelegatingValidator {
		private readonly Predicate<object> condition;
		public IPropertyValidator InnerValidator { get; private set; }

		public DelegatingValidator(Predicate<object> condition, IPropertyValidator innerValidator) {
			this.condition = condition;
			InnerValidator = innerValidator;
		}

		public IEnumerable<ValidationFailure> Validate(PropertyValidatorContext context) {
			if (condition(context.Instance)) {
				return InnerValidator.Validate(context);
			}
			return Enumerable.Empty<ValidationFailure>();
		}

		public string ErrorMessageTemplate {
			get { return InnerValidator.ErrorMessageTemplate; }
		}

		public ICollection<Func<object, object>> CustomMessageFormatArguments {
			get { return InnerValidator.CustomMessageFormatArguments; }
		}

		public bool SupportsStandaloneValidation {
			get { return false; }
		}

		public Type ErrorMessageResourceType {
			get { return InnerValidator.ErrorMessageResourceType; }
		}

		public string ErrorMessageResourceName {
			get { return InnerValidator.ErrorMessageResourceName; }
		}

		public Func<object, object> CustomStateProvider {
			get { return InnerValidator.CustomStateProvider; }
			set { InnerValidator.CustomStateProvider = value; }
		}

		public void SetErrorMessage(string message) {
			InnerValidator.SetErrorMessage(message);
		}

		public void SetErrorMessage(Type errorMessageResourceType, string resourceName) {
			InnerValidator.SetErrorMessage(errorMessageResourceType, resourceName);
		}

		public void SetErrorMessage(Expression<Func<string>> resourceSelector) {
			InnerValidator.SetErrorMessage(resourceSelector);
		}

		IPropertyValidator IDelegatingValidator.InnerValidator {
			get { return InnerValidator; }
		}
	}

	public interface IDelegatingValidator : IPropertyValidator {
		IPropertyValidator InnerValidator { get; }
	}
}