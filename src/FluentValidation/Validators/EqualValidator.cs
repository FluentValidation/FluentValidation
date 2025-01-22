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
using System.Collections.Generic;
using System.Reflection;

public class EqualValidator<T, TProperty>(Func<T, TProperty> comparisonProperty, MemberInfo member, string memberDisplayName, IEqualityComparer<TProperty> comparer = null)
	: PropertyValidator<T, TProperty>, IEqualValidator {
	public override string Name => "EqualValidator";


	public EqualValidator(TProperty valueToCompare, IEqualityComparer<TProperty> comparer = null) : this(null, null, null, comparer) {
		ValueToCompare = valueToCompare;
	}

	public override bool IsValid(ValidationContext<T> context, TProperty value) {
		var comparisonValue = GetComparisonValue(context);
		bool success = Compare(comparisonValue, value);

		if (!success) {
			context.MessageFormatter.AppendArgument("ComparisonValue", comparisonValue);
			context.MessageFormatter.AppendArgument("ComparisonProperty", memberDisplayName ?? "");

			return false;
		}

		return true;
	}

	private TProperty GetComparisonValue(ValidationContext<T> context) {
		if (comparisonProperty != null) {
			return comparisonProperty(context.InstanceToValidate);
		}

		return ValueToCompare;
	}

	public Comparison Comparison => Comparison.Equal;

	public MemberInfo MemberToCompare { get; } = member;
	public TProperty ValueToCompare { get; }

	object IComparisonValidator.ValueToCompare => ValueToCompare;

	protected bool Compare(TProperty comparisonValue, TProperty propertyValue) {
		if (comparer != null) {
			return comparer.Equals(comparisonValue, propertyValue);
		}

		return Equals(comparisonValue, propertyValue);
	}

	protected override string GetDefaultMessageTemplate(string errorCode) {
		return Localized(errorCode, Name);
	}
}

public interface IEqualValidator : IComparisonValidator { }
