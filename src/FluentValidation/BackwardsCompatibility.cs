#region License
// Copyright 2008-2009 Jeremy Skinner (http://www.jeremyskinner.co.uk)
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
// The latest version of this file can be found at http://www.codeplex.com/FluentValidation
#endregion

#region Nothing to see here...backwards compatibility hackery to follow
namespace FluentValidation.Validators {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Attributes;
	using Internal;
	using Results;

	[Obsolete("Generic versions of IPropertyValidator have been deprecated. Custom validators should inherit from the abstract base class FluentValidation.Validators.PropertyValidator")]
// ReSharper disable UnusedTypeParameter
	public interface IPropertyValidator<T> {
// ReSharper restore UnusedTypeParameter
	}

	[Obsolete("Generic versions of IPropertyValidator have been deprecated. Custom validators should inherit from the abstract base class FluentValidation.Validators.PropertyValidator")]
	public interface IPropertyValidator<TInstance, TProperty> : IPropertyValidator<TInstance> {
		PropertyValidatorResult Validate(PropertyValidatorContext<TInstance, TProperty> context);
	}

	[Obsolete("The generic version of PropertyValidatorContext has been deprecated. Please use PropertyValidatorContext instead.")]
	public class PropertyValidatorContext<T, TProperty> {
		readonly Func<T, TProperty> propertyValueFunc;
		bool propertyValueSet;
		TProperty propertyValue;
		readonly IEnumerable<Func<T, object>> customFormatArgs;

		public TProperty PropertyValue {
			get {
				if (!propertyValueSet) {
					propertyValue = propertyValueFunc(Instance);
					propertyValueSet = true;
				}
				return propertyValue;
			}
		}

		public string PropertyDescription { get; private set; }
		public T Instance { get; private set; }
		public string CustomError { get; private set; }

		public PropertyValidatorContext(string propertyDescription, T instance, Func<T, TProperty> propertyValueFunc)
			: this(propertyDescription, instance, propertyValueFunc, null, null) {
		}

		public PropertyValidatorContext(string propertyDescription, T instance, Func<T, TProperty> propertyValueFunc, string customError, IEnumerable<Func<T, object>> customFormatArgs) {
			propertyValueFunc.Guard("propertyValueFunc cannot be null");
			CustomError = customError;
			PropertyDescription = propertyDescription;
			Instance = instance;
			this.customFormatArgs = customFormatArgs;
			this.propertyValueFunc = propertyValueFunc;
		}

		public string GetFormattedErrorMessage(Type type, MessageFormatter formatter) {
			string error = CustomError ?? ValidationMessageAttribute.GetMessage(type);
			if (customFormatArgs != null) {
				formatter.AppendAdditionalArguments(
					customFormatArgs.Select(func => func(Instance)).ToArray()
					);
			}
			return formatter.BuildMessage(error);
		}
	}

	[Obsolete]
	internal class BackwardsCompatibilityValidatorAdaptor<T, TProperty> : PropertyValidator {
		readonly IPropertyValidator<T, TProperty> obsoleteValidator;

		public BackwardsCompatibilityValidatorAdaptor(IPropertyValidator<T, TProperty> obsoleteValidator)
			: base(null as string) {
			this.obsoleteValidator = obsoleteValidator;
		}

		public override IEnumerable<ValidationFailure> Validate(PropertyValidatorContext context) {
			Func<T, TProperty> propertyValueFunc = x => (TProperty)context.PropertyValueFunc(x);
			IEnumerable<Func<T, object>> customMessageFormatArgs = ConvertNewFormatArgsToOldFormatArgs();

			var obsoleteContext = new PropertyValidatorContext<T, TProperty>(context.PropertyDescription, (T)context.Instance, propertyValueFunc, ErrorMessageSource.BuildErrorMessage(), customMessageFormatArgs);

			var oldResult = obsoleteValidator.Validate(obsoleteContext);
			
			var results = new List<ValidationFailure>();

			if (! oldResult.IsValid) {
				var newResult = new ValidationFailure(context.PropertyName, oldResult.Error, context.PropertyValue);
				results.Add(newResult);
			}

			return results;
		}

		IEnumerable<Func<T, object>> ConvertNewFormatArgsToOldFormatArgs() {
			return CustomMessageFormatArguments.Select(func => new Func<T, object>(x => func(x))).ToList();
		}

		protected override bool IsValid(PropertyValidatorContext context) {
			throw new NotImplementedException();
		}
	}
}

namespace FluentValidation {
	using System;
	using Internal;
	using Results;
	using Validators;

	public static class ObsoleteExtensions {
		[Obsolete("This method has a typo in its name. Please use AppendPropertyName instead.")]
		public static MessageFormatter AppendProperyName(this MessageFormatter formatter, string name) {
			return formatter.AppendPropertyName(name);
		}

		[Obsolete("The generic IPropertyValidator interface has been deprecated. Please modify your custom validators to inherit from the abstract base class FluentValidation.Validators.PropertyValidator.")]
		public static IRuleBuilderOptions<T, TProperty> SetValidator<T,TProperty>(this IRuleBuilder<T, TProperty> rule, IPropertyValidator<T,TProperty> validator) {
			return rule.SetValidator(new BackwardsCompatibilityValidatorAdaptor<T, TProperty>(validator));
		}

		public static ValidationResult Validate<T>(this IValidator<T> validator, T instance, IValidatorSelector selector) {
			var context = new ValidationContext<T>(instance, new PropertyChain(), selector);
			return validator.Validate(context);
		}
	}
}

namespace FluentValidation.Attributes {
	using System;
	using Internal;
	using Resources;

	[Obsolete("The ValidationMessage attribute has been deprecated. Your validators should now inherit from the abstract base class FluentValidation.Validators.PropertyValidator and specify the error message in the constructor.")]
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class ValidationMessageAttribute : Attribute {
		public string Key { get; set; }
		public string Message { get; set; }

		public static string GetMessage(Type type) {
			var attribute = (ValidationMessageAttribute)GetCustomAttribute(type, typeof(ValidationMessageAttribute), false);

			if (attribute == null) {
				throw new InvalidOperationException(string.Format("Type '{0}' does does not declare a ValidationMessageAttribute.", type.Name));
			}

			if (string.IsNullOrEmpty(attribute.Key) && string.IsNullOrEmpty(attribute.Message)) {
				throw new InvalidOperationException(string.Format("Type '{0}' declares a ValidationMessageAttribute but neither the Key nor Message are set.", type.Name));
			}

			if (!string.IsNullOrEmpty(attribute.Message)) {
				return attribute.Message;
			}

			var accessor = ResourceHelper.BuildResourceAccessor(attribute.Key, ValidatorOptions.ResourceProviderType ?? typeof(Messages));
			var message = accessor.Accessor();

			if (message == null) {
				throw new InvalidOperationException(string.Format("Could not find a resource key with the name '{0}'.", attribute.Key));
			}

			return message;
		}
	}
}

namespace FluentValidation.Results {
	using System;

	[Obsolete("PropertyValidatorResult is obsolete - custom validators should now inherit from FluentValidation.Validators.PropertyValidator")]
	public class PropertyValidatorResult {
		public PropertyValidatorResult(string error) {
			Error = error;
		}

		public string Error { get; private set; }

		public bool IsValid {
			get { return Error == null; }
		}

		public static PropertyValidatorResult Success() {
			return new PropertyValidatorResult(null);
		}

		public static PropertyValidatorResult Failure(string error) {
			return new PropertyValidatorResult(error);
		}
	}
}

#endregion