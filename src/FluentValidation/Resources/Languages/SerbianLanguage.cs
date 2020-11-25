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

	internal class SerbianLanguage {
		public const string Culture = "sr";

		public static string GetTranslation(string key) => key switch {
			nameof(EmailValidator) => "'{PropertyName}' nije validna email adresa.",
			nameof(GreaterThanOrEqualValidator) => "'{PropertyName}' mora biti veće ili jednako '{ComparisonValue}'.",
			nameof(GreaterThanValidator) => "'{PropertyName}' mora biti veće od '{ComparisonValue}'.",
			nameof(LengthValidator) => "'{PropertyName}' mora imati između {MinLength} i {MaxLength} karatkera. Uneseno je {TotalLength} karaktera.",
			nameof(MinimumLengthValidator) => "'{PropertyName}' mora imati najmanje {MinLength} karaktera. Uneseno je {TotalLength} karaktera.",
			nameof(MaximumLengthValidator) => "'{PropertyName}' ne smije imati više od {MaxLength} karaktera. Uneseno je {TotalLength} karaktera.",
			nameof(LessThanOrEqualValidator) => "'{PropertyName}' mora biti manje ili jednako '{ComparisonValue}'.",
			nameof(LessThanValidator) => "'{PropertyName}' mora biti manje od '{ComparisonValue}'.",
			nameof(NotEmptyValidator) => "'{PropertyName}' ne smije biti prazan.",
			nameof(NotEqualValidator) => "'{PropertyName}' ne smije biti jednak '{ComparisonValue}'.",
			nameof(NotNullValidator) => "'{PropertyName}' ne smije biti prazan.",
			nameof(PredicateValidator) => "Zadan uslov nije ispunjen za '{PropertyName}'.",
			nameof(AsyncPredicateValidator) => "Zadan uslov nije ispunjen za '{PropertyName}'.",
			nameof(RegularExpressionValidator) => "'{PropertyName}' nije u odgovarajućem formatu.",
			nameof(EqualValidator) => "'{PropertyName}' mora biti jednak '{ComparisonValue}'.",
			nameof(ExactLengthValidator) => "'{PropertyName}' mora imati tačno {MaxLength} karaktera. Uneseno je {TotalLength} karaktera.",
			nameof(InclusiveBetweenValidator) => "'{PropertyName}' mora biti između {From} i {To}. Uneseno je {Value}.",
			nameof(ExclusiveBetweenValidator) => "'{PropertyName}' mora biti između {From} i {To} (ekskluzivno). Uneseno je {Value}.",
			nameof(CreditCardValidator) => "'{PropertyName}' nije validna kreditna kartica.",
			nameof(ScalePrecisionValidator) => "'{PropertyName}' ne smije imati više od {ExpectedPrecision} cifara, sa dozvoljenih {ExpectedScale} decimalnih mjesta. Uneseno je {Digits} cifara i {ActualScale} decimalnih mjesta.",
			nameof(EmptyValidator) => "'{PropertyName}' mora biti prazno.",
			nameof(NullValidator) => "'{PropertyName}' mora biti prazno.",
			nameof(EnumValidator) => "'{PropertyName}' ima raspon vrijednosti koji ne uključuje '{PropertyValue}'.",
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
