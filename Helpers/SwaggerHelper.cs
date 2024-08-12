using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace TenderInfoAPI.Helpers
{
    public static class SwaggerHelper
    {
        public static OpenApiSchema CreateEnumSchema<T>() where T : struct, Enum
        {
            return new OpenApiSchema
            {
                Type = "string",
                Enum = Enum.GetNames(typeof(T))
                            .Select(name => new OpenApiString(name) as IOpenApiAny)
                            .ToList()
            };
        }
    }
}
