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
	using Internal;
	using Resources;

	public class InclusiveBetweenValidator : PropertyValidator, IBetweenValidator {
		public InclusiveBetweenValidator(IComparable from, IComparable to) : base(new LanguageStringSource(nameof(InclusiveBetweenValidator))) {
			To = to;
			From = from;

			if (to.CompareTo(from) == -1) {
				throw new ArgumentOutOfRangeException(nameof(to), "To should be larger than from.");
			}

		}

		public IComparable From { get; }
		public IComparable To { get; }

		protected override bool IsValid(PropertyValidatorContext context) {
			var propertyValue = (IComparable)context.PropertyValue;

			// If the value is null then we abort and assume success.
			// This should not be a failure condition - only a NotNull/NotEmpty should cause a null to fail.
			if (propertyValue == null) return true;

			if (propertyValue.CompareTo(From) < 0 || propertyValue.CompareTo(To) > 0) {

				context.MessageFormatter
					.AppendArgument("From", From)
					.AppendArgument("To", To)
					.AppendArgument("Value", context.PropertyValue);

				return false;
			}
			return true;
		}
	}

	public interface IBetweenValidator : IPropertyValidator {
		IComparable From { get; }
		IComparable To { get; }
	}
}
