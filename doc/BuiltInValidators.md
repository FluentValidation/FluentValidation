## Built in Validators

FluentValidation ships with several built-in validators. The error message for each validator can contain special placeholders that will be filled in when the error message is constructed.

## NotNull Validator
Description: Ensures that the specified property is not null. 

Example:
```csharp
RuleFor(customer => customer.Surname).NotNull();
```
Example error: 'Surname' must not be empty.
String format args:
* {PropertyName} = The name of the property being validated
* {PropertyValue} = The current value of the property

## NotEmpty Validator
Description: Ensures that the specified property is not null, an empty string or whitespace (or the default value for value types, eg 0 for int)

Example:
```csharp
RuleFor(customer => customer.Surname).NotEmpty();
```
Example error: 'Surname' should not be empty.
String format args:
* {PropertyName} = The name of the property being validated
* {PropertyValue} = The current value of the property

## NotEqual Validator

Description: Ensures that the value of the specified property is not equal to a particular value (or not equal to the value of another property)

Example:
```csharp
//Not equal to a particular value
RuleFor(customer => customer.Surname).NotEqual("Foo");

//Not equal to another property
RuleFor(customer => customer.Surname).NotEqual(customer => customer.Forename);
```
Example error: 'Surname' should not be equal to 'Foo'
String format args: 
* {PropertyName} = The name of the property being validated
* {ComparisonValue} = Value that the property should not equal

## Equal Validator
Description: Ensures that the value of the specified property is equal to a particular value (or equal to the value of another property)

Example:
```csharp
//Equal to a particular value
RuleFor(customer => customer.Surname).Equal("Foo");

//Equal to another property
RuleFor(customer => customer.Password).Equal(customer => customer.PasswordConfirmation);
```
Example error: 'Surname' should be equal to 'Foo'
String format args: 
* {PropertyName} = The name of the property being validated
* {ComparisonValue} = Value that the property should equal
* {PropertyValue} = The current value of the property

## Length Validator
Description: Ensures that the length of a particular string property is within the specified range.

Example:
```csharp
RuleFor(customer => customer.Surname).Length(1, 250); //must be between 1 and 250 chars (inclusive)
```
Example error: 'Surname' must be between 1 and 250 characters. You entered 251 characters.
Note: Only valid on string properties.
String format args: 
* {PropertyName} = The name of the property being validated
* {MinLength} = Minimum length
* {MaxLength} = Maximum length
* {TotalLength} = Number of characters entered
* {PropertyValue} = The current value of the property

## Less Than Validator
Description: Ensures that the value of the specified property is less than a particular value (or less than the value of another property) 
Example:
```csharp
//Less than a particular value
RuleFor(customer => customer.CreditLimit).LessThan(100);

//Less than another property
RuleFor(customer => customer.CreditLimit).LessThan(customer => customer.MaxCreditLimit);
```
Example error: 'Credit Limit' must be less than 100.
Notes: Only valid on types that implement IComparable<T>
String format args: 
* {PropertyName} = The name of the property being validated
* {ComparisonValue} - The value to which the property was compared
* {PropertyValue} = The current value of the property

## Less Than Or Equal Validator
Description: Ensures that the value of the specified property is less than or equal to a particular value (or less than or equal to the value of another property) 
Example:
```csharp
//Less than a particular value
RuleFor(customer => customer.CreditLimit).LessThanOrEqual(100);

//Less than another property
RuleFor(customer => customer.CreditLimit).LessThanOrEqual(customer => customer.MaxCreditLimit);
```
Example error: 'Credit Limit' must be less than or equal to 100.
Notes: Only valid on types that implement IComparable<T>
* {PropertyName} = The name of the property being validated
* {ComparisonValue} - The value to which the property was compared
* {PropertyValue} = The current value of the property

## Greater Than Validator
Description: Ensures that the value of the specified property is greater than a particular value (or greater than the value of another property) 
Example:
```csharp
//Greater than a particular value
RuleFor(customer => customer.CreditLimit).GreaterThan(0);

//Greater than another property
RuleFor(customer => customer.CreditLimit).GreaterThan(customer => customer.MinimumCreditLimit);
```
Example error: 'Credit Limit' must be greater than 0.
Notes: Only valid on types that implement IComparable<T>
* {PropertyName} = The name of the property being validated
* {ComparisonValue} - The value to which the property was compared
* {PropertyValue} = The current value of the property

## Greater Than Or Equal Validator
Description: Ensures that the value of the specified property is greater than or equal to a particular value (or greater than or equal to the value of another property) 
Example:
```csharp
//Greater than a particular value
RuleFor(customer => customer.CreditLimit).GreaterThanOrEqual(1);

//Greater than another property
RuleFor(customer => customer.CreditLimit).GreaterThanOrEqual(customer => customer.MinimumCreditLimit);
```
Example error: 'Credit Limit' must be greater than or equal to 1.
Notes: Only valid on types that implement IComparable<T>
* {PropertyName} = The name of the property being validated
* {ComparisonValue} - The value to which the property was compared
* {PropertyValue} = The current value of the property

## Predicate Validator
Also known as "Must"
Description: Passes the value of the specified property into a custom delegate that can perform custom validation logic on the value

Example:
```csharp
RuleFor(customer => customer.Surname).Must(surname => "Foo".Equals(surname));
```
Example error: The specified condition was not met for 'Surname' 
String format args: 
* {PropertyName} = The name of the property being validated
* {PropertyValue} = The current value of the property

Note that there is an additional overload for Must that also accepts an instance of the parent object being validated. This can be useful if you want to compare the current property with another property from inside the predicate:

```csharp
RuleFor(customer => customer.Surname).Must((customer, surname) => surname != customer.Forename)
```

(Note that in this particular example, it would be better to use the cross-property version of NotEqual)

## Regular Expression Validator
Description: Ensures that the value of the specified property matches the given regular expression. 
Example:
```csharp
RuleFor(customer => customer.Surname).Matches("some regex here");
```
Example error: 'Surname' is not in the correct format.
String format args: 
* {PropertyName} = The name of the property being validated
* {PropertyValue} = The current value of the property

## Email Validator
Description: Ensures that the value of the specified property is a valid email address format. 
Example:
```csharp
RuleFor(customer => customer.Email).EmailAddress();
```
Example error: 'Email' is not a valid email address.
String format args: 
* {PropertyName} = The name of the property being validated
* {PropertyValue} = The current value of the property
