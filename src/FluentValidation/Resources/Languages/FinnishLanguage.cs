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

	internal class FinnishLanguage : Language {
		public override string Name => "fi";

		public FinnishLanguage() {
			Translate<EmailValidator>("'{PropertyName}' ei ole kelvollinen sähköpostiosoite.");
			Translate<GreaterThanOrEqualValidator>("'{PropertyName}' pitää olla suurempi tai yhtä suuri kuin '{ComparisonValue}'.");
			Translate<GreaterThanValidator>("'{PropertyName}' pitää olla suurempi kuin '{ComparisonValue}'.");
			Translate<LengthValidator>("'{PropertyName}' pitää olla välillä {MinLength} ja {MaxLength} merkkiä. Syötit {TotalLength} merkkiä.");
			Translate<MinimumLengthValidator>("'{PropertyName}' on oltava suurempi tai yhtä suuri kuin {MinLength} merkkiä. Syöt {TotalLength} merkkejä.");
			Translate<MaximumLengthValidator>("'{PropertyName}' on oltava pienempi tai yhtä suuri kuin {MaxLength} merkkiä. Syöt {TotalLength} merkkejä.");
			Translate<LessThanOrEqualValidator>("'{PropertyName}' pitää olla pienempi tai yhtä suuri kuin '{ComparisonValue}'.");
			Translate<LessThanValidator>("'{PropertyName}' pitää olla pienempi kuin '{ComparisonValue}'.");
			Translate<NotEmptyValidator>("'{PropertyName}' ei pitäisi olla tyhjä.");
			Translate<NotEqualValidator>("'{PropertyName}' ei voi olla yhtäsuuri kuin '{ComparisonValue}'.");
			Translate<NotNullValidator>("'{PropertyName}' ei voi olla tyhjä.");
			Translate<PredicateValidator>("Määritetty ehto ei toteutunut '{PropertyName}'.");
			Translate<AsyncPredicateValidator>("Määritetty ehto ei toteutunut '{PropertyName}'.");
			Translate<RegularExpressionValidator>("'{PropertyName}' ei ole oikeassa muodossa.");
			Translate<EqualValidator>("'{PropertyName}' pitäisi olla yhtäsuuri kuin '{ComparisonValue}'.");
			Translate<ExactLengthValidator>("'{PropertyName}' pitää olla {MaxLength} merkkiä pitkä. Syötit {TotalLength} merkkiä.");
			Translate<ExclusiveBetweenValidator>("'{PropertyName}' pitää olla välillä {From} ja {To} (exclusive). Syötit {Value}.");
			Translate<InclusiveBetweenValidator>("'{PropertyName}' pitää olla välillä {From} ja {To}. Syötit {Value}.");
			Translate<CreditCardValidator>("'{PropertyName}' ei ole kelvollinen luottokortin numero.");
			Translate<ScalePrecisionValidator>("'{PropertyName}' ei saa olla enempää kuin {expectedPrecision} numerot, joiden arvo on {expectedScale} desimaaleja. {digits} numeroita ja {actualScale} desimaaleja löytyi.");
			Translate<EmptyValidator>("'{PropertyName}' pitäisi olla tyhjä.");
			Translate<NullValidator>("'{PropertyName}' pitäisi olla tyhjä.");
			Translate<EnumValidator>("'{PropertyName}' sisältää joukon arvoja, jotka eivät sisällä '{PropertyValue}'.");
		}
	}
}
