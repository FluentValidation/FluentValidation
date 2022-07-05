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

namespace FluentValidation.Resources;

internal class BulgarianLanguage {
	public const string Culture = "bg";

	public static string GetTranslation(string key) => key switch {
		"EmailValidator" => "'{PropertyName}' не е валиден е-мейл адрес.",
		"GreaterThanOrEqualValidator" => "'{PropertyName}' трябва да бъде по-голямо или равно на  '{ComparisonValue}'.",
		"GreaterThanValidator" => "'{PropertyName}' трябва да бъде по-голямо от '{ComparisonValue}'.",
		"LengthValidator" => "'{PropertyName}' символите трябва да бъдат между {MinLength} и {MaxLength}. Въведохте {TotalLength} знака.",
		"MinimumLengthValidator" => "Дължината на '{PropertyName}' трябва да бъде най-малко {MinLength} брой символи. Въведохте {TotalLength} знака.",
		"MaximumLengthValidator" => "Дължината на '{PropertyName}' трябва да бъде  {MaxLength} брой символи. Въведохте {TotalLength} знака.",
		"LessThanOrEqualValidator" => "'{PropertyName}' трябва да бъде по-малко или равно на '{ComparisonValue}'.",
		"LessThanValidator" => "'{PropertyName}' трябва да бъде по-малко от '{ComparisonValue}'.",
		"NotEmptyValidator" => "'{PropertyName}' не трябва да бъде празно.",
		"NotEqualValidator" => "'{PropertyName}' не трябва да бъде равно на '{ComparisonValue}'.",
		"NotNullValidator" => "'{PropertyName}' не трябва да бъде празно.",
		"PredicateValidator" => "Специалните изисквания за '{PropertyName}' не са спазени.",
		"AsyncPredicateValidator" => "Специалните изисквания за '{PropertyName}' не са спазени.",
		"RegularExpressionValidator" => "'{PropertyName}' не е в правилния формат.",
		"EqualValidator" => "'{PropertyName}' трябва да бъде равно на '{ComparisonValue}'.",
		"ExactLengthValidator" => "'{PropertyName}' трябва да бъде {MaxLength} брой на символите. Въведохте {TotalLength} знака.",
		"InclusiveBetweenValidator" => "'{PropertyName}' трябва да бъде между {From} и {To}. Вие въведохте {PropertyValue}.",
		"ExclusiveBetweenValidator" => "'{PropertyName}' трябва да бъде между {From} и {To} (изключително). Вие въведохте {PropertyValue}.",
		"CreditCardValidator" => "'{PropertyName}' не е валиден номер на кредитна карта.",
		"ScalePrecisionValidator" => "'{PropertyName}' не трябва да е повече от {ExpectedPrecision} цифри и трябва да бъде до {ExpectedScale} знака след запетаята. В момента има {Digits} цифри и {ActualScale} знака след запетаята.",
		"EmptyValidator" => "'{PropertyName}' трябва да бъде празно.",
		"NullValidator" => "'{PropertyName}' трябва да бъде празно.",
		"EnumValidator" => "'{PropertyName}' има диапазон, които не обхващат '{PropertyValue}'.",
		// Additional fallback messages used by clientside validation integration.
		"Length_Simple" => "'{PropertyName}' трябва да бъде межди {MinLength} и {MaxLength} брой символи.",
		"MinimumLength_Simple" => "Дължината на '{PropertyName}' трябва да бъде поне {MinLength} символи.",
		"MaximumLength_Simple" => "Дължината на '{PropertyName}' трябва да бъде {MaxLength} или по-малко брой символи.",
		"ExactLength_Simple" => "'{PropertyName}' трябва да бъде {MaxLength} дължина на символите.",
		"InclusiveBetween_Simple" => "'{PropertyName}' трябва да бъде между {From} и {To}.",
		_ => null,
	};
}
