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
	using System.Linq;
	using System.Linq.Expressions;
	using Results;
	using Validators;
	using Internal;

	internal interface IAttributeMetadataValidator {
		Attribute ToAttribute();
	}

	internal class AttributeMetadataValidator : IPropertyValidator, IAttributeMetadataValidator {
		readonly Attribute attribute;

		public AttributeMetadataValidator(Attribute attributeConverter) {
			attribute = attributeConverter;
		}

		public IEnumerable<ValidationFailure> Validate(PropertyValidatorContext context) {
			return Enumerable.Empty<ValidationFailure>();
		}

		public string ErrorMessageTemplate {
			get { return null; }
			set { }
		}

		public ICollection<Func<object, object>> CustomMessageFormatArguments {
			get { return null; }
		}

		public bool SupportsStandaloneValidation {
			get { return false; }
		}

		public Type ErrorMessageResourceType {
			get { throw new NotImplementedException(); }
		}

		public string ErrorMessageResourceName {
			get { throw new NotImplementedException(); }
		}

		public Func<object, object> CustomStateProvider {
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public void SetErrorMessage(string message) {
			throw new NotImplementedException();
		}

		public void SetErrorMessage(Type errorMessageResourceType, string resourceName) {
			throw new NotImplementedException();
		}

		public void SetErrorMessage(Expression<Func<string>> resourceSelector) {
			throw new NotImplementedException();
		}

		public Attribute ToAttribute() {
			return attribute;
		}
	}
}