#region License

// Copyright (c) .NET Foundation and contributors.
//
// Licensed under the Apache License, Version 2.0 (the "License",
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

	internal class NorwegianBokmalLanguage {
		public const string Culture = "nb";

		public static string GetTranslation(string key) => key switch {
			nameof(EmailValidator) => "'{PropertyName}' er ikke en gyldig e-postadresse.",
			nameof(GreaterThanOrEqualValidator) => "'{PropertyName}' skal være større enn eller lik '{ComparisonValue}'.",
			nameof(GreaterThanValidator) => "'{PropertyName}' skal være større enn '{ComparisonValue}'.",
			nameof(LengthValidator) => "'{PropertyName}' skal være mellom {MinLength} og {MaxLength} tegn. Du har tastet inn {TotalLength} tegn.",
			nameof(MinimumLengthValidator) => "'{PropertyName}' skal være større enn eller lik {MinLength} tegn. Du tastet inn {TotalLength} tegn.",
			nameof(MaximumLengthValidator) => "'{PropertyName}' skal være mindre enn eller lik {MaxLength} tegn. Du tastet inn {TotalLength} tegn.",
			nameof(LessThanOrEqualValidator) => "'{PropertyName}' skal være mindre enn eller lik '{ComparisonValue}'.",
			nameof(LessThanValidator) => "'{PropertyName}' skal være mindre enn '{ComparisonValue}'.",
			nameof(NotEmptyValidator) => "'{PropertyName}' kan ikke være tom.",
			nameof(NotEqualValidator) => "'{PropertyName}' kan ikke være lik med '{ComparisonValue}'.",
			nameof(NotNullValidator) => "'{PropertyName}' kan ikke være tom.",
			nameof(PredicateValidator) => "Den angitte betingelsen var ikke oppfylt for '{PropertyName}'.",
			nameof(AsyncPredicateValidator) => "Den angitte betingelsen var ikke oppfylt for '{PropertyName}'.",
			nameof(RegularExpressionValidator) => "'{PropertyName}' har ikke riktig format.",
			nameof(EqualValidator) => "'{PropertyName}' skal være lik med '{ComparisonValue}'.",
			nameof(ExactLengthValidator) => "'{PropertyName}' skal være {MaxLength} tegn langt. Du har tastet inn {TotalLength} tegn.",
			nameof(InclusiveBetweenValidator) => "'{PropertyName}' skal være mellom {From} og {To}. Du har tastet inn {Value}.",
			nameof(ExclusiveBetweenValidator) => "'{PropertyName}' skal være mellom {From} og {To} (eksklusiv). Du har tastet inn {Value}.",
			nameof(CreditCardValidator) => "'{PropertyName}' er ikke et gyldig kredittkortnummer.",
			nameof(ScalePrecisionValidator) => "'{PropertyName}' kan ikke være mer enn {ExpectedPrecision} siffer totalt, med hensyn til {ExpectedScale} desimaler. {Digits} siffer og {ActualScale} desimaler ble funnet.",
			nameof(EmptyValidator) => "'{PropertyName}' skal være tomt.",
			nameof(NullValidator) => "'{PropertyName}' skal være tomt.",
			nameof(EnumValidator) => "'{PropertyName}' har en rekke verdier men inneholder ikke '{PropertyValue}'.",
			// Additional fallback messages used by clientside validation integration.
			"Length_Simple" => "'{PropertyName}' skal være mellom {MinLength} og {MaxLength} tegn.",
			"MinimumLength_Simple" => "'{PropertyName}' skal være større enn eller lik {MinLength} tegn.",
			"MaximumLength_Simple" => "'{PropertyName}' skal være mindre enn eller lik {MaxLength} tegn.",
			"ExactLength_Simple" => "'{PropertyName}' skal være {MaxLength} tegn langt.",
			"InclusiveBetween_Simple" => "'{PropertyName}' skal være mellom {From} og {To}.",
			_ => null,
		};
	}
}
