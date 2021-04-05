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

	internal class CzechLanguage {
		public const string Culture = "cs";

		public static string GetTranslation(string key) => key switch {
			"EmailValidator" => "Pole '{PropertyName}' musí obsahovat platnou emailovou adresu.",
			"GreaterThanOrEqualValidator" => "Hodnota pole '{PropertyName}' musí být větší nebo rovna '{ComparisonValue}'.",
			"GreaterThanValidator" => "Hodnota pole '{PropertyName}' musí být větší než '{ComparisonValue}'.",
			"LengthValidator" => "Délka pole '{PropertyName}' musí být v rozsahu {MinLength} až {MaxLength} znaků. Vámi zadaná délka je {TotalLength} znaků.",
			"MinimumLengthValidator" => "Délka pole '{PropertyName}' musí být větší nebo rovna {MinLength} znakům. Vámi zadaná délka je {TotalLength} znaků.",
			"MaximumLengthValidator" => "Délka pole '{PropertyName}' musí být menší nebo rovna {MaxLength} znakům. Vámi zadaná délka je {TotalLength} znaků.",
			"LessThanOrEqualValidator" => "Hodnota pole '{PropertyName}' musí být menší nebo rovna '{ComparisonValue}'.",
			"LessThanValidator" => "Hodnota pole '{PropertyName}' musí být menší než '{ComparisonValue}'.",
			"NotEmptyValidator" => "Pole '{PropertyName}' nesmí být prázdné.",
			"NotEqualValidator" => "Pole '{PropertyName}' nesmí být rovno '{ComparisonValue}'.",
			"NotNullValidator" => "Pole '{PropertyName}' nesmí být prázdné.",
			"PredicateValidator" => "Nebyla splněna podmínka pro pole '{PropertyName}'.",
			"AsyncPredicateValidator" => "Nebyla splněna podmínka pro pole '{PropertyName}'.",
			"RegularExpressionValidator" => "Pole '{PropertyName}' nemá správný formát.",
			"EqualValidator" => "Hodnota pole '{PropertyName}' musí být rovna '{ComparisonValue}'.",
			"ExactLengthValidator" => "Délka pole '{PropertyName}' musí být {MaxLength} znaků. Vámi zadaná délka je {TotalLength} znaků.",
			"InclusiveBetweenValidator" => "Hodnota pole '{PropertyName}' musí být mezi {From} a {To} (včetně). Vámi zadaná hodnota je {PropertyValue}.",
			"ExclusiveBetweenValidator" => "Hodnota pole '{PropertyName}' musí být větší než {From} a menší než {To}. Vámi zadaná hodnota je {PropertyValue}.",
			"CreditCardValidator" => "Pole '{PropertyName}' musí obsahovat platné číslo platební karty.",
			"ScalePrecisionValidator" => "Pole '{PropertyName}' nesmí mít víc než {ExpectedPrecision} číslic a {ExpectedScale} desetinných míst. Vámi bylo zadáno {Digits} číslic a {ActualScale} desetinných míst.",
			"EmptyValidator" => "Pole '{PropertyName}' musí být prázdné.",
			"NullValidator" => "Pole '{PropertyName}' musí být prázdné.",
			"EnumValidator" => "Pole '{PropertyName}' má rozsah hodnot, které neobsahují '{PropertyValue}'.",
			// Additional fallback messages used by clientside validation integration.
			"Length_Simple" => "Délka pole '{PropertyName}' musí být v rozsahu {MinLength} až {MaxLength} znaků.",
			"MinimumLength_Simple" => "Délka pole '{PropertyName}' musí být větší nebo rovna {MinLength} znakům.",
			"MaximumLength_Simple" => "Délka pole '{PropertyName}' musí být menší nebo rovna {MaxLength} znakům.",
			"ExactLength_Simple" => "Délka pole '{PropertyName}' musí být {MaxLength} znaků.",
			"InclusiveBetween_Simple" => "Hodnota pole '{PropertyName}' musí být mezi {From} a {To} (včetně).",
			_ => null,
		};
	}
}
