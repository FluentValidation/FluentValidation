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

namespace FluentValidation.AspNetCore {
	using System;
	using FluentValidation.Internal;
	using System.Reflection;
	using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
	using Microsoft.Extensions.DependencyInjection;
	using System.Linq;

	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
	public class CustomizeValidatorAttribute : Attribute {

		/// <summary>
		/// Specifies the ruleset which should be used when executing this validator.
		/// This can be a comma separated list of rulesets. The string "*" can be used to indicate all rulesets.
		/// The string "default" can be used to specify those rules not in an explict ruleset.
		/// </summary>
		public string RuleSet { get; set; }

		/// <summary>
		/// Specifies a whitelist of properties that should be validated, as a comma-separated list.
		/// </summary>
		public string Properties { get; set; }

		/// <summary>
		/// Specifies an interceptor that can be used to customize the validation process.
		/// </summary>
		public Type Interceptor { get; set; }

		/// <summary>
		/// Indicates whether this model should skip being validated. The default is false.
		/// </summary>
		public bool Skip { get; set; }

		/// <summary>
		/// Builds a validator selector from the options specified in the attribute's properties.
		/// </summary>
		/// <param name="mvContext"></param>
		public IValidatorSelector ToValidatorSelector(ModelValidationContext mvContext) {
			IValidatorSelector selector;

			if (!string.IsNullOrEmpty(RuleSet)) {
				var rulesets = RuleSet.Split(',', ';')
					.Select(x => x.Trim())
					.ToArray();
				selector = CreateRulesetValidatorSelector(mvContext, rulesets);
			}
			else if (!string.IsNullOrEmpty(Properties)) {
				var properties = Properties.Split(',', ';')
					.Select(x => x.Trim())
					.ToArray();
				selector = CreateMemberNameValidatorSelector(mvContext, properties);
			}
			else {
				selector = CreateDefaultValidatorSelector(mvContext);
			}

			return selector;

		}

		protected virtual IValidatorSelector CreateRulesetValidatorSelector(ModelValidationContext mvContext, string[] ruleSets) {
			var cfg = mvContext.ActionContext.HttpContext.RequestServices.GetRequiredService<ValidatorConfiguration>();
			return cfg.ValidatorSelectors.RulesetValidatorSelectorFactory(ruleSets);
		}

		protected virtual IValidatorSelector CreateMemberNameValidatorSelector(ModelValidationContext mvContext, string[] properties) {
			var cfg = mvContext.ActionContext.HttpContext.RequestServices.GetValidatorConfiguration();
			return cfg.ValidatorSelectors.MemberNameValidatorSelectorFactory(properties);
		}

		protected virtual IValidatorSelector CreateDefaultValidatorSelector(ModelValidationContext mvContext) {
			var cfg = mvContext.ActionContext.HttpContext.RequestServices.GetValidatorConfiguration();
			return cfg.ValidatorSelectors.DefaultValidatorSelectorFactory();
		}

		public IValidatorInterceptor GetInterceptor() {
			if (Interceptor == null) return null;

			if (!typeof(IValidatorInterceptor).IsAssignableFrom(Interceptor)) {
				if (typeof(IActionContextValidatorInterceptor).IsAssignableFrom(Interceptor)) return null;
				throw new InvalidOperationException("Type {0} is not an IValidatorInterceptor. The Interceptor property of CustomizeValidatorAttribute must implement IValidatorInterceptor.");
			}

			var instance = Activator.CreateInstance(Interceptor) as IValidatorInterceptor;

			if (instance == null) {
				throw new InvalidOperationException("Type {0} is not an IValidatorInterceptor. The Interceptor property of CustomizeValidatorAttribute must implement IValidatorInterceptor.");
			}

			return instance;
		}

		[Obsolete]
		internal IActionContextValidatorInterceptor GetActionContextInterceptor() {
			if (Interceptor == null) return null;

			if (!typeof(IActionContextValidatorInterceptor).IsAssignableFrom(Interceptor)) {
				if (typeof(IValidatorInterceptor).IsAssignableFrom(Interceptor)) return null;
				throw new InvalidOperationException("Type {0} is not an IActionContextValidatorInterceptor. The Interceptor property of CustomizeValidatorAttribute must implement IActionContextValidatorInterceptor.");
			}

			var instance = Activator.CreateInstance(Interceptor) as IActionContextValidatorInterceptor;

			if (instance == null) {
				throw new InvalidOperationException("Type {0} is not an IActionContextValidatorInterceptor. The Interceptor property of CustomizeValidatorAttribute must implement IActionContextValidatorInterceptor.");
			}

			return instance;
		}
	}
}
