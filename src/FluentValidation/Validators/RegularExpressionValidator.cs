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
	using System.Text.RegularExpressions;
	using Attributes;
	using Internal;
	using Resources;
	using Results;

	public class RegularExpressionValidator : PropertyValidator, IRegularExpressionValidator {
		readonly string expression;
		readonly Regex regex;

		public RegularExpressionValidator(string expression) : base(() => Messages.regex_error) {
			this.expression = expression;
			regex = new Regex(expression);

		}

		public RegularExpressionValidator(Regex regex) : base(() => Messages.regex_error) {
			this.expression = regex.ToString();
			this.regex = regex;
		}

		public RegularExpressionValidator(string expression, RegexOptions options) : base(() => Messages.regex_error) {
			this.expression = expression;
			this.regex = new Regex(expression, options);
		}

		protected override bool IsValid(PropertyValidatorContext context) {
			if (context.PropertyValue != null && !regex.IsMatch((string)context.PropertyValue)) {
				return false;
			}
			return true;
		}

		public string Expression {
			get { return expression; }
		}
	}

	public interface IRegularExpressionValidator : IPropertyValidator {
		string Expression { get; }
	}
}