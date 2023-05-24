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

internal class TajikLanguage {
	public const string Culture = "tg";

	public static string GetTranslation(string key) => key switch {
		"EmailValidator" => "'{PropertyName}' суроғаи почтаи электронии дуруст нест.",
		"GreaterThanOrEqualValidator" => "'{PropertyName}' бояд аз '{ComparisonValue}' бузургтар ё баробар бошад.",
		"GreaterThanValidator" => "'{PropertyName}' бояд аз '{ComparisonValue}' бузургтар бошад.",
		"LengthValidator" => "'{PropertyName}' бояд дар байни аломатҳои {MinLength} ва {MaxLength} бошад. Миқдори аломатҳои воридшуда: {TotalLength}",
		"MinimumLengthValidator" => "Дарозии '{PropertyName}' бояд ҳадди аққал {MinLength} аломат бошад. Миқдори аломатҳои воридшуда: {TotalLength}",
		"MaximumLengthValidator" => "Дарозии '{PropertyName}' бояд {MaxLength} аломат ё камтар бошад. Миқдори аломатҳои воридшуда: {TotalLength}",
		"LessThanOrEqualValidator" => "'{PropertyName}' бояд аз '{ComparisonValue}' камтар ё баробар бошад.",
		"LessThanValidator" => "'{PropertyName}' бояд камтар аз '{ComparisonValue}' бошад.",
		"NotEmptyValidator" => "'{PropertyName}' набояд холӣ бошад.",
		"NotEqualValidator" => "'{PropertyName}' набояд ба '{ComparisonValue}' баробар бошад.",
		"NotNullValidator" => "'{PropertyName}' набояд холӣ бошад.",
		"PredicateValidator" => "Шарти муайяншуда барои '{PropertyName}' иҷро нашуд.",
		"AsyncPredicateValidator" => "Шарти муайяншуда барои '{PropertyName}' иҷро нашуд.",
		"RegularExpressionValidator" => "'{PropertyName}' дар формати дуруст нест.",
		"EqualValidator" => "'{PropertyName}' бояд ба '{ComparisonValue}' баробар бошад.",
		"ExactLengthValidator" => "'{PropertyName}' бояд дарозии {MaxLength} аломат бошад. Миқдори аломатҳои воридшуда: {TotalLength}",
		"InclusiveBetweenValidator" => "'{PropertyName}' бояд байни {From} ва {To} бошад. Шумо {PropertyValue}-ро ворид кардаед.",
		"ExclusiveBetweenValidator" => "'{PropertyName}' бояд байни {From} ва {To} (ба истиснои ҳаминҳо) бошад. Шумо {PropertyValue}-ро ворид кардаед.",
		"CreditCardValidator" => "'{PropertyName}' рақами дурусти корти кредитӣ нест.",
		"ScalePrecisionValidator" => "'{PropertyName}' набояд дар маҷмӯъ аз рақамҳои {ExpectedPrecision} зиёд бошад, бо назардошти он барои даҳҳо {ExpectedScale}. {Digits} рақам ва {ActualScale} даҳӣ ёфт шуданд.",
		"EmptyValidator" => "'{PropertyName}' бояд холӣ бошад.",
		"NullValidator" => "'{PropertyName}' бояд холӣ бошад.",
		"EnumValidator" => "'{PropertyName}' як қатор арзишҳо дорад, ки '{PropertyValue}'-ро дар бар намегирад.",
		// Additional fallback messages used by clientside validation integration.
		"Length_Simple" => "'{PropertyName}' бояд дар байни {MinLength} ва {MaxLength} аломат бошад.",
		"MinimumLength_Simple" => "Дарозии '{PropertyName}' бояд ҳадди аққал {MinLength} аломат бошад.",
		"MaximumLength_Simple" => "Дарозии '{PropertyName}' бояд {MaxLength} аломат ё камтар бошад.",
		"ExactLength_Simple" => "'{PropertyName}' бояд дарозии {MaxLength} аломат бошад.",
		"InclusiveBetween_Simple" => "'{PropertyName}' бояд байни {From} ва {To} бошад.",
		_ => null,
	};
}
