using System;
using FluentValidation.Attributes;
using FluentValidation.Sample.Mvc6.Validators;

namespace FluentValidation.Sample.Mvc6.Models
{
    /// <summary>
    /// Summary description for Person
    /// </summary>
    [Validator(typeof(PersonValidator))]
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
    }
}