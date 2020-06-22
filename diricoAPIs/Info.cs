using Microsoft.OpenApi.Models;

namespace diricoAPIs
{
    internal class Info : OpenApiInfo
    {
        public string Title { get; set; }
        public string Version { get; set; }
    }
}