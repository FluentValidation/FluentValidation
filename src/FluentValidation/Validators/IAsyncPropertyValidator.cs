namespace FluentValidation.Validators {
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Results;

    /// <summary>
    /// A custom asynchronous property validator.
    /// This interface should not be implemented directly in your code as it is subject to change.
    /// Please inherit from <see cref="AsyncPropertyValidator">AsyncPropertyValidator</see> instead.
    /// </summary>
    public interface IAsyncPropertyValidator : IPropertyValidator {
        Task<IEnumerable<ValidationFailure>> ValidateAsync(PropertyValidatorContext context);
    }
}