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
	using Resources;
	using System.Linq;

	public class EmptyValidator<T,TProperty> : PropertyValidator<T,TProperty> {

		public override string Name => "EmptyValidator";

		public override bool IsValid(ValidationContext<T> context, TProperty value) {
			switch (value) {
				case null:
				case string s when string.IsNullOrWhiteSpace(s):
				case ICollection {Count: 0}:
				case Array {Length: 0}:
				case IEnumerable e when !e.Cast<object>().Any():
					return true;
			}

			//TODO: Rewrite to avoid boxing
			if (Equals(value, default(TProperty))) {
				// Note: Code analysis indicates "Expression is always false" but this is incorrect.
				return true;
			}

			return false;
		}

		protected override string GetDefaultMessageTemplate(string errorCode) {
			return Localized(errorCode, Name);
		}
	}
}
