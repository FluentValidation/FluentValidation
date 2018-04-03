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

	internal class UkrainianLanguage : Language {
		public override string Name => "uk";

		public UkrainianLanguage() {
			Translate<EmailValidator>("'{PropertyName}' не є email-адресою.");
			Translate<GreaterThanOrEqualValidator>("'{PropertyName}' має бути більшим, або дорівнювати '{ComparisonValue}'.");
			Translate<GreaterThanValidator>("'{PropertyName}' має бути більшим за '{ComparisonValue}'.");
			Translate<LengthValidator>("'{PropertyName}' має бути довжиною від {MinLength} до {MaxLength} символів. Ви ввели {TotalLength} символів.");
			Translate<MinimumLengthValidator>("Довжина '{PropertyName}' має бути не меншою ніж {MinLength} символів. Ви ввели {TotalLength} символів.");
			Translate<MaximumLengthValidator>("Довжина '{PropertyName}' має бути {MaxLength} символів, або менше. Ви ввели {TotalLength} символів.");
			Translate<LessThanOrEqualValidator>("'{PropertyName}' має бути меншим, або дорівнювати '{ComparisonValue}'.");
			Translate<LessThanValidator>("'{PropertyName}' має бути меншим за '{ComparisonValue}'.");
			Translate<NotEmptyValidator>("'{PropertyName}' не може бути порожнім.");
			Translate<NotEqualValidator>("'{PropertyName}' не може дорівнювати '{ComparisonValue}'.");
			Translate<NotNullValidator>("'{PropertyName}' не може бути порожнім.");
			Translate<PredicateValidator>("Вказана умова не є задовільною для '{PropertyName}'.");
			Translate<AsyncPredicateValidator>("Вказана умова не є задовільною для '{PropertyName}'.");
			Translate<RegularExpressionValidator>("'{PropertyName}' має неправильний формат.");
			Translate<EqualValidator>("'{PropertyName}' має дорівнювати '{ComparisonValue}'.");
			Translate<ExactLengthValidator>("'{PropertyName}' має бути довжиною {MaxLength} символів. Ви ввели {TotalLength} символів.");
			Translate<InclusiveBetweenValidator>("'{PropertyName}' має бути між {From} та {To} (включно). Ви ввели {Value}.");
			Translate<ExclusiveBetweenValidator>("'{PropertyName}' має бути між {From} та {To}. Ви ввели {Value}.");
			Translate<CreditCardValidator>("'{PropertyName}' не є номером кредитної картки.");
			Translate<ScalePrecisionValidator>("'{PropertyName}' не може мати більше за {expectedPrecision} цифр всього, з {expectedScale} десятковими знаками. {digits} цифр та {actualScale} десяткових знаків знайдено.");
			Translate<EmptyValidator>("'{PropertyName}' має бути порожнім.");
			Translate<NullValidator>("'{PropertyName}' має бути порожнім.");
			Translate<EnumValidator>("'{PropertyName}' має діапазон значень, який не включає '{PropertyValue}'.");
		}
	}
}
