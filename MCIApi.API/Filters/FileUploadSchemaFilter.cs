using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;

namespace MCIApi.API.Filters
{
    public class FileUploadSchemaFilter : ISchemaFilter
    {
        public void Apply(Microsoft.OpenApi.Models.OpenApiSchema schema, SchemaFilterContext context)
        {
            // This filter runs during schema generation
            // We can use it to configure IFormFile schemas
            if (context.Type == typeof(IFormFile) || 
                context.Type == typeof(IFormFile[]) ||
                context.Type == typeof(System.Collections.Generic.List<IFormFile>))
            {
                schema.Type = "string";
                schema.Format = "binary";
            }
        }
    }
}

