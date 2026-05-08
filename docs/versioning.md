# Versionamento

O SDK segue [Semantic Versioning 2.0.0](https://semver.org/lang/pt-BR/) com regras
adicionais para o pacote `Mecanica.Hermes.Contracts` (eventos de mensageria).

## Versão única para todos os pacotes

Os 6 pacotes compartilham **uma única versão** definida em
`Directory.Build.props`:

```xml
<PropertyGroup>
  <Version>1.0.0-preview1</Version>
</PropertyGroup>
```

Razão: simplifica o consumo (`<PackageReference Version="1.0.0">` em todos os
pacotes do SDK) e remove a necessidade de matrix de compatibilidade entre
sub-pacotes. O custo é que um bump em `AspNetCore` empurra também um bump em
`Core` — aceitável dado o número pequeno de pacotes.

## Quando bumpar cada parte do SemVer

| Mudança | Bump |
|---|---|
| Adicionar tipo / método novo, sem quebrar APIs existentes | MINOR (`1.0.0` → `1.1.0`) |
| Bug fix sem mudar assinatura | PATCH (`1.0.0` → `1.0.1`) |
| Renomear/remover tipo público, mudar assinatura, mover tipo entre namespaces | MAJOR (`1.0.0` → `2.0.0`) |
| Alterar comportamento default de uma API existente (mesmo sem mudar assinatura) | MAJOR — quebra de contrato comportamental |
| Mudar versão do TFM (`net10.0` → `net11.0`) | MAJOR |
| Adicionar campo opcional em `Mecanica.Hermes.Contracts` | **MAJOR** (ver abaixo) |

## Contracts — política especial

O pacote `Mecanica.Hermes.Contracts` carrega **eventos cross-service** que trafegam
em filas RabbitMQ. Mudanças aqui afetam consumers em produção que podem rodar
versões diferentes da do publisher.

### Regra de evolução: `EntityName` versionado

Cada evento é declarado com sufixo `.vN` no `EntityName`:

```csharp
[EntityName("ordem-de-servico.aguardando-pagamento.v1")]
public sealed record OrdemDeServicoAguardandoPagamentoEvent(...);
```

Esse `.v1` define a fila/exchange no RabbitMQ. **Nunca alterar a forma de um
evento publicado** — sempre criar um novo:

```csharp
// v1 mantido para compat
[EntityName("ordem-de-servico.aguardando-pagamento.v1")]
public sealed record OrdemDeServicoAguardandoPagamentoEvent(
    Guid OrdemDeServicoId, Guid ClienteId, decimal Valor);

// v2 adicionado em paralelo
[EntityName("ordem-de-servico.aguardando-pagamento.v2")]
public sealed record OrdemDeServicoAguardandoPagamentoEventV2(
    Guid OrdemDeServicoId, Guid ClienteId, decimal Valor, string Moeda);
```

### Deprecation window

1. **Publicar `.v2`**: producer começa a publicar **ambos** (`.v1` e `.v2`).
2. **Migrar consumers**: cada microsserviço consumidor publica versão que escuta
   `.v2` (e mantém handler `.v1` durante a transição).
3. **Aguardar drenagem**: monitorar fila `.v1` até chegar a zero msg/s sustentado.
4. **Remover producer `.v1`**: producer para de publicar `.v1`.
5. **Remover consumer `.v1`**: cada consumidor remove handler `.v1` no próximo release.
6. **Marcar `.v1` como `[Obsolete]` no SDK** e removê-lo no próximo MAJOR (`v2.0.0`).

> O **bump da versão NuGet** do `Contracts` em si segue SemVer normal —
> adicionar `OrdemDeServicoAguardandoPagamentoEventV2` é MINOR (tipo novo,
> nada quebrado). O que requer cuidado é a coordenação operacional acima.

### Por que MAJOR ao adicionar campo "opcional"?

MassTransit serializa `record`s posicionalmente em alguns transports. Adicionar
um parâmetro ao `record` muda a ordem do construtor e quebra deserialização em
consumers compilados contra a versão antiga. **Sempre** preferir um novo evento
versionado em vez de "estender" um existente.

## Pre-releases

Pre-releases usam o sufixo padrão NuGet:

| Tag | Significado |
|---|---|
| `1.0.0-preview1` | versão de bootstrap ainda em validação |
| `1.0.0-rc1` | release candidate — API congelada, smoke tests rodando em consumer real |
| `1.0.0` | release oficial |

Pre-releases são publicados via `workflow_dispatch` (ver [`ci-cd.md`](./ci-cd.md));
releases oficiais via push de tag `v*.*.*`.

## Como bumpar a versão

1. Atualizar `Version` em `Directory.Build.props`:
   ```xml
   <Version>1.1.0</Version>
   ```
2. Commit + PR.
3. Após merge em `main`, criar tag:
   ```bash
   git tag -a v1.1.0 -m "Release 1.1.0 — adiciona ITracedCommand"
   git push origin v1.1.0
   ```
4. Workflow `publish-nuget.yml` dispara automaticamente, builda e publica
   os 6 pacotes na versão `1.1.0`.

## Compatibilidade entre versões

| Versão SDK | TFM | .NET runtime exigido no consumidor |
|---|---|---|
| 1.x | `net10.0` | .NET 10 |

Quando o ecossistema migrar para .NET 11, isso vira `2.x` (MAJOR — TFM trocou).
