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

	internal class ArabicLanguage {
		public const string Culture = "ar";

		public static string GetTranslation(string key) => key switch {
			nameof(EmailValidator) => "'{PropertyName}' ليس بريد الكتروني صحيح.",
			nameof(GreaterThanOrEqualValidator) => "'{PropertyName}' يجب أن يكون أكبر من أو يساوي '{ComparisonValue}'.",
			nameof(GreaterThanValidator) => "'{PropertyName}' يجب أن يكون أكبر من '{ComparisonValue}'.",
			nameof(LengthValidator) => "'{PropertyName}' عدد الحروف يجب أن يكون بين {MinLength} و {MaxLength}. عدد ما تم ادخاله {TotalLength}.",
			nameof(MinimumLengthValidator) => "الحد الأدنى لعدد الحروف في '{PropertyName}' هو {MinLength}. عدد ما تم ادخاله {TotalLength}.",
			nameof(MaximumLengthValidator) => "الحد الأقصى لعدد الحروف في '{PropertyName}' هو {MaxLength}. عدد ما تم ادخاله {TotalLength}.",
			nameof(LessThanOrEqualValidator) => "'{PropertyName}' يجب أن يكون أقل من أو يساوي '{ComparisonValue}'.",
			nameof(LessThanValidator) => "'{PropertyName}' يجب أن يكون أقل من '{ComparisonValue}'.",
			nameof(NotEmptyValidator) => "'{PropertyName}' لا يجب أن يكون فارغاً.",
			nameof(NotEqualValidator) => "'{PropertyName}' يجب ألا يساوي '{ComparisonValue}'.",
			nameof(NotNullValidator) => "'{PropertyName}' لا يجب أن يكون فارغاً.",
			nameof(PredicateValidator) => "الشرط المحدد لا يتفق مع '{PropertyName}'.",
			nameof(AsyncPredicateValidator) => "الشرط المحدد لا يتفق مع '{PropertyName}'.",
			nameof(RegularExpressionValidator) => "'{PropertyName}' ليس بالتنسيق الصحيح.",
			nameof(EqualValidator) => "'{PropertyName}' يجب أن يساوي '{ComparisonValue}'.",
			nameof(ExactLengthValidator) => "الحد الأقصى لعدد الحروف في '{PropertyName}' هو {MaxLength}. عدد ما تم ادخاله {TotalLength}.",
			nameof(InclusiveBetweenValidator) => "'{PropertyName}' يجب أن يكون بين {From} و {To}. ما تم ادخاله {Value}.",
			nameof(ExclusiveBetweenValidator) => "'{PropertyName}' يجب أن يكون بين {From} و {To} (حصرياً). ما تم ادخاله {Value}.",
			nameof(CreditCardValidator) => "'{PropertyName}' ليس رقم بطاقة ائتمان صحيح.",
			nameof(ScalePrecisionValidator) => "'{PropertyName}' لا يجب أن يكون أكبر من {ExpectedPrecision} رقما صحيحاً في المجمل, ومسموح بـ {ExpectedScale} أرقام عشرية. ما تم ادخاله {Digits} أرقام صحيحة و {ActualScale} أرقام عشرية.",
			nameof(EmptyValidator) => "'{PropertyName}' يجب أن يكون فارغاً.",
			nameof(NullValidator) => "'{PropertyName}' يجب أن يكون فارغاً.",
			nameof(EnumValidator) => "'{PropertyName}' يحتوي على مجموعة من القيم التي لا تتضمن '{PropertyValue}'.",
			// Additional fallback messages used by clientside validation integration.
			"Length_Simple" => "'{PropertyName}' عدد الحروف يجب أن يكون بين {MinLength} و {MaxLength}.",
			"MinimumLength_Simple" => "الحد الأدنى لعدد الحروف في '{PropertyName}' هو {MinLength}.",
			"MaximumLength_Simple" => "الحد الأقصى لعدد الحروف في '{PropertyName}' هو {MaxLength}.",
			"ExactLength_Simple" => "الحد الأقصى لعدد الحروف في '{PropertyName}' هو {MaxLength}.",
			"InclusiveBetween_Simple" => "'{PropertyName}' يجب أن يكون بين {From} و {To}.",
			_ => null,
		};
	}
}
