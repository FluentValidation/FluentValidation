namespace FluentValidation.Tests {
  using System.Linq;
  using Internal;
  using Validators;
  using Xunit;

  public class InheritanceValidatorExtensionTests {


    private readonly AbstractValidator<Person> _validator;

    public InheritanceValidatorExtensionTests() {
      _validator = new TestValidator();
    }
     
    [Fact]
    public void AddInheritance_should_create_InheritanceValidator() {
      _validator.RuleFor(x => x.Vehicle).AddInheritance(inheritance => { });

      var rule = (PropertyRule)_validator.First();
      var validator = (ChildValidatorAdaptor)rule.CurrentValidator;
      validator.ValidatorType.ShouldEqual(typeof(InheritanceValidator<IVehicle>));

    }
     
    
  }
}
