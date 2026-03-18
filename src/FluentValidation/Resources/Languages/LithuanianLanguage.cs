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

internal class LithuanianLanguage {
	public const string Culture = "lt";

	public static string GetTranslation(string key) => key switch {
		"EmailValidator" => "'{PropertyName}' nėra galiojantis el. pašto adresas.",
		"GreaterThanOrEqualValidator" => "'{PropertyName}' turi būti didesnis arba lygus '{ComparisonValue}'.",
		"GreaterThanValidator" => "'{PropertyName}' turi būti didesnis už '{ComparisonValue}'.",
		"LengthValidator" => "'{PropertyName}' turi būti nuo {MinLength} iki {MaxLength} simbolių. Jūs įvedėte {TotalLength} simbolius.",
		"MinimumLengthValidator" => "'{PropertyName}' ilgis turi būti bent {MinLength} simbolių. Jūs įvedėte {TotalLength} simbolius.",
		"MaximumLengthValidator" => "'{PropertyName}' ilgis turi būti ne daugiau kaip {MaxLength} simbolių. Jūs įvedėte {TotalLength} simbolius.",
		"LessThanOrEqualValidator" => "'{PropertyName}' turi būti mažesnis arba lygus '{ComparisonValue}'.",
		"LessThanValidator" => "'{PropertyName}' turi būti mažesnis už '{ComparisonValue}'.",
		"NotEmptyValidator" => "'{PropertyName}' negali būti tuščias.",
		"NotEqualValidator" => "'{PropertyName}' negali būti lygus '{ComparisonValue}'.",
		"NotNullValidator" => "'{PropertyName}' negali būti tuščias.",
		"PredicateValidator" => "Nurodyta sąlyga neįvykdyta laukui '{PropertyName}'.",
		"AsyncPredicateValidator" => "Nurodyta sąlyga neįvykdyta laukui '{PropertyName}'.",
		"RegularExpressionValidator" => "'{PropertyName}' nėra tinkamo formato.",
		"EqualValidator" => "'{PropertyName}' turi būti lygus '{ComparisonValue}'.",
		"ExactLengthValidator" => "'{PropertyName}' ilgis turi būti {MaxLength} simbolių. Jūs įvedėte {TotalLength} simbolius.",
		"InclusiveBetweenValidator" => "'{PropertyName}' turi būti tarp {From} ir {To}. Jūs įvedėte {PropertyValue}.",
		"ExclusiveBetweenValidator" => "'{PropertyName}' turi būti tarp {From} ir {To} (išskirtinai). Jūs įvedėte {PropertyValue}.",
		"CreditCardValidator" => "'{PropertyName}' nėra galiojantis kredito kortelės numeris.",
		"ScalePrecisionValidator" => "'{PropertyName}' negali turėti daugiau kaip {ExpectedPrecision} skaitmenų, iš kurių {ExpectedScale} gali būti po kablelio. Rasta {Digits} skaitmenys ir {ActualScale} dešimtainiai.",
		"EmptyValidator" => "'{PropertyName}' turi būti tuščias.",
		"NullValidator" => "'{PropertyName}' turi būti tuščias.",
		"EnumValidator" => "'{PropertyName}' reikšmių sąraše nėra '{PropertyValue}'.",
		// Additional fallback messages used by clientside validation integration.
		"Length_Simple" => "'{PropertyName}' turi būti nuo {MinLength} iki {MaxLength} simbolių.",
		"MinimumLength_Simple" => "'{PropertyName}' ilgis turi būti bent {MinLength} simbolių.",
		"MaximumLength_Simple" => "'{PropertyName}' ilgis turi būti ne daugiau kaip {MaxLength} simbolių.",
		"ExactLength_Simple" => "'{PropertyName}' ilgis turi būti {MaxLength} simbolių.",
		"InclusiveBetween_Simple" => "'{PropertyName}' turi būti tarp {From} ir {To}.",
		_ => null,
	};
}
