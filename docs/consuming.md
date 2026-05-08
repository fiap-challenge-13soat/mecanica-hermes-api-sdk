# Como consumir o SDK

Os pacotes do SDK ficam hospedados em **GitHub Packages** sob a org
`fiap-challenge-13soat`. Não estão publicados em
[`nuget.org`](https://www.nuget.org). Cada repo consumidor (
`mecanica-hermes-api-ordem-servico`, `mecanica-hermes-cadastros`,
`mecanica-hermes-api-pagamentos`) precisa de um pequeno setup para autenticar.

## Pré-requisitos

- Conta GitHub membro da org `fiap-challenge-13soat`.
- Personal Access Token (PAT) **clássico** com scope `read:packages` para uso local.
- Se o repo do SDK estiver com visibilidade **public**, o `read:packages` não é
  necessário para baixar — só para publicar. Para a configuração atual (private),
  é obrigatório.

## Setup no repo consumidor

### 1. `nuget.config` no root do repo

```xml
<?xml version="1.0" encoding="utf-8"?>
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

> **Não commitar tokens.** As credenciais usam variáveis de ambiente
> (`%GITHUB_USERNAME%`, `%GITHUB_TOKEN%`) — o NuGet expande na hora do restore.

### 2. Variáveis de ambiente locais

**PowerShell (Windows):**

```powershell
$env:GITHUB_USERNAME = "rogeriogelonezi"
$env:GITHUB_TOKEN    = "ghp_xxx..."
```

Para persistir entre sessões:

```powershell
[Environment]::SetEnvironmentVariable("GITHUB_USERNAME", "rogeriogelonezi", "User")
[Environment]::SetEnvironmentVariable("GITHUB_TOKEN",    "ghp_xxx...",     "User")
```

**Bash/zsh (Linux/Mac):**

```bash
export GITHUB_USERNAME=rogeriogelonezi
export GITHUB_TOKEN=ghp_xxx...
```

### 3. PackageReferences nos `.csproj`

```xml
<PackageReference Include="Mecanica.Hermes.Shared.Core" Version="1.0.0" />
<PackageReference Include="Mecanica.Hermes.Shared.Application" Version="1.0.0" />
<PackageReference Include="Mecanica.Hermes.Shared.AspNetCore" Version="1.0.0" />
<PackageReference Include="Mecanica.Hermes.Shared.Observability" Version="1.0.0" />
<PackageReference Include="Mecanica.Hermes.Shared.MassTransit" Version="1.0.0" />
<PackageReference Include="Mecanica.Hermes.Contracts" Version="1.0.0" />
```

Apenas os pacotes efetivamente usados. Por exemplo, um consumer só de eventos
(BFF, job isolado) precisa só de `Mecanica.Hermes.Contracts`.

Se o repo já usa Central Package Management (`Directory.Packages.props`), as
versões vão **lá** e os `.csproj` ficam só com `<PackageReference Include="..." />`.

### 4. Restore

```bash
dotnet restore
```

Se a config estiver correta, os pacotes baixam de `nuget.pkg.github.com` e o
build prossegue normalmente.

> Para validar a configuração **dentro do próprio repo do SDK** (útil durante
> onboarding), rode `just restore` no clone do `mecanica-hermes-shared-sdk` —
> as mesmas credenciais (`GITHUB_USERNAME` / `GITHUB_TOKEN`) são usadas pelo
> NuGet em qualquer projeto da máquina.

## Setup no CI dos repos consumidores

Workflows do GitHub Actions já recebem `GITHUB_TOKEN` automaticamente — basta
expor para o restore. Exemplo num `build-and-tests-ci.yml`:

```yaml
- name: Setup .NET
  uses: actions/setup-dotnet@v5
  with:
    dotnet-version: "10.0.x"

- name: Restore
  env:
    GITHUB_USERNAME: ${{ github.actor }}
    GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
  run: dotnet restore
```

Permissões mínimas no workflow:

```yaml
permissions:
  contents: read
  packages: read
```

> **Não precisa criar PAT separado.** O `GITHUB_TOKEN` injetado pelo Actions
> já tem `packages: read` se a org permitir leitura de packages internos.

## Setup local em IDE

**Visual Studio / Rider** lê o `nuget.config` automaticamente. Basta as variáveis
de ambiente estarem configuradas no shell de onde o IDE foi aberto, ou nas
variáveis de usuário do sistema.

Em alguns casos o NuGet armazena credenciais em texto simples por máquina
(`%APPDATA%\NuGet\NuGet.Config` no Windows). Evitar — preferir as variáveis de
ambiente para que credenciais não saiam do shell session.

## Troubleshooting

### `401 Unauthorized` no restore

- PAT expirado ou sem scope `read:packages`. Gerar novo em
  [github.com/settings/tokens](https://github.com/settings/tokens).
- `Username` errado no `nuget.config` — usar o handle GitHub, não o email.
- Variável `%GITHUB_TOKEN%` não está expandida no shell onde o `dotnet restore`
  rodou. Validar com `echo $env:GITHUB_TOKEN` (PowerShell) / `echo $GITHUB_TOKEN`
  (Bash).

### `403 Forbidden` no restore

- Conta GitHub não é membro da org `fiap-challenge-13soat`.
- Repo do SDK está com Single Sign-On (SSO) ativo e o PAT não foi autorizado
  para a org. Em **Settings → Developer settings → Tokens → Configure SSO**,
  autorizar o token para a org.

### `Unable to find package` apesar do source configurado

- `nuget.config` no diretório errado — precisa estar no root do repo (ou em
  diretório pai do `.csproj`). Validar com `dotnet nuget list source` —
  `github-mechermes` deve aparecer como Enabled.
- Nome do pacote digitado errado. Os 6 pacotes são:
  - `Mecanica.Hermes.Shared.Core`
  - `Mecanica.Hermes.Shared.Application`
  - `Mecanica.Hermes.Shared.AspNetCore`
  - `Mecanica.Hermes.Shared.Observability`
  - `Mecanica.Hermes.Shared.MassTransit`
  - `Mecanica.Hermes.Contracts`

### Build local sem rede (offline)

```bash
dotnet restore --source ~/.nuget/packages
```

Funciona desde que o cache local já tenha os `.nupkg`. Para popular o cache
sem rede contínua, rodar `dotnet restore` uma vez online — fica em
`~/.nuget/packages/mecanica.hermes.shared.*/<versão>/`.

## Atualizar para uma nova versão

1. Bump no `<PackageReference Version="...">` (ou em `Directory.Packages.props`
   se o repo usa CPM).
2. `dotnet restore`.
3. Em caso de MAJOR (ex.: `1.x` → `2.x`), conferir
   [`versioning.md`](./versioning.md) para a deprecation window de eventos
   antes de mergear o bump em produção.
