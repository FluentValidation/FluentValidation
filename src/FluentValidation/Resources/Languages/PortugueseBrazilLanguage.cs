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

	internal class PortugueseBrazilLanguage {
		public const string Culture = "pt-BR";

		public static string GetTranslation(string key) => key switch {
			nameof(EmailValidator) => "'{PropertyName}' é um endereço de email inválido.",
			nameof(GreaterThanOrEqualValidator) => "'{PropertyName}' deve ser superior ou igual a '{ComparisonValue}'.",
			nameof(GreaterThanValidator) => "'{PropertyName}' deve ser superior a '{ComparisonValue}'.",
			nameof(LengthValidator) => "'{PropertyName}' deve ter entre {MinLength} e {MaxLength} caracteres. Você digitou {TotalLength} caracteres.",
			nameof(MinimumLengthValidator) => "'{PropertyName}' deve ser maior ou igual a {MinLength} caracteres. Você digitou {TotalLength} caracteres.",
			nameof(MaximumLengthValidator) => "'{PropertyName}' deve ser menor ou igual a {MaxLength} caracteres. Você digitou {TotalLength} caracteres.",
			nameof(LessThanOrEqualValidator) => "'{PropertyName}' deve ser inferior ou igual a '{ComparisonValue}'.",
			nameof(LessThanValidator) => "'{PropertyName}' deve ser inferior a '{ComparisonValue}'.",
			nameof(NotEmptyValidator) => "'{PropertyName}' deve ser informado.",
			nameof(NotEqualValidator) => "'{PropertyName}' deve ser diferente de '{ComparisonValue}'.",
			nameof(NotNullValidator) => "'{PropertyName}' não pode ser nulo.",
			nameof(PredicateValidator) => "'{PropertyName}' não atende a condição definida.",
			nameof(AsyncPredicateValidator) => "'{PropertyName}' não atende a condição definida.",
			nameof(RegularExpressionValidator) => "'{PropertyName}' não está no formato correto.",
			nameof(EqualValidator) => "'{PropertyName}' deve ser igual a '{ComparisonValue}'.",
			nameof(ExactLengthValidator) => "'{PropertyName}' deve ter no máximo {MaxLength} caracteres. Você digitou {TotalLength} caracteres.",
			nameof(ExclusiveBetweenValidator) => "'{PropertyName}' deve, exclusivamente, estar entre {From} e {To}. Você digitou {Value}.",
			nameof(InclusiveBetweenValidator) => "'{PropertyName}' deve estar entre {From} e {To}. Você digitou {Value}.",
			nameof(CreditCardValidator) => "'{PropertyName}' não é um número válido de cartão de crédito.",
			nameof(ScalePrecisionValidator) => "'{PropertyName}' não pode ter mais do que {ExpectedPrecision} dígitos no total, com {ExpectedScale} dígitos decimais. {Digits} dígitos e {ActualScale} decimais foram informados.",
			nameof(EmptyValidator) => "'{PropertyName}' deve estar vazio.",
			nameof(NullValidator) => "'{PropertyName}' deve estar null.",
			nameof(EnumValidator) => "'{PropertyName}' possui um intervalo de valores que não inclui '{PropertyValue}'.",
			// Additional fallback messages used by clientside validation integration.
			"Length_Simple" => "'{PropertyName}' deve ter entre {MinLength} e {MaxLength} caracteres.",
			"MinimumLength_Simple" => "'{PropertyName}' deve ser maior ou igual a {MinLength} caracteres.",
			"MaximumLength_Simple" => "'{PropertyName}' deve ser menor ou igual a {MaxLength} caracteres.",
			"ExactLength_Simple" => "'{PropertyName}' deve ter no máximo {MaxLength} caracteres.",
			"InclusiveBetween_Simple" => "'{PropertyName}' deve estar entre {From} e {To}.",
			_ => null,
		};
	}
}
