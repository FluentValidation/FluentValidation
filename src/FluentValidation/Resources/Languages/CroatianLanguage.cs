#region License

// Copyright (c) .NET Foundation and contributors.
//
// Licensed under the Apache License, Version 2.0 (the "License",
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

	internal class CroatianLanguage {
		public const string Culture = "hr";

		public static string GetTranslation(string key) => key switch {
			nameof(EmailValidator) => "'{PropertyName}' nije ispravna e-mail adresa.",
			nameof(GreaterThanOrEqualValidator) => "'{PropertyName}' mora biti veći ili jednak '{ComparisonValue}'.",
			nameof(GreaterThanValidator) => "'{PropertyName}' mora biti veći od '{ComparisonValue}'.",
			nameof(LengthValidator) => "'{PropertyName}' mora biti između {MinLength} i {MaxLength} znakova. Upisali ste {TotalLength} znakova.",
			nameof(MinimumLengthValidator) => "'{PropertyName}' mora imati duljinu veću ili jednaku {MinLength}. Unijeli ste {TotalLength} znakova.",
			nameof(MaximumLengthValidator) => "'{PropertyName}' mora imati duljinu manju ili jednaku {MaxLength}. Unijeli ste {TotalLength} znakova.",
			nameof(LessThanOrEqualValidator) => "'{PropertyName}' mora biti manji ili jednak '{ComparisonValue}'.",
			nameof(LessThanValidator) => "'{PropertyName}' mora biti manji od '{ComparisonValue}'.",
			nameof(NotEmptyValidator) => "'{PropertyName}' ne smije biti prazan.",
			nameof(NotEqualValidator) => "'{PropertyName}' ne smije biti jednak '{ComparisonValue}'.",
			nameof(NotNullValidator) => "Niste upisali '{PropertyName}'",
			nameof(PredicateValidator) => "'{PropertyName}' nije ispravan.",
			nameof(AsyncPredicateValidator) => "'{PropertyName}' nije ispravan.",
			nameof(RegularExpressionValidator) => "'{PropertyName}' nije u odgovarajućem formatu.",
			nameof(EqualValidator) => "'{PropertyName}' mora biti jednak '{ComparisonValue}'.",
			nameof(ExactLengthValidator) => "'{PropertyName}' mora sadržavati {MaxLength} znakova. Upisali ste {TotalLength} znakova.",
			nameof(InclusiveBetweenValidator) => "'{PropertyName}' mora biti između {From} i {To}. Upisali ste {Value}.",
			nameof(ExclusiveBetweenValidator) => "'{PropertyName}' mora biti između {From} i {To} (ne uključujući granice). Upisali ste {Value}.",
			nameof(CreditCardValidator) => "'{PropertyName}' nije odgovarajuća kreditna kartica.",
			nameof(ScalePrecisionValidator) => "'{PropertyName}' ne smije imati više od {ExpectedPrecision} znamenki, sa {ExpectedScale} decimalna mjesta. Upisali ste {Digits} znamenki i {ActualScale} decimalna mjesta.",
			nameof(EmptyValidator) => "'{PropertyName}' mora biti prazan.",
			nameof(NullValidator) => "'{PropertyName}' mora biti prazan.",
			nameof(EnumValidator) => "'{PropertyName}' ima raspon vrijednosti koji ne uključuje '{PropertyValue}'.",
			// Additional fallback messages used by clientside validation integration.
			"Length_Simple" => "'{PropertyName}' mora biti između {MinLength} i {MaxLength} znakova.",
			"MinimumLength_Simple" => "'{PropertyName}' mora imati duljinu veću ili jednaku {MinLength}.",
			"MaximumLength_Simple" => "'{PropertyName}' mora imati duljinu manju ili jednaku {MaxLength}.",
			"ExactLength_Simple" => "'{PropertyName}' mora sadržavati {MaxLength} znakova.",
			"InclusiveBetween_Simple" => "'{PropertyName}' mora biti između {From} i {To}.",
			_ => null,
		};
	}
}
