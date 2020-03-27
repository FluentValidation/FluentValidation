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

namespace FluentValidation.Resources {
	using Validators;

	internal class CroatianLanguage : Language {
		public const string Culture = "hr";
		public override string Name => Culture;

		public CroatianLanguage() {
			Translate<EmailValidator>("'{PropertyName}' nije ispravna e-mail adresa.");
			Translate<GreaterThanOrEqualValidator>("'{PropertyName}' mora biti veći ili jednak '{ComparisonValue}'.");
			Translate<GreaterThanValidator>("'{PropertyName}' mora biti veći od '{ComparisonValue}'.");
			Translate<LengthValidator>("'{PropertyName}' mora biti između {MinLength} i {MaxLength} znakova. Upisali ste {TotalLength} znakova.");
			Translate<MinimumLengthValidator>("'{PropertyName}' mora imati duljinu veću ili jednaku {MinLength}. Unijeli ste {TotalLength} znakova.");
			Translate<MaximumLengthValidator>("'{PropertyName}' mora imati duljinu manju ili jednaku {MaxLength}. Unijeli ste {TotalLength} znakova.");
			Translate<LessThanOrEqualValidator>("'{PropertyName}' mora biti manji ili jednak '{ComparisonValue}'.");
			Translate<LessThanValidator>("'{PropertyName}' mora biti manji od '{ComparisonValue}'.");
			Translate<NotEmptyValidator>("'{PropertyName}' ne smije biti prazan.");
			Translate<NotEqualValidator>("'{PropertyName}' ne smije biti jednak '{ComparisonValue}'.");
			Translate<NotNullValidator>("Niste upisali '{PropertyName}'");
			Translate<PredicateValidator>("'{PropertyName}' nije ispravan.");
			Translate<AsyncPredicateValidator>("'{PropertyName}' nije ispravan.");
			Translate<RegularExpressionValidator>("'{PropertyName}' nije u odgovarajućem formatu.");
			Translate<EqualValidator>("'{PropertyName}' mora biti jednak '{ComparisonValue}'.");
			Translate<ExactLengthValidator>("'{PropertyName}' mora sadržavati {MaxLength} znakova. Upisali ste {TotalLength} znakova.");
			Translate<InclusiveBetweenValidator>("'{PropertyName}' mora biti između {From} i {To}. Upisali ste {Value}.");
			Translate<ExclusiveBetweenValidator>("'{PropertyName}' mora biti između {From} i {To} (ne uključujući granice). Upisali ste {Value}.");
			Translate<CreditCardValidator>("'{PropertyName}' nije odgovarajuća kreditna kartica.");
			Translate<ScalePrecisionValidator>("'{PropertyName}' ne smije imati više od {ExpectedPrecision} znamenki, sa {ExpectedScale} decimalna mjesta. Upisali ste {Digits} znamenki i {ActualScale} decimalna mjesta.");
			Translate<EmptyValidator>("'{PropertyName}' mora biti prazan.");
			Translate<NullValidator>("'{PropertyName}' mora biti prazan.");
			Translate<EnumValidator>("'{PropertyName}' ima raspon vrijednosti koji ne uključuje '{PropertyValue}'.");
			// Additional fallback messages used by clientside validation integration.
			Translate("Length_Simple", "'{PropertyName}' mora biti između {MinLength} i {MaxLength} znakova.");
			Translate("MinimumLength_Simple", "'{PropertyName}' mora imati duljinu veću ili jednaku {MinLength}.");
			Translate("MaximumLength_Simple", "'{PropertyName}' mora imati duljinu manju ili jednaku {MaxLength}.");
			Translate("ExactLength_Simple", "'{PropertyName}' mora sadržavati {MaxLength} znakova.");
			Translate("InclusiveBetween_Simple", "'{PropertyName}' mora biti između {From} i {To}.");
		}
	}
}
