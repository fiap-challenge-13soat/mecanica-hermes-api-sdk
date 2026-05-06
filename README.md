# Mecânica Hermes — Shared SDK

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

SemVer. Quebras de schema em `Mecanica.Hermes.Contracts` viram MAJOR bump (ex.: `pagamento.confirmado.v2`). Pacotes são versionados juntos via `Directory.Build.props`.

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

```bash
dotnet build -c Release
dotnet test -c Release --no-build
dotnet pack -c Release -o ./artifacts
```
