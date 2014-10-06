using System;
using FluentValidation;
using FluentValidation.Sample.Mvc6.Models;

namespace FluentValidation.Sample.Mvc6.Validators
{
    /// <summary>
    /// Summary description for PersonValidator
    /// </summary>
    public class PersonValidator : AbstractValidator<Person>
    {
        public PersonValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.Name).Length(0, 10).WithMessage("Please enter a name with length 0-10 please");
            RuleFor(x => x.Email).EmailAddress();
            RuleFor(x => x.Age).InclusiveBetween(18, 60);
        }
    }
}