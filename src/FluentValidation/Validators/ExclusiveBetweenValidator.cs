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
	/// Performs range validation where the property value must be between the two specified values (exclusive).
	/// </summary>
	public class ExclusiveBetweenValidator<T, TProperty> : RangeValidator<T, TProperty> {

		public override string Name => "ExclusiveBetweenValidator";

		public ExclusiveBetweenValidator(TProperty from, TProperty to, IComparer<TProperty> comparer) : base(from, to, comparer) {
		}

		protected override bool HasError(TProperty value) {
			return Compare(value, From) <= 0 || Compare(value, To) >= 0;
		}
	}

	public interface IBetweenValidator : IPropertyValidator {
		object From { get; }
		object To { get; }
	}
}
