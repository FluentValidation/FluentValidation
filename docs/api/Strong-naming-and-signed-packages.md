As of FluentValidation 7.0, all packages are signed and have a strong named. 

Prior to 7.0, there were two versions of each package - a signed and an unsigned version. This caused issues with Nuget dependency resolution, for example:

- My project depends on PackageA and PackageB
- PackageA depends on FluentValidation
- PackageB depends on FluentValidation-signed

This will cause a dependency resolution issue, so the decision was made to sign all the FluentValidation dlls going forward, and only use a single package. This also means that as a consumer, you can use the main FluentValidation package regardless of whether your project is strong named or not.

There are some items to note about this change:

The AssemblyVersion for all FluentValidation packages going forward is 7.0.0.0. Even when there are new releases of FluentValidaiton, this will remain unchanged. For example, FluentValidation 7.1 has a package version of 7.1, an AssemblyFileVersion of 7.1 but still has an AssemblyVersion of 7.0.0.0. This means that going forward, you can hot-swap the FluentValidation dlls, like you would be able as if the dll were unsigned. It also means that you will not need to use Assembly Binding Redirects when you upgrade FluentValidation. 

Note that if you have several projects that depend on an older, unsigned version of FluentValidation, you will need to recompile all of them. This is a one-time process that is necessary when moving from an unsigned to a signed package.