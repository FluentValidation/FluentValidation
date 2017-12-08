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

namespace FluentValidation.Tests {
	using System;
	using System.Globalization;
    using System.Linq;
    using System.Threading;
    using Xunit;

	
	public class ValidateAndThrowTester {
		public ValidateAndThrowTester() {
			CultureScope.SetDefaultCulture();
		}

		[Fact]
		public void Throws_exception() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotNull()
			};

			typeof(ValidationException).ShouldBeThrownBy(() => validator.ValidateAndThrow(new Person()));
		}

        [Fact]
        public void Throws_exception_with_a_ruleset() {
            var validator = new TestValidator {
                v => v.RuleFor(x => x.Surname).NotNull()
            };

            const string ruleSetName = "blah";
            validator.RuleSet(ruleSetName, () => {
                validator.RuleFor(x => x.Forename).NotNull();
            });

            typeof(ValidationException).ShouldBeThrownBy(() => validator.ValidateAndThrow(new Person(), ruleSetName));
        }

        [Fact]
		public void Throws_exception_async() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotNull()
			};

			typeof(ValidationException).ShouldBeThrownBy(() => {
				try {
					validator.ValidateAndThrowAsync(new Person()).Wait();
				}
				catch (AggregateException agrEx) {
					throw agrEx.InnerException;
				}
			});
		}

        [Fact]
        public void Throws_exception_with_a_ruleset_async()
        {
            var validator = new TestValidator {
                v => v.RuleFor(x => x.Surname).NotNull()
            };

            const string ruleSetName = "blah";
            validator.RuleSet(ruleSetName, () => {
                validator.RuleFor(x => x.Forename).NotNull();
            });

            typeof(ValidationException).ShouldBeThrownBy(() => {
                try
                {
                    validator.ValidateAndThrowAsync(new Person(), ruleSetName).Wait();
                }
                catch (AggregateException agrEx)
                {
                    throw agrEx.InnerException;
                }
            });
        }

        [Fact]
		public void Does_not_throw_when_valid() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotNull()
			};

			validator.ValidateAndThrow(new Person {Surname = "foo"});
		}

        [Fact]
        public void Does_not_throw_when_valid_and_a_ruleset()
        {
            var validator = new TestValidator {
                v => v.RuleFor(x => x.Surname).NotNull()
            };

            const string ruleSetName = "blah";
            validator.RuleSet(ruleSetName, () => {
                validator.RuleFor(x => x.Forename).NotNull();
            });

            var person = new Person {
                Forename = "foo",
                Surname = "foo"
            };
            validator.ValidateAndThrow(person, ruleSetName);
        }

        [Fact]
        public void Does_not_throw_when_valid_async() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotNull()
			};

			validator.ValidateAndThrowAsync(new Person { Surname = "foo" }).Wait();
		}

        [Fact]
        public void Does_not_throw_when_valid_and_a_ruleset_async()
        {
            var validator = new TestValidator {
                v => v.RuleFor(x => x.Surname).NotNull()
            };

            const string ruleSetName = "blah";
            validator.RuleSet(ruleSetName, () => {
                validator.RuleFor(x => x.Forename).NotNull();
            });

            var person = new Person
            {
                Forename = "foo",
                Surname = "foo"
            };
            validator.ValidateAndThrowAsync(person, ruleSetName).Wait();
        }

        [Fact]
		public void Populates_errors() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotNull()
			};

			var ex = (ValidationException)typeof(ValidationException).ShouldBeThrownBy(() => validator.ValidateAndThrow(new Person()));
			ex.Errors.Count().ShouldEqual(1);
		}

		[Fact]
		public void ToString_provides_error_details() {
			var validator = new TestValidator {
				v => v.RuleFor(x => x.Surname).NotNull(),
				v => v.RuleFor(x => x.Forename).NotNull()
			};

			var ex = typeof(ValidationException).ShouldBeThrownBy(() => validator.ValidateAndThrow(new Person()));
            string expected = "FluentValidation.ValidationException: Validation failed: " + Environment.NewLine + " -- 'Surname' must not be empty.\r\n -- 'Forename' must not be empty.";
            Assert.True(ex.ToString().StartsWith(expected));
		}
	}
}