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

	internal class DanishLanguage : Language {
		public override string Name => "da";

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
			Translate<ScalePrecisionValidator>("'{PropertyName}' må ikke være mere end {expectedPrecision} cifre i alt, med hensyn til {expectedScale} decimaler. {digits} cifre og {actualScale} decimaler blev fundet.");
			Translate<EmptyValidator>("'{PropertyName}' skal være tomt.");
			Translate<NullValidator>("'{PropertyName}' skal være tomt.");
			Translate<EnumValidator>("'{PropertyName}' har en række værdier, der ikke indeholder '{PropertyValue}'.");
		}
	}
}