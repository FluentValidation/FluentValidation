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

	internal class FrenchLanguage : Language {
		public override string Name => "fr";

		public FrenchLanguage() {
			Translate<EmailValidator>("'{PropertyName}' n'est pas une adresse email valide.");
			Translate<GreaterThanOrEqualValidator>("'{PropertyName}' doit être plus grand ou égal à '{ComparisonValue}'.");
			Translate<GreaterThanValidator>("'{PropertyName}' doit être plus grand que '{ComparisonValue}'.");
			Translate<LengthValidator>("'{PropertyName}' doit contenir entre {MinLength} et {MaxLength} caractères. {TotalLength} caractères ont été saisis.");
			Translate<MinimumLengthValidator>("'{PropertyName}' doit être supérieur ou égal à {MinLength} caractères. Vous avez saisi {TotalLength} caractères.");
			Translate<MaximumLengthValidator>("'{PropertyName}' doit être inférieur ou égal à {MaxLength} caractères. Vous avez saisi {TotalLength} caractères.");
			Translate<LessThanOrEqualValidator>("'{PropertyName}' doit être plus petit ou égal à '{ComparisonValue}'.");
			Translate<LessThanValidator>("'{PropertyName}' doit être plus petit que '{ComparisonValue}'.");
			Translate<NotEmptyValidator>("'{PropertyName}' ne doit pas être vide.");
			Translate<NotEqualValidator>("'{PropertyName}' ne doit pas être égal à '{ComparisonValue}'.");
			Translate<NotNullValidator>("'{PropertyName}' ne doit pas avoir la valeur null.");
			Translate<PredicateValidator>("'{PropertyName}' ne respecte pas la condition fixée.");
			Translate<AsyncPredicateValidator>("'{PropertyName}' ne respecte pas la condition fixée.");
			Translate<RegularExpressionValidator>("'{PropertyName}' n'a pas le bon format.");
			Translate<EqualValidator>("'{PropertyName}' doit être égal à '{ComparisonValue}'.");
			Translate<ExactLengthValidator>("'{PropertyName}' doit être d’une longueur de {MaxLength} caractères. {TotalLength} caractères ont été saisis.");
			Translate<ExclusiveBetweenValidator>("'{PropertyName}' doit être entre {From} et {To} (exclusif). Vous avez saisi {Value}.");
			Translate<InclusiveBetweenValidator>("'{PropertyName}' doit être entre {From} et {To}. Vous avez saisi {Value}.");
			Translate<CreditCardValidator>("'{PropertyName}' n'est pas un numéro de carte de crédit valide.");
			Translate<ScalePrecisionValidator>("'{PropertyName}' ne doit pas dépasser {expectedPrecision} chiffres au total, avec une tolérance pour les décimales {expectedScale}. Les chiffres {digits} et les décimales {actualScale} ont été trouvés.");
			Translate<EmptyValidator>("'{PropertyName}' devrait être vide.");
			Translate<NullValidator>("'{PropertyName}' devrait être vide.");
			Translate<EnumValidator>("'{PropertyName}' a une plage de valeurs qui n'inclut pas '{PropertyValue}'.");
		}
	}
}
