using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Utilities
{
    /// <summary>
    /// 非同期検証ユーティリティ
    /// </summary>
    public class AsyncValidator<T>
    {
        private readonly List<AsyncValidationRule<T>> _rules = new();

        public void AddRule(AsyncValidationRule<T> rule)
        {
            _rules.Add(rule);
        }

        public async Task<ValidationResult> ValidateAsync(T value)
        {
            var errors = new List<string>();
            foreach (var rule in _rules)
            {
                if (!await rule.ValidateAsync(value))
                {
                    errors.Add(rule.ErrorMessage);
                }
            }
            return new ValidationResult(errors);
        }
    }
}
