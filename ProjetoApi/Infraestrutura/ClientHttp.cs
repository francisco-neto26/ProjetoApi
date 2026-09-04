using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoApi.Infraestrutura
{
    //transporte HTTP: faz GET numa URL e devolve o corpo como texto.    
    public class ClienteHttp
    {
        // HttpClient estático
        private static readonly HttpClient _http = Criar();

        private static HttpClient Criar()
        {
            var http = new HttpClient();
            http.DefaultRequestHeaders.UserAgent.ParseAdd("ProjetoApi/1.0");
            return http;
        }

        // GET na URL. Devolve o corpo (inclusive em 4xx, que traz JSON de erro),
        // ou null se a requisição falhar (sem internet / timeout).
        public async Task<string?> ObterRetornoAsync(string url)
        {
            try
            {                
                var resposta = await _http.GetAsync(url);
                return await resposta.Content.ReadAsStringAsync();
            }
            catch(HttpRequestException)
            {
                return null;
            }
            catch(TaskCanceledException)
            {
                return null;
            }
        }
    }
}