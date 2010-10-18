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

namespace FluentValidation.TestHelper {
	using System;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	using Internal;

	public class ValidatorTester<T, TValue> where T : class {
		private readonly IValidator<T> validator;
		private readonly TValue value;
		private readonly MemberInfo member;

		public ValidatorTester(Expression<Func<T, TValue>> expression, IValidator<T> validator, TValue value) {
			this.validator = validator;
			this.value = value;
			member = expression.GetMember();
		}


		public void ValidateNoError(T instanceToValidate) {
			SetValue(instanceToValidate);

			var count = validator.Validate(instanceToValidate).Errors.Count(x => x.PropertyName == member.Name);

			if (count > 0) {
				throw new ValidationTestException(string.Format("Expected no validation errors for property {0}", member.Name));
			}
		}

		public void ValidateError(T instanceToValidate) {
			SetValue(instanceToValidate);
			var count = validator.Validate(instanceToValidate).Errors.Count(x => x.PropertyName == member.Name);

			if (count == 0) {
				throw new ValidationTestException(string.Format("Expected a validation error for property {0}", member.Name));
			}
		}

		private void SetValue(object instance) {
			var property = member as PropertyInfo;
			if (property != null) {
				property.SetValue(instance, value, null);
				return;
			}

			var field = member as FieldInfo;
			if (field != null) {
				field.SetValue(instance, value);
			}
		}
	}
}