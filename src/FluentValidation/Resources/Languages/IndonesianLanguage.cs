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

	internal class IndonesianLanguage {
		public const string Culture = "id";

		public static string GetTranslation(string key) => key switch {
			"EmailValidator" => "'{PropertyName}' bukan alamat email yang benar.",
			"GreaterThanOrEqualValidator" => "'{PropertyName}' harus lebih besar dari atau sama dengan '{ComparisonValue}'.",
			"GreaterThanValidator" => "'{PropertyName}' harus lebih besar dari '{ComparisonValue}'.",
			"LengthValidator" => "'{PropertyName}' harus di antara {MinLength} dan {MaxLength} karakter. Anda memasukkan {TotalLength} karakter.",
			"MinimumLengthValidator" => "Panjang dari '{PropertyName}' harus paling tidak {MinLength} karakter. Anda memasukkan {TotalLength} karakter.",
			"MaximumLengthValidator" => "Panjang dari '{PropertyName}' harus {MaxLength} karakter atau kurang. Anda memasukkan {TotalLength} karakter.",
			"LessThanOrEqualValidator" => "'{PropertyName}' harus kurang dari atau sama dengan '{ComparisonValue}'.",
			"LessThanValidator" => "'{PropertyName}' harus kurang dari '{ComparisonValue}'.",
			"NotEmptyValidator" => "'{PropertyName}' tidak boleh kosong.",
			"NotEqualValidator" => "'{PropertyName}' tidak boleh sama dengan '{ComparisonValue}'.",
			"NotNullValidator" => "'{PropertyName}' tidak boleh kosong.",
			"PredicateValidator" => "Kondisi yang ditentukan tidak terpenuhi untuk '{PropertyName}'.",
			"AsyncPredicateValidator" => "Kondisi yang ditentukan tidak terpenuhi untuk '{PropertyName}'.",
			"RegularExpressionValidator" => "'{PropertyName}' bukan dalam format yang benar.",
			"EqualValidator" => "'{PropertyName}' harus sama dengan '{ComparisonValue}'.",
			"ExactLengthValidator" => "'{PropertyName}' harus {MaxLength} karakter panjangnya. Anda memasukkan {TotalLength} karakter.",
			"InclusiveBetweenValidator" => "'{PropertyName}' harus di antara {From} dan {To}. Anda memasukkan {PropertyValue}.",
			"ExclusiveBetweenValidator" => "'{PropertyName}' harus di antara {From} dan {To} (exclusive). Anda memasukkan {PropertyValue}.",
			"CreditCardValidator" => "'{PropertyName}' bukan nomor kartu kredit yang benar.",
			"ScalePrecisionValidator" => "Jumlah digit '{PropertyName}' tidak boleh lebih dari {ExpectedPrecision}, dengan toleransi {ExpectedScale} desimal. {Digits} digit dan {ActualScale} desimal ditemukan.",
			"EmptyValidator" => "'{PropertyName}' harus kosong.",
			"NullValidator" => "'{PropertyName}' harus kosong.",
			"EnumValidator" => "'{PropertyName}' memiliki rentang nilai yang tidak mengikutsertakan '{PropertyValue}'.",
			// Additional fallback messages used by clientside validation integration.
			"Length_Simple" => "'{PropertyName}' harus di antara {MinLength} dan {MaxLength} karakter.",
			"MinimumLength_Simple" => "Panjang dari '{PropertyName}' harus paling tidak {MinLength} karakter.",
			"MaximumLength_Simple" => "Panjang dari '{PropertyName}' harus {MaxLength} karakter atau fewer.",
			"ExactLength_Simple" => "'{PropertyName}' harus {MaxLength} karakter panjangnya.",
			"InclusiveBetween_Simple" => "'{PropertyName}' harus di antara {From} dan {To}.",
			_ => null,
		};
	}
}
