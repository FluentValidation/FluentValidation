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

	internal class RussianLanguage : Language {
		public const string Culture = "ru";
		public override string Name => Culture;

		public RussianLanguage() {
			Translate<EmailValidator>("'{PropertyName}' неверный email адрес.");
			Translate<GreaterThanOrEqualValidator>("'{PropertyName}' должно быть больше или равно '{ComparisonValue}'.");
			Translate<GreaterThanValidator>("'{PropertyName}' должно быть больше '{ComparisonValue}'.");
			Translate<LengthValidator>("'{PropertyName}' должно быть длиной от {MinLength} до {MaxLength} символов. Вы ввели {TotalLength} символов.");
			Translate<MinimumLengthValidator>("'{PropertyName}' должно быть больше или равно символам {MinLength}. Вы ввели символы {TotalLength}.");
			Translate<MaximumLengthValidator>("'{PropertyName}' должно быть меньше или равно символам {MaxLength}. Вы ввели символы {TotalLength}.");
			Translate<LessThanOrEqualValidator>("'{PropertyName}' должно быть меньше или равно '{ComparisonValue}'.");
			Translate<LessThanValidator>("'{PropertyName}' должно быть меньше '{ComparisonValue}'.");
			Translate<NotEmptyValidator>("'{PropertyName}' не должно быть пусто.");
			Translate<NotEqualValidator>("'{PropertyName}' не должно быть равно '{ComparisonValue}'.");
			Translate<NotNullValidator>("'{PropertyName}' обязано быть непустым.");
			Translate<PredicateValidator>("Указанное условие не было выполнено для '{PropertyName}'.");
			Translate<AsyncPredicateValidator>("Указанное условие не было выполнено для '{PropertyName}'.");
			Translate<RegularExpressionValidator>("'{PropertyName}' имеет неверный формат.");
			Translate<EqualValidator>("'{PropertyName}' должно быть равно '{ComparisonValue}'.");
			Translate<ExactLengthValidator>("'{PropertyName}' должно иметь длину {MaxLength} символа. Вы ввели {TotalLength} символов.");
			Translate<InclusiveBetweenValidator>("'{PropertyName}' Обязано быть от {From} до {To}. Вы ввели {Value}.");
			Translate<ExclusiveBetweenValidator>("'{PropertyName}' должно быть между {From} и {To} (не включая). Вы ввели {Value}.");
			Translate<CreditCardValidator>("'{PropertyName}' неверный номер карты.");
			Translate<ScalePrecisionValidator>("'{PropertyName}' не должно быть более {ExpectedPrecision} цифр всего, с {ExpectedScale} десятичными знаками. {Digits} цифр и {ActualScale} десятичных знаков обнаружено.");
			Translate<EmptyValidator>("'{PropertyName}' должно быть пусто.");
			Translate<NullValidator>("'{PropertyName}' обязано быть пустым.");
			Translate<EnumValidator>("'{PropertyName}' имеет диапазон значений, который не содержит '{PropertyValue}'.");
			// Additional fallback messages used by clientside validation integration.
			Translate("Length_Simple", "'{PropertyName}' должно быть длиной от {MinLength} до {MaxLength} символов.");
			Translate("MinimumLength_Simple", "'{PropertyName}' должно быть больше или равно символам {MinLength}.");
			Translate("MaximumLength_Simple", "'{PropertyName}' должно быть меньше или равно символам {MaxLength}.");
			Translate("ExactLength_Simple", "'{PropertyName}' должно иметь длину {MaxLength} символа.");
			Translate("InclusiveBetween_Simple", "'{PropertyName}' Обязано быть от {From} до {To}.");

		}
	}
}
