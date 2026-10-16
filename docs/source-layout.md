# Organización del código fuente

La estructura mantiene juntos los proyectos y la configuración de desarrollo.

Todo el código fuente y los archivos que gobiernan su desarrollo se ubican dentro de `src/`:

```text
src/
    DocuNet.slnx
    Directory.Build.props
    Directory.Packages.props
    global.json
    .editorconfig
    .gitignore
    .gitattributes
    DocuNet.Cli/
    DocuNet.Core/
    DocuNet.Rendering/
    tests/
        Directory.Packages.props
        DocuNet.Tests/
        DocuNet.TestFixtures/
tests/
    fixtures/
    results/
docs/
    requirements/
```

- La solución y todos los proyectos, incluido el proyecto de pruebas, permanecen en `src/`.
- `src/Directory.Build.props` contiene las propiedades comunes de los proyectos.
- `src/Directory.Packages.props` habilita la gestión central de NuGet y contiene las versiones de Cli, Core y Rendering. Los proyectos bajo `src/tests/` usan automáticamente su catálogo independiente `src/tests/Directory.Packages.props`, sin importar el catálogo de producción.
- `src/global.json` selecciona el SDK; ejecute los comandos de desarrollo desde `src/` para que se aplique.
- `src/.editorconfig` define las convenciones del código.
- `src/.gitattributes` preserva los recursos y plantillas heredados sin conversión de fin de línea ni normalización de su formato original.
- `src/.gitignore` reúne las exclusiones de .NET/C#, JetBrains Rider, Visual Studio Code, Visual Studio y los sistemas operativos. Se generó desde [Toptal/gitignore.io](https://www.toptal.com/developers/gitignore/api/rider,visualstudio,visualstudiocode,csharp,dotnetcore,windows,linux,macos). El servicio ofrece la plantilla genérica VisualStudio para Visual Studio 2026.
- `tests/fixtures/` y `tests/results/` en la raíz contienen insumos y evidencias del arnés, no el proyecto de pruebas.
- `docs/` conserva documentación y `docs/requirements/` expedientes. Las muestras privadas se mantienen fuera del código fuente e ignoradas en `samples/`.

Desde la raíz:

```powershell
cd src
dotnet restore
dotnet build
dotnet test
dotnet run --project DocuNet.Cli -- --help
```

Mantenga esta estructura al agregar proyectos o modificar las instrucciones de desarrollo.
