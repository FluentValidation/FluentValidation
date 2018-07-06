namespace FluentValidation
{
	using System;
	using System.Reflection;

	/// <summary>
	/// Gets validators for method parameters.
	/// </summary>
	[Obsolete]
	public interface IParameterValidatorFactory {
		/// <summary>
		/// Gets a validator for <paramref name="parameterInfo"/>.
		/// </summary>
		/// <param name="parameterInfo">The <see cref="ParameterInfo"/> instance to get a validator for.</param>
		/// <returns>Created <see cref="IValidator"/> instance; <see langword="null"/> if a validator cannot be
		/// created.</returns>
		IValidator GetValidator(ParameterInfo parameterInfo);
	}
}