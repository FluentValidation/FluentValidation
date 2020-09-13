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

	internal class AlbanianLanguage {
		public const string Culture = "sq";

		public static string GetTranslation(string key) => key switch {
			nameof(EmailValidator) => "'{PropertyName}' nuk është një adresë e saktë emaili.",
			nameof(GreaterThanOrEqualValidator) => "'{PropertyName}' duhet të jetë më e madhe se ose e barabartë me '{ComparisonValue}'.",
			nameof(GreaterThanValidator) => "'{PropertyName}' duhet të jetë më e madhe se '{ComparisonValue}'.",
			nameof(LengthValidator) => "'{PropertyName}' duhet të jetë midis {MinLength} dhe {MaxLength} karakteresh. Ju keni shkruar {TotalLength} karaktere.",
			nameof(MinimumLengthValidator) => "Gjatësia e '{PropertyName}' duhet të jetë të paktën {MinLength} karaktere. Ju keni shkruar {TotalLength} karaktere.",
			nameof(MaximumLengthValidator) => "Gjatësia e '{PropertyName}' duhet të jetë {MaxLength} karaktere ose më pak. Ju keni shkruar {TotalLength} karaktere.",
			nameof(LessThanOrEqualValidator) => "'{PropertyName}'  duhet të jetë më e vogël ose e barabartë me '{ComparisonValue}'.",
			nameof(LessThanValidator) => "'{PropertyName}' duhet të jetë më e vogël se '{ComparisonValue}'.",
			nameof(NotEmptyValidator) => "'{PropertyName}' nuk duhet të jetë bosh.",
			nameof(NotEqualValidator) => "'{PropertyName}' nuk duhet të jetë e barabartë me '{ComparisonValue}'.",
			nameof(NotNullValidator) => "'{PropertyName}' nuk duhet të jetë bosh.",
			nameof(PredicateValidator) => "Kushti i specifikuar nuk u arrit për '{PropertyName}'.",
			nameof(AsyncPredicateValidator) => "Kushti i specifikuar nuk u arrit për '{PropertyName}'.",
			nameof(RegularExpressionValidator) => "'{PropertyName}' nuk është në formatin e duhur.",
			nameof(EqualValidator) => "'{PropertyName}' duhet të jetë e barabartë me '{ComparisonValue}'.",
			nameof(ExactLengthValidator) => "'{PropertyName}' duhet të jetë {MaxLength} karaktere në gjatësi. Ju keni shkruar {TotalLength} karaktere.",
			nameof(InclusiveBetweenValidator) => "'{PropertyName}' duhet të jetë midis {From} dhe {To}. Ju keni shkruar {Value}.",
			nameof(ExclusiveBetweenValidator) => "'{PropertyName}' duhet të jetë midis {From} dhe {To} (përjashtuese). Ju keni shkruar {Value}.",
			nameof(CreditCardValidator) => "'{PropertyName}' nuk është nje numër i vlefshëm karte krediti.",
			nameof(ScalePrecisionValidator) => "'{PropertyName}' nuk mund të jetë më shumë se {ExpectedPrecision} shifra në total, me hapësirë për {ExpectedScale} shifra dhjetore. {Digits} shifra dhe {ActualScale} shifra dhjetore u gjetën.",
			nameof(EmptyValidator) => "'{PropertyName}' nuk duhet të jetë bosh.",
			nameof(NullValidator) => "'{PropertyName}' duhet të jetë bosh.",
			nameof(EnumValidator) => "'{PropertyName}' ka një varg vlerash të cilat nuk përfshijnë '{PropertyValue}'.",
			// Additional fallback messages used by clientside validation integration.
			"Length_Simple" => "'{PropertyName}' duhet të jetë midis {MinLength} dhe {MaxLength} karakteresh.",
			"MinimumLength_Simple" => "Gjatësia e '{PropertyName}' duhet të jetë të paktën {MinLength} karaktere.",
			"MaximumLength_Simple" => "Gjatësia e '{PropertyName}' duhet të jetë {MaxLength} karaktere ose më pak.",
			"ExactLength_Simple" => "'{PropertyName}' duhet të jetë {MaxLength} karaktere në gjatësi.",
			"InclusiveBetween_Simple" => "'{PropertyName}' duhet të jetë midis {From} dhe {To}.",
			_ => null,
		};
	}
}
