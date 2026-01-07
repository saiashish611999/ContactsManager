using System.ComponentModel.DataAnnotations;

namespace ContactsManager.Services.Helper;
public static class ValidationHelper
{
    public static void ValidateModel(object model)
    {
        ValidationContext context = new ValidationContext(model);
        List<ValidationResult> results = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(model, context, results, true);
        if (!isValid)
        {
            string errorMessages = string.Join("\n", results.Select(err => err.ErrorMessage));
            throw new ArgumentException(results.FirstOrDefault()?.ErrorMessage);
        }
    }
}
