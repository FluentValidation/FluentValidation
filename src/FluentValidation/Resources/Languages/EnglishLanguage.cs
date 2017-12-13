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
// The latest version of this file can be found at https://github.com/JeremySkinner/FluentValidation
#endregion

namespace FluentValidation.Resources {
	using Validators;

	internal class EnglishLanguage : Language {
		public override string Name => "en";

		public EnglishLanguage() {
			Translate<EmailValidator>("'{PropertyName}' is not a valid email address.");
			Translate<GreaterThanOrEqualValidator>("'{PropertyName}' must be greater than or equal to '{ComparisonValue}'.");
			Translate<GreaterThanValidator>("'{PropertyName}' must be greater than '{ComparisonValue}'.");
			Translate<LengthValidator>("'{PropertyName}' must be between {MinLength} and {MaxLength} characters. You entered {TotalLength} characters.");
			Translate<MinimumLengthValidator>("The length of '{PropertyName}' must be at least {MinLength} characters. You entered {TotalLength} characters.");
			Translate<MaximumLengthValidator>("The length of '{PropertyName}' must {MaxLength} characters or fewer. You entered {TotalLength} characters.");
			Translate<LessThanOrEqualValidator>("'{PropertyName}' must be less than or equal to '{ComparisonValue}'.");
			Translate<LessThanValidator>("'{PropertyName}' must be less than '{ComparisonValue}'.");
			Translate<NotEmptyValidator>("'{PropertyName}' should not be empty.");
			Translate<NotEqualValidator>("'{PropertyName}' should not be equal to '{ComparisonValue}'.");
			Translate<NotNullValidator>("'{PropertyName}' must not be empty.");
			Translate<PredicateValidator>("The specified condition was not met for '{PropertyName}'.");
			Translate<AsyncPredicateValidator>("The specified condition was not met for '{PropertyName}'.");
			Translate<RegularExpressionValidator>("'{PropertyName}' is not in the correct format.");
			Translate<EqualValidator>("'{PropertyName}' should be equal to '{ComparisonValue}'.");
			Translate<ExactLengthValidator>("'{PropertyName}' must be {MaxLength} characters in length. You entered {TotalLength} characters.");
			Translate<InclusiveBetweenValidator>("'{PropertyName}' must be between {From} and {To}. You entered {Value}.");
			Translate<ExclusiveBetweenValidator>("'{PropertyName}' must be between {From} and {To} (exclusive). You entered {Value}.");
			Translate<CreditCardValidator>("'{PropertyName}' is not a valid credit card number.");
			Translate<ScalePrecisionValidator>("'{PropertyName}' may not be more than {expectedPrecision} digits in total, with allowance for {expectedScale} decimals. {digits} digits and {actualScale} decimals were found.");
			Translate<EmptyValidator>("'{PropertyName}' should be empty.");
			Translate<NullValidator>("'{PropertyName}' must be empty.");
			Translate<EnumValidator>("'{PropertyName}' has a range of values which does not include '{PropertyValue}'.");
		}
	}
}