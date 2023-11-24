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

namespace FluentValidation.Resources;

internal class KhmerLanguage {
	public const string Culture = "km";

	public static string GetTranslation(string key) => key switch {
		"EmailValidator" => "'{PropertyName}'មិនមែនជាអ៊ីមែលត្រឹមត្រូវទេ។",
		"GreaterThanOrEqualValidator" => "'{PropertyName}'ត្រូវតែធំជាង ឬស្មើ'{ComparisonValue}។'",
		"GreaterThanValidator" => "'{PropertyName}'ត្រូវតែធំជាង'{ComparisonValue}'។",
		"LengthValidator" => "'{PropertyName}'ត្រូវតែនៅចន្លោះពី{MinLength}ទៅ{MaxLength}តួអក្សរ។ អ្នកបានបញ្ចូល{TotalLength}តួអក្សរ។",
		"MinimumLengthValidator" => "ប្រវែង'{PropertyName}'ត្រូវតែមានយ៉ាងហោចណាស់{MinLength}តួអក្សរ។ អ្នកបានបញ្ចូល{TotalLength}តួអក្សរ។",
		"MaximumLengthValidator" => "ប្រវែង'{PropertyName}'ត្រូវតែមាន{MaxLength}តួអក្សរ ឬតិចជាងនេះ។ អ្នកបានបញ្ចូល{TotalLength}តួអក្សរ។",
		"LessThanOrEqualValidator" => "'{PropertyName}'ត្រូវតែតិចជាង ឬស្មើនឹង'{ComparisonValue}'។",
		"LessThanValidator" => "'{PropertyName}'ត្រូវតែតិចជាង'{ComparisonValue}'។",
		"NotEmptyValidator" => "'{PropertyName}'មិនត្រូវទទេទេ។",
		"NotEqualValidator" => "'{PropertyName}'មិនត្រូវស្មើនឹង'{ComparisonValue}'ទេ។",
		"NotNullValidator" => "'{PropertyName}'មិនត្រូវទទេទេ។",
		"PredicateValidator" => "លក្ខខណ្ឌដែលបានបញ្ជាក់មិនត្រូវបានបំពេញសម្រាប់'{PropertyName}'ទេ។",
		"AsyncPredicateValidator" => "លក្ខខណ្ឌដែលបានបញ្ជាក់មិនត្រូវបានបំពេញសម្រាប់'{PropertyName}'ទេ។",
		"RegularExpressionValidator" => "'{PropertyName}'មិនមានទម្រង់ត្រឹមត្រូវទេ។",
		"EqualValidator" => "'{PropertyName}'ត្រូវតែស្មើនឹង'{ComparisonValue}'។",
		"ExactLengthValidator" => "'{PropertyName}'ត្រូវតែមាន{MaxLength}តួអក្សរ។ អ្នកបានបញ្ចូល{TotalLength}តួអក្សរ។",
		"InclusiveBetweenValidator" => "'{PropertyName}'ត្រូវតែស្ថិតនៅចន្លោះ{From}និង{To}។ អ្នកបានបញ្ចូល{PropertyValue}។",
		"ExclusiveBetweenValidator" => "'{PropertyName}'ត្រូវតែស្ថិតនៅចន្លោះ{From}និង {To}(ដាច់ខាត)។ អ្នកបានបញ្ចូល{PropertyValue}។",
		"CreditCardValidator" => "'{PropertyName}'មិនមែនជាលេខកាតឥណទានត្រឹមត្រូវទេ។",
		"ScalePrecisionValidator" => "'{PropertyName}'មិនត្រូវលើសពី{ExpectedPrecision}ខ្ទង់ជាសរុប ដោយមានការអនុញ្ញាតសម្រាប់ទសភាគ{ExpectedScale}។ ទសភាគ{Digits}ខ្ទង់និង {ActualScale}ត្រូវបានរកឃើញ។",
		"EmptyValidator" => "លក្ខខណ្ឌដែលបានបញ្ជាក់មិនត្រូវបានបំពេញសម្រាប់'{PropertyName}'ទេ។",
		"NullValidator" => "លក្ខខណ្ឌដែលបានបញ្ជាក់មិនត្រូវបានបំពេញសម្រាប់'{PropertyName}'ទេ។",
		"EnumValidator" => "'{PropertyName}'មានជួរតម្លៃដែលមិនរួមបញ្ចូល'{PropertyValue}'។",
		// Additional fallback messages used by clientside validation integration.
		"Length_Simple" => "'{PropertyName}'ត្រូវតែស្ថិតនៅចន្លោះតួអក្សរ{MinLength}ទៅ {MaxLength}តួអក្សរ។",
		"MinimumLength_Simple" => "ប្រវែង'{PropertyName}'ត្រូវតែមានយ៉ាងហោចណាស់{MinLength}តួអក្សរ។",
		"MaximumLength_Simple" => "ប្រវែង'{PropertyName}'ត្រូវតែមាន{MaxLength}តួអក្សរ ឬតិចជាងនេះ។",
		"ExactLength_Simple" => "'{PropertyName}'ត្រូវតែមាន{MaxLength}តួអក្សរ។",
		"InclusiveBetween_Simple" => "'{PropertyName}'ត្រូវតែស្ថិតនៅចន្លោះ{From}និង{To}។",
		_ => null,
	};
}
