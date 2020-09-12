#region License

// Copyright (c) .NET Foundation and contributors.
//
// Licensed under the Apache License, Version 2.0 (the "License",
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

	internal class SlovakLanguage {
		public const string Culture = "sk";

		public static string GetTranslation(string key) => key switch {
			nameof(EmailValidator) => "Pole '{PropertyName}' musí obsahovať platnú emailovú adresu.",
			nameof(GreaterThanOrEqualValidator) => "Hodnota poľa '{PropertyName}' musí byť väčšia alebo sa rovnať '{ComparisonValue}'.",
			nameof(GreaterThanValidator) => "Hodnota poľa '{PropertyName}' musí byť väčšia ako '{ComparisonValue}'.",
			nameof(LengthValidator) => "Dĺžka poľa '{PropertyName}' musí byť medzi {MinLength} a {MaxLength} znakmi. Vami zadaná dĺžka je {TotalLength} znakov.",
			nameof(MinimumLengthValidator) => "Dĺžka poľa '{PropertyName}' musí byť väčšia alebo rovná {MinLength} znakom. Vami zadaná dĺžka je {TotalLength} znakov.",
			nameof(MaximumLengthValidator) => "Dĺžka poľa '{PropertyName}' musí byť menšia alebo rovná {MaxLength} znakom. Vami zadaná dĺžka je {TotalLength} znakov.",
			nameof(LessThanOrEqualValidator) => "Hodnota poľa '{PropertyName}' musí byť menšia alebo sa rovnať '{ComparisonValue}'.",
			nameof(LessThanValidator) => "Hodnota poľa '{PropertyName}' musí byť menšia ako '{ComparisonValue}'.",
			nameof(NotEmptyValidator) => "Pole '{PropertyName}' nesmie byť prázdne.",
			nameof(NotEqualValidator) => "Pole '{PropertyName}' sa nesmie rovnať '{ComparisonValue}'.",
			nameof(NotNullValidator) => "Pole '{PropertyName}' nesmie byť prázdne.",
			nameof(PredicateValidator) => "Nebola splnená podmienka pre pole '{PropertyName}'.",
			nameof(AsyncPredicateValidator) => "Nebola splnená podmienka pre pole '{PropertyName}'.",
			nameof(RegularExpressionValidator) => "Pole '{PropertyName}' nemá správný formát.",
			nameof(EqualValidator) => "Hodnota poľa '{PropertyName}' musí byť rovná '{ComparisonValue}'.",
			nameof(ExactLengthValidator) => "Dĺžka poľa '{PropertyName}' musí byť {MaxLength} znakov. Vami zadaná dĺžka je {TotalLength} znakov.",
			nameof(InclusiveBetweenValidator) => "Hodnota poľa '{PropertyName}' musí byť medzi {From} a {To} (vrátane). Vami zadaná hodnota je {Value}.",
			nameof(ExclusiveBetweenValidator) => "Hodnota poľa '{PropertyName}' musí byť väčšia ako {From} a menšia ako {To}. Vami zadaná hodnota {Value}.",
			nameof(CreditCardValidator) => "Pole '{PropertyName}' nie je správné číslo kreditnej karty.",
			nameof(ScalePrecisionValidator) => "Pole '{PropertyName}' nemôže mať viac  ako {ExpectedPrecision} čísiel a {ExpectedScale} desatinných miest. Vami bolo zadané {Digits} číslic a {ActualScale} desatinných miest.",
			nameof(EmptyValidator) => "Pole '{PropertyName}' musí byť prázdne.",
			nameof(NullValidator) => "Pole '{PropertyName}' musí byť prázdne.",
			nameof(EnumValidator) => "Pole '{PropertyName}' má rozsah hodnôt, ktoré neobsahujú '{PropertyValue}'.",
			// Additional fallback messages used by clientside validation integration.
			"Length_Simple" => "Dĺžka poľa '{PropertyName}' musí byť medzi {MinLength} a {MaxLength} znakmi.",
			"MinimumLength_Simple" => "Dĺžka poľa '{PropertyName}' musí byť väčšia alebo rovná {MinLength} znakom.",
			"MaximumLength_Simple" => "Dĺžka poľa '{PropertyName}' musí byť menšia alebo rovná {MaxLength} znakom.",
			"ExactLength_Simple" => "Dĺžka poľa '{PropertyName}' musí byť {MaxLength} znakov. ",
			"InclusiveBetween_Simple" => "Hodnota poľa '{PropertyName}' musí byť medzi {From} a {To} (vrátane).",
			_ => null,
		};
	}
}
