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

namespace FluentValidation.Mvc.MetadataExtensions {
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.ComponentModel.DataAnnotations;
	using System.Web.Mvc;
	using Validators;

	public static class MetadataExtensions {

		public static IRuleBuilder<T, TProperty> HiddenInput<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder) {
			return ruleBuilder.SetValidator(new AttributeMetadataValidator(new HiddenInputAttribute()));
		}

		public static IRuleBuilder<T, TProperty> HiddenInput<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, bool displayValue) {
			return ruleBuilder.SetValidator(new AttributeMetadataValidator(new HiddenInputAttribute { DisplayValue = displayValue }));
		}

		public static IRuleBuilder<T, TProperty> UIHint<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, string hint) {
			return ruleBuilder.SetValidator(new AttributeMetadataValidator(new UIHintAttribute(hint)));
		}

		public static IRuleBuilder<T, TProperty> UIHint<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, string hint, string presentationLayer) {
			return ruleBuilder.SetValidator(new AttributeMetadataValidator(new UIHintAttribute(hint, presentationLayer)));
		}

		public static IRuleBuilder<T, TProperty> Scaffold<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, bool scaffold) {
			return ruleBuilder.SetValidator(new AttributeMetadataValidator(new ScaffoldColumnAttribute(scaffold)));
		}

		public static IRuleBuilder<T, TProperty> DataType<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, DataType dataType) {
			return ruleBuilder.SetValidator(new AttributeMetadataValidator(new DataTypeAttribute(dataType)));
		}
		public static IRuleBuilder<T, TProperty> DataType<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, string customDataType) {
			return ruleBuilder.SetValidator(new AttributeMetadataValidator(new DataTypeAttribute(customDataType)));
		}

		public static IRuleBuilder<T, TProperty> DisplayName<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, string name) {
#if NET4
			return ruleBuilder.SetValidator(new AttributeMetadataValidator(new DisplayAttribute { Name = name }));
#else
			return ruleBuilder.SetValidator(new AttributeMetadataValidator(new DisplayNameAttribute(name)));
#endif
		}

		public static IDisplayFormatBuilder<T, TProperty> DisplayFormat<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder) {
			return new DisplayFormatBuilder<T, TProperty>(ruleBuilder);
		}

		public static IRuleBuilder<T, TProperty> ReadOnly<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, bool readOnly) {
			return ruleBuilder.SetValidator(new AttributeMetadataValidator(new ReadOnlyAttribute(readOnly)));
		}

		public interface IDisplayFormatBuilder<T, TProperty> : IRuleBuilder<T, TProperty> {
			IDisplayFormatBuilder<T, TProperty> NullDisplayText(string text);
			IDisplayFormatBuilder<T, TProperty> DataFormatString(string text);
			IDisplayFormatBuilder<T, TProperty> ApplyFormatInEditMode(bool apply);
			IDisplayFormatBuilder<T, TProperty> ConvertEmptyStringToNull(bool convert);
		}

		private class DisplayFormatBuilder<T, TProperty> : IDisplayFormatBuilder<T, TProperty> {
			readonly IRuleBuilder<T, TProperty> builder;
			readonly DisplayFormatAttribute attribute = new DisplayFormatAttribute();

			public DisplayFormatBuilder(IRuleBuilder<T, TProperty> builder) {
				this.builder = builder;
				builder.SetValidator(new AttributeMetadataValidator(attribute));
			}

			public IRuleBuilderOptions<T, TProperty> SetValidator(IPropertyValidator validator) {
				return builder.SetValidator(validator);
			}

			public IRuleBuilderOptions<T, TProperty> SetValidator(IValidator validator) {
				return builder.SetValidator(validator);

			}

			public IDisplayFormatBuilder<T, TProperty> NullDisplayText(string text) {
				attribute.NullDisplayText = text;
				return this;
			}

			public IDisplayFormatBuilder<T, TProperty> DataFormatString(string text) {
				attribute.DataFormatString = text;
				return this;
			}

			public IDisplayFormatBuilder<T, TProperty> ApplyFormatInEditMode(bool apply) {
				attribute.ApplyFormatInEditMode = apply;
				return this;
			}

			public IDisplayFormatBuilder<T, TProperty> ConvertEmptyStringToNull(bool convert) {
				attribute.ConvertEmptyStringToNull = convert;
				return this;
			}
		}
	}
}