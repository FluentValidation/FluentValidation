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

	public abstract class RangeValidator<T, TProperty> : PropertyValidator<T, TProperty>, IBetweenValidator {

		public RangeValidator(TProperty from, TProperty to) {
			To = to;
			From = from;

			if (Compare(to, from) == -1) {
				throw new ArgumentOutOfRangeException(nameof(to), "To should be larger than from.");
			}
		}
		public RangeValidator(TProperty from, TProperty to, IComparer comparer) {
			To = to;
			From = from;

			explicitComparer = comparer;

			if (comparer.Compare(to, from) == -1) {
				throw new ArgumentOutOfRangeException(nameof(to), "To should be larger than from.");
			}
		}

		readonly IComparer explicitComparer = null;

		public object From { get; }
		public object To { get; }

		object IBetweenValidator.From => From;
		object IBetweenValidator.To => To;

		protected int Compare(object a, object b) {

			// Use explicitComparer first
			if (explicitComparer != null)
				return explicitComparer.Compare(a, b);
			else if (a is IComparable comparableA)
				return comparableA.CompareTo(b);
			else
				throw new NotSupportedException("Object should either implement IComparable or IComparer should be passed");
		}
	}
}
