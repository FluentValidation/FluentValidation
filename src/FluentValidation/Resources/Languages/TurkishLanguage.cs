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

	internal class TurkishLanguage : Language {
		public const string Culture = "tr";
		public override string Name => Culture;

		public TurkishLanguage() {
			Translate<EmailValidator>("'{PropertyName}'  geçerli bir e-posta adresi değil.");
			Translate<GreaterThanOrEqualValidator>("'{PropertyName}' değeri '{ComparisonValue}' değerinden büyük veya eşit olmalı.");
			Translate<GreaterThanValidator>("'{PropertyName}' değeri '{ComparisonValue}' değerinden büyük olmalı.");
			Translate<LengthValidator>("'{PropertyName}', {MinLength} ve {MaxLength} arasında karakter uzunluğunda olmalı . Toplam {TotalLength} adet karakter girdiniz.");
			Translate<MinimumLengthValidator>("'{PropertyName}', {MinLength} karakterden büyük veya eşit olmalıdır. {TotalLength} karakter girdiniz.");
			Translate<MaximumLengthValidator>("'{PropertyName}', {MaxLength} karakterden küçük veya eşit olmalıdır. {TotalLength} karakter girdiniz.");
			Translate<LessThanOrEqualValidator>("'{PropertyName}', '{ComparisonValue}' değerinden küçük veya eşit olmalı.");
			Translate<LessThanValidator>("'{PropertyName}', '{ComparisonValue}' değerinden küçük olmalı.");
			Translate<NotEmptyValidator>("'{PropertyName}' boş olmamalı.");
			Translate<NotEqualValidator>("'{PropertyName}', '{ComparisonValue}' değerine eşit olmamalı.");
			Translate<NotNullValidator>("'{PropertyName}' boş olamaz.");
			Translate<PredicateValidator>("Belirtilen durum '{PropertyName}' için geçerli değil.");
			Translate<AsyncPredicateValidator>("Belirtilen durum '{PropertyName}' için geçerli değil.");
			Translate<RegularExpressionValidator>("'{PropertyName}' değerinin formatı doğru değil.");
			Translate<EqualValidator>("'{PropertyName}', '{ComparisonValue}' değerine eşit olmalı.");
			Translate<ExactLengthValidator>("'{PropertyName}', {MaxLength} karakter uzunluğunda olmalı. {TotalLength} adet karakter girdiniz.");
			Translate<InclusiveBetweenValidator>("'{PropertyName}', {From} ve {To} arasında olmalı. {Value} değerini girdiniz.");
			Translate<ExclusiveBetweenValidator>("'{PropertyName}', {From} ve {To} (dahil değil) arasında olmalı. {Value} değerini girdiniz.");
			Translate<CreditCardValidator>("'{PropertyName}' geçerli bir kredi kartı numarası değil.");
			Translate<ScalePrecisionValidator>("'{PropertyName}', {ExpectedScale} ondalıkları için toplamda {ExpectedPrecision} rakamdan fazla olamaz. {Digits} basamak ve {ActualScale} basamak bulundu.");
			Translate<EmptyValidator>("'{PropertyName}' boş olmalıdır.");
			Translate<NullValidator>("'{PropertyName}' boş olmalıdır.");
			Translate<EnumValidator>("'{PropertyName}', '{PropertyValue}' içermeyen bir değer aralığı içeriyor.");
			// Additional fallback messages used by clientside validation integration.
			Translate("Length_Simple", "'{PropertyName}', {MinLength} ve {MaxLength} arasında karakter uzunluğunda olmalı.");
			Translate("MinimumLength_Simple", "'{PropertyName}', {MinLength} karakterden büyük veya eşit olmalıdır.");
			Translate("MaximumLength_Simple", "'{PropertyName}', {MaxLength} karakterden küçük veya eşit olmalıdır.");
			Translate("ExactLength_Simple", "'{PropertyName}', {MaxLength} karakter uzunluğunda olmalı.");
			Translate("InclusiveBetween_Simple", "'{PropertyName}', {From} ve {To} arasında olmalı.");
		}
	}
}
