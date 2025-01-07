using System.ComponentModel.DataAnnotations;

namespace ContactsManager.Services.Helpers;

public static class ModelValidator
{
    public static void ModelValidationHelper(object modelObject)
    {
        ValidationContext context = new ValidationContext(modelObject);
        List<ValidationResult> validationResults = new List<ValidationResult>();

        bool isValid =  Validator.TryValidateObject(modelObject, context, validationResults, true);

        if (!isValid)
        {
            throw new ArgumentException(validationResults.First().ErrorMessage);
        }
    }
}
