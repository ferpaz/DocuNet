# Avisos y licencias de terceros

DocuNet es un proyecto independiente de Fernando Paz, distribuido bajo [MIT](license.md). Esa licencia cubre sus aportaciones propias y no sustituye las condiciones de los componentes siguientes.

## Docu: inspiración y plantillas

[Docu](https://github.com/jagregory/docu), creado por James Gregory, inspiró el proyecto. Las plantillas suministradas y los scripts de navegación/ejemplos se basan en sus recursos y se han adaptado para DocuNet.

Copyright (c) 2009, James Gregory. Licencia BSD-3-Clause: se conserva el [texto completo](docs/licenses/docu-BSD-3-Clause.txt), incluidos copyright, condiciones y descargo de responsabilidad. No se implica respaldo del autor original.

## Recursos del sitio generado

| Recurso | Autoría original | Licencia conservada |
| --- | --- | --- |
| jQuery 1.3.2 | Copyright (c) 2009 John Resig | MIT, elegida entre las alternativas del encabezado original |
| Sizzle 0.9.3, incluido en jQuery | Copyright 2009, The Dojo Foundation | MIT, elegida entre las alternativas originales |
| jQuery.ScrollTo 1.4.2 | Copyright (c) 2007–2009 Ariel Flesler | MIT, elegida entre las alternativas originales |
| Base de CSS HTML5 Boilerplate | Copyright (c) HTML5 Boilerplate | MIT |

Textos: [jQuery/Sizzle/ScrollTo](docs/licenses/jQuery-Sizzle-ScrollTo-MIT.txt) y [HTML5 Boilerplate](docs/licenses/HTML5-Boilerplate-MIT.txt). Los encabezados originales de los JS se mantienen intactos.

Cada sitio generado con las plantillas suministradas incluye THIRD-PARTY-NOTICES.txt y un enlace desde el footer. Conserve ese archivo y los avisos al distribuir el sitio o sus plantillas.

Fuentes: [jQuery](https://jquery.com/license/), [ScrollTo](https://github.com/flesler/jquery.scrollTo), [HTML5 Boilerplate](https://github.com/h5bp/html5-boilerplate).

## Dependencias de producción del CLI

Las versiones siguientes corresponden a los paquetes fijados en los archivos packages.lock.json. Los paquetes de compilación/pruebas tienen sus propias condiciones; una referencia NuGet no los convierte en código propio de DocuNet.

| Paquete o grupo | Versión | Licencia y avisos incluidos |
| --- | --- | --- |
| Spark | 1.8.2.4-alpha | Copyright (c) Louis DeJardin 2008–2024; [Apache-2.0](docs/licenses/Spark-Apache-2.0.txt), según el texto del paquete |
| MarkdownSharp | 2.0.5 | Copyright (c) 2018 Stack Exchange; [MIT](docs/licenses/MarkdownSharp-MIT.txt) |
| Microsoft.CodeAnalysis.Common, CSharp y VisualBasic | 4.14.0 | .NET Foundation and Contributors; [MIT](docs/licenses/Roslyn-MIT.txt) y [avisos adicionales](docs/licenses/Roslyn-ThirdPartyNotices.rtf) |
| Microsoft.CodeAnalysis.Analyzers, solo compilación | 3.11.0 | MIT, según metadatos NuGet; no se distribuye como ensamblado del CLI |
| Microsoft.Extensions.Caching.Abstractions/Memory, DependencyInjection.Abstractions, Logging.Abstractions, Options y Primitives | 9.0.9 | .NET Foundation and Contributors; [MIT](docs/licenses/NET-Foundation-MIT.txt) y [avisos adicionales](docs/licenses/NET-ThirdPartyNotices.txt) |
| System.Configuration.ConfigurationManager, Diagnostics.EventLog y Security.Cryptography.ProtectedData | 9.0.9 | Mismos [MIT](docs/licenses/NET-Foundation-MIT.txt) y [avisos .NET](docs/licenses/NET-ThirdPartyNotices.txt) |
| Microsoft.NETFramework.ReferenceAssemblies.net48 | 1.0.3 | Copyright (c) 2019 Microsoft; [MIT](docs/licenses/NET-Framework-ReferenceAssemblies-MIT.txt), indicado por licenseUrl del paquete |

Los archivos de referencia net48 se copian como metadatos; no se ejecutan ni son el runtime de .NET Framework.

Los textos de Spark y los avisos adicionales .NET/Roslyn se conservan desde los paquetes exactos, sin resumir sus condiciones. Fuentes de licencias: [Spark](https://github.com/SparkViewEngine/spark/blob/a33890e3edb8b9a5517135f42ab296375d39fcaa/license.txt), [MarkdownSharp](https://github.com/StackExchange/MarkdownSharp/blob/master/LICENSE), [Roslyn](https://github.com/dotnet/roslyn/blob/main/License.txt), [referencias Microsoft](https://github.com/Microsoft/dotnet/blob/main/LICENSE).

## Distribución y mantenimiento

Al preparar una distribución del CLI, incluya license.md, este documento y docs/licenses junto al ejecutable. El sitio generado recibe automáticamente sus propios avisos de recursos, sin incluir dependencias del CLI que no forman parte del HTML.

Si actualiza paquetes o sustituye recursos, revise sus licencias, titulares y avisos. No elimine un aviso solo por reemplazar la atribución visual del footer. Las bibliotecas heredadas se modernizarán según la hoja de ruta; su inclusión aquí no acredita que estén actualizadas o libres de vulnerabilidades.
