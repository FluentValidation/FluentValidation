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

	internal class GeorgianLanguage : Language {
		public const string Culture = "ka";
		public override string Name => Culture;

		public GeorgianLanguage() {
			Translate<EmailValidator>("'{PropertyName}' არ არის ვალიდური ელ.ფოსტის მისამართი.");
			Translate<GreaterThanOrEqualValidator>("'{PropertyName}' უნდა იყოს '{ComparisonValue}'-ზე მეტი ან ტოლი.");
			Translate<GreaterThanValidator>("'{PropertyName}' უნდა იყოს '{ComparisonValue}'-ზე მეტი.");
			Translate<LengthValidator>("'{PropertyName}' უნდა იყოს {MinLength}-დან {MaxLength} სიმბოლომდე. თქვენ შეიყვანეთ {TotalLength} სიმბოლო.");
			Translate<MinimumLengthValidator>("'{PropertyName}'-ის სიგრძე უნდა აღემატებოდეს {MinLength} სიმბოლოს. თქვენ შეიყვანეთ {TotalLength} სიმბოლო.");
			Translate<MaximumLengthValidator>("'{PropertyName}'-ის სიგრძე არ უნდა აღემატებოდეს {MaxLength} სიმბოლოს. თქვენ შეიყვანეთ {TotalLength} სიმბოლო.");
			Translate<LessThanOrEqualValidator>("'{PropertyName}' უნდა იყოს '{ComparisonValue}'-ზე ნაკლები ან ტოლი.");
			Translate<LessThanValidator>("'{PropertyName}' უნდა იყოს '{ComparisonValue}'-ზე ნაკლები.");
			Translate<NotEmptyValidator>("'{PropertyName}' არ უნდა იყოს ცარიელი.");
			Translate<NotEqualValidator>("'{PropertyName}' არ უნდა უდრიდეს '{ComparisonValue}'-ს.");
			Translate<NotNullValidator>("'{PropertyName}' არ უნდა იყოს ცარიელი.");
			Translate<PredicateValidator>("'{PropertyName}'-ისთვის განსაზღვრული პირობა არ დაკმაყოფილდა.");
			Translate<AsyncPredicateValidator>("'{PropertyName}'-ისთვის განსაზღვრული პირობა არ დაკმაყოფილდა.");
			Translate<RegularExpressionValidator>("'{PropertyName}'-ის ფორმატი არასწორია.");
			Translate<EqualValidator>("'{PropertyName}' უნდა უდრიდეს '{ComparisonValue}'-ს.");
			Translate<ExactLengthValidator>("'{PropertyName}' უნდა უდრიდეს {MaxLength} სიმბოლოს. თქვენ შეიყვანეთ {TotalLength} სიმბოლო.");
			Translate<InclusiveBetweenValidator>("'{PropertyName}' უნდა იყოს {From}-დან {To}-მდე (ჩათვლით). თქვენ შეიყვანეთ {Value}.");
			Translate<ExclusiveBetweenValidator>("'{PropertyName}' უნდა იყოს {From}-სა და {To}-ს შორის. თქვენ შეიყვანეთ {Value}.");
			Translate<CreditCardValidator>("'{PropertyName}' არ არის ვალიდური საკრედიტო ბარათის ნომერი.");
			Translate<ScalePrecisionValidator>("'{PropertyName}' არ უნდა იყოს ჯამში {ExpectedPrecision} ციფრზე მეტი, {ExpectedScale} ათობითი ციფრის ჩათვლით. თქვენ შეიყვანეთ {Digits} ციფრი და {ActualScale} ათობითი სიმბოლო.");
			Translate<EmptyValidator>("'{PropertyName}' უნდა იყოს ცარიელი.");
			Translate<NullValidator>("'{PropertyName}' უნდა იყოს ცარიელი.");
			Translate<EnumValidator>("'{PropertyValue}' არ შედის '{PropertyName}'-ის დასაშვებ მნიშვნელობებში.");
			// Additional fallback messages used by clientside validation integration.
			Translate("Length_Simple", "'{PropertyName}' უნდა იყოს {MinLength}-დან {MaxLength} სიმბოლომდე.");
			Translate("MinimumLength_Simple", "'{PropertyName}'-ის სიგრძე უნდა აღემატებოდეს {MinLength} სიმბოლოს.");
			Translate("MaximumLength_Simple", "'{PropertyName}'-ის სიგრძე არ უნდა აღემატებოდეს {MaxLength} სიმბოლოს.");
			Translate("ExactLength_Simple", "'{PropertyName}' უნდა უდრიდეს {MaxLength} სიმბოლოს.");
			Translate("InclusiveBetween_Simple", "'{PropertyName}' უნდა იყოს {From}-დან {To}-მდე (ჩათვლით).");
		}
	}
}
