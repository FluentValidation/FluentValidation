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

	internal class HungarianLanguage : Language {
		public const string Culture = "hu";
		public override string Name => Culture;

		public HungarianLanguage() {
			Translate<EmailValidator>("'{PropertyName}' nem érvényes email cím.");
			Translate<GreaterThanOrEqualValidator>("A(z) '{PropertyName}' nagyobb vagy egyenlő kell, hogy legyen, mint '{ComparisonValue}'.");
			Translate<GreaterThanValidator>("A(z) '{PropertyName}' nagyobb kell, hogy legyen, mint '{ComparisonValue}'.");
			Translate<LengthValidator>("A(z) '{PropertyName}' legalább {MinLength}, de legfeljebb {MaxLength} karakter kell, hogy legyen. Ön {TotalLength} karaktert adott meg.");
			Translate<MinimumLengthValidator>("A(z) '{PropertyName}' legalább {MinLength} karakter kell, hogy legyen. Ön {TotalLength} karaktert adott meg.");
			Translate<MaximumLengthValidator>("A(z) '{PropertyName}' legfeljebb {MaxLength} karakter lehet csak. Ön {TotalLength} karaktert adott meg.");
			Translate<LessThanOrEqualValidator>("A(z) '{PropertyName}' kisebb vagy egyenlő kell, hogy legyen, mint '{ComparisonValue}'.");
			Translate<LessThanValidator>("A(z) '{PropertyName}' kisebb kell, hogy legyen, mint '{ComparisonValue}'.");
			Translate<NotEmptyValidator>("A(z) '{PropertyName}' nem lehet üres.");
			Translate<NotEqualValidator>("A(z) '{PropertyName}' nem lehet egyenlő ezzel: '{ComparisonValue}'.");
			Translate<NotNullValidator>("A(z) '{PropertyName}' nem lehet üres.");
			Translate<PredicateValidator>("A megadott feltétel nem teljesült a(z) '{PropertyName}' mezőre.");
			Translate<AsyncPredicateValidator>("A megadott feltétel nem teljesült a(z) '{PropertyName}' mezőre.");
			Translate<RegularExpressionValidator>("A(z) '{PropertyName}' nem a megfelelő formátumban van.");
			Translate<EqualValidator>("A(z) '{PropertyName}' egyenlő kell, hogy legyen ezzel: '{ComparisonValue}'.");
			Translate<ExactLengthValidator>("A(z) '{PropertyName}' pontosan {MaxLength} karakter kell, hogy legyen. Ön {TotalLength} karaktert adott meg.");
			Translate<InclusiveBetweenValidator>("A(z) '{PropertyName}' nem lehet kisebb, mint {From} és nem lehet nagyobb, mint {To}. Ön ezt adta: {Value}.");
			Translate<ExclusiveBetweenValidator>("A(z) '{PropertyName}' nagyobb, mint {From} és kisebb, mint {To} kell, hogy legyen. Ön ezt adta: {Value}.");
			Translate<CreditCardValidator>("'{PropertyName}' nem érvényes bankkártyaszám.");
			Translate<ScalePrecisionValidator>("A(z) '{PropertyName}' összesen nem lehet több {ExpectedPrecision} számjegynél, {ExpectedScale} tizedesjegy pontosság mellett. {Digits} számjegy és {ActualScale} tizedesjegy pontosság lett megadva.");
			Translate<EmptyValidator>("A(z) '{PropertyName}' üres kell, hogy legyen.");
			Translate<NullValidator>("A(z) '{PropertyName}' üres kell, hogy legyen.");
			Translate<EnumValidator>("A(z) '{PropertyName}' csak olyan értékek közül választható, ami nem foglalja magába a(z) '{PropertyValue}' értéket.");
			// Additional fallback messages used by clientside validation integration.
			Translate("Length_Simple", "A(z) '{PropertyName}' {MinLength} és {MaxLength} karakter között kell, hogy legyen.");
			Translate("MinimumLength_Simple", "A(z) '{PropertyName}' hossza legalább {MinLength} karakter kell, hogy legyen.");
			Translate("MaximumLength_Simple", "A(z) '{PropertyName}' hossza legfeljebb {MaxLength} karakter lehet csak.");
			Translate("ExactLength_Simple", "A(z) '{PropertyName}' pontosan {MaxLength} karakter hosszú lehet csak.");
			Translate("InclusiveBetween_Simple", "A(z) '{PropertyName}' {From} és {To} között kell, hogy legyen (befoglaló).");
		}
	}
}
