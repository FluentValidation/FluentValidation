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

	internal class CzechLanguage : Language {
		public const string Culture = "cs";
		public override string Name => Culture;

		public CzechLanguage() {
			Translate<EmailValidator>("Pole '{PropertyName}' musí obsahovat platnou emailovou adresu.");
			Translate<GreaterThanOrEqualValidator>("Hodnota pole '{PropertyName}' musí být větší nebo rovna '{ComparisonValue}'.");
			Translate<GreaterThanValidator>("Hodnota pole '{PropertyName}' musí být větší než '{ComparisonValue}'.");
			Translate<LengthValidator>("Délka pole '{PropertyName}' musí být v rozsahu {MinLength} až {MaxLength} znaků. Vámi zadaná délka je {TotalLength} znaků.");
			Translate<MinimumLengthValidator>("Délka pole '{PropertyName}' musí být větší nebo rovna {MinLength} znakům. Vámi zadaná délka je {TotalLength} znaků.");
			Translate<MaximumLengthValidator>("Délka pole '{PropertyName}' musí být menší nebo rovna {MaxLength} znakům. Vámi zadaná délka je {TotalLength} znaků.");
			Translate<LessThanOrEqualValidator>("Hodnota pole '{PropertyName}' musí být menší nebo rovna '{ComparisonValue}'.");
			Translate<LessThanValidator>("Hodnota pole '{PropertyName}' musí být menší než '{ComparisonValue}'.");
			Translate<NotEmptyValidator>("Pole '{PropertyName}' nesmí být prázdné.");
			Translate<NotEqualValidator>("Pole '{PropertyName}' nesmí být rovno '{ComparisonValue}'.");
			Translate<NotNullValidator>("Pole '{PropertyName}' nesmí být prázdné.");
			Translate<PredicateValidator>("Nebyla splněna podmínka pro pole '{PropertyName}'.");
			Translate<AsyncPredicateValidator>("Nebyla splněna podmínka pro pole '{PropertyName}'.");
			Translate<RegularExpressionValidator>("Pole '{PropertyName}' nemá správný formát.");
			Translate<EqualValidator>("Hodnota pole '{PropertyName}' musí být rovna '{ComparisonValue}'.");
			Translate<ExactLengthValidator>("Délka pole '{PropertyName}' musí být {MaxLength} znaků. Vámi zadaná délka je {TotalLength} znaků.");
			Translate<InclusiveBetweenValidator>("Hodnota pole '{PropertyName}' musí být mezi {From} a {To} (včetně). Vámi zadaná hodnota je {Value}.");
			Translate<ExclusiveBetweenValidator>("Hodnota pole '{PropertyName}' musí být větší než {From} a menší než {To}. Vámi zadaná hodnota je {Value}.");
			Translate<CreditCardValidator>("Pole '{PropertyName}' musí obsahovat platné číslo platební karty.");
			Translate<ScalePrecisionValidator>("Pole '{PropertyName}' nesmí mít víc než {ExpectedPrecision} číslic a {ExpectedScale} desetinných míst. Vámi bylo zadáno {Digits} číslic a {ActualScale} desetinných míst.");
			Translate<EmptyValidator>("Pole '{PropertyName}' musí být prázdné.");
			Translate<NullValidator>("Pole '{PropertyName}' musí být prázdné.");
			Translate<EnumValidator>("Pole '{PropertyName}' má rozsah hodnot, které neobsahují '{PropertyValue}'.");
			// Additional fallback messages used by clientside validation integration.
			Translate("Length_Simple", "Délka pole '{PropertyName}' musí být v rozsahu {MinLength} až {MaxLength} znaků.");
			Translate("MinimumLength_Simple", "Délka pole '{PropertyName}' musí být větší nebo rovna {MinLength} znakům.");
			Translate("MaximumLength_Simple", "Délka pole '{PropertyName}' musí být menší nebo rovna {MaxLength} znakům.");
			Translate("ExactLength_Simple", "Délka pole '{PropertyName}' musí být {MaxLength} znaků.");
			Translate("InclusiveBetween_Simple", "Hodnota pole '{PropertyName}' musí být mezi {From} a {To} (včetně).");
		}
	}
}
