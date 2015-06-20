namespace FluentValidation.TestHelper
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Results;

    public interface ITestPropertyChain<TValue>
    {
        ITestPropertyChain<TValue1> Property<TValue1>(Expression<Func<TValue, TValue1>> memberAccessor);
        IEnumerable<ValidationFailure> ShouldHaveValidationError();
        void ShouldNotHaveValidationError();
    }
}