using System;
using System.Collections.Generic;
using System.Text;

namespace FluentValidation.Resources {
	using Validators;

	internal class ThaiLanguage {
		public const string Culture = "th";

		public static string GetTranslation(string key) => key switch {
			"EmailValidator" => "'{PropertyName}'ไม่ใช่อีเมลที่ถูกต้อง",
			"GreaterThanOrEqualValidator" => "'{PropertyName}'ต้องมีค่ามากกว่าหรือเท่ากับ'{ComparisonValue}'",
			"GreaterThanValidator" => "'{PropertyName}'ต้องมีค่ามากกว่า'{ComparisonValue}'",
			"LengthValidator" => "'{PropertyName}'ต้องมีจำนวนตัวอักษรอยู่ระหว่าง{MinLength}และ{MaxLength}ตัวอักษร คุณให้ข้อมูลทั้งหมด{TotalLength}ตัวอักษร",
			"MinimumLengthValidator" => "จำนวนตัวอักษร'{PropertyName}'ต้องมีอย่างน้อย{MinLength}ตัวอักษร คุณให้ข้อมูลทั้งหมด{TotalLength}ตัวอักษร",
			"MaximumLengthValidator" => "จำนวนตัวอักษร'{PropertyName}'ต้องเท่ากับ{MaxLength}ตัวอักษรหรือน้อยกว่า คุณให้ข้อมูลทั้งหมด{TotalLength}ตัวอักษร",
			"LessThanOrEqualValidator" => "'{PropertyName}'ต้องมีค่าน้อยกว่าหรือเท่ากับ'{ComparisonValue}'.",
			"LessThanValidator" => "'{PropertyName}'ต้องมีค่าน้อยกว่า'{ComparisonValue}'",
			"NotEmptyValidator" => "'{PropertyName}'ต้องไม่มีค่าว่างเปล่า",
			"NotEqualValidator" => "'{PropertyName}'ต้องไม่เท่ากับ'{ComparisonValue}'",
			"NotNullValidator" => "'{PropertyName}'ต้องมีค่า",
			"PredicateValidator" => "ข้อมูลของ'{PropertyName}'ผิดกฎเกณฑ์ที่กำหนดไว้",
			"AsyncPredicateValidator" => "ข้อมูลของ'{PropertyName}'ผิดกฎเกณฑ์ที่กำหนดไว้",
			"RegularExpressionValidator" => "ข้อมูลของ'{PropertyName}'ผิดรูปแบบ",
			"EqualValidator" => "'{PropertyName}'ต้องมีค่าเท่ากับ'{ComparisonValue}'",
			"ExactLengthValidator" => "'{PropertyName}'ต้องมีจำนวนตัวอักษร{MaxLength}ตัวอักษร คุณให้ข้อมูลทั้งหมด{TotalLength}ตัวอักษร",
			"InclusiveBetweenValidator" => "'{PropertyName}'ต้องมีค่าระหว่าง{From}ถึง{To} คุณให้ข้อมูล{PropertyValue}",
			"ExclusiveBetweenValidator" => "'{PropertyName}'ต้องมีค่าอยู่ระหว่างแต่ไม่รวม{From}และ{To} คุณให้ข้อมูล{PropertyValue}",
			"CreditCardValidator" => "'{PropertyName}'ไม่ใช่ตัวเลขบัตรเครดิตที่ถูกต้อง",
			"ScalePrecisionValidator" => "'{PropertyName}'ต้องไม่มีจำนวนตัวเลขมากกว่า{ExpectedPrecision}ตำแหน่งทั้งหมด และมีจุดทศนิยม{ExpectedScale}ตำแหน่ง ข้อมูลมีตัวเลขค่าเต็ม{Digits}ตำแหน่งและจุดทศนิยม{ActualScale}ตำแหน่ง",
			"EmptyValidator" => "'{PropertyName}'ต้องมีค่าว่างเปล่า",
			"NullValidator" => "'{PropertyName}'ต้องไม่มีค่า",
			"EnumValidator" => "ค่าของ'{PropertyValue}'ไม่ได้อยู่ในค่าของ'{PropertyName}'",
			// Additional fallback messages used by clientside validation integration.
			"Length_Simple" => "'{PropertyName}'ต้องมีจำนวนตัวอักษรระหว่าง{MinLength}แหละ{MaxLength}ตัวอักษร",
			"MinimumLength_Simple" => "จำนวนตัวอักษรของ'{PropertyName}'ต้องมีอย่างน้อย{MinLength}ตัวอักษร",
			"MaximumLength_Simple" => "จำนวนตัวอักษรของ'{PropertyName}'ต้องเท่ากับหรือน้อยกว่า{MaxLength}ตัวอักษร",
			"ExactLength_Simple" => "จำนวนตัวอักษรของ'{PropertyName}'ต้องเท่ากับ{MaxLength}ตัวอักษร",
			"InclusiveBetween_Simple" => "'{PropertyName}'ต้องมีค่าระหว่าง{From}ถึง{To}",
			_ => null,
		};
	}
}
