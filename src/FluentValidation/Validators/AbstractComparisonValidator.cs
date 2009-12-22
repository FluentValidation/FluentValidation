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
	using System.Linq.Expressions;
	using System.Reflection;
	using Attributes;
	using Internal;
	using Results;

	public abstract class AbstractComparisonValidator<T, TProperty> : IPropertyValidator, IComparisonValidator where TProperty : IComparable<TProperty> {

		readonly Func<T, TProperty> valueToCompareFunc;

		protected AbstractComparisonValidator(Expression<Func<T, TProperty>> valueToCompareFunc) {
			valueToCompareFunc.Guard("The value to compare cannot be null.");
			MemberToCompare = valueToCompareFunc.GetMember();

			if (MemberToCompare == null) {
				if (valueToCompareFunc.Body.NodeType == ExpressionType.Constant) {
					var constant = valueToCompareFunc.Body as ConstantExpression;
					ValueToCompare = constant != null ? (TProperty)constant.Value : default(TProperty);
				}
			}

			this.valueToCompareFunc = valueToCompareFunc.Compile();
		}

		public PropertyValidatorResult Validate(PropertyValidatorContext context) {
			var value = valueToCompareFunc((T)context.Instance);

			if (context.PropertyValue == null || !IsValid((TProperty)context.PropertyValue, value)) {
				var formatter = new MessageFormatter()
					.AppendProperyName(context.PropertyDescription)
					.AppendArgument("ComparisonValue", value);

				string error = context.GetFormattedErrorMessage(GetType(), formatter);
				return PropertyValidatorResult.Failure(error);
			}

			return PropertyValidatorResult.Success();
		}

		public abstract bool IsValid(TProperty value, TProperty valueToCompare);
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