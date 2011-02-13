namespace FluentValidation
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Internal;
    using Results;
    using Validators;

    public class SharedConditionRuleHelper<T>
    {
        Func<T, bool> whenPredicate;
        Dictionary<Type, IList<object>> ruleOptions;

        public SharedConditionRuleHelper()
            : this(x => true)
        {
        }

        public SharedConditionRuleHelper(Func<T, bool> whenPredicate)
        {
            this.whenPredicate = whenPredicate;
            this.ruleOptions = new Dictionary<Type, IList<object>>();
        }

        public void Unless(Func<T, bool> predicate)
        {
            foreach (var pair in ruleOptions)
            {
                Type typeOfRuleOptions = pair.Key;

                Type generic = typeof(UnlessVisistor<>);
                Type specific = generic.MakeGenericType(typeof(T), typeOfRuleOptions);
                ConstructorInfo ctorInfo = specific.GetConstructor(new Type[] { predicate.GetType() });
                object o = ctorInfo.Invoke(new object[] { predicate });
                IUnlessVisistor visitor = o as IUnlessVisistor;
                visitor.Visit(pair.Value);
            }
        }

        public interface IUnlessVisistor
        {
            void Visit(IList<object> rulesToVisit);
        }

        public class UnlessVisistor<TProperty> : IUnlessVisistor
        {
            Func<T, bool> unlessClause;

            public UnlessVisistor(Func<T, bool> unlessClause)
            {
                this.unlessClause = unlessClause;
            }

            public void Visit(IList<object> rulesToVisit)
            {
                foreach (var rule in rulesToVisit.Cast<IRuleBuilderOptions<T, TProperty>>())
                {
                    rule.Unless(unlessClause);
                }
            }
        }

        public SharedConditionRuleHelper<T> Add<TProperty>(IRuleBuilderOptions<T, TProperty> builderOptions)
        {
            builderOptions.When(whenPredicate);

            Type typeOfProperty = typeof(TProperty);

            if (!ruleOptions.ContainsKey(typeOfProperty))
            {
                ruleOptions.Add(typeOfProperty, new List<object>());
            }

            ruleOptions[typeOfProperty].Add(builderOptions);

            return this;
        }
    }
}
