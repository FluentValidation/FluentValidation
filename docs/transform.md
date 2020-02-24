# Transforming Values

As of FluentValidation 9.0, you can use the `Transform` method to transform a property value prior to validation being performed against it. For example, if you have property of type `string` that atually contains numeric input, you could use `Transform` to convert the string value to a number. 


```csharp
RuleFor(x => x.SomeStringProperty)
    .Transform(value => int.TryParse(value, out int val) ? (int?) val : null)
    .GreaterThan(10);
```

This rule transforms the value from a `string` to an nullable `int` (returning `null` if the value couldn't be converted). A greater-than check is then performed on the resulting value. 

Syntactically this is not particularly nice to read, so this can be cleaned up by using an extension method:

```csharp
public static class ValidationExtensions {
	public static IRuleBuilder<T, int?> TransformToInt<T>(this IRuleBuilderInitial<T, string> ruleBuilder) {
		return ruleBuilder.Transform(value => int.TryParse(value, out int val) ? (int?) val : null);
	} 
}
```

The rule can then be written as:

```csharp
RuleFor(x => x.SomeStringProperty)
    .TransformToInt()
    .GreaterThan(10);
```


```eval_rst
.. note::
  FluentValidation 8.x supported a limited version of the Transform method that could only be used to perform transformations on the same type (eg if the property is a string, the result of the transformation must also be a string). FluentValidation 9.0 allows transformations to be performed that change the type.
```
