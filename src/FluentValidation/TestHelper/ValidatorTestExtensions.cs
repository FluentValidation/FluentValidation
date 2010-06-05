#region License
// Copyright 2008-2009 Jeremy Skinner (http://www.jeremyskinner.co.uk)
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

namespace FluentValidation.TestHelper {
	using System;
	using System.Linq.Expressions;

	public static class ValidationTestExtension {
		public static void ShouldHaveValidationErrorFor<T, TValue>(this IValidator<T> validator,
		                                                           Expression<Func<T, TValue>> expression, TValue value) where T : class, new() {
			new ValidatorTester<T, TValue>(expression, validator, value).ValidateError(new T());
		}

		public static void ShouldHaveValidationErrorFor<T, TValue>(this IValidator<T> validator, Expression<Func<T, TValue>> expression, T objectToTest) where T : class {
			var value = expression.Compile()(objectToTest);
			new ValidatorTester<T, TValue>(expression, validator, value).ValidateError(objectToTest);
		}

		public static void ShouldNotHaveValidationErrorFor<T, TValue>(this IValidator<T> validator,
		                                                              Expression<Func<T, TValue>> expression, TValue value) where T : class, new() {
			new ValidatorTester<T, TValue>(expression, validator, value).ValidateNoError(new T());
		}

		public static void ShouldNotHaveValidationErrorFor<T, TValue>(this IValidator<T> validator, Expression<Func<T, TValue>> expression, T objectToTest) where T : class {
			var value = expression.Compile()(objectToTest);
			new ValidatorTester<T, TValue>(expression, validator, value).ValidateNoError(objectToTest);
		}

	}
}