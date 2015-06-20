namespace FluentValidation.TestHelper.Fluent
{
    using System.Linq;
    using System.Reflection;
    using FluentValidation;
    using Results;

    public class FluentValidationResult<T, TValue> where T : class
    {
        public ValidationResult Result { get; private set; }
        public MemberAccessor<T, TValue> MemberAccessor { get; private set; }

        public FluentValidationResult(ValidationResult validationResult, MemberAccessor<T, TValue> memberAccessor)
        {
            Result = validationResult;
            MemberAccessor = memberAccessor;
        }

        public IFluentPropertyChain<TValue> Which
        {
            get
            {
                var resultTester = new FluentValidationResultTester<T, TValue>(this);
                return new FluentPropertyChain<TValue, TValue>(resultTester, Enumerable.Empty<MemberInfo>());
            }
        }
    }
}