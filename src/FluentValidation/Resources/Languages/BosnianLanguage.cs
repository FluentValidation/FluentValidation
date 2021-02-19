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

	internal class BosnianLanguage {
		public const string Culture = "bs";

		public static string GetTranslation(string key) => key switch {
			"EmailValidator" => "'{PropertyName}' nije validna email adresa.",
			"GreaterThanOrEqualValidator" => "'{PropertyName}' mora biti veće ili jednako '{ComparisonValue}'.",
			"GreaterThanValidator" => "'{PropertyName}' mora biti veće od '{ComparisonValue}'.",
			"LengthValidator" => "'{PropertyName}' mora imati između {MinLength} i {MaxLength} karatkera. Uneseno je {TotalLength} karaktera.",
			"MinimumLengthValidator" => "'{PropertyName}' mora imati najmanje {MinLength} karaktera. Uneseno je {TotalLength} karaktera.",
			"MaximumLengthValidator" => "'{PropertyName}' ne smije imati više od {MaxLength} karaktera. Uneseno je {TotalLength} karaktera.",
			"LessThanOrEqualValidator" => "'{PropertyName}' mora biti manje ili jednako '{ComparisonValue}'.",
			"LessThanValidator" => "'{PropertyName}' mora biti manje od '{ComparisonValue}'.",
			"NotEmptyValidator" => "'{PropertyName}' ne smije biti prazan.",
			"NotEqualValidator" => "'{PropertyName}' ne smije biti jednak '{ComparisonValue}'.",
			"NotNullValidator" => "'{PropertyName}' ne smije biti prazan.",
			"PredicateValidator" => "Zadan uslov nije ispunjen za '{PropertyName}'.",
			"AsyncPredicateValidator" => "Zadan uslov nije ispunjen za '{PropertyName}'.",
			"RegularExpressionValidator" => "'{PropertyName}' nije u odgovarajućem formatu.",
			"EqualValidator" => "'{PropertyName}' mora biti jednak '{ComparisonValue}'.",
			"ExactLengthValidator" => "'{PropertyName}' mora imati tačno {MaxLength} karaktera. Uneseno je {TotalLength} karaktera.",
			"InclusiveBetweenValidator" => "'{PropertyName}' mora biti između {From} i {To}. Uneseno je {PropertyValue}.",
			"ExclusiveBetweenValidator" => "'{PropertyName}' mora biti između {From} i {To} (ekskluzivno). Uneseno je {PropertyValue}.",
			"CreditCardValidator" => "'{PropertyName}' nije validna kreditna kartica.",
			"ScalePrecisionValidator" => "'{PropertyName}' ne smije imati više od {ExpectedPrecision} cifara, sa dozvoljenih {ExpectedScale} decimalnih mjesta. Uneseno je {Digits} cifara i {ActualScale} decimalnih mjesta.",
			"EmptyValidator" => "'{PropertyName}' mora biti prazno.",
			"NullValidator" => "'{PropertyName}' mora biti prazno.",
			"EnumValidator" => "'{PropertyName}' ima raspon vrijednosti koji ne uključuje '{PropertyValue}'.",
			// Additional fallback messages used by clientside validation integration.
			"Length_Simple" => "'{PropertyName}' mora imati između {MinLength} i {MaxLength} karaktera.",
			"MinimumLength_Simple" => "'{PropertyName}' mora imati najmanje {MinLength} karaktera.",
			"MaximumLength_Simple" => "'{PropertyName}' ne smije imati više od {MaxLength} karaktera.",
			"ExactLength_Simple" => "'{PropertyName}' mora imati tačno {MaxLength} karaktera.",
			"InclusiveBetween_Simple" => "'{PropertyName}' mora biti između {From} i {To}.",
			_ => null,
		};
	}
}
