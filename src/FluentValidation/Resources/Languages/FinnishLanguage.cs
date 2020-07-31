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

	internal class FinnishLanguage : Language {
		public const string Culture = "fi";
		public override string Name => Culture;

		public FinnishLanguage() {
			Translate<EmailValidator>("'{PropertyName}' ei ole kelvollinen sähköpostiosoite.");
			Translate<GreaterThanOrEqualValidator>("'{PropertyName}' pitää olla suurempi tai yhtä suuri kuin '{ComparisonValue}'.");
			Translate<GreaterThanValidator>("'{PropertyName}' pitää olla suurempi kuin '{ComparisonValue}'.");
			Translate<LengthValidator>("'{PropertyName}' pitää olla {MinLength}-{MaxLength} merkkiä. Syötit {TotalLength} merkkiä.");
			Translate<MinimumLengthValidator>("'{PropertyName}' pitää olla vähintään {MinLength} merkkiä. Syötit {TotalLength} merkkiä.");
			Translate<MaximumLengthValidator>("'{PropertyName}' saa olla enintään {MaxLength} merkkiä. Syötit {TotalLength} merkkiä.");
			Translate<LessThanOrEqualValidator>("'{PropertyName}' pitää olla pienempi tai yhtä suuri kuin '{ComparisonValue}'.");
			Translate<LessThanValidator>("'{PropertyName}' pitää olla pienempi kuin '{ComparisonValue}'.");
			Translate<NotEmptyValidator>("'{PropertyName}' ei voi olla tyhjä.");
			Translate<NotEqualValidator>("'{PropertyName}' ei voi olla yhtä suuri kuin '{ComparisonValue}'.");
			Translate<NotNullValidator>("'{PropertyName}' ei voi olla tyhjä.");
			Translate<PredicateValidator>("'{PropertyName}' määritetty ehto ei toteutunut.");
			Translate<AsyncPredicateValidator>("'{PropertyName}' määritetty ehto ei toteutunut.");
			Translate<RegularExpressionValidator>("'{PropertyName}' ei ole oikeassa muodossa.");
			Translate<EqualValidator>("'{PropertyName}' pitäisi olla yhtä suuri kuin '{ComparisonValue}'.");
			Translate<ExactLengthValidator>("'{PropertyName}' pitää olla {MaxLength} merkkiä. Syötit {TotalLength} merkkiä.");
			Translate<ExclusiveBetweenValidator>("'{PropertyName}' pitää olla suljetulla välillä {From}-{To}. Syötit {Value}.");
			Translate<InclusiveBetweenValidator>("'{PropertyName}' pitää olla välillä {From}-{To}. Syötit {Value}.");
			Translate<CreditCardValidator>("'{PropertyName}' ei ole kelvollinen luottokortin numero.");
			Translate<ScalePrecisionValidator>("'{PropertyName}' ei saa sisältää enempää kuin {ExpectedPrecision} numeroa, sallien {ExpectedScale} desimaalia. {Digits} numeroa ja {ActualScale} desimaalia löytyi.");
			Translate<EmptyValidator>("'{PropertyName}' pitäisi olla tyhjä.");
			Translate<NullValidator>("'{PropertyName}' pitäisi olla tyhjä.");
			Translate<EnumValidator>("'{PropertyName}' arvoista ei löydy '{PropertyValue}'.");
			// Additional fallback messages used by clientside validation integration.
			Translate("Length_Simple", "'{PropertyName}' pitää olla {MinLength}-{MaxLength} merkkiä.");
			Translate("MinimumLength_Simple", "'{PropertyName}' saa olla vähintään {MinLength} merkkiä.");
			Translate("MaximumLength_Simple", "'{PropertyName}' pitää olla enintään {MaxLength} merkkiä.");
			Translate("ExactLength_Simple", "'{PropertyName}' pitää olla {MaxLength} merkkiä pitkä.");
			Translate("InclusiveBetween_Simple", "'{PropertyName}' pitää olla välillä {From}-{To}.");
		}
	}
}
