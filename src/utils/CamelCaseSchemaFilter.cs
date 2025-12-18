using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json;

namespace talearc_backend.src.utils;

/// <summary>
/// Swagger Schema 小驼峰命名过滤器
/// </summary>
public class CamelCaseSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema.Properties == null || schema.Properties.Count == 0)
            return;

        var properties = schema.Properties.ToDictionary(
            p => JsonNamingPolicy.CamelCase.ConvertName(p.Key),
            p => p.Value
        );

        schema.Properties.Clear();
        foreach (var property in properties)
        {
            schema.Properties.Add(property);
        }
    }
}
