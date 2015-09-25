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

namespace FluentValidation.Validators {
	using System;
	using System.Text.RegularExpressions;
	using Attributes;
	using Internal;
	using Resources;
	using Results;

	public class RegularExpressionValidator : PropertyValidator, IRegularExpressionValidator {
		string expression;
		readonly RegexOptions? regexOptions;
		
		readonly Func<object, string> expressionFunc;
		readonly Func<object, Regex> regexFunc;

		public RegularExpressionValidator(string expression) : base(() => Messages.regex_error) {
			this.expression = expression;
		}

		public RegularExpressionValidator(Regex regex) : base(() => Messages.regex_error) {
			this.expression = regex.ToString();
			this.regexFunc = x => regex;
		}

		public RegularExpressionValidator(string expression, RegexOptions options) : base(() => Messages.regex_error) {
			this.expression = expression;
			this.regexOptions = options;
		}

		public RegularExpressionValidator(Func<object, string> expression)
			: base(() => Messages.regex_error)
		{
			this.expressionFunc = expression;
		}

		public RegularExpressionValidator(Func<object, Regex> regex)
			: base(() => Messages.regex_error)
		{
			this.regexFunc = regex;
		}

		public RegularExpressionValidator(Func<object, string> expression, RegexOptions options)
			: base(() => Messages.regex_error)
		{
			this.expressionFunc = expression;
			this.regexOptions = options;
		}

		protected override bool IsValid(PropertyValidatorContext context) {
			Regex regex = null;

			if (regexOptions.HasValue)
			{
				if (regexFunc != null)
				{
					Regex regexOrig = regexFunc(context.Instance);
					expression = regexOrig.ToString();
					regex = new Regex(regexOrig.ToString(), regexOptions.Value);
				}
				else if (expressionFunc != null)
				{
					expression = expressionFunc(context.Instance);
					regex = new Regex(expression, regexOptions.Value);
				}
				else
				{
					regex = new Regex(expression, regexOptions.Value);
				}
			}
			else
			{
				if (regexFunc != null)
				{
					regex = regexFunc(context.Instance);
					expression = regex.ToString();
				}
				else if (expressionFunc != null)
				{
					expression = expressionFunc(context.Instance);
					regex = new Regex(expression);
				}
				else
				{
					regex = new Regex(expression);
				}
			}

			if (context.PropertyValue != null && !regex.IsMatch((string)context.PropertyValue)) {
				context.MessageFormatter.AppendArgument("RegularExpression", regex.ToString());
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