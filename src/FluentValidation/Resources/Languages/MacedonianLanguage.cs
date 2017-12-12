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

	internal class MacedonianLanguage : Language {
		public override string Name => "mk";

		public MacedonianLanguage() {
			Translate<EmailValidator>("'{PropertyName}' не е валидна емаил адреса.");
			Translate<GreaterThanOrEqualValidator>("Вредноста на '{PropertyName}' мора да биде поголема или еднаква на '{ComparisonValue}'.");
			Translate<GreaterThanValidator>("Вредноста на '{PropertyName}' мора да биде поголема од '{ComparisonValue}'.");
			Translate<LengthValidator>("Должината на '{PropertyName}' мора да биде помеѓу {MinLength} и {MaxLength} карактери. Имате внесено вкупно {TotalLength} карактери.");
			Translate<MinimumLengthValidator>("Должината на '{PropertyName}' мора да биде поголема или еднаква на {MinLength} знаци. Внесовте {TotalLength} знаци.");
			Translate<MaximumLengthValidator>("Должината на '{PropertyName}' мора да биде помала или еднаква на {MaxLength} знаци. Внесовте {TotalLength} знаци.");
			Translate<LessThanOrEqualValidator>("Вредноста на '{PropertyName}' мора да биде помала или еднаква на '{ComparisonValue}'.");
			Translate<LessThanValidator>("Вредноста на '{PropertyName}' мора да биде помала од '{ComparisonValue}'.");
			Translate<NotEmptyValidator>("Вредноста на '{PropertyName}' не треба да биде празна.");
			Translate<NotEqualValidator>("Вредноста на '{PropertyName}' би требало да биде еднаква на '{ComparisonValue}'.");
			Translate<NotNullValidator>("Вредноста на '{PropertyName}' не треба да биде празна.");
			Translate<PredicateValidator>("Специфичната состојба не беше најдена за  '{PropertyName}'.");
			Translate<AsyncPredicateValidator>("Специфичната состојба не беше најдена за  '{PropertyName}'.");
			Translate<RegularExpressionValidator>("'{PropertyName}' не е во правилниот формат.");
			Translate<EqualValidator>("Вредноста на '{PropertyName}' би требало да биде еднаква на '{ComparisonValue}'.");
			Translate<ExactLengthValidator>("Должината на '{PropertyName}' мора да биде {MaxLength} карактери. Имате внесено вкупно {TotalLength} карактери.");
			Translate<InclusiveBetweenValidator>("Вредноста на '{PropertyName}' мора да биде помеѓу {From} и {To}. Имате внесено {Value}.");
			Translate<ExclusiveBetweenValidator>("Вредноста на '{PropertyName}' мора да биде од {From} до {To} (исклучително). Имате внесено вредност {Value}.");
			Translate<CreditCardValidator>("'{PropertyName}' не е валиден бројот на кредитната картичка.");
			Translate<ScalePrecisionValidator>("'{PropertyName}' не би требало да биде повеќе од  {expectedPrecision} цифри вкупно, со дозволени  {expectedScale} децимали. {digits} цифри и {actualScale} децимали беа најдени.");
			Translate<EmptyValidator>("'{PropertyName}' треба да биде празна.");
			Translate<NullValidator>("'{PropertyName}' треба да биде празна.");
			Translate<EnumValidator>("'{PropertyName}' има низа вредности кои не вклучуваат '{PropertyValue}'.");

		}
	}
}