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
#if !NET461 && !NETSTANDARD2_0
	using System.Collections.Immutable;
#endif

	public class EmptyValidator : PropertyValidator, IEmptyValidator {
		readonly object _defaultValueForType;

		public EmptyValidator(object defaultValueForType) {
			_defaultValueForType = defaultValueForType;
		}

		protected override bool IsValid(PropertyValidatorContext context) {
			return NotEmptyValidator.IsEmpty(context.PropertyValue, _defaultValueForType);
		}

		protected override string GetDefaultMessageTemplate() {
			return Localized(nameof(EmptyValidator));
		}
	}

	#if !NET461 && !NETSTANDARD2_0
	internal class EmptyImmutableArrayValidator<TElement> : PropertyValidator, INotEmptyValidator {
		protected override bool IsValid(PropertyValidatorContext context) {
			if (context.PropertyValue == null) return true;
			var value = (ImmutableArray<TElement>)context.PropertyValue;
			return value.IsDefaultOrEmpty;
		}

		protected override string GetDefaultMessageTemplate() {
			return Localized(nameof(EmptyValidator));
		}
	}
	#endif

	public interface IEmptyValidator : IPropertyValidator {
	}
}
