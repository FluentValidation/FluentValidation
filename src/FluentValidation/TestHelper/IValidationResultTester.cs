namespace FluentValidation.TestHelper
{
    using System.Collections.Generic;
    using System.Reflection;
    using Results;

    public interface IValidationResultTester
    {
        IEnumerable<ValidationFailure> ShouldHaveError(IEnumerable<MemberInfo> properties);
        void ShouldNotHaveError(IEnumerable<MemberInfo> properties);
    }
}