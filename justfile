# Receitas de desenvolvimento local. Requer 'just' (https://github.com/casey/just).
# Roda em PowerShell ou bash. Comando default: lista as receitas.
set windows-shell := ["pwsh.exe", "-NoLogo", "-NoProfile", "-Command"]

default:
    @just --list

# --- Build & test --------------------------------------------------------

# Restaura pacotes NuGet da solução.
restore:
    dotnet restore Mecanica.Hermes.Shared.slnx

# Build da solução em Debug.
build:
    dotnet build Mecanica.Hermes.Shared.slnx --nologo

# Build em Release (mesma config dos workflows de CI/publish).
build-release:
    dotnet build Mecanica.Hermes.Shared.slnx --nologo --configuration Release

# Roda os smoke tests em Debug.
test:
    dotnet test Mecanica.Hermes.Shared.slnx --nologo

# Roda os smoke tests em Release (espelha o build-and-tests-ci).
test-release:
    dotnet test Mecanica.Hermes.Shared.slnx --nologo --configuration Release

# --- Formatação ----------------------------------------------------------

# Aplica formatação (whitespace + style + analyzers) na solução.
format:
    dotnet format Mecanica.Hermes.Shared.slnx

# Verifica formatação sem alterar arquivos. Falha se algo estiver fora do padrão.
# Espelha exatamente as checagens do workflow .github/workflows/format-ci.yml.
format-check:
    dotnet format whitespace Mecanica.Hermes.Shared.slnx --verify-no-changes --verbosity diagnostic
    dotnet format style Mecanica.Hermes.Shared.slnx --verify-no-changes --verbosity diagnostic
    dotnet format analyzers Mecanica.Hermes.Shared.slnx --verify-no-changes --verbosity diagnostic
    @just _check-line-length

# Verifica que nenhum .cs excede 120 colunas (max_line_length do .editorconfig).
_check-line-length:
    #!pwsh
    $max = 120
    $violations = Get-ChildItem -Path src,tests -Recurse -Filter *.cs |
        Where-Object { $_.FullName -notmatch '[\\/](bin|obj|Migrations)[\\/]' } |
        ForEach-Object {
            $file = $_.FullName
            $n = 0
            foreach ($line in [System.IO.File]::ReadLines($file)) {
                $n++
                if ($line.Length -gt $max) {
                    "{0}:{1}: {2} cols" -f $file, $n, $line.Length
                }
            }
        }
    if ($violations) {
        $violations | ForEach-Object { Write-Host $_ }
        Write-Host ""
        Write-Error ("{0} linha(s) excedem {1} colunas (max_line_length do .editorconfig)." -f $violations.Count, $max)
        exit 1
    }
    Write-Host ("OK: nenhuma linha excede {0} colunas." -f $max)

# --- Pack (preview .nupkg) -----------------------------------------------

# Gera os 6 .nupkg + .snupkg em ./artifacts usando a versão do Directory.Build.props.
pack:
    dotnet pack Mecanica.Hermes.Shared.slnx --nologo --configuration Release --output ./artifacts

# Gera os 6 .nupkg + .snupkg em ./artifacts forçando uma versão específica.
# Uso: just pack-version 1.0.0-preview2
pack-version version:
    dotnet pack Mecanica.Hermes.Shared.slnx --nologo --configuration Release --output ./artifacts /p:Version={{version}}

# Inspeciona o conteúdo de um .nupkg gerado (lista assemblies + dependências).
# Uso: just nupkg-inspect Mecanica.Hermes.Shared.Core
nupkg-inspect package:
    #!pwsh
    $pkg = Get-ChildItem -Path ./artifacts -Filter "{{package}}.*.nupkg" | Select-Object -First 1
    if (-not $pkg) {
        Write-Error "Pacote {{package}}.*.nupkg não encontrado em ./artifacts. Rode 'just pack' antes."
        exit 1
    }
    Write-Host ("Inspecionando: " + $pkg.FullName)
    $tmp = Join-Path $env:TEMP ("nupkg-inspect-" + [guid]::NewGuid().ToString())
    Expand-Archive -Path $pkg.FullName -DestinationPath $tmp -Force
    Write-Host ""
    Write-Host "--- Assemblies ---"
    Get-ChildItem -Path $tmp -Recurse -Filter *.dll | ForEach-Object { $_.FullName.Replace($tmp, "") }
    Write-Host ""
    Write-Host "--- .nuspec (dependências) ---"
    Get-Content (Join-Path $tmp ($pkg.BaseName -replace '\.\d.+$','') -Resolve -ErrorAction SilentlyContinue) -ErrorAction SilentlyContinue
    Get-ChildItem -Path $tmp -Filter *.nuspec | ForEach-Object { Get-Content $_.FullName }
    Remove-Item -Recurse -Force $tmp

# --- Limpeza -------------------------------------------------------------

# Remove artefatos de build (bin, obj, TestResults, ./artifacts, .temp).
clean:
    Get-ChildItem -Path src,tests -Recurse -Directory -Include bin,obj,TestResults | Remove-Item -Recurse -Force -ErrorAction SilentlyContinue
    Remove-Item -Recurse -Force ./artifacts -ErrorAction SilentlyContinue
    Remove-Item -Recurse -Force .temp -ErrorAction SilentlyContinue
