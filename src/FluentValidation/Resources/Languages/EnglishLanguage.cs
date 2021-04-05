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

#pragma warning disable 618

namespace FluentValidation.Resources {

	internal class EnglishLanguage {
		public const string Culture = "en";
		public const string AmericanCulture = "en-US";
		public const string BritishCulture = "en-GB";

		public static string GetTranslation(string key) => key switch {
			"EmailValidator" => "'{PropertyName}' is not a valid email address.",
			"GreaterThanOrEqualValidator" => "'{PropertyName}' must be greater than or equal to '{ComparisonValue}'.",
			"GreaterThanValidator" => "'{PropertyName}' must be greater than '{ComparisonValue}'.",
			"LengthValidator" => "'{PropertyName}' must be between {MinLength} and {MaxLength} characters. You entered {TotalLength} characters.",
			"MinimumLengthValidator" => "The length of '{PropertyName}' must be at least {MinLength} characters. You entered {TotalLength} characters.",
			"MaximumLengthValidator" => "The length of '{PropertyName}' must be {MaxLength} characters or fewer. You entered {TotalLength} characters.",
			"LessThanOrEqualValidator" => "'{PropertyName}' must be less than or equal to '{ComparisonValue}'.",
			"LessThanValidator" => "'{PropertyName}' must be less than '{ComparisonValue}'.",
			"NotEmptyValidator" => "'{PropertyName}' must not be empty.",
			"NotEqualValidator" => "'{PropertyName}' must not be equal to '{ComparisonValue}'.",
			"NotNullValidator" => "'{PropertyName}' must not be empty.",
			"PredicateValidator" => "The specified condition was not met for '{PropertyName}'.",
			"AsyncPredicateValidator" => "The specified condition was not met for '{PropertyName}'.",
			"RegularExpressionValidator" => "'{PropertyName}' is not in the correct format.",
			"EqualValidator" => "'{PropertyName}' must be equal to '{ComparisonValue}'.",
			"ExactLengthValidator" => "'{PropertyName}' must be {MaxLength} characters in length. You entered {TotalLength} characters.",
			"InclusiveBetweenValidator" => "'{PropertyName}' must be between {From} and {To}. You entered {PropertyValue}.",
			"ExclusiveBetweenValidator" => "'{PropertyName}' must be between {From} and {To} (exclusive). You entered {PropertyValue}.",
			"CreditCardValidator" => "'{PropertyName}' is not a valid credit card number.",
			"ScalePrecisionValidator" => "'{PropertyName}' must not be more than {ExpectedPrecision} digits in total, with allowance for {ExpectedScale} decimals. {Digits} digits and {ActualScale} decimals were found.",
			"EmptyValidator" => "'{PropertyName}' must be empty.",
			"NullValidator" => "'{PropertyName}' must be empty.",
			"EnumValidator" => "'{PropertyName}' has a range of values which does not include '{PropertyValue}'.",
			// Additional fallback messages used by clientside validation integration.
			"Length_Simple" => "'{PropertyName}' must be between {MinLength} and {MaxLength} characters.",
			"MinimumLength_Simple" => "The length of '{PropertyName}' must be at least {MinLength} characters.",
			"MaximumLength_Simple" => "The length of '{PropertyName}' must be {MaxLength} characters or fewer.",
			"ExactLength_Simple" => "'{PropertyName}' must be {MaxLength} characters in length.",
			"InclusiveBetween_Simple" => "'{PropertyName}' must be between {From} and {To}.",
			_ => null,
		};
	}
}
