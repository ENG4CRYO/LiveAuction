using LiveAuction.Application.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Results;

namespace LiveAuction.api.Factories
{
    public class CustomResultFactory : IFluentValidationAutoValidationResultFactory
    {
        public IActionResult CreateActionResult(ActionExecutingContext context, ValidationProblemDetails? validationProblemDetails)
        {
            var errors = context.ModelState
               .Where(e => e.Value.Errors.Count > 0)
               .ToDictionary(
                   kvp => kvp.Key, 
                   kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
               );

            var response = ApiResponse<object>.Failure("Validation Failed", errors);


            return new BadRequestObjectResult(response);
        }
    }
}