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
	using System.Reflection;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.ModelBinding;
	using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Options;
	using FluentValidation;
	using System.Collections.Generic;
	using System.Linq;
	using Microsoft.AspNetCore.Http;
	using Microsoft.Extensions.DependencyInjection.Extensions;
	using Microsoft.Extensions.Logging;

	public static class FluentValidationMvcExtensions {
		/// <summary>
		///     Adds Fluent Validation services to the specified
		///     <see cref="T:Microsoft.Extensions.DependencyInjection.IMvcBuilder" />.
		/// </summary>
		/// <returns>
		///     An <see cref="T:Microsoft.Extensions.DependencyInjection.IMvcCoreBuilder" /> that can be used to further configure the
		///     MVC services.
		/// </returns>
		public static IMvcCoreBuilder AddFluentValidation(this IMvcCoreBuilder mvcBuilder, Action<FluentValidationMvcConfiguration> configurationExpression = null) {
			var config = new FluentValidationMvcConfiguration(ValidatorOptions.Global);
			configurationExpression?.Invoke(config);

			mvcBuilder.Services.AddValidatorsFromAssemblies(config.AssembliesToRegister);

			RegisterServices(mvcBuilder.Services, config);

			mvcBuilder.AddMvcOptions(options => {
				// Check if the providers have already been added.
				// We shouldn't have to do this, but there's a bug in the ASP.NET Core integration
				// testing components that can cause Configureservices to be called multple times
				// meaning we end up with duplicates.

				if (!options.ModelMetadataDetailsProviders.Any(x => x is FluentValidationBindingMetadataProvider)) {
					options.ModelMetadataDetailsProviders.Add(new FluentValidationBindingMetadataProvider());
				}

				if (!options.ModelValidatorProviders.Any(x => x is FluentValidationModelValidatorProvider)) {
					options.ModelValidatorProviders.Insert(0, new FluentValidationModelValidatorProvider(config.ImplicitlyValidateChildProperties));
				}
			});

			return mvcBuilder;
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
			var config = new FluentValidationMvcConfiguration(ValidatorOptions.Global);
			configurationExpression?.Invoke(config);

			mvcBuilder.Services.AddValidatorsFromAssemblies(config.AssembliesToRegister);

			RegisterServices(mvcBuilder.Services, config);

			mvcBuilder.AddMvcOptions(options => {
				options.ModelMetadataDetailsProviders.Add(new FluentValidationBindingMetadataProvider());
				options.ModelValidatorProviders.Insert(0, new FluentValidationModelValidatorProvider(config.ImplicitlyValidateChildProperties));
			});

			return mvcBuilder;
		}

		private static void RegisterServices(IServiceCollection services, FluentValidationMvcConfiguration config) {
			services.AddSingleton(config.ValidatorOptions);

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
				return new FluentValidationObjectModelValidator(metadataProvider, options.ModelValidatorProviders, config.RunDefaultMvcValidationAfterFluentValidationExecutes);
			}));

			if (config.ClientsideEnabled) {
				// Clientside validation requires access to the HttpContext, but MVC's clientside API does not provide it,
				// so we need to inject the HttpContextAccessor instead.
				// This is not registered by default, so add it in if the user hasn't done so.
				services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

				services.TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<MvcViewOptions>, FluentValidationViewOptionsSetup>(s => {
					return new FluentValidationViewOptionsSetup(config.ClientsideConfig, s.GetService<IHttpContextAccessor>());
				}));
			}
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
