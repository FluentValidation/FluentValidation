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
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.ModelBinding;
	using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Options;
	using FluentValidation;
	using System.Linq;
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
		public static IMvcCoreBuilder AddFluentValidation(this IMvcCoreBuilder mvcBuilder, Action<FluentValidationMvcConfiguration> configurationExpression = null) {
			mvcBuilder.Services.AddFluentValidation(configurationExpression);
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
			mvcBuilder.Services.AddFluentValidation(configurationExpression);
			return mvcBuilder;
		}

		/// <summary>
		///     Adds Fluent Validation services to the specified
		///     <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
		/// </summary>
		/// <returns>
		///     A reference to this instance after the operation has completed.
		/// </returns>
		public static IServiceCollection AddFluentValidation(this IServiceCollection services, Action<FluentValidationMvcConfiguration> configurationExpression = null) {
			var config = new FluentValidationMvcConfiguration(ValidatorOptions.Global);
			configurationExpression?.Invoke(config);

			services.AddValidatorsFromAssemblies(config.AssembliesToRegister, config.ServiceLifetime, config.TypeFilter, config.IncludeInternalValidatorTypes);
			services.AddSingleton(config.ValidatorOptions);

			if (config.ValidatorFactory != null) {
				// Allow user to register their own IValidatorFactory instance, before falling back to try resolving by Type.
				var factory = config.ValidatorFactory;
				services.Add(ServiceDescriptor.Scoped(s => factory));
			}
			else {
				services.Add(ServiceDescriptor.Scoped(typeof(IValidatorFactory), config.ValidatorFactoryType ?? typeof(ServiceProviderValidatorFactory)));
			}

			if (config.AutomaticValidationEnabled) {
				services.Add(ServiceDescriptor.Singleton<IObjectModelValidator, FluentValidationObjectModelValidator>(s => {
					var options = s.GetRequiredService<IOptions<MvcOptions>>().Value;
					var metadataProvider = s.GetRequiredService<IModelMetadataProvider>();
					return new FluentValidationObjectModelValidator(metadataProvider, options.ModelValidatorProviders, !config.DisableDataAnnotationsValidation);
				}));

				services.Configure<MvcOptions>(options => {
					// Check if the providers have already been added.
					// We shouldn't have to do this, but there's a bug in the ASP.NET Core integration
					// testing components that can cause Configureservices to be called multple times
					// meaning we end up with duplicates.

					if (!options.ModelMetadataDetailsProviders.Any(x => x is FluentValidationBindingMetadataProvider)) {
						options.ModelMetadataDetailsProviders.Add(new FluentValidationBindingMetadataProvider());
					}

					if (!options.ModelValidatorProviders.Any(x => x is FluentValidationModelValidatorProvider)) {
						options.ModelValidatorProviders.Insert(0, new FluentValidationModelValidatorProvider(
							config.ImplicitlyValidateChildProperties,
							config.ImplicitlyValidateRootCollectionElements));
					}
				});
			}

			if (config.ClientsideEnabled) {
				// Clientside validation requires access to the HttpContext, but MVC's clientside API does not provide it,
				// so we need to inject the HttpContextAccessor instead.
				// This is not registered by default, so add it in if the user hasn't done so.
				services.AddHttpContextAccessor();

				services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<MvcViewOptions>, FluentValidationViewOptionsSetup>(s => {
					return new FluentValidationViewOptionsSetup(config.ClientsideConfig, s.GetService<IHttpContextAccessor>());
				}));
			}

			return services;
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
