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

	internal class ArabicLanguage : Language {
		public const string Culture = "ar";
		public override string Name => Culture;

		public ArabicLanguage() {
			Translate<EmailValidator>("'{PropertyName}' ليس بريد الكتروني صحيح.");
			Translate<GreaterThanOrEqualValidator>("'{PropertyName}' يجب أن يكون أكبر من أو يساوي '{ComparisonValue}'.");
			Translate<GreaterThanValidator>("'{PropertyName}' يجب أن يكون أكبر من '{ComparisonValue}'.");
			Translate<LengthValidator>("'{PropertyName}' عدد الحروف يجب أن يكون بين {MinLength} و {MaxLength}. عدد ما تم ادخاله {TotalLength}.");
			Translate<MinimumLengthValidator>("الحد الأدنى لعدد الحروف في '{PropertyName}' هو {MinLength}. عدد ما تم ادخاله {TotalLength}.");
			Translate<MaximumLengthValidator>("الحد الأقصى لعدد الحروف في '{PropertyName}' هو {MaxLength}. عدد ما تم ادخاله {TotalLength}.");
			Translate<LessThanOrEqualValidator>("'{PropertyName}' يجب أن يكون أقل من أو يساوي '{ComparisonValue}'.");
			Translate<LessThanValidator>("'{PropertyName}' يجب أن يكون أقل من '{ComparisonValue}'.");
			Translate<NotEmptyValidator>("'{PropertyName}' لا يجب أن يكون فارغاً.");
			Translate<NotEqualValidator>("'{PropertyName}' يجب ألا يساوي '{ComparisonValue}'.");
			Translate<NotNullValidator>("'{PropertyName}' لا يجب أن يكون فارغاً.");
			Translate<PredicateValidator>("الشرط المحدد لا يتفق مع '{PropertyName}'.");
			Translate<AsyncPredicateValidator>("الشرط المحدد لا يتفق مع '{PropertyName}'.");
			Translate<RegularExpressionValidator>("'{PropertyName}' ليس بالتنسيق الصحيح.");
			Translate<EqualValidator>("'{PropertyName}' يجب أن يساوي '{ComparisonValue}'.");
			Translate<ExactLengthValidator>("الحد الأقصى لعدد الحروف في '{PropertyName}' هو {MaxLength}. عدد ما تم ادخاله {TotalLength}.");
			Translate<InclusiveBetweenValidator>("'{PropertyName}' يجب أن يكون بين {From} و {To}. ما تم ادخاله {Value}.");
			Translate<ExclusiveBetweenValidator>("'{PropertyName}' يجب أن يكون بين {From} و {To} (حصرياً). ما تم ادخاله {Value}.");
			Translate<CreditCardValidator>("'{PropertyName}' ليس رقم بطاقة ائتمان صحيح.");
			Translate<ScalePrecisionValidator>("'{PropertyName}' لا يجب أن يكون أكبر من {ExpectedPrecision} رقما صحيحاً في المجمل, ومسموح بـ {ExpectedScale} أرقام عشرية. ما تم ادخاله {Digits} أرقام صحيحة و {ActualScale} أرقام عشرية.");
			Translate<EmptyValidator>("'{PropertyName}' يجب أن يكون فارغاً.");
			Translate<NullValidator>("'{PropertyName}' يجب أن يكون فارغاً.");
			Translate<EnumValidator>("'{PropertyName}' يحتوي على مجموعة من القيم التي لا تتضمن '{PropertyValue}'.");
			// Additional fallback messages used by clientside validation integration.
			Translate("Length_Simple", "'{PropertyName}' عدد الحروف يجب أن يكون بين {MinLength} و {MaxLength}.");
			Translate("MinimumLength_Simple", "الحد الأدنى لعدد الحروف في '{PropertyName}' هو {MinLength}.");
			Translate("MaximumLength_Simple", "الحد الأقصى لعدد الحروف في '{PropertyName}' هو {MaxLength}.");
			Translate("ExactLength_Simple", "الحد الأقصى لعدد الحروف في '{PropertyName}' هو {MaxLength}.");
			Translate("InclusiveBetween_Simple", "'{PropertyName}' يجب أن يكون بين {From} و {To}.");
		}
	}
}
