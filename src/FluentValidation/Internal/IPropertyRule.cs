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

namespace FluentValidation.Internal {
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using Results;
	using Validators;

	public interface IPropertyRule<T> : IValidationRule<T> {
		string CustomPropertyName { get; set; }
		string PropertyName { get; set; }
		string PropertyDescription { get; }
		MemberInfo Member { get; }
	}
	
	public interface ISimplePropertyRule<T> : IPropertyRule<T> {
		string CustomValidationMessage { get; set; }
		IList<Func<T, object>> CustomMessageFormatArguments { get; }
		IPropertyValidator<T> Validator { get; }
		Func<T, object> CustomStateProvider { get; set; }
		Action<T> OnFailure { get; set; }
	}

	public interface ISimplePropertyRule<T, TProperty> : ISimplePropertyRule<T>  {
		new IPropertyValidator<T, TProperty> Validator { get; set; }
	}
}