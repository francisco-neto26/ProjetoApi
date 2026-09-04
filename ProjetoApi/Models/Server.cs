using System.Text.Json.Serialization;

namespace ProjetoApi.Models
{
    public class Server
    {
        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;
    }
}
