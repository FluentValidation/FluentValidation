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
	using Resources;

	public class LengthValidator : PropertyValidator, ILengthValidator {
		public int Min { get; }
		public int Max { get; }

		public Func<object, int> MinFunc { get; set; }
		public Func<object, int> MaxFunc { get; set; }

		public LengthValidator(int min, int max) {
			Max = max;
			Min = min;

			if (max != -1 && max < min) {
				throw new ArgumentOutOfRangeException(nameof(max), "Max should be larger than min.");
			}
		}

		public LengthValidator(Func<object, int> min, Func<object, int> max) {
			MaxFunc = max;
			MinFunc = min;
		}

		protected override bool IsValid(PropertyValidatorContext context) {
			if (context.PropertyValue == null) return true;

			var min = Min;
			var max = Max;

			if (MaxFunc != null && MinFunc != null) {
				max = MaxFunc(context.InstanceToValidate);
				min = MinFunc(context.InstanceToValidate);
			}

			int length = context.PropertyValue.ToString().Length;

			if (length < min || (length > max && max != -1)) {
				context.MessageFormatter
					.AppendArgument("MinLength", min)
					.AppendArgument("MaxLength", max)
					.AppendArgument("TotalLength", length);

				return false;
			}

			return true;
		}

		protected override string GetDefaultMessageTemplate() {
			return Localized(nameof(LengthValidator));
		}
	}

	public class ExactLengthValidator : LengthValidator {
		public ExactLengthValidator(int length) : base(length,length) {

		}

		public ExactLengthValidator(Func<object, int> length)
			: base(length, length) {

		}

		protected override string GetDefaultMessageTemplate() {
			return Localized(nameof(ExactLengthValidator));
		}
	}

	public class MaximumLengthValidator : LengthValidator {
		public MaximumLengthValidator(int max)
			: base(0, max) {

		}

		public MaximumLengthValidator(Func<object, int> max)
			: base(obj => 0, max) {

		}

		protected override string GetDefaultMessageTemplate() {
			return Localized(nameof(MaximumLengthValidator));
		}
	}

	public class MinimumLengthValidator : LengthValidator {

		public MinimumLengthValidator(int min)
			: base(min, -1) {

		}

		public MinimumLengthValidator(Func<object, int> min)
			: base(min, obj => -1) {

		}

		protected override string GetDefaultMessageTemplate() {
			return Localized(nameof(MinimumLengthValidator));
		}
	}

	public interface ILengthValidator : IPropertyValidator {
		int Min { get; }
		int Max { get; }
	}
}
