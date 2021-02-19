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

	internal class PolishLanguage {
		public const string Culture = "pl";

		public static string GetTranslation(string key) => key switch {
			"EmailValidator" => "Pole '{PropertyName}' nie zawiera poprawnego adresu email.",
			"GreaterThanOrEqualValidator" => "Wartość pola '{PropertyName}' musi być równa lub większa niż '{ComparisonValue}'.",
			"GreaterThanValidator" => "Wartość pola '{PropertyName}' musi być większa niż '{ComparisonValue}'.",
			"LengthValidator" => "Długość pola '{PropertyName}' musi się zawierać pomiędzy {MinLength} i {MaxLength} znaki(ów). Wprowadzono {TotalLength} znaki(ów).",
			"MinimumLengthValidator" => "Długość pola '{PropertyName}' musi być większa lub równa {MinLength} znaki(ów). Wprowadzono {TotalLength} znaki(ów).",
			"MaximumLengthValidator" => "Długość pola '{PropertyName}' musi być mniejszy lub równy {MaxLength} znaki(ów). Wprowadzono {TotalLength} znaki(ów).",
			"LessThanOrEqualValidator" => "Wartość pola '{PropertyName}' musi być równa lub mniejsza niż '{ComparisonValue}'.",
			"LessThanValidator" => "Wartość pola '{PropertyName}' musi być mniejsza niż '{ComparisonValue}'.",
			"NotEmptyValidator" => "Pole '{PropertyName}' nie może być puste.",
			"NotEqualValidator" => "Pole '{PropertyName}' nie może być równe '{ComparisonValue}'.",
			"NotNullValidator" => "Pole '{PropertyName}' nie może być puste.",
			"PredicateValidator" => "Określony warunek nie został spełniony dla pola '{PropertyName}'.",
			"AsyncPredicateValidator" => "Określony warunek nie został spełniony dla pola '{PropertyName}'.",
			"RegularExpressionValidator" => "'{PropertyName}' wprowadzono w niepoprawnym formacie.",
			"EqualValidator" => "Wartość pola '{PropertyName}' musi być równa '{ComparisonValue}'.",
			"ExactLengthValidator" => "Pole '{PropertyName}' musi posiadać długość {MaxLength} znaki(ów). Wprowadzono {TotalLength} znaki(ów).",
			"InclusiveBetweenValidator" => "Wartość pola '{PropertyName}' musi się zawierać pomiędzy {From} i {To}. Wprowadzono {PropertyValue}.",
			"ExclusiveBetweenValidator" => "Wartość pola '{PropertyName}' musi się zawierać pomiędzy {From} i {To} (wyłącznie). Wprowadzono {PropertyValue}.",
			"CreditCardValidator" => "Pole '{PropertyName}' nie zawiera poprawnego numer karty kredytowej.",
			"ScalePrecisionValidator" => "Wartość pola '{PropertyName}' nie może mieć więcej niż {ExpectedPrecision} cyfr z dopuszczalną dokładnością {ExpectedScale} cyfr po przecinku. Znaleziono {Digits} cyfr i {ActualScale} cyfr po przecinku.",
			"EmptyValidator" => "Pole '{PropertyName}' musi być puste.",
			"NullValidator" => "Pole '{PropertyName}' musi być puste.",
			"EnumValidator" => "Pole '{PropertyName}' ma zakres wartości, który nie obejmuje {PropertyValue}.",
			// Additional fallback messages used by clientside validation integration.
			"Length_Simple" => "Długość pola '{PropertyName}' musi się zawierać pomiędzy {MinLength} i {MaxLength} znaki(ów).",
			"MinimumLength_Simple" => "Długość pola '{PropertyName}' musi być większa lub równa {MinLength} znaki(ów).",
			"MaximumLength_Simple" => "Długość pola '{PropertyName}' musi być mniejszy lub równy {MaxLength} znaki(ów).",
			"ExactLength_Simple" => "Pole '{PropertyName}' musi posiadać długość {MaxLength} znaki(ów).",
			"InclusiveBetween_Simple" => "Wartość pola '{PropertyName}' musi się zawierać pomiędzy {From} i {To}.",
			_ => null,
		};
	}
}
