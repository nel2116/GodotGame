using System.Collections.Generic;

namespace Core.Utilities
{
    /// <summary>
    /// 検証結果
    /// </summary>
    public class ValidationResult
    {
        public IReadOnlyList<string> Errors { get; }

        public bool IsValid => Errors.Count == 0;

        public ValidationResult(IReadOnlyList<string> errors)
        {
            Errors = errors;
        }
    }
}
