# Tecnología y convenciones

Tecnologías utilizadas y convenciones para contribuir al proyecto.

| Área | Tecnología |
| --- | --- |
| CLI y bibliotecas | C#, .NET 10; referencias anulables habilitadas |
| Solución | Formato .slnx, proyectos y configuración bajo src/ |
| Lectura de DLL | System.Reflection.Metadata / PEReader; sin ejecutar código ni constructores de atributos de entrada |
| Documentación XML | System.Xml y LINQ to XML; conservar IDs originales, prohibir DTD |
| Plantillas | Spark externo editable; 1.8.2.4-alpha con Roslyn, validado en .NET 10 |
| Pruebas | **xUnit.net** |
| Mocks y sustitutos | **NSubstitute**, cuando se necesiten colaboradores simulados |
| Validación HTML | AngleSharp en pruebas; comprobar destinos, fragmentos e IDs con parser HTML |
| IDE | JetBrains Rider, Visual Studio Code, Visual Studio 2026 |

No usar MSTest, NUnit, Moq ni otro framework de mocks por defecto. Evitar mocks para los casos que deban probar realmente metadatos, archivos o renderizado; usar fixtures propios y directorios aislados.

Paquetes de pruebas fijados: xunit.v3 4.0.2 (nombre del paquete de xUnit.net), xunit.runner.visualstudio 4.0.1, Microsoft.NET.Test.Sdk 18.10.1 y NSubstitute 6.2.0. AngleSharp 1.8.5 se utiliza para validar HTML. Las dependencias resueltas quedan en packages.lock.json; no desactivar auditoría ni warnings para ocultar fallos.

Fuentes consultadas: [xUnit](https://www.nuget.org/packages/xunit.v3), [NSubstitute](https://www.nuget.org/packages/NSubstitute), [AngleSharp](https://www.nuget.org/packages/AngleSharp). Se contrastaron versiones estables con el feed oficial NuGet al implementarlas; una versión declarada no sustituye la validación.

Proyecto de pruebas: src/tests/DocuNet.Tests/. Ejecutar desde src:

```powershell
dotnet restore
dotnet build
dotnet test
```

La [organización del código fuente](source-layout.md) detalla la estructura. Los resultados reales por plataforma se registran en el expediente del requerimiento.

Metadatos de plataforma heredada: Microsoft.NETFramework.ReferenceAssemblies.net48 1.0.3. Sus DLL de referencia se copian como datos a ReferenceAssemblies/net48; no se cargan en el CLR. Estas referencias permiten resolver herencia de Framework sin instalar ni ejecutar su runtime en el sistema anfitrión.

Cobertura: Microsoft.Testing.Extensions.CodeCoverage 18.12.0, integrado con Microsoft.Testing.Platform. Umbrales por proyecto de producción: 90% de líneas y 80% de ramas; procedimiento y evidencia en [pruebas y cobertura](testing.md).

## Versión y metadatos de los ensamblados

`src/Directory.Build.props` centraliza la versión de DocuNet (`1.0.0`), autor y compañía (`Fernando Paz`), producto, título por proyecto, descripciones y copyright (`Copyright (c) 2026 Fernando Paz`). La expresión de licencia de los paquetes es `MIT`, conforme a `license.md`; los avisos y licencias de terceros se conservan por separado.

El SDK genera los atributos de ensamblado a partir de estas propiedades: con `Version=1.0.0`, `AssemblyVersion` y `FileVersion` resultan en `1.0.0.0`. La versión informativa parte de `1.0.0` y puede incluir el hash del commit que añade el SDK para trazabilidad. Para nuevas entregas, actualizar `Version` en ese archivo y verificar los metadatos de las DLL y del ejecutable publicados. Los metadatos específicos de los fixtures de pruebas pueden sobrescribir los valores comunes.

Referencia: [atributos de ensamblado generados por el SDK de .NET](https://learn.microsoft.com/en-us/dotnet/standard/assembly/set-attributes-project-file).

## Gestión central de paquetes NuGet

Usar Central Package Management de NuGet. `src/Directory.Packages.props` habilita la gestión central y define las versiones de los paquetes de DocuNet.Cli, DocuNet.Core y DocuNet.Rendering. Los proyectos DocuNet.Tests y DocuNet.TestFixtures, ubicados bajo `src/tests/`, usan automáticamente el catálogo independiente `src/tests/Directory.Packages.props` más cercano a sus proyectos; TestFixtures participa en este grupo aunque actualmente no necesita paquetes directos.

Los proyectos conservan sus `PackageReference` y opciones como `PrivateAssets`, `IncludeAssets` y `GeneratePathProperty`, pero no declaran versiones ni `VersionOverride`. Agregar una versión al catálogo no agrega automáticamente una dependencia a todos los proyectos. Las dependencias transitivas de los proyectos de producción referenciados por las pruebas conservan su resolución habitual.

Para cambiar dependencias, editar el catálogo correspondiente, restaurar y revisar los cambios de `packages.lock.json`. Mantener los archivos de bloqueo por proyecto y comprobar `dotnet restore --locked-mode`, compilación y pruebas antes de integrar los cambios.

Referencia: [Central Package Management de NuGet](https://learn.microsoft.com/en-us/nuget/consume-packages/central-package-management).
