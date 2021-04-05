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
	internal class VietnameseLanguage {
		public const string Culture = "vi";

		public static string GetTranslation(string key) => key switch {
			"EmailValidator" => "'{PropertyName}' không phải là một email hợp lệ.",
			"GreaterThanOrEqualValidator" => "'{PropertyName}' phải lớn hơn hoặc bằng với '{ComparisonValue}'.",
			"GreaterThanValidator" => "'{PropertyName}' phải lớn hơn '{ComparisonValue}'.",
			"LengthValidator" => "'{PropertyName}' phải nằm trong khoảng từ {MinLength} đến {MaxLength} kí tự. Bạn đã nhập {TotalLength} kí tự.",
			"MinimumLengthValidator" => "Độ dài tối thiểu của '{PropertyName}' phải là {MinLength} kí tự. Bạn đã nhập {TotalLength} kí tự.",
			"MaximumLengthValidator" => "Độ dài tối đa của '{PropertyName}' phải là {MaxLength} kí tự hoặc ít hơn. Bạn đã nhập {TotalLength} kí tự.",
			"LessThanOrEqualValidator" => "'{PropertyName}' phải nhỏ hơn hoặc bằng '{ComparisonValue}'.",
			"LessThanValidator" => "'{PropertyName}' phải nhỏ hơn '{ComparisonValue}'.",
			"NotEmptyValidator" => "'{PropertyName}' không được rỗng.",
			"NotEqualValidator" => "'{PropertyName}' không được bằng '{ComparisonValue}'.",
			"NotNullValidator" => "'{PropertyName}' phải có giá trị.",
			"PredicateValidator" => "Không thỏa mãn điều kiện chỉ định đối với '{PropertyName}'.",
			"AsyncPredicateValidator" => "Không thỏa mãn điều kiện chỉ định đối với '{PropertyName}'.",
			"RegularExpressionValidator" => "'{PropertyName}' không đúng định dạng.",
			"EqualValidator" => "'{PropertyName}' phải bằng '{ComparisonValue}'.",
			"ExactLengthValidator" => "'{PropertyName}' phải có độ dài chính xác {MaxLength} kí tự. Bạn đã nhập {TotalLength} kí tự.",
			"InclusiveBetweenValidator" => "'{PropertyName}' phải có giá trị trong khoảng từ {From} đến {To}. Bạn đã nhập {PropertyValue}.",
			"ExclusiveBetweenValidator" => "'{PropertyName}' phải có giá trị trong khoảng giữa {From} và {To} (không bao gồm hai giới hạn). Bạn đã nhập {PropertyValue}.",
			"CreditCardValidator" => "'{PropertyName}' không đúng định dạng thẻ tín dụng.",
			"ScalePrecisionValidator" => "'{PropertyName}' không được vượt quá {ExpectedPrecision} chữ số tổng cộng và {ExpectedScale} chữ số phần thập phân. Phát hiện {Digits} chữ số và {ActualScale} chữ số phần thập phân.",
			"EmptyValidator" => "'{PropertyName}' phải là rỗng.",
			"NullValidator" => "'{PropertyName}' không được chứa giá trị.",
			"EnumValidator" => "'{PropertyName}' nằm trong một tập giá trị không bao gồm '{PropertyValue}'.",
			// Additional fallback messages used by clientside validation integration.
			"Length_Simple" => "'{PropertyName}' phải nằm trong khoảng từ {MinLength} đến {MaxLength} kí tự.",
			"MinimumLength_Simple" => "Độ dài tối thiểu của '{PropertyName}' phải là {MinLength} kí tự.",
			"MaximumLength_Simple" => "Độ dài tối đa của '{PropertyName}' phải là {MaxLength} kí tự hoặc ít hơn.",
			"ExactLength_Simple" => "'{PropertyName}' phải có độ dài chính xác {MaxLength} kí tự.",
			"InclusiveBetween_Simple" => "'{PropertyName}' phải có giá trị trong khoảng từ {From} đến {To}.",
			_ => null,
		};
	}
}
