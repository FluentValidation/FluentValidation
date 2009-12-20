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

namespace FluentValidation.Resources {
	using System;
	using System.Resources;

	public class DefaultResourceManager : ResourceManager {
		public DefaultResourceManager() : base("FluentValidation.Resources.Messages", typeof(DefaultResourceManager).Assembly) {
		}

		public const string ExactLengthValidatorError = "exact_length_error";
		public const string LengthValidatorError = "length_error";
		public const string RegexError = "regex_error";
		public const string PredicateError = "predicate_error";
		public const string NotNull = "notnull_error";
		public const string NotEqual = "notequal_error";
		public const string NotEmpty = "notempty_error";
		public const string Email = "email_error";
		public const string LessThan = "lessthan_error";
		public const string GreaterThan = "greaterthan_error";
		public const string GreaterThanOrEqual = "greaterthanorequal_error";
		public const string LessThanOrEqual = "lessthanorequal_error";
		public const string Equal = "equal_error";
		public const string InclusiveBetweenValidatorError = "inclusivebetween_error";
		public const string ExclusiveBetweenValidatorError = "exclusivebetween_error";

		static Func<ResourceManager> resourceManagerFunc = () => new DefaultResourceManager();

		public static ResourceManager Current {
			get { return resourceManagerFunc(); }
		}

		public static void SetResourceManagerProvider(Func<ResourceManager> resouceManagerProvider) {
			resourceManagerFunc = resouceManagerProvider;
		}
	}
}