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
	using Attributes;
	using Internal;
	using Resources;
	using Results;

	[ValidationMessage(Key = DefaultResourceManager.ExclusiveBetweenValidatorError)]
	public class ExclusiveBetweenValidator<TInstance, TProperty> : IPropertyValidator<TInstance, TProperty>, IBetweenValidator<TProperty> where TProperty : IComparable<TProperty>, IComparable {
		public ExclusiveBetweenValidator(TProperty from, TProperty to) {
			To = to;
			From = from;

			if (to.CompareTo(from) == -1) {
				throw new ArgumentOutOfRangeException("to", "To should be larger than from.");
			}
		}

		public TProperty From { get; private set; }
		public TProperty To { get; private set; }

		public PropertyValidatorResult Validate(PropertyValidatorContext<TInstance, TProperty> context) {
			var propertyValue = (IComparable)context.PropertyValue;

			if (propertyValue.CompareTo(From) <= 0 || propertyValue.CompareTo(To) >= 0) {

				var formatter = new MessageFormatter()
					.AppendProperyName(context.PropertyDescription)
					.AppendArgument("From", From)
					.AppendArgument("To", To)
					.AppendArgument("Value", context.PropertyValue);

				string error = context.GetFormattedErrorMessage(GetType(), formatter);
				return PropertyValidatorResult.Failure(error);
			}
			return PropertyValidatorResult.Success();
		}
	}
}