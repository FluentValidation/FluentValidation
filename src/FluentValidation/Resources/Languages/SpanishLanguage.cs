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

	internal class SpanishLanguage : Language {
		public override string Name => "es";

		public SpanishLanguage() {
			Translate<EmailValidator>("'{PropertyName}' no es una dirección de correo electrónico válida.");
			Translate<GreaterThanOrEqualValidator>("'{PropertyName}' debe ser mayor o igual que '{ComparisonValue}'.");
			Translate<GreaterThanValidator>("'{PropertyName}' debe ser mayor que '{ComparisonValue}'.");
			Translate<LengthValidator>("'{PropertyName}' debe tener entre {MinLength} y {MaxLength} caracter(es). Actualmente tiene {TotalLength} caracter(es).");
			Translate<MinimumLengthValidator>("'{PropertyName}' debe ser mayor o igual que {MinLength} caracteres. Ingresó {TotalLength} caracter(es).");
			Translate<MaximumLengthValidator>("'{PropertyName}' debe ser menor o igual que {MaxLength} caracteres. Ingresó {TotalLength} caracter(es).");
			Translate<LessThanOrEqualValidator>("'{PropertyName}' debe ser menor o igual que '{ComparisonValue}'.");
			Translate<LessThanValidator>("'{PropertyName}' debe ser menor que '{ComparisonValue}'.");
			Translate<NotEmptyValidator>("'{PropertyName}' no debería estar vacío.");
			Translate<NotEqualValidator>("'{PropertyName}' no debería ser igual a '{ComparisonValue}'.");
			Translate<NotNullValidator>("'{PropertyName}' no debe estar vacío.");
			Translate<PredicateValidator>("'{PropertyName}' no cumple con la condición especificada.");
			Translate<AsyncPredicateValidator>("'{PropertyName}' no cumple con la condición especificada.");
			Translate<RegularExpressionValidator>("'{PropertyName}' no tiene el formato correcto.");
			Translate<EqualValidator>("'{PropertyName}' debería ser igual a '{ComparisonValue}'.");
			Translate<ExactLengthValidator>("'{PropertyName}' debe tener un largo de {MaxLength} caracteres. Actualmente tiene {TotalLength} caracter(es).");
			Translate<ExclusiveBetweenValidator>("'{PropertyName}' debe estar entre {From} y {To} (exclusivo). Actualmente tiene {Value}.");
			Translate<InclusiveBetweenValidator>("'{PropertyName}' debe estar entre {From} y {To}. Actualmente tiene {Value}.");
			Translate<CreditCardValidator>("'{PropertyName}' no es un número de tarjeta de crédito válido.");
			Translate<ScalePrecisionValidator>("'{PropertyName}' no debe tener más de {expectedPrecision} dígitos en total, con margen para {expectedScale} decimales. Se encontraron {digits} y {actualScale} decimales.");
			Translate<EmptyValidator>("'{PropertyName}' debe estar vacío.");
			Translate<NullValidator>("'{PropertyName}' debe estar vacío.");
			Translate<EnumValidator>("'{PropertyName}' tiene un rango de valores que no incluye '{PropertyValue}'.");
		}
	}
}
