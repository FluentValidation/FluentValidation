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
	using System.Linq.Expressions;
	using System.Reflection;
	using Attributes;
	using Internal;
	using Results;

	public abstract class AbstractComparisonValidator : PropertyValidator, IComparisonValidator {

		readonly Func<object, object> valueToCompareFunc;

		protected AbstractComparisonValidator(IComparable value, Expression<Func<string>> errorMessageSelector) : base(errorMessageSelector) {
			value.Guard("value must not be null.");
			ValueToCompare = value;
		}

		protected AbstractComparisonValidator(Func<object, object> valueToCompareFunc, MemberInfo member, Expression<Func<string>> errorMessageSelector)
			: base(errorMessageSelector) {
			this.valueToCompareFunc = valueToCompareFunc;
			this.MemberToCompare = member;
		}

		protected sealed override bool IsValid(PropertyValidatorContext context) {
			if(context.PropertyValue == null) {
				// If we're working with a nullable type then this rule should not be applied.
				// If you want to ensure that it's never null then a NotNull rule should also be applied. 
				return true;
			}
			
			var value = GetComparisonValue(context);

			if (!IsValid((IComparable)context.PropertyValue, value)) {
				context.MessageFormatter.AppendArgument("ComparisonValue", value);
				return false;
			}

			return true;
		}

		private IComparable GetComparisonValue(PropertyValidatorContext context) {
			if(valueToCompareFunc != null) {
				return (IComparable)valueToCompareFunc(context.Instance);
			}

			return (IComparable)ValueToCompare;
		}

		public abstract bool IsValid(IComparable value, IComparable valueToCompare);
		public abstract Comparison Comparison { get; }
		public MemberInfo MemberToCompare { get; private set; }
		public object ValueToCompare { get; private set; }
	}

	public interface IComparisonValidator : IPropertyValidator {
		Comparison Comparison { get; }
		MemberInfo MemberToCompare { get; }
		object ValueToCompare { get; }
	}

	public enum Comparison {
		Equal,
		NotEqual,
		LessThan,
		GreaterThan,
		GreaterThanOrEqual,
		LessThanOrEqual
	}
}