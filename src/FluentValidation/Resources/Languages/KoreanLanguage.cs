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

	internal class KoreanLanguage {
		public const string Culture = "ko";

		public static string GetTranslation(string key) => key switch {
			"CreditCardValidator" => "'{PropertyName}'이(가) 올바른 신용카드 번호가 아닙니다.",
			"EmailValidator" => "'{PropertyName}'이(가) 올바른 이메일 주소가 아닙니다.",
			"EqualValidator" => "'{PropertyName}'은(는) '{ComparisonValue}'이어야 합니다.",
			"ExactLengthValidator" => "'{PropertyName}'은(는) {MaxLength} 글자이하의 문자열이어야 합니다. 입력한 문자열은 {TotalLength} 글자 입니다.",
			"ExclusiveBetweenValidator" => "'{PropertyName}'은(는) {From} 이상 {To} 미만이어야 합니다. 입력한 값은 {PropertyValue}입니다.",
			"GreaterThanOrEqualValidator" => "'{PropertyName}'은(는) '{ComparisonValue}'이상이어야 합니다.",
			"GreaterThanValidator" => "'{PropertyName}'은(는) '{ComparisonValue}'보다 커야 합니다.",
			"InclusiveBetweenValidator" => "'{PropertyName}'은(는) {From} 이상 {To} 이하여야 합니다. 입력한 값은 {PropertyValue}입니다.",
			"LengthValidator" => "'{PropertyName}'은(는) {MinLength} 글자 이상 {MaxLength} 글자 이하여야 합니다. 입력한 문자열은 {TotalLength} 글자입니다.",
			"MinimumLengthValidator" => "'{PropertyName}'은 {MinLength} 자 이상의 값이어야합니다. {TotalLength} 문자를 입력했습니다.",
			"MaximumLengthValidator" => "'{PropertyName}'은 (는) {MaxLength} 자 이하 여야합니다. {TotalLength} 문자를 입력했습니다.",
			"LessThanOrEqualValidator" => "'{PropertyName}'은(는) '{ComparisonValue}' 이하여야 합니다.",
			"LessThanValidator" => "'{PropertyName}'은(는) '{ComparisonValue}' 보다 작아야 합니다.",
			"NotEmptyValidator" => "'{PropertyName}'은(는) 최소한 한 글자 이상이어야 합니다.",
			"NotEqualValidator" => "'{PropertyName}'은(는) '{ComparisonValue}'와 달라야 합니다.",
			"NotNullValidator" => "'{PropertyName}'은(는) 반드시 입력해야 합니다.",
			"PredicateValidator" => "'{PropertyName}'이(가) 유효하지 않습니다.",
			"AsyncPredicateValidator" => "'{PropertyName}'이(가) 유효하지 않습니다.",
			"RegularExpressionValidator" => "'{PropertyName}'이(가) 잘못된 형식입니다.",
			"ScalePrecisionValidator" => "'{PropertyName}'은(는) 소수점 이하 {ExpectedScale}자리 이하, 총 {ExpectedPrecision}자리 이하의 숫자여야 합니다. 입력한 값은 소수점 이하 {ActualScale}자리이고 총 {Digits}자리입니다.",
			"EmptyValidator" => "'{PropertyName}'이 비어 있어야합니다.",
			"NullValidator" => "'{PropertyName}'이 비어 있어야합니다.",
			"EnumValidator" => "'{PropertyName}'에는 '{PropertyValue}'가 포함되지 않은 값 범위가 있습니다.",
			// Additional fallback messages used by clientside validation integration.
			"ExactLength_Simple" => "'{PropertyName}'은(는) {MaxLength} 글자이하의 문자열이어야 합니다.",
			"InclusiveBetween_Simple" => "'{PropertyName}'은(는) {From} 이상 {To} 이하여야 합니다.",
			"Length_Simple" => "'{PropertyName}'은(는) {MinLength} 글자 이상 {MaxLength} 글자 이하여야 합니다.",
			"MinimumLength_Simple" => "'{PropertyName}'은 {MinLength} 자 이상의 값이어야합니다.",
			"MaximumLength_Simple" => "'{PropertyName}'은 (는) {MaxLength} 자 이하 여야합니다.",
			_ => null,
		};
	}
}
