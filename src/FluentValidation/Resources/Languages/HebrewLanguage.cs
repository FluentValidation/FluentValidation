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

	internal class HebrewLanguage {
		public const string Culture = "he";

		public static string GetTranslation(string key) => key switch {
			nameof(EmailValidator) => "'{PropertyName}' אינה כתובת דוא\"ל חוקית.",
			nameof(GreaterThanOrEqualValidator) => "'{PropertyName}' חייב להיות גדול או שווה ל- '{ComparisonValue}'.",
			nameof(GreaterThanValidator) => "'{PropertyName}' חייב להיות גדול מ- '{ComparisonValue}'.",
			nameof(LengthValidator) => "אורך '{PropertyName}' חייב להיות בין {MinLength} ל- {MaxLength}. הזנת {TotalLength} תווים.",
			nameof(MinimumLengthValidator) => "אורך '{PropertyName}' חייב להיות לפחות {MinLength} תווים. הזנת {TotalLength} תווים.",
			nameof(MaximumLengthValidator) => "אורך '{PropertyName}' חייב להיות {MaxLength} תווים או פחות. הזנת {TotalLength} תווים.",
			nameof(LessThanOrEqualValidator) => "'{PropertyName}' חייב להיות קטן או שווה ל- '{ComparisonValue}'.",
			nameof(LessThanValidator) => "'{PropertyName}' חייב להיות קטן מ- '{ComparisonValue}'.",
			nameof(NotEmptyValidator) => "'{PropertyName}' לא אמור להיות ריק.",
			nameof(NotEqualValidator) => "'{PropertyName}' לא יכול להיות שווה ל- '{ComparisonValue}'.",
			nameof(NotNullValidator) => "'{PropertyName}' לא יכול להיות ריק.",
			nameof(PredicateValidator) => "התנאי שצוין לא התקיים עבור '{PropertyName}'.",
			nameof(AsyncPredicateValidator) => "התנאי שצוין לא התקיים עבור '{PropertyName}'.",
			nameof(RegularExpressionValidator) => "'{PropertyName}' אינו בפורמט הנכון.",
			nameof(EqualValidator) => "'{PropertyName}' אמור להיות שווה ל- '{ComparisonValue}'.",
			nameof(ExactLengthValidator) => "'{PropertyName}' חייב להיות באורך {MaxLength} תווים. הזנת {TotalLength} תווים.",
			nameof(InclusiveBetweenValidator) => "'{PropertyName}' חייב להיות בין {From} לבין {To}. הזנת {Value}.",
			nameof(ExclusiveBetweenValidator) => "'{PropertyName}' חייב להיות בין {From} לבין {To} (לא כולל). הזנת {Value}.",
			nameof(CreditCardValidator) => "'{PropertyName}' אינו מספר כרטיס אשראי חוקי.",
			nameof(ScalePrecisionValidator) => "'{PropertyName}' לא יכול לכלול יותר מ- {ExpectedPrecision} ספרות בסך הכל, עם הקצבה של {ExpectedScale} ספרות עשרוניות. נמצאו {Digits} ספרות ו- {ActualScale} ספרות עשרוניות.",
			nameof(EmptyValidator) => "'{PropertyName}' אמור להיות ריק.",
			nameof(NullValidator) => "'{PropertyName}' חייב להיות ריק.",
			nameof(EnumValidator) => "'{PropertyName}' מכיל טווח ערכים שאינו כולל את '{PropertyValue}'.",
			// Additional fallback messages used by clientside validation integration.
			"Length_Simple" => "אורך '{PropertyName}' חייב להיות בין {MinLength} ל- {MaxLength}.",
			"MinimumLength_Simple" => "אורך '{PropertyName}' חייב להיות לפחות {MinLength} תווים.",
			"MaximumLength_Simple" => "אורך '{PropertyName}' חייב להיות {MaxLength} תווים או פחות.",
			"ExactLength_Simple" => "'{PropertyName}' חייב להיות באורך {MaxLength} תווים.",
			"InclusiveBetween_Simple" => "'{PropertyName}' חייב להיות בין {From} לבין {To}.",
			_ => null,
		};
	}
}
