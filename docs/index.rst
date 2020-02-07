.. FluentValidation documentation master file, created by
   sphinx-quickstart on Wed Feb  5 15:31:13 2020.
   You can adapt this file completely to your liking, but it should at least
   contain the root `toctree` directive.

FluentValidation
================

FluentValidation is a A .NET library for building strongly-typed validation rules.

The following platforms are supported:

* .NET 4.6.1+
* .NET Core 2.0+
* `.NET Standard 2.0+ <https://docs.microsoft.com/en-us/dotnet/standard/net-standard>`_

For automatic validation with ASP.NET, FluentValidation supports ASP.NET Core 2.1+ (3.1 recommended)

If you're new to using FluentValidation, check out the :doc:`start` page.

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
  :hidden:
  :maxdepth: 2
  :caption: Getting Started

  installation
  collections
  start 
  rulesets
  including-rules

.. _config-docs:
.. toctree::
  :hidden:
  :maxdepth: 2
  :caption: Configuring Validators

  configuring
  conditions


.. _validator-docs:
.. toctree::
  :hidden:
  :maxdepth: 2
  :caption: Building Rules

  built-in-validators
  custom-validators

.. _localization-docs:
.. toctree::
  :hidden:
  :maxdepth: 0
  :caption: Localization

  localization

.. _testing-docs:
.. toctree::
  :hidden: 
  :maxdepth: 2
  :caption: Testing

  testing

.. _aspnet-docs:
.. toctree::
  :hidden:
  :maxdepth: 2
  :caption: ASP.NET Integration 

  aspnet
  mvc5
  webapi
  
.. _advanced-docs:
.. toctree::
  :hidden:
  :maxdepth: 2
  :caption: Advanced

  async
  advanced

.. _upgrade-docs:
.. toctree::
  :hidden:
  :maxdepth: 2
  :caption: Uprade Guides

  upgrading-to-8

