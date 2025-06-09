using System.Collections.Generic;

namespace Core.Utilities
{
    /// <summary>
    /// 検証ユーティリティ
    /// </summary>
    public class Validator<T>
    {
        private readonly List<ValidationRule<T>> _rules = new();

        public void AddRule(ValidationRule<T> rule)
        {
            _rules.Add(rule);
        }

        public ValidationResult Validate(T value)
        {
            var errors = new List<string>();

            foreach (var rule in _rules)
            {
                if (!rule.Validate(value))
                {
                    errors.Add(rule.ErrorMessage);
                }
            }

            return new ValidationResult(errors);
        }
    }
}
