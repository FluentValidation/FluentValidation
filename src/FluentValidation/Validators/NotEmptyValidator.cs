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
using System.Collections;
using System.Collections.Generic;

public class NotEmptyValidator<T,TProperty> : PropertyValidator<T, TProperty>, INotEmptyValidator {

	public override string Name => "NotEmptyValidator";

	public override bool IsValid(ValidationContext<T> context, TProperty value) {
		if (value == null) {
			return false;
		}

		if (value is string s && string.IsNullOrWhiteSpace(s)) {
			return false;
		}

		if (value is ICollection col && col.Count == 0) {
			return false;
		}

		if (value is IEnumerable e && IsEmpty(e)) {
			return false;
		}

		return !EqualityComparer<TProperty>.Default.Equals(value, default);
	}

	protected override string GetDefaultMessageTemplate(string errorCode) {
		return Localized(errorCode, Name);
	}

	private static bool IsEmpty(IEnumerable enumerable) {
		var enumerator = enumerable.GetEnumerator();

		using (enumerator as IDisposable) {
			return !enumerator.MoveNext();
		}
	}
}

public interface INotEmptyValidator : IPropertyValidator {
}
