.. FluentValidation documentation master file, created by
   sphinx-quickstart on Wed Feb  5 15:31:13 2020.
   You can adapt this file completely to your liking, but it should at least
   contain the root `toctree` directive.

FluentValidation
================

FluentValidation is a .NET library for building strongly-typed validation rules.

FluentValidation 11 supports the following platforms:

* .NET Core 3.1
* .NET 5
* .NET 6
* .NET 7
* .NET 8
* `.NET Standard 2.0 <https://docs.microsoft.com/en-us/dotnet/standard/net-standard>`_

For automatic validation with ASP.NET, FluentValidation supports ASP.NET running on .NET Core 3.1, .NET 5 or .NET 6.

If you're new to using FluentValidation, check out the :doc:`start` page.

.. note::
  If you use FluentValidation in a commercial project, please
  `sponsor the project financially <https://github.com/sponsors/JeremySkinner>`_. 
  FluentValidation is developed for free by `@JeremySkinner <https://github.com/JeremySkinner>`_
  in his spare time and financial sponsorship helps keep the project going. Please sponsor the project 
  via either `GitHub sponsors <https://github.com/sponsors/JeremySkinner>`_ or `OpenCollective <https://opencollective.com/FluentValidation>`_.

Example
=======

.. code-block:: c#

   public class CustomerValidator : AbstractValidator<Customer> 
   {
     public CustomerValidator()
     {
       RuleFor(x => x.Surname).NotEmpty();
       RuleFor(x => x.Forename).NotEmpty().WithMessage("Please specify a first name");
       RuleFor(x => x.Discount).NotEqual(0).When(x => x.HasDiscount);
       RuleFor(x => x.Address).Length(20, 250);
       RuleFor(x => x.Postcode).Must(BeAValidPostcode).WithMessage("Please specify a valid postcode");
     }

     private bool BeAValidPostcode(string postcode) 
     {
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

.. _config-docs:
.. toctree::
  :maxdepth: 1
  :caption: Configuring Validators

  configuring
  conditions

.. _validator-docs:
.. toctree::
  :maxdepth: 1
  :caption: Building Rules

  built-in-validators
  custom-validators

.. _feature-docs:
.. toctree::
  :maxdepth: 1
  :caption: Other Features

  including-rules
  specific-properties
  rulesets
  cascade
  di
  async
  severity
  error-codes
  custom-state

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

.. _advanced-docs:
.. toctree::
  :maxdepth: 1
  :caption: Advanced

  dependentrules
  inheritance
  advanced

.. _aspnet-docs:
.. toctree::
  :maxdepth: 1
  :caption: ASP.NET Integration

  aspnet
  blazor

.. _upgrading-docs:
.. toctree::
  :maxdepth: 1
  :caption: Upgrading

  upgrading-to-11
  upgrading-to-10
  upgrading-to-9
  upgrading-to-8