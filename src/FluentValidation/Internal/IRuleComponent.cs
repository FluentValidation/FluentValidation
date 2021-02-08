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

namespace FluentValidation.Internal {
	using Validators;

	/// <summary>
	/// An individual component within a rule with a validator attached.
	/// </summary>
	public interface IRuleComponent {
		/// <summary>
		/// Whether or not this validator has a condition associated with it.
		/// </summary>
		bool HasCondition { get; }

		/// <summary>
		/// Whether or not this validator has an async condition associated with it.
		/// </summary>
		bool HasAsyncCondition { get; }

		/// <summary>
		/// The validator associated with this component.
		/// </summary>
		IPropertyValidator Validator { get; }

		/// <summary>
		/// Gets the raw unformatted error message. Placeholders will not have been rewritten.
		/// </summary>
		/// <returns></returns>
		string GetUnformattedErrorMessage();
	}
}
