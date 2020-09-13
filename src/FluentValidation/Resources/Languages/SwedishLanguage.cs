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
	using Validators;

	internal class SwedishLanguage {
		public const string Culture = "sv";

		public static string GetTranslation(string key) => key switch {
			nameof(EmailValidator) => "\"{PropertyName}\" är inte en giltig e-postadress.",
			nameof(GreaterThanOrEqualValidator) => "\"{PropertyName}\" måste vara större än eller lika med {ComparisonValue}.",
			nameof(GreaterThanValidator) => "\"{PropertyName}\" måste vara större än {ComparisonValue}.",
			nameof(LengthValidator) => "\"{PropertyName}\" måste vara mellan {MinLength} och {MaxLength} tecken långt. Du angav {TotalLength} tecken.",
			nameof(MinimumLengthValidator) => "\"{PropertyName}\" måste vara större än eller lika med {MinLength} tecken. Du har skrivit in {TotalLength} tecken.",
			nameof(MaximumLengthValidator) => "\"{PropertyName}\" måste vara mindre än eller lika med {MaxLength} tecken. Du har skrivit in {TotalLength} tecken.",
			nameof(LessThanOrEqualValidator) => "\"{PropertyName}\" måste vara mindre än eller lika med {ComparisonValue}.",
			nameof(LessThanValidator) => "\"{PropertyName}\" måste vara mindre än {ComparisonValue}.",
			nameof(NotEmptyValidator) => "\"{PropertyName}\" måste anges.",
			nameof(NotEqualValidator) => "\"{PropertyName}\" får inte vara lika med \"{ComparisonValue}\".",
			nameof(NotNullValidator) => "\"{PropertyName}\" måste anges.",
			nameof(PredicateValidator) => "Det angivna villkoret uppfylldes inte för \"{PropertyName}\".",
			nameof(AsyncPredicateValidator) => "Det angivna villkoret uppfylldes inte för \"{PropertyName}\".",
			nameof(RegularExpressionValidator) => "\"{PropertyName}\" har inte ett korrekt format.",
			nameof(EqualValidator) => "\"{PropertyName}\" måste vara lika med \"{ComparisonValue}\".",
			nameof(ExactLengthValidator) => "\"{PropertyName}\" måste vara {MaxLength} tecken långt. Du angav {TotalLength} tecken.",
			nameof(InclusiveBetweenValidator) => "\"{PropertyName}\" måste vara mellan {From} och {To}. Du angav {Value}.",
			nameof(ExclusiveBetweenValidator) => "\"{PropertyName}\" måste vara mellan {From} och {To} (gränsvärdena exkluderade). Du angav {Value}.",
			nameof(CreditCardValidator) => "\"{PropertyName}\" no es un número de tarjeta de crédito válido.",
			nameof(ScalePrecisionValidator) => "\"{PropertyName}\" får inte vara mer än {ExpectedPrecision} siffror totalt, med förbehåll för {ExpectedScale} decimaler. {Digits} siffror och {ActualScale} decimaler hittades.",
			nameof(EmptyValidator) => "\"{PropertyName}\" ska vara tomt.",
			nameof(NullValidator) => "\"{PropertyName}\" ska vara tomt.",
			nameof(EnumValidator) => "\"{PropertyName}\" har ett antal värden som inte inkluderar \"{PropertyValue}\".",
			// Additional fallback messages used by clientside validation integration.
			"Length_Simple" => "\"{PropertyName}\" måste vara mellan {MinLength} och {MaxLength} tecken långt.",
			"MinimumLength_Simple" => "\"{PropertyName}\" måste vara större än eller lika med {MinLength} tecken.",
			"MaximumLength_Simple" => "\"{PropertyName}\" måste vara mindre än eller lika med {MaxLength} tecken.",
			"ExactLength_Simple" => "\"{PropertyName}\" måste vara {MaxLength} tecken långt.",
			"InclusiveBetween_Simple" => "\"{PropertyName}\" måste vara mellan {From} och {To}.",
			_ => null,
		};
	}
}
