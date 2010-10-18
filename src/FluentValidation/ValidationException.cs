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
// The latest version of this file can be found at http://www.codeplex.com/FluentValidation
#endregion

namespace FluentValidation {
	using System;
	using System.Collections.Generic;
	using Results;
	using System.Linq;

	public class ValidationException : Exception {
		public IEnumerable<ValidationFailure> Errors { get; private set; }

		public ValidationException(IEnumerable<ValidationFailure> errors) : base(BuildErrorMesage(errors)) {
			Errors = errors;
		}

		private static string BuildErrorMesage(IEnumerable<ValidationFailure> errors) {
			var arr = errors.Select(x => "\r\n -- " + x.ErrorMessage).ToArray();
			return "Validation failed: " + string.Join("", arr);
		}
	}
}