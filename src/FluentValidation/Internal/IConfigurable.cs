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
#endregion

namespace FluentValidation.Internal {
	using System;
	using System.ComponentModel;

	/// <summary>
	/// Represents an object that is configurable.
	/// </summary>
	/// <typeparam name="TConfiguration">Type of object being configured</typeparam>
	/// <typeparam name="TNext">Return type</typeparam>
	public interface IConfigurable<TConfiguration, out TNext> {
		/// <summary>
		/// Configures the current object.
		/// </summary>
		/// <param name="configurator">Action to configure the object.</param>
		/// <returns></returns>
		TNext Configure(Action<TConfiguration> configurator);
	}
}