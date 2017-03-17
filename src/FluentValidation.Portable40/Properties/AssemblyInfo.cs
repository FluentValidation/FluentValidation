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

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;

[assembly: AssemblyTitle("FluentValidation")]
[assembly: AssemblyDescription("FluentValidation")]
[assembly: AssemblyProduct("FluentValidation")]
[assembly: AssemblyCopyright("Copyright (c) Jeremy Skinner 2008-2017")]
#if !PORTABLE && !PORTABLE40
[assembly: ComVisible(false)]
#endif
[assembly: CLSCompliant(true)]
[assembly: System.Resources.NeutralResourcesLanguage("en")]