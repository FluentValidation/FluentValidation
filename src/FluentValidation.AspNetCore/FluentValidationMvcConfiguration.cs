namespace FluentValidation.AspNetCore {
	using System;
	using System.Collections.Generic;
	using System.Reflection;

	/// <summary>
	/// FluentValidation asp.net core configuration
	/// </summary>
	public class FluentValidationMvcConfiguration {
		/// <summary>
		/// The type of validator factory to use. Uses the ServiceProviderValidatorFactory by default.
		/// </summary>
		public Type ValidatorFactoryType { get; set; }

		/// <summary>
		/// The validator factory to use. Uses the ServiceProviderValidatorFactory by default. 
		/// </summary>
		public IValidatorFactory ValidatorFactory { get; set; }
		/// <summary>
		/// Whether to remove all of MVC's default validator providers when registering FluentValidation. False by default. 
		/// </summary>
		[Obsolete]
		public bool ClearValidatorProviders { get; set; }

		/// <summary>
		/// Whether to run MVC's default validation process (including DataAnnotations) after FluentValidation is executed. True by default. 
		/// </summary>
		public bool RunDefaultMvcValidationAfterFluentValidationExecutes { get; set; } = true;

		/// <summary>
		/// Enables or disables localization support within FluentValidation
		/// </summary>
		public bool LocalizationEnabled {
			get => ValidatorOptions.LanguageManager.Enabled;
			set => ValidatorOptions.LanguageManager.Enabled = value;
		}

		/// <summary>
		/// Whether or not child properties should be implicitly validated if a matching validator can be found. By default this is false, and you should wire up child validators using SetValidator.
		/// </summary>
		public bool ImplicitlyValidateChildProperties { get; set; }

		internal bool ClientsideEnabled = true;
		internal Action<FluentValidationClientModelValidatorProvider> ClientsideConfig = x => {};
		internal List<Assembly> AssembliesToRegister { get; } = new List<Assembly>();

		/// <summary>
		/// Registers all validators derived from AbstractValidator within the assembly containing the specified type
		/// </summary>
		public FluentValidationMvcConfiguration RegisterValidatorsFromAssemblyContaining<T>() {
			return RegisterValidatorsFromAssemblyContaining(typeof(T));
		}

		/// <summary>
		/// Registers all validators derived from AbstractValidator within the assembly containing the specified type
		/// </summary>
		public FluentValidationMvcConfiguration RegisterValidatorsFromAssemblyContaining(Type type) {
			return RegisterValidatorsFromAssembly(type.GetTypeInfo().Assembly);
		}

		/// <summary>
		/// Registers all validators derived from AbstractValidator within the specified assembly
		/// </summary>
		public FluentValidationMvcConfiguration RegisterValidatorsFromAssembly(Assembly assembly) {
			ValidatorFactoryType = typeof(ServiceProviderValidatorFactory);
			AssembliesToRegister.Add(assembly);
			return this;
		}

		/// <summary>
		/// Configures clientside vlaidation support
		/// </summary>
		/// <param name="clientsideConfig"></param>
		/// <param name="enabled">Whether clientisde validation integration is enabled</param>
		/// <returns></returns>
		public FluentValidationMvcConfiguration ConfigureClientsideValidation(Action<FluentValidationClientModelValidatorProvider> clientsideConfig=null, bool enabled=true) {
			if (clientsideConfig != null) {
				ClientsideConfig = clientsideConfig;
			}
			ClientsideEnabled = enabled;
			return this;
		}
		
	}
}