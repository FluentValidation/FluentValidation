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

	internal class TurkishLanguage {
		public const string Culture = "tr";

		public static string GetTranslation(string key) => key switch {
			nameof(EmailValidator) => "'{PropertyName}'  geçerli bir e-posta adresi değil.",
			nameof(GreaterThanOrEqualValidator) => "'{PropertyName}' değeri '{ComparisonValue}' değerinden büyük veya eşit olmalı.",
			nameof(GreaterThanValidator) => "'{PropertyName}' değeri '{ComparisonValue}' değerinden büyük olmalı.",
			nameof(LengthValidator) => "'{PropertyName}', {MinLength} ve {MaxLength} arasında karakter uzunluğunda olmalı . Toplam {TotalLength} adet karakter girdiniz.",
			nameof(MinimumLengthValidator) => "'{PropertyName}', {MinLength} karakterden büyük veya eşit olmalıdır. {TotalLength} karakter girdiniz.",
			nameof(MaximumLengthValidator) => "'{PropertyName}', {MaxLength} karakterden küçük veya eşit olmalıdır. {TotalLength} karakter girdiniz.",
			nameof(LessThanOrEqualValidator) => "'{PropertyName}', '{ComparisonValue}' değerinden küçük veya eşit olmalı.",
			nameof(LessThanValidator) => "'{PropertyName}', '{ComparisonValue}' değerinden küçük olmalı.",
			nameof(NotEmptyValidator) => "'{PropertyName}' boş olmamalı.",
			nameof(NotEqualValidator) => "'{PropertyName}', '{ComparisonValue}' değerine eşit olmamalı.",
			nameof(NotNullValidator) => "'{PropertyName}' boş olamaz.",
			nameof(PredicateValidator) => "Belirtilen durum '{PropertyName}' için geçerli değil.",
			nameof(AsyncPredicateValidator) => "Belirtilen durum '{PropertyName}' için geçerli değil.",
			nameof(RegularExpressionValidator) => "'{PropertyName}' değerinin formatı doğru değil.",
			nameof(EqualValidator) => "'{PropertyName}', '{ComparisonValue}' değerine eşit olmalı.",
			nameof(ExactLengthValidator) => "'{PropertyName}', {MaxLength} karakter uzunluğunda olmalı. {TotalLength} adet karakter girdiniz.",
			nameof(InclusiveBetweenValidator) => "'{PropertyName}', {From} ve {To} arasında olmalı. {Value} değerini girdiniz.",
			nameof(ExclusiveBetweenValidator) => "'{PropertyName}', {From} ve {To} (dahil değil) arasında olmalı. {Value} değerini girdiniz.",
			nameof(CreditCardValidator) => "'{PropertyName}' geçerli bir kredi kartı numarası değil.",
			nameof(ScalePrecisionValidator) => "'{PropertyName}', {ExpectedScale} ondalıkları için toplamda {ExpectedPrecision} rakamdan fazla olamaz. {Digits} basamak ve {ActualScale} basamak bulundu.",
			nameof(EmptyValidator) => "'{PropertyName}' boş olmalıdır.",
			nameof(NullValidator) => "'{PropertyName}' boş olmalıdır.",
			nameof(EnumValidator) => "'{PropertyName}', '{PropertyValue}' içermeyen bir değer aralığı içeriyor.",
			// Additional fallback messages used by clientside validation integration.
			"Length_Simple" => "'{PropertyName}', {MinLength} ve {MaxLength} arasında karakter uzunluğunda olmalı.",
			"MinimumLength_Simple" => "'{PropertyName}', {MinLength} karakterden büyük veya eşit olmalıdır.",
			"MaximumLength_Simple" => "'{PropertyName}', {MaxLength} karakterden küçük veya eşit olmalıdır.",
			"ExactLength_Simple" => "'{PropertyName}', {MaxLength} karakter uzunluğunda olmalı.",
			"InclusiveBetween_Simple" => "'{PropertyName}', {From} ve {To} arasında olmalı.",
			_ => null,
		};
	}
}
