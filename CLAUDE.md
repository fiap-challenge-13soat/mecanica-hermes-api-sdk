# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Ecosystem Context

This repository is **one of five** in the Mecânica Hermes ecosystem. They live as siblings under
`C:\git\mecanica-hermes\` (or equivalent). When working here, keep in mind:

| Repo | Role | Key tech |
| --- | --- | --- |
| `mecanica-hermes-shared-sdk` | **This repo** — 6 pacotes NuGet em GitHub Packages | .NET 10, MassTransit.Abstractions |
| `mecanica-hermes-api-ordem-servico` | Orquestra ordens de serviço | .NET 10, PostgreSQL, RabbitMQ |
| `mecanica-hermes-cadastros` | Cadastros de Clientes e Produtos | .NET 10, PostgreSQL |
| `mecanica-hermes-api-pagamentos` | Integração Mercado Pago | .NET 10, MongoDB |
| `mecanica-hermes-tests-e2e` | Suíte E2E Robot Framework BDD | Python 3.11 |

### Status atual

- **PR #1 aberto** em `feature/bootstrap-sdk` — primeiro release do SDK (`v1.0.0-preview1`).
- **Repositório aceita force-push? NÃO.** A regra do usuário é: nunca push direto na `main`.
  Toda mudança via branch + PR.
- **Ainda não consumido** pelos 3 repos de API. Isso vai acontecer só depois do PR #1
  ser mergeado e da tag `v1.0.0` ser publicada em GitHub Packages.
- **Não publicado em nuget.org**. Distribuição é privada via GitHub Packages na org
  `fiap-challenge-13soat`.

## Commands

Use `just` (recomendado — espelha o CI):

```bash
just                # lista todas as receitas
just build          # build Debug
just test           # smoke tests (22 tests, ~1s)
just format         # aplica formatação
just format-check   # verifica formatação (4 checagens, igual CI)
just pack           # gera os 6 .nupkg + .snupkg em ./artifacts
just pack-version 1.0.0-preview2   # gera com versão custom
just nupkg-inspect Mecanica.Hermes.Shared.Core   # lista assemblies+deps de um nupkg
just clean          # remove bin/obj/artifacts/.temp
```

Equivalentes diretos (sem just):

```bash
dotnet restore Mecanica.Hermes.Shared.slnx
dotnet build Mecanica.Hermes.Shared.slnx -c Release
dotnet test Mecanica.Hermes.Shared.slnx -c Release --no-build
dotnet pack Mecanica.Hermes.Shared.slnx -c Release -o ./artifacts
```

## Arquitetura

**6 pacotes NuGet** organizados em camadas de dependência crescente. Cada pacote só pode
referenciar os de cima na pirâmide.

```text
Mecanica.Hermes.Shared.Core           (zero deps externas)
├── Mecanica.Hermes.Shared.Application    (+ MediatR)
├── Mecanica.Hermes.Shared.AspNetCore     (+ ASP.NET, FluentValidation, Serilog)
├── Mecanica.Hermes.Shared.Observability  (+ OpenTelemetry)
└── Mecanica.Hermes.Shared.MassTransit    (+ MassTransit)   ← Tier 2, skeleton em v1.0
        depende de: Application

Mecanica.Hermes.Contracts             (independente; só MassTransit.Abstractions)
```

### Pacotes em detalhe

| Pacote | Conteúdo | Dependências externas |
| --- | --- | --- |
| `Mecanica.Hermes.Shared.Core` | `Result<T>`, `BaseDomain`, `AggregateRoot`, `IDomainEvent`, `DomainEventNameAttribute`, `DomainClock`, `IUnitOfWork`, `ConcurrencyConflictException`, `PaginationParams`, `PagedResult` | nenhuma |
| `Mecanica.Hermes.Shared.Application` | `IAsyncCommand`, `IAsyncMessagePublisher`, `AsyncCommandHandler`, `LoggingBehavior` | MediatR, Microsoft.Extensions.Logging.Abstractions |
| `Mecanica.Hermes.Shared.AspNetCore` | `CorrelationIdMiddleware`, `DevelopmentAuthenticationMiddleware`, `MiddlewareExtensions`, `ValidationFilter<T>`, `ApiProblemDetails`, `EndpointConventions`, `HealthCheckTags` | ASP.NET Core, FluentValidation, Serilog |
| `Mecanica.Hermes.Shared.Observability` | `ActivitySources` (planejado: `OtlpEnvironment`, `EnvironmentDetector`, hooks em `ObservabilityConfiguration`) | OpenTelemetry.* |
| `Mecanica.Hermes.Shared.MassTransit` | (Tier 2 — esqueleto) `ConfigureSaga<TStateMachine, TState>`, bus factory, `KebabCaseEndpointNameFormatterFactory`, `MassTransitMessagePublisher` | MassTransit, MassTransit.RabbitMQ |
| `Mecanica.Hermes.Contracts` | 7 integration events versionados via `[EntityName(".v1")]` | MassTransit.Abstractions |

### Princípios de design

1. **Core sem deps externas** — usável em projetos que não usam MediatR/MassTransit/ASP.NET.
2. **Contracts é desacoplado** — não referencia `Core` nem `Application`. Permite consumidores
   externos (BFFs, jobs) usarem só os contratos.
3. **MassTransit é Tier 2** — fica como esqueleto em v1.0; código real migra após os 3 repos
   adotarem Tier 1.
4. **Migração "zero refactor"** — em v1.0, nenhum tipo público mudou de assinatura. A migração
   dos 3 repos consumidores deve ser só `using` + `<PackageReference>`. Refactors com mudança
   de API ficam para v1.1.x.

## Cross-service Integration Events

7 contratos cross-service em `Mecanica.Hermes.Contracts`. Todos versionados com sufixo `.v1`:

| `EntityName` | Publisher | Consumer(s) |
| --- | --- | --- |
| `ordem-de-servico.aguardando-aprovacao.v1` | OS | Cadastros |
| `orcamento-aprovado-pelo-cliente.v1` | Cadastros | OS |
| `orcamento-rejeitado-pelo-cliente.v1` | Cadastros | OS |
| `ordem-de-servico.aguardando-pagamento.v1` | OS | Pagamentos |
| `link-pagamento-gerado.v1` | Pagamentos | OS |
| `pagamento.confirmado.v1` | Pagamentos | OS |
| `pagamento.recusado.v1` | Pagamentos | OS |

**Regra de evolução** (documentada em `docs/versioning.md`): nunca alterar a forma de um evento
publicado. Para qualquer mudança de schema, criar `.v2` em paralelo. Producer publica ambos
durante a deprecation window. Consumers migram para `.v2` antes de remover handler `.v1`.
Drenar fila `.v1` antes de marcar como `[Obsolete]` no SDK e remover no próximo MAJOR.

## Versionamento

- **Versão única para todos os 6 pacotes**, definida em `Directory.Build.props`.
- **SemVer estrito** — bump MAJOR para qualquer rename/remoção/move-de-namespace de tipo público
  ou para mudança de comportamento default.
- **Adicionar campo em record de Contracts é MAJOR** (MassTransit serializa posicionalmente em
  alguns transports).
- Pre-releases com sufixo NuGet padrão: `1.0.0-preview1`, `1.0.0-rc1`, `1.0.0`.

Detalhes em `docs/versioning.md`.

## CI/CD (4 workflows)

| Workflow | Trigger | O quê |
| --- | --- | --- |
| `build-and-tests-ci.yml` | push `main`/`feature/**`, PR | restore + build + test + pack preview, upload artifact |
| `format-ci.yml` | push `main`, PR | `dotnet format whitespace/style/analyzers --verify-no-changes` + line-length check |
| `sonarqube-ci.yml` | push `main`, PR | SonarCloud (project key `fiap-challenge-13soat_mecanica-hermes-shared-sdk`) |
| `publish-nuget.yml` | push tag `v*.*.*` ou `workflow_dispatch` | build + test + pack + push para GitHub Packages |

### Pré-requisitos para publish funcionar

Em **`fiap-challenge-13soat` → Settings → Actions → General → Workflow permissions** deve
estar marcado **`Read and write permissions`**. Sem isso, `dotnet nuget push` falha com
`403 Forbidden` mesmo com `permissions: packages: write` declarado no YAML.

Detalhes em `docs/ci-cd.md`.

## Como consumir nos outros repos

Os repos de API consomem via `nuget.config` na raiz:

```xml
<configuration>
  <packageSources>
    <clear />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
    <add key="github-mechermes" value="https://nuget.pkg.github.com/fiap-challenge-13soat/index.json" />
  </packageSources>
  <packageSourceCredentials>
    <github-mechermes>
      <add key="Username" value="%GITHUB_USERNAME%" />
      <add key="ClearTextPassword" value="%GITHUB_TOKEN%" />
    </github-mechermes>
  </packageSourceCredentials>
</configuration>
```

Variáveis `%GITHUB_USERNAME%` e `%GITHUB_TOKEN%` (PAT com `read:packages`) expandidas pelo NuGet
em runtime. Em CI, usar `${{ github.actor }}` e `${{ secrets.GITHUB_TOKEN }}` com
`permissions: packages: read`.

Detalhes em `docs/consuming.md`.

## Testes

Suíte de **smoke tests** em `tests/Mecanica.Hermes.Shared.Tests/` — 22 testes:

- `Core/ResultTests` (6 tests) — `Result/Result<T>` sucesso/falha/status codes
- `Core/PaginationParamsTests` (8 tests via `Theory`) — coerce de Page/PageSize, cálculo de Skip
- `Contracts/IntegrationEventNamesTests` (~8 tests) — garante que os 7 EntityNames `.v1` não
  mudaram (smoke contra renomeação acidental)

Não há integration tests aqui — esses ficam no PR de migração de cada repo consumidor.

## Convenções

- Sintaxe `.slnx` (Solution File Format new); use `Mecanica.Hermes.Shared.slnx` em todos os
  comandos `dotnet`.
- **Central Package Management** via `Directory.Packages.props` — versões dos pacotes ficam
  centralizadas; os `.csproj` declaram `<PackageReference Include="..." />` sem `Version=`.
- **Source Link** ativado em `Directory.Build.props`: gera `.snupkg` separado e permite
  step-into no source do GitHub no debugger do consumidor.
- **EOL=lf** via `.editorconfig` (raiz). No Windows, `core.autocrlf=true` causa noise nos
  diffs de arquivos `.cs` recém-tocados — resolver com `git add --renormalize` antes de
  commitar.

## Documentação

Em `docs/`:

- `architecture.md` — diagrama dos 6 pacotes, princípios de design
- `ci-cd.md` — 4 workflows do GitHub Actions, procedimento de release
- `versioning.md` — política SemVer + evolução de contratos
- `consuming.md` — setup nos repos consumidores, troubleshooting
