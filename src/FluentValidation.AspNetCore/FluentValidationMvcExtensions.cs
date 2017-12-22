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

namespace FluentValidation.AspNetCore {
	using System;
	using System.Reflection;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.ModelBinding;
	using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Options;
	using FluentValidation;
	using System.Collections.Generic;
	using Microsoft.AspNetCore.Http;
	using Microsoft.Extensions.DependencyInjection.Extensions;

	public static class FluentValidationMvcExtensions {
		/// <summary>
		///     Adds Fluent Validation services to the specified
		///     <see cref="T:Microsoft.Extensions.DependencyInjection.IMvcBuilder" />.
		/// </summary>
		/// <returns>
		///     An <see cref="T:Microsoft.Extensions.DependencyInjection.IMvcCoreBuilder" /> that can be used to further configure the
		///     MVC services.
		/// </returns>
		public static IMvcCoreBuilder AddFluentValidation(this IMvcCoreBuilder mvcBuilder, Action<FluentValidationMvcConfiguration> configurationExpression=null) {
			var expr = configurationExpression ?? delegate { };
			var config = new FluentValidationMvcConfiguration();

			expr(config);

			if (config.AssembliesToRegister.Count > 0)
			{
				RegisterTypes(config.AssembliesToRegister, mvcBuilder.Services);
			}

			RegisterServices(mvcBuilder.Services, config);
			// clear all model validation providers since fluent validation will be handling everything

#pragma warning disable CS0612 // Type or member is obsolete
			if (config.ClearValidatorProviders) {
#pragma warning restore CS0612 // Type or member is obsolete
				mvcBuilder.AddMvcOptions(
					options => {
						options.ModelValidatorProviders.Clear();
					});
			}

			mvcBuilder.AddMvcOptions(options => {
				options.ModelMetadataDetailsProviders.Add(new FluentValidationBindingMetadataProvider());
				options.ModelValidatorProviders.Insert(0, new FluentValidationModelValidatorProvider(config.ImplicitlyValidateChildProperties));
			});

			return mvcBuilder;
		}

		private static void RegisterServices(IServiceCollection services, FluentValidationMvcConfiguration config) {
			if (config.ValidatorFactory != null) {
				// Allow user to register their own IValidatorFactory instance, before falling back to try resolving by Type. 
				var factory = config.ValidatorFactory;
				services.Add(ServiceDescriptor.Transient(s => factory));
			}
			else {
				services.Add(ServiceDescriptor.Transient(typeof(IValidatorFactory), config.ValidatorFactoryType ?? typeof(ServiceProviderValidatorFactory)));
			}

			services.Add(ServiceDescriptor.Singleton<IObjectModelValidator, FluentValidationObjectModelValidator>(s => {
				var options = s.GetRequiredService<IOptions<MvcOptions>>().Value;
				var metadataProvider = s.GetRequiredService<IModelMetadataProvider>();
				return new FluentValidationObjectModelValidator(metadataProvider, options.ModelValidatorProviders, config.RunDefaultMvcValidationAfterFluentValidationExecutes, config.ImplicitlyValidateChildProperties);
			}));


			if (config.ClientsideEnabled)
			{
				services.TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<MvcViewOptions>, FluentValidationViewOptionsSetup>(s =>
					{
						return new FluentValidationViewOptionsSetup(config.ClientsideConfig, s.GetService<IHttpContextAccessor>());
					}));
			}


		}

		/// <summary>
		///     Adds Fluent Validation services to the specified
		///     <see cref="T:Microsoft.Extensions.DependencyInjection.IMvcBuilder" />.
		/// </summary>
		/// <returns>
		///     An <see cref="T:Microsoft.Extensions.DependencyInjection.IMvcBuilder" /> that can be used to further configure the
		///     MVC services.
		/// </returns>
		public static IMvcBuilder AddFluentValidation(this IMvcBuilder mvcBuilder, Action<FluentValidationMvcConfiguration> configurationExpression = null) {
			// add all IValidator to MVC's service provider

			var expr = configurationExpression ?? delegate { };
			var config = new FluentValidationMvcConfiguration();

			expr(config);

			if (config.AssembliesToRegister.Count > 0) {
				RegisterTypes(config.AssembliesToRegister, mvcBuilder.Services);
			}

			RegisterServices(mvcBuilder.Services, config);

			// clear all model validation providers since fluent validation will be handling everything
#pragma warning disable CS0612 // Type or member is obsolete
			if (config.ClearValidatorProviders) {
#pragma warning restore CS0612 // Type or member is obsolete
				mvcBuilder.AddMvcOptions(
					options => {
						options.ModelValidatorProviders.Clear();
					});
			}
			mvcBuilder.AddMvcOptions(options => {
				options.ModelMetadataDetailsProviders.Add(new FluentValidationBindingMetadataProvider());
				options.ModelValidatorProviders.Insert(0, new FluentValidationModelValidatorProvider(config.ImplicitlyValidateChildProperties));
			});

			return mvcBuilder;
		}

		private static void RegisterTypes(IEnumerable<Assembly> assembliesToRegister, IServiceCollection services) {
			AssemblyScanner.FindValidatorsInAssemblies(assembliesToRegister).ForEach(pair => {
				services.Add(ServiceDescriptor.Transient(pair.InterfaceType, pair.ValidatorType));
			});
		}
	}

	internal class FluentValidationViewOptionsSetup : IConfigureOptions<MvcViewOptions> {
		private readonly Action<FluentValidationClientModelValidatorProvider> _action;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public FluentValidationViewOptionsSetup(Action<FluentValidationClientModelValidatorProvider> action, IHttpContextAccessor httpContextAccessor) {
			_action = action;
			_httpContextAccessor = httpContextAccessor;
		}

		public void Configure(MvcViewOptions options) {
			var provider = new FluentValidationClientModelValidatorProvider(_httpContextAccessor);
			_action(provider);
			options.ClientModelValidatorProviders.Add(provider);
		}
	}
}