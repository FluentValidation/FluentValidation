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
			"EmailValidator" => "'{PropertyName}' é um endereço de email inválido.",
			"GreaterThanOrEqualValidator" => "'{PropertyName}' deve ser superior ou igual a '{ComparisonValue}'.",
			"GreaterThanValidator" => "'{PropertyName}' deve ser superior a '{ComparisonValue}'.",
			"LengthValidator" => "'{PropertyName}' deve ter {MinLength} a {MaxLength} caracteres. Introduziu {TotalLength} caracteres.",
			"MinimumLengthValidator" => "'{PropertyName}' deve ser maior ou igual a caracteres {MinLength}. Você digitou caracteres {TotalLength}.",
			"MaximumLengthValidator" => "'{PropertyName}' deve ser menor ou igual a caracteres {MaxLength}. Você digitou caracteres {TotalLength}.",
			"LessThanOrEqualValidator" => "'{PropertyName}' deve ser inferior ou igual a '{ComparisonValue}'.",
			"LessThanValidator" => "'{PropertyName}' deve ser inferior a '{ComparisonValue}'.",
			"NotEmptyValidator" => "'{PropertyName}' deve ser definido.",
			"NotEqualValidator" => "'{PropertyName}' deve ser diferente de '{ComparisonValue}'.",
			"NotNullValidator" => "'{PropertyName}' não pode ser nulo.",
			"PredicateValidator" => "'{PropertyName}' não verifica a condição definida.",
			"AsyncPredicateValidator" => "'{PropertyName}' não verifica a condição definida.",
			"RegularExpressionValidator" => "'{PropertyName}' não se encontra no formato correcto.",
			"EqualValidator" => "'{PropertyName}' deve ser igual a '{ComparisonValue}'.",
			"ExactLengthValidator" => "'{PropertyName}' deve ter o comprimento de {MaxLength} caracteres. Introduziu {TotalLength} caracteres.",
			"ExclusiveBetweenValidator" => "'{PropertyName}' deve estar entre {From} e {To} (exclusivo). Introduziu {PropertyValue}.",
			"InclusiveBetweenValidator" => "'{PropertyName}' deve estar entre {From} e {To}. Introduziu {PropertyValue}.",
			"CreditCardValidator" => "'{PropertyName}' não é um número de cartão de crédito válido.",
			"ScalePrecisionValidator" => "'{PropertyName}' pode não ser mais do que dígitos {ExpectedPrecision} no total, com permissão para decimais de {ExpectedScale}. {Digits} dígitos e {ActualScale} decimais foram encontrados.",
			"EmptyValidator" => "'{PropertyName}' deve estar vazio.",
			"NullValidator" => "'{PropertyName}' deve estar vazio.",
			"EnumValidator" => "'{PropertyName}' possui um intervalo de valores que não inclui '{PropertyValue}'.",
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
