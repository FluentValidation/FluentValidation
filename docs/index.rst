.. FluentValidation documentation master file, created by
   sphinx-quickstart on Wed Feb  5 15:31:13 2020.
   You can adapt this file completely to your liking, but it should at least
   contain the root `toctree` directive.

FluentValidation
================

FluentValidation is a .NET library for building strongly-typed validation rules.

FluentValidation 10 supports the following platforms:

* .NET Core 3.1
* .NET 5
* `.NET Standard 2.0 <https://docs.microsoft.com/en-us/dotnet/standard/net-standard>`_

For automatic validation with ASP.NET, FluentValidation supports ASP.NET running on .NET Core 3.1 or .NET 5

If you're new to using FluentValidation, check out the :doc:`start` page.

.. note::
  FluentValidation is developed and supported by `@JeremySkinner <https://github.com/JeremySkinner>`_
  in his spare time. If you find FluentValidation useful, or if you use FluentValidation in a commercial environment, then
  please consider financially supporting the project on one of the following platforms, which will help keep the project going.

  * `GitHub sponsors <https://github.com/sponsors/JeremySkinner>`_
  * `OpenCollective <https://opencollective.com/FluentValidation>`_

Example
=======

.. code-block:: c#

   public class CustomerValidator : AbstractValidator<Customer> {
     public CustomerValidator() {
       RuleFor(x => x.Surname).NotEmpty();
       RuleFor(x => x.Forename).NotEmpty().WithMessage("Please specify a first name");
       RuleFor(x => x.Discount).NotEqual(0).When(x => x.HasDiscount);
       RuleFor(x => x.Address).Length(20, 250);
       RuleFor(x => x.Postcode).Must(BeAValidPostcode).WithMessage("Please specify a valid postcode");
     }

     private bool BeAValidPostcode(string postcode) {
       // custom postcode validating logic goes here
     }
   }


.. _user-docs:
.. toctree::
  :maxdepth: 1
  :caption: Getting Started

  installation
  start
  collections
  inheritance
  rulesets
  including-rules
  upgrading-to-10
  upgrading-to-9
  upgrading-to-8

.. _config-docs:
.. toctree::
  :maxdepth: 1
  :caption: Configuring Validators

  configuring
  conditions
  severity
  error-codes
  custom-state

.. _validator-docs:
.. toctree::
  :maxdepth: 1
  :caption: Building Rules

  built-in-validators
  custom-validators

.. _localization-docs:
.. toctree::
  :maxdepth: 1
  :caption: Localization

  localization

.. _testing-docs:
.. toctree::
  :maxdepth: 1
  :caption: Testing

  testing

.. _aspnet-docs:
.. toctree::
  :maxdepth: 1
  :caption: ASP.NET Integration

  aspnet
  blazor

.. _advanced-docs:
.. toctree::
  :maxdepth: 1
  :caption: Advanced

  async
  transform
  advanced
