using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace MCIApi.API.Filters
{
    public class FileUploadOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Get all parameters with [FromForm] attribute
            var allParameters = context.MethodInfo.GetParameters().ToList();
            
            var fileParameters = allParameters
                .Where(p => p.GetCustomAttribute<FromFormAttribute>() != null &&
                           (p.ParameterType == typeof(IFormFile) || 
                            p.ParameterType == typeof(IFormFile[]) ||
                            p.ParameterType == typeof(List<IFormFile>)))
                .ToList();

            var formParameters = allParameters
                .Where(p => p.GetCustomAttribute<FromFormAttribute>() != null)
                .ToList();

            if (fileParameters.Any() || formParameters.Any())
            {
                // Clear existing parameters that are from form
                if (operation.Parameters != null)
                {
                    var parametersToRemove = operation.Parameters
                        .Where(p => formParameters.Any(fp => fp.Name?.Equals(p.Name, StringComparison.OrdinalIgnoreCase) == true))
                        .ToList();
                    
                    foreach (var paramToRemove in parametersToRemove)
                    {
                        operation.Parameters.Remove(paramToRemove);
                    }
                }

                // Create or update request body for multipart/form-data
                if (operation.RequestBody == null)
                {
                    operation.RequestBody = new OpenApiRequestBody
                    {
                        Required = true,
                        Content = new Dictionary<string, OpenApiMediaType>()
                    };
                }

                if (!operation.RequestBody.Content.ContainsKey("multipart/form-data"))
                {
                    operation.RequestBody.Content["multipart/form-data"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Required = new HashSet<string>(),
                            Properties = new Dictionary<string, OpenApiSchema>()
                        }
                    };
                }

                var schema = operation.RequestBody.Content["multipart/form-data"].Schema;
                if (schema.Properties == null)
                {
                    schema.Properties = new Dictionary<string, OpenApiSchema>();
                }
                if (schema.Required == null)
                {
                    schema.Required = new HashSet<string>();
                }

                // Add file parameters (direct IFormFile parameters)
                foreach (var param in fileParameters)
                {
                    var paramName = param.Name ?? "file";
                    schema.Properties[paramName] = new OpenApiSchema
                    {
                        Type = "string",
                        Format = "binary",
                        Description = "File to upload"
                    };
                    schema.Required.Add(paramName);
                }

                // Handle DTO parameters - expand their properties into form fields
                foreach (var param in formParameters.Where(p => !fileParameters.Contains(p)))
                {
                    var paramType = param.ParameterType;
                    
                    // Check if it's a DTO/class (not a primitive or simple type)
                    if (paramType.IsClass && paramType != typeof(string))
                    {
                        // Expand DTO properties into form fields
                        var properties = paramType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                        foreach (var prop in properties)
                        {
                            var propName = prop.Name;
                            var propType = prop.PropertyType;
                            
                            // Skip if property has [BindNever] or similar attributes
                            if (prop.GetCustomAttribute<Microsoft.AspNetCore.Mvc.ModelBinding.BindNeverAttribute>() != null)
                                continue;
                            
                            OpenApiSchema propSchema;
                            
                            // Handle IFormFile properties
                            if (propType == typeof(IFormFile) || propType == typeof(IFormFile[]) || propType == typeof(List<IFormFile>))
                            {
                                propSchema = new OpenApiSchema
                                {
                                    Type = "string",
                                    Format = "binary",
                                    Description = "File to upload"
                                };
                                
                                // Check if required
                                var requiredAttr = prop.GetCustomAttribute<RequiredAttribute>();
                                if (requiredAttr != null)
                                {
                                    schema.Required.Add(propName);
                                }
                            }
                            else
                            {
                                // Get the underlying type if nullable
                                var underlyingType = Nullable.GetUnderlyingType(propType) ?? propType;
                                
                                propSchema = MapTypeToOpenApiSchema(underlyingType);
                                
                                if (propSchema == null)
                                {
                                    // Skip complex nested objects or unsupported types
                                    continue;
                                }
                                
                                // Check if required
                            // Only add to required if there's an explicit [Required] attribute
                                // For UPDATE DTOs, all fields should be optional unless explicitly marked as [Required]
                                var requiredAttr = prop.GetCustomAttribute<RequiredAttribute>();
                                if (requiredAttr != null)
                                {
                                    schema.Required.Add(propName);
                                }
                                
                                schema.Properties[propName] = propSchema;
                            }
                        }
                    }
                    else
                    {
                        // Handle simple form parameters (non-DTO)
                        var paramName = param.Name ?? "value";
                        var underlyingType = Nullable.GetUnderlyingType(paramType) ?? paramType;
                        
                        OpenApiSchema paramSchema;
                        if (underlyingType == typeof(string))
                        {
                            paramSchema = new OpenApiSchema
                            {
                                Type = "string"
                            };
                        }
                        else if (underlyingType.IsPrimitive || underlyingType.IsValueType)
                        {
                            paramSchema = new OpenApiSchema
                            {
                                Type = "string" // Form data is always string
                            };
                        }
                        else
                        {
                            paramSchema = new OpenApiSchema
                            {
                                Type = "string"
                            };
                        }

                        schema.Properties[paramName] = paramSchema;
                        
                        // Only add to required if parameter is not nullable and doesn't have a default value
                        var isNullable = Nullable.GetUnderlyingType(paramType) != null;
                        
                        if (!isNullable)
                        {
                            if (param.HasDefaultValue && param.DefaultValue == null)
                            {
                                // Optional parameter
                            }
                            else if (!param.HasDefaultValue)
                            {
                                schema.Required.Add(paramName);
                            }
                        }
                    }
                }
            }
        }

        private OpenApiSchema? MapTypeToOpenApiSchema(Type type)
        {
            if (type == typeof(string))
            {
                return new OpenApiSchema { Type = "string" };
            }
            else if (type == typeof(bool))
            {
                return new OpenApiSchema { Type = "boolean" };
            }
            else if (type == typeof(int) || type == typeof(short) || type == typeof(long) || type == typeof(byte))
            {
                return new OpenApiSchema { Type = "integer", Format = type == typeof(int) ? "int32" : type == typeof(long) ? "int64" : "int32" };
            }
            else if (type == typeof(decimal) || type == typeof(float) || type == typeof(double))
            {
                return new OpenApiSchema { Type = "number", Format = type == typeof(decimal) ? "double" : type == typeof(float) ? "float" : "double" };
            }
            else if (type == typeof(DateTime) || type == typeof(DateTimeOffset) || type == typeof(DateOnly))
            {
                return new OpenApiSchema { Type = "string", Format = "date-time" };
            }
            else if (type == typeof(Guid))
            {
                return new OpenApiSchema { Type = "string", Format = "uuid" };
            }
            else if (type.IsEnum)
            {
                return new OpenApiSchema { Type = "integer", Format = "int32" };
            }
            else if (type.IsArray || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)))
            {
                var elementType = type.IsArray ? type.GetElementType() : type.GetGenericArguments()[0];
                var elementSchema = MapTypeToOpenApiSchema(elementType!);
                if (elementSchema != null)
                {
                    return new OpenApiSchema
                    {
                        Type = "array",
                        Items = elementSchema
                    };
                }
            }
            
            // For other types, return null to skip them
            return null;
        }
    }
}

