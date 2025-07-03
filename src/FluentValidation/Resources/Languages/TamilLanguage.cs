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

internal class TamilLanguage {
	public const string Culture = "ta";

	public static string GetTranslation(string key) => key switch {
		"EmailValidator" => "'{PropertyName}' ஒரு செல்லுபடியான மின்னஞ்சல் முகவரி அல்ல.",
		"GreaterThanOrEqualValidator" => "'{PropertyName}' '{ComparisonValue}'-ஐ விட அதிகமாக அல்லது சமமாக இருக்க வேண்டும்.",
		"GreaterThanValidator" => "'{PropertyName}' '{ComparisonValue}'-ஐ விட அதிகமாக இருக்க வேண்டும்.",
		"LengthValidator" => "'{PropertyName}' {MinLength} மற்றும் {MaxLength} எழுத்துகளுக்கு இடையில் இருக்க வேண்டும். நீங்கள் {TotalLength} எழுத்துகள் உள்ளீடு செய்துள்ளீர்கள்.",
		"MinimumLengthValidator" => "'{PropertyName}' இன் நீளம் குறைந்தது {MinLength} எழுத்துகள் இருக்க வேண்டும். நீங்கள் {TotalLength} எழுத்துகள் உள்ளீடு செய்துள்ளீர்கள்.",
		"MaximumLengthValidator" => "'{PropertyName}' இன் நீளம் அதிகபட்சம் {MaxLength} எழுத்துகள் இருக்க வேண்டும். நீங்கள் {TotalLength} எழுத்துகள் உள்ளீடு செய்துள்ளீர்கள்.",
		"LessThanOrEqualValidator" => "'{PropertyName}' '{ComparisonValue}'-ஐ விட குறைவாக அல்லது சமமாக இருக்க வேண்டும்.",
		"LessThanValidator" => "'{PropertyName}' '{ComparisonValue}'-ஐ விட குறைவாக இருக்க வேண்டும்.",
		"NotEmptyValidator" => "'{PropertyName}' காலியாக இருக்கக்கூடாது.",
		"NotEqualValidator" => "'{PropertyName}' '{ComparisonValue}'-இற்குச் சமமாக இருக்கக்கூடாது.",
		"NotNullValidator" => "'{PropertyName}' காலியாக இருக்கக்கூடாது.",
		"PredicateValidator" => "'{PropertyName}'க்கு குறிப்பிடப்பட்ட நிபந்தனை பூர்த்தி செய்யவில்லை.",
		"AsyncPredicateValidator" => "'{PropertyName}'க்கு குறிப்பிடப்பட்ட நிபந்தனை பூர்த்தி செய்யவில்லை.",
		"RegularExpressionValidator" => "'{PropertyName}' சரியான வடிவத்தில் இல்லை.",
		"EqualValidator" => "'{PropertyName}' '{ComparisonValue}'-இற்குச் சமமாக இருக்க வேண்டும்.",
		"ExactLengthValidator" => "'{PropertyName}' {MaxLength} எழுத்துகள் நீளமாக இருக்க வேண்டும். நீங்கள் {TotalLength} எழுத்துகள் உள்ளீடு செய்துள்ளீர்கள்.",
		"InclusiveBetweenValidator" => "'{PropertyName}' {From} மற்றும் {To} இடையில் இருக்க வேண்டும். நீங்கள் {PropertyValue} உள்ளீடு செய்துள்ளீர்கள்.",
		"ExclusiveBetweenValidator" => "'{PropertyName}' {From} மற்றும் {To} இடையே (விலக்கப்பட) இருக்க வேண்டும். நீங்கள் {PropertyValue} உள்ளீடு செய்துள்ளீர்கள்.",
		"CreditCardValidator" => "'{PropertyName}' ஒரு செல்லுபடியான கிரெடிட் கார்டு எண் அல்ல.",
		"ScalePrecisionValidator" => "'{PropertyName}' {ExpectedPrecision} இலக்கங்களை மொத்தமாக (அதில் {ExpectedScale} தசம இலக்கங்கள்) மீறக்கூடாது. {Digits} இலக்கங்கள் மற்றும் {ActualScale} தசமங்கள் உள்ளன.",
		"EmptyValidator" => "'{PropertyName}' காலியாக இருக்க வேண்டும்.",
		"NullValidator" => "'{PropertyName}' காலியாக இருக்க வேண்டும்.",
		"EnumValidator" => "'{PropertyName}' இன் மதிப்பானது '{PropertyValue}'-ஐ உள்ளடக்கவில்லை.",
		// Additional fallback messages
		"Length_Simple" => "'{PropertyName}' {MinLength} மற்றும் {MaxLength} எழுத்துகளுக்கு இடையில் இருக்க வேண்டும்.",
		"MinimumLength_Simple" => "'{PropertyName}' குறைந்தது {MinLength} எழுத்துகள் இருக்க வேண்டும்.",
		"MaximumLength_Simple" => "'{PropertyName}' அதிகபட்சம் {MaxLength} எழுத்துகள் இருக்க வேண்டும்.",
		"ExactLength_Simple" => "'{PropertyName}' {MaxLength} எழுத்துகள் நீளமாக இருக்க வேண்டும்.",
		"InclusiveBetween_Simple" => "'{PropertyName}' {From} மற்றும் {To} இடையில் இருக்க வேண்டும்.",
		_ => null,
	};
}
