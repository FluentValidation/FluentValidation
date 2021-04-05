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
			"EmailValidator" => "'{PropertyName}' ei ole kelvollinen sähköpostiosoite.",
			"GreaterThanOrEqualValidator" => "'{PropertyName}' pitää olla suurempi tai yhtä suuri kuin '{ComparisonValue}'.",
			"GreaterThanValidator" => "'{PropertyName}' pitää olla suurempi kuin '{ComparisonValue}'.",
			"LengthValidator" => "'{PropertyName}' pitää olla {MinLength}-{MaxLength} merkkiä. Syötit {TotalLength} merkkiä.",
			"MinimumLengthValidator" => "'{PropertyName}' pitää olla vähintään {MinLength} merkkiä. Syötit {TotalLength} merkkiä.",
			"MaximumLengthValidator" => "'{PropertyName}' saa olla enintään {MaxLength} merkkiä. Syötit {TotalLength} merkkiä.",
			"LessThanOrEqualValidator" => "'{PropertyName}' pitää olla pienempi tai yhtä suuri kuin '{ComparisonValue}'.",
			"LessThanValidator" => "'{PropertyName}' pitää olla pienempi kuin '{ComparisonValue}'.",
			"NotEmptyValidator" => "'{PropertyName}' ei voi olla tyhjä.",
			"NotEqualValidator" => "'{PropertyName}' ei voi olla yhtä suuri kuin '{ComparisonValue}'.",
			"NotNullValidator" => "'{PropertyName}' ei voi olla tyhjä.",
			"PredicateValidator" => "'{PropertyName}' määritetty ehto ei toteutunut.",
			"AsyncPredicateValidator" => "'{PropertyName}' määritetty ehto ei toteutunut.",
			"RegularExpressionValidator" => "'{PropertyName}' ei ole oikeassa muodossa.",
			"EqualValidator" => "'{PropertyName}' pitäisi olla yhtä suuri kuin '{ComparisonValue}'.",
			"ExactLengthValidator" => "'{PropertyName}' pitää olla {MaxLength} merkkiä. Syötit {TotalLength} merkkiä.",
			"ExclusiveBetweenValidator" => "'{PropertyName}' pitää olla suljetulla välillä {From}-{To}. Syötit {PropertyValue}.",
			"InclusiveBetweenValidator" => "'{PropertyName}' pitää olla välillä {From}-{To}. Syötit {PropertyValue}.",
			"CreditCardValidator" => "'{PropertyName}' ei ole kelvollinen luottokortin numero.",
			"ScalePrecisionValidator" => "'{PropertyName}' ei saa sisältää enempää kuin {ExpectedPrecision} numeroa, sallien {ExpectedScale} desimaalia. {Digits} numeroa ja {ActualScale} desimaalia löytyi.",
			"EmptyValidator" => "'{PropertyName}' pitäisi olla tyhjä.",
			"NullValidator" => "'{PropertyName}' pitäisi olla tyhjä.",
			"EnumValidator" => "'{PropertyName}' arvoista ei löydy '{PropertyValue}'.",
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
