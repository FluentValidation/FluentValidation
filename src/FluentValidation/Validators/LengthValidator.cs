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

namespace FluentValidation.Validators {
	using System;
	using System.Linq.Expressions;
	using Attributes;
	using Resources;

	public class LengthValidator : PropertyValidator, ILengthValidator {
		public int Min { get; private set; }
		public int Max { get; private set; }

		public Func<object, int> MinFunc { get; set; }
		public Func<object, int> MaxFunc { get; set; }

		public LengthValidator(int min, int max) : this(min, max, () => Messages.length_error) {
		}

		public LengthValidator(int min, int max, Expression<Func<string>> errorMessageResourceSelector) : base(errorMessageResourceSelector) {
			Max = max;
			Min = min;

			if (max != -1 && max < min) {
				throw new ArgumentOutOfRangeException("max", "Max should be larger than min.");
			}
		}

		public LengthValidator(Func<object, int> min, Func<object, int> max)
			: this(min, max, () => Messages.length_error) {
		}

		public LengthValidator(Func<object, int> min, Func<object, int> max, Expression<Func<string>> errorMessageResourceSelector) : base(errorMessageResourceSelector) {
			MaxFunc = max;
			MinFunc = min;
		}

		protected override bool IsValid(PropertyValidatorContext context) {
			if (context.PropertyValue == null) return true;

			if (MaxFunc != null && MinFunc != null)
			{
				Max = MaxFunc(context.Instance);
				Min = MinFunc(context.Instance);
			}

			int length = context.PropertyValue.ToString().Length;

			if (length < Min || (length > Max && Max != -1)) {
				context.MessageFormatter
					.AppendArgument("MinLength", Min)
					.AppendArgument("MaxLength", Max)
					.AppendArgument("TotalLength", length);

				return false;
			}

			return true;
		}
	}

	public class ExactLengthValidator : LengthValidator {
		public ExactLengthValidator(int length) : base(length,length, () => Messages.exact_length_error) {
			
		}

		public ExactLengthValidator(Func<object, int> length)
			: base(length, length, () => Messages.exact_length_error) {

		}
	}

	public class MaximumLengthValidator : LengthValidator {
		public MaximumLengthValidator(int max) : this(max, () => Messages.length_error) {

		}

		public MaximumLengthValidator(int max, Expression<Func<string>> errorMessageResourceSelector)
			: base(0, max, errorMessageResourceSelector) {

		}

		public MaximumLengthValidator(Func<object, int> max) : 
			this(max, () => Messages.length_error) { 

		}

		public MaximumLengthValidator(Func<object, int> max, Expression<Func<string>> errorMessageResourceSelector)
			: base(obj => 0, max, errorMessageResourceSelector) {

		}
	}

	public class MinimumLengthValidator : LengthValidator {
		public MinimumLengthValidator(int min) : this(min, () => Messages.length_error) {

		}

		public MinimumLengthValidator(int min, Expression<Func<string>> errorMessageResourceSelector) 
			: base(min, -1, errorMessageResourceSelector) {

		}

		public MinimumLengthValidator(Func<object, int> min)
			: this(min, () => Messages.length_error) {

		}

		public MinimumLengthValidator(Func<object, int> min, Expression<Func<string>> errorMessageResourceSelector)
			: base(min, obj => -1, errorMessageResourceSelector) {

		}
	}

	public interface ILengthValidator : IPropertyValidator {
		int Min { get; }
		int Max { get; }
	}
}