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

namespace FluentValidation.Validators;

using System;
using System.Reflection;
using Internal;

/// <summary>
/// Base class for all comparison validators
/// </summary>
public abstract class AbstractComparisonValidator<T, TProperty> : PropertyValidator<T,TProperty>, IComparisonValidator where TProperty : IComparable<TProperty>, IComparable {
	readonly Func<T, (bool HasValue, TProperty Value)> _valueToCompareFuncForNullables;
	private readonly Func<T, TProperty> _valueToCompareFunc;
	private readonly string _comparisonMemberDisplayName;

	/// <summary>
	/// </summary>
	/// <param name="value"></param>
	protected AbstractComparisonValidator(TProperty value) {
		ArgumentNullException.ThrowIfNull(value);
		ValueToCompare = value;
	}

	/// <summary>
	/// </summary>
	/// <param name="valueToCompareFunc"></param>
	/// <param name="member"></param>
	/// <param name="memberDisplayName"></param>
	protected AbstractComparisonValidator(Func<T, (bool HasValue, TProperty Value)> valueToCompareFunc, MemberInfo member, string memberDisplayName) {
		_valueToCompareFuncForNullables = valueToCompareFunc;
		_comparisonMemberDisplayName = memberDisplayName;
		MemberToCompare = member;
	}

	/// <summary>
	/// </summary>
	/// <param name="valueToCompareFunc"></param>
	/// <param name="member"></param>
	/// <param name="memberDisplayName"></param>
	protected AbstractComparisonValidator(Func<T, TProperty> valueToCompareFunc, MemberInfo member, string memberDisplayName) {
		_valueToCompareFunc = valueToCompareFunc;
		_comparisonMemberDisplayName = memberDisplayName;
		MemberToCompare = member;
	}

	/// <summary>
	/// Performs the comparison
	/// </summary>
	/// <param name="context"></param>
	/// <param name="propertyValue"></param>
	/// <returns></returns>
	public sealed override bool IsValid(ValidationContext<T> context, TProperty propertyValue) {
		if(propertyValue == null) {
			// If we're working with a nullable type then this rule should not be applied.
			// If you want to ensure that it's never null then a NotNull rule should also be applied.
			return true;
		}

		var valueToCompare = GetComparisonValue(context);

		if (!valueToCompare.HasValue || !IsValid(propertyValue, valueToCompare.Value)) {
			context.MessageFormatter.AppendArgument("ComparisonValue", valueToCompare.HasValue ? valueToCompare.Value : "");
			context.MessageFormatter.AppendArgument("ComparisonProperty", _comparisonMemberDisplayName ?? "");
			return false;
		}

		return true;
	}

	public (bool HasValue, TProperty Value) GetComparisonValue(ValidationContext<T> context) {
		if(_valueToCompareFunc != null) {
			var value = _valueToCompareFunc(context.InstanceToValidate);
			return (value != null, value);
		}
		if (_valueToCompareFuncForNullables != null) {
			return _valueToCompareFuncForNullables(context.InstanceToValidate);
		}

		return (ValueToCompare != null, ValueToCompare);
	}

	/// <summary>
	/// Override to perform the comparison
	/// </summary>
	/// <param name="value"></param>
	/// <param name="valueToCompare"></param>
	/// <returns></returns>
	public abstract bool IsValid(TProperty value, TProperty valueToCompare);

	/// <summary>
	/// Metadata- the comparison type
	/// </summary>
	public abstract Comparison Comparison { get; }
	/// <summary>
	/// Metadata- the member being compared
	/// </summary>
	public MemberInfo MemberToCompare { get; private set; }

	/// <summary>
	/// The value being compared
	/// </summary>
	public TProperty ValueToCompare { get; }

	/// <summary>
	/// Comparison value as non-generic for metadata.
	/// </summary>
	object IComparisonValidator.ValueToCompare =>
		// For clientside validation to work, we must return null if MemberToCompare or valueToCompareFunc is set.
		// We can't rely on ValueToCompare being null itself as it's generic, and will be initialized
		// as default(TProperty) which for non-nullable value types will emit the
		// default value for the type rather than null. See https://github.com/FluentValidation/FluentValidation/issues/1721
		MemberToCompare != null || _valueToCompareFunc != null ? null : ValueToCompare;
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
