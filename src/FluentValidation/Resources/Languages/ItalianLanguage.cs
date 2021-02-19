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

	internal class ItalianLanguage {
		public const string Culture = "it";

		public static string GetTranslation(string key) => key switch {
			"EmailValidator" => "'{PropertyName}' non è un indirizzo email valido.",
			"EqualValidator" => "'{PropertyName}' dovrebbe essere uguale a '{ComparisonValue}'.",
			"ExactLengthValidator" => "'{PropertyName}' deve essere lungo {MaxLength} caratteri. Hai inserito {TotalLength} caratteri.",
			"ExclusiveBetweenValidator" => "'{PropertyName}' deve essere compreso tra {From} e {To} (esclusi). Hai inserito {PropertyValue}.",
			"GreaterThanOrEqualValidator" => "'{PropertyName}' deve essere maggiore o uguale a '{ComparisonValue}'.",
			"GreaterThanValidator" => "'{PropertyName}' deve essere maggiore di '{ComparisonValue}'.",
			"InclusiveBetweenValidator" => "'{PropertyName}' deve essere compreso tra {From} e {To}. Hai inserito {PropertyValue}.",
			"LengthValidator" => "'{PropertyName}' deve essere lungo tra i {MinLength} e {MaxLength} caratteri. Hai inserito {TotalLength} caratteri.",
			"MinimumLengthValidator" => "'{PropertyName}' deve essere maggiore o uguale a {MinLength} caratteri. Hai inserito {TotalLength} caratteri.",
			"MaximumLengthValidator" => "'{PropertyName}' deve essere minore o uguale a {MaxLength} caratteri. Hai inserito {TotalLength} caratteri.",
			"LessThanOrEqualValidator" => "'{PropertyName}' deve essere minore o uguale a '{ComparisonValue}'.",
			"LessThanValidator" => "'{PropertyName}' deve essere minore di '{ComparisonValue}'.",
			"NotEmptyValidator" => "'{PropertyName}' non può essere vuoto.",
			"NotEqualValidator" => "'{PropertyName}' non può essere uguale a '{ComparisonValue}'.",
			"NotNullValidator" => "'{PropertyName}' non può essere vuoto.",
			"PredicateValidator" => "La condizione non è verificata per '{PropertyName}'.",
			"AsyncPredicateValidator" => "La condizione non è verificata per '{PropertyName}'.",
			"RegularExpressionValidator" => "'{PropertyName}' non è nel formato corretto.",
			"CreditCardValidator" => "'{PropertyName}' non è un numero di carta di credito valido.",
			"ScalePrecisionValidator" => "'{PropertyName}' potrebbe non avere più di {ExpectedPrecision} cifre in totale, con una tolleranza per {ExpectedScale} decimali. Sono stati trovate {Digits} cifre e {ActualScale} decimali.",
			"EmptyValidator" => "'{PropertyName}' dovrebbe essere vuoto.",
			"NullValidator" => "'{PropertyName}' dovrebbe essere vuoto.",
			"EnumValidator" => "'{PropertyName}' ha un intervallo di valori che non include '{PropertyValue}'.",
			// Additional fallback messages used by clientside validation integration.
			"ExactLength_Simple" => "'{PropertyName}' deve essere lungo {MaxLength} caratteri.",
			"InclusiveBetween_Simple" => "'{PropertyName}' deve essere compreso tra {From} e {To}.",
			"Length_Simple" => "'{PropertyName}' deve essere lungo tra i {MinLength} e {MaxLength} caratteri.",
			"MinimumLength_Simple" => "'{PropertyName}' deve essere maggiore o uguale a {MinLength} caratteri.",
			"MaximumLength_Simple" => "'{PropertyName}' deve essere minore o uguale a {MaxLength} caratteri.",
			_ => null,
		};
	}
}
