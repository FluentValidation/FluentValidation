namespace FluentValidation.TestHelper
{
    using System.Linq;
    using System.Reflection;
    using FluentValidation;
    using Results;

    public class TestValidationResult<T, TValue> where T : class
    {
        public ValidationResult Result { get; private set; }
        public MemberAccessor<T, TValue> MemberAccessor { get; private set; }

        public TestValidationResult(ValidationResult validationResult, MemberAccessor<T, TValue> memberAccessor)
        {
            Result = validationResult;
            MemberAccessor = memberAccessor;
        }

        public ITestPropertyChain<TValue> Which
        {
            get
            {
                var resultTester = new ValidationResultTester<T, TValue>(this);
                return new TestPropertyChain<TValue, TValue>(resultTester, Enumerable.Empty<MemberInfo>());
            }
        }
    }
}