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

internal class TeluguLanguage {
	public const string Culture = "te";

	public static string GetTranslation(string key) => key switch {
		"EmailValidator" => "'{PropertyName}' చెల్లుబాటు అయ్యే ఈమెయిల్ చిరునామా కాదు.",
		"GreaterThanOrEqualValidator" => "'{PropertyName}' తప్పనిసరిగా '{ComparisonValue}' కంటే ఎక్కువ లేదా సమానం అయ్యి ఉండాలి.",
		"GreaterThanValidator" => "'{PropertyName}' తప్పనిసరిగా '{ComparisonValue}' కంటే ఎక్కువ అయ్యి ఉండాలి.",
		"LengthValidator" => "'{PropertyName}' {MinLength} మరియు {MaxLength} అక్షరాల మధ్య ఉండాలి. మీరు {TotalLength} అక్షరాలు నమోదు చేశారు.",
		"MinimumLengthValidator" => "'{PropertyName}' యొక్క పొడవు కనీసం {MinLength} అక్షరాలు ఉండాలి. మీరు {TotalLength} అక్షరాలు నమోదు చేశారు.",
		"MaximumLengthValidator" => "'{PropertyName}' యొక్క పొడవు {MaxLength} అక్షరాలు లేదా అంతకంటే తక్కువ ఉండాలి. మీరు {TotalLength} అక్షరాలు నమోదు చేశారు.",
		"LessThanOrEqualValidator" => "'{PropertyName}' తప్పనిసరిగా '{ComparisonValue}' కంటే తక్కువ లేదా సమానం అయ్యి ఉండాలి.",
		"LessThanValidator" => "'{PropertyName}' తప్పనిసరిగా '{ComparisonValue}' కంటే తక్కువ అయ్యి ఉండాలి.",
		"NotEmptyValidator" => "'{PropertyName}' ఖాళీగా ఉండకూడదు.",
		"NotEqualValidator" => "'{PropertyName}' తప్పనిసరిగా '{ComparisonValue}' కి సమానం అయ్యి ఉండకూడదు.",
		"NotNullValidator" => "'{PropertyName}' ఖాళీగా ఉండకూడదు.",
		"PredicateValidator" => "'{PropertyName}' కోసం పేర్కొన్న షరతు తీర్చబడలేదు.",
		"AsyncPredicateValidator" => "'{PropertyName}' కోసం పేర్కొన్న షరతు తీర్చబడలేదు.",
		"RegularExpressionValidator" => "'{PropertyName}' సరైన ఫార్మాట్‌లో లేదు.",
		"EqualValidator" => "'{PropertyName}' తప్పనిసరిగా '{ComparisonValue}' కి సమానం అయ్యి ఉండాలి.",
		"ExactLengthValidator" => "'{PropertyName}' తప్పనిసరిగా {MaxLength} అక్షరాల పొడవు ఉండాలి. మీరు {TotalLength} అక్షరాలు నమోదు చేశారు.",
		"InclusiveBetweenValidator" => "'{PropertyName}' తప్పనిసరిగా {From} మరియు {To} మధ్య ఉండాలి. మీరు {PropertyValue} నమోదు చేశారు.",
		"ExclusiveBetweenValidator" => "'{PropertyName}' తప్పనిసరిగా {From} మరియు {To} మధ్య (ఎక్స్‌క్లూజివ్) ఉండాలి. మీరు {PropertyValue} నమోదు చేశారు.",
		"CreditCardValidator" => "'{PropertyName}' చెల్లుబాటు అయ్యే క్రెడిట్ కార్డ్ నంబర్ కాదు.",
		"ScalePrecisionValidator" => "'{PropertyName}' మొత్తం {ExpectedPrecision} అంకెలకు మించి ఉండకూడదు, {ExpectedScale} దశాంశాలకు అనుమతి ఉంది. {Digits} అంకెలు మరియు {ActualScale} దశాంశాలు కనుగొనబడ్డాయి.",
		"EmptyValidator" => "'{PropertyName}' ఖాళీగా ఉండాలి.",
		"NullValidator" => "'{PropertyName}' ఖాళీగా ఉండాలి.",
		"EnumValidator" => "'{PropertyName}' లో '{PropertyValue}' చేర్చబడని విలువల పరిధి ఉంది.",
		"GuidValidator" => "'{PropertyName}' చెల్లుబాటు అయ్యే GUID కాదు.",
		// Additional fallback messages used by clientside validation integration.
		"Length_Simple" => "'{PropertyName}' {MinLength} మరియు {MaxLength} అక్షరాల మధ్య ఉండాలి.",
		"MinimumLength_Simple" => "'{PropertyName}' యొక్క పొడవు కనీసం {MinLength} అక్షరాలు ఉండాలి.",
		"MaximumLength_Simple" => "'{PropertyName}' యొక్క పొడవు {MaxLength} అక్షరాలు లేదా అంతకంటే తక్కువ ఉండాలి.",
		"ExactLength_Simple" => "'{PropertyName}' తప్పనిసరిగా {MaxLength} అక్షరాల పొడవు ఉండాలి.",
		"InclusiveBetween_Simple" => "'{PropertyName}' తప్పనిసరిగా {From} మరియు {To} మధ్య ఉండాలి.",
		_ => null,
	};
}
