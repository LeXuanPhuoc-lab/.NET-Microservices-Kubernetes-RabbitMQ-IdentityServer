using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace AuctionService.Validations
{
    public static class ValidationExtension
    {
        public static ValidationProblemDetails ToValidationProblemDetails(this ValidationResult result)
        {
            // Init validation problems detail
            var validationProblemDetails = new ValidationProblemDetails()
            {
                Status = StatusCodes.Status400BadRequest
            };

            foreach(var validationFailure in result.Errors)
            {
                // Check exist validation error
                if(!validationProblemDetails.Errors.ContainsKey(validationFailure.PropertyName)){ // Not exist
                    // Add error to problems detail
                    validationProblemDetails.Errors.Add(new KeyValuePair<string, string[]>(
                        validationFailure.PropertyName,
                        new[]{validationFailure.ErrorMessage}));
                }else{
                    validationProblemDetails.Errors[validationFailure.PropertyName].Concat(
                        new[]{validationFailure.ErrorMessage});
                }
            }

            return validationProblemDetails;
        }
    }


}