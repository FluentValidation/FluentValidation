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

	internal class HungarianLanguage {
		public const string Culture = "hu";

		public static string GetTranslation(string key) => key switch {
			nameof(EmailValidator) => "'{PropertyName}' nem érvényes email cím.",
			nameof(GreaterThanOrEqualValidator) => "A(z) '{PropertyName}' nagyobb vagy egyenlő kell, hogy legyen, mint '{ComparisonValue}'.",
			nameof(GreaterThanValidator) => "A(z) '{PropertyName}' nagyobb kell, hogy legyen, mint '{ComparisonValue}'.",
			nameof(LengthValidator) => "A(z) '{PropertyName}' legalább {MinLength}, de legfeljebb {MaxLength} karakter kell, hogy legyen. Ön {TotalLength} karaktert adott meg.",
			nameof(MinimumLengthValidator) => "A(z) '{PropertyName}' legalább {MinLength} karakter kell, hogy legyen. Ön {TotalLength} karaktert adott meg.",
			nameof(MaximumLengthValidator) => "A(z) '{PropertyName}' legfeljebb {MaxLength} karakter lehet csak. Ön {TotalLength} karaktert adott meg.",
			nameof(LessThanOrEqualValidator) => "A(z) '{PropertyName}' kisebb vagy egyenlő kell, hogy legyen, mint '{ComparisonValue}'.",
			nameof(LessThanValidator) => "A(z) '{PropertyName}' kisebb kell, hogy legyen, mint '{ComparisonValue}'.",
			nameof(NotEmptyValidator) => "A(z) '{PropertyName}' nem lehet üres.",
			nameof(NotEqualValidator) => "A(z) '{PropertyName}' nem lehet egyenlő ezzel: '{ComparisonValue}'.",
			nameof(NotNullValidator) => "A(z) '{PropertyName}' nem lehet üres.",
			nameof(PredicateValidator) => "A megadott feltétel nem teljesült a(z) '{PropertyName}' mezőre.",
			nameof(AsyncPredicateValidator) => "A megadott feltétel nem teljesült a(z) '{PropertyName}' mezőre.",
			nameof(RegularExpressionValidator) => "A(z) '{PropertyName}' nem a megfelelő formátumban van.",
			nameof(EqualValidator) => "A(z) '{PropertyName}' egyenlő kell, hogy legyen ezzel: '{ComparisonValue}'.",
			nameof(ExactLengthValidator) => "A(z) '{PropertyName}' pontosan {MaxLength} karakter kell, hogy legyen. Ön {TotalLength} karaktert adott meg.",
			nameof(InclusiveBetweenValidator) => "A(z) '{PropertyName}' nem lehet kisebb, mint {From} és nem lehet nagyobb, mint {To}. Ön ezt adta: {Value}.",
			nameof(ExclusiveBetweenValidator) => "A(z) '{PropertyName}' nagyobb, mint {From} és kisebb, mint {To} kell, hogy legyen. Ön ezt adta: {Value}.",
			nameof(CreditCardValidator) => "'{PropertyName}' nem érvényes bankkártyaszám.",
			nameof(ScalePrecisionValidator) => "A(z) '{PropertyName}' összesen nem lehet több {ExpectedPrecision} számjegynél, {ExpectedScale} tizedesjegy pontosság mellett. {Digits} számjegy és {ActualScale} tizedesjegy pontosság lett megadva.",
			nameof(EmptyValidator) => "A(z) '{PropertyName}' üres kell, hogy legyen.",
			nameof(NullValidator) => "A(z) '{PropertyName}' üres kell, hogy legyen.",
			nameof(EnumValidator) => "A(z) '{PropertyName}' csak olyan értékek közül választható, ami nem foglalja magába a(z) '{PropertyValue}' értéket.",
			// Additional fallback messages used by clientside validation integration.
			"Length_Simple" => "A(z) '{PropertyName}' {MinLength} és {MaxLength} karakter között kell, hogy legyen.",
			"MinimumLength_Simple" => "A(z) '{PropertyName}' hossza legalább {MinLength} karakter kell, hogy legyen.",
			"MaximumLength_Simple" => "A(z) '{PropertyName}' hossza legfeljebb {MaxLength} karakter lehet csak.",
			"ExactLength_Simple" => "A(z) '{PropertyName}' pontosan {MaxLength} karakter hosszú lehet csak.",
			"InclusiveBetween_Simple" => "A(z) '{PropertyName}' {From} és {To} között kell, hogy legyen (befoglaló).",
			_ => null,
		};
	}
}
