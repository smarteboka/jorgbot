using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ValidationExtensions
    {
        public static IEnumerable<string> ValidationErrors(this object @this)
        {
            var context = new ValidationContext(@this, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(@this, context, results, true);
            foreach (var validationResult in results)
            {
                yield return validationResult.ErrorMessage;
            }
        }
    }
}