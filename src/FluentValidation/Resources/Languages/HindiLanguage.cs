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

	internal class HindiLanguage {
		public const string Culture = "hi";

		public static string GetTranslation(string key) => key switch {
			"EmailValidator" => "'{PropertyName}' मान्य ईमेल एड्रेस नहीं है।",
			"GreaterThanOrEqualValidator" => "'{PropertyName}' '{ComparisonValue}' से अधिक या के उसके बराबर होनी चाहिए।",
			"GreaterThanValidator" => "'{PropertyName}' '{ComparisonValue}' से अधिक होनी चाहिए।",
			"LengthValidator" => "'{PropertyName}' {MinLength} और {MaxLength} अक्षरों के बीच होना चाहिए। आपने {TotalLength} अक्षर दर्ज किए हैं।",
			"MinimumLengthValidator" => "'{PropertyName}' {MinLength} वर्णों से अधिक या उसके बराबर होना चाहिए। आपने {TotalLength} वर्णों को दर्ज किया है",
			"MaximumLengthValidator" => "'{PropertyName}' {MaxLength} वर्णों से कम या उसके बराबर होना चाहिए। आपने {TotalLength} वर्णों को दर्ज किया है",
			"LessThanOrEqualValidator" => "'{PropertyName}' '{ComparisonValue}' से कम या के उसके बराबर होनी चाहिए।",
			"LessThanValidator" => "'{PropertyName}' '{ComparisonValue}' से कम होनी चाहिए।",
			"NotEmptyValidator" => "'{PropertyName}' खाली नहीं होना चाहिए।",
			"NotEqualValidator" => "'{PropertyName}' '{ComparisonValue}' से बराबर नहीं होना चाहिए।",
			"NotNullValidator" => "'{PropertyName}' खाली नहीं होना चाहिए।",
			"PredicateValidator" => "निर्दिष्ट स्थिति को '{PropertyName}' के लिए पूरा नहीं किया गया।",
			"AsyncPredicateValidator" => "निर्दिष्ट स्थिति को '{PropertyName}' के लिए पूरा नहीं किया गया।",
			"RegularExpressionValidator" => "'{PropertyName}' सही प्रारूप में नहीं है।",
			"EqualValidator" => "'{PropertyName}' '{ComparisonValue}' से बराबर होना चाहिए।",
			"ExactLengthValidator" => "'{PropertyName}' {MaxLength} अक्षरों के उसके बराबर होनी चाहिए। आपने {TotalLength} अक्षर दर्ज किए हैं।",
			"InclusiveBetweenValidator" => "'{PropertyName}' {From} और {To} के बीच में होनी चाहिए।. आपने {PropertyValue} दर्ज किया है।",
			"ExclusiveBetweenValidator" => "'{PropertyName}' {From} और {To} (अनन्य) के बीच में होनी चाहिए।. आपने {PropertyValue} दर्ज किया है।",
			"CreditCardValidator" => "'{PropertyName}' मान्य क्रेडिट कार्ड नंबर नहीं है।",
			"ScalePrecisionValidator" => "'{PropertyName}' कुल में {ExpectedPrecision} अंकों से अधिक नहीं हो सकता है, {ExpectedScale} दशमलव के के साथ।. {Digits} अंक और {ActualScale} दशमलव पाए गए है।",
			"EmptyValidator" => "'{PropertyName}' खाली होना चाहिए।",
			"NullValidator" => "'{PropertyName}' खाली होना चाहिए।",
			"EnumValidator" => "'{PropertyName}' में कई मान हैं जिनमें '{PropertyValue}' शामिल नहीं है।",
			// Additional fallback messages used by clientside validation integration.
			"Length_Simple" => "'{PropertyName}' {MinLength} और {MaxLength} अक्षरों के बीच होना चाहिए।",
			"MinimumLength_Simple" => "'{PropertyName}' {MinLength} वर्णों से अधिक या उसके बराबर होना चाहिए।",
			"MaximumLength_Simple" => "'{PropertyName}' {MaxLength} वर्णों से कम या उसके बराबर होना चाहिए।",
			"ExactLength_Simple" => "'{PropertyName}' {MaxLength} अक्षरों के उसके बराबर होनी चाहिए।",
			"InclusiveBetween_Simple" => "'{PropertyName}' {From} और {To} के बीच में होनी चाहिए।",
			_ => null,
		};
	}
}
