#region License
// Copyright (c) Jeremy Skinner (http://www.jeremyskinner.co.uk)
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
// The latest version of this file can be found at http://www.codeplex.com/FluentValidation
#endregion

namespace FluentValidation.Tests {
	using System;
	using System.Linq;
	using Mvc;
	using NUnit.Framework;
	using FluentValidation.Mvc.MetadataExtensions;
	//These tests are largely based upon DataAnnotationsModelMetadataProviderTest from the MVC2 source code.

	[TestFixture]
	public class FluentValidationModelMetadataProviderTester {
		FluentValidationModelMetadataProvider provider;

		[SetUp]
		public void Setup() {
			provider = new FluentValidationModelMetadataProvider(new TestValidatorFactory());
		}

		[Test]
		public void GetMetadataForProperties_returns_types_and_property_names() {
			var result = provider.GetMetadataForProperties("foo", typeof(string));

			bool hasProperties = result.Any(x => x.ModelType == typeof(int) && x.PropertyName == "Length" && (int)x.Model == 3);

			hasProperties.ShouldBeTrue();
		}

		[Test]
		public void GetMetadataForProperty_returns_type_and_property_name() {
			var result = provider.GetMetadataForProperty(null, typeof(string), "Length");

			result.ModelType.ShouldEqual(typeof(int));
			result.PropertyName.ShouldEqual("Length");
		}

		[Test]
		public void GetMetadataForType_returns_type_with_null_PropertyName() {
			var result = provider.GetMetadataForType(null, typeof(string));

			result.ModelType.ShouldEqual(typeof(string));
			result.PropertyName.ShouldBeNull();
		}

		private class HiddenModel {
			public int NotHidden { get; set; }
			public int DefaultHidden { get; set; }
			public int HiddenWIthDisplayValueFalse { get; set; }
			public int HiddenAndUIHint { get; set; }

			public class Validator : AbstractValidator<HiddenModel> {
				public Validator() {
					RuleFor(x => x.DefaultHidden).HiddenInput();
					RuleFor(x => x.HiddenWIthDisplayValueFalse).HiddenInput(false);
					RuleFor(x => x.HiddenAndUIHint).HiddenInput().UIHint("CustomUIHint");
				}
			}
		}

		[Test]
		public void HiddenValidator_sets_TemplateHint_and_HideSurroundingChrome() {
			var noAttributeMetadata = provider.GetMetadataForProperty(null, typeof(HiddenModel), "NotHidden");
			noAttributeMetadata.TemplateHint.ShouldBeNull();
			noAttributeMetadata.HideSurroundingHtml.ShouldBeFalse();

			var defaultHiddenMetadata = provider.GetMetadataForProperty(null, typeof(HiddenModel), "DefaultHidden");
			defaultHiddenMetadata.TemplateHint.ShouldEqual("HiddenInput");
			defaultHiddenMetadata.HideSurroundingHtml.ShouldBeFalse();

			var hiddenWithDisplayValueFalseMetadata = provider.GetMetadataForProperty(null, typeof(HiddenModel), "HiddenWithDisplayValueFalse");
			hiddenWithDisplayValueFalseMetadata.TemplateHint.ShouldEqual("HiddenInput");
			hiddenWithDisplayValueFalseMetadata.HideSurroundingHtml.ShouldBeTrue();

			// UIHint overrides the template hint from Hidden
			provider.GetMetadataForProperty(null, typeof(HiddenModel), "HiddenAndUIHint").TemplateHint.ShouldEqual("CustomUIHint");
		}

		private class UIHintModel {
			public int NoHint { get; set; }
			public int DefaultUIHint { get; set; }
			public int MvcUIHint { get; set; }
			public int NoMvcUIHint { get; set; }
			public int MultipleUIHint { get; set; }

			public class Validator : AbstractValidator<UIHintModel> {
				public Validator() {
					RuleFor(x => x.DefaultUIHint).UIHint("MyCustomTemplate");
					RuleFor(x => x.MvcUIHint).UIHint("MyMvcTemplate", "MVC");
					RuleFor(x => x.NoMvcUIHint).UIHint("MyWebFormsTemplate", "WebForms");
					RuleFor(x => x.MultipleUIHint).UIHint("MyDefaultTemplate").UIHint("MyWebFormsTemplate", "WebForms").UIHint("MyMvcTemplate", "MVC");
				}
			}
		}

		[Test]
		public void UIHintValidator_sets_template_hint() {
			provider.GetMetadataForProperty(null, typeof(UIHintModel), "NoHint").TemplateHint.ShouldBeNull();
			provider.GetMetadataForProperty(null, typeof(UIHintModel), "DefaultUIHint").TemplateHint.ShouldEqual("MyCustomTemplate");
			provider.GetMetadataForProperty(null, typeof(UIHintModel), "MvcUIHint").TemplateHint.ShouldEqual("MyMvcTemplate");
			provider.GetMetadataForProperty(null, typeof(UIHintModel), "NoMvcUIHint").TemplateHint.ShouldBeNull();
			provider.GetMetadataForProperty(null, typeof(UIHintModel), "MultipleUIHint").TemplateHint.ShouldEqual("MyMvcTemplate");
		}

		private class DataTypeModel {
			public int NoAttribute { get; set; }
			public string EmailAddressProperty { get; set; }
			public string EmailAddressValidator { get; set; }
			public int CustomDataTypeProperty { get; set; }

			public class Validator : AbstractValidator<DataTypeModel> {
				public Validator() {
					RuleFor(x => x.EmailAddressValidator).EmailAddress();
					RuleFor(x => x.EmailAddressProperty).DataType(System.ComponentModel.DataAnnotations.DataType.EmailAddress);
					RuleFor(x => x.CustomDataTypeProperty).DataType("CustomDataType");
				}
			}
		}

		[Test]
		public void DataTypeAttributeSetsDataType() {
			Assert.IsNull(provider.GetMetadataForProperty(null, typeof(DataTypeModel), "NoAttribute").DataTypeName);
			Assert.AreEqual("EmailAddress", provider.GetMetadataForProperty(null, typeof(DataTypeModel), "EmailAddressProperty").DataTypeName);
			Assert.AreEqual("EmailAddress", provider.GetMetadataForProperty(null, typeof(DataTypeModel), "EmailAddressValidator").DataTypeName);
			Assert.AreEqual("CustomDataType", provider.GetMetadataForProperty(null, typeof(DataTypeModel), "CustomDataTypeProperty").DataTypeName);
		}

		class IsReadOnlyModel {
			public int NoAttribute { get; set; }
			public int ReadOnlyTrue { get; set; }
			public int ReadOnlyFalse { get; set; }

			public class Validator : AbstractValidator<IsReadOnlyModel> {
				public Validator() {
					RuleFor(x => x.ReadOnlyTrue).ReadOnly(true);
					RuleFor(x => x.ReadOnlyFalse).ReadOnly(false);
				}
			}
		}

		[Test]
		public void ReadOnlyAttributeSetsIsReadOnly() {
			// Act & Assert
			Assert.IsFalse(provider.GetMetadataForProperty(null, typeof(IsReadOnlyModel), "NoAttribute").IsReadOnly);
			Assert.IsTrue(provider.GetMetadataForProperty(null, typeof(IsReadOnlyModel), "ReadOnlyTrue").IsReadOnly);
			Assert.IsFalse(provider.GetMetadataForProperty(null, typeof(IsReadOnlyModel), "ReadOnlyFalse").IsReadOnly);
		}

		class DisplayFormatModel {
			public int NoAttribute { get; set; }
			public int NullDisplayText { get; set; }
			public int DisplayFormatString { get; set; }
			public int DisplayAndEditFormatString { get; set; }
			public int ConvertEmptyStringToNullTrue { get; set; }
			public int ConvertEmptyStringToNullFalse { get; set; }

			public class Validator : AbstractValidator<DisplayFormatModel> {
				public Validator() {
					RuleFor(x => x.NullDisplayText).DisplayFormat().NullDisplayText("(null value)");
					RuleFor(x => x.DisplayFormatString).DisplayFormat().DataFormatString("Data {0} format");
					RuleFor(x => x.DisplayAndEditFormatString).DisplayFormat().DataFormatString("Data {0} format").ApplyFormatInEditMode(true);
					RuleFor(x => x.ConvertEmptyStringToNullTrue).DisplayFormat().ConvertEmptyStringToNull(true);
					RuleFor(x => x.ConvertEmptyStringToNullFalse).DisplayFormat().ConvertEmptyStringToNull(false);
				}
			}
		}

		[Test]
		public void DisplayFormaAttributetSetsNullDisplayText() {
			Assert.IsNull(provider.GetMetadataForProperty(null, typeof(DisplayFormatModel), "NoAttribute").NullDisplayText);
			Assert.AreEqual("(null value)", provider.GetMetadataForProperty(null, typeof(DisplayFormatModel), "NullDisplayText").NullDisplayText);
		}

		[Test]
		public void DisplayFormatAttributeSetsDisplayFormatString() {
			Assert.IsNull(provider.GetMetadataForProperty(null, typeof(DisplayFormatModel), "NoAttribute").DisplayFormatString);
			Assert.AreEqual("Data {0} format", provider.GetMetadataForProperty(null, typeof(DisplayFormatModel), "DisplayFormatString").DisplayFormatString);
			Assert.AreEqual("Data {0} format", provider.GetMetadataForProperty(null, typeof(DisplayFormatModel), "DisplayAndEditFormatString").DisplayFormatString);
		}

		[Test]
		public void DisplayFormatAttributeSetEditFormatString() {
			Assert.IsNull(provider.GetMetadataForProperty(null, typeof(DisplayFormatModel), "NoAttribute").EditFormatString);
			Assert.IsNull(provider.GetMetadataForProperty(null, typeof(DisplayFormatModel), "DisplayFormatString").EditFormatString);
			Assert.AreEqual("Data {0} format", provider.GetMetadataForProperty(null, typeof(DisplayFormatModel), "DisplayAndEditFormatString").EditFormatString);
		}

		[Test]
		public void DisplayFormatAttributeSetsConvertEmptyStringToNull() {
			Assert.IsTrue(provider.GetMetadataForProperty(null, typeof(DisplayFormatModel), "NoAttribute").ConvertEmptyStringToNull);
			Assert.IsTrue(provider.GetMetadataForProperty(null, typeof(DisplayFormatModel), "ConvertEmptyStringToNullTrue").ConvertEmptyStringToNull);
			Assert.IsFalse(provider.GetMetadataForProperty(null, typeof(DisplayFormatModel), "ConvertEmptyStringToNullFalse").ConvertEmptyStringToNull);
		}

		class ScaffoldColumnModel {
			public int NoAttribute { get; set; }
			public int ScaffoldColumnTrue { get; set; }
			public int ScaffoldColumnFalse { get; set; }

			public class Validator : AbstractValidator<ScaffoldColumnModel> {
				public Validator() {
					RuleFor(x => x.ScaffoldColumnTrue).Scaffold(true);
					RuleFor(x => x.ScaffoldColumnFalse).Scaffold(false);
				}
			}
		}

		[Test]
		public void ScaffoldColumnAttributeSetsShowForDisplay() {
			Assert.IsTrue(provider.GetMetadataForProperty(null, typeof(ScaffoldColumnModel), "NoAttribute").ShowForDisplay);
			Assert.IsTrue(provider.GetMetadataForProperty(null, typeof(ScaffoldColumnModel), "ScaffoldColumnTrue").ShowForDisplay);
			Assert.IsFalse(provider.GetMetadataForProperty(null, typeof(ScaffoldColumnModel), "ScaffoldColumnFalse").ShowForDisplay);
		}

		[Test]
		public void ScaffoldColumnAttributeSetsShowForEdit() {
			Assert.IsTrue(provider.GetMetadataForProperty(null, typeof(ScaffoldColumnModel), "NoAttribute").ShowForEdit);
			Assert.IsTrue(provider.GetMetadataForProperty(null, typeof(ScaffoldColumnModel), "ScaffoldColumnTrue").ShowForEdit);
			Assert.IsFalse(provider.GetMetadataForProperty(null, typeof(ScaffoldColumnModel), "ScaffoldColumnFalse").ShowForEdit);
		}

		//		[DisplayColumn("NoPropertyWithThisName")]
		//		class UnknownDisplayColumnModel { }

		[Test, Ignore]
		public void SimpleDisplayNameWithUnknownDisplayColumnThrows() {
			//			typeof(InvalidOperationException).ShouldBeThrownBy(() => {
			//				var x = provider.GetMetadataForType(() => new UnknownDisplayColumnModel(), typeof(UnknownDisplayColumnModel)).SimpleDisplayText;
			//			});
		}

		//		[DisplayColumn("WriteOnlyProperty")]
		//		class WriteOnlyDisplayColumnModel {
		//			public int WriteOnlyProperty { set { } }
		//		}

		//		[DisplayColumn("PrivateReadPublicWriteProperty")]
		//		class PrivateReadPublicWriteDisplayColumnModel {
		//			public int PrivateReadPublicWriteProperty { private get; set; }
		//		}

		[Test, Ignore]
		public void SimpleDisplayTextForTypeWithWriteOnlyDisplayColumnThrows() {
			//			typeof(InvalidOperationException).ShouldBeThrownBy(() => {
			//				var x = provider.GetMetadataForType(() => new WriteOnlyDisplayColumnModel(), typeof(WriteOnlyDisplayColumnModel)).SimpleDisplayText;
			//			});

			//			typeof(InvalidOperationException).ShouldBeThrownBy(() => {
			//				var x = provider.GetMetadataForType(() => new PrivateReadPublicWriteDisplayColumnModel(), typeof(PrivateReadPublicWriteDisplayColumnModel)).SimpleDisplayText= 
			//			});
		}

		/*
				[DisplayColumn("DisplayColumnProperty")]
				class SimpleDisplayTextAttributeModel {
					public int FirstProperty { get { return 42; } }

					[ScaffoldColumn(false)]
					public string DisplayColumnProperty { get; set; }
				}
		*/

		[Test, Ignore]
		public void SimpleDisplayTextForNonNullClassWithNonNullDisplayColumnValue() {
			/*	string expected = "Custom property display value";
				SimpleDisplayTextAttributeModel model = new SimpleDisplayTextAttributeModel { DisplayColumnProperty = expected };
				var metadata = provider.GetMetadataForType(() => model, typeof(SimpleDisplayTextAttributeModel));

				string result = metadata.SimpleDisplayText;

				Assert.AreEqual(expected, result);*/
		}

		[Test, Ignore]
		public void SimpleDisplayTextForNullClassRevertsToDefaultBehavior() {
			/*
						string expected = "Null Display Text";
						var metadata = provider.GetMetadataForType(null, typeof(SimpleDisplayTextAttributeModel));
						metadata.NullDisplayText = expected;

						string result = metadata.SimpleDisplayText;

						Assert.AreEqual(expected, result);
			*/
		}

		[Test, Ignore]
		public void SimpleDisplayTextForNonNullClassWithNullDisplayColumnValueRevertsToDefaultBehavior() {
			/*
						SimpleDisplayTextAttributeModel model = new SimpleDisplayTextAttributeModel();
						var metadata = provider.GetMetadataForType(() => model, typeof(SimpleDisplayTextAttributeModel));

						string result = metadata.SimpleDisplayText;

						Assert.AreEqual("42", result);    // Falls back to the default logic of first property value
			*/
		}

		class DisplayNameModel {
			public int Without { get; set; }
			public int With { get; set; }

			public class Validator : AbstractValidator<DisplayNameModel> {
				public Validator() {
					RuleFor(x => x.With).DisplayName("Custom property name");
				}
			}
		}

		[Test]
		public void DisplayNameAttributeSetsDisplayName() {
			Assert.IsNull(provider.GetMetadataForProperty(null, typeof(DisplayNameModel), "Without").DisplayName);
			Assert.AreEqual("Custom property name", provider.GetMetadataForProperty(null, typeof(DisplayNameModel), "With").DisplayName);
		}

		[Test]
		public void RequiredAttributeSetsRequired() {
			provider.GetMetadataForProperty(null, typeof(RequiredModel), "WithNotNull")
				.IsRequired.ShouldBeTrue();

			provider.GetMetadataForProperty(null, typeof(RequiredModel), "WithNotEmpty")
				.IsRequired.ShouldBeTrue();

			provider.GetMetadataForProperty(null, typeof(RequiredModel), "WithNothing")
				.IsRequired.ShouldBeFalse();

		}


		class RequiredModel {
			public string WithNotNull { get; set; }
			public string WithNotEmpty { get; set; }
			public string WithNothing { get; set; }

			public class Validator : AbstractValidator<RequiredModel> {
				public Validator() {
					RuleFor(x => x.WithNotNull).NotNull();
					RuleFor(x => x.WithNotEmpty).NotEmpty();
				}
			}
		}


		private class TestValidatorFactory : IValidatorFactory {
			public IValidator<T> GetValidator<T>() {
				throw new NotImplementedException();
			}

			public IValidator GetValidator(Type type) {

				if (type == typeof(HiddenModel)) {
					return new HiddenModel.Validator();
				}
				if (type == typeof(UIHintModel)) {
					return new UIHintModel.Validator();
				}
				if (type == typeof(DataTypeModel)) {
					return new DataTypeModel.Validator();
				}
				if (type == typeof(IsReadOnlyModel)) {
					return new IsReadOnlyModel.Validator();
				}
				if (type == typeof(DisplayFormatModel)) {
					return new DisplayFormatModel.Validator();
				}
				if (type == typeof(ScaffoldColumnModel)) {
					return new ScaffoldColumnModel.Validator();
				}
				if (type == typeof(DisplayNameModel)) {
					return new DisplayNameModel.Validator();
				}
				if(type == typeof(RequiredModel)) {
					return new RequiredModel.Validator();
				}

				return null;
			}
		}
	}
}