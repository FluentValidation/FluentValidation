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

internal class BelarusianLanguage {
	public const string Culture = "be";

	public static string GetTranslation(string key) => key switch {
		"EmailValidator" => "'{PropertyName}' некарэктны email адрас.",
		"GreaterThanOrEqualValidator" => "'{PropertyName}' павінна быць больш або роўна '{ComparisonValue}'.",
		"GreaterThanValidator" => "'{PropertyName}' павінна быць больш '{ComparisonValue}'.",
		"LengthValidator" => "'{PropertyName}' павінна мець даўжыню ад {MinLength} да {MaxLength} сімвалаў. Колькасць уведзеных сімвалаў: {TotalLength}.",
		"MinimumLengthValidator" => "'{PropertyName}' павінна мець даўжыню не менш за {MinLength} сімвалаў. Колькасць уведзеных сімвалаў: {TotalLength}.",
		"MaximumLengthValidator" => "'{PropertyName}' павінна мець даўжыню не больш за {MaxLength} сімвалаў. Колькасць уведзеных сімвалаў: {TotalLength}.",
		"LessThanOrEqualValidator" => "'{PropertyName}' павінна быць менш або роўна '{ComparisonValue}'.",
		"LessThanValidator" => "'{PropertyName}' павінна быць менш '{ComparisonValue}'.",
		"NotEmptyValidator" => "'{PropertyName}' павінна быць запоўнена.",
		"NotEqualValidator" => "'{PropertyName}' не павінна быць аднолькава з '{ComparisonValue}'.",
		"NotNullValidator" => "'{PropertyName}' павінна быць вызначана.",
		"PredicateValidator" => "Не выканана вызначаная ўмова для '{PropertyName}'.",
		"AsyncPredicateValidator" => "Не выканана вызначаная ўмова для '{PropertyName}'.",
		"RegularExpressionValidator" => "'{PropertyName}' не адпавядае вызначанаму фармату.",
		"EqualValidator" => "'{PropertyName}' павінна быць аднолькава з '{ComparisonValue}'.",
		"ExactLengthValidator" => "'{PropertyName}' павінна мець даўжыню {MaxLength} сімвала(ў). Колькасць уведзеных сімвалаў: {TotalLength}.",
		"InclusiveBetweenValidator" => "'{PropertyName}' павінна быць у дыяпазоне ад {From} да {To}. Уведзенае значэнне: {PropertyValue}.",
		"ExclusiveBetweenValidator" => "'{PropertyName}' павінна быць у дыяпазоне ад {From} да {To} (не ўключаючы гэтыя значэнні). Уведзенае значэнне: {PropertyValue}.",
		"CreditCardValidator" => "'{PropertyName}' некарэктны нумар карты.",
		"ScalePrecisionValidator" => "'{PropertyName}' павінна ўтрымліваць не больш за {ExpectedPrecision} лічбаў усяго, у тым ліку {ExpectedScale} дзесятковых знакаў. Уведзенае значэнне ўтрымлівае {Digits} лічбаў і {ActualScale} дзесятковых знакаў.",
		"EmptyValidator" => "'{PropertyName}' павінна быць пустым.",
		"NullValidator" => "'{PropertyName}' павінна быць не вызначана.",
		"EnumValidator" => "'{PropertyName}' утрымлівае недапушчальнае значэнне '{PropertyValue}'.",
		// Additional fallback messages used by clientside validation integration.
		"Length_Simple" => "'{PropertyName}' павінна мець даўжыню ад {MinLength} да {MaxLength} сімвалаў.",
		"MinimumLength_Simple" => "'{PropertyName}' павінна мець даўжыню не менш за {MinLength} сімвалаў.",
		"MaximumLength_Simple" => "'{PropertyName}' павінна мець даўжыню не больш за {MaxLength} сімвалаў.",
		"ExactLength_Simple" => "'{PropertyName}' павінна мець даўжыню {MaxLength} сімвала(ў).",
		"InclusiveBetween_Simple" => "'{PropertyName}' павінна быць у дыяпазоне ад {From} да {To}.",
		_ => null,
	};
}
