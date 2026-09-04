using System;
using System.Collections.Generic;

namespace ProjetoApi.UI
{
    // Helpers de entrada/saída no console, reutilizados pelo menu.
    public static class ConsoleUI
    {
        // Título destacado (cabeçalho de cada nível do menu).
        public static void Titulo(string texto)
        {
            Console.WriteLine();
            Console.WriteLine($"=== {texto} ===");
        }

        // Lista itens numerados a partir de 1. O 0 fica reservado pra "voltar/sair.
        public static void ListarItens(IEnumerable<string> itens)
        {
            var i = 1;
            foreach(var item in itens)
            {
                Console.WriteLine($"{i} - {item}");
                i++;
            }
        }

        // Lê uma opção inteira entre 0 e max, re-perguntando enquanto for inválida.
        public static int LerOpcao(int max)
        {
            while(true)
            {
                Console.Write("Escolha uma opção: ");
                var entrada = Console.ReadLine();

                if(int.TryParse(entrada, out var opcao) && opcao >= 0 && opcao <= max)
                    return opcao;

                Console.WriteLine("Opção inválida. Tente novamente.");
            }
        }

        // Lê entrada do usuário.
        public static string LerTexto(string rotulo)
        {
            Console.Write($"{rotulo}: ");
            return (Console.ReadLine() ?? string.Empty).Trim();
        }

        // Pausa até o Enter (útil depois de exibir um resultado).
        public static void Pausar()
        {
            Console.WriteLine();
            Console.Write("Pressione Enter para continuar...");
            Console.ReadLine();
        }
    }
}
