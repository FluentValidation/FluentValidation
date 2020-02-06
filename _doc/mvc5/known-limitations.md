---
title: Known Limitations
---
MVC 5 performs validation in two passes. First it tries to convert the input values from the request into the types declared in your model, and then it performs model-level validation using FluentValidation. If you have non-nullable types in your model (such as `int` or `DateTime`) and there are no values submitted in the request, model-level validations will be skipped, and only the type conversion errors will be returned. 

This is a limitation of MVC 5's validation infrastructure, and there is no way to disable this behaviour. If you want all validation failures to be returned in one go, you must ensure that any value types are marked as nullable in your model (you can still enforce non-nullability with a `NotNull` or `NotEmpty` rule as necessary, but the underlying type must allow nulls). This only applies to MVC5 and WebApi 2. ASP.NET Core does not suffer from this issue as the validation infrastructure has been improved. 
