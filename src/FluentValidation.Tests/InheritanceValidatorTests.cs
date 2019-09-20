namespace FluentValidation.Tests {
  using System;
  using System.Collections.Generic;
  using Xunit;

  public class InheritanceValidatorTests {


    [Fact]
    public void InheritanceValidator_should_pass_if_bike_hasbell_is_true() {

      var validator = new TestValidator(v => {
        v.RuleFor(x => x.Vehicle).AddInheritance(i => {
          i.Include<Car>(b => { b.RuleFor(m => m.IsElectric).Must(x => x); });
          i.Include<Bike>(b => { b.RuleFor(m => m.HasBell).Must(x => x); });
        });
      });

      var result = validator.Validate(new Person { Vehicle = new Bike() { HasBell = true } });
      result.IsValid.ShouldBeTrue();

    }

    [Fact]
    public void InheritanceValidator_should_pass_if_bike_hasbell_is_true_and_title_is_notnull() {

      var validator = new TestValidator(v => {
        v.RuleFor(x => x.Vehicle.Title).NotNull();
        v.RuleFor(x => x.Vehicle).AddInheritance(i => {
          i.Include<Car>(b => { b.RuleFor(m => m.IsElectric).Must(x => x); });
          i.Include<Bike>(b => { b.RuleFor(m => m.HasBell).Must(x => x); });
        });
      });

      var result = validator.Validate(new Person { Vehicle = new Bike() { HasBell = true, Title = "My awesome bike" } });
      result.IsValid.ShouldBeTrue();

    }

    [Fact]
    public void InheritanceValidator_should_fail_if_bike_hasbell_is_true_and_title_is_null() {

      var validator = new TestValidator(v => {
        v.RuleFor(x => x.Vehicle.Title).NotNull();
        v.RuleFor(x => x.Vehicle).AddInheritance(i => {
          i.Include<Car>(b => { b.RuleFor(m => m.IsElectric).Must(x => x); });
          i.Include<Bike>(b => { b.RuleFor(m => m.HasBell).Must(x => x); });
        });
      });

      var result = validator.Validate(new Person { Vehicle = new Bike() { HasBell = true } });
      result.IsValid.ShouldBeFalse();

    }


    [Fact]
    public void InheritanceValidator_should_pass_if_car_iselectric_is_true() {

      var validator = new TestValidator(v => {
        v.RuleFor(x => x.Vehicle).AddInheritance(i => {
          i.Include<Car>(b => { b.RuleFor(m => m.IsElectric).Must(x => x); });
          i.Include<Bike>(b => { b.RuleFor(m => m.HasBell).Must(x => x); });
        });
      });

      var result = validator.Validate(new Person { Vehicle = new Car() { IsElectric = true } });

      result.IsValid.ShouldBeTrue();

    }

    [Fact]
    public void InheritanceValidator_should_fail_if_car_iselectric_is_false() {

      var validator = new TestValidator(v => {
        v.RuleFor(x => x.Vehicle).AddInheritance(i => {
          i.Include<Car>(b => { b.RuleFor(m => m.IsElectric).Must(x => x); });
          i.Include<Bike>(b => { b.RuleFor(m => m.HasBell).Must(x => x); });
        });
      });

      var result = validator.Validate(new Person { Vehicle = new Car() });

      result.IsValid.ShouldBeFalse();

    }


    [Fact]
    public void InheritanceValidator_should_pass_without_inlinevalidator_if_bike_hasbell_is_true() {

      var validator = new TestValidator(v => {
        v.RuleFor(x => x.Vehicle).AddInheritance(i => {
          i.Include<Car>(b => { b.RuleFor(m => m.IsElectric).Must(x => x); });
          i.Include<Bike>(new BikeTestValidator(m => m.RuleFor(z => z.HasBell).Must(x => x)));
        });
      });

      var result = validator.Validate(new Person { Vehicle = new Bike() { HasBell = true } });

      result.IsValid.ShouldBeTrue();

    }

    [Fact]
    public void InheritanceValidator_should_pass_if_list_wantedvehicles_passed_conditions() {



      var validator = new TestValidator(v => {
        v.RuleForEach(x => x.WantedVehicles).AddInheritance(i => {
          i.Include<Car>(b => { b.RuleFor(m => m.IsElectric).Must(x => x); });
          i.Include<Bike>(b => { b.RuleFor(m => m.HasBell).Must(x => x); });
        });
      }); 

      var result = validator.Validate(new Person { WantedVehicles = new List<IVehicle>() {
        new Car() { IsElectric = true },
        new Bike() { HasBell = true }
      } });

      result.IsValid.ShouldBeTrue();

    }
    public class BikeTestValidator : InlineValidator<Bike> {
      public BikeTestValidator() {

      } 

      public BikeTestValidator(params Action<BikeTestValidator>[] actions) {
        foreach (var action in actions) {
          action(this);
        }
      }
    }

  }
}
