using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AspNetCoreIdentityApp.Web.Extensions
{
    public static class ModelStateExtension
    {
        public static void AddModelErrorList(this ModelStateDictionary modelState, List<string> errors)
        {
            errors.ForEach(e =>
            {
                modelState.AddModelError(string.Empty, e);
            });
        }
    }
}
