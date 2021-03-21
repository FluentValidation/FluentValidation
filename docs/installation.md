# Installation

```eval_rst
.. note::
    If you are upgrading to FluentValidation 10 from an older version, `please read the upgrade notes <upgrading-to-10.html>`_.
```

Before creating any validators, you will need to add a reference to FluentValidation.dll in your project. The simplest way to do this is to use either the NuGet package manager, or the dotnet CLI.

Using the NuGet package manager console within Visual Studio run the following command:

```
Install-Package FluentValidation
```

Or using the .net core CLI from a terminal window:

```
dotnet add package FluentValidation
```

For integration with ASP.NET Core, install the [FluentValidation.AspNetCore](https://www.nuget.org/packages/FluentValidation.AspNetCore/) package from Visual Studio:

```shell
Install-Package FluentValidation.AspNetCore
```

or from the command line:

```shell
dotnet add package FluentValidation.AspNetCore
```
