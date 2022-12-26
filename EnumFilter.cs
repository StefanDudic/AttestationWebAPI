using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AttestationProject
{
    internal sealed class EnumSchemaFilter : ISchemaFilter
    {
        // Enabling Swagger to show enums as value 
        public void Apply(OpenApiSchema model, SchemaFilterContext context)
        {
            if (context.Type.IsEnum)
            {
                model.Enum.Clear();
                Enum
                   .GetNames(context.Type)
                   .ToList()
                   .ForEach(name => model.Enum.Add(new OpenApiString($"{name}")));
                model.Type = "string";
                model.Format = string.Empty;
            }
        }
    }
}
