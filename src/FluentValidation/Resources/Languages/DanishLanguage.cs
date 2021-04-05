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

	internal class DanishLanguage {
		public const string Culture = "da";

		public static string GetTranslation(string key) => key switch {
			"EmailValidator" => "'{PropertyName}' er ikke en gyldig e-mail-adresse.",
			"GreaterThanOrEqualValidator" => "'{PropertyName}' skal være større end eller lig med '{ComparisonValue}'.",
			"GreaterThanValidator" => "'{PropertyName}' skal være større end '{ComparisonValue}'.",
			"LengthValidator" => "'{PropertyName}' skal være mellem {MinLength} og {MaxLength} tegn. Du har indtastet {TotalLength} tegn.",
			"MinimumLengthValidator" => "'{PropertyName}' skal være større end eller lig med {MinLength} tegn. Du indtastede {TotalLength} tegn.",
			"MaximumLengthValidator" => "'{PropertyName}' skal være mindre end eller lig med {MaxLength} tegn. Du indtastede {TotalLength} tegn.",
			"LessThanOrEqualValidator" => "'{PropertyName}' skal være mindre end eller lig med '{ComparisonValue}'.",
			"LessThanValidator" => "'{PropertyName}' skal være mindre end '{ComparisonValue}'.",
			"NotEmptyValidator" => "'{PropertyName}' bør ikke være tom.",
			"NotEqualValidator" => "'{PropertyName}' bør ikke være lig med '{ComparisonValue}'.",
			"NotNullValidator" => "'{PropertyName}' må ikke være tomme.",
			"PredicateValidator" => "Den angivne betingelse var ikke opfyldt for '{PropertyName}'.",
			"AsyncPredicateValidator" => "Den angivne betingelse var ikke opfyldt for '{PropertyName}'.",
			"RegularExpressionValidator" => "'{PropertyName}' er ikke i det rigtige format.",
			"EqualValidator" => "'{PropertyName}' skal være lig med '{ComparisonValue}'.",
			"ExactLengthValidator" => "'{PropertyName}' skal være {MaxLength} tegn langt. Du har indtastet {TotalLength} tegn.",
			"InclusiveBetweenValidator" => "'{PropertyName}' skal være mellem {From} og {To}. Du har indtastet {PropertyValue}.",
			"ExclusiveBetweenValidator" => "'{PropertyName}' skal være mellem {From} og {To} (eksklusiv). Du har indtastet {PropertyValue}.",
			"CreditCardValidator" => "'{PropertyName}' er ikke et gyldigt kreditkortnummer.",
			"ScalePrecisionValidator" => "'{PropertyName}' må ikke være mere end {ExpectedPrecision} cifre i alt, med hensyn til {ExpectedScale} decimaler. {Digits} cifre og {ActualScale} decimaler blev fundet.",
			"EmptyValidator" => "'{PropertyName}' skal være tomt.",
			"NullValidator" => "'{PropertyName}' skal være tomt.",
			"EnumValidator" => "'{PropertyName}' har en række værdier, der ikke indeholder '{PropertyValue}'.",
			// Additional fallback messages used by clientside validation integration.
			"Length_Simple" => "'{PropertyName}' skal være mellem {MinLength} og {MaxLength} tegn.",
			"MinimumLength_Simple" => "'{PropertyName}' skal være større end eller lig med {MinLength} tegn.",
			"MaximumLength_Simple" => "'{PropertyName}' skal være mindre end eller lig med {MaxLength} tegn.",
			"ExactLength_Simple" => "'{PropertyName}' skal være {MaxLength} tegn langt.",
			"InclusiveBetween_Simple" => "'{PropertyName}' skal være mellem {From} og {To}.",
			_ => null,
		};
	}
}
