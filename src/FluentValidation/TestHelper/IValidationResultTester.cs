namespace FluentValidation.TestHelper
{
    using System.Collections.Generic;
    using System.Reflection;
    using Results;

    public interface IValidationResultTester
    {
        IEnumerable<ValidationFailure> ShouldHaveValidationError(IEnumerable<MemberInfo> properties);
        void ShouldNotHaveValidationError(IEnumerable<MemberInfo> properties);
    }
}