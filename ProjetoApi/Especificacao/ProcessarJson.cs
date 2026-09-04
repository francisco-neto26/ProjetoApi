using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using ProjetoApi.Models;

namespace ProjetoApi.Especificacao
{
    // Lê o JSON local e monta a árvore
    // Categoria -> Endpoint -> Parametro que o menu consome (tudo em memória, 1x no startup).
    public class ProcessarJson
    {
        public Server? Server { get; private set; }
        // Verbos HTTP válidos.
        private static readonly HashSet<string> Verbos =
            new(StringComparer.OrdinalIgnoreCase)
            { "get", "post", "put", "delete", "patch" };

        // Lê o JSON e devolve as categorias já com seus endpoints.
        public List<Categoria> Carregar(string caminhoJson)
        {
            var json = File.ReadAllText(caminhoJson);
            var raiz = JsonNode.Parse(json)
                ?? throw new InvalidOperationException("Spec local não é um JSON válido.");

            //cria uma Categoria por tag (indexada pelo nome, pra achar rápido depois)
            var categorias = new Dictionary<string, Categoria>(StringComparer.OrdinalIgnoreCase);

            if(raiz["tags"] is JsonArray tags)
            {
                foreach(var tag in tags)
                {
                    var nome = (string?)tag?["name"];
                    if(string.IsNullOrEmpty(nome))
                        continue;

                    categorias[nome] = new Categoria
                    {
                        Nome = nome,
                        Descricao = (string?)tag?["description"]
                    };
                }
            }

            //percorre os paths e anexa cada endpoint na categoria (tag) dele
            if(raiz["paths"] is JsonObject paths)
            {
                foreach(var caminho in paths)
                {
                    if(caminho.Value is not JsonObject operacoes)
                        continue;

                    foreach(var verbo in operacoes)
                    {
                        if(!Verbos.Contains(verbo.Key) || verbo.Value is null)
                            continue;

                        var op = verbo.Value;

                        var endpoint = new Endpoint
                        {
                            Metodo = verbo.Key.ToUpperInvariant(),
                            Caminho = caminho.Key.ToLowerInvariant(),
                            Resumo = (string?)op["summary"],
                            Parametros = LerParametros(op["parameters"])
                        };

                        var categoria = ObterCategoria(categorias, op);
                        categoria.Endpoints.Add(endpoint);
                    }
                }
            }


            if(raiz["servers"] is JsonArray servers)
            {
                var url = (string?)servers.FirstOrDefault()?["url"] ?? string.Empty;

                if(!string.IsNullOrEmpty(url))
                {
                    url = url.TrimEnd('/') + "/api";                    
                }                    

                Server = new Server { Url = url.ToLowerInvariant() };
            }

            return categorias.Values.ToList();
        }

        // Monta os parâmetros de uma operação.
        private static List<Parametro> LerParametros(JsonNode? parametros)
        {
            var lista = new List<Parametro>();

            if(parametros is not JsonArray array)
                return lista;

            foreach(var p in array)
            {
                if(p is null)
                    continue;

                lista.Add(new Parametro
                {
                    Nome = ((string?)p["name"] ?? string.Empty).ToLower(),
                    Local = (string?)p["in"] ?? string.Empty,
                    Obrigatorio = (bool?)p["required"] ?? false,
                    Tipo = (string?)p["schema"]?["type"]
                });
            }

            return lista;
        }

        // Acha a categoria pela tag da operação, cria "Outros" quando tag for ausente.
        private static Categoria ObterCategoria(
            Dictionary<string, Categoria> categorias, JsonNode op)
        {
            var nomeTag = (string?)(op["tags"] as JsonArray)?.FirstOrDefault() ?? "Outros";

            if(!categorias.TryGetValue(nomeTag, out var categoria))
            {
                categoria = new Categoria { Nome = nomeTag };
                categorias[nomeTag] = categoria;
            }

            return categoria;
        }
    }
}
