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

	internal class IndonesianLanguage : Language {
		public const string Culture = "id";
		public override string Name => Culture;

		public IndonesianLanguage() {
			Translate<EmailValidator>("'{PropertyName}' bukan alamat email yang benar.");
			Translate<GreaterThanOrEqualValidator>("'{PropertyName}' harus lebih besar dari atau sama dengan '{ComparisonValue}'.");
			Translate<GreaterThanValidator>("'{PropertyName}' harus lebih besar dari '{ComparisonValue}'.");
			Translate<LengthValidator>("'{PropertyName}' harus di antara {MinLength} dan {MaxLength} karakter. Anda memasukkan {TotalLength} karakter.");
			Translate<MinimumLengthValidator>("Panjang dari '{PropertyName}' harus paling tidak {MinLength} karakter. Anda memasukkan {TotalLength} karakter.");
			Translate<MaximumLengthValidator>("Panjang dari '{PropertyName}' harus {MaxLength} karakter atau kurang. Anda memasukkan {TotalLength} karakter.");
			Translate<LessThanOrEqualValidator>("'{PropertyName}' harus kurang dari atau sama dengan '{ComparisonValue}'.");
			Translate<LessThanValidator>("'{PropertyName}' harus kurang dari '{ComparisonValue}'.");
			Translate<NotEmptyValidator>("'{PropertyName}' tidak boleh kosong.");
			Translate<NotEqualValidator>("'{PropertyName}' tidak boleh sama dengan '{ComparisonValue}'.");
			Translate<NotNullValidator>("'{PropertyName}' tidak boleh kosong.");
			Translate<PredicateValidator>("Kondisi yang ditentukan tidak terpenuhi untuk '{PropertyName}'.");
			Translate<AsyncPredicateValidator>("Kondisi yang ditentukan tidak terpenuhi untuk '{PropertyName}'.");
			Translate<RegularExpressionValidator>("'{PropertyName}' bukan dalam format yang benar.");
			Translate<EqualValidator>("'{PropertyName}' harus sama dengan '{ComparisonValue}'.");
			Translate<ExactLengthValidator>("'{PropertyName}' harus {MaxLength} karakter panjangnya. Anda memasukkan {TotalLength} karakter.");
			Translate<InclusiveBetweenValidator>("'{PropertyName}' harus di antara {From} dan {To}. Anda memasukkan {Value}.");
			Translate<ExclusiveBetweenValidator>("'{PropertyName}' harus di antara {From} dan {To} (exclusive). Anda memasukkan {Value}.");
			Translate<CreditCardValidator>("'{PropertyName}' bukan nomor kartu kredit yang benar.");
			Translate<ScalePrecisionValidator>("Jumlah digit '{PropertyName}' tidak boleh lebih dari {ExpectedPrecision}, dengan toleransi {ExpectedScale} desimal. {Digits} digit dan {ActualScale} desimal ditemukan.");
			Translate<EmptyValidator>("'{PropertyName}' harus kosong.");
			Translate<NullValidator>("'{PropertyName}' harus kosong.");
			Translate<EnumValidator>("'{PropertyName}' memiliki rentang nilai yang tidak mengikutsertakan '{PropertyValue}'.");
			// Additional fallback messages used by clientside validation integration.
			Translate("Length_Simple", "'{PropertyName}' harus di antara {MinLength} dan {MaxLength} karakter.");
			Translate("MinimumLength_Simple", "Panjang dari '{PropertyName}' harus paling tidak {MinLength} karakter.");
			Translate("MaximumLength_Simple", "Panjang dari '{PropertyName}' harus {MaxLength} karakter atau fewer.");
			Translate("ExactLength_Simple", "'{PropertyName}' harus {MaxLength} karakter panjangnya.");
			Translate("InclusiveBetween_Simple", "'{PropertyName}' harus di antara {From} dan {To}.");
		}
	}
}
