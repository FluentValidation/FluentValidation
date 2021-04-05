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

	internal class RussianLanguage {
		public const string Culture = "ru";

		public static string GetTranslation(string key) => key switch {
			"EmailValidator" => "'{PropertyName}' неверный email адрес.",
			"GreaterThanOrEqualValidator" => "'{PropertyName}' должно быть больше или равно '{ComparisonValue}'.",
			"GreaterThanValidator" => "'{PropertyName}' должно быть больше '{ComparisonValue}'.",
			"LengthValidator" => "'{PropertyName}' должно быть длиной от {MinLength} до {MaxLength} символов. Вы ввели {TotalLength} символов.",
			"MinimumLengthValidator" => "'{PropertyName}' должно быть больше или равно символам {MinLength}. Вы ввели символы {TotalLength}.",
			"MaximumLengthValidator" => "'{PropertyName}' должно быть меньше или равно символам {MaxLength}. Вы ввели символы {TotalLength}.",
			"LessThanOrEqualValidator" => "'{PropertyName}' должно быть меньше или равно '{ComparisonValue}'.",
			"LessThanValidator" => "'{PropertyName}' должно быть меньше '{ComparisonValue}'.",
			"NotEmptyValidator" => "'{PropertyName}' не должно быть пусто.",
			"NotEqualValidator" => "'{PropertyName}' не должно быть равно '{ComparisonValue}'.",
			"NotNullValidator" => "'{PropertyName}' обязано быть непустым.",
			"PredicateValidator" => "Указанное условие не было выполнено для '{PropertyName}'.",
			"AsyncPredicateValidator" => "Указанное условие не было выполнено для '{PropertyName}'.",
			"RegularExpressionValidator" => "'{PropertyName}' имеет неверный формат.",
			"EqualValidator" => "'{PropertyName}' должно быть равно '{ComparisonValue}'.",
			"ExactLengthValidator" => "'{PropertyName}' должно иметь длину {MaxLength} символа. Вы ввели {TotalLength} символов.",
			"InclusiveBetweenValidator" => "'{PropertyName}' Обязано быть от {From} до {To}. Вы ввели {PropertyValue}.",
			"ExclusiveBetweenValidator" => "'{PropertyName}' должно быть между {From} и {To} (не включая). Вы ввели {PropertyValue}.",
			"CreditCardValidator" => "'{PropertyName}' неверный номер карты.",
			"ScalePrecisionValidator" => "'{PropertyName}' не должно быть более {ExpectedPrecision} цифр всего, с {ExpectedScale} десятичными знаками. {Digits} цифр и {ActualScale} десятичных знаков обнаружено.",
			"EmptyValidator" => "'{PropertyName}' должно быть пусто.",
			"NullValidator" => "'{PropertyName}' обязано быть пустым.",
			"EnumValidator" => "'{PropertyName}' имеет диапазон значений, который не содержит '{PropertyValue}'.",
			// Additional fallback messages used by clientside validation integration.
			"Length_Simple" => "'{PropertyName}' должно быть длиной от {MinLength} до {MaxLength} символов.",
			"MinimumLength_Simple" => "'{PropertyName}' должно быть больше или равно символам {MinLength}.",
			"MaximumLength_Simple" => "'{PropertyName}' должно быть меньше или равно символам {MaxLength}.",
			"ExactLength_Simple" => "'{PropertyName}' должно иметь длину {MaxLength} символа.",
			"InclusiveBetween_Simple" => "'{PropertyName}' Обязано быть от {From} до {To}.",

			_ => null,
		};
	}
}
