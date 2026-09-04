namespace ProjetoApi.Models
{
    //Parametros do Endpoint vem de parameters[] no path.
    public class Parametro
    {
        // Nome.
        public string Nome { get; set; } = string.Empty;

        //Onde entra: "path" ou "query".
        public string Local { get; set; } = string.Empty;

        //Para tratar parametro obrigatorio.
        public bool Obrigatorio { get; set; }

        //Tipo declarado (string, integer...).
        public string? Tipo { get; set; }
    }
}
