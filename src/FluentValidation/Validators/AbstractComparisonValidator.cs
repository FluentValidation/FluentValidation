#region License
// Copyright (c) .NET Foundation and contributors.
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
// The latest version of this file can be found at https://github.com/FluentValidation/FluentValidation
#endregion

namespace FluentValidation.Validators {
	using System;
	using System.Reflection;
	using Internal;
	using Resources;

	/// <summary>
	/// Base class for all comparison validators
	/// </summary>
	public abstract class AbstractComparisonValidator : PropertyValidator, IComparisonValidator {

		readonly Func<object, object> _valueToCompareFunc;
		private readonly string _comparisonMemberDisplayName;

		/// <summary>
		/// </summary>
		/// <param name="value"></param>
		/// <param name="errorSource"></param>
		[Obsolete("This constructor will be removed from FV10. Use the overload that doesn't take an errorSource")]
		protected AbstractComparisonValidator(IComparable value, IStringSource errorSource) : base(errorSource) {
			value.Guard("value must not be null.", nameof(value));
			ValueToCompare = value;
		}

		/// <summary>
		/// </summary>
		/// <param name="value"></param>
		protected AbstractComparisonValidator(IComparable value) {
			value.Guard("value must not be null.", nameof(value));
			ValueToCompare = value;
		}

		/// <summary>
		/// </summary>
		/// <param name="valueToCompareFunc"></param>
		/// <param name="member"></param>
		/// <param name="memberDisplayName"></param>
		/// <param name="errorSource"></param>
		[Obsolete("This constructor will be removed from FV10. Use the overload that doesn't take an errorSource")]
		protected AbstractComparisonValidator(Func<object, object> valueToCompareFunc, MemberInfo member, string memberDisplayName, IStringSource errorSource) : base(errorSource) {
			_valueToCompareFunc = valueToCompareFunc;
			_comparisonMemberDisplayName = memberDisplayName;
			MemberToCompare = member;
		}

		/// <summary>
		/// </summary>
		/// <param name="valueToCompareFunc"></param>
		/// <param name="member"></param>
		/// <param name="memberDisplayName"></param>
		protected AbstractComparisonValidator(Func<object, object> valueToCompareFunc, MemberInfo member, string memberDisplayName) {
			_valueToCompareFunc = valueToCompareFunc;
			_comparisonMemberDisplayName = memberDisplayName;
			MemberToCompare = member;
		}

		/// <summary>
		/// Performs the comparison
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		protected sealed override bool IsValid(PropertyValidatorContext context) {
			if(context.PropertyValue == null) {
				// If we're working with a nullable type then this rule should not be applied.
				// If you want to ensure that it's never null then a NotNull rule should also be applied.
				return true;
			}

			var value = GetComparisonValue(context);

			if (!IsValid((IComparable)context.PropertyValue, value)) {
				context.MessageFormatter.AppendArgument("ComparisonValue", value);
				context.MessageFormatter.AppendArgument("ComparisonProperty", _comparisonMemberDisplayName ?? "");
				return false;
			}

			return true;
		}

		public IComparable GetComparisonValue(PropertyValidatorContext context) {
			if(_valueToCompareFunc != null) {
				return (IComparable)_valueToCompareFunc(context.InstanceToValidate);
			}

			return (IComparable)ValueToCompare;
		}

		/// <summary>
		/// Override to perform the comparison
		/// </summary>
		/// <param name="value"></param>
		/// <param name="valueToCompare"></param>
		/// <returns></returns>
		public abstract bool IsValid(IComparable value, IComparable valueToCompare);

		/// <summary>
		/// Metadata- the comparison type
		/// </summary>
		public abstract Comparison Comparison { get; }
		/// <summary>
		/// Metadata- the member being compared
		/// </summary>
		public MemberInfo MemberToCompare { get; private set; }
		/// <summary>
		/// Metadata- the value being compared
		/// </summary>
		public object ValueToCompare { get; private set; }
	}

	/// <summary>
	/// Defines a comparison validator
	/// </summary>
	public interface IComparisonValidator : IPropertyValidator {
		/// <summary>
		/// Metadata- the comparison type
		/// </summary>
		Comparison Comparison { get; }
		/// <summary>
		/// Metadata- the member being compared
		/// </summary>
		MemberInfo MemberToCompare { get; }
		/// <summary>
		/// Metadata- the value being compared
		/// </summary>
		object ValueToCompare { get; }
	}

#pragma warning disable 1591
	public enum Comparison {
		Equal,
		NotEqual,
		LessThan,
		GreaterThan,
		GreaterThanOrEqual,
		LessThanOrEqual
	}
#pragma warning restore 1591

}
