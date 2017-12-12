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

	internal class CzechLanguage : Language {
		public override string Name => "cs";

		public CzechLanguage() {
			Translate<EmailValidator>("Pole '{PropertyName}' musí obsahovat platnou emailovou adresu.");
			Translate<GreaterThanOrEqualValidator>("Hodnota pole '{PropertyName}' musí být větší nebo rovna '{ComparisonValue}'.");
			Translate<GreaterThanValidator>("Hodnota pole '{PropertyName}' musí být větší než '{ComparisonValue}'.");
			Translate<LengthValidator>("Délka pole '{PropertyName}' musí být v rozsahu {MinLength} až {MaxLength} znaků. Vámi zadaná délka je {TotalLength} znaků.");
			Translate<MinimumLengthValidator>("Délka pole '{PropertyName}' musí být větší nebo roven znakům {MinLength}. Zadali jste znaky {TotalLength}.");
			Translate<MaximumLengthValidator>("Délka pole '{PropertyName}' musí být menší nebo rovno {MaxLength} znakům. Zadali jste znaky {TotalLength}.");
			Translate<LessThanOrEqualValidator>("Hodnota pole '{PropertyName}' musí být menší nebo rovna '{ComparisonValue}'.");
			Translate<LessThanValidator>("Hodnota pole '{PropertyName}' musí být menší než '{ComparisonValue}'.");
			Translate<NotEmptyValidator>("Pole '{PropertyName}' nesmí být prázdné.");
			Translate<NotEqualValidator>("Pole '{PropertyName}' nesmí být rovno '{ComparisonValue}'.");
			Translate<NotNullValidator>("Pole '{PropertyName}' nesmí být prázdné.");
			Translate<PredicateValidator>("Nebyla splněna podmínka pro pole '{PropertyName}'.");
			Translate<AsyncPredicateValidator>("Nebyla splněna podmínka pro pole '{PropertyName}'.");
			Translate<RegularExpressionValidator>("Pole '{PropertyName}' nemá správný formát.");
			Translate<EqualValidator>("Hodnota pole '{PropertyName}' musí být rovna '{ComparisonValue}'.");
			Translate<ExactLengthValidator>("Hodnota pole '{PropertyName}' musí být {MaxLength} znaků dlouhá. Vámi zadaná délka je {TotalLength} znaků.");
			Translate<InclusiveBetweenValidator>("Hodnota pole '{PropertyName}' musí být mezi {From} a {To} (včetně). Vámi zadaná hodnota je {Value}.");
			Translate<ExclusiveBetweenValidator>("Hodnota pole '{PropertyName}' musí být větší než {From} a menší než {To}. Vámi zadaná hodnota je {Value}.");
			Translate<CreditCardValidator>("Pole '{PropertyName}' musí obsahovat platné číslo platební karty.");
			Translate<ScalePrecisionValidator>("Pole '{PropertyName}' nesmí mít víc jak {expectedPrecision} číslic, a {expectedScale} desetinných míst. Vámi bylo zadáno {digits} číslic a {actualScale} desetinných míst.");
			Translate<EmptyValidator>("'{PropertyName}' by měl být prázdný.");
			Translate<NullValidator>("'{PropertyName}' musí být prázdné.");
			Translate<EnumValidator>("'{PropertyName}' má rozsah hodnot, které neobsahují '{PropertyValue}'.");
		}
	}
}
