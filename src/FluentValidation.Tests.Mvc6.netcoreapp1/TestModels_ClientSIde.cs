namespace FluentValidation.Tests.AspNetCore {
	public class ClientsideModel {
		public string CreditCard { get; set; }
		public string Email { get; set; }
		public string EqualTo { get; set; }
		public string MaxLength { get; set; }
		public string MinLength { get; set; }
		public int Range { get; set; }
		public string RegEx { get; set; }
		public string Required { get; set; }
		public string Length { get; set; }
		public string Required2 { get; set; }
		public string RequiredInsidePartial { get; set; }
		public string ExactLength { get; set; }
		public int GreaterThan { get; set; }
		public int GreaterThanOrEqual { get; set; }
		public int LessThan { get; set; }
		public int LessThanOrEqual { get; set; }
		public string LengthWithMessage { get; set; }
		public string CustomPlaceholder { get; set; }
		public string LengthCustomPlaceholders { get; set; }
		public string CustomName { get; set; }
		public string MessageWithContext { get; set; }
		public int CustomNameValueType { get; set; }
		public string LocalizedName { get; set; }
	}

	public class ClientsideRulesetModel {
		public string CustomName1 { get; set; }
		public string CustomName2 { get; set; }
		public string CustomName3 { get; set; }
	}

	public class ClientsideRulesetValidator : AbstractValidator<ClientsideRulesetModel> {
		public ClientsideRulesetValidator() {
			RuleSet("Foo", () => {
				RuleFor(x => x.CustomName1).NotNull().WithMessage("first");
			});
			RuleSet("Bar", () => {
				RuleFor(x => x.CustomName2).NotNull().WithMessage("second");
			});

			RuleFor(x => x.CustomName3).NotNull().WithMessage("third");

		}
	}

	public class ClientsideScopedDependency {  }


	public class ClientsideModelValidator : AbstractValidator<ClientsideModel> {
		// Need to inject a scoped dependency here to validate that we allow scoped dependencies when generating clientside rules, as MvcViewOptionSetup is always resolved from root container.
		// So we may end up with a cannot resolve from root provider error if things aren't configured properly.
		public ClientsideModelValidator(ClientsideScopedDependency dep) {
			RuleFor(x => x.CreditCard).CreditCard();
			RuleFor(x => x.Email).EmailAddress();
			RuleFor(x => x.EqualTo).Equal(x => x.Required);
			RuleFor(x => x.MaxLength).MaximumLength(2);
			RuleFor(x => x.MinLength).MinimumLength(1);
			RuleFor(x => x.Range).InclusiveBetween(1, 5);
			RuleFor(x => x.RegEx).Matches("[0-9]");
			RuleFor(x => x.Required).NotEmpty();
			RuleFor(x => x.Required2).NotEmpty();
			RuleFor(x => x.RequiredInsidePartial).NotEmpty();

			RuleFor(x => x.Length).Length(1, 4);
			RuleFor(x => x.ExactLength).Length(4);
			RuleFor(x => x.LessThan).LessThan(10);
			RuleFor(x => x.LessThanOrEqual).LessThanOrEqualTo(10);
			RuleFor(x => x.GreaterThan).GreaterThan(1);
			RuleFor(x => x.GreaterThanOrEqual).GreaterThanOrEqualTo(1);

			RuleFor(x => x.LengthWithMessage).Length(1, 10).WithMessage("Foo");
			RuleFor(x => x.CustomPlaceholder).NotNull().WithMessage("{PropertyName} is null.");
			RuleFor(x => x.LengthCustomPlaceholders).Length(1, 5).WithMessage("Must be between {MinLength} and {MaxLength}.");

			RuleFor(x => x.CustomName).NotNull().WithName("Foo");
			RuleFor(x => x.LocalizedName).NotNull().WithLocalizedName(() => TestMessages.notnull_error);
			RuleFor(x => x.CustomNameValueType).NotNull().WithName("Foo");
			RuleFor(x => x.MessageWithContext).NotNull().WithMessage(x => $"Foo {x.Required}");


		}
	}
}