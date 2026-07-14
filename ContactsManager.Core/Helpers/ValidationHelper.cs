using System.ComponentModel.DataAnnotations;

namespace ContactsManager.Core.Helpers;
public static class ValidationHelper
{
    public static void ValidateRequest(object obj)
    {
        ValidationContext context = new ValidationContext(obj);

        List<ValidationResult> results = [];

        bool isValid = Validator.TryValidateObject(obj, context, results, validateAllProperties: true);

        if (!isValid)
        {
            string? errors = string.Join("\n", results.Select(err => err.ErrorMessage));

            throw new ArgumentException(errors);
        }
    }
}
