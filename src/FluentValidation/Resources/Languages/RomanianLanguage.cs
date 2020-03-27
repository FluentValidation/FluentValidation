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

	internal class RomanianLanguage : Language {
		public const string Culture = "ro";
		public override string Name => Culture;

		public RomanianLanguage() {
			Translate<EmailValidator>("'{PropertyName}' nu este o adresă de email validă.");
			Translate<GreaterThanOrEqualValidator>("'{PropertyName}' trebuie să fie mai mare sau egală cu '{ComparisonValue}'.");
			Translate<GreaterThanValidator>("'{PropertyName}' trebuie să fie mai mare ca '{ComparisonValue}'.");
			Translate<LengthValidator>("'{PropertyName}' trebuie să fie între {MinLength} şi {MaxLength} caractere. Ați introdus {TotalLength} caractere.");
			Translate<MinimumLengthValidator>("'{PropertyName}' trebuie să fie mai mare sau egală cu caracterele {MinLength}. Ați introdus {TotalLength} caractere.");
			Translate<MaximumLengthValidator>("'{PropertyName}' trebuie să fie mai mică sau egală cu caracterele {MaxLength}. Ați introdus {TotalLength} caractere.");
			Translate<LessThanOrEqualValidator>("'{PropertyName}' trebuie să fie mai mică sau egală cu '{ComparisonValue}'.");
			Translate<LessThanValidator>("'{PropertyName}' trebuie să fie mai mică decât '{ComparisonValue}'.");
			Translate<NotEmptyValidator>("'{PropertyName}' nu ar trebui să fie goală.");
			Translate<NotEqualValidator>("'{PropertyName}' nu ar trebui să fie egală cu '{ComparisonValue}'.");
			Translate<NotNullValidator>("'{PropertyName}' nu trebui să fie goală.");
			Translate<PredicateValidator>("Condiția specificată nu a fost îndeplinită de '{PropertyName}'.");
			Translate<AsyncPredicateValidator>("Condiția specificată nu a fost îndeplinită de '{PropertyName}'.");
			Translate<RegularExpressionValidator>("'{PropertyName}' nu este în formatul corect.");
			Translate<EqualValidator>("'{PropertyName}' ar trebui să fie egal cu '{ComparisonValue}'.");
			Translate<ExactLengthValidator>("'{PropertyName}' trebui să aibe lungimea maximă {MaxLength} de caractere. Ai introdus {TotalLength} caractere.");
			Translate<InclusiveBetweenValidator>("'{PropertyName}' trebuie sa fie între {From} şi {To}. Ai introdus {Value}.");
			Translate<ExclusiveBetweenValidator>("'{PropertyName}' trebuie sa fie între {From} şi {To} (exclusiv). Ai introdus {Value}.");
			Translate<CreditCardValidator>("'{PropertyName}' nu este un număr de card de credit valid.");
			Translate<ScalePrecisionValidator>("'{PropertyName}' nu poate fi mai mare decât {ExpectedPrecision} de cifre în total, cu alocație pentru {ExpectedScale} zecimale. {Digits} cifre şi {ActualScale} au fost găsite zecimale.");
			Translate<EmptyValidator>("'{PropertyName}' ar trebui să fie goală.");
			Translate<NullValidator>("'{PropertyName}' trebuie să fie goală.");
			Translate<EnumValidator>("'{PropertyName}' are o serie de valori care nu sunt incluse în '{PropertyValue}'.");
			// Additional fallback messages used by clientside validation integration.
			Translate("Length_Simple", "'{PropertyName}' trebuie să fie între {MinLength} şi {MaxLength} caractere.");
			Translate("MinimumLength_Simple", "'{PropertyName}' trebuie să fie mai mare sau egală cu caracterele {MinLength}.");
			Translate("MaximumLength_Simple", "'{PropertyName}' trebuie să fie mai mică sau egală cu caracterele {MaxLength}.");
			Translate("ExactLength_Simple", "'{PropertyName}' trebui să aibe lungimea maximă {MaxLength} de caractere.");
			Translate("InclusiveBetween_Simple", "'{PropertyName}' trebuie sa fie între {From} şi {To}.");

		}
	}
}
