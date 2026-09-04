using ProjetoApi.Models;
using System.Text;
using System.Text.Json.Nodes;

namespace ProjetoApi.Utils
{
    // Transforma o JSON retornado pela API em texto amigável (campo: valor).
    // Objeto -> lista de campos; array -> itens numerados; aninhados -> indentados.
    public static class FormatadorResposta
    {
        public static string Formatar(string? json, Endpoint endpoint)
        {
            Console.WriteLine(json);
            if(string.IsNullOrWhiteSpace(json))
                return "(sem resposta)";

            JsonNode? jsonNode;
            try { jsonNode = JsonNode.Parse(json); }
            catch { return json; }

            if(jsonNode is null)
                return "(resposta vazia)";

            var jsonRetorno = new StringBuilder();
            Escrever(jsonNode, jsonRetorno, 0);  
            return jsonRetorno.ToString();
        }

        private static void Escrever(JsonNode? jsonNode, StringBuilder jsonRetorno, int nivel)
        {
            var indent = new string(' ', nivel * 2);

            switch(jsonNode)
            {
                case JsonObject obj:
                    foreach(var prop in obj)
                        EscreverPropriedade(prop.Key, prop.Value, jsonRetorno, nivel);
                    break;

                case JsonArray arr:
                    var i = 1;
                    foreach(var item in arr)
                    {
                        jsonRetorno.AppendLine($"{indent}[{i}]");
                        Escrever(item, jsonRetorno, nivel + 1);
                        i++;
                    }
                    break;

                default:
                    jsonRetorno.AppendLine($"{indent}{jsonNode?.ToString()}");
                    break;
            }
        }

        private static void EscreverPropriedade(string nome, JsonNode? valor, StringBuilder jsonRetorno, int nivel)
        {
            var indent = new string(' ', nivel * 2);

            if(valor is JsonObject || valor is JsonArray)
            {
                jsonRetorno.AppendLine($"{indent}{nome}:");
                Escrever(valor, jsonRetorno, nivel + 1);
            }
            else
            {
                jsonRetorno.AppendLine($"{indent}{nome}: {valor?.ToString()}");
            }
        }
    }
}