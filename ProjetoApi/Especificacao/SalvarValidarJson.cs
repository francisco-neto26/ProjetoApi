using System;
using System.IO;
using System.Threading.Tasks;
using ProjetoApi.Especificacao;

namespace ProjetoApi.Especificacao
{
    // Mantém o arquivo local atualizado:
    // compara o JSON baixado com o salvo e grava só quando muda (ou quando não existe).
    // Se não houver internet, usa o arquivo local que já estiver salvo.
    public class SalvarValidarJson
    {
        // Arquivo na pasta de execução do console.
        private static readonly string CaminhoArquivo =
            Path.Combine(AppContext.BaseDirectory, "brasilapi-openapi.json");

        private readonly ObterJsonEstrutura _downloader = new();

        // Garante o arquivo local atualizado e devolve o caminho dele.
        public async Task<string> AtualizarJsonAsync()
        {
            var jsonBaixado = await _downloader.BaixarJsonAsync();

            //Se falha no download vai pro json salvo.
            if(jsonBaixado is null)
                return ArquivoLocal();

            //salva só se estiver diferente ou se ainda não existir.
            if(await validaJsonAsync(jsonBaixado))
                await File.WriteAllTextAsync(CaminhoArquivo, jsonBaixado);

            return CaminhoArquivo;
        }

        //valida se o JSON baixado é diferente do salvo ou se não existe.
        private static async Task<bool> validaJsonAsync(string baixado)
        {
            if(!File.Exists(CaminhoArquivo))
                return true;

            var atual = await File.ReadAllTextAsync(CaminhoArquivo);
            return !string.Equals(atual, baixado, StringComparison.Ordinal);
        }

        //usa o arquivo local se existir, senão falha.
        private static string ArquivoLocal()
        {
            if(File.Exists(CaminhoArquivo))
                return CaminhoArquivo;

            throw new InvalidOperationException(
                "Não foi possível baixar o spec e não há arquivo local para usar.");
        }
    }
}
