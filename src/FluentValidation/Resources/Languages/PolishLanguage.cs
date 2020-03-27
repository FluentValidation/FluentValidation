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

	internal class PolishLanguage : Language {
		public const string Culture = "pl";
		public override string Name => Culture;

		public PolishLanguage() {
			Translate<EmailValidator>("Pole '{PropertyName}' nie zawiera poprawnego adresu email.");
			Translate<GreaterThanOrEqualValidator>("Wartość pola '{PropertyName}' musi być równa lub większa niż '{ComparisonValue}'.");
			Translate<GreaterThanValidator>("Wartość pola '{PropertyName}' musi być większa niż '{ComparisonValue}'.");
			Translate<LengthValidator>("Długość pola '{PropertyName}' musi się zawierać pomiędzy {MinLength} i {MaxLength} znaki(ów). Wprowadzono {TotalLength} znaki(ów).");
			Translate<MinimumLengthValidator>("Długość pola '{PropertyName}' musi być większa lub równa {MinLength} znaki(ów). Wprowadzono {TotalLength} znaki(ów).");
			Translate<MaximumLengthValidator>("Długość pola '{PropertyName}' musi być mniejszy lub równy {MaxLength} znaki(ów). Wprowadzono {TotalLength} znaki(ów).");
			Translate<LessThanOrEqualValidator>("Wartość pola '{PropertyName}' musi być równa lub mniejsza niż '{ComparisonValue}'.");
			Translate<LessThanValidator>("Wartość pola '{PropertyName}' musi być mniejsza niż '{ComparisonValue}'.");
			Translate<NotEmptyValidator>("Pole '{PropertyName}' nie może być puste.");
			Translate<NotEqualValidator>("Pole '{PropertyName}' nie może być równe '{ComparisonValue}'.");
			Translate<NotNullValidator>("Pole '{PropertyName}' nie może być puste.");
			Translate<PredicateValidator>("Określony warunek nie został spełniony dla pola '{PropertyName}'.");
			Translate<AsyncPredicateValidator>("Określony warunek nie został spełniony dla pola '{PropertyName}'.");
			Translate<RegularExpressionValidator>("'{PropertyName}' wprowadzono w niepoprawnym formacie.");
			Translate<EqualValidator>("Wartość pola '{PropertyName}' musi być równa '{ComparisonValue}'.");
			Translate<ExactLengthValidator>("Pole '{PropertyName}' musi posiadać długość {MaxLength} znaki(ów). Wprowadzono {TotalLength} znaki(ów).");
			Translate<InclusiveBetweenValidator>("Wartość pola '{PropertyName}' musi się zawierać pomiędzy {From} i {To}. Wprowadzono {Value}.");
			Translate<ExclusiveBetweenValidator>("Wartość pola '{PropertyName}' musi się zawierać pomiędzy {From} i {To} (wyłącznie). Wprowadzono {Value}.");
			Translate<CreditCardValidator>("Pole '{PropertyName}' nie zawiera poprawnego numer karty kredytowej.");
			Translate<ScalePrecisionValidator>("Wartość pola '{PropertyName}' nie może mieć więcej niż {ExpectedPrecision} cyfr z dopuszczalną dokładnością {ExpectedScale} cyfr po przecinku. Znaleziono {Digits} cyfr i {ActualScale} cyfr po przecinku.");
			Translate<EmptyValidator>("Pole '{PropertyName}' musi być puste.");
			Translate<NullValidator>("Pole '{PropertyName}' musi być puste.");
			Translate<EnumValidator>("Pole '{PropertyName}' ma zakres wartości, który nie obejmuje {PropertyValue}.");
			// Additional fallback messages used by clientside validation integration.
			Translate("Length_Simple", "Długość pola '{PropertyName}' musi się zawierać pomiędzy {MinLength} i {MaxLength} znaki(ów).");
			Translate("MinimumLength_Simple", "Długość pola '{PropertyName}' musi być większa lub równa {MinLength} znaki(ów).");
			Translate("MaximumLength_Simple", "Długość pola '{PropertyName}' musi być mniejszy lub równy {MaxLength} znaki(ów).");
			Translate("ExactLength_Simple", "Pole '{PropertyName}' musi posiadać długość {MaxLength} znaki(ów).");
			Translate("InclusiveBetween_Simple", "Wartość pola '{PropertyName}' musi się zawierać pomiędzy {From} i {To}.");
		}
	}
}
