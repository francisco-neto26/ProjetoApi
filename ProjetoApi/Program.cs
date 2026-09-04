using ProjetoApi.Especificacao;
using ProjetoApi.Services;
using ProjetoApi.UI;

internal class Program
{
    static async Task Main(string[] args)
    {
        var caminho = await new SalvarValidarJson().AtualizarJsonAsync();

        var processador = new ProcessarJson();
        var categorias = processador.Carregar(caminho);

        var baseUrl = processador.Server?.Url ?? "https://brasilapi.com.br/api";
        var api = new BrasilApiService(baseUrl);

        await new MenuConsole(categorias, api).IniciarAsync();
    }
}
