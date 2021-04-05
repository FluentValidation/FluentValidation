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

	internal class JapaneseLanguage {
		public const string Culture = "ja";

		public static string GetTranslation(string key) => key switch {
			"EmailValidator" => "'{PropertyName}' は有効なメールアドレスではありません。",
			"GreaterThanOrEqualValidator" => "'{PropertyName}' は '{ComparisonValue} 以上でなければなりません'.",
			"GreaterThanValidator" => "'{PropertyName}' は '{ComparisonValue}' よりも大きくなければなりません。",
			"LengthValidator" => "'{PropertyName}' は {MinLength} から {MaxLength} 文字の間で入力する必要があります。 {TotalLength} 文字入力されています。",
			"MinimumLengthValidator" => "'{PropertyName}' は少なくとも {MinLength} 文字を入力しなければなりません。 {TotalLength} 文字入力されています。",
			"MaximumLengthValidator" => "'{PropertyName}' は {MaxLength} 文字以下でなければなりません。 {TotalLength}  文字入力されています。",
			"LessThanOrEqualValidator" => "'{PropertyName}' は '{ComparisonValue}' 以下である必要があります。",
			"LessThanValidator" => "'{PropertyName}' は '{ComparisonValue}' 未満である必要があります。",
			"NotEmptyValidator" => "'{PropertyName}' は空であってはなりません。",
			"NotEqualValidator" => "'{PropertyName}' は '{ComparisonValue}' と等しくなってはなりません。",
			"NotNullValidator" => "'{PropertyName}' は空であってはなりません。",
			"PredicateValidator" => "'{PropertyName}' は指定された条件が満たされませんでした。",
			"AsyncPredicateValidator" => "'{PropertyName}' は指定された条件が満たされませんでした。",
			"RegularExpressionValidator" => "'{PropertyName}' は正しい形式ではありません。",
			"EqualValidator" => "'{PropertyName}' は '{ComparisonValue}' と等しくなくてはなりません。",
			"ExactLengthValidator" => "'{PropertyName}' は {MaxLength} 文字でなくてはなりません。 {TotalLength} 文字入力されています。",
			"InclusiveBetweenValidator" => "'{PropertyName}' は {From} から {To} までの間でなければなりません。 {PropertyValue} と入力されています。",
			"ExclusiveBetweenValidator" => "'{PropertyName}' は {From} と {To} の間でなければなりません。 {PropertyValue} と入力されています。",
			"CreditCardValidator" => "'{PropertyName}' は有効なクレジットカード番号ではありません。",
			"ScalePrecisionValidator" => "'{PropertyName}' は合計で {ExpectedPrecision} 桁、小数点以下は{ExpectedScale} 桁を超えてはなりません。 {Digits} 桁、小数点以下は{ActualScale} で入力されています。",
			"EmptyValidator" => "'{PropertyName}' は空でなければなりません。",
			"NullValidator" => "'{PropertyName}' は空でなければなりません。",
			"EnumValidator" => "'{PropertyName}' の範囲に '{PropertyValue}' は含まれていません。",
			// Additional fallback messages used by clientside validation integration.
			"Length_Simple" => "'{PropertyName}' は {MinLength} から {MaxLength} 文字の間で入力する必要があります。",
			"MinimumLength_Simple" => "'{PropertyName}' は少なくとも {MinLength} 文字を入力しなければなりません。",
			"MaximumLength_Simple" => "'{PropertyName}' は {MaxLength} 文字以下でなければなりません。",
			"ExactLength_Simple" => "'{PropertyName}' は {MaxLength} 文字でなくてはなりません。",
			"InclusiveBetween_Simple" => "'{PropertyName}' は {From} から {To} までの間でなければなりません。",
			_ => null,
		};
	}
}
