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
			nameof(EmailValidator) => "Pole '{PropertyName}' nie zawiera poprawnego adresu email.",
			nameof(GreaterThanOrEqualValidator) => "Wartość pola '{PropertyName}' musi być równa lub większa niż '{ComparisonValue}'.",
			nameof(GreaterThanValidator) => "Wartość pola '{PropertyName}' musi być większa niż '{ComparisonValue}'.",
			nameof(LengthValidator) => "Długość pola '{PropertyName}' musi się zawierać pomiędzy {MinLength} i {MaxLength} znaki(ów). Wprowadzono {TotalLength} znaki(ów).",
			nameof(MinimumLengthValidator) => "Długość pola '{PropertyName}' musi być większa lub równa {MinLength} znaki(ów). Wprowadzono {TotalLength} znaki(ów).",
			nameof(MaximumLengthValidator) => "Długość pola '{PropertyName}' musi być mniejszy lub równy {MaxLength} znaki(ów). Wprowadzono {TotalLength} znaki(ów).",
			nameof(LessThanOrEqualValidator) => "Wartość pola '{PropertyName}' musi być równa lub mniejsza niż '{ComparisonValue}'.",
			nameof(LessThanValidator) => "Wartość pola '{PropertyName}' musi być mniejsza niż '{ComparisonValue}'.",
			nameof(NotEmptyValidator) => "Pole '{PropertyName}' nie może być puste.",
			nameof(NotEqualValidator) => "Pole '{PropertyName}' nie może być równe '{ComparisonValue}'.",
			nameof(NotNullValidator) => "Pole '{PropertyName}' nie może być puste.",
			nameof(PredicateValidator) => "Określony warunek nie został spełniony dla pola '{PropertyName}'.",
			nameof(AsyncPredicateValidator) => "Określony warunek nie został spełniony dla pola '{PropertyName}'.",
			nameof(RegularExpressionValidator) => "'{PropertyName}' wprowadzono w niepoprawnym formacie.",
			nameof(EqualValidator) => "Wartość pola '{PropertyName}' musi być równa '{ComparisonValue}'.",
			nameof(ExactLengthValidator) => "Pole '{PropertyName}' musi posiadać długość {MaxLength} znaki(ów). Wprowadzono {TotalLength} znaki(ów).",
			nameof(InclusiveBetweenValidator) => "Wartość pola '{PropertyName}' musi się zawierać pomiędzy {From} i {To}. Wprowadzono {Value}.",
			nameof(ExclusiveBetweenValidator) => "Wartość pola '{PropertyName}' musi się zawierać pomiędzy {From} i {To} (wyłącznie). Wprowadzono {Value}.",
			nameof(CreditCardValidator) => "Pole '{PropertyName}' nie zawiera poprawnego numer karty kredytowej.",
			nameof(ScalePrecisionValidator) => "Wartość pola '{PropertyName}' nie może mieć więcej niż {ExpectedPrecision} cyfr z dopuszczalną dokładnością {ExpectedScale} cyfr po przecinku. Znaleziono {Digits} cyfr i {ActualScale} cyfr po przecinku.",
			nameof(EmptyValidator) => "Pole '{PropertyName}' musi być puste.",
			nameof(NullValidator) => "Pole '{PropertyName}' musi być puste.",
			nameof(EnumValidator) => "Pole '{PropertyName}' ma zakres wartości, który nie obejmuje {PropertyValue}.",
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
