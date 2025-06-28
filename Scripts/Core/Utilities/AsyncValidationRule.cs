using System;

namespace Core.Utilities
{
    /// <summary>
    /// 非同期バリデーションルール
    /// </summary>
    public class AsyncValidationRule<T>
    {
        public Func<T, System.Threading.Tasks.Task<bool>> ValidateAsync { get; }
        public string ErrorMessage { get; }

        public AsyncValidationRule(Func<T, System.Threading.Tasks.Task<bool>> validateAsync, string errorMessage)
        {
            ValidateAsync = validateAsync;
            ErrorMessage = errorMessage;
        }
    }
}
