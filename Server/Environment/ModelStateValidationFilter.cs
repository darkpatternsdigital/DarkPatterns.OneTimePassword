using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DarkPatterns.OneTimePassword.Environment;

public class ModelStateValidationFilter : IActionFilter
{
	public void OnActionExecuting(ActionExecutingContext context)
	{
		if (!context.ModelState.IsValid)
		{
			context.Result = new BadRequestResult();
		}
	}

	public void OnActionExecuted(ActionExecutedContext context) { }
}
