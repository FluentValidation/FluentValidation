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
	using System.Collections;
	using System.Reflection;
	using Resources;

	public class EqualValidator : PropertyValidator, IComparisonValidator {
		readonly Func<object, object> _func;
		private readonly string _memberDisplayName;
		readonly IEqualityComparer _comparer;

		public EqualValidator(object valueToCompare, IEqualityComparer comparer = null) {
			ValueToCompare = valueToCompare;
			_comparer = comparer;
		}

		public EqualValidator(Func<object, object> comparisonProperty, MemberInfo member, string memberDisplayName, IEqualityComparer comparer = null) {
			_func = comparisonProperty;
			_memberDisplayName = memberDisplayName;
			MemberToCompare = member;
			_comparer = comparer;
		}

		protected override bool IsValid(PropertyValidatorContext context) {
			var comparisonValue = GetComparisonValue(context);
			bool success = Compare(comparisonValue, context.PropertyValue);

			if (!success) {
				context.MessageFormatter.AppendArgument("ComparisonValue", comparisonValue);
				context.MessageFormatter.AppendArgument("ComparisonProperty", _memberDisplayName ?? "");

				return false;
			}

			return true;
		}

		private object GetComparisonValue(PropertyValidatorContext context) {
			if (_func != null) {
				return _func(context.InstanceToValidate);
			}

			return ValueToCompare;
		}

		public Comparison Comparison => Comparison.Equal;

		public MemberInfo MemberToCompare { get; private set; }
		public object ValueToCompare { get; private set; }

		protected bool Compare(object comparisonValue, object propertyValue) {
			if (_comparer != null) {
				return _comparer.Equals(comparisonValue, propertyValue);
			}

			return Equals(comparisonValue, propertyValue);
		}

		protected override string GetDefaultMessageTemplate() {
			return Localized(nameof(EqualValidator));
		}
	}
}
