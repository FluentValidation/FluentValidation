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
	using System.Collections.Generic;

	/// <summary>
	/// Base class for range validation.
	/// </summary>
	public abstract class RangeValidator<T, TProperty> : PropertyValidator<T, TProperty>, IBetweenValidator {

		readonly IComparer<TProperty> _explicitComparer;

		public RangeValidator(TProperty from, TProperty to, IComparer<TProperty> comparer) {
			To = to;
			From = from;

			_explicitComparer = comparer;

			if (comparer.Compare(to, from) == -1) {
				throw new ArgumentOutOfRangeException(nameof(to), "To should be larger than from.");
			}
		}

		public TProperty From { get; }
		public TProperty To { get; }

		object IBetweenValidator.From => From;
		object IBetweenValidator.To => To;

		protected abstract bool HasError(TProperty value);

		public override bool IsValid(ValidationContext<T> context, TProperty value) {
			// If the value is null then we abort and assume success.
			// This should not be a failure condition - only a NotNull/NotEmpty should cause a null to fail.
			if (value == null) return true;

			if (HasError(value)) {

				context.MessageFormatter
					.AppendArgument("From", From)
					.AppendArgument("To", To);

				return false;
			}
			return true;
		}

		protected int Compare(TProperty a, TProperty b) {
			return _explicitComparer.Compare(a, b);
		}

		protected override string GetDefaultMessageTemplate(string errorCode) {
			return Localized(errorCode, Name);
		}
	}

	public static class RangeValidatorFactory {
		public static ExclusiveBetweenValidator<T, TProperty> CreateExclusiveBetween<T,TProperty>(TProperty from, TProperty to)
			where TProperty : IComparable<TProperty>, IComparable =>
			new ExclusiveBetweenValidator<T, TProperty>(from, to, ComparableComparer<TProperty>.Instance);

		public static InclusiveBetweenValidator<T, TProperty> CreateInclusiveBetween<T,TProperty>(TProperty from, TProperty to)
			where TProperty : IComparable<TProperty>, IComparable {
			return new InclusiveBetweenValidator<T, TProperty>(from, to, ComparableComparer<TProperty>.Instance);
		}
	}
}
