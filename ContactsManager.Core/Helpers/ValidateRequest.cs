using System.ComponentModel.DataAnnotations;

namespace ContactsManager.Core.Helpers;
public static class ValidateRequest
{
    public static void ValidateRequestObject(object obj)
    {
        ValidationContext context = new ValidationContext(obj);

        List<ValidationResult> results = new List<ValidationResult>();

        bool isValid = Validator.TryValidateObject(obj, context, results, true);

        if (!isValid)
        {
            string? errorMessage = string.Join("\n", results.Select(err => err.ErrorMessage));

            throw new ArgumentException(errorMessage);
        }
    }
}
