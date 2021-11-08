## General support, feedback, and discussions
FluentValidation is maintained on a voluntary basis, and unfortunately this means we are unable to provide general support or answer questions on usage due to the time and effort required to moderate these. The issue tracker should only be used for bug reports in the core FluentValidation library, or feature requests where appropriate. Requests for general support or questions on usage will be closed. We appreciate that this may appear strict, but is necessary to protect the free time and mental health of the project's maintainers. Thank you for understanding.

## Supporting the project
If you use FluentValidation in a commercial project, please sponsor the project financially. FluentValidation is developed and supported by [@JeremySkinner](https://github.com/JeremySkinner) for free in his spare time and financial sponsorship helps keep the project going. You can sponsor the project via either [GitHub sponsors](https://github.com/sponsors/JeremySkinner) or [OpenCollective](https://opencollective.com/FluentValidation).


## Filing bug reports and feature requests
The best way to get your bug fixed is to be as detailed as you can be about the problem.

Please check both the documentation at https://fluentvalidation.net and old issues first to see if your question has already been answered.

If not, then please provide the exact version of FluentValidation that you're using along with a detailed explanation of the issue and complete steps to reproduce the problem. Issues that don't provide enough information to reproduce will be closed.

Please ensure all sample code is properly formatted and readable (GitHub supports [markdown](https://github.github.com/github-flavored-markdown/)). Issues that don't include all necessary code (or a sample project) to reproduce will be closed.

We do our best to respond to all bug reports and feature requests, but FluentValidation is maintained on a voluntary basis and we cannot guarantee how quickly these will be looked at.

## Contributing Code
Please open an issue to discuss new feature requests before submitting a Pull Request. This allows the maintainers to discuss whether your feature is a suitable fit for the project before any code is written. Please don't open a pull request without first discussing whether the feature fits with the project roadmap.

## Building the code
Run `Build.cmd` (windows) or build.sh (Linux/mac) from the command line. This builds the project and runs tests. Building requires the following software to be installed:

* Windows Powershell or Powershell Core
* .NET Core 3.1 SDK
* .NET Core 2.1 SDK
* .NET 5 SDK

## Contributing code and content
You will need to sign a [Contributor License Agreement](https://cla.dotnetfoundation.org/) before submitting your pull request.

Make sure you can build the code. Familiarize yourself with the project workflow and our coding conventions. If you don't know what a pull request is read this article: https://help.github.com/articles/using-pull-requests.

If you wish to submit a new feature, please open an issue to discuss it with the project maintainers - don't open a pull request without first discussing whether the feature fits with the project roadmap.

Tests must be provided for all pull requests that add or change functionality.

Please ensure that you follow the existing code-style when adding new code to the project. This may seem pedantic, but it makes it much easier to review pull requests when contributed code matches the existing project style. Specifically:
- Please ensure that your editor is configured to use tabs for indentation, not spaces
- Please ensure that the project copyright notice is included in the header for all files.
- Please ensure `using` statements are inside the namespace declaration
- Please ensure that all opening braces are on the end of line:

```csharp
// Opening braces should be on the end of the line like this
if (foo) {

}

// Not like this:
if (foo)
{

}
```

## Building the Documentation

The documentation is built separately from the source code. Building the documentation requires Python 3 and pip. This is then used to install Sphinx and dependencies, which then enable `make` to build the site.

For example, on Linux / within WSL:

* `sudo apt install python3-pip`
* `cd docs` to go to the docs directory
* `pip3 install -r requirements.txt` to install the required packages for the docs
* On WSL, you may need to exit and restart at this point.
* `PATH=$PATH:~/.local/lib/python3.8/site-packages` (you may want to add this to your `.bashrc` file as well.)
* `make html` to build the site or `make serve` to watch for changes.
