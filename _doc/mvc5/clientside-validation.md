---
title: Clientside Validation
---

Note that FluentValidation will also work with ASP.NET MVC's client-side validation, but not all rules are supported. For example, any rules defined using a condition (with When/Unless), custom validators, or calls to Must will not run on the client side. The following validators are supported on the client:

* NotNull/NotEmpty
* Matches (regex)
* InclusiveBetween (range)
* CreditCard
* Email
* EqualTo (cross-property equality comparison)
* Length
