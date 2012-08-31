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
	using System.Collections;
	using System.Reflection;
	using Attributes;
	using Internal;
	using Resources;

	public class EqualValidator : PropertyValidator, IComparisonValidator {
		readonly Func<object, object> func;
		readonly IEqualityComparer comparer;

		public EqualValidator(object valueToCompare) : base(() => Messages.equal_error) {
			this.ValueToCompare = valueToCompare;
		}

		public EqualValidator(object valueToCompare, IEqualityComparer comparer)
			: base(() => Messages.equal_error) {
			ValueToCompare = valueToCompare;
			this.comparer = comparer;
		}

		public EqualValidator(Func<object, object> comparisonProperty, MemberInfo member)
			: base(() => Messages.equal_error)  {
			func = comparisonProperty;
			MemberToCompare = member;
		}

		public EqualValidator(Func<object, object> comparisonProperty, MemberInfo member, IEqualityComparer comparer)
			: base(() => Messages.equal_error) {
			func = comparisonProperty;
			MemberToCompare = member;
			this.comparer = comparer;
		}

		protected override bool IsValid(PropertyValidatorContext context) {
			var comparisonValue = GetComparisonValue(context);
			bool success = Compare(comparisonValue, context.PropertyValue);

			if (!success) {
				context.MessageFormatter.AppendArgument("PropertyValue", comparisonValue);
				return false;
			}

			return true;
		}

		private object GetComparisonValue(PropertyValidatorContext context) {
			if(func != null) {
				return func(context.Instance);
			}

			return ValueToCompare;
		}

		public Comparison Comparison {
			get { return Comparison.Equal; }
		}

		public MemberInfo MemberToCompare { get; private set; }
		public object ValueToCompare { get; private set; }

		protected bool Compare(object comparisonValue, object propertyValue) {
			if(comparer != null) {
				return comparer.Equals(comparisonValue, propertyValue);
			}

			return Internal.Comparer.GetEqualsResult((IComparable)comparisonValue, (IComparable)propertyValue);
		}
	}
}