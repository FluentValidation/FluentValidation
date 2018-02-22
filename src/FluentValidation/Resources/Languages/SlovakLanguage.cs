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

	internal class SlovakLanguage : Language {
		public override string Name => "sk";

		public SlovakLanguage() {
			Translate<EmailValidator>("Pole '{PropertyName}' musí obsahovať platnú emailovú adresu.");
			Translate<GreaterThanOrEqualValidator>("Hodnota poľa '{PropertyName}' musí byť väčšia alebo sa rovnať '{ComparisonValue}'.");
			Translate<GreaterThanValidator>("Hodnota poľa '{PropertyName}' musí byť väčšia ako '{ComparisonValue}'.");
			Translate<LengthValidator>("Dĺžka poľa '{PropertyName}' musí byť medzi {MinLength} a {MaxLength} znakmi. Vami zadaná dĺžka je {TotalLength} znakov.");
			Translate<MinimumLengthValidator>("Dĺžka poľa '{PropertyName}' musí byť väčšia alebo rovná {MinLength} znakom. Vami zadaná dĺžka je {TotalLength} znakov.");
			Translate<MaximumLengthValidator>("Dĺžka poľa '{PropertyName}' musí byť menšia alebo rovná {MaxLength} znakom. Vami zadaná dĺžka je {TotalLength} znakov.");
			Translate<LessThanOrEqualValidator>("Hodnota poľa '{PropertyName}' musí byť menšia alebo sa rovnať '{ComparisonValue}'.");
			Translate<LessThanValidator>("Hodnota poľa '{PropertyName}' musí byť menšia ako '{ComparisonValue}'.");
			Translate<NotEmptyValidator>("Pole '{PropertyName}' nesmie byť prázdné.");
			Translate<NotEqualValidator>("Pole '{PropertyName}' sa nesmie rovnať '{ComparisonValue}'.");
			Translate<NotNullValidator>("Pole '{PropertyName}' nesmie byť prázdné.");
			Translate<PredicateValidator>("Nebola splnená podmienka pre pole '{PropertyName}'.");
			Translate<AsyncPredicateValidator>("Nebola splnená podmienka pre pole '{PropertyName}'.");
			Translate<RegularExpressionValidator>("Pole '{PropertyName}' nemá správný formát.");
			Translate<EqualValidator>("Hodnota poľa '{PropertyName}' musí byť rovná '{ComparisonValue}'.");
			Translate<ExactLengthValidator>("Dĺžka poľa '{PropertyName}' musí byť {MaxLength} znakov. Vami zadaná dĺžka je {TotalLength} znakov.");
			Translate<InclusiveBetweenValidator>("Hodnota poľa '{PropertyName}' musí byť medzi {From} a {To} (vrátane). Vami zadaná hodnota je {Value}.");
			Translate<ExclusiveBetweenValidator>("Hodnota poľa '{PropertyName}' musí byť väčšia ako {From} a menšia ako {To}. Vami zadaná hodnota {Value}.");
			Translate<CreditCardValidator>("Pole '{PropertyName}' nie je správné číslo kreditnej karty.");
			Translate<ScalePrecisionValidator>("Pole '{PropertyName}' nemôže mať viac  ako {expectedPrecision} čísiel a {expectedScale} desatinných miest. Vami bolo zadané {digits} číslic a {actualScale} desatinných miest.");
			Translate<EmptyValidator>("Pole '{PropertyName}' musí byť prázdne.");
			Translate<NullValidator>("Pole '{PropertyName}' musí byť prázdne.");
			Translate<EnumValidator>("Pole '{PropertyName}' má rozsah hodnôt, ktoré neobsahujú '{PropertyValue}'.");
		}
	}
}
