#region License
// Copyright (c) Jeremy Skinner (http://www.jeremyskinner.co.uk)
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
// The latest version of this file can be found at https://github.com/jeremyskinner/FluentValidation
#endregion

#pragma warning disable 1591
namespace FluentValidation.TestHelper {
	using System;
	using System.Collections.Generic;
	using FluentValidation.Results;

	public class ValidationTestException : Exception {
		public List<ValidationFailure> Errors { get; }

		public ValidationTestException(string message) : base(message) {
		}

		public ValidationTestException(string message, List<ValidationFailure> errors) : this(message) {
			Errors = errors;
		}
	}
}
