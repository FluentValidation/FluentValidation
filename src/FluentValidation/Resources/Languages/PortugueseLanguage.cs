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

	internal class PortugueseLanguage {
		public const string Culture = "pt";

		public static string GetTranslation(string key) => key switch {
			nameof(EmailValidator) => "'{PropertyName}' é um endereço de email inválido.",
			nameof(GreaterThanOrEqualValidator) => "'{PropertyName}' deve ser superior ou igual a '{ComparisonValue}'.",
			nameof(GreaterThanValidator) => "'{PropertyName}' deve ser superior a '{ComparisonValue}'.",
			nameof(LengthValidator) => "'{PropertyName}' deve ter {MinLength} a {MaxLength} caracteres. Introduziu {TotalLength} caracteres.",
			nameof(MinimumLengthValidator) => "'{PropertyName}' deve ser maior ou igual a caracteres {MinLength}. Você digitou caracteres {TotalLength}.",
			nameof(MaximumLengthValidator) => "'{PropertyName}' deve ser menor ou igual a caracteres {MaxLength}. Você digitou caracteres {TotalLength}.",
			nameof(LessThanOrEqualValidator) => "'{PropertyName}' deve ser inferior ou igual a '{ComparisonValue}'.",
			nameof(LessThanValidator) => "'{PropertyName}' deve ser inferior a '{ComparisonValue}'.",
			nameof(NotEmptyValidator) => "'{PropertyName}' deve ser definido.",
			nameof(NotEqualValidator) => "'{PropertyName}' deve ser diferente de '{ComparisonValue}'.",
			nameof(NotNullValidator) => "'{PropertyName}' não pode ser nulo.",
			nameof(PredicateValidator) => "'{PropertyName}' não verifica a condição definida.",
			nameof(AsyncPredicateValidator) => "'{PropertyName}' não verifica a condição definida.",
			nameof(RegularExpressionValidator) => "'{PropertyName}' não se encontra no formato correcto.",
			nameof(EqualValidator) => "'{PropertyName}' deve ser igual a '{ComparisonValue}'.",
			nameof(ExactLengthValidator) => "'{PropertyName}' deve ter o comprimento de {MaxLength} caracteres. Introduziu {TotalLength} caracteres.",
			nameof(ExclusiveBetweenValidator) => "'{PropertyName}' deve estar entre {From} e {To} (exclusivo). Introduziu {Value}.",
			nameof(InclusiveBetweenValidator) => "'{PropertyName}' deve estar entre {From} e {To}. Introduziu {Value}.",
			nameof(CreditCardValidator) => "'{PropertyName}' não é um número de cartão de crédito válido.",
			nameof(ScalePrecisionValidator) => "'{PropertyName}' pode não ser mais do que dígitos {ExpectedPrecision} no total, com permissão para decimais de {ExpectedScale}. {Digits} dígitos e {ActualScale} decimais foram encontrados.",
			nameof(EmptyValidator) => "'{PropertyName}' deve estar vazio.",
			nameof(NullValidator) => "'{PropertyName}' deve estar vazio.",
			nameof(EnumValidator) => "'{PropertyName}' possui um intervalo de valores que não inclui '{PropertyValue}'.",
			// Additional fallback messages used by clientside validation integration.
			"Length_Simple" => "'{PropertyName}' deve ter {MinLength} a {MaxLength} caracteres.",
			"MinimumLength_Simple" => "'{PropertyName}' deve ser maior ou igual a caracteres {MinLength}.",
			"MaximumLength_Simple" => "'{PropertyName}' deve ser menor ou igual a caracteres {MaxLength}.",
			"ExactLength_Simple" => "'{PropertyName}' deve ter o comprimento de {MaxLength} caracteres.",
			"InclusiveBetween_Simple" => "'{PropertyName}' deve estar entre {From} e {To}.",
			_ => null,
		};
	}
}
