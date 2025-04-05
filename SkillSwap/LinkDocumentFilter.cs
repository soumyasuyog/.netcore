using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class LinkDocumentFilter : IDocumentFilter
{
    private readonly string _baseUrl;

    public LinkDocumentFilter ( string baseUrl )
    {
        _baseUrl = baseUrl;
    }

    public void Apply ( OpenApiDocument swaggerDoc, DocumentFilterContext context )
    {
        swaggerDoc.ExternalDocs = new OpenApiExternalDocs
        {
            Description = "API documentation",
            Url = new System.Uri(_baseUrl)
        };

    }


    public class BaseUrlDocumentFilter : IDocumentFilter
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BaseUrlDocumentFilter ( IHttpContextAccessor httpContextAccessor )
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Apply ( OpenApiDocument swaggerDoc, DocumentFilterContext context )
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";

            swaggerDoc.Servers.Clear();
            swaggerDoc.Servers.Add(new OpenApiServer { Url = baseUrl });
        }
    }
}
