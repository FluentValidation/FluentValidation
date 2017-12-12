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

	internal class DutchLanguage : Language {
		public override string Name => "nl";

		public DutchLanguage() {
			Translate<EmailValidator>("'{PropertyName}' is geen geldig email adres.");
			Translate<EqualValidator>("'{PropertyName}' moet gelijk zijn aan '{ComparisonValue}'.");
			Translate<GreaterThanOrEqualValidator>("'{PropertyName}' moet meer dan of gelijk zijn aan '{ComparisonValue}'.");
			Translate<GreaterThanValidator>("'{PropertyName}' moet groter zijn dan '{ComparisonValue}'.");
			Translate<LengthValidator>("De lengte van '{PropertyName}' moet tussen {MinLength} en {MaxLength} karakters zijn. Er zijn {TotalLength} karakters ingevoerd.");
			Translate<MinimumLengthValidator>("De lengte van '{PropertyName}' moet groter zijn dan of gelijk aan {MinLength} tekens. U hebt {TotalLength} -tekens ingevoerd.");
			Translate<MaximumLengthValidator>("De lengte van '{PropertyName}' moet kleiner zijn dan of gelijk aan {MaxLength} tekens. U hebt {TotalLength} -tekens ingevoerd.");
			Translate<LessThanOrEqualValidator>("'{PropertyName}' moet minder dan of gelijk zijn aan '{ComparisonValue}'.");
			Translate<LessThanValidator>("'{PropertyName}' moet minder zijn dan '{ComparisonValue}'.");
			Translate<NotEmptyValidator>("'{PropertyName}' mag niet leeg zijn.");
			Translate<NotEqualValidator>("'{PropertyName}' moet anders zijn dan '{ComparisonValue}'.");
			Translate<NotNullValidator>("'{PropertyName}' mag niet leeg zijn.");
			Translate<PredicateValidator>("'{PropertyName}' voldoet niet aan de vereisten.");
			Translate<AsyncPredicateValidator>("'{PropertyName}' voldoet niet aan de vereisten.");
			Translate<RegularExpressionValidator>("'{PropertyName}' voldoet niet aan het verwachte formaat.");
			Translate<ExactLengthValidator>("De lengte van '{PropertyName}' moet {MaxLength} karakters zijn. Er zijn {TotalLength} karakters ingevoerd.");
			Translate<EnumValidator>("'{PropertyValue}' komt niet voor in het bereik van '{PropertyName}'.");
			Translate<CreditCardValidator>("'{PropertyName}' is geen geldig credit card nummer.");
			Translate<EmptyValidator>("'{PropertyName}' hoort leeg te zijn.");
			Translate<ExclusiveBetweenValidator>("'{PropertyName}' moet na {From} komen en voor {To} liggen. Je hebt ingevuld {Value}.");
			Translate<InclusiveBetweenValidator>("'{PropertyName}' moet tussen {From} en {To} liggen. Je hebt ingevuld {Value}.");
			Translate<ScalePrecisionValidator>("'{PropertyName}' mag in totaal niet meer dan {expectedPrecision} decimalen nauwkeurig zijn, met een grote van {expectedScale} gehele getallen. Er zijn {digits} decimalen en een grote van {actualScale} gehele getallen gevonden.");
			Translate<NullValidator>("'{PropertyName}' moet leeg zijn.");
		}
	}
}