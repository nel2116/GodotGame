using NUnit.Framework;
using System.Threading.Tasks;
using Core.Utilities;

namespace Tests.Core.Utilities
{
    public class AsyncValidatorTests
    {
        [Test]
        public async Task ValidateAsync_AllRulesPass_ReturnsValid()
        {
            var validator = new AsyncValidator<int>();
            validator.AddRule(new AsyncValidationRule<int>(v => Task.FromResult(v > 0), "must be positive"));
            var result = await validator.ValidateAsync(1);
            Assert.IsTrue(result.IsValid);
        }

        [Test]
        public async Task ValidateAsync_Fails_ReturnsErrors()
        {
            var validator = new AsyncValidator<int>();
            validator.AddRule(new AsyncValidationRule<int>(v => Task.FromResult(v > 0), "must be positive"));
            var result = await validator.ValidateAsync(-1);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
        }
    }
}
