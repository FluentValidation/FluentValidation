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

		internal class SlovenianLanguage : Language {
				public const string Culture = "sl";
				public override string Name => Culture;

				public SlovenianLanguage() {
						Translate<EmailValidator>("'{PropertyName}' ni veljaven e-poštni naslov.");
						Translate<GreaterThanOrEqualValidator>("'{PropertyName}' mora biti večji ali enak '{ComparisonValue}'.");
						Translate<GreaterThanValidator>("'{PropertyName}' mora biti večji od '{ComparisonValue}'.");
						Translate<LengthValidator>("'{PropertyName}' mora imeti dolžino med {MinLength} in {MaxLength} znakov. Vnesli ste {TotalLength} znakov.");
						Translate<MinimumLengthValidator>("'{PropertyName}' mora imeti dolžino večjo ali enako {MinLength}. Vnesli ste {TotalLength} znakova.");
						Translate<MaximumLengthValidator>("'{PropertyName}' mora imeti dolžino manjšo ali enako {MaxLength}. Vnesli ste {TotalLength} znakova.");
						Translate<LessThanOrEqualValidator>("'{PropertyName}' mora biti manjši ali enak '{ComparisonValue}'.");
						Translate<LessThanValidator>("'{PropertyName}' mora biti manjši od '{ComparisonValue}'.");
						Translate<NotEmptyValidator>("'{PropertyName}' ne sme biti prazen.");
						Translate<NotEqualValidator>("'{PropertyName}' ne sme biti enak '{ComparisonValue}'.");
						Translate<NotNullValidator>("'{PropertyName}' ne sme biti prazen.");
						Translate<PredicateValidator>("Določen pogoj ni bil izpolnjen za '{PropertyName}'.");
						Translate<AsyncPredicateValidator>("Določen pogoj ni bil izpolnjen za '{PropertyName}'.");
						Translate<RegularExpressionValidator>("'{PropertyName}' ni v pravilni obliki.");
						Translate<EqualValidator>("'{PropertyName}' mora biti enak '{ComparisonValue}'.");
						Translate<ExactLengthValidator>("'{PropertyName}' mora imeti {MaxLength} znakov. Vnesli ste {TotalLength} znakov.");
						Translate<InclusiveBetweenValidator>("'{PropertyName}' mora biti med {From} in {To}. Vnesli ste {Value}.");
						Translate<ExclusiveBetweenValidator>("'{PropertyName}' mora biti med {From} in {To} (izključno). Vnesli ste {Value}.");
						Translate<CreditCardValidator>("'{PropertyName}' ni veljavna številka kreditne kartice.");
						Translate<ScalePrecisionValidator>("'{PropertyName}' ne sme biti več kot {ExpectedPrecision} natančno števk in {ExpectedScale} decimalk. Vnesli ste {Digits} števk in {ActualScale} decimalk.");
						Translate<EmptyValidator>("'{PropertyName}' mora biti prazen.");
						Translate<NullValidator>("'{PropertyName}' mora biti prazen.");
						Translate<EnumValidator>("'{PropertyName}' ima obseg vrednosti, ki ne vključuje '{PropertyValue}'.");
						// Additional fallback messages used by clientside validation integration.
						Translate("Length_Simple", "'{PropertyName}' imeti dolžino med {MinLength} in {MaxLength} znakov. ");
						Translate("MinimumLength_Simple", "'{PropertyName}' mora imeti dolžino večjo ali enako {MinLength}.");
						Translate("MaximumLength_Simple", "'{PropertyName}' mora imeti dolžino manjšo ali enako {MaxLength}.");
						Translate("ExactLength_Simple", "'{PropertyName}' mora imeti {MaxLength} znakov.");
						Translate("InclusiveBetween_Simple", "'{PropertyName}' mora biti med {From} in {To}.");
				}
		}
}

