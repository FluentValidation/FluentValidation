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
			Translate<EmailValidator>("'{PropertyName}' není správná emailová adresa.");
			Translate<GreaterThanOrEqualValidator>("Hodnota pole '{PropertyName}' musí být větší nebo rovna '{ComparisonValue}'.");
			Translate<GreaterThanValidator>("Hodnota pole '{PropertyName}' musí být větší než '{ComparisonValue}'.");
			Translate<LengthValidator>("Délka pole '{PropertyName}' must be between {MinLength} and {MaxLength} characters. Vy jste zadal {TotalLength} znaků.");
			Translate<MinimumLengthValidator>("Délka pole '{PropertyName}' must be between {MinLength} and {MaxLength} characters. Vy jste zadal {TotalLength} znaků.");
			Translate<MaximumLengthValidator>("Délka pole '{PropertyName}' must be between {MinLength} and {MaxLength} characters. Vy jste zadal {TotalLength} znaků.");
			Translate<LessThanOrEqualValidator>("Hodnota pole '{PropertyName}' musí být menší nebo rovna '{ComparisonValue}'.");
			Translate<LessThanValidator>("Hodnota pole '{PropertyName}' musí být menší než '{ComparisonValue}'.");
			Translate<NotEmptyValidator>("Pole '{PropertyName}' nesmí být prázdné");
			Translate<NotEqualValidator>("Pole '{PropertyName}' nesmí být rovno '{ComparisonValue}'.");
			Translate<NotNullValidator>("Pole '{PropertyName}' nesmí být prázdné.");
			Translate<PredicateValidator>("Nebyla splněna podmínka pro pole '{PropertyName}'.");
			Translate<AsyncPredicateValidator>("Nebyla splněna podmínka pro pole '{PropertyName}'.");
			Translate<RegularExpressionValidator>("Pole '{PropertyName}' nemá správný formát");
			Translate<EqualValidator>("Hodnota pole '{PropertyName}' by měla být rovna '{ComparisonValue}'.");
			Translate<ExactLengthValidator>("Hodnota pole '{PropertyName}' musí být {MaxLength} znaků dlouhá. Vy jste zadal {TotalLength} znaků.");
			Translate<InclusiveBetweenValidator>("Hodnota pole '{PropertyName}' musí být mezi {From} a {To} (včetně). Vy jste zadal {Value}.");
			Translate<ExclusiveBetweenValidator>("Hodnota pole '{PropertyName}' musí být větší než {From} a menší než {To}. Vy jste zadal {Value}.");
			Translate<CreditCardValidator>("'{PropertyName}' není správné číslo kreditní karty.");
			Translate<ScalePrecisionValidator>("Pole '{PropertyName}' nemůže mít víc jak {expectedPrecision} cifer, a {expectedScale} desetinných míst. Vy jste zadal {digits} cifer a {actualScale} desetinných míst.");

		}
	}
}