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

internal class CatalanLanguage {
	public const string Culture = "ca";

	public static string GetTranslation(string key) => key switch {
		"EmailValidator" => "'{PropertyName}' no és una adreça de correu electrònic vàlida.",
		"GreaterThanOrEqualValidator" => "'{PropertyName}' ha de ser més gran o igual que '{ComparisonValue}'.",
		"GreaterThanValidator" => "'{PropertyName}' ha de ser més gran que '{ComparisonValue}'.",
		"LengthValidator" => "'{PropertyName}' ha de tenir entre {MinLength} i {MaxLength} caràcters. Actualment té {TotalLength} caràcters.",
		"MinimumLengthValidator" => "'{PropertyName}' ha de ser més gran o igual que {MinLength} caràcters. Ha inserit {TotalLength} caràcters.",
		"MaximumLengthValidator" => "'{PropertyName}' ha de ser menor o igual que {MaxLength} caràcters. Ha inserit {TotalLength} caràcters.",
		"LessThanOrEqualValidator" => "'{PropertyName}' ha de ser menor o igual que '{ComparisonValue}'.",
		"LessThanValidator" => "'{PropertyName}' ha de ser menor que '{ComparisonValue}'.",
		"NotEmptyValidator" => "'{PropertyName}' no hauria d'estar buit.",
		"NotEqualValidator" => "'{PropertyName}' no hauria de ser igual a '{ComparisonValue}'.",
		"NotNullValidator" => "'{PropertyName}' no ha d'estar buit.",
		"PredicateValidator" => "'{PropertyName}' no compleix amb la condició especificada.",
		"AsyncPredicateValidator" => "'{PropertyName}' no compleix amb la condició especificada.",
		"RegularExpressionValidator" => "'{PropertyName}' no té el format correcte.",
		"EqualValidator" => "'{PropertyName}' hauria de ser igual a '{ComparisonValue}'.",
		"ExactLengthValidator" => "'{PropertyName}' ha de tenir una llargada de {MaxLength} caràcters. Actualment té {TotalLength} caràcters.",
		"ExclusiveBetweenValidator" => "'{PropertyName}' ha d'estar entre {From} i {To} (exclusiu). Actualment té un valor de {PropertyValue}.",
		"InclusiveBetweenValidator" => "'{PropertyName}' ha d'estar entre {From} i {To}. Actualment té un valor de {PropertyValue}.",
		"CreditCardValidator" => "'{PropertyName}' no és un número de targeta de crèdit vàlid.",
		"ScalePrecisionValidator" => "'{PropertyName}' no ha de tenir més de {ExpectedPrecision} dígits en total, amb marge per {ExpectedScale} decimals. S'han trobat {Digits} i {ActualScale} decimals.",
		"EmptyValidator" => "'{PropertyName}' ha d'estar buit.",
		"NullValidator" => "'{PropertyName}' ha d'estar buit.",
		"EnumValidator" => "'{PropertyName}' té un rang de valors que no inclou '{PropertyValue}'.",
		// Additional fallback messages used by clientside validation integration.
		"Length_Simple" => "'{PropertyName}' ha de tenir entre {MinLength} i {MaxLength} caràcters.",
		"MinimumLength_Simple" => "'{PropertyName}' ha de ser més gran o igual que {MinLength} caràcters.",
		"MaximumLength_Simple" => "'{PropertyName}' ha de ser menor o igual que {MaxLength} caràcters.",
		"ExactLength_Simple" => "'{PropertyName}' ha de tenir una longitud de {MaxLength} caràcters.",
		"InclusiveBetween_Simple" => "'{PropertyName}' ha d'estar entre {From} i {To}.",
		_ => null,
	};
}
