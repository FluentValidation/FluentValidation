namespace FluentValidation.TestHelper.Fluent
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Results;

    public interface IFluentPropertyChain<TValue>
    {
        IFluentPropertyChain<TValue1> Property<TValue1>(Expression<Func<TValue, TValue1>> memberAccessor);
        IEnumerable<ValidationFailure> ShouldHaveError();
        void ShouldNotHaveError();
    }
}