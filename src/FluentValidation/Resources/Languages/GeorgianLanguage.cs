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

	internal class GeorgianLanguage : Language {
		public override string Name => "ka";

		public GeorgianLanguage() {
			Translate<EmailValidator>("'{PropertyName}' არ არის ვალიდური იმეილის მისამართი.");
			Translate<GreaterThanOrEqualValidator>("'{PropertyName}' უნდა იყოს '{ComparisonValue}'-ზე მეტი ან ტოლი.");
			Translate<GreaterThanValidator>("'{PropertyName}' უნდა იყოს '{ComparisonValue}'-ზე მეტი.");
			Translate<LengthValidator>("'{PropertyName}' უნდა იყოს {MinLength}-დან {MaxLength} სიმბოლომდე. თქვენ შეიყვანეთ {TotalLength} სიმბოლო.");
			Translate<MinimumLengthValidator>("'{PropertyName}' უნდა იყოს მეტი ან ტოლია {MinLength} სიმბოლოები. თქვენ შეიტანეთ {TotalLength} სიმბოლოები.");
			Translate<MaximumLengthValidator>("'{PropertyName}' უნდა იყოს ნაკლები ან ტოლია {MaxLength} სიმბოლოები. თქვენ შეიტანეთ {TotalLength} სიმბოლოები.");
			Translate<LessThanOrEqualValidator>("'{PropertyName}' უნდა იყოს '{ComparisonValue}'-ზე ნაკლები ან ტოლი.");
			Translate<LessThanValidator>("'{PropertyName}' უნდა იყოს '{ComparisonValue}'-ზე ნაკლები.");
			Translate<NotEmptyValidator>("'{PropertyName}' არ უნდა იყოს ცარიელი.");
			Translate<NotEqualValidator>("'{PropertyName}' არ უნდა უდრიდეს '{ComparisonValue}'-ს.");
			Translate<NotNullValidator>("'{PropertyName}' არ უნდა იყოს ცარიელი.");
			Translate<PredicateValidator>("'{PropertyName}'-ისთვის განსაზღვრული კრიტერიუმი არ დაკმაყოფილდა.");
			Translate<AsyncPredicateValidator>("'{PropertyName}'-ისთვის განსაზღვრული კრიტერიუმი არ დაკმაყოფილდა.");
			Translate<RegularExpressionValidator>("'{PropertyName}' არასწორ ფორმატშია.");
			Translate<EqualValidator>("'{PropertyName}' უნდა უდრიდეს '{ComparisonValue}'-ს.");
			Translate<ExactLengthValidator>("'{PropertyName}' უნდა იყოს {MaxLength} სიმბოლო. თქვენ შეიყვანეთ {TotalLength} სიმბოლო.");
			Translate<InclusiveBetweenValidator>("'{PropertyName}' უნდა იყოს {From}-დან {To}-მდე (ჩათვლით). თქვენ შეიყვანეთ {Value}.");
			Translate<ExclusiveBetweenValidator>("'{PropertyName}' უნდა იყოს {From}-სა და {To}-ს შორის. თქვენ შეიყვანეთ {Value}.");
			Translate<CreditCardValidator>("'{PropertyName}' არ არის ვალიდური საკრედიტო ბარათის ნომერი.");
			Translate<ScalePrecisionValidator>("'{PropertyName}' არ უნდა იყოს ჯამში {expectedPrecision} ციფრზე მეტი, {expectedScale} ათობითი ციფრის ჩათვლით. თქვენ შეიყვანეთ {digits} ციფრი და {actualScale} ათობითი სიმბოლო.");
			Translate<EmptyValidator>("'{PropertyName}' უნდა იყოს ცარიელი.");
			Translate<NullValidator>("'{PropertyName}' უნდა იყოს ცარიელი.");
			Translate<EnumValidator>("'{PropertyValue}' არ შედის '{PropertyName}'-ის დასაშვებ მნიშვნელობებში.");
		}
	}
}
