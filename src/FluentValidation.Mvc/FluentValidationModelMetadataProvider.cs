#region License
// Copyright 2008-2009 Jeremy Skinner (http://www.jeremyskinner.co.uk)
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

namespace FluentValidation.Mvc {
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.ComponentModel.DataAnnotations;
	using System.Linq;
	using System.Web.Mvc;
	using Internal;
	using Validators;

	public class FluentValidationModelMetadataProvider : DataAnnotationsModelMetadataProvider {
		readonly IValidatorFactory factory;

		public FluentValidationModelMetadataProvider(IValidatorFactory factory) {
			this.factory = factory;
		}

		protected override ModelMetadata GetMetadataForProperty(Func<object> modelAccessor, Type containerType, PropertyDescriptor propertyDescriptor) {
			var attributes = ConvertFVMetaDataToAttributes(containerType, propertyDescriptor.Name);
			return CreateMetadata(attributes, containerType, modelAccessor, propertyDescriptor.PropertyType, propertyDescriptor.Name);
		}

		IEnumerable<Attribute> ConvertFVMetaDataToAttributes(Type type, string name) {
			var validator = factory.GetValidator(type);

			if (validator == null) {
				return Enumerable.Empty<Attribute>();
			}

			IEnumerable<IPropertyValidator> validators;

			if (name == null) {
				validators = validator.CreateDescriptor().GetMembersWithValidators().SelectMany(x => x);
			}
			else {
				validators = validator.CreateDescriptor().GetValidatorsForMember(name);
			}

			var attributes = validators.OfType<IAttributeMetadataValidator>()
				.Select(x => x.ToAttribute())
				.Concat(SpecialCaseValidatorConversions(validators));



			return attributes.ToList();
		}

		IEnumerable<Attribute> SpecialCaseValidatorConversions(IEnumerable<IPropertyValidator> validators) {

			//Email Validator should be convertible to DataType EmailAddress.
			return validators
				.OfType<IEmailValidator>()
				.Select(x => new DataTypeAttribute(DataType.EmailAddress))
				.Cast<Attribute>();
		}

		IEnumerable<Attribute> ConvertFVMetaDataToAttributes(Type type) {
			return ConvertFVMetaDataToAttributes(type, null);
		}

		public override ModelMetadata GetMetadataForType(Func<object> modelAccessor, Type modelType) {
			var attributes = ConvertFVMetaDataToAttributes(modelType);
			return CreateMetadata(attributes, null /* containerType */, modelAccessor, modelType, null /* propertyName */);
		}
	}
}