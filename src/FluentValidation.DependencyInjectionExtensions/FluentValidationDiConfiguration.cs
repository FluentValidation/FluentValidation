
namespace FluentValidation {
	using System;
	using Microsoft.Extensions.DependencyInjection;

	public class FluentValidationDiConfiguration {

		public FluentValidationDiConfiguration(ValidatorConfiguration validatorOptions) {
			ValidatorOptions = validatorOptions;
		}

		/// <summary>
		/// Options that are used to configure all validators.
		/// </summary>
		public ValidatorConfiguration ValidatorOptions { get; }

		/// <summary>
		/// The type of validator factory to use. Uses the ServiceProviderValidatorFactory by default.
		/// </summary>
		public Type ValidatorFactoryType { get; set; }

		/// <summary>
		/// The validator factory to use. Uses the ServiceProviderValidatorFactory by default.
		/// </summary>
		public IValidatorFactory ValidatorFactory { get; set; }

		/// <summary>
		/// Enables or disables localization support within FluentValidation
		/// </summary>
		public bool LocalizationEnabled {
			get => ValidatorOptions.LanguageManager.Enabled;
			set => ValidatorOptions.LanguageManager.Enabled = value;
		}

		/// <summary>
		/// Filter function to exclude types from assembly scanning registration
		/// </summary>
		public Func<AssemblyScanner.AssemblyScanResult, bool> TypeFilter { get; set; }

		/// <summary>
		/// Sets the default ServiceLifetime for the validator and factory registrations
		/// </summary>
		public ServiceLifetime ServiceLifetime { get; set; } = ServiceLifetime.Transient;
	}
}
