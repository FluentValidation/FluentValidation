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

internal class LatvianLanguage {
	public const string Culture = "lv";

	public static string GetTranslation(string key) => key switch {
		"EmailValidator" => "'{PropertyName}' nesatur pareizu e-pasta adresi.",
		"GreaterThanOrEqualValidator" => "'{PropertyName}' vērtībai ir jābūt lielākai vai vienādai ar '{ComparisonValue}'.",
		"GreaterThanValidator" => "'{PropertyName}' vērtībai ir jābūt lielākai par '{ComparisonValue}'.",
		"LengthValidator" => "'{PropertyName}' vērtībai ir jābūt no {MinLength} līdz {MaxLength} simbolu garai. Ievadīti {TotalLength} simboli.",
		"MinimumLengthValidator" => "'{PropertyName}' garumam ir jābūt vismaz {MinLength} simbolu garam. Ievadīti {TotalLength} simboli.",
		"MaximumLengthValidator" => "'{PropertyName}' garumam ir jābūt maksimāli {MaxLength} simbolu garam. Ievadīti {TotalLength} simboli.",
		"LessThanOrEqualValidator" => "'{PropertyName}' vērtībai ir jābūt mazākai vai vienādai ar '{ComparisonValue}'.",
		"LessThanValidator" => "'{PropertyName}' vērtībai ir jābūt mazākai par '{ComparisonValue}'.",
		"NotEmptyValidator" => "'{PropertyName}' vērtība nevar būt tukša.",
		"NotEqualValidator" => "'{PropertyName}' vērtība nedrīkst būt vienāda ar '{ComparisonValue}'.",
		"NotNullValidator" => "'{PropertyName}' vērtība nevar būt tukša.",
		"PredicateValidator" => "Definētā pārbaude nepieļauj ievadīto '{PropertyName}' vērtību.",
		"AsyncPredicateValidator" => "Definētā pārbaude nepieļauj ievadīto '{PropertyName}' vērtību.",
		"RegularExpressionValidator" => "'{PropertyName}' nav ievadīts vajadzīgajā formātā.",
		"EqualValidator" => "'{PropertyName}' vērtībai ir jābūt vienādai ar '{ComparisonValue}'.",
		"ExactLengthValidator" => "'{PropertyName}' vērtībai ir jābūt {MaxLength} simbolu garai. Ievadīti {TotalLength} simboli.",
		"InclusiveBetweenValidator" => "'{PropertyName}' vērtībai ir jābūt no {From} līdz {To}. Ievadītā vērtība: {PropertyValue}.",
		"ExclusiveBetweenValidator" => "'{PropertyName}' vērtībai ir jābūt no {From} līdz {To} (neiekļaujot šīs vērtības). Ievadītā vērtība: {PropertyValue}.",
		"CreditCardValidator" => "'{PropertyName}' nesatur pareizu kredītkartes numuru.",
		"ScalePrecisionValidator" => "'{PropertyName}' vērtība nedrīkst saturēt vairāk par {ExpectedPrecision} ciparu kopā, tajā skaitā {ExpectedScale} ciparu aiz komata. Ievadītā vērtība satur {Digits} ciparu kopā un {ActualScale} ciparu aiz komata.",
		"EmptyValidator" => "'{PropertyName}' jābūt tukšai.",
		"NullValidator" => "'{PropertyName}' jābūt tukšai.",
		"EnumValidator" => "'{PropertyName}' satur noteiktas vērtības, kuras neietver ievadīto '{PropertyValue}'.",
		// Additional fallback messages used by clientside validation integration.
		"Length_Simple" => "'{PropertyName}' vērtībai ir jābūt no {MinLength} līdz {MaxLength} simbolu garai.",
		"MinimumLength_Simple" => "'{PropertyName}' vērtībai ir jābūt vismaz {MinLength} simbolu garai.",
		"MaximumLength_Simple" => "'{PropertyName}' vērtībai ir jābūt maksimums {MaxLength} simbolu garai.",
		"ExactLength_Simple" => "'{PropertyName}' vērtībai ir jābūt {MaxLength} simbolu garai.",
		"InclusiveBetween_Simple" => "'{PropertyName}' vērtībai ir jābūt no {From} līdz {To}.",
		_ => null,
	};
}
