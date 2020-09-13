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

	internal class ChineseTraditionalLanguage {
		public const string Culture = "zh-TW";

		public static string GetTranslation(string key) => key switch {
			nameof(EmailValidator) => "'{PropertyName}' 不是有效的電子郵件地址。",
			nameof(GreaterThanOrEqualValidator) => "'{PropertyName}' 必須大於或等於 '{ComparisonValue}'。",
			nameof(GreaterThanValidator) => "'{PropertyName}' 必須大於 '{ComparisonValue}'。",
			nameof(LengthValidator) => "'{PropertyName}' 的長度必須在 {MinLength} 到 {MaxLength} 字符，您輸入了 {TotalLength} 字符。",
			nameof(MinimumLengthValidator) => "'{PropertyName}' 必須大於或等於{MinLength}個字符。您輸入了{TotalLength}個字符。",
			nameof(MaximumLengthValidator) => "'{PropertyName}' 必須小於或等於{MaxLength}個字符。您輸入了{TotalLength}個字符。",
			nameof(LessThanOrEqualValidator) => "'{PropertyName}' 必須小於或等於 '{ComparisonValue}'。",
			nameof(LessThanValidator) => "'{PropertyName}' 必須小於 '{ComparisonValue}'。",
			nameof(NotEmptyValidator) => "'{PropertyName}' 不能為空。",
			nameof(NotEqualValidator) => "'{PropertyName}' 不能和 '{ComparisonValue}' 相等。",
			nameof(NotNullValidator) => "'{PropertyName}' 不能為Null。",
			nameof(PredicateValidator) => "'{PropertyName}' 不符合指定的條件。",
			nameof(AsyncPredicateValidator) => "'{PropertyName}' 不符合指定的條件。",
			nameof(RegularExpressionValidator) => "'{PropertyName}' 的格式不正確。",
			nameof(EqualValidator) => "'{PropertyName}' 應該和 '{ComparisonValue}' 相等。",
			nameof(ExactLengthValidator) => "'{PropertyName}' 必須是 {MaxLength} 個字符，您輸入了 {TotalLength} 字符。",
			nameof(InclusiveBetweenValidator) => "'{PropertyName}' 必須在 {From} (包含)和 {To} (包含)之間， 您輸入了 {Value}。",
			nameof(ExclusiveBetweenValidator) => "'{PropertyName}' 必須在 {From} (不包含)和 {To} (不包含)之間， 您輸入了 {Value}。",
			nameof(CreditCardValidator) => "'{PropertyName}' 不是有效的信用卡號碼。",
			nameof(ScalePrecisionValidator) => "'{PropertyName}' 總位數不能超過 {ExpectedPrecision} 位，其中小數部份 {ExpectedScale} 位。您共計輸入了 {Digits} 位數字，其中小數部份{ActualScale} 位。",
			nameof(EmptyValidator) => "'{PropertyName}' 必須為空。",
			nameof(NullValidator) => "'{PropertyName}' 必須為Null。",
			nameof(EnumValidator) => "'{PropertyName}' 的數值範圍不包含 '{PropertyValue}'。",
			// Additional fallback messages used by clientside validation integration.
			"Length_Simple" => "'{PropertyName}' 的長度必須在 {MinLength} 到 {MaxLength} 字符。",
			"MinimumLength_Simple" => "'{PropertyName}' 必須大於或等於{MinLength}個字符。",
			"MaximumLength_Simple" => "'{PropertyName}' 必須小於或等於{MaxLength}個字符。",
			"ExactLength_Simple" => "'{PropertyName}' 必須是 {MaxLength} 個字符。",
			"InclusiveBetween_Simple" => "'{PropertyName}' 必須在 {From} (包含)和 {To} (包含)之間。",
			_ => null,
		};
	}
}
