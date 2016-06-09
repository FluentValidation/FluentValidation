using System;
using System.Linq;
using System.Reflection;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace FluentValidation.MvcCore
{
    /// <summary>
    /// Extensions for configuring MVC with FluentValidation using an <see cref="IMvcBuilder"/>.
    /// </summary>
    public static class FluentValidationMvcCoreBuilderExtensions
    {
        /// <summary>
        /// Registers MVC FluentValidation.
        /// </summary>
        /// <param name="builder">The <see cref="IMvcBuilder"/>.</param>
        /// <param name="assemblyToScan">assembly to scann for validators.</param>
        /// <param name="additionalAssembliesToScann">Additional assemblies to scann for validators.</param>
        /// <returns>The <see cref="IMvcBuilder"/>.</returns>
        public static IMvcBuilder AddFluentValidation(this IMvcBuilder builder, Assembly assemblyToScan, params Assembly[] additionalAssembliesToScann)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (assemblyToScan == null) 
            {
                throw new ArgumentNullException(nameof(assemblyToScan));
            }

            AddFluentValidationServices(builder.Services, new[] { assemblyToScan }.Concat(additionalAssembliesToScann));
            return builder;
        }

        private static void AddFluentValidationServices(IServiceCollection services, IEnumerable<Assembly> assembliesToScann)
        {
             services.TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<MvcOptions>, FluentValidationMvcOptionsSetup>());
             services.TryAddSingleton<IValidatorFactory, FluentValidationValidatorFactory>();
             // add all validators..
             var openGenericType = typeof(IValidator<>);
            
             var validators = from type in assembliesToScann.SelectMany(assembly => assembly.GetTypes())
						let interfaces = type.GetInterfaces()
						let genericInterfaces = interfaces.Where(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == openGenericType)
						let matchingInterface = genericInterfaces.FirstOrDefault()
						where matchingInterface != null
						select new { InterfaceType = matchingInterface,  ValidatorType = type };

             foreach (var validator in validators) 
             {
                services.TryAddTransient(validator.InterfaceType, validator.ValidatorType);
             }
        }
        
    }
}