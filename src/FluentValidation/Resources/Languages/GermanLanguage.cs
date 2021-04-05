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
			"EmailValidator" => "'{PropertyName}' ist keine gültige E-Mail-Adresse.",
			"GreaterThanOrEqualValidator" => "Der Wert von '{PropertyName}' muss grösser oder gleich '{ComparisonValue}' sein.",
			"GreaterThanValidator" => "Der Wert von '{PropertyName}' muss grösser sein als '{ComparisonValue}'.",
			"LengthValidator" => "Die Länge von '{PropertyName}' muss zwischen {MinLength} und {MaxLength} Zeichen liegen. Es wurden {TotalLength} Zeichen eingetragen.",
			"MinimumLengthValidator" => "Die Länge von '{PropertyName}' muss größer oder gleich {MinLength} sein. Sie haben {TotalLength} Zeichen eingegeben.",
			"MaximumLengthValidator" => "Die Länge von '{PropertyName}' muss kleiner oder gleich {MaxLength} sein. Sie haben {TotalLength} Zeichen eingegeben.",
			"LessThanOrEqualValidator" => "Der Wert von '{PropertyName}' muss kleiner oder gleich '{ComparisonValue}' sein.",
			"LessThanValidator" => "Der Wert von '{PropertyName}' muss kleiner sein als '{ComparisonValue}'.",
			"NotEmptyValidator" => "'{PropertyName}' darf nicht leer sein.",
			"NotEqualValidator" => "'{PropertyName}' darf nicht '{ComparisonValue}' sein.",
			"NotNullValidator" => "'{PropertyName}' darf kein Nullwert sein.",
			"PredicateValidator" => "Der Wert von '{PropertyName}' entspricht nicht der festgelegten Bedingung.",
			"AsyncPredicateValidator" => "Der Wert von '{PropertyName}' entspricht nicht der festgelegten Bedingung.",
			"RegularExpressionValidator" => "'{PropertyName}' weist ein ungültiges Format auf.",
			"EqualValidator" => "'{PropertyName}' muss gleich '{ComparisonValue}' sein.",
			"ExactLengthValidator" => "'{PropertyName}' muss genau {MaxLength} lang sein. Es wurden {TotalLength} eingegeben.",
			"ExclusiveBetweenValidator" => "'{PropertyName}' muss zwischen {From} und {To} sein (exklusiv). Es wurde {PropertyValue} eingegeben.",
			"InclusiveBetweenValidator" => "'{PropertyName}' muss zwischen {From} and {To} sein. Es wurde {PropertyValue} eingegeben.",
			"CreditCardValidator" => "'{PropertyName}' ist keine gültige Kreditkartennummer.",
			"ScalePrecisionValidator" => "'{PropertyName}' darf insgesamt nicht mehr als {ExpectedPrecision} Ziffern enthalten, mit Berücksichtigung von {ExpectedScale} Dezimalstellen. Es wurden {Digits} Ziffern und {ActualScale} Dezimalstellen gefunden.",
			"EmptyValidator" => "'{PropertyName}' sollte leer sein.",
			"NullValidator" => "'{PropertyName}' sollte leer sein.",
			"EnumValidator" => "'{PropertyName}' hat einen Wertebereich, der '{PropertyValue}' nicht enthält.",
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
