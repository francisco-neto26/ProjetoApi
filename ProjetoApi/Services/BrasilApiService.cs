using ProjetoApi.Infraestrutura;
using ProjetoApi.Models;

namespace ProjetoApi.Services
{
    // Aciona a BrasilAPI para o endpoint escolhido e devolve o corpo do retorno.
    public class BrasilApiService
    {
        private readonly string _baseUrl;
        private readonly ClienteHttp _http = new();

        public BrasilApiService(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public async Task<string?> ConsultarAsync(
            Endpoint endpoint, IReadOnlyDictionary<string, string> valores)
        {   
            
            var url = _baseUrl + EndpointCompleto(endpoint, valores);
            return await _http.ObterRetornoAsync(url);
        }

        // Substitui os parametros e monta a query string.
        private static string EndpointCompleto(
            Endpoint endpoint, IReadOnlyDictionary<string, string> valores)
        {
            var caminho = endpoint.Caminho;
            var query = new List<string>();

            foreach(var parametro in endpoint.Parametros)
            {
                if(!valores.TryGetValue(parametro.Nome, out var valor) || string.IsNullOrEmpty(valor))
                    continue;

                var v = Uri.EscapeDataString(valor);
                Console.WriteLine($"Substituindo parametro {parametro.Nome} com valor {v} no local {parametro.Local}");
                if(parametro.Local.Equals("path", StringComparison.OrdinalIgnoreCase))
                    caminho = caminho.Replace("{" + parametro.Nome + "}", v);
                else if(parametro.Local.Equals("query", StringComparison.OrdinalIgnoreCase))
                    query.Add($"{Uri.EscapeDataString(parametro.Nome)}={v}");
            }

            if(query.Count > 0)
                caminho += "?" + string.Join("&", query);

            return caminho;
        }
    }
}