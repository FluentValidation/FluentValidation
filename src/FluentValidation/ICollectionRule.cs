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

namespace FluentValidation {
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Represents a rule defined against a collection with RuleForEach.
	/// </summary>
	/// <typeparam name="T">Root object</typeparam>
	/// <typeparam name="TElement">Type of each element in the collection</typeparam>
	public interface ICollectionRule<T, TElement> : IValidationRule<T, TElement> {
		/// <summary>
		/// Filter that should include/exclude items in the collection.
		/// </summary>
		public Func<TElement, bool> Filter { get; set; }

		/// <summary>
		/// Constructs the indexer in the property name associated with the error message.
		/// By default this is "[" + index + "]"
		/// </summary>
		public Func<T, IEnumerable<TElement>, TElement, int, string> IndexBuilder { get; set; }
	}
}
