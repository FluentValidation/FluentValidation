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
			nameof(EmailValidator) => "'{PropertyName}' मान्य ईमेल एड्रेस नहीं है।",
			nameof(GreaterThanOrEqualValidator) => "'{PropertyName}' '{ComparisonValue}' से अधिक या के उसके बराबर होनी चाहिए।",
			nameof(GreaterThanValidator) => "'{PropertyName}' '{ComparisonValue}' से अधिक होनी चाहिए।",
			nameof(LengthValidator) => "'{PropertyName}' {MinLength} और {MaxLength} अक्षरों के बीच होना चाहिए। आपने {TotalLength} अक्षर दर्ज किए हैं।",
			nameof(MinimumLengthValidator) => "'{PropertyName}' {MinLength} वर्णों से अधिक या उसके बराबर होना चाहिए। आपने {TotalLength} वर्णों को दर्ज किया है",
			nameof(MaximumLengthValidator) => "'{PropertyName}' {MaxLength} वर्णों से कम या उसके बराबर होना चाहिए। आपने {TotalLength} वर्णों को दर्ज किया है",
			nameof(LessThanOrEqualValidator) => "'{PropertyName}' '{ComparisonValue}' से कम या के उसके बराबर होनी चाहिए।",
			nameof(LessThanValidator) => "'{PropertyName}' '{ComparisonValue}' से कम होनी चाहिए।",
			nameof(NotEmptyValidator) => "'{PropertyName}' खाली नहीं होना चाहिए।",
			nameof(NotEqualValidator) => "'{PropertyName}' '{ComparisonValue}' से बराबर नहीं होना चाहिए।",
			nameof(NotNullValidator) => "'{PropertyName}' खाली नहीं होना चाहिए।",
			nameof(PredicateValidator) => "निर्दिष्ट स्थिति को '{PropertyName}' के लिए पूरा नहीं किया गया।",
			nameof(AsyncPredicateValidator) => "निर्दिष्ट स्थिति को '{PropertyName}' के लिए पूरा नहीं किया गया।",
			nameof(RegularExpressionValidator) => "'{PropertyName}' सही प्रारूप में नहीं है।",
			nameof(EqualValidator) => "'{PropertyName}' '{ComparisonValue}' से बराबर होना चाहिए।",
			nameof(ExactLengthValidator) => "'{PropertyName}' {MaxLength} अक्षरों के उसके बराबर होनी चाहिए। आपने {TotalLength} अक्षर दर्ज किए हैं।",
			nameof(InclusiveBetweenValidator) => "'{PropertyName}' {From} और {To} के बीच में होनी चाहिए।. आपने {Value} दर्ज किया है।",
			nameof(ExclusiveBetweenValidator) => "'{PropertyName}' {From} और {To} (अनन्य) के बीच में होनी चाहिए।. आपने {Value} दर्ज किया है।",
			nameof(CreditCardValidator) => "'{PropertyName}' मान्य क्रेडिट कार्ड नंबर नहीं है।",
			nameof(ScalePrecisionValidator) => "'{PropertyName}' कुल में {ExpectedPrecision} अंकों से अधिक नहीं हो सकता है, {ExpectedScale} दशमलव के के साथ।. {Digits} अंक और {ActualScale} दशमलव पाए गए है।",
			nameof(EmptyValidator) => "'{PropertyName}' खाली होना चाहिए।",
			nameof(NullValidator) => "'{PropertyName}' खाली होना चाहिए।",
			nameof(EnumValidator) => "'{PropertyName}' में कई मान हैं जिनमें '{PropertyValue}' शामिल नहीं है।",
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
