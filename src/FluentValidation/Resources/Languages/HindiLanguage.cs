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

	internal class HindiLanguage : Language	{
		public override string Name => "hi";

		public HindiLanguage() {
			Translate<EmailValidator>("'{PropertyName}' मान्य ईमेल एड्रेस नहीं है।");
			Translate<GreaterThanOrEqualValidator>("'{PropertyName}' '{ComparisonValue}' से अधिक या के उसके बराबर होनी चाहिए।");
			Translate<GreaterThanValidator>("'{PropertyName}' '{ComparisonValue}' से अधिक होनी चाहिए।");
			Translate<LengthValidator>("'{PropertyName}' {MinLength} और {MaxLength} अक्षरों के बीच होना चाहिए। आपने {TotalLength} अक्षर दर्ज किए हैं।");
			Translate<MinimumLengthValidator>("'{PropertyName}' {MinLength} वर्णों से अधिक या उसके बराबर होना चाहिए। आपने {TotalLength} वर्णों को दर्ज किया है");
			Translate<MaximumLengthValidator>("'{PropertyName}' {MaxLength} वर्णों से कम या उसके बराबर होना चाहिए। आपने {TotalLength} वर्णों को दर्ज किया है");
			Translate<LessThanOrEqualValidator>("'{PropertyName}' '{ComparisonValue}' से कम या के उसके बराबर होनी चाहिए।");
			Translate<LessThanValidator>("'{PropertyName}' '{ComparisonValue}' से कम होनी चाहिए।");
			Translate<NotEmptyValidator>("'{PropertyName}' खाली नहीं होना चाहिए।");
			Translate<NotEqualValidator>("'{PropertyName}' '{ComparisonValue}' से बराबर नहीं होना चाहिए।");
			Translate<NotNullValidator>("'{PropertyName}' खाली नहीं होना चाहिए।");
			Translate<PredicateValidator>("निर्दिष्ट स्थिति को '{PropertyName}' के लिए पूरा नहीं किया गया।");
			Translate<AsyncPredicateValidator>("निर्दिष्ट स्थिति को '{PropertyName}' के लिए पूरा नहीं किया गया।");
			Translate<RegularExpressionValidator>("'{PropertyName}' सही प्रारूप में नहीं है।");
			Translate<EqualValidator>("'{PropertyName}' '{ComparisonValue}' से बराबर होना चाहिए।");
			Translate<ExactLengthValidator>("'{PropertyName}' {MaxLength} अक्षरों के उसके बराबर होनी चाहिए। आपने {TotalLength} अक्षर दर्ज किए हैं।");
			Translate<InclusiveBetweenValidator>("'{PropertyName}' {From} और {To} के बीच में होनी चाहिए।. आपने {Value} दर्ज किया है।");
			Translate<ExclusiveBetweenValidator>("'{PropertyName}' {From} और {To} (अनन्य) के बीच में होनी चाहिए।. आपने {Value} दर्ज किया है।");
			Translate<CreditCardValidator>("'{PropertyName}' मान्य क्रेडिट कार्ड नंबर नहीं है।");
			Translate<ScalePrecisionValidator>("'{PropertyName}' कुल में {expectedPrecision} अंकों से अधिक नहीं हो सकता है, {expectedScale} दशमलव के के साथ।. {digits} अंक और {actualScale} दशमलव पाए गए है।");
			Translate<EmptyValidator>("'{PropertyName}' खाली होना चाहिए।");
			Translate<NullValidator>("'{PropertyName}' खाली होना चाहिए।");
			Translate<EnumValidator>("'{PropertyName}' में कई मान हैं जिनमें '{PropertyValue}' शामिल नहीं है।");
		}
	}
}