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

namespace FluentValidation.Resources {
	using Validators;

	internal class NorwegianBokmalLanguage : Language {
		public const string Culture = "nb";
		public override string Name => Culture;

		public NorwegianBokmalLanguage() {
			Translate<EmailValidator>("'{PropertyName}' er ikke en gyldig e-postadresse.");
			Translate<GreaterThanOrEqualValidator>("'{PropertyName}' skal være større enn eller lik '{ComparisonValue}'.");
			Translate<GreaterThanValidator>("'{PropertyName}' skal være større enn '{ComparisonValue}'.");
			Translate<LengthValidator>("'{PropertyName}' skal være mellom {MinLength} og {MaxLength} tegn. Du har tastet inn {TotalLength} tegn.");
			Translate<MinimumLengthValidator>("'{PropertyName}' skal være større enn eller lik {MinLength} tegn. Du tastet inn {TotalLength} tegn.");
			Translate<MaximumLengthValidator>("'{PropertyName}' skal være mindre enn eller lik {MaxLength} tegn. Du tastet inn {TotalLength} tegn.");
			Translate<LessThanOrEqualValidator>("'{PropertyName}' skal være mindre enn eller lik '{ComparisonValue}'.");
			Translate<LessThanValidator>("'{PropertyName}' skal være mindre enn '{ComparisonValue}'.");
			Translate<NotEmptyValidator>("'{PropertyName}' kan ikke være tom.");
			Translate<NotEqualValidator>("'{PropertyName}' kan ikke være lik med '{ComparisonValue}'.");
			Translate<NotNullValidator>("'{PropertyName}' kan ikke være tom.");
			Translate<PredicateValidator>("Den angitte betingelsen var ikke oppfylt for '{PropertyName}'.");
			Translate<AsyncPredicateValidator>("Den angitte betingelsen var ikke oppfylt for '{PropertyName}'.");
			Translate<RegularExpressionValidator>("'{PropertyName}' har ikke riktig format.");
			Translate<EqualValidator>("'{PropertyName}' skal være lik med '{ComparisonValue}'.");
			Translate<ExactLengthValidator>("'{PropertyName}' skal være {MaxLength} tegn langt. Du har tastet inn {TotalLength} tegn.");
			Translate<InclusiveBetweenValidator>("'{PropertyName}' skal være mellom {From} og {To}. Du har tastet inn {Value}.");
			Translate<ExclusiveBetweenValidator>("'{PropertyName}' skal være mellom {From} og {To} (eksklusiv). Du har tastet inn {Value}.");
			Translate<CreditCardValidator>("'{PropertyName}' er ikke et gyldig kredittkortnummer.");
			Translate<ScalePrecisionValidator>("'{PropertyName}' kan ikke være mer enn {ExpectedPrecision} siffer totalt, med hensyn til {ExpectedScale} desimaler. {Digits} siffer og {ActualScale} desimaler ble funnet.");
			Translate<EmptyValidator>("'{PropertyName}' skal være tomt.");
			Translate<NullValidator>("'{PropertyName}' skal være tomt.");
			Translate<EnumValidator>("'{PropertyName}' har en rekke verdier men inneholder ikke '{PropertyValue}'.");
			// Additional fallback messages used by clientside validation integration.
			Translate("Length_Simple", "'{PropertyName}' skal være mellom {MinLength} og {MaxLength} tegn.");
			Translate("MinimumLength_Simple", "'{PropertyName}' skal være større enn eller lik {MinLength} tegn.");
			Translate("MaximumLength_Simple", "'{PropertyName}' skal være mindre enn eller lik {MaxLength} tegn.");
			Translate("ExactLength_Simple", "'{PropertyName}' skal være {MaxLength} tegn langt.");
			Translate("InclusiveBetween_Simple", "'{PropertyName}' skal være mellom {From} og {To}.");
		}
	}
}
