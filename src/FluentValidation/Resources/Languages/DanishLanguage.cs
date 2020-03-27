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

	internal class DanishLanguage : Language {
		public const string Culture = "da";
		public override string Name => Culture;

		public DanishLanguage() {
			Translate<EmailValidator>("'{PropertyName}' er ikke en gyldig e-mail-adresse.");
			Translate<GreaterThanOrEqualValidator>("'{PropertyName}' skal være større end eller lig med '{ComparisonValue}'.");
			Translate<GreaterThanValidator>("'{PropertyName}' skal være større end '{ComparisonValue}'.");
			Translate<LengthValidator>("'{PropertyName}' skal være mellem {MinLength} og {MaxLength} tegn. Du har indtastet {TotalLength} tegn.");
			Translate<MinimumLengthValidator>("'{PropertyName}' skal være større end eller lig med {MinLength} tegn. Du indtastede {TotalLength} tegn.");
			Translate<MaximumLengthValidator>("'{PropertyName}' skal være mindre end eller lig med {MaxLength} tegn. Du indtastede {TotalLength} tegn.");
			Translate<LessThanOrEqualValidator>("'{PropertyName}' skal være mindre end eller lig med '{ComparisonValue}'.");
			Translate<LessThanValidator>("'{PropertyName}' skal være mindre end '{ComparisonValue}'.");
			Translate<NotEmptyValidator>("'{PropertyName}' bør ikke være tom.");
			Translate<NotEqualValidator>("'{PropertyName}' bør ikke være lig med '{ComparisonValue}'.");
			Translate<NotNullValidator>("'{PropertyName}' må ikke være tomme.");
			Translate<PredicateValidator>("Den angivne betingelse var ikke opfyldt for '{PropertyName}'.");
			Translate<AsyncPredicateValidator>("Den angivne betingelse var ikke opfyldt for '{PropertyName}'.");
			Translate<RegularExpressionValidator>("'{PropertyName}' er ikke i det rigtige format.");
			Translate<EqualValidator>("'{PropertyName}' skal være lig med '{ComparisonValue}'.");
			Translate<ExactLengthValidator>("'{PropertyName}' skal være {MaxLength} tegn langt. Du har indtastet {TotalLength} tegn.");
			Translate<InclusiveBetweenValidator>("'{PropertyName}' skal være mellem {From} og {To}. Du har indtastet {Value}.");
			Translate<ExclusiveBetweenValidator>("'{PropertyName}' skal være mellem {From} og {To} (eksklusiv). Du har indtastet {Value}.");
			Translate<CreditCardValidator>("'{PropertyName}' er ikke et gyldigt kreditkortnummer.");
			Translate<ScalePrecisionValidator>("'{PropertyName}' må ikke være mere end {ExpectedPrecision} cifre i alt, med hensyn til {ExpectedScale} decimaler. {Digits} cifre og {ActualScale} decimaler blev fundet.");
			Translate<EmptyValidator>("'{PropertyName}' skal være tomt.");
			Translate<NullValidator>("'{PropertyName}' skal være tomt.");
			Translate<EnumValidator>("'{PropertyName}' har en række værdier, der ikke indeholder '{PropertyValue}'.");
			// Additional fallback messages used by clientside validation integration.
			Translate("Length_Simple", "'{PropertyName}' skal være mellem {MinLength} og {MaxLength} tegn.");
			Translate("MinimumLength_Simple", "'{PropertyName}' skal være større end eller lig med {MinLength} tegn.");
			Translate("MaximumLength_Simple", "'{PropertyName}' skal være mindre end eller lig med {MaxLength} tegn.");
			Translate("ExactLength_Simple", "'{PropertyName}' skal være {MaxLength} tegn langt.");
			Translate("InclusiveBetween_Simple", "'{PropertyName}' skal være mellem {From} og {To}.");
		}
	}
}
