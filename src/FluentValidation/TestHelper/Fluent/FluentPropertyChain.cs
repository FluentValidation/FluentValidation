namespace FluentValidation.TestHelper.Fluent
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using FluentValidation;
    using Results;

    internal class FluentPropertyChain<TValue, TValue1> : IFluentPropertyChain<TValue>
    {
        private readonly IFluentValidationResultTester validationResultTester;
        private readonly IEnumerable<MemberInfo> properties;

        public FluentPropertyChain(IFluentValidationResultTester validationResultTester, IEnumerable<MemberInfo> properties = null)
        {
            this.validationResultTester = validationResultTester;
            this.properties = properties ?? Enumerable.Empty<MemberInfo>();
        }

        public IFluentPropertyChain<TValue2> Property<TValue2>(Expression<Func<TValue, TValue2>> memberAccessor)
        {
            return new FluentPropertyChain<TValue2, TValue1>(validationResultTester, properties.Concat(new[] { ((MemberAccessor<TValue, TValue2>)memberAccessor).Member }));
        }

        public IEnumerable<ValidationFailure> ShouldHaveError()
        {
           return validationResultTester.ShouldHaveError(properties);
        }

        public void ShouldNotHaveError()
        {
            validationResultTester.ShouldNotHaveError(properties);
        }
    }
}