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
		readonly Func<object, Regex> regexFunc;

		public RegularExpressionValidator(string expression) :base(new LanguageStringSource(nameof(RegularExpressionValidator))) {
			this.Expression = expression;

			var regex = CreateRegex(expression);
			this.regexFunc = x => regex;
		}

		public RegularExpressionValidator(Regex regex) : base(new LanguageStringSource(nameof(RegularExpressionValidator))) {
			this.Expression = regex.ToString();
			this.regexFunc = x => regex;
		}

		public RegularExpressionValidator(string expression, RegexOptions options) : base(new LanguageStringSource(nameof(RegularExpressionValidator))) {
			this.Expression = expression;
			var regex = CreateRegex(expression, options);
			this.regexFunc = x => regex;
		}

		public RegularExpressionValidator(Func<object, string> expressionFunc) : base(new LanguageStringSource(nameof(RegularExpressionValidator))) {
			this.regexFunc = x => CreateRegex(expressionFunc(x));
		}

		public RegularExpressionValidator(Func<object, Regex> regexFunc) : base(new LanguageStringSource(nameof(RegularExpressionValidator))) {
			this.regexFunc = regexFunc;
		}

		public RegularExpressionValidator(Func<object, string> expression, RegexOptions options) : base(new LanguageStringSource(nameof(RegularExpressionValidator))) {

			this.regexFunc = x => CreateRegex(expression(x), options);
		}

		protected override bool IsValid(PropertyValidatorContext context) {
			var regex = regexFunc(context.Instance);
			
			if (regex != null && context.PropertyValue != null && !regex.IsMatch((string) context.PropertyValue)) {
				context.MessageFormatter.AppendArgument("RegularExpression", regex.ToString());
				return false;
			}
			return true;
		}

		private static Regex CreateRegex(string expression, RegexOptions options=RegexOptions.None) {
			// Workaround for CVE-2015-2526
			// If no REGEX_DEFAULT_MATCH_TIMEOUT is specified in the AppDomain, default to 2 seconds. 
			// if we're on Netstandard 1.0 we don't have access to AppDomain, so just always use 2 second timeout there. 

#if NETSTANDARD1_0
			return new Regex(expression, options, TimeSpan.FromSeconds(2.0));
#else
			try {
				if (AppDomain.CurrentDomain.GetData("REGEX_DEFAULT_MATCH_TIMEOUT") == null) {
					return new Regex(expression, options, TimeSpan.FromSeconds(2.0));
				}
			}
			catch
			{
			}

			return new Regex(expression, options);
#endif
		}

		public string Expression { get; }
	}

	public interface IRegularExpressionValidator : IPropertyValidator {
		string Expression { get; }
	}
}