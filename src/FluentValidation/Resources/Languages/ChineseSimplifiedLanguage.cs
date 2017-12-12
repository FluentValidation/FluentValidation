#region License
// Copyright (c) Jeremy Skinner (http://www.jeremyskinner.co.uk)
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
// The latest version of this file can be found at https://github.com/JeremySkinner/FluentValidation
#endregion

namespace FluentValidation.Resources {
	using Validators;

	internal class ChineseSimplifiedLanguage : Language {
		public override string Name => "zh-CN";

		public ChineseSimplifiedLanguage() {
			Translate<EmailValidator>("'{PropertyName}' 不是有效的电子邮件地址。");
			Translate<GreaterThanOrEqualValidator>("'{PropertyName}' 必须大于或等于 '{ComparisonValue}'。");
			Translate<GreaterThanValidator>("'{PropertyName}' 必须大于 '{ComparisonValue}'。");
			Translate<LengthValidator>("'{PropertyName}' 的长度必须在 {MinLength} 到 {MaxLength} 字符，您已经输入了 {TotalLength} 字符。");
			Translate<MinimumLengthValidator>("\"{PropertyName}\"必须大于或等于{MinLength}个字符。您输入了{TotalLength}个字符。");
			Translate<MaximumLengthValidator>("\"{PropertyName}\"必须小于或等于{MaxLength}个字符。您输入了{TotalLength}个字符。");
			Translate<LessThanOrEqualValidator>("'{PropertyName}' 必须小于或等于 '{ComparisonValue}'。");
			Translate<LessThanValidator>("'{PropertyName}' 必须小于 '{ComparisonValue}'。");
			Translate<NotEmptyValidator>("请填写 '{PropertyName}'。");
			Translate<NotEqualValidator>("'{PropertyName}' 不能和 '{ComparisonValue}' 相等。");
			Translate<NotNullValidator>("请填写 '{PropertyName}'。");
			Translate<PredicateValidator>("指定的条件不符合 '{PropertyName}'。");
			Translate<AsyncPredicateValidator>("指定的条件不符合 '{PropertyName}'。");
			Translate<RegularExpressionValidator>("'{PropertyName}' 的格式不正确。");
			Translate<EqualValidator>("'{PropertyName}' 应该和 '{ComparisonValue}' 相等。");
			Translate<ExactLengthValidator>("'{PropertyName}' 必须是 {MaxLength} 个字符，您已经输入了 {TotalLength} 字符。");
			Translate<InclusiveBetweenValidator>("'{PropertyName}' 必须在 {From} 和 {To} 之间， 您输入了 {Value}。");
			Translate<ExclusiveBetweenValidator>("'{PropertyName}' 必须在 {From} 和 {To} 之外， 您输入了 {Value}。");
			Translate<CreditCardValidator>("'{PropertyName}' 不是有效的信用卡号。");
			Translate<ScalePrecisionValidator>("'{PropertyName}' 总位数不能超过 {expectedPrecision} 位，其中整数部分 {expectedScale} 位。您填写了 {digits} 位小数和 {actualScale} 位整数。");
			Translate<EmptyValidator>("\"{PropertyName}\"应该是空的。");
			Translate<NullValidator>("\"{PropertyName}\"必须为空。");
			Translate<EnumValidator>("\"{PropertyName}\"的值范围不包含\"{PropertyValue}\"。");
		}
	}
}