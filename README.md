# Mecânica Hermes — Shared SDK

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/)

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=fiap-challenge-13soat_mecanica-hermes-shared-sdk&metric=alert_status&token=cb19f9f842a7972ed0b9a881bf4ba19155ac172e)](https://sonarcloud.io/summary/new_code?id=fiap-challenge-13soat_mecanica-hermes-shared-sdk)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=fiap-challenge-13soat_mecanica-hermes-shared-sdk&metric=coverage&token=cb19f9f842a7972ed0b9a881bf4ba19155ac172e)](https://sonarcloud.io/summary/new_code?id=fiap-challenge-13soat_mecanica-hermes-shared-sdk)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=fiap-challenge-13soat_mecanica-hermes-shared-sdk&metric=sqale_rating&token=cb19f9f842a7972ed0b9a881bf4ba19155ac172e)](https://sonarcloud.io/summary/new_code?id=fiap-challenge-13soat_mecanica-hermes-shared-sdk)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=fiap-challenge-13soat_mecanica-hermes-shared-sdk&metric=reliability_rating&token=cb19f9f842a7972ed0b9a881bf4ba19155ac172e)](https://sonarcloud.io/summary/new_code?id=fiap-challenge-13soat_mecanica-hermes-shared-sdk)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=fiap-challenge-13soat_mecanica-hermes-shared-sdk&metric=security_rating&token=cb19f9f842a7972ed0b9a881bf4ba19155ac172e)](https://sonarcloud.io/summary/new_code?id=fiap-challenge-13soat_mecanica-hermes-shared-sdk)

SDK compartilhado para os microsserviços do ecossistema Mecânica Hermes (`mecanica-hermes-api-ordem-servico`, `mecanica-hermes-cadastros`, `mecanica-hermes-api-pagamentos`).

## Pacotes

| Pacote | Dependências externas | Conteúdo |
|---|---|---|
| `Mecanica.Hermes.Shared.Core` | nenhuma | `Result<T>`, `BaseDomain`, `AggregateRoot`, `IDomainEvent`, `IUnitOfWork`, `PaginationParams`, `PagedResult` |
| `Mecanica.Hermes.Shared.Application` | MediatR | `IAsyncCommand`, `IAsyncMessagePublisher`, `AsyncCommandHandler`, `LoggingBehavior` |
| `Mecanica.Hermes.Shared.AspNetCore` | ASP.NET Core, FluentValidation, Serilog | `CorrelationIdMiddleware`, `DevelopmentAuthenticationMiddleware`, `ValidationFilter<T>`, `ApiProblemDetails`, `EndpointConventions`, `HealthCheckTags` |
| `Mecanica.Hermes.Shared.Observability` | OpenTelemetry | `ActivitySources` |
| `Mecanica.Hermes.Shared.MassTransit` | MassTransit | (planejado: bus factory, ConfigureSaga genérico) |
| `Mecanica.Hermes.Contracts` | MassTransit.Abstractions | 7 integration events versionados (`.v1`) |

## Versionamento

Quebras de schema em `Mecanica.Hermes.Contracts` viram MAJOR bump (ex.: `pagamento.confirmado.v2`). Pacotes são versionados juntos via `Directory.Build.props`.

## Como consumir

```xml
<PackageReference Include="Mecanica.Hermes.Shared.Core" Version="1.0.0" />
<PackageReference Include="Mecanica.Hermes.Contracts" Version="1.0.0" />
```

Os pacotes são publicados em **GitHub Packages**. Configure `nuget.config` no consumidor:

```xml
<configuration>
  <packageSources>
    <add key="github-mechermes" value="https://nuget.pkg.github.com/fiap-challenge-13soat/index.json" />
  </packageSources>
  <packageSourceCredentials>
    <github-mechermes>
      <add key="Username" value="USERNAME" />
      <add key="ClearTextPassword" value="%GITHUB_TOKEN%" />
    </github-mechermes>
  </packageSourceCredentials>
</configuration>
```

## Build

Receitas via [`just`](https://github.com/casey/just) (recomendado — espelha o CI):

```bash
just                # lista todas as receitas
just build          # build Debug
just test           # smoke tests
just format         # aplica formatação
just format-check   # verifica formatação (mesmas checagens do CI)
just pack           # gera os 6 .nupkg + .snupkg em ./artifacts
just clean          # limpa bin/obj/artifacts/.temp
```

Sem `just` (equivalentes diretos):

```bash
dotnet build Mecanica.Hermes.Shared.slnx -c Release
dotnet test Mecanica.Hermes.Shared.slnx -c Release --no-build
dotnet pack Mecanica.Hermes.Shared.slnx -c Release -o ./artifacts
```

## Documentação

Documentação detalhada em [`docs/`](./docs/):

- [`architecture.md`](./docs/architecture.md) — desenho dos 6 pacotes e princípios de design
- [`ci-cd.md`](./docs/ci-cd.md) — workflows do GitHub Actions e procedimento de release
- [`versioning.md`](./docs/versioning.md) — política de SemVer e evolução de contratos
- [`consuming.md`](./docs/consuming.md) — setup do `nuget.config` nos repos consumidores
