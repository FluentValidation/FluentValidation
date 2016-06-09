using System;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FluentValidation.MvcCore
{
    /// <summary>
    /// Sets up default options for <see cref="MvcOptions"/>.
    /// </summary>
    public class FluentValidationMvcOptionsSetup : ConfigureOptions<MvcOptions>
    {
        public FluentValidationMvcOptionsSetup(IServiceProvider serviceProvider)
            : base(options => ConfigureMvc(options, serviceProvider))
        {
        }

        public static void ConfigureMvc(MvcOptions options, IServiceProvider serviceProvider)
        {
           var validatorFactory = serviceProvider.GetService<IValidatorFactory>();
            
            options.ModelValidatorProviders.Add(new FluentValidationModelValidatorProvider(validatorFactory));
        }
    }
}
