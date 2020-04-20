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

	internal class VietnameseLanguage : Language {
		public const string Culture = "vi";
		public override string Name => Culture;

		public VietnameseLanguage() {
			Translate<EmailValidator>("'{PropertyName}' không phải là một địa chỉ email hợp lệ.");
			Translate<GreaterThanOrEqualValidator>("'{PropertyName}' phải lớn hơn hoặc bằng '{ComparisonValue}'.");
			Translate<GreaterThanValidator>("'{PropertyName}' phải lớn hơn '{ComparisonValue}'.");
			Translate<LengthValidator>("'{PropertyName}' phải có từ {MinLength} đến {MaxLength} ký tự. Bạn đã nhập {TotalLength} ký tự.");
			Translate<MinimumLengthValidator>("Độ dài của '{PropertyName}' cần ít nhất {MinLength} ký tự. Bạn đã nhập {TotalLength} ký tự.");
			Translate<MaximumLengthValidator>("Độ dài của '{PropertyName}' phải là {MaxLength} ký tự trở xuống. Bạn đã nhập {TotalLength} ký tự.");
			Translate<LessThanOrEqualValidator>("'{PropertyName}' phải nhỏ hơn hoặc bằng'{ComparisonValue}'.");
			Translate<LessThanValidator>("'{PropertyName}' phải nhỏ hơn '{ComparisonValue}'.");
			Translate<NotEmptyValidator>("'{PropertyName}' không được để trống.");
			Translate<NotEqualValidator>("'{PropertyName}' không được bằng '{ComparisonValue}'.");
			Translate<NotNullValidator>("'{PropertyName}' không được để trống.");
			Translate<PredicateValidator>("Điều kiện quy định không được đáp ứng cho '{PropertyName}'.");
			Translate<AsyncPredicateValidator>("Điều kiện quy định không được đáp ứng cho '{PropertyName}'.");
			Translate<RegularExpressionValidator>("'{PropertyName}' không đúng định dạng.");
			Translate<EqualValidator>("'{PropertyName}' phải bằng '{ComparisonValue}'.");
			Translate<ExactLengthValidator>("'{PropertyName}' phải dài {MaxLength} ký tự. Bạn đã nhập {TotalLength} ký tự.");
			Translate<InclusiveBetweenValidator>("'{PropertyName}' phải ở giữa {From} và {To}. Bạn đã nhập {Value}.");
			Translate<ExclusiveBetweenValidator>("'{PropertyName}' phải ở giữa {From} và {To} (riêng). Bạn đã nhập {Value}.");
			Translate<CreditCardValidator>("'{PropertyName}' số thẻ tín dụng không hợp lệ.");
			Translate<ScalePrecisionValidator>("'{PropertyName}' không được có tổng cộng quá {ExpectedPrecision} chữ số, với phần mở rộng {ExpectedScale} số thập phân. Hiện đã có {Digits} chữ số và {ActualScale} số thập phân.");
			Translate<EmptyValidator>("'{PropertyName}' phải để trống.");
			Translate<NullValidator>("'{PropertyName}' phải để trống.");
			Translate<EnumValidator>("'{PropertyName}' có một loạt các giá trị mà không bao gồm '{PropertyValue}'.");
			// Additional fallback messages used by clientside validation integration.
			Translate("Length_Simple", "'{PropertyName}' phải có từ {MinLength} đến {MaxLength} ký tự.");
			Translate("MinimumLength_Simple", "Độ dài của '{PropertyName}' phải có ít nhất {MinLength} ký tự.");
			Translate("MaximumLength_Simple", "Độ dài của '{PropertyName}' phải là {MaxLength} ký tự trở xuống.");
			Translate("ExactLength_Simple", "'{PropertyName}' phải là {MaxLength} ký tự.");
			Translate("InclusiveBetween_Simple", "'{PropertyName}' phải ở giữa {From} và {To}.");
		}
	}
}
