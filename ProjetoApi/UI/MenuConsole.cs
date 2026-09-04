using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjetoApi.Models;
using ProjetoApi.Services;
using ProjetoApi.Utils;

namespace ProjetoApi.UI
{
    // Orquestra o menu: Categoria -> Endpoint -> Parâmetros -> chama a API -> exibe.
    // Recebe as categorias e o service prontos (montados no Program).
    internal class MenuConsole
    {
        private readonly List<Categoria> _categorias;
        private readonly BrasilApiService _api;

        public MenuConsole(List<Categoria> categorias, BrasilApiService api)
        {
            _categorias = categorias;
            _api = api;
        }

        public async Task IniciarAsync()
        {
            while(true)
            {
                var categoria = EscolherCategoria();
                if(categoria is null)
                    break;

                var endpoint = EscolherEndpoint(categoria);
                if(endpoint is null)
                    continue;

                await ExecutarAsync(endpoint);
            }
        }

        // 1º nível: categorias (só as que têm endpoints).
        private Categoria? EscolherCategoria()
        {
            var categorias = _categorias
                .Where(c => c.Endpoints.Count > 0)
                .OrderBy(c => c.Nome)
                .ToList();

            ConsoleUI.Titulo("Categorias");
            ConsoleUI.ListarItens(categorias.Select(c => c.Nome));
            System.Console.WriteLine("0 - Sair");

            var opcao = ConsoleUI.LerOpcao(categorias.Count);
            return opcao == 0 ? null : categorias[opcao - 1];
        }

        // 2º nível: endpoints da categoria.
        private Endpoint? EscolherEndpoint(Categoria categoria)
        {
            var endpoints = categoria.Endpoints;

            ConsoleUI.Titulo(categoria.Nome);
            ConsoleUI.ListarItens(endpoints.Select(e => e.Resumo ?? e.Caminho));
            System.Console.WriteLine("0 - Voltar");

            var opcao = ConsoleUI.LerOpcao(endpoints.Count);
            return opcao == 0 ? null : endpoints[opcao - 1];
        }

        // 3º nível: coleta parâmetros, chama a API e exibe o resultado.
        private async Task ExecutarAsync(Endpoint endpoint)
        {
            var valores = LerParametros(endpoint);

            ConsoleUI.Titulo("Consultando...");         
            var resposta = await _api.ConsultarAsync(endpoint, valores);

            if(resposta is null)
                System.Console.WriteLine("Falha ao consultar (sem conexão ou tempo esgotado).");
            else
                System.Console.WriteLine(FormatadorResposta.Formatar(resposta, endpoint));

            ConsoleUI.Pausar();
        }

        // Pergunta um valor por parâmetro. Obrigatório re-pergunta até preencher;
        // opcional pode ficar vazio (aí não entra na URL).
        private static Dictionary<string, string> LerParametros(Endpoint endpoint)
        {
            var valores = new Dictionary<string, string>();

            foreach(var p in endpoint.Parametros)
            {
                string valor;
                while(true)
                {
                    var rotulo = p.Obrigatorio ? p.Nome : $"{p.Nome} (opcional)";
                    valor = ConsoleUI.LerTexto(rotulo);

                    if(!p.Obrigatorio || !string.IsNullOrEmpty(valor))
                        break;

                    System.Console.WriteLine("Esse parâmetro é obrigatório.");
                }

                if(!string.IsNullOrEmpty(valor))
                    valores[p.Nome] = valor;
            }

            return valores;
        }
    }
}