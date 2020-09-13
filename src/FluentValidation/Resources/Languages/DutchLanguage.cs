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

	internal class DutchLanguage {
		public const string Culture = "nl";

		public static string GetTranslation(string key) => key switch {
			nameof(EmailValidator) => "'{PropertyName}' is geen geldig email adres.",
			nameof(EqualValidator) => "'{PropertyName}' moet gelijk zijn aan '{ComparisonValue}'.",
			nameof(GreaterThanOrEqualValidator) => "'{PropertyName}' moet meer dan of gelijk zijn aan '{ComparisonValue}'.",
			nameof(GreaterThanValidator) => "'{PropertyName}' moet groter zijn dan '{ComparisonValue}'.",
			nameof(LengthValidator) => "De lengte van '{PropertyName}' moet tussen {MinLength} en {MaxLength} karakters zijn. Er zijn {TotalLength} karakters ingevoerd.",
			nameof(MinimumLengthValidator) => "De lengte van '{PropertyName}' moet groter zijn dan of gelijk aan {MinLength} tekens. U hebt {TotalLength} -tekens ingevoerd.",
			nameof(MaximumLengthValidator) => "De lengte van '{PropertyName}' moet kleiner zijn dan of gelijk aan {MaxLength} tekens. U hebt {TotalLength} -tekens ingevoerd.",
			nameof(LessThanOrEqualValidator) => "'{PropertyName}' moet minder dan of gelijk zijn aan '{ComparisonValue}'.",
			nameof(LessThanValidator) => "'{PropertyName}' moet minder zijn dan '{ComparisonValue}'.",
			nameof(NotEmptyValidator) => "'{PropertyName}' mag niet leeg zijn.",
			nameof(NotEqualValidator) => "'{PropertyName}' moet anders zijn dan '{ComparisonValue}'.",
			nameof(NotNullValidator) => "'{PropertyName}' mag niet leeg zijn.",
			nameof(PredicateValidator) => "'{PropertyName}' voldoet niet aan de vereisten.",
			nameof(AsyncPredicateValidator) => "'{PropertyName}' voldoet niet aan de vereisten.",
			nameof(RegularExpressionValidator) => "'{PropertyName}' voldoet niet aan het verwachte formaat.",
			nameof(ExactLengthValidator) => "De lengte van '{PropertyName}' moet {MaxLength} karakters zijn. Er zijn {TotalLength} karakters ingevoerd.",
			nameof(EnumValidator) => "'{PropertyValue}' komt niet voor in het bereik van '{PropertyName}'.",
			nameof(CreditCardValidator) => "'{PropertyName}' is geen geldig credit card nummer.",
			nameof(EmptyValidator) => "'{PropertyName}' hoort leeg te zijn.",
			nameof(ExclusiveBetweenValidator) => "'{PropertyName}' moet na {From} komen en voor {To} liggen. Je hebt ingevuld {Value}.",
			nameof(InclusiveBetweenValidator) => "'{PropertyName}' moet tussen {From} en {To} liggen. Je hebt ingevuld {Value}.",
			nameof(ScalePrecisionValidator) => "'{PropertyName}' mag in totaal niet meer dan {ExpectedPrecision} decimalen nauwkeurig zijn, met een grootte van {ExpectedScale} gehele getallen. Er zijn {Digits} decimalen en een grootte van {ActualScale} gehele getallen gevonden.",
			nameof(NullValidator) => "'{PropertyName}' moet leeg zijn.",
			// Additional fallback messages used by clientside validation integration.
			"Length_Simple" => "De lengte van '{PropertyName}' moet tussen {MinLength} en {MaxLength} karakters zijn.",
			"MinimumLength_Simple" => "De lengte van '{PropertyName}' moet groter zijn dan of gelijk aan {MinLength} tekens.",
			"MaximumLength_Simple" => "De lengte van '{PropertyName}' moet kleiner zijn dan of gelijk aan {MaxLength} tekens.",
			"ExactLength_Simple" => "De lengte van '{PropertyName}' moet {MaxLength} karakters zijn.",
			"InclusiveBetween_Simple" => "'{PropertyName}' moet tussen {From} en {To} liggen.",
			_ => null,
		};
	}
}
