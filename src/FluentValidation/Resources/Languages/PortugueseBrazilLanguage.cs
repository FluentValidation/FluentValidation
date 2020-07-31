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

	internal class PortugueseBrazilLanguage : Language {
		public const string Culture = "pt-BR";
		public override string Name => Culture;

		public PortugueseBrazilLanguage() {
			Translate<EmailValidator>("'{PropertyName}' é um endereço de email inválido.");
			Translate<GreaterThanOrEqualValidator>("'{PropertyName}' deve ser superior ou igual a '{ComparisonValue}'.");
			Translate<GreaterThanValidator>("'{PropertyName}' deve ser superior a '{ComparisonValue}'.");
			Translate<LengthValidator>("'{PropertyName}' deve ter entre {MinLength} e {MaxLength} caracteres. Você digitou {TotalLength} caracteres.");
			Translate<MinimumLengthValidator>("'{PropertyName}' deve ser maior ou igual a {MinLength} caracteres. Você digitou {TotalLength} caracteres.");
			Translate<MaximumLengthValidator>("'{PropertyName}' deve ser menor ou igual a {MaxLength} caracteres. Você digitou {TotalLength} caracteres.");
			Translate<LessThanOrEqualValidator>("'{PropertyName}' deve ser inferior ou igual a '{ComparisonValue}'.");
			Translate<LessThanValidator>("'{PropertyName}' deve ser inferior a '{ComparisonValue}'.");
			Translate<NotEmptyValidator>("'{PropertyName}' deve ser informado.");
			Translate<NotEqualValidator>("'{PropertyName}' deve ser diferente de '{ComparisonValue}'.");
			Translate<NotNullValidator>("'{PropertyName}' não pode ser nulo.");
			Translate<PredicateValidator>("'{PropertyName}' não atende a condição definida.");
			Translate<AsyncPredicateValidator>("'{PropertyName}' não atende a condição definida.");
			Translate<RegularExpressionValidator>("'{PropertyName}' não está no formato correto.");
			Translate<EqualValidator>("'{PropertyName}' deve ser igual a '{ComparisonValue}'.");
			Translate<ExactLengthValidator>("'{PropertyName}' deve ter no máximo {MaxLength} caracteres. Você digitou {TotalLength} caracteres.");
			Translate<ExclusiveBetweenValidator>("'{PropertyName}' deve, exclusivamente, estar entre {From} e {To}. Você digitou {Value}.");
			Translate<InclusiveBetweenValidator>("'{PropertyName}' deve estar entre {From} e {To}. Você digitou {Value}.");
			Translate<CreditCardValidator>("'{PropertyName}' não é um número válido de cartão de crédito.");
			Translate<ScalePrecisionValidator>("'{PropertyName}' não pode ter mais do que {ExpectedPrecision} dígitos no total, com {ExpectedScale} dígitos decimais. {Digits} dígitos e {ActualScale} decimais foram informados.");
			Translate<EmptyValidator>("'{PropertyName}' deve estar vazio.");
			Translate<NullValidator>("'{PropertyName}' deve estar null.");
			Translate<EnumValidator>("'{PropertyName}' possui um intervalo de valores que não inclui '{PropertyValue}'.");
			// Additional fallback messages used by clientside validation integration.
			Translate("Length_Simple", "'{PropertyName}' deve ter entre {MinLength} e {MaxLength} caracteres.");
			Translate("MinimumLength_Simple", "'{PropertyName}' deve ser maior ou igual a {MinLength} caracteres.");
			Translate("MaximumLength_Simple", "'{PropertyName}' deve ser menor ou igual a {MaxLength} caracteres.");
			Translate("ExactLength_Simple", "'{PropertyName}' deve ter no máximo {MaxLength} caracteres.");
			Translate("InclusiveBetween_Simple", "'{PropertyName}' deve estar entre {From} e {To}.");
		}
	}
}
