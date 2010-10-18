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
	using System.Collections;
	using System.Reflection;
	using Attributes;
	using Internal;
	using Resources;

	public class NotEqualValidator : PropertyValidator, IComparisonValidator {
		readonly IEqualityComparer comparer;
		readonly PropertySelector func;

		public NotEqualValidator(PropertySelector func, MemberInfo memberToCompare) : base(() => Messages.notequal_error) {
			this.func = func;
			MemberToCompare = memberToCompare;
		}

		public NotEqualValidator(PropertySelector func, MemberInfo memberToCompare, IEqualityComparer equalityComparer)
			: base(() => Messages.notequal_error) {
			this.func = func;
			this.comparer = equalityComparer;
			MemberToCompare = memberToCompare;
		}

		public NotEqualValidator(object comparisonValue)
			: base(() => Messages.notequal_error) {
			ValueToCompare = comparisonValue;
			SupportsStandaloneValidation = true;
		}

		public NotEqualValidator(object comparisonValue, IEqualityComparer equalityComparer)
			: base(() => Messages.notequal_error) {
			ValueToCompare = comparisonValue;
			comparer = equalityComparer;
			SupportsStandaloneValidation = true;
		}

		protected override bool IsValid(PropertyValidatorContext context) {
			var comparisonValue = GetComparisonValue(context);
			bool success = !Compare(comparisonValue, context.PropertyValue);

			if (!success) {
				context.MessageFormatter.AppendArgument("PropertyValue", context.PropertyValue);
				return false;
			}

			return true;
		}

		private object GetComparisonValue(PropertyValidatorContext context) {
			if (func != null) {
				return func(context.Instance);
			}

			return ValueToCompare;
		}

		public Comparison Comparison {
			get { return Comparison.NotEqual; }
		}

		public MemberInfo MemberToCompare { get; private set; }
		public object ValueToCompare { get; private set; }

		protected bool Compare(object comparisonValue, object propertyValue) {
			if(comparer != null) {
				return comparer.Equals(comparisonValue, propertyValue);
			}
			return Equals(comparisonValue, propertyValue);
		}
	}
}