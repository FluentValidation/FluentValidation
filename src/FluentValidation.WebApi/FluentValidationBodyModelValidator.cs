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
// The latest version of this file can be found at https://github.com/jeremyskinner/FluentValidation
#endregion

namespace FluentValidation.WebApi
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;
	using System.Globalization;
	using System.Linq;
	using System.Net.Http.Formatting;
	using System.Runtime.CompilerServices;
	using System.Web.Http.Controllers;
	using System.Web.Http.Metadata;
	using System.Web.Http.ModelBinding;
	using System.Web.Http.Validation;

	public class FluentValidationBodyModelValidator : DefaultBodyModelValidator {
		protected override bool ValidateNodeAndChildren(ModelMetadata metadata, BodyModelValidatorContext validationContext, object container, IEnumerable<ModelValidator> validators) {
			bool isValid = base.ValidateNodeAndChildren(metadata, validationContext, container, validators);

			var model = GetModel(metadata);

			if (!isValid && model != null && !HasAlreadyBeenValidated(validationContext, model)) {
				// default impl skips validating root node if any children fail, so we explicitly validate it in this scenario
				var rootModelValidators = validationContext.ActionContext.GetValidators(metadata);
				var rootIsValid = ShallowValidate(metadata, validationContext, container, rootModelValidators);
				return rootIsValid && isValid;
			}
			return isValid;
		}

		protected override bool ShallowValidate(ModelMetadata metadata, BodyModelValidatorContext validationContext, object container, IEnumerable<ModelValidator> validators) {
			var valid = base.ShallowValidate(metadata, validationContext, container, validators);

			var model = GetModel(metadata);

			if (model != null && validators.Any(x => x is FluentValidationModelValidator)) {
				HashSet<object> progress = GetProgress(validationContext);
				progress.Add(model);
			}

			return valid;
		}

		private object GetModel(ModelMetadata meta) {

			object model = null;

			try
			{
				model = meta.Model;
			}
			catch { }

			return model;
		}

		private bool HasAlreadyBeenValidated(BodyModelValidatorContext validationContext, object model) {
			return GetProgress(validationContext).Contains(model);
		}

		private HashSet<object> GetProgress(BodyModelValidatorContext context) {
			HashSet<object> progress;

			if (!context.ActionContext.Request.Properties.ContainsKey("_FV_Progress")) {
				context.ActionContext.Request.Properties["_FV_Progress"] = progress = new HashSet<object>();
			}
			else {
				progress = (HashSet<object>) context.ActionContext.Request.Properties["_FV_Progress"];
			}

			return progress;
		}
	}
}
