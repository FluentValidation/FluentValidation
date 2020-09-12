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

	internal class SpanishLanguage {
		public const string Culture = "es";

		public static string GetTranslation(string key) => key switch {
			nameof(EmailValidator) => "'{PropertyName}' no es una dirección de correo electrónico válida.",
			nameof(GreaterThanOrEqualValidator) => "'{PropertyName}' debe ser mayor o igual que '{ComparisonValue}'.",
			nameof(GreaterThanValidator) => "'{PropertyName}' debe ser mayor que '{ComparisonValue}'.",
			nameof(LengthValidator) => "'{PropertyName}' debe tener entre {MinLength} y {MaxLength} caracter(es). Actualmente tiene {TotalLength} caracter(es).",
			nameof(MinimumLengthValidator) => "'{PropertyName}' debe ser mayor o igual que {MinLength} caracteres. Ingresó {TotalLength} caracter(es).",
			nameof(MaximumLengthValidator) => "'{PropertyName}' debe ser menor o igual que {MaxLength} caracteres. Ingresó {TotalLength} caracter(es).",
			nameof(LessThanOrEqualValidator) => "'{PropertyName}' debe ser menor o igual que '{ComparisonValue}'.",
			nameof(LessThanValidator) => "'{PropertyName}' debe ser menor que '{ComparisonValue}'.",
			nameof(NotEmptyValidator) => "'{PropertyName}' no debería estar vacío.",
			nameof(NotEqualValidator) => "'{PropertyName}' no debería ser igual a '{ComparisonValue}'.",
			nameof(NotNullValidator) => "'{PropertyName}' no debe estar vacío.",
			nameof(PredicateValidator) => "'{PropertyName}' no cumple con la condición especificada.",
			nameof(AsyncPredicateValidator) => "'{PropertyName}' no cumple con la condición especificada.",
			nameof(RegularExpressionValidator) => "'{PropertyName}' no tiene el formato correcto.",
			nameof(EqualValidator) => "'{PropertyName}' debería ser igual a '{ComparisonValue}'.",
			nameof(ExactLengthValidator) => "'{PropertyName}' debe tener un largo de {MaxLength} caracteres. Actualmente tiene {TotalLength} caracter(es).",
			nameof(ExclusiveBetweenValidator) => "'{PropertyName}' debe estar entre {From} y {To} (exclusivo). Actualmente tiene {Value}.",
			nameof(InclusiveBetweenValidator) => "'{PropertyName}' debe estar entre {From} y {To}. Actualmente tiene {Value}.",
			nameof(CreditCardValidator) => "'{PropertyName}' no es un número de tarjeta de crédito válido.",
			nameof(ScalePrecisionValidator) => "'{PropertyName}' no debe tener más de {ExpectedPrecision} dígitos en total, con margen para {ExpectedScale} decimales. Se encontraron {Digits} y {ActualScale} decimales.",
			nameof(EmptyValidator) => "'{PropertyName}' debe estar vacío.",
			nameof(NullValidator) => "'{PropertyName}' debe estar vacío.",
			nameof(EnumValidator) => "'{PropertyName}' tiene un rango de valores que no incluye '{PropertyValue}'.",
			// Additional fallback messages used by clientside validation integration.
			"Length_Simple" => "'{PropertyName}' debe tener entre {MinLength} y {MaxLength} caracter(es).",
			"MinimumLength_Simple" => "'{PropertyName}' debe ser mayor o igual que {MinLength} caracteres.",
			"MaximumLength_Simple" => "'{PropertyName}' debe ser menor o igual que {MaxLength} caracteres.",
			"ExactLength_Simple" => "'{PropertyName}' debe tener un largo de {MaxLength} caracteres.",
			"InclusiveBetween_Simple" => "'{PropertyName}' debe estar entre {From} y {To}.",
			_ => null,
		};
	}
}
