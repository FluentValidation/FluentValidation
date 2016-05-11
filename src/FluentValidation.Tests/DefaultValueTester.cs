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
// The latest version of this file can be found at http://www.codeplex.com/FluentValidation
#endregion

namespace FluentValidation {
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using NUnit.Framework;
    using Tests;


    public class DefaultValueTester {
        [Test]
        public void Resolve_default_of_value_type() {
            var expected = default(DateTime);
            var value = DefaultValue.Resolve<DateTime>();

            value.ShouldEqual(expected);
        }

        [Test]
        public void Resolve_default_of_reference_type()
        {
            var expected = default(Person);
            var value = DefaultValue.Resolve<Person>();

            value.ShouldEqual(expected);
        }  

        [Test]
        public void Resolve_default_of_nullable_value_type()
        {
            var expected = default(DateTime);
            var value = DefaultValue.Resolve<DateTime?>();

            value.ShouldEqual(expected);
        }  
    }
}