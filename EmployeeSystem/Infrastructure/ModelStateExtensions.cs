using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EmployeeSystem.Infrastructure
{
    public static class ModelStateExtensions
    {
        public static object ToSimpleErrors(this ModelStateDictionary modelState)
        {
            return modelState
                .Where(x => x.Value is { Errors.Count: > 0 })
                .Select(x => new
                {
                    Field = x.Key,
                    Errors = x.Value!.Errors.Select(e => e.ErrorMessage)
                });
        }
    }
}