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
	using System.Text.RegularExpressions;
	using Attributes;
	using Internal;
	using Resources;
	using Results;

	//Email regex from http://hexillion.com/samples/#Regex
	[ValidationMessage(Key=DefaultResourceManager.Email)]
	public class EmailValidator<TInstance> : IPropertyValidator, IRegularExpressionValidator, IEmailValidator {
		private readonly Regex regex;
		const string expression = @"^(?:[\w\!\#\$\%\&\'\*\+\-\/\=\?\^\`\{\|\}\~]+\.)*[\w\!\#\$\%\&\'\*\+\-\/\=\?\^\`\{\|\}\~]+@(?:(?:(?:[a-zA-Z0-9](?:[a-zA-Z0-9\-](?!\.)){0,61}[a-zA-Z0-9]?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9\-](?!$)){0,61}[a-zA-Z0-9]?)|(?:\[(?:(?:[01]?\d{1,2}|2[0-4]\d|25[0-5])\.){3}(?:[01]?\d{1,2}|2[0-4]\d|25[0-5])\]))$";

		public EmailValidator() {
			regex = new Regex(expression, RegexOptions.IgnoreCase);
		}

		public PropertyValidatorResult Validate(PropertyValidatorContext context) {
			if (context.PropertyValue == null) return PropertyValidatorResult.Success();

			if (!regex.IsMatch((string)context.PropertyValue)) {
				var formatter = new MessageFormatter().AppendProperyName(context.PropertyDescription);
				string error = context.GetFormattedErrorMessage(typeof(EmailValidator<TInstance>), formatter);
				return PropertyValidatorResult.Failure(error);
			}
			return PropertyValidatorResult.Success();
		}

		public string Expression {
			get { return expression; }
		}
	}

	public interface IEmailValidator : IRegularExpressionValidator {
		
	}
}