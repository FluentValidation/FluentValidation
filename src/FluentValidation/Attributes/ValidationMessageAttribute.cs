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

namespace FluentValidation.Attributes {
	using System;
	using System.Globalization;
	using Resources;

	/// <summary>
	/// Defines the resource key to be used for a particular property validator. 
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class ValidationMessageAttribute : Attribute {
		public string Key { get; set; }
		public string Message { get; set; }

		public static string GetMessage(Type type) {
			var attribute = (ValidationMessageAttribute)GetCustomAttribute(type, typeof(ValidationMessageAttribute), false);

			if(attribute == null) {
				throw new InvalidOperationException(string.Format("Type '{0}' does does not declare a ValidationMessageAttribute.", type.Name));
			}
            
			if(string.IsNullOrEmpty(attribute.Key) && string.IsNullOrEmpty(attribute.Message)) {
				throw new InvalidOperationException(string.Format("Type '{0}' declares a ValidationMessageAttribute but neither the Key nor Message are set.", type.Name));
			}

			if(!string.IsNullOrEmpty(attribute.Message)) {
				return attribute.Message;
			}

			var message =  DefaultResourceManager.Current.GetString(attribute.Key, CultureInfo.CurrentCulture);

			if(message == null) {
				throw new InvalidOperationException(string.Format("Could not find a resource key with the name '{0}'.", attribute.Key));				
			}

			return message;
		}
	}
}