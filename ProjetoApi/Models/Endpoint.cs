using System.Collections.Generic;
using System.Reflection.Metadata;

namespace ProjetoApi.Models
{
    //Endpoint da API montada a partir de paths[]. É o que o menu aciona.
    public class Endpoint
    {
        
        //Verbo HTTP é sempre "GET".
        public string Metodo { get; set; } = "GET";

        //Completo da rota Ex.: "/cnpj/v1/{cnpj}".
        public string Caminho { get; set; } = string.Empty;

        //Descrição amigável.
        public string? Resumo { get; set; }

        //Parâmetros que o endpoint aceita.
        public List<Parametro> Parametros { get; set; } = new();
    }
}
