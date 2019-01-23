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
	using System.Collections;
	using Resources;

	public class CountValidator : PropertyValidator {
		public int Min { get; }
		public int Max { get; }

		public CountValidator(int min, int max) : base(new LanguageStringSource(nameof(CountValidator))) {
			if (min < 0)
				throw new ArgumentOutOfRangeException(nameof(min), "Min should be equal to or greater than zero.");
			if (max < min)
				throw new ArgumentOutOfRangeException(nameof(max), "Max should be equal to or greater than min.");

			Min = min;
			Max = max;
		}

		protected override bool IsValid(PropertyValidatorContext context) {
			if (!(context.PropertyValue is ICollection collection))
				return true;

			var total = collection.Count;

			if (total >= Min && total <= Max)
				return true;

			context.MessageFormatter
				.AppendArgument("Min", Min)
				.AppendArgument("Max", Max)
				.AppendArgument("Total", total);
			return false;
		}
	}
}