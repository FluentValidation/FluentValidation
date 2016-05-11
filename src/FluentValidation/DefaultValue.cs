namespace FluentValidation
{
    using System;

    public static class DefaultValue
    {
        public static object Resolve<TProperty>()
        {
            var underlyingType = Nullable.GetUnderlyingType(typeof(TProperty));
            return underlyingType == null ? default(TProperty) : ResolveUnderlyng(underlyingType);
        }

        static object ResolveUnderlyng(Type t)
        {
            return t.IsValueType ? Activator.CreateInstance(t) : null;
        }
    }
}