#region License
// Copyright (c) Jeremy Skinner (http://www.jeremyskinner.co.uk) and contributors.
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

namespace FluentValidation.AspNetCore {
	using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

	internal class FluentValidationBindingMetadataProvider : IBindingMetadataProvider {
		public const string Prefix = "_FV_REQUIRED|";

		/// <summary>
		/// If we're validating a non-nullable value type then
		/// MVC will automatically add a "Required" error message.
		/// We prefix these messages with a placeholder, so we can identify and remove them
		/// during the validation process.
		/// <see cref="FluentValidationVisitor"/>
		/// <see cref="MvcValidationHelper.RemoveImplicitRequiredErrors"/>
		/// <see cref="MvcValidationHelper.ReApplyImplicitRequiredErrorsNotHandledByFV"/>
		/// </summary>
		/// <param name="context"></param>
		public void CreateBindingMetadata(BindingMetadataProviderContext context) {
			if (context.Key.MetadataKind == ModelMetadataKind.Property) {
				var original = context.BindingMetadata.ModelBindingMessageProvider.ValueMustNotBeNullAccessor;
				context.BindingMetadata.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(s => Prefix + original(s));
			}
		}
	}
}
