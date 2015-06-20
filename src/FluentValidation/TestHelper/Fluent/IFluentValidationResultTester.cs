namespace FluentValidation.TestHelper.Fluent
{
    using System.Collections.Generic;
    using System.Reflection;
    using Results;

    public interface IFluentValidationResultTester
    {
        IEnumerable<ValidationFailure> ShouldHaveError(IEnumerable<MemberInfo> properties);
        void ShouldNotHaveError(IEnumerable<MemberInfo> properties);
    }
}