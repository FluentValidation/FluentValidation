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

#endregion License

namespace FluentValidation.Attributes
{
	using Internal;
	using System;
	using System.Reflection;

	/// <summary>
	/// Implementation of <see cref="IValidatorFactory"/> and <see cref="IParameterValidatorFactory"/> that looks for
	/// the <see cref="ValidatorAttribute"/> instance on the specified <see cref="Type"/> or
	/// <see cref="ParameterInfo"/> in order to provide the validator instance.
	/// </summary>
//	[Obsolete("The AttributedValidatorFactory is no longer recommended. If you're using ASP.NET Core, you should switch to using the Service Provider infrastructure for registering validators. For MVC5, WebApi2 or non-web scenarios, please consider using an IoC container instead.")]
	public class AttributedValidatorFactory : IValidatorFactory, IParameterValidatorFactory
	{
		private readonly Func<Type, IValidator> instanceFactory;

		private readonly InstanceCache cache = new InstanceCache();

		/// <summary>
		/// Creates an instance of <see cref="AttributedValidatorFactory"/>.
		/// </summary>
		public AttributedValidatorFactory()
		{
			instanceFactory = null;
		}

		/// <summary>
		/// Creates an instance of <see cref="AttributedValidatorFactory"/> with the supplied instance factory delegate
		/// used for creation of <see cref="IValidator"/> instances.
		/// </summary>
		/// <param name="instanceFactory">The <see cref="IValidator"/> instance factory delegate.</param>
		public AttributedValidatorFactory(Func<Type, IValidator> instanceFactory)
		{
			this.instanceFactory = instanceFactory;
		}

		/// <summary>
		/// Gets a validator for the appropriate type.
		/// </summary>
		public IValidator<T> GetValidator<T>()
		{
			return (IValidator<T>)GetValidator(typeof(T));
		}

		/// <summary>
		/// Gets a validator for the appropriate type.
		/// </summary>
		/// <returns>Created <see cref="IValidator"/> instance; <see langword="null"/> if a validator cannot be
		/// created.</returns>
		public virtual IValidator GetValidator(Type type)
		{
			if (type == null)
			{
				return null;
			}

			var attribute = type.GetTypeInfo().GetCustomAttribute<ValidatorAttribute>();

			return GetValidator(attribute);
		}

		/// <summary>
		/// Gets a validator for <paramref name="parameterInfo"/>.
		/// </summary>
		/// <param name="parameterInfo">The <see cref="ParameterInfo"/> instance to get a validator for.</param>
		/// <returns>Created <see cref="IValidator"/> instance; <see langword="null"/> if a validator cannot be
		/// created.</returns>
		public virtual IValidator GetValidator(ParameterInfo parameterInfo)
		{
			if (parameterInfo == null)
			{
				return null;
			}

			var attribute = parameterInfo.GetCustomAttribute<ValidatorAttribute>();

			return GetValidator(attribute);
		}

		private IValidator GetValidator(ValidatorAttribute attribute)
		{
			if (attribute == null || attribute.ValidatorType == null)
			{
				return null;
			}

			var validator = instanceFactory == null
				? cache.GetOrCreateInstance(attribute.ValidatorType)
				: cache.GetOrCreateInstance(attribute.ValidatorType, instanceFactory);

			return validator as IValidator;
		}
	}
}