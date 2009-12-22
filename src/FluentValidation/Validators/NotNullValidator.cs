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
	using Attributes;
	using Internal;
	using Resources;
	using Results;

	[ValidationMessage(Key=DefaultResourceManager.NotNull)]
	public class NotNullValidator<TInstance, TProperty> : IPropertyValidator, INotNullValidator {
		public PropertyValidatorResult Validate(PropertyValidatorContext context) {
			if (context.PropertyValue == null) {
				var formatter = new MessageFormatter().AppendProperyName(context.PropertyDescription);
				string error = context.GetFormattedErrorMessage(typeof(NotNullValidator<TInstance, TProperty>), formatter);
				return PropertyValidatorResult.Failure(error);
			}
			return PropertyValidatorResult.Success();
		}
	}

	public interface INotNullValidator : IPropertyValidator {
	}
}