# Transforming Values

As of FluentValidation 9.5, you can apply a transformation to a property value prior to validation being performed against it. For example, if you have property of type `string` that actually contains numeric input, you could apply a transformation to convert the string value to a number.


```csharp
Transform(from: x => x.SomeStringProperty, to: value => int.TryParse(value, out int val) ? (int?) val : null)
    .GreaterThan(10);
```

This rule transforms the value from a `string` to an nullable `int` (returning `null` if the value couldn't be converted). A greater-than check is then performed on the resulting value.

Syntactically this is not particularly nice to read, so the logic for the transformation can optionally be moved into a separate method:

```csharp
Transform(x => x.SomeStringProperty, StringToNullableInt)
    .GreaterThan(10);

int? StringToNullableInt(string value)
  => int.TryParse(value, out int val) ? (int?) val : null;

```

This syntax is available in FluentValidation 9.5 and newer.

There is also a `TransformForEach` method available, which performs the transformation against each item in a collection.


## Transforming Values (9.0 - 9.4)

Prior to FluentValidation 9.5, you can use the `Transform` method after a call to `RuleFor` to achieve the same result.

```csharp
RuleFor(x => x.SomeStringProperty)
    .Transform(value => int.TryParse(value, out int val) ? (int?) val : null)
    .GreaterThan(10);
```

This `Transform` method is marked as obsolete as of FluentValidation 9.5 and is removed in FluentValidation 10.0. In newer versions of FluentValidation the transformation should be applied by calling `Transform` as the first method in the chain (see above).
