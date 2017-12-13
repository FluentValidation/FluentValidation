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

	internal class SwedishLanguage : Language {
		public override string Name => "sv";

		public SwedishLanguage() {
			Translate<EmailValidator>("\"{PropertyName}\" är inte en giltig e-postadress.");
			Translate<GreaterThanOrEqualValidator>("\"{PropertyName}\" måste vara större än eller lika med {ComparisonValue}.");
			Translate<GreaterThanValidator>("\"{PropertyName}\" måste vara större än {ComparisonValue}.");
			Translate<LengthValidator>("\"{PropertyName}\" måste vara mellan {MinLength} och {MaxLength} tecken långt. Du angav {TotalLength} tecken.");
			Translate<MinimumLengthValidator>("\"{PropertyName}\" måste vara större än eller lika med {MinLength} tecken. Du har skrivit in {TotalLength} tecken.");
			Translate<MaximumLengthValidator>("\"{PropertyName}\" måste vara mindre än eller lika med {MaxLength} tecken. Du har skrivit in {TotalLength} tecken.");
			Translate<LessThanOrEqualValidator>("\"{PropertyName}\" måste vara mindre än eller lika med {ComparisonValue}.");
			Translate<LessThanValidator>("\"{PropertyName}\" måste vara mindre än {ComparisonValue}.");
			Translate<NotEmptyValidator>("\"{PropertyName}\" måste anges.");
			Translate<NotEqualValidator>("\"{PropertyName}\" får inte vara lika med \"{ComparisonValue}\".");
			Translate<NotNullValidator>("\"{PropertyName}\" måste anges.");
			Translate<PredicateValidator>("Det angivna villkoret uppfylldes inte för \"{PropertyName}\".");
			Translate<AsyncPredicateValidator>("Det angivna villkoret uppfylldes inte för \"{PropertyName}\".");
			Translate<RegularExpressionValidator>("\"{PropertyName}\" har inte ett korrekt format.");
			Translate<EqualValidator>("\"{PropertyName}\" måste vara lika med \"{ComparisonValue}\".");
			Translate<ExactLengthValidator>("\"{PropertyName}\" måste vara {MaxLength} tecken långt. Du angav {TotalLength} tecken.");
			Translate<InclusiveBetweenValidator>("\"{PropertyName}\" måste vara mellan {From} och {To}. Du angav {Value}.");
			Translate<ExclusiveBetweenValidator>("\"{PropertyName}\" måste vara mellan {From} och {To} (gränsvärdena exkluderade). Du angav {Value}.");
			Translate<CreditCardValidator>("\"{PropertyName}\" no es un número de tarjeta de crédito válido.");
			Translate<ScalePrecisionValidator>("\"{PropertyName}\" får inte vara mer än {expectedPrecision} siffror totalt, med förbehåll för {expectedScale} decimaler. {digits} siffror och {actualScale} decimaler hittades.");
			Translate<EmptyValidator>("\"{PropertyName}\" ska vara tomt.");
			Translate<NullValidator>("\"{PropertyName}\" ska vara tomt.");
			Translate<EnumValidator>("\"{PropertyName}\" har ett antal värden som inte inkluderar \"{PropertyValue}\".");
		}
	}
}