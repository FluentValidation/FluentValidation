﻿#region License
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
// The latest version of this file can be found at https://github.com/JeremySkinner/FluentValidation
#endregion
namespace FluentValidation.Tests {
	using System;
	using System.Globalization;
	using System.Threading;

	public class CultureScope : IDisposable {
		CultureInfo _originalUiCulture;
		CultureInfo _originalCulture;

		public CultureScope(CultureInfo culture) {
			_originalCulture = Thread.CurrentThread.CurrentCulture;
			_originalUiCulture = Thread.CurrentThread.CurrentUICulture;

			Thread.CurrentThread.CurrentCulture = culture;
			Thread.CurrentThread.CurrentUICulture = culture;
		}

		public CultureScope(string culture) : this(new CultureInfo(culture)) {
			
		}

		public void Dispose() {
			Thread.CurrentThread.CurrentCulture = _originalCulture;
			Thread.CurrentThread.CurrentUICulture = _originalUiCulture;
		}

		public static void SetDefaultCulture() {
			Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
			Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
		}
	}
}