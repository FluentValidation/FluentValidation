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

	internal class ItalianLanguage : Language {
		public const string Culture = "it";
		public override string Name => Culture;

		public ItalianLanguage() {
			Translate<EmailValidator>("'{PropertyName}' non è un indirizzo email valido.");
			Translate<EqualValidator>("'{PropertyName}' dovrebbe essere uguale a '{ComparisonValue}'.");
			Translate<ExactLengthValidator>("'{PropertyName}' deve essere lungo {MaxLength} caratteri. Hai inserito {TotalLength} caratteri.");
			Translate<ExclusiveBetweenValidator>("'{PropertyName}' deve essere compreso tra {From} e {To} (esclusi). Hai inserito {Value}.");
			Translate<GreaterThanOrEqualValidator>("'{PropertyName}' deve essere maggiore o uguale a '{ComparisonValue}'.");
			Translate<GreaterThanValidator>("'{PropertyName}' deve essere maggiore di '{ComparisonValue}'.");
			Translate<InclusiveBetweenValidator>("'{PropertyName}' deve essere compreso tra {From} e {To}. Hai inserito {Value}.");
			Translate<LengthValidator>("'{PropertyName}' deve essere lungo tra i {MinLength} e {MaxLength} caratteri. Hai inserito {TotalLength} caratteri.");
			Translate<MinimumLengthValidator>("'{PropertyName}' deve essere maggiore o uguale a {MinLength} caratteri. Hai inserito {TotalLength} caratteri.");
			Translate<MaximumLengthValidator>("'{PropertyName}' deve essere minore o uguale a {MaxLength} caratteri. Hai inserito {TotalLength} caratteri.");
			Translate<LessThanOrEqualValidator>("'{PropertyName}' deve essere minore o uguale a '{ComparisonValue}'.");
			Translate<LessThanValidator>("'{PropertyName}' deve essere minore di '{ComparisonValue}'.");
			Translate<NotEmptyValidator>("'{PropertyName}' non può essere vuoto.");
			Translate<NotEqualValidator>("'{PropertyName}' non può essere uguale a '{ComparisonValue}'.");
			Translate<NotNullValidator>("'{PropertyName}' non può essere vuoto.");
			Translate<PredicateValidator>("La condizione non è verificata per '{PropertyName}'.");
			Translate<AsyncPredicateValidator>("La condizione non è verificata per '{PropertyName}'.");
			Translate<RegularExpressionValidator>("'{PropertyName}' non è nel formato corretto.");
			Translate<CreditCardValidator>("'{PropertyName}' non è un numero di carta di credito valido.");
			Translate<ScalePrecisionValidator>("'{PropertyName}' potrebbe non avere più di {ExpectedPrecision} cifre in totale, con una tolleranza per {ExpectedScale} decimali. Sono stati trovate {Digits} cifre e {ActualScale} decimali.");
			Translate<EmptyValidator>("'{PropertyName}' dovrebbe essere vuoto.");
			Translate<NullValidator>("'{PropertyName}' dovrebbe essere vuoto.");
			Translate<EnumValidator>("'{PropertyName}' ha un intervallo di valori che non include '{PropertyValue}'.");
			// Additional fallback messages used by clientside validation integration.
			Translate("ExactLength_Simple", "'{PropertyName}' deve essere lungo {MaxLength} caratteri.");
			Translate("InclusiveBetween_Simple", "'{PropertyName}' deve essere compreso tra {From} e {To}.");
			Translate("Length_Simple", "'{PropertyName}' deve essere lungo tra i {MinLength} e {MaxLength} caratteri.");
			Translate("MinimumLength_Simple", "'{PropertyName}' deve essere maggiore o uguale a {MinLength} caratteri.");
			Translate("MaximumLength_Simple", "'{PropertyName}' deve essere minore o uguale a {MaxLength} caratteri.");
		}
	}
}
