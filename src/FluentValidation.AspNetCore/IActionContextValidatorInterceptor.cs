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
namespace FluentValidation.AspNetCore {
	using System;
	using FluentValidation.Results;
	using Microsoft.AspNetCore.Mvc;
	using FluentValidation;

	/// <summary>
	/// Specifies an interceptor that can be used to provide hooks that will be called before and after ASP.NET Core validation occurs.
	/// </summary>
	[Obsolete("IActionContextValidatorInterceptor will be removed and be combined with IValidatorInterceptor in the next major version")]
	public interface IActionContextValidatorInterceptor {
		/// <summary>
		/// Invoked before MVC validation takes place which allows the ValidationContext to be customized prior to validation.
		/// It should return a ValidationContext object.
		/// </summary>
		/// <param name="controllerContext">Action Context</param>
		/// <param name="validationContext">Validation Context</param>
		/// <returns>Validation Context</returns>
		IValidationContext BeforeMvcValidation(ActionContext actionContext, IValidationContext validationContext);

		/// <summary>
		/// Invoked after MVC validation takes place which allows the result to be customized.
		/// It should return a ValidationResult.
		/// </summary>
		/// <param name="actionContext">Action Context</param>
		/// <param name="validationContext">Validation Context</param>
		/// <param name="result">The result of validation.</param>
		/// <returns>Validation Context</returns>
		ValidationResult AfterMvcValidation(ActionContext actionContext, IValidationContext validationContext, ValidationResult result);
	}
}
