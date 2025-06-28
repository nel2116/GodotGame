using System;

namespace Core.Utilities
{
    /// <summary>
    /// 単一の検証ルール
    /// </summary>
    public class ValidationRule<T>
    {
        public Func<T, bool> Validate { get; }
        public string ErrorMessage { get; }

        public ValidationRule(Func<T, bool> validate, string errorMessage)
        {
            Validate = validate;
            ErrorMessage = errorMessage;
        }
    }
}
