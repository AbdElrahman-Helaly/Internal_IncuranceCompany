using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Reflection;

namespace MCIApi.API.Filters
{
    public class FileUploadParameterFilter : IParameterFilter
    {
        public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
        {
            var paramInfo = context.ParameterInfo;
            if (paramInfo != null)
            {
                var fromFormAttr = paramInfo.GetCustomAttribute<FromFormAttribute>();
                var bindNeverAttr = paramInfo.GetCustomAttribute<Microsoft.AspNetCore.Mvc.ModelBinding.BindNeverAttribute>();
                
                if (fromFormAttr != null && 
                    (paramInfo.ParameterType == typeof(IFormFile) || 
                     paramInfo.ParameterType == typeof(IFormFile[]) ||
                     paramInfo.ParameterType == typeof(List<IFormFile>)))
                {
                    // Always exclude IFormFile parameters from being treated as regular parameters
                    // The operation filter will handle them in the request body
                    // Set both In and Schema to null to completely suppress the parameter
                    parameter.In = null;
                    parameter.Schema = null;
                    parameter.Name = null;
                    parameter.Description = null;
                    parameter.Required = false;
                }
            }
        }
    }
}

