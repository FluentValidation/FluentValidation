#region License

// Copyright (c) .NET Foundation and contributors.
//
// Licensed under the Apache License, Version 2.0 (the "License",
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

	internal class UkrainianLanguage {
		public const string Culture = "uk";

		public static string GetTranslation(string key) => key switch {
			nameof(EmailValidator) => "'{PropertyName}' не є email-адресою.",
			nameof(GreaterThanOrEqualValidator) => "'{PropertyName}' має бути більшим, або дорівнювати '{ComparisonValue}'.",
			nameof(GreaterThanValidator) => "'{PropertyName}' має бути більшим за '{ComparisonValue}'.",
			nameof(LengthValidator) => "'{PropertyName}' має бути довжиною від {MinLength} до {MaxLength} символів. Ви ввели {TotalLength} символів.",
			nameof(MinimumLengthValidator) => "Довжина '{PropertyName}' має бути не меншою ніж {MinLength} символів. Ви ввели {TotalLength} символів.",
			nameof(MaximumLengthValidator) => "Довжина '{PropertyName}' має бути {MaxLength} символів, або менше. Ви ввели {TotalLength} символів.",
			nameof(LessThanOrEqualValidator) => "'{PropertyName}' має бути меншим, або дорівнювати '{ComparisonValue}'.",
			nameof(LessThanValidator) => "'{PropertyName}' має бути меншим за '{ComparisonValue}'.",
			nameof(NotEmptyValidator) => "'{PropertyName}' не може бути порожнім.",
			nameof(NotEqualValidator) => "'{PropertyName}' не може дорівнювати '{ComparisonValue}'.",
			nameof(NotNullValidator) => "'{PropertyName}' не може бути порожнім.",
			nameof(PredicateValidator) => "Вказана умова не є задовільною для '{PropertyName}'.",
			nameof(AsyncPredicateValidator) => "Вказана умова не є задовільною для '{PropertyName}'.",
			nameof(RegularExpressionValidator) => "'{PropertyName}' має неправильний формат.",
			nameof(EqualValidator) => "'{PropertyName}' має дорівнювати '{ComparisonValue}'.",
			nameof(ExactLengthValidator) => "'{PropertyName}' має бути довжиною {MaxLength} символів. Ви ввели {TotalLength} символів.",
			nameof(InclusiveBetweenValidator) => "'{PropertyName}' має бути між {From} та {To} (включно). Ви ввели {Value}.",
			nameof(ExclusiveBetweenValidator) => "'{PropertyName}' має бути між {From} та {To}. Ви ввели {Value}.",
			nameof(CreditCardValidator) => "'{PropertyName}' не є номером кредитної картки.",
			nameof(ScalePrecisionValidator) => "'{PropertyName}' не може мати більше за {ExpectedPrecision} цифр всього, з {ExpectedScale} десятковими знаками. {Digits} цифр та {ActualScale} десяткових знаків знайдено.",
			nameof(EmptyValidator) => "'{PropertyName}' має бути порожнім.",
			nameof(NullValidator) => "'{PropertyName}' має бути порожнім.",
			nameof(EnumValidator) => "'{PropertyName}' має діапазон значень, який не включає '{PropertyValue}'.",
			// Additional fallback messages used by clientside validation integration.
			"Length_Simple" => "'{PropertyName}' має бути довжиною від {MinLength} до {MaxLength} символів.",
			"MinimumLength_Simple" => "Довжина '{PropertyName}' має бути не меншою ніж {MinLength} символів.",
			"MaximumLength_Simple" => "Довжина '{PropertyName}' має бути {MaxLength} символів, або менше.",
			"ExactLength_Simple" => "'{PropertyName}' має бути довжиною {MaxLength} символів.",
			"InclusiveBetween_Simple" => "'{PropertyName}' має бути між {From} та {To} (включно).",
			_ => null,
		};
	}
}
