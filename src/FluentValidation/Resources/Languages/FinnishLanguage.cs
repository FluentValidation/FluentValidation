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

	internal class FinnishLanguage {
		public const string Culture = "fi";

		public static string GetTranslation(string key) => key switch {
			nameof(EmailValidator) => "'{PropertyName}' ei ole kelvollinen sähköpostiosoite.",
			nameof(GreaterThanOrEqualValidator) => "'{PropertyName}' pitää olla suurempi tai yhtä suuri kuin '{ComparisonValue}'.",
			nameof(GreaterThanValidator) => "'{PropertyName}' pitää olla suurempi kuin '{ComparisonValue}'.",
			nameof(LengthValidator) => "'{PropertyName}' pitää olla {MinLength}-{MaxLength} merkkiä. Syötit {TotalLength} merkkiä.",
			nameof(MinimumLengthValidator) => "'{PropertyName}' pitää olla vähintään {MinLength} merkkiä. Syötit {TotalLength} merkkiä.",
			nameof(MaximumLengthValidator) => "'{PropertyName}' saa olla enintään {MaxLength} merkkiä. Syötit {TotalLength} merkkiä.",
			nameof(LessThanOrEqualValidator) => "'{PropertyName}' pitää olla pienempi tai yhtä suuri kuin '{ComparisonValue}'.",
			nameof(LessThanValidator) => "'{PropertyName}' pitää olla pienempi kuin '{ComparisonValue}'.",
			nameof(NotEmptyValidator) => "'{PropertyName}' ei voi olla tyhjä.",
			nameof(NotEqualValidator) => "'{PropertyName}' ei voi olla yhtä suuri kuin '{ComparisonValue}'.",
			nameof(NotNullValidator) => "'{PropertyName}' ei voi olla tyhjä.",
			nameof(PredicateValidator) => "'{PropertyName}' määritetty ehto ei toteutunut.",
			nameof(AsyncPredicateValidator) => "'{PropertyName}' määritetty ehto ei toteutunut.",
			nameof(RegularExpressionValidator) => "'{PropertyName}' ei ole oikeassa muodossa.",
			nameof(EqualValidator) => "'{PropertyName}' pitäisi olla yhtä suuri kuin '{ComparisonValue}'.",
			nameof(ExactLengthValidator) => "'{PropertyName}' pitää olla {MaxLength} merkkiä. Syötit {TotalLength} merkkiä.",
			nameof(ExclusiveBetweenValidator) => "'{PropertyName}' pitää olla suljetulla välillä {From}-{To}. Syötit {Value}.",
			nameof(InclusiveBetweenValidator) => "'{PropertyName}' pitää olla välillä {From}-{To}. Syötit {Value}.",
			nameof(CreditCardValidator) => "'{PropertyName}' ei ole kelvollinen luottokortin numero.",
			nameof(ScalePrecisionValidator) => "'{PropertyName}' ei saa sisältää enempää kuin {ExpectedPrecision} numeroa, sallien {ExpectedScale} desimaalia. {Digits} numeroa ja {ActualScale} desimaalia löytyi.",
			nameof(EmptyValidator) => "'{PropertyName}' pitäisi olla tyhjä.",
			nameof(NullValidator) => "'{PropertyName}' pitäisi olla tyhjä.",
			nameof(EnumValidator) => "'{PropertyName}' arvoista ei löydy '{PropertyValue}'.",
			// Additional fallback messages used by clientside validation integration.
			"Length_Simple" => "'{PropertyName}' pitää olla {MinLength}-{MaxLength} merkkiä.",
			"MinimumLength_Simple" => "'{PropertyName}' saa olla vähintään {MinLength} merkkiä.",
			"MaximumLength_Simple" => "'{PropertyName}' pitää olla enintään {MaxLength} merkkiä.",
			"ExactLength_Simple" => "'{PropertyName}' pitää olla {MaxLength} merkkiä pitkä.",
			"InclusiveBetween_Simple" => "'{PropertyName}' pitää olla välillä {From}-{To}.",
			_ => null,
		};
	}
}
