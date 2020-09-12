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

	internal class PersianLanguage {
		public const string Culture = "fa";

		public static string GetTranslation(string key) => key switch {
			nameof(EmailValidator) => "'{PropertyName}' وارد شده قالب صحیح یک ایمیل را ندارد.",
			nameof(GreaterThanOrEqualValidator) => "'{PropertyName}' باید بیشتر یا مساوی '{ComparisonValue}' باشد.",
			nameof(GreaterThanValidator) => "'{PropertyName}' باید بیشتر از '{ComparisonValue}' باشد.",
			nameof(LengthValidator) => "'{PropertyName}' باید حداقل {MinLength} و حداکثر {MaxLength} کاراکتر داشته باشد. اما مقدار وارد شده {TotalLength} کاراکتر دارد.",
			nameof(MinimumLengthValidator) => "'{PropertyName}' باید بزرگتر یا برابر با {MinLength} کاراکتر باشد. شما تعداد {TotalLength} کاراکتر را وارد کردید",
			nameof(MaximumLengthValidator) => "'{PropertyName}' باید کمتر یا مساوی {MaxLength} باشد. {TotalLength} را وارد کردید",
			nameof(LessThanOrEqualValidator) => "'{PropertyName}' باید کمتر یا مساوی '{ComparisonValue}' باشد.",
			nameof(LessThanValidator) => "'{PropertyName}' باید کمتر از '{ComparisonValue}' باشد.",
			nameof(NotEmptyValidator) => "وارد کردن '{PropertyName}' ضروری است.",
			nameof(NotEqualValidator) => "'{PropertyName}' نباید برابر با '{ComparisonValue}' باشد.",
			nameof(NotNullValidator) => "وارد کردن '{PropertyName}' ضروری است.",
			nameof(PredicateValidator) => "شرط تعیین شده برای '{PropertyName}' برقرار نیست.",
			nameof(AsyncPredicateValidator) => "شرط تعیین شده برای '{PropertyName}' برقرار نیست.",
			nameof(RegularExpressionValidator) => "'{PropertyName}' دارای قالب صحیح نیست.",
			nameof(EqualValidator) => "مقادیر وارد شده برای '{PropertyName}' و '{ComparisonValue}' یکسان نیستند.",
			nameof(ExactLengthValidator) => "'{PropertyName}' باید دقیقا {MaxLength} کاراکتر باشد اما مقدار وارد شده {TotalLength} کاراکتر دارد.",
			nameof(InclusiveBetweenValidator) => "'{PropertyName}' باید بین {From} و {To} باشد. اما مقدار وارد شده ({Value}) در این محدوده نیست.",
			nameof(ExclusiveBetweenValidator) => "'{PropertyName}' باید بیشتر از {From} و کمتر از {To} باشد. اما مقدار وارد شده ({Value}) در این محدوده نیست.",
			nameof(CreditCardValidator) => "'{PropertyName}' وارد شده معتبر نیست.",
			nameof(ScalePrecisionValidator) => "'{PropertyName}' نباید بیش از {ExpectedPrecision} رقم، شامل {ExpectedScale} رقم اعشار داشته باشد. مقدار وارد شده {Digits} رقم و {ActualScale} رقم اعشار دارد.",
			nameof(EmptyValidator) => "'{PropertyName}' باید خالی باشد.",
			nameof(NullValidator) => "'{PropertyName}' باید خالی باشد.",
			nameof(EnumValidator) => "مقدار '{PropertyValue}' در لیست مقادیر قابل قبول برای '{PropertyName}' نمی باشد.",
			// Additional fallback messages used by clientside validation integration.
			"Length_Simple" => "'{PropertyName}' باید حداقل {MinLength} و حداکثر {MaxLength} کاراکتر داشته باشد.",
			"MinimumLength_Simple" => "'{PropertyName}' باید بزرگتر یا برابر با {MinLength} کاراکتر باشد.",
			"MaximumLength_Simple" => "'{PropertyName}' باید کمتر یا مساوی {MaxLength} باشد.",
			"ExactLength_Simple" => "'{PropertyName}' باید دقیقا {MaxLength} کاراکتر.",
			"InclusiveBetween_Simple" => "'{PropertyName}' باید بین {From} و {To} باشد.",
			_ => null,
		};
	}
}
