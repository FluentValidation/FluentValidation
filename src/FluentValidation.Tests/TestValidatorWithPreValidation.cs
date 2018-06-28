using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentValidation.Tests {
	using Results;

	public class TestValidatorWithPreValidation : AbstractValidator<Person> {
		public TestValidatorWithPreValidation() {
		}

		public Func<ValidationContext<Person>, ValidationResult, bool> PreValidationMethod { get; set; }

		protected override bool PreValidation(ValidationContext<Person> context, ValidationResult result) {
			return PreValidationMethod?.Invoke(context, result) ?? base.PreValidation(context, result);
		}
	}
}