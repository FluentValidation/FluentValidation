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

	internal class AlbanianLanguage : Language {
		public const string Culture = "sq";
		public override string Name => Culture;

		public AlbanianLanguage() {
			Translate<EmailValidator>("'{PropertyName}' nuk është një adresë e saktë emaili.");
			Translate<GreaterThanOrEqualValidator>("'{PropertyName}' duhet të jetë më e madhe se ose e barabartë me '{ComparisonValue}'.");
			Translate<GreaterThanValidator>("'{PropertyName}' duhet të jetë më e madhe se '{ComparisonValue}'.");
			Translate<LengthValidator>("'{PropertyName}' duhet të jetë midis {MinLength} dhe {MaxLength} karakteresh. Ju keni shkruar {TotalLength} karaktere.");
			Translate<MinimumLengthValidator>("Gjatësia e '{PropertyName}' duhet të jetë të paktën {MinLength} karaktere. Ju keni shkruar {TotalLength} karaktere.");
			Translate<MaximumLengthValidator>("Gjatësia e '{PropertyName}' duhet të jetë {MaxLength} karaktere ose më pak. Ju keni shkruar {TotalLength} karaktere.");
			Translate<LessThanOrEqualValidator>("'{PropertyName}'  duhet të jetë më e vogël ose e barabartë me '{ComparisonValue}'.");
			Translate<LessThanValidator>("'{PropertyName}' duhet të jetë më e vogël se '{ComparisonValue}'.");
			Translate<NotEmptyValidator>("'{PropertyName}' nuk duhet të jetë bosh.");
			Translate<NotEqualValidator>("'{PropertyName}' nuk duhet të jetë e barabartë me '{ComparisonValue}'.");
			Translate<NotNullValidator>("'{PropertyName}' nuk duhet të jetë bosh.");
			Translate<PredicateValidator>("Kushti i specifikuar nuk u arrit për '{PropertyName}'.");
			Translate<AsyncPredicateValidator>("Kushti i specifikuar nuk u arrit për '{PropertyName}'.");
			Translate<RegularExpressionValidator>("'{PropertyName}' nuk është në formatin e duhur.");
			Translate<EqualValidator>("'{PropertyName}' duhet të jetë e barabartë me '{ComparisonValue}'.");
			Translate<ExactLengthValidator>("'{PropertyName}' duhet të jetë {MaxLength} karaktere në gjatësi. Ju keni shkruar {TotalLength} karaktere.");
			Translate<InclusiveBetweenValidator>("'{PropertyName}' duhet të jetë midis {From} dhe {To}. Ju keni shkruar {Value}.");
			Translate<ExclusiveBetweenValidator>("'{PropertyName}' duhet të jetë midis {From} dhe {To} (përjashtuese). Ju keni shkruar {Value}.");
			Translate<CreditCardValidator>("'{PropertyName}' nuk është nje numër i vlefshëm karte krediti.");
			Translate<ScalePrecisionValidator>("'{PropertyName}' nuk mund të jetë më shumë se {ExpectedPrecision} shifra në total, me hapësirë për {ExpectedScale} shifra dhjetore. {Digits} shifra dhe {ActualScale} shifra dhjetore u gjetën.");
			Translate<EmptyValidator>("'{PropertyName}' nuk duhet të jetë bosh.");
			Translate<NullValidator>("'{PropertyName}' duhet të jetë bosh.");
			Translate<EnumValidator>("'{PropertyName}' ka një varg vlerash të cilat nuk përfshijnë '{PropertyValue}'.");
			// Additional fallback messages used by clientside validation integration.
			Translate("Length_Simple", "'{PropertyName}' duhet të jetë midis {MinLength} dhe {MaxLength} karakteresh.");
			Translate("MinimumLength_Simple", "Gjatësia e '{PropertyName}' duhet të jetë të paktën {MinLength} karaktere.");
			Translate("MaximumLength_Simple", "Gjatësia e '{PropertyName}' duhet të jetë {MaxLength} karaktere ose më pak.");
			Translate("ExactLength_Simple", "'{PropertyName}' duhet të jetë {MaxLength} karaktere në gjatësi.");
			Translate("InclusiveBetween_Simple", "'{PropertyName}' duhet të jetë midis {From} dhe {To}.");
		}
	}
}
