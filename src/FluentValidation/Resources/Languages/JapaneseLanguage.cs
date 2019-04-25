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

	internal class JapaneseLanguage : Language {
		public override string Name => "ja";

		public JapaneseLanguage() {
			Translate<EmailValidator>("'{PropertyName}' は有効なメールアドレスではありません。");
			Translate<GreaterThanOrEqualValidator>("'{PropertyName}' は '{ComparisonValue} 以上でなければなりません'.");
			Translate<GreaterThanValidator>("'{PropertyName}' は '{ComparisonValue}' よりも大きくなければなりません。");
			Translate<LengthValidator>("'{PropertyName}' は {MinLength} から {MaxLength} 文字の間で入力する必要があります。 {TotalLength} 文字入力されています。");
			Translate<MinimumLengthValidator>("'{PropertyName}' は少なくとも {MinLength} 文字を入力しなければなりません。 {TotalLength} 文字入力されています。");
			Translate<MaximumLengthValidator>("'{PropertyName}' は {MaxLength} 文字以下でなければなりません。 {TotalLength}  文字入力されています。");
			Translate<LessThanOrEqualValidator>("'{PropertyName}' は '{ComparisonValue}' 以下である必要があります。");
			Translate<LessThanValidator>("'{PropertyName}' は '{ComparisonValue}' 未満である必要があります。");
			Translate<NotEmptyValidator>("'{PropertyName}' は空であってはなりません。");
			Translate<NotEqualValidator>("'{PropertyName}' は '{ComparisonValue}' と等しくなってはなりません。");
			Translate<NotNullValidator>("'{PropertyName}' は空であってはなりません。");
			Translate<PredicateValidator>("'{PropertyName}' は指定された条件が満たされませんでした。");
			Translate<AsyncPredicateValidator>("'{PropertyName}' は指定された条件が満たされませんでした。");
			Translate<RegularExpressionValidator>("'{PropertyName}' は正しい形式ではありません。");
			Translate<EqualValidator>("'{PropertyName}' は '{ComparisonValue}' と等しくなくてはなりません。");
			Translate<ExactLengthValidator>("'{PropertyName}' は {MaxLength} 文字でなくてはなりません。 {TotalLength} 文字入力されています。");
			Translate<InclusiveBetweenValidator>("'{PropertyName}' は {From} から {To} までの間でなければなりません。 {Value} と入力されています。");
			Translate<ExclusiveBetweenValidator>("'{PropertyName}' は {From} と {To} の間でなければなりません。 {Value} と入力されています。");
			Translate<CreditCardValidator>("'{PropertyName}' は有効なクレジットカード番号ではありません。");
			Translate<ScalePrecisionValidator>("'{PropertyName}' は合計で {ExpectedPrecision} 桁、小数点以下は{ExpectedScale} 桁を超えてはなりません。 {Digits} 桁、小数点以下は{ActualScale} で入力されています。");
			Translate<EmptyValidator>("'{PropertyName}' は空でなければなりません。");
			Translate<NullValidator>("'{PropertyName}' は空でなければなりません。");
			Translate<EnumValidator>("'{PropertyName}' の範囲に '{PropertyValue}' は含まれていません。");
			// Additional fallback messages used by clientside validation integration.
			Translate("Length_Simple", "'{PropertyName}' は {MinLength} から {MaxLength} 文字の間で入力する必要があります。");
			Translate("MinimumLength_Simple", "'{PropertyName}' は少なくとも {MinLength} 文字を入力しなければなりません。");
			Translate("MaximumLength_Simple", "'{PropertyName}' は {MaxLength} 文字以下でなければなりません。");
			Translate("ExactLength_Simple", "'{PropertyName}' は {MaxLength} 文字でなくてはなりません。");
			Translate("InclusiveBetween_Simple", "'{PropertyName}' は {From} から {To} までの間でなければなりません。");
		}
	}
}
