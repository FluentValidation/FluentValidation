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

	internal class HebrewLanguage : Language {
		public const string Culture = "he";
		public override string Name => Culture;

		public HebrewLanguage() {
			Translate<EmailValidator>("'{PropertyName}' אינה כתובת דוא\"ל חוקית.");
			Translate<GreaterThanOrEqualValidator>("'{PropertyName}' חייב להיות גדול או שווה ל- '{ComparisonValue}'.");
			Translate<GreaterThanValidator>("'{PropertyName}' חייב להיות גדול מ- '{ComparisonValue}'.");
			Translate<LengthValidator>("אורך '{PropertyName}' חייב להיות בין {MinLength} ל- {MaxLength}. הזנת {TotalLength} תווים.");
			Translate<MinimumLengthValidator>("אורך '{PropertyName}' חייב להיות לפחות {MinLength} תווים. הזנת {TotalLength} תווים.");
			Translate<MaximumLengthValidator>("אורך '{PropertyName}' חייב להיות {MaxLength} תווים או פחות. הזנת {TotalLength} תווים.");
			Translate<LessThanOrEqualValidator>("'{PropertyName}' חייב להיות קטן או שווה ל- '{ComparisonValue}'.");
			Translate<LessThanValidator>("'{PropertyName}' חייב להיות קטן מ- '{ComparisonValue}'.");
			Translate<NotEmptyValidator>("'{PropertyName}' לא אמור להיות ריק.");
			Translate<NotEqualValidator>("'{PropertyName}' לא יכול להיות שווה ל- '{ComparisonValue}'.");
			Translate<NotNullValidator>("'{PropertyName}' לא יכול להיות ריק.");
			Translate<PredicateValidator>("התנאי שצוין לא התקיים עבור '{PropertyName}'.");
			Translate<AsyncPredicateValidator>("התנאי שצוין לא התקיים עבור '{PropertyName}'.");
			Translate<RegularExpressionValidator>("'{PropertyName}' אינו בפורמט הנכון.");
			Translate<EqualValidator>("'{PropertyName}' אמור להיות שווה ל- '{ComparisonValue}'.");
			Translate<ExactLengthValidator>("'{PropertyName}' חייב להיות באורך {MaxLength} תווים. הזנת {TotalLength} תווים.");
			Translate<InclusiveBetweenValidator>("'{PropertyName}' חייב להיות בין {From} לבין {To}. הזנת {Value}.");
			Translate<ExclusiveBetweenValidator>("'{PropertyName}' חייב להיות בין {From} לבין {To} (לא כולל). הזנת {Value}.");
			Translate<CreditCardValidator>("'{PropertyName}' אינו מספר כרטיס אשראי חוקי.");
			Translate<ScalePrecisionValidator>("'{PropertyName}' לא יכול לכלול יותר מ- {ExpectedPrecision} ספרות בסך הכל, עם הקצבה של {ExpectedScale} ספרות עשרוניות. נמצאו {Digits} ספרות ו- {ActualScale} ספרות עשרוניות.");
			Translate<EmptyValidator>("'{PropertyName}' אמור להיות ריק.");
			Translate<NullValidator>("'{PropertyName}' חייב להיות ריק.");
			Translate<EnumValidator>("'{PropertyName}' מכיל טווח ערכים שאינו כולל את '{PropertyValue}'.");
			// Additional fallback messages used by clientside validation integration.
			Translate("Length_Simple", "אורך '{PropertyName}' חייב להיות בין {MinLength} ל- {MaxLength}.");
			Translate("MinimumLength_Simple", "אורך '{PropertyName}' חייב להיות לפחות {MinLength} תווים.");
			Translate("MaximumLength_Simple", "אורך '{PropertyName}' חייב להיות {MaxLength} תווים או פחות.");
			Translate("ExactLength_Simple", "'{PropertyName}' חייב להיות באורך {MaxLength} תווים.");
			Translate("InclusiveBetween_Simple", "'{PropertyName}' חייב להיות בין {From} לבין {To}.");
		}
	}
}
