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

	internal class ChineseSimplifiedLanguage {
		public const string Culture = "zh-CN";

		public static string GetTranslation(string key) => key switch {
			"EmailValidator" => "'{PropertyName}' 不是有效的电子邮件地址。",
			"GreaterThanOrEqualValidator" => "'{PropertyName}' 必须大于或等于 '{ComparisonValue}'。",
			"GreaterThanValidator" => "'{PropertyName}' 必须大于 '{ComparisonValue}'。",
			"LengthValidator" => "'{PropertyName}' 的长度必须在 {MinLength} 到 {MaxLength} 字符，您输入了 {TotalLength} 字符。",
			"MinimumLengthValidator" => "'{PropertyName}' 必须大于或等于{MinLength}个字符。您输入了{TotalLength}个字符。",
			"MaximumLengthValidator" => "'{PropertyName}' 必须小于或等于{MaxLength}个字符。您输入了{TotalLength}个字符。",
			"LessThanOrEqualValidator" => "'{PropertyName}' 必须小于或等于 '{ComparisonValue}'。",
			"LessThanValidator" => "'{PropertyName}' 必须小于 '{ComparisonValue}'。",
			"NotEmptyValidator" => "'{PropertyName}' 不能为空。",
			"NotEqualValidator" => "'{PropertyName}' 不能和 '{ComparisonValue}' 相等。",
			"NotNullValidator" => "'{PropertyName}' 不能为Null。",
			"PredicateValidator" => "'{PropertyName}' 不符合指定的条件。",
			"AsyncPredicateValidator" => "'{PropertyName}' 不符合指定的条件。",
			"RegularExpressionValidator" => "'{PropertyName}' 的格式不正确。",
			"EqualValidator" => "'{PropertyName}' 应该和 '{ComparisonValue}' 相等。",
			"ExactLengthValidator" => "'{PropertyName}' 必须是 {MaxLength} 个字符，您输入了 {TotalLength} 字符。",
			"InclusiveBetweenValidator" => "'{PropertyName}' 必须在 {From} (包含)和 {To} (包含)之间， 您输入了 {PropertyValue}。",
			"ExclusiveBetweenValidator" => "'{PropertyName}' 必须在 {From} (不包含)和 {To} (不包含)之间， 您输入了 {PropertyValue}。",
			"CreditCardValidator" => "'{PropertyName}' 不是有效的信用卡号。",
			"ScalePrecisionValidator" => "'{PropertyName}' 总位数不能超过 {ExpectedPrecision} 位，其中小数部分 {ExpectedScale} 位。您共计输入了 {Digits} 位数字，其中小数部分{ActualScale} 位。",
			"EmptyValidator" => "'{PropertyName}' 必须为空。",
			"NullValidator" => "'{PropertyName}' 必须为Null。",
			"EnumValidator" => "'{PropertyName}' 的值范围不包含 '{PropertyValue}'。",
			// Additional fallback messages used by clientside validation integration.
			"Length_Simple" => "'{PropertyName}' 的长度必须在 {MinLength} 到 {MaxLength} 字符。",
			"MinimumLength_Simple" => "'{PropertyName}' 必须大于或等于{MinLength}个字符。",
			"MaximumLength_Simple" => "'{PropertyName}' 必须小于或等于{MaxLength}个字符。",
			"ExactLength_Simple" => "'{PropertyName}' 必须是 {MaxLength} 个字符。",
			"InclusiveBetween_Simple" => "'{PropertyName}' 必须在 {From} (包含)和 {To} (包含)之间。",
			_ => null,
		};
	}
}
