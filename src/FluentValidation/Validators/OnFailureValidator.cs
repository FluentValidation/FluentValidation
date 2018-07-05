using FluentValidation.Internal;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FluentValidation.Validators
{
	public class OnFailureValidator<T, TProperty> : NoopPropertyValidator
	{
		private readonly IPropertyValidator _innerValidator;
		private readonly Action<T> _onFailureSuperSimple;
		private readonly Action<T, PropertyValidatorContext> _onFailureSimple;
		private readonly Action<T, PropertyValidatorContext, string> _onFailureComplex;
		private readonly string _errorMessage;
		private readonly Func<T, TProperty, object>[] _messageProvider;

		public OnFailureValidator(IPropertyValidator innerValidator, Action<T, PropertyValidatorContext, string> onFailure, string errorMessage, Func<T, TProperty, object>[] messageProvider)
		{
			messageProvider.Guard("A messageProvider must be provided.", nameof(messageProvider));

			_innerValidator = innerValidator;
			_onFailureComplex = onFailure;
			_errorMessage = errorMessage;
			_messageProvider = messageProvider;
		}

		public OnFailureValidator(IPropertyValidator innerValidator, Action<T, PropertyValidatorContext> onFailure)
		{
			_innerValidator = innerValidator;
			_onFailureSimple = onFailure;
		}

		public OnFailureValidator(IPropertyValidator innerValidator, Action<T> onFailure)
		{
			_innerValidator = innerValidator;
			_onFailureSuperSimple = onFailure;
		}

		public override IEnumerable<ValidationFailure> Validate(PropertyValidatorContext context)
		{
			var results = _innerValidator.Validate(context);
			if (!results.Any()) return results;

			if (string.IsNullOrWhiteSpace(_errorMessage))
			{
				if (_onFailureSuperSimple != null)
				{
					_onFailureSuperSimple((T)context.Instance);
				}
				else
				{
					_onFailureSimple((T)context.Instance, context);
				}
			}
			else
			{
				var messageFormatter = new MessageFormatter();
				messageFormatter.AppendPropertyName(context.PropertyName);
				messageFormatter.AppendArgument("PropertyValue", context.PropertyValue);
				try
				{
					messageFormatter.AppendAdditionalArguments(
						_messageProvider.Select(func => func((T)context.Instance, (TProperty)context.PropertyValue)).ToArray());
				}
				catch (Exception ex)
				{
					//TODO log exceptions
					//Debug.WriteLine(ex);
				}

				var msg = messageFormatter.BuildMessage(_errorMessage);
				_onFailureComplex((T)context.Instance, context, msg);
			}

			return results;
		}
	}
}
