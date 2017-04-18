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
	using System.Linq;
	using System.Collections.Generic;
	using Microsoft.Extensions.DependencyInjection.Extensions;
	using Resources;

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

			if (config.ClearValidatorProviders) {
				mvcBuilder.AddMvcOptions(
					options => {
						options.ModelValidatorProviders.Clear();
					});
			}
			return mvcBuilder;
		}

		private static void RegisterServices(IServiceCollection services, FluentValidationMvcConfiguration config) {
			if (config.ValidatorFactory != null) {
				// Allow user to register their own IValidatorFactory instance, before falling back to try resolving by Type. 
				var factory = config.ValidatorFactory;
				services.Add(ServiceDescriptor.Scoped(s => factory));
			}
			else {
				services.Add(ServiceDescriptor.Scoped(typeof(IValidatorFactory), config.ValidatorFactoryType ?? typeof(ServiceProviderValidatorFactory)));
			}

			services.Add(ServiceDescriptor.Singleton<IObjectModelValidator, FluentValidationObjectModelValidator>(s => {
				var options = s.GetRequiredService<IOptions<MvcOptions>>().Value;
				var metadataProvider = s.GetRequiredService<IModelMetadataProvider>();
				return new FluentValidationObjectModelValidator(metadataProvider, options.ModelValidatorProviders);
			}));

			if (config.ClientsideEnabled)
			{
				services.TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<MvcViewOptions>, FluentValidationViewOptionsSetup>(s =>
					{
						return new FluentValidationViewOptionsSetup(s.GetService<IValidatorFactory>(), config.ClientsideConfig);
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
			if (config.ClearValidatorProviders) {
				mvcBuilder.AddMvcOptions(
					options => {
						options.ModelValidatorProviders.Clear();
					});
			}
			return mvcBuilder;
		}

		private static void RegisterTypes(IEnumerable<Assembly> assembliesToRegister, IServiceCollection services) {
			var openGenericType = typeof(IValidator<>);

			var query = from a in assembliesToRegister.Distinct()
                        from type in a.GetTypes().Where(c => !(c.GetTypeInfo().IsAbstract || c.GetTypeInfo().IsGenericTypeDefinition))
						let interfaces = type.GetInterfaces()
						let genericInterfaces = interfaces.Where(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == openGenericType)
						let matchingInterface = genericInterfaces.FirstOrDefault()
						where matchingInterface != null
						select new { matchingInterface, type };

			foreach (var pair in query) {
				services.Add(ServiceDescriptor.Transient(pair.matchingInterface, pair.type));
			}
		}
	}

	internal class FluentValidationViewOptionsSetup : IConfigureOptions<MvcViewOptions>
	{
		private readonly IValidatorFactory _factory;
		private readonly Action<FluentValidationClientModelValidatorProvider> _action;

		public FluentValidationViewOptionsSetup(IValidatorFactory factory, Action<FluentValidationClientModelValidatorProvider> action)
		{
			_factory = factory;
			_action = action;
		}

		public void Configure(MvcViewOptions options)
		{
			var provider = new FluentValidationClientModelValidatorProvider(_factory);
			_action(provider);
			options.ClientModelValidatorProviders.Add(provider);
		}
	}

	public class FluentValidationMvcConfiguration {
	    public Type ValidatorFactoryType { get; set; }
		public IValidatorFactory ValidatorFactory { get; set; }
	    internal List<Assembly> AssembliesToRegister { get; } = new List<Assembly>();
		public bool ClearValidatorProviders { get; set; }

		public bool LocalizationEnabled {
			get => ValidatorOptions.LanguageManager.Enabled;
			set => ValidatorOptions.LanguageManager.Enabled = value;
		}

		internal bool ClientsideEnabled = true;
	    internal Action<FluentValidationClientModelValidatorProvider> ClientsideConfig = x => {};

	    public FluentValidationMvcConfiguration RegisterValidatorsFromAssemblyContaining<T>() {
		    return RegisterValidatorsFromAssemblyContaining(typeof(T));
	    }

	    public FluentValidationMvcConfiguration RegisterValidatorsFromAssemblyContaining(Type type) {
		    return RegisterValidatorsFromAssembly(type.GetTypeInfo().Assembly);
	    }

	    public FluentValidationMvcConfiguration RegisterValidatorsFromAssembly(Assembly assembly) {
		    ValidatorFactoryType = typeof(ServiceProviderValidatorFactory);
		    AssembliesToRegister.Add(assembly);
		    return this;
	    }

	    public FluentValidationMvcConfiguration ConfigureClientsideValidation(Action<FluentValidationClientModelValidatorProvider> clientsideConfig=null, bool enabled=true) {
		    if (clientsideConfig != null) {
			    ClientsideConfig = clientsideConfig;
		    }
		    ClientsideEnabled = enabled;
		    return this;
	    }
		
	}
}