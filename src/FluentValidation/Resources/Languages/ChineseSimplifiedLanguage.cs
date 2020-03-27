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

	internal class ChineseSimplifiedLanguage : Language {
		public const string Culture = "zh-CN";
		public override string Name => Culture;

		public ChineseSimplifiedLanguage() {
			Translate<EmailValidator>("'{PropertyName}' 不是有效的电子邮件地址。");
			Translate<GreaterThanOrEqualValidator>("'{PropertyName}' 必须大于或等于 '{ComparisonValue}'。");
			Translate<GreaterThanValidator>("'{PropertyName}' 必须大于 '{ComparisonValue}'。");
			Translate<LengthValidator>("'{PropertyName}' 的长度必须在 {MinLength} 到 {MaxLength} 字符，您输入了 {TotalLength} 字符。");
			Translate<MinimumLengthValidator>("'{PropertyName}' 必须大于或等于{MinLength}个字符。您输入了{TotalLength}个字符。");
			Translate<MaximumLengthValidator>("'{PropertyName}' 必须小于或等于{MaxLength}个字符。您输入了{TotalLength}个字符。");
			Translate<LessThanOrEqualValidator>("'{PropertyName}' 必须小于或等于 '{ComparisonValue}'。");
			Translate<LessThanValidator>("'{PropertyName}' 必须小于 '{ComparisonValue}'。");
			Translate<NotEmptyValidator>("'{PropertyName}' 不能为空。");
			Translate<NotEqualValidator>("'{PropertyName}' 不能和 '{ComparisonValue}' 相等。");
			Translate<NotNullValidator>("'{PropertyName}' 不能为Null。");
			Translate<PredicateValidator>("'{PropertyName}' 不符合指定的条件。");
			Translate<AsyncPredicateValidator>("'{PropertyName}' 不符合指定的条件。");
			Translate<RegularExpressionValidator>("'{PropertyName}' 的格式不正确。");
			Translate<EqualValidator>("'{PropertyName}' 应该和 '{ComparisonValue}' 相等。");
			Translate<ExactLengthValidator>("'{PropertyName}' 必须是 {MaxLength} 个字符，您输入了 {TotalLength} 字符。");
			Translate<InclusiveBetweenValidator>("'{PropertyName}' 必须在 {From} (包含)和 {To} (包含)之间， 您输入了 {Value}。");
			Translate<ExclusiveBetweenValidator>("'{PropertyName}' 必须在 {From} (不包含)和 {To} (不包含)之间， 您输入了 {Value}。");
			Translate<CreditCardValidator>("'{PropertyName}' 不是有效的信用卡号。");
			Translate<ScalePrecisionValidator>("'{PropertyName}' 总位数不能超过 {ExpectedPrecision} 位，其中小数部分 {ExpectedScale} 位。您共计输入了 {Digits} 位数字，其中小数部分{ActualScale} 位。");
			Translate<EmptyValidator>("'{PropertyName}' 必须为空。");
			Translate<NullValidator>("'{PropertyName}' 必须为Null。");
			Translate<EnumValidator>("'{PropertyName}' 的值范围不包含 '{PropertyValue}'。");
			// Additional fallback messages used by clientside validation integration.
			Translate("Length_Simple", "'{PropertyName}' 的长度必须在 {MinLength} 到 {MaxLength} 字符。");
			Translate("MinimumLength_Simple", "'{PropertyName}' 必须大于或等于{MinLength}个字符。");
			Translate("MaximumLength_Simple", "'{PropertyName}' 必须小于或等于{MaxLength}个字符。");
			Translate("ExactLength_Simple", "'{PropertyName}' 必须是 {MaxLength} 个字符。");
			Translate("InclusiveBetween_Simple", "'{PropertyName}' 必须在 {From} (包含)和 {To} (包含)之间。");
		}
	}
}
