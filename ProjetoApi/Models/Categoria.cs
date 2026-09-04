using System.Text.Json.Serialization;

namespace ProjetoApi.Models
{
    // Uma categoria de consultas vem de tags[] no JSON. 1º nível do menu.
    public class Categoria
    {
        [JsonPropertyName("name")]
        public string Nome { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string? Descricao { get; set; }

        // Lista de endpoints que pertencem a essa categoria.
        public List<Endpoint> Endpoints { get; set; } = new();
    }
}