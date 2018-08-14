---
title: Installation
---
<div class="callout-block callout-info"><div class="icon-holder">*&nbsp;*{: .fa .fa-info-circle}
</div><div class="content">
{: .callout-title}
#### Upgrading to FluentValidation 8

If you are upgrading to FluentValidation 8 from an older version, please [read the upgrade notes](../upgrading-to-8.html)

</div></div>


Before creating any validators, you will need to add a reference to FluentValidation.dll in your project. FluentValidation is available as either a netstandard2.0 library or as a net45 library for older projects.

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

For integration with legacy ASP.NET MVC 5 or WebApi 2 projects, you can use the FluentValidation.Mvc5 and FluentValidation.WebApi packages respectively.

```shell
Install-Package FluentValidation.Mvc5
Install-Package FluentValidation.WebApi
```
