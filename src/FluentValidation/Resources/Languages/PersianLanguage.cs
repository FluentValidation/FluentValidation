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

	internal class PersianLanguage : Language {
		public override string Name => "fa";

		public PersianLanguage() {
			Translate<EmailValidator>("'{PropertyName}' وارد شده قالب صحیح یک ایمیل را ندارد.");
			Translate<GreaterThanOrEqualValidator>("'{PropertyName}' باید بیشتر یا مساوی '{ComparisonValue}' باشد.");
			Translate<GreaterThanValidator>("'{PropertyName}' باید بیشتر از '{ComparisonValue}' باشد.");
			Translate<LengthValidator>("'{PropertyName}' باید حداقل {MinLength} و حداکثر {MaxLength} کاراکتر داشته باشد. اما مقدار وارد شده {TotalLength} کاراکتر دارد.");
			Translate<MinimumLengthValidator>("'{PropertyName}' باید بزرگتر یا برابر با {MinLength} کاراکتر باشد. شما شخصیت {TotalLength} را وارد کردید");
			Translate<MaximumLengthValidator>("'{PropertyName}' باید کمتر یا مساوی {MaxLength} باشد. {TotalLength} را وارد کردید");
			Translate<LessThanOrEqualValidator>("'{PropertyName}' باید کمتر یا مساوی '{ComparisonValue}' باشد.");
			Translate<LessThanValidator>("'{PropertyName}' باید کمتر از '{ComparisonValue}' باشد.");
			Translate<NotEmptyValidator>("وارد کردن '{PropertyName}' ضروری است.");
			Translate<NotEqualValidator>("'{PropertyName}' نباید برابر با '{ComparisonValue}' باشد.");
			Translate<NotNullValidator>("وارد کردن '{PropertyName}' ضروری است.");
			Translate<PredicateValidator>("شرط تعیین شده برای '{PropertyName}' برقرار نیست.");
			Translate<AsyncPredicateValidator>("شرط تعیین شده برای '{PropertyName}' برقرار نیست.");
			Translate<RegularExpressionValidator>("'{PropertyName}' دارای قالب صحیح نیست.");
			Translate<EqualValidator>("مقادیر وارد شده برای '{PropertyName}' و '{ComparisonValue}' یکسان نیستند.");
			Translate<ExactLengthValidator>("'{PropertyName}' باید دقیقا {MaxLength} کاراکتر باشد اما مقدار وارد شده {TotalLength} کاراکتر دارد.");
			Translate<InclusiveBetweenValidator>("'{PropertyName}' باید بین {From} و {To} باشد. اما مقدار وارد شده ({Value}) در این محدوده نیست.");
			Translate<ExclusiveBetweenValidator>("'{PropertyName}' باید بیشتر از {From} و کمتر از {To} باشد. اما مقدار وارد شده ({Value}) در این محدوده نیست.");
			Translate<CreditCardValidator>("'{PropertyName}' وارد شده معتبر نیست.");
			Translate<ScalePrecisionValidator>("'{PropertyName}' نباید بیش از {expectedPrecision} رقم، شامل {expectedScale} رقم اعشار داشته باشد. مقدار وارد شده {digits} رقم و {actualScale} رقم اعشار دارد.");
			Translate<EmptyValidator>("'{PropertyName}' باید خالی باشد.");
			Translate<NullValidator>("'{PropertyName}' باید خالی باشد.");

		}
	}
}