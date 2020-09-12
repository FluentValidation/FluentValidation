#region License

// Copyright (c) .NET Foundation and contributors.
//
// Licensed under the Apache License, Version 2.0 (the "License",
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

	internal class RomanianLanguage {
		public const string Culture = "ro";

		public static string GetTranslation(string key) => key switch {
			nameof(EmailValidator) => "'{PropertyName}' nu este o adresă de email validă.",
			nameof(GreaterThanOrEqualValidator) => "'{PropertyName}' trebuie să fie mai mare sau egală cu '{ComparisonValue}'.",
			nameof(GreaterThanValidator) => "'{PropertyName}' trebuie să fie mai mare ca '{ComparisonValue}'.",
			nameof(LengthValidator) => "'{PropertyName}' trebuie să fie între {MinLength} şi {MaxLength} caractere. Ați introdus {TotalLength} caractere.",
			nameof(MinimumLengthValidator) => "'{PropertyName}' trebuie să fie mai mare sau egală cu caracterele {MinLength}. Ați introdus {TotalLength} caractere.",
			nameof(MaximumLengthValidator) => "'{PropertyName}' trebuie să fie mai mică sau egală cu caracterele {MaxLength}. Ați introdus {TotalLength} caractere.",
			nameof(LessThanOrEqualValidator) => "'{PropertyName}' trebuie să fie mai mică sau egală cu '{ComparisonValue}'.",
			nameof(LessThanValidator) => "'{PropertyName}' trebuie să fie mai mică decât '{ComparisonValue}'.",
			nameof(NotEmptyValidator) => "'{PropertyName}' nu ar trebui să fie goală.",
			nameof(NotEqualValidator) => "'{PropertyName}' nu ar trebui să fie egală cu '{ComparisonValue}'.",
			nameof(NotNullValidator) => "'{PropertyName}' nu trebui să fie goală.",
			nameof(PredicateValidator) => "Condiția specificată nu a fost îndeplinită de '{PropertyName}'.",
			nameof(AsyncPredicateValidator) => "Condiția specificată nu a fost îndeplinită de '{PropertyName}'.",
			nameof(RegularExpressionValidator) => "'{PropertyName}' nu este în formatul corect.",
			nameof(EqualValidator) => "'{PropertyName}' ar trebui să fie egal cu '{ComparisonValue}'.",
			nameof(ExactLengthValidator) => "'{PropertyName}' trebui să aibe lungimea maximă {MaxLength} de caractere. Ai introdus {TotalLength} caractere.",
			nameof(InclusiveBetweenValidator) => "'{PropertyName}' trebuie sa fie între {From} şi {To}. Ai introdus {Value}.",
			nameof(ExclusiveBetweenValidator) => "'{PropertyName}' trebuie sa fie între {From} şi {To} (exclusiv). Ai introdus {Value}.",
			nameof(CreditCardValidator) => "'{PropertyName}' nu este un număr de card de credit valid.",
			nameof(ScalePrecisionValidator) => "'{PropertyName}' nu poate fi mai mare decât {ExpectedPrecision} de cifre în total, cu alocație pentru {ExpectedScale} zecimale. {Digits} cifre şi {ActualScale} au fost găsite zecimale.",
			nameof(EmptyValidator) => "'{PropertyName}' ar trebui să fie goală.",
			nameof(NullValidator) => "'{PropertyName}' trebuie să fie goală.",
			nameof(EnumValidator) => "'{PropertyName}' are o serie de valori care nu sunt incluse în '{PropertyValue}'.",
			// Additional fallback messages used by clientside validation integration.
			"Length_Simple" => "'{PropertyName}' trebuie să fie între {MinLength} şi {MaxLength} caractere.",
			"MinimumLength_Simple" => "'{PropertyName}' trebuie să fie mai mare sau egală cu caracterele {MinLength}.",
			"MaximumLength_Simple" => "'{PropertyName}' trebuie să fie mai mică sau egală cu caracterele {MaxLength}.",
			"ExactLength_Simple" => "'{PropertyName}' trebui să aibe lungimea maximă {MaxLength} de caractere.",
			"InclusiveBetween_Simple" => "'{PropertyName}' trebuie sa fie între {From} şi {To}.",
			_ => null,
		};
	}
}
