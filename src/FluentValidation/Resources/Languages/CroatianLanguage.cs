#region License
// Copyright (c) Jeremy Skinner (http://www.jeremyskinner.co.uk)
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
// The latest version of this file can be found at https://github.com/JeremySkinner/FluentValidation
#endregion

namespace FluentValidation.Resources {
	using Validators;

	internal class CroatianLanguage : Language {
		public override string Name => "hr";

		public CroatianLanguage() {
			Translate<EmailValidator>("'{PropertyName}' nije validna email adresa.");
			Translate<GreaterThanOrEqualValidator>("'{PropertyName}' mora biti veći ili jednak '{ComparisonValue}'.");
			Translate<GreaterThanValidator>("'{PropertyName}' mora biti veći od '{ComparisonValue}'.");
			Translate<LengthValidator>("'{PropertyName}' mora biti između {MinLength} i {MaxLength} karaktera. Upisali ste {TotalLength} karaktera.");
			Translate<MinimumLengthValidator>("'{PropertyName}' mora biti veći ili jednak znakovima {MinLength}. Unijeli ste znakove {TotalLength}.");
			Translate<MaximumLengthValidator>("'{PropertyName}' mora biti manji ili jednak likovima {MaxLength}. Unijeli ste znakove {TotalLength}.");
			Translate<LessThanOrEqualValidator>("'{PropertyName}' mora biti manji ili jednak '{ComparisonValue}'.");
			Translate<LessThanValidator>("'{PropertyName}' mora biti manji od '{ComparisonValue}'.");
			Translate<NotEmptyValidator>("'{PropertyName}' ne smije biti prazan.");
			Translate<NotEqualValidator>("'{PropertyName}' ne smije biti jednak '{ComparisonValue}'.");
			Translate<NotNullValidator>("Niste upisali '{PropertyName}'");
			Translate<PredicateValidator>("'{PropertyName}' nije validan.");
			Translate<AsyncPredicateValidator>("'{PropertyName}' nije validan.");
			Translate<RegularExpressionValidator>("'{PropertyName}' nije u odgovarajućem formatu.");
			Translate<EqualValidator>("'{PropertyName}' mora biti jednak '{ComparisonValue}'.");
			Translate<ExactLengthValidator>("'{PropertyName}' mora sadržavati {MaxLength} karaktera. Upisali ste {TotalLength} karaktera.");
			Translate<InclusiveBetweenValidator>("'{PropertyName}' mora biti između {From} i {To}. Upisali ste {Value}.");
			Translate<ExclusiveBetweenValidator>("'{PropertyName}' mora biti između {From} i {To} (exclusive). Upisali ste {Value}.");
			Translate<CreditCardValidator>("'{PropertyName}' nije odgovarajuća kreditna kartica.");
			Translate<ScalePrecisionValidator>("'{PropertyName}' ne smije imati više od {expectedPrecision} znamenki, sa dopuštenjem od {expectedScale} decimalna mjesta. Upisali ste {digits} znamenki i {actualScale} decimalna mjesta.");
			Translate<EmptyValidator>("'{PropertyName}' mora biti prazan.");
			Translate<NullValidator>("'{PropertyName}' mora biti prazan.");
			Translate<EnumValidator>("'{PropertyName}' ima raspon vrijednosti koji ne uključuje '{PropertyValue}'.");

		}
	}
}