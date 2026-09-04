using ProjetoApi.Infraestrutura;
using System;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ProjetoApi.Especificacao
{
    // Busca o spec da BrasilAPI direto da página de docs.
    // A página é um app Next.js que injeta o spec inteiro dentro do
    // <script id="__NEXT_DATA__">, no caminho props.pageProps.spec.
    public class ObterJsonEstrutura
    {
        private const string UrlDocs = "https://brasilapi.com.br/docs";

        private static readonly ClienteHttp _http = new();

        // Não escapa acentos (grava "Não" em vez de "N\u00E3o") e indenta o JSON.
        private static readonly JsonSerializerOptions _jsonOpcoes = new()
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        // Baixa a página e devolve o spec como JSON (string) normalizado, retorna null se falhar (ex.: sem internet).
        public async Task<string?> BaixarJsonAsync()
        {
            var html = await _http.ObterRetornoAsync(UrlDocs);
            if(html is null)
                return null;

            return ExtrairJsonDoHtml(html);
        }

        // Isola o conteúdo do <script id="__NEXT_DATA__"> e navega até
        // props.pageProps.spec, devolvendo esse nó como JSON normalizado.
        public string ExtrairJsonDoHtml(string html)
        {
            const string marcadorId = "id=\"__NEXT_DATA__\"";

            //acha o script pelo id (independe da ordem dos atributos)
            var posId = html.IndexOf(marcadorId, StringComparison.Ordinal);
            if(posId < 0)
                throw new InvalidOperationException("Script __NEXT_DATA__ não encontrado no HTML.");

            //o conteúdo começa depois do '>' que fecha a tag de abertura
            var inicio = html.IndexOf('>', posId);
            if(inicio < 0)
                throw new InvalidOperationException("Tag __NEXT_DATA__ malformada.");
            inicio++; // pula o '>'

            //termina no próximo </script>
            var fim = html.IndexOf("</script>", inicio, StringComparison.Ordinal);
            if(fim < 0)
                throw new InvalidOperationException("Fechamento do __NEXT_DATA__ não encontrado.");

            var jsonNext = html.Substring(inicio, fim - inicio).Trim();

            //parseia e navega até o spec
            var raiz = JsonNode.Parse(jsonNext)
                ?? throw new InvalidOperationException("__NEXT_DATA__ não é um JSON válido.");

            var spec = raiz["props"]?["pageProps"]?["spec"]
                ?? throw new InvalidOperationException("Caminho props.pageProps.spec não encontrado.");

            return spec.ToJsonString(_jsonOpcoes);
        }
    }
}
