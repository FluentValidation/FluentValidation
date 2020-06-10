namespace FluentValidation.Validators {
  using System;
  using System.Collections;
  using System.Linq;
  using Resources;

  public class NotContainsValidator : PropertyValidator {
    public NotContainsValidator(object comparisonValue) : base(new LanguageStringSource(nameof(NotContainsValidator))) {
      ValueToContains = comparisonValue;
    }

    protected override bool IsValid(PropertyValidatorContext context) {
      if (ValueToContains is string) {
        return !(string.IsNullOrWhiteSpace(ValueToContains.ToString()) ||
            !ValueToContains.ToString().Contains(context.PropertyValue.ToString()));
      }

      switch (ValueToContains) {
        case null:
        case ICollection c when c.Count == 0 || !c.Cast<object>().Contains(context.PropertyValue):
        case Array a when a.Length == 0 || !a.Cast<object>().Contains(context.PropertyValue):
        case IEnumerable e when !e.Cast<object>().Any() || !e.Cast<object>().Contains(context.PropertyValue):
        case IList l when !l.Cast<object>().Any() || !l.Cast<object>().Contains(context.PropertyValue):
          return false;
      }

      return true;
    }

    public object ValueToContains { get; private set; }
  }
}
