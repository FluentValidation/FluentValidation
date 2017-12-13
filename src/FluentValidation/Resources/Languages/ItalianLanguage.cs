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

	internal class ItalianLanguage : Language {
		public override string Name => "it";

		public ItalianLanguage() {
			Translate<EmailValidator>("'{PropertyName}' non è un indirizzo email valido.");
			Translate<EqualValidator>("'{PropertyName}' dovrebbe essere uguale a '{ComparisonValue}'.");
			Translate<ExactLengthValidator>("'{PropertyName}' deve essere lungo {MaxLength} caratteri. Hai inserito {TotalLength} caratteri.");
			Translate<ExclusiveBetweenValidator>("'{PropertyName}' deve essere compreso tra {From} e {To} (esclusi). Hai inserito {Value}.");
			Translate<GreaterThanOrEqualValidator>("'{PropertyName}' deve essere maggiore o uguale a '{ComparisonValue}'.");
			Translate<GreaterThanValidator>("'{PropertyName}' deve essere maggiore di '{ComparisonValue}'.");
			Translate<InclusiveBetweenValidator>("'{PropertyName}' deve essere compreso tra {From} e {To}. Hai inserito {Value}.");
			Translate<LengthValidator>("'{PropertyName}' deve essere lungo tra i {MinLength} e {MaxLength} caratteri. Hai inserito {TotalLength} caratteri.");
			Translate<MinimumLengthValidator>("'{PropertyName}' deve essere maggiore o uguale a {MinLength} caratteri. Hai inserito i caratteri {TotalLength}.");
			Translate<MaximumLengthValidator>("'{PropertyName}' deve essere minore o uguale a {MaxLength} caratteri. Hai inserito i caratteri {TotalLength}.");
			Translate<LessThanOrEqualValidator>("'{PropertyName}' deve essere minore o uguale a '{ComparisonValue}'.");
			Translate<LessThanValidator>("'{PropertyName}' deve essere minore di '{ComparisonValue}'.");
			Translate<NotEmptyValidator>("'{PropertyName}' non può essere vuoto.");
			Translate<NotEqualValidator>("'{PropertyName}' non puo essere uguale a '{ComparisonValue}'.");
			Translate<NotNullValidator>("'{PropertyName}' non puo essere vuoto.");
			Translate<PredicateValidator>("La condizione non è verificata per '{PropertyName}'.");
			Translate<AsyncPredicateValidator>("La condizione non è verificata per '{PropertyName}'.");
			Translate<RegularExpressionValidator>("'{PropertyName}' non è nel formato corretto.");
			Translate<CreditCardValidator>("'{PropertyName}' non è un numero di carta di credito valido.");
			Translate<ScalePrecisionValidator>("'{PropertyName}' potrebbe non avere più di {expectedPrecision} cifre in totale, con una tolleranza per decimali {expectedScale}. Sono stati trovati i decimali {digits} e i decimali {actualScale}.");
			Translate<EmptyValidator>("'{PropertyName}' dovrebbe essere vuoto.");
			Translate<NullValidator>("'{PropertyName}' dovrebbe essere vuoto.");
			Translate<EnumValidator>("'{PropertyName}' ha un intervallo di valori che non include '{PropertyValue}'.");
		}
	}
}