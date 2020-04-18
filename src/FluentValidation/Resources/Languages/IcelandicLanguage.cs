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

	internal class IcelandicLanguage : Language {
		public const string Culture = "is";
		public override string Name => Culture;

		public IcelandicLanguage() {
			Translate<EmailValidator>("'{PropertyName}' er ekki gilt netfang.");
			Translate<GreaterThanOrEqualValidator>("'{PropertyName}' verður að vera meiri en eða jöfn '{ComparisonValue}'.");
			Translate<GreaterThanValidator>("'{PropertyName}' verður að vera meiri en '{ComparisonValue}'.");
			Translate<LengthValidator>("'{PropertyName}' verður að vera á milli {MinLength} og {MaxLength} stafir. Þú slóst inn {TotalLength} stafi.");
			Translate<MinimumLengthValidator>("Lengdin '{PropertyName}' verður að vera að minnsta kosti {MinLength} stafir. Þú slóst inn {TotalLength} stafi.");
			Translate<MaximumLengthValidator>("Lengd '{PropertyName}' verður að vera {MaxLength} stafir eða færri. Þú slóst inn {TotalLength} stafi.");
			Translate<LessThanOrEqualValidator>("'{PropertyName}' verður að vera minna en eða jafnt og '{ComparisonValue}'.");
			Translate<LessThanValidator>("'{PropertyName}' verður að vera minna en '{ComparisonValue}'.");
			Translate<NotEmptyValidator>("'{PropertyName} má ekki vera tómt.");
			Translate<NotEqualValidator>("'{PropertyName}' má ekki vera jafnt og '{ComparisonValue}'.");
			Translate<NotNullValidator>("'{PropertyName} má ekki vera tómt.");
			Translate<PredicateValidator>("Tilgreindu skilyrði var ekki uppfyllt fyrir '{PropertyName}'.");
			Translate<AsyncPredicateValidator>("Tilgreindu skilyrði var ekki uppfyllt fyrir '{PropertyName}'.");
			Translate<RegularExpressionValidator>("'{PropertyName}' er ekki með réttu sniði.");
			Translate<EqualValidator>("'{PropertyName}' verður að vera jafnt og '{ComparisonValue}'.");
			Translate<ExactLengthValidator>("'{PropertyName}' verður að vera {MaxLength} stafir að lengd. Þú slóst inn {TotalLength} stafi.");
			Translate<InclusiveBetweenValidator>("'{PropertyName}' verður að frá {From} til {To}. Þú slóst inn {Value}.");
			Translate<ExclusiveBetweenValidator>("'{PropertyName}' verður að vera á milli {From} og {To}. Þú slóst inn {Value}.");
			Translate<CreditCardValidator>("'{PropertyName}' er ekki gilt kreditkortanúmer.");
			Translate<ScalePrecisionValidator>("'{PropertyName}' má ekki vera meira en {ExpectedPrecision} tölustafir samtals, með heimild fyrir {ExpectedScale} aukastöfum. {Digits} tölustafir og {ActualScale} aukastafir fundust.");
			Translate<EmptyValidator>("'{PropertyName}' verður að vera tómt.");
			Translate<NullValidator>("'{PropertyName}' verður að vera tómt.");
			Translate<EnumValidator>("'{PropertyName}' hefur svið gilda sem innihalda ekki '{PropertyValue}'.");
			// Additional fallback messages used by clientside validation integration.
			Translate("Length_Simple", "'{PropertyName}' verður að vera á milli {MinLength} og {MaxLength} stafir.");
			Translate("MinimumLength_Simple", "Lengdin '{PropertyName}' verður að vera að minnsta kosti {MinLength} stafir.");
			Translate("MaximumLength_Simple", "Lengd '{PropertyName}' verður að vera {MaxLength} stafir eða færri.");
			Translate("ExactLength_Simple", "'{PropertyName}' verður að vera {MaxLength} stafir að lengd.");
			Translate("InclusiveBetween_Simple", "'{PropertyName}' verður að vera á milli {From} og {To}.");
		}
	}
}


