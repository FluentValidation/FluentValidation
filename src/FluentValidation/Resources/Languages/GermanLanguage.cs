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

	internal class GermanLanguage : Language {
		public override string Name => "de";

		public GermanLanguage() {
			Translate<EmailValidator>("'{PropertyName}' ist keine gültige E-Mail-Adresse.");
			Translate<GreaterThanOrEqualValidator>("Der Wert von '{PropertyName}' muss grösser oder gleich '{ComparisonValue}' sein.");
			Translate<GreaterThanValidator>("Der Wert von '{PropertyName}' muss grösser sein als '{ComparisonValue}'.");
			Translate<LengthValidator>("Die Länge von '{PropertyName}' muss zwischen {MinLength} und {MaxLength} Zeichen liegen. Es wurden {TotalLength} Zeichen eingetragen.");
			Translate<MinimumLengthValidator>("Die Länge von '{PropertyName}' muss grösser als {MinLength} Zeichen liegen. Es wurden {TotalLength} Zeichen eingetragen.");
			Translate<MaximumLengthValidator>("Die Länge von '{PropertyName}' muss kleiner als {MaxLength} Zeichen liegen. Es wurden {TotalLength} Zeichen eingetragen.");
			Translate<LessThanOrEqualValidator>("Der Wert von '{PropertyName}' muss kleiner oder gleich '{ComparisonValue}' sein.");
			Translate<LessThanValidator>("Der Wert von '{PropertyName}' muss kleiner sein als '{ComparisonValue}'.");
			Translate<NotEmptyValidator>("'{PropertyName}' darf nicht leer sein.");
			Translate<NotEqualValidator>("'{PropertyName}' darf nicht '{ComparisonValue}' sein.");
			Translate<NotNullValidator>("'{PropertyName}' darf keinen Null-Wert aufweisen.");
			Translate<PredicateValidator>("Der Wert von '{PropertyName}' entspricht nicht der festgelegten Bedingung.");
			Translate<AsyncPredicateValidator>("Der Wert von '{PropertyName}' entspricht nicht der festgelegten Bedingung.");
			Translate<RegularExpressionValidator>("'{PropertyName}' weist ein ungültiges Format auf.");
			Translate<EqualValidator>("'{PropertyName}' muss gleich '{ComparisonValue}' sein.");
			Translate<ExactLengthValidator>("'{PropertyName}' muss genau {MaxLength} lang sein. Es wurden {TotalLength} eingegeben.");
			Translate<ExclusiveBetweenValidator>("'{PropertyName}' muss zwischen {From} und {To} sein (exklusiv). Es wurde {Value} eingegeben.");
			Translate<InclusiveBetweenValidator>("'{PropertyName}' muss zwischen {From} and {To} sein. Es wurde {Value} eingegeben.");

		}
	}
}