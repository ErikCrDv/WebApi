using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApi.Filters
{
    public class MyActionFilter : IActionFilter
    {
        private readonly ILogger<MyActionFilter> logger;

        public MyActionFilter(ILogger<MyActionFilter> logger)
        {
            this.logger = logger;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            logger.LogInformation("AFTER EXECUTING");
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            logger.LogInformation("BEFORE EXECUTING");
        }
    }
}
