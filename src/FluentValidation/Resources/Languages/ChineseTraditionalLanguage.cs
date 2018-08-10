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

	internal class ChineseTraditionalLanguage : Language {
		public override string Name => "zh-TW";

		public ChineseTraditionalLanguage() {
			Translate<EmailValidator>("'{PropertyName}' 不是有效的電子郵件地址。");
			Translate<GreaterThanOrEqualValidator>("'{PropertyName}' 必須大於或等於 '{ComparisonValue}'。");
			Translate<GreaterThanValidator>("'{PropertyName}' 必須大於 '{ComparisonValue}'。");
			Translate<LengthValidator>("'{PropertyName}' 的長度必須在 {MinLength} 到 {MaxLength} 字符，您輸入了 {TotalLength} 字符。");
			Translate<MinimumLengthValidator>("'{PropertyName}' 必須大於或等於{MinLength}個字符。您輸入了{TotalLength}個字符。");
			Translate<MaximumLengthValidator>("'{PropertyName}' 必須小於或等於{MaxLength}個字符。您輸入了{TotalLength}個字符。");
			Translate<LessThanOrEqualValidator>("'{PropertyName}' 必須小於或等於 '{ComparisonValue}'。");
			Translate<LessThanValidator>("'{PropertyName}' 必須小於 '{ComparisonValue}'。");
			Translate<NotEmptyValidator>("'{PropertyName}' 不能為空。");
			Translate<NotEqualValidator>("'{PropertyName}' 不能和 '{ComparisonValue}' 相等。");
			Translate<NotNullValidator>("'{PropertyName}' 不能為Null。");
			Translate<PredicateValidator>("'{PropertyName}' 不符合指定的條件。");
			Translate<AsyncPredicateValidator>("'{PropertyName}' 不符合指定的條件。");
			Translate<RegularExpressionValidator>("'{PropertyName}' 的格式不正確。");
			Translate<EqualValidator>("'{PropertyName}' 應該和 '{ComparisonValue}' 相等。");
			Translate<ExactLengthValidator>("'{PropertyName}' 必須是 {MaxLength} 個字符，您輸入了 {TotalLength} 字符。");
			Translate<InclusiveBetweenValidator>("'{PropertyName}' 必須在 {From} (包含)和 {To} (包含)之間， 您輸入了 {Value}。");
			Translate<ExclusiveBetweenValidator>("'{PropertyName}' 必須在 {From} (不包含)和 {To} (不包含)之間， 您輸入了 {Value}。");
			Translate<CreditCardValidator>("'{PropertyName}' 不是有效的信用卡號碼。");
			Translate<ScalePrecisionValidator>("'{PropertyName}' 總位數不能超過 {expectedPrecision} 位，其中小數部份 {expectedScale} 位。您共計輸入了 {digits} 位數字，其中小數部份{actualScale} 位。");
			Translate<EmptyValidator>("'{PropertyName}' 必須為空。");
			Translate<NullValidator>("'{PropertyName}' 必須為Null。");
			Translate<EnumValidator>("'{PropertyName}' 的數值範圍不包含 '{PropertyValue}'。");
		}
	}
}
