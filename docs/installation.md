# Installation

```eval_rst
.. note::
    If you are upgrading to FluentValidation 9 from an older version, `please read the upgrade notes <upgrading-to-9.html>`_.
```

Before creating any validators, you will need to add a reference to FluentValidation.dll in your project. FluentValidation is available as either a netstandard2.0 library or as a net461 library for older projects.

The simplest way to do this is to use either the NuGet package manager, or the dotnet CLI.

Using the NuGet package manager console within Visual Studio run the following command:

```
Install-Package FluentValidation
```

Or using the .net core CLI from a terminal window:

```
dotnet add package FluentValidation
```

For integration with ASP.NET Core, install the FluentValidation.AspNetCore package:

```shell
Install-Package FluentValidation.AspNetCore
```