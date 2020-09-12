#region License

// Copyright (c) .NET Foundation and contributors.
//
// Licensed under the Apache License, Version 2.0 (the "License",
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

	internal class GeorgianLanguage {
		public const string Culture = "ka";

		public static string GetTranslation(string key) => key switch {
			nameof(EmailValidator) => "'{PropertyName}' არ არის ვალიდური ელ.ფოსტის მისამართი.",
			nameof(GreaterThanOrEqualValidator) => "'{PropertyName}' უნდა იყოს '{ComparisonValue}'-ზე მეტი ან ტოლი.",
			nameof(GreaterThanValidator) => "'{PropertyName}' უნდა იყოს '{ComparisonValue}'-ზე მეტი.",
			nameof(LengthValidator) => "'{PropertyName}' უნდა იყოს {MinLength}-დან {MaxLength} სიმბოლომდე. თქვენ შეიყვანეთ {TotalLength} სიმბოლო.",
			nameof(MinimumLengthValidator) => "'{PropertyName}'-ის სიგრძე უნდა აღემატებოდეს {MinLength} სიმბოლოს. თქვენ შეიყვანეთ {TotalLength} სიმბოლო.",
			nameof(MaximumLengthValidator) => "'{PropertyName}'-ის სიგრძე არ უნდა აღემატებოდეს {MaxLength} სიმბოლოს. თქვენ შეიყვანეთ {TotalLength} სიმბოლო.",
			nameof(LessThanOrEqualValidator) => "'{PropertyName}' უნდა იყოს '{ComparisonValue}'-ზე ნაკლები ან ტოლი.",
			nameof(LessThanValidator) => "'{PropertyName}' უნდა იყოს '{ComparisonValue}'-ზე ნაკლები.",
			nameof(NotEmptyValidator) => "'{PropertyName}' არ უნდა იყოს ცარიელი.",
			nameof(NotEqualValidator) => "'{PropertyName}' არ უნდა უდრიდეს '{ComparisonValue}'-ს.",
			nameof(NotNullValidator) => "'{PropertyName}' არ უნდა იყოს ცარიელი.",
			nameof(PredicateValidator) => "'{PropertyName}'-ისთვის განსაზღვრული პირობა არ დაკმაყოფილდა.",
			nameof(AsyncPredicateValidator) => "'{PropertyName}'-ისთვის განსაზღვრული პირობა არ დაკმაყოფილდა.",
			nameof(RegularExpressionValidator) => "'{PropertyName}'-ის ფორმატი არასწორია.",
			nameof(EqualValidator) => "'{PropertyName}' უნდა უდრიდეს '{ComparisonValue}'-ს.",
			nameof(ExactLengthValidator) => "'{PropertyName}' უნდა უდრიდეს {MaxLength} სიმბოლოს. თქვენ შეიყვანეთ {TotalLength} სიმბოლო.",
			nameof(InclusiveBetweenValidator) => "'{PropertyName}' უნდა იყოს {From}-დან {To}-მდე (ჩათვლით). თქვენ შეიყვანეთ {Value}.",
			nameof(ExclusiveBetweenValidator) => "'{PropertyName}' უნდა იყოს {From}-სა და {To}-ს შორის. თქვენ შეიყვანეთ {Value}.",
			nameof(CreditCardValidator) => "'{PropertyName}' არ არის ვალიდური საკრედიტო ბარათის ნომერი.",
			nameof(ScalePrecisionValidator) => "'{PropertyName}' არ უნდა იყოს ჯამში {ExpectedPrecision} ციფრზე მეტი, {ExpectedScale} ათობითი ციფრის ჩათვლით. თქვენ შეიყვანეთ {Digits} ციფრი და {ActualScale} ათობითი სიმბოლო.",
			nameof(EmptyValidator) => "'{PropertyName}' უნდა იყოს ცარიელი.",
			nameof(NullValidator) => "'{PropertyName}' უნდა იყოს ცარიელი.",
			nameof(EnumValidator) => "'{PropertyValue}' არ შედის '{PropertyName}'-ის დასაშვებ მნიშვნელობებში.",
			// Additional fallback messages used by clientside validation integration.
			"Length_Simple" => "'{PropertyName}' უნდა იყოს {MinLength}-დან {MaxLength} სიმბოლომდე.",
			"MinimumLength_Simple" => "'{PropertyName}'-ის სიგრძე უნდა აღემატებოდეს {MinLength} სიმბოლოს.",
			"MaximumLength_Simple" => "'{PropertyName}'-ის სიგრძე არ უნდა აღემატებოდეს {MaxLength} სიმბოლოს.",
			"ExactLength_Simple" => "'{PropertyName}' უნდა უდრიდეს {MaxLength} სიმბოლოს.",
			"InclusiveBetween_Simple" => "'{PropertyName}' უნდა იყოს {From}-დან {To}-მდე (ჩათვლით).",
			_ => null,
		};
	}
}
