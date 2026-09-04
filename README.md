# ProjetoApi

Aplicativo de **console em C# (.NET 10)** que consome a [BrasilAPI](https://brasilapi.com.br) de forma **dinâmica**: em vez de ter uma tela fixa para cada consulta, ele lê a **especificação OpenAPI** da própria BrasilAPI e monta o menu automaticamente a partir dela. O usuário navega por categorias, escolhe uma consulta, informa os parâmetros e vê o resultado formatado.

## O que ele faz

- **Baixa a especificação (spec) da BrasilAPI** direto da página de documentação e a mantém salva localmente.
- **Sincroniza o spec no startup**: compara o que está no site com o arquivo salvo e só atualiza quando muda (e funciona offline usando o arquivo local, se já existir).
- **Monta um menu dinâmico** a partir do spec:
  1. **Categoria** (ex.: CNPJ, CEP, BANKS, FIPE...) — vem das `tags`.
  2. **Consulta / endpoint** — vem dos `paths` de cada categoria.
  3. **Parâmetros** — o app pergunta apenas os parâmetros que aquele endpoint exige.
- **Aciona a API** com os valores informados e **exibe o retorno de forma amigável** (lista `campo: valor`, com arrays e objetos aninhados indentados) em vez de JSON cru.

Como o menu é gerado a partir do spec, ao surgirem novos endpoints na BrasilAPI eles aparecem no menu **sem alterar o código**.

## Como funciona (visão geral)

```
Startup (Program)
  └─ SalvarValidarJson ── baixa o spec (ObterJsonEstrutura) e o mantém atualizado em disco
        └─ ProcessarJson ── lê o JSON salvo e monta a árvore Categoria → Endpoint → Parâmetro
              └─ MenuConsole ── navega os 3 níveis e coleta os parâmetros
                    └─ BrasilApiService ── monta a URL do endpoint e chama a API
                          └─ FormatadorResposta ── transforma o JSON de retorno em texto amigável
```

O spec é lido **uma única vez** no startup e mantido em memória; o menu navega essa estrutura sem reprocessar o arquivo a cada interação.

## Estrutura do projeto

```
ProjetoApi/
├── Program.cs                     # ponto de entrada: monta as dependências e inicia o menu
├── Especificacao/
│   ├── ObterJsonEstrutura.cs      # baixa a página de docs e extrai o spec (JSON)
│   ├── SalvarValidarJson.cs       # compara/salva o spec local (sincronização)
│   └── ProcessarJson.cs           # parseia o spec e monta a árvore de categorias/endpoints
├── Models/
│   ├── Categoria.cs               # categoria (tag) + seus endpoints
│   ├── Endpoint.cs                # uma operação da API (caminho, resumo, parâmetros)
│   ├── Parametro.cs               # parâmetro de um endpoint (nome, local, obrigatório, tipo)
│   └── Server.cs                  # URL base declarada no spec
├── Services/
│   └── BrasilApiService.cs        # monta a URL do endpoint escolhido e chama a API
├── Infraestrutura/
│   └── ClienteHttp.cs             # único ponto de transporte HTTP (GET → texto)
├── UI/
│   ├── MenuConsole.cs             # orquestra o fluxo dos 3 níveis do menu
│   └── ConsoleUI.cs               # helpers de entrada/saída no console
└── Utils/
    └── FormatadorResposta.cs      # formata o JSON de retorno em lista amigável
```

## Requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/download) instalado
- Conexão com a internet na primeira execução (para baixar o spec)

## Como executar

Na raiz do repositório:

```bash
dotnet run --project ProjetoApi
```

Na primeira execução o app baixa o spec da BrasilAPI e o salva na pasta de execução. Depois é só seguir o menu:

```
=== Categorias ===
1 - BANKS
2 - CEP
3 - CNPJ
...
0 - Sair
Escolha uma opção:
```

## Observações e limitações

- **A leitura do spec depende da página de docs da BrasilAPI.** O spec é extraído do HTML dessa página (de um bloco interno do framework que ela usa). Se a BrasilAPI mudar a estrutura desse HTML, a extração pode quebrar — é uma escolha consciente do projeto, feita por não haver um endpoint público estável do spec.
- **Base da API:** o spec declara a URL sem o segmento `/api`; o projeto acrescenta esse segmento ao montar as chamadas.
- **Somente `GET`:** todos os endpoints da BrasilAPI usados aqui são de leitura (`GET`).
- **Limite de requisições:** a BrasilAPI possui limite de uso; muitas chamadas em sequência podem retornar erro de "muitas requisições".

## Objetivo

Projeto de estudo, focado em consumir APIs REST, ler uma especificação OpenAPI e organizar o código por responsabilidades (download, sincronização, parsing, transporte HTTP, UI e formatação de saída).
