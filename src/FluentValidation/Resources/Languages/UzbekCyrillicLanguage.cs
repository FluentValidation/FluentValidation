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

namespace FluentValidation.Resources;

internal class UzbekCyrillicLanguage {
	public const string Culture = "uz-Cyrl-UZ";

	public static string GetTranslation(string key) => key switch {
		"EmailValidator" => "'{PropertyName}' яроқли электрон почта манзили эмас.",
		"GreaterThanOrEqualValidator" => "'{PropertyName}' камида '{ComparisonValue}'га тенг бўлиши шарт.",
		"GreaterThanValidator" => "'{PropertyName}' '{ComparisonValue}'дан катта бўлиши шарт.",
		"LengthValidator" => "'{PropertyName}' {MinLength}тадан {MaxLength}тагача белгидан иборат бўлиши шарт. Сиз {TotalLength}та белги киритдингиз.",
		"MinimumLengthValidator" => "'{PropertyName}' камида {MinLength}та белгидан иборат бўлиши шарт. Сиз {TotalLength}та белги киритдингиз.",
		"MaximumLengthValidator" => "'{PropertyName}' кўпи билан {MaxLength}та белгидан иборат бўлиши шарт. Сиз {TotalLength}та белги киритдингиз.",
		"LessThanOrEqualValidator" => "'{PropertyName}' '{ComparisonValue}'дан кам ёки тенг бўлиши шарт.",
		"LessThanValidator" => "'{PropertyName}' '{ComparisonValue}'дан кам бўлиши шарт.",
		"NotEmptyValidator" => "'{PropertyName}' бўш бўлиши мумкин эмас.",
		"NotEqualValidator" => "'{PropertyName}' '{ComparisonValue}'га тенг бўлиши мумкин эмас.",
		"NotNullValidator" => "'{PropertyName}' бўш бўлиши мумкин эмас.",
		"PredicateValidator" => "'{PropertyName}'нинг белгиланган шарти бажарилмади.",
		"AsyncPredicateValidator" => "'{PropertyName}'нинг белгиланган шарти бажарилмади.",
		"RegularExpressionValidator" => "'{PropertyName}' нотўғри форматда.",
		"EqualValidator" => "'{PropertyName}' '{ComparisonValue}'га тенг бўлиши шарт.",
		"ExactLengthValidator" => "'{PropertyName}' айнан {MaxLength}та белгидан иборат бўлиши шарт. Сиз {TotalLength}та белги киритдингиз.",
		"InclusiveBetweenValidator" => "'{PropertyName}' {From}дан {To}гача бўлиши шарт. Сиз {PropertyValue} киритдингиз.",
		"ExclusiveBetweenValidator" => "'{PropertyName}' {From}дан {To}гача бўлиши шарт (ушбу қийматлар ҳисобга олинмайди). Сиз {PropertyValue} киритдингиз.",
		"CreditCardValidator" => "'{PropertyName}' яроқли кредит карта рақами эмас.",
		"ScalePrecisionValidator" => "'{PropertyName}' бутун қисми жами {ExpectedPrecision}та рақамдан ошмаслиги шарт, жумладан рухсат этилган {ExpectedScale} хона каср(лар) аниқликда. Бутун қисмда {Digits}та рақам(лар) ва {ActualScale} хона каср(лар) топилди.",
		"EmptyValidator" => "'{PropertyName}' бўш бўлиши шарт.",
		"NullValidator" => "'{PropertyName}' бўш бўлиши шарт.",
		"EnumValidator" => "'{PropertyName}' қийматлари орасида '{PropertyValue}' йўқ.",
		// Additional fallback messages used by clientside validation integration.
		"Length_Simple" => "'{PropertyName}' {MinLength}тадан {MaxLength}тагача белгидан иборат бўлиши шарт.",
		"MinimumLength_Simple" => "'{PropertyName}' камида {MinLength}та белгидан иборат бўлиши шарт.",
		"MaximumLength_Simple" => "'{PropertyName}' кўпи билан {MaxLength}та белгидан иборат бўлиши шарт.",
		"ExactLength_Simple" => "'{PropertyName}' айнан {MaxLength}та белгидан иборат бўлиши шарт.",
		"InclusiveBetween_Simple" => "'{PropertyName}'нинг қиймати {From}дан {To}гача бўлиши шарт.",
		_ => null,
	};
}
