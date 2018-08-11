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

	internal class PortugueseBrazilLanguage : Language {
		public override string Name => "pt-BR";

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
			Translate<ScalePrecisionValidator>("'{PropertyName}' não pode ter mais do que {expectedPrecision} dígitos no total, com {expectedScale} dígitos decimais. {digits} dígitos e {actualScale} decimais foram informados.");
			Translate<EmptyValidator>("'{PropertyName}' deve estar vazio.");
			Translate<NullValidator>("'{PropertyName}' deve estar null.");
			Translate<EnumValidator>("'{PropertyName}' possui um intervalo de valores que não inclui '{PropertyValue}'.");
		}
	}
}