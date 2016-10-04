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

			mvcBuilder.AddMvcOptions(
				options => {
					options.ModelValidatorProviders.Clear();

				});

			return mvcBuilder;
		}

		private static void RegisterServices(IServiceCollection services, FluentValidationMvcConfiguration config) {
			services.Add(ServiceDescriptor.Singleton(typeof(IValidatorFactory), config.ValidatorFactoryType ?? typeof(ServiceProviderValidatorFactory)));

			services.Add(ServiceDescriptor.Singleton<IObjectModelValidator, FluentValidationObjectModelValidator>(s => {
				var options = s.GetRequiredService<IOptions<MvcOptions>>().Value;
				var metadataProvider = s.GetRequiredService<IModelMetadataProvider>();
				return new FluentValidationObjectModelValidator(metadataProvider, options.ModelValidatorProviders, s.GetRequiredService<IValidatorFactory>());
			}));


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
			mvcBuilder.AddMvcOptions(
			    options => {
			        options.ModelValidatorProviders.Clear();

                });

            return mvcBuilder;
		}

		private static void RegisterTypes(IEnumerable<Assembly> assembliesToRegister, IServiceCollection services) {
			var openGenericType = typeof(IValidator<>);

			var query = from a in assembliesToRegister.Distinct()
                        from type in a.GetTypes()
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

    public class FluentValidationMvcConfiguration {
	    public Type ValidatorFactoryType { get; set; }
	    internal List<Assembly> AssembliesToRegister { get; } = new List<Assembly>();

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
    }
}