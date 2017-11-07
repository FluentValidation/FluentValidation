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

	internal class TurkishLanguage : Language {
		public override string Name => "tr";

		public TurkishLanguage() {
			Translate<EmailValidator>("'{PropertyName}'  geçerli email adresi değil.");
			Translate<GreaterThanOrEqualValidator>("'{PropertyName}' değeri '{ComparisonValue}' değerinden büyük veya eşit olmalı.");
			Translate<GreaterThanValidator>("'{PropertyName}' değeri '{ComparisonValue}' değerinden büyük olmalı.");
			Translate<LengthValidator>("'{PropertyName}' değeri  {MinLength} ve {MaxLength} arasında karakter uzunluğunda olmalı . Toplam {TotalLength} adet karakter girdiniz.");
			Translate<MinimumLengthValidator>("'{PropertyName}' değeri {MinLength} ve 1000 arasında karakter uzunluğunda olmalı . Toplam {TotalLength} adet karakter girdiniz.");
			Translate<MaximumLengthValidator>("'{PropertyName}' değeri 0 ve {MaxLength} arasında karakter uzunluğunda olmalı . Toplam {TotalLength} adet karakter girdiniz.");
			Translate<LessThanOrEqualValidator>("'{PropertyName}' değeri '{ComparisonValue}' değerinden küçük veya eşit olmalı.");
			Translate<LessThanValidator>("'{PropertyName}' değeri  '{ComparisonValue}' değerinden küçük olmalı.");
			Translate<NotEmptyValidator>("'{PropertyName}' değeri boş olmamalı.");
			Translate<NotEqualValidator>("'{PropertyName}' değeri '{ComparisonValue}' değerine eşit olmamalı.");
			Translate<NotNullValidator>("'{PropertyName}' değeri boş olamaz.");
			Translate<PredicateValidator>("Belirtilen durum '{PropertyName}' için geçerli değil.");
			Translate<AsyncPredicateValidator>("Belirtilen durum '{PropertyName}' için geçerli değil.");
			Translate<RegularExpressionValidator>("'{PropertyName}' değerinin formatı doğru değil.");
			Translate<EqualValidator>("'{PropertyName}' değeri '{ComparisonValue}' değerine eşit olmalı.");
			Translate<ExactLengthValidator>("'{PropertyName}' değeri {MaxLength} karakter uzunluğunda olmalı. {TotalLength} adet karakter girdiniz.");
			Translate<InclusiveBetweenValidator>("'{PropertyName}' değeri {From} ve {To} arasında olmalı. {Value} değerini girdiniz.");
			Translate<ExclusiveBetweenValidator>("'{PropertyName}' değeri {From} ve {To} (dahil değil) arasında olmalı. {Value} değerini girdiniz.");
			Translate<CreditCardValidator>("'{PropertyName}' geçerli kredi kartı numarası değil.");
		}
	}
}