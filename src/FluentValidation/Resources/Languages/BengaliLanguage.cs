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
	internal class BengaliLanguage {
		public const string Culture = "bn";

		public static string GetTranslation(string key) => key switch {
			nameof(EmailValidator) => "'{PropertyName}' বৈধ ইমেইল ঠিকানা নয়।",
			nameof(GreaterThanOrEqualValidator) => "'{PropertyName}' অবশ্যই '{ComparisonValue}' এর সমান অথবা বেশি হবে।",
			nameof(GreaterThanValidator) => "'{PropertyName}' অবশ্যই '{ComparisonValue}' এর বেশি হবে।",
			nameof(LengthValidator) => "'{PropertyName}' এর অক্ষর সংখ্যা অবশ্যই {MinLength} থেকে {MaxLength} এর মধ্যে হবে। আপনি {TotalLength}টি অক্ষর প্রদান করেছেন।",
			nameof(MinimumLengthValidator) => "'{PropertyName}' এর অক্ষর সংখ্যা কমপক্ষে {MinLength} অথবা এর চেয়ে বেশি হবে। আপনি {TotalLength}টি অক্ষর প্রদান করেছেন।",
			nameof(MaximumLengthValidator) => "'{PropertyName}' এর অক্ষর সংখ্যা সর্বোচ্চ {MaxLength} অথবা এর চেয়ে কম হবে। আপনি {TotalLength}টি অক্ষর প্রদান করেছেন।",
			nameof(LessThanOrEqualValidator) => "'{PropertyName}' অবশ্যই '{ComparisonValue}' এর সমান অথবা কম হবে।",
			nameof(LessThanValidator) => "'{PropertyName}' অবশ্যই '{ComparisonValue}' এর চেয়ে কম হবে।",
			nameof(NotEmptyValidator) => "'{PropertyName}' খালি হতে পারবে না।",
			nameof(NotEqualValidator) => "'{PropertyName}' '{ComparisonValue}' হতে পারবেনা।",
			nameof(NotNullValidator) => "'{PropertyName}' খালি হতে পারবে না।",
			nameof(PredicateValidator) => "নির্ধারিত শর্তটি '{PropertyName}' এর জন্য মেটেনি।",
			nameof(AsyncPredicateValidator) => "নির্ধারিত শর্তটি '{PropertyName}' এর জন্য মেটেনি।",
			nameof(RegularExpressionValidator) => "'{PropertyName}' সঠিক বিন্যাসে নেই।",
			nameof(EqualValidator) => "'{PropertyName}' অবশ্যই '{ComparisonValue}' এর সমান হবে।",
			nameof(ExactLengthValidator) => "'{PropertyName}' এর অক্ষর সংখ্যা অবশ্যই {MaxLength}টি হবে। আপনি {TotalLength}টি অক্ষর প্রদান করেছেন।",
			nameof(InclusiveBetweenValidator) => "'{PropertyName}' অবশ্যই {From} থেকে {To} এর মধ্যে হবে। আপনি {Value} প্রদান করেছেন।",
			nameof(ExclusiveBetweenValidator) => "'{PropertyName}' অবশ্যই {From} থেকে {To} এর বাহিরে হবে না। আপনি {Value} প্রদান করেছেন।",
			nameof(CreditCardValidator) => "'{PropertyName}' বৈধ ক্রেডিট কার্ড সংখ্যা নয়।",
			nameof(ScalePrecisionValidator) => "'{PropertyName}' মোট {ExpectedPrecision} অঙ্কের বেশি হবে না। {ExpectedScale} বৈধ দশমাংশ, কিন্তু প্রদত্ত {Digits} সংখ্যাটি {ActualScale} দশমাংশের",
			nameof(EmptyValidator) => "'{PropertyName}' অবশ্যই খালি হবে।",
			nameof(NullValidator) => "'{PropertyName}' অবশ্যই খালি হবে।",
			nameof(EnumValidator) => "'{PropertyValue}' '{PropertyName}' এর সীমা লঙ্ঘন করে।",
			// Additional fallback messages used by clientside validation integration.
			"Length_Simple" => "'{PropertyName}' এর অক্ষর সংখ্যা অবশ্যই {MinLength} থেকে {MaxLength} এর মধ্যে হবে।",
			"MinimumLength_Simple" => "'{PropertyName}' এর অক্ষর সংখ্যা কমপক্ষে {MinLength} অথবা এর চেয়ে বেশি হবে।",
			"MaximumLength_Simple" => "'{PropertyName}' এর অক্ষর সংখ্যা সর্বোচ্চ {MaxLength}টি অথবা এর চেয়ে কম হবে।",
			"ExactLength_Simple" => "'{PropertyName}' এর অক্ষর সংখ্যা অবশ্যই {MaxLength}টি হবে।",
			"InclusiveBetween_Simple" => "'{PropertyName}' অবশ্যই {From} থেকে {To} এর মধ্যে হবে।",
			_ => null,
		};
	}
}
