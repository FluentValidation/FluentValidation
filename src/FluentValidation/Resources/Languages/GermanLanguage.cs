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

	internal class GermanLanguage {
		public const string Culture = "de";

		public static string GetTranslation(string key) => key switch {
			nameof(EmailValidator) => "'{PropertyName}' ist keine gültige E-Mail-Adresse.",
			nameof(GreaterThanOrEqualValidator) => "Der Wert von '{PropertyName}' muss grösser oder gleich '{ComparisonValue}' sein.",
			nameof(GreaterThanValidator) => "Der Wert von '{PropertyName}' muss grösser sein als '{ComparisonValue}'.",
			nameof(LengthValidator) => "Die Länge von '{PropertyName}' muss zwischen {MinLength} und {MaxLength} Zeichen liegen. Es wurden {TotalLength} Zeichen eingetragen.",
			nameof(MinimumLengthValidator) => "Die Länge von '{PropertyName}' muss größer oder gleich {MinLength} sein. Sie haben {TotalLength} Zeichen eingegeben.",
			nameof(MaximumLengthValidator) => "Die Länge von '{PropertyName}' muss kleiner oder gleich {MaxLength} sein. Sie haben {TotalLength} Zeichen eingegeben.",
			nameof(LessThanOrEqualValidator) => "Der Wert von '{PropertyName}' muss kleiner oder gleich '{ComparisonValue}' sein.",
			nameof(LessThanValidator) => "Der Wert von '{PropertyName}' muss kleiner sein als '{ComparisonValue}'.",
			nameof(NotEmptyValidator) => "'{PropertyName}' darf nicht leer sein.",
			nameof(NotEqualValidator) => "'{PropertyName}' darf nicht '{ComparisonValue}' sein.",
			nameof(NotNullValidator) => "'{PropertyName}' darf kein Nullwert sein.",
			nameof(PredicateValidator) => "Der Wert von '{PropertyName}' entspricht nicht der festgelegten Bedingung.",
			nameof(AsyncPredicateValidator) => "Der Wert von '{PropertyName}' entspricht nicht der festgelegten Bedingung.",
			nameof(RegularExpressionValidator) => "'{PropertyName}' weist ein ungültiges Format auf.",
			nameof(EqualValidator) => "'{PropertyName}' muss gleich '{ComparisonValue}' sein.",
			nameof(ExactLengthValidator) => "'{PropertyName}' muss genau {MaxLength} lang sein. Es wurden {TotalLength} eingegeben.",
			nameof(ExclusiveBetweenValidator) => "'{PropertyName}' muss zwischen {From} und {To} sein (exklusiv). Es wurde {Value} eingegeben.",
			nameof(InclusiveBetweenValidator) => "'{PropertyName}' muss zwischen {From} and {To} sein. Es wurde {Value} eingegeben.",
			nameof(CreditCardValidator) => "'{PropertyName}' ist keine gültige Kreditkartennummer.",
			nameof(ScalePrecisionValidator) => "'{PropertyName}' darf insgesamt nicht mehr als {ExpectedPrecision} Ziffern enthalten, mit Berücksichtigung von {ExpectedScale} Dezimalstellen. Es wurden {Digits} Ziffern und {ActualScale} Dezimalstellen gefunden.",
			nameof(EmptyValidator) => "'{PropertyName}' sollte leer sein.",
			nameof(NullValidator) => "'{PropertyName}' sollte leer sein.",
			nameof(EnumValidator) => "'{PropertyName}' hat einen Wertebereich, der '{PropertyValue}' nicht enthält.",
			// Additional fallback messages used by clientside validation integration.
			"Length_Simple" => "Die Länge von '{PropertyName}' muss zwischen {MinLength} und {MaxLength} Zeichen liegen.",
			"MinimumLength_Simple" => "Die Länge von '{PropertyName}' muss größer oder gleich {MinLength} sein.",
			"MaximumLength_Simple" => "Die Länge von '{PropertyName}' muss kleiner oder gleich {MaxLength} sein.",
			"ExactLength_Simple" => "'{PropertyName}' muss genau {MaxLength} lang sein.",
			"InclusiveBetween_Simple" => "'{PropertyName}' muss zwischen {From} and {To} sein.",
			_ => null,
		};
	}
}
