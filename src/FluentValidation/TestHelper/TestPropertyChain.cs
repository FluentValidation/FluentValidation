namespace FluentValidation.TestHelper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using FluentValidation;
    using Results;

    internal class TestPropertyChain<TValue, TValue1> : ITestPropertyChain<TValue>
    {
        private readonly IValidationResultTester validationResultTester;
        private readonly IEnumerable<MemberInfo> properties;

        public TestPropertyChain(IValidationResultTester validationResultTester, IEnumerable<MemberInfo> properties = null)
        {
            this.validationResultTester = validationResultTester;
            this.properties = properties ?? Enumerable.Empty<MemberInfo>();
        }

        public ITestPropertyChain<TValue2> Property<TValue2>(Expression<Func<TValue, TValue2>> memberAccessor)
        {
            return new TestPropertyChain<TValue2, TValue1>(validationResultTester, properties.Concat(new[] { ((MemberAccessor<TValue, TValue2>)memberAccessor).Member }));
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