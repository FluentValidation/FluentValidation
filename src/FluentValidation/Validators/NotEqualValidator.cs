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
	using System.Collections.Generic;
	using System.Linq.Expressions;
	using System.Reflection;
	using Attributes;
	using Internal;
	using Resources;
	using Results;

	[ValidationMessage(Key=DefaultResourceManager.NotEqual)]
	public class NotEqualValidator<TInstance, TProperty> : IPropertyValidator<TInstance, TProperty>, IComparisonValidator {
		readonly IEqualityComparer<TProperty> comparer;
		readonly Func<TInstance, TProperty> func;

		public NotEqualValidator(Expression<Func<TInstance, TProperty>> expression) {
			func = expression.Compile();

			MemberToCompare = expression.GetMember();
			
			if (MemberToCompare == null && expression.Body.NodeType == ExpressionType.Constant) {
				var constant = expression.Body as ConstantExpression;
				ValueToCompare = constant != null ? (TProperty)constant.Value : default(TProperty);
			}
		}

		public NotEqualValidator(Expression<Func<TInstance, TProperty>> expression, IEqualityComparer<TProperty> comparer) : this(expression) {
			this.comparer = comparer;
		}

		public PropertyValidatorResult Validate(PropertyValidatorContext context) {
			var comparisonValue = func((TInstance)context.Instance);
			bool success = !Compare(comparisonValue, (TProperty)context.PropertyValue);

			if (!success) {
				var formatter = new MessageFormatter()
					.AppendProperyName(context.PropertyDescription)
					.AppendArgument("PropertyValue", context.PropertyValue);

				string error = context.GetFormattedErrorMessage(typeof(NotEqualValidator<TInstance, TProperty>), formatter);
				return PropertyValidatorResult.Failure(error);
			}

			return PropertyValidatorResult.Success();
		}

		public Comparison Comparison {
			get { return Comparison.NotEqual; }
		}

		public MemberInfo MemberToCompare { get; private set; }
		public object ValueToCompare { get; private set; }

		protected bool Compare(TProperty comparisonValue, TProperty propertyValue) {
			if(comparer != null) {
				return comparer.Equals(comparisonValue, propertyValue);
			}
			return Equals(comparisonValue, propertyValue);
		}
	}
}