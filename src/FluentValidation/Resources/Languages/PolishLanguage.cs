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

	internal class PolishLanguage : Language {
		public override string Name => "pl";

		public PolishLanguage() {
			Translate<EmailValidator>("Pole '{PropertyName}' nie zawiera poprawnego adresu email.");
			Translate<GreaterThanOrEqualValidator>("Wartość pola '{PropertyName}' musi być równa lub większa niż  '{ComparisonValue}'.");
			Translate<GreaterThanValidator>("Wartość pola '{PropertyName}' musi być większa niż '{ComparisonValue}'.");
			Translate<LengthValidator>("Długość pola '{PropertyName}' musi się zawierać pomiędzy {MinLength} i {MaxLength} znaki(ów). Wprowadzono {TotalLength} znaki(ów).");
			Translate<MinimumLengthValidator>("Długość pola \"{PropertyName}\" musi być większa lub równa {MinLength} znaki(ów). Wprowadzono {TotalLength} znaki(ów).");
			Translate<MaximumLengthValidator>("Długość pola \"{PropertyName}\" musi być mniejszy lub równy {MaxLength} znaki(ów). Wprowadzono {TotalLength} znaki(ów).");
			Translate<LessThanOrEqualValidator>("Wartość pola '{PropertyName}' musi być równa lub mniejsza niż '{ComparisonValue}'.");
			Translate<LessThanValidator>("Wartość pola '{PropertyName}' musi być mniejsza niż '{ComparisonValue}'.");
			Translate<NotEmptyValidator>("Pole '{PropertyName}' nie może być puste.");
			Translate<NotEqualValidator>("Pole '{PropertyName}' nie może być równe '{ComparisonValue}'.");
			Translate<NotNullValidator>("Pole '{PropertyName}' nie może być puste.");
			Translate<PredicateValidator>("Okreslony warunek nie został spełniony dla pola '{PropertyName}'.");
			Translate<AsyncPredicateValidator>("Okreslony warunek nie został spełniony dla pola '{PropertyName}'.");
			Translate<RegularExpressionValidator>("'{PropertyName}' wprowadzono w niepoprawnym formacie.");
			Translate<EqualValidator>("Wartość pola '{PropertyName}' powinna być równa '{ComparisonValue}'.");
			Translate<ExactLengthValidator>("Pole '{PropertyName}' musi posiadać długość {MaxLength} znaki(ów). Wprowadzono {TotalLength} znaki(ów).");
			Translate<InclusiveBetweenValidator>("'{PropertyName}' musi się zawierać pomiędzy {From} i {To}. Wprowadzono {Value}.");
			Translate<ExclusiveBetweenValidator>("'{PropertyName}' musi się zawierać pomiędzy {From} i {To} (wyłącznie). Wprowadzono {Value}.");
			Translate<CreditCardValidator>("Pole '{PropertyName}' nie zawiera poprawnego numer karty kredytowej.");
			Translate<ScalePrecisionValidator>("Wartość pola '{PropertyName}' nie może mieć więcej niż {expectedPrecision} cyfr z dopuszczalną dokładnością {expectedScale} cyfr po przecinku. Znaleziono {digits} cyfr i {actualScale} cyfr po przecinku.");
			Translate<EmptyValidator>("\"{PropertyName}\" powinno być puste.");
			Translate<NullValidator>("\"{PropertyName}\" powinno być puste.");
			Translate<EnumValidator>("\"{PropertyName}\" ma zakres wartości, który nie obejmuje {PropertyValue}.");
		}
	}
}