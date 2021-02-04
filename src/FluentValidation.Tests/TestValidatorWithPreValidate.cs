using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentValidation.Tests {
	using Results;

	public class TestValidatorWithPreValidate : InlineValidator<Person> {
		public TestValidatorWithPreValidate() {
		}

		public Func<ValidationContext<Person>, ValidationResult, bool> PreValidateMethod { get; set; }

		protected override bool PreValidate(ValidationContext<Person> context, ValidationResult result) {
			return PreValidateMethod?.Invoke(context, result) ?? base.PreValidate(context, result);
		}
	}
}
