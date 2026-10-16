# DocuNet

Genera documentación HTML estática para bibliotecas .NET a partir de sus ensamblados y comentarios XML.

DocuNet es una herramienta de línea de comandos escrita en C# y .NET 10. Lee metadatos sin ejecutar el código del ensamblado documentado. El sitio resultante puede consultarse localmente o publicarse en un alojamiento estático, sin backend.

[Ver demo: publicación y ejemplo local](docs/demo.md) — demo sintética preparada para GitHub Pages; la URL pública estará disponible después del primer despliegue.

## Características

- Documenta tipos y miembros públicos: propiedades, campos, métodos y eventos, incluidos miembros heredados.
- Conserva identificadores XML para genéricos, sobrecargas y referencias entre miembros.
- Admite entradas DLL/EXE de .NET Framework y .NET moderno.
- Genera enlaces relativos y anclas únicas para las sobrecargas.
- Permite personalizar HTML, CSS y JavaScript mediante plantillas Spark externas, sin recompilar.
- Regenera archivos administrados y protege archivos ajenos o modificados manualmente.

## Requisitos

- SDK de .NET 10 compatible con [src/global.json](src/global.json).
- DLL o EXE administrado y su archivo de documentación XML.
- Dependencias necesarias para resolver clases base, junto al ensamblado de entrada.

La herramienta se ha probado en Windows y Linux. Las ampliaciones de plataforma se describen en la [hoja de ruta](docs/roadmap.md#distribución-por-plataforma-y-arquitectura).

Para producir XML desde un proyecto C#, habilite esta propiedad:

```xml
<PropertyGroup>
  <GenerateDocumentationFile>true</GenerateDocumentationFile>
</PropertyGroup>
```

No es necesario proporcionar PDB.

## Compilar y probar

Desde una copia local del repositorio:

```shell
cd src
dotnet restore --locked-mode
dotnet build
dotnet test
dotnet run --project DocuNet.Cli -- --help
```

La solución y todos los proyectos están bajo src. Las pruebas usan xUnit.net y NSubstitute y no necesitan ensamblados privados.

## Inicio rápido

Después de compilar, puede generar un sitio de ejemplo usando el fixture incluido. Ejecute desde src:

```shell
dotnet run --no-build --project DocuNet.Cli -- --output=../output/demo tests/DocuNet.TestFixtures/bin/Debug/net10.0/DocuNet.TestFixtures.dll
```

Abra output/demo/index.htm desde la raíz del repositorio.

Para documentar su biblioteca:

```shell
dotnet run --project DocuNet.Cli -- --output=../output/library "/ruta/MiBiblioteca.dll"
```

Si no especifica archivos XML, DocuNet busca un compañero con el mismo nombre que cada ensamblado. También puede proporcionarlos explícitamente:

```shell
dotnet run --project DocuNet.Cli -- --output=../output/library "/ruta/MiBiblioteca.dll" "/ruta/MiBiblioteca.xml"
```

Las rutas relativas se resuelven desde el directorio actual. Sustituya las rutas de los ejemplos por las de su equipo.

## Opciones del CLI

| Opción o entrada | Comportamiento |
| --- | --- |
| --help | Muestra la ayuda |
| --output=path | Directorio de salida; predeterminado: ./output |
| --templates=path | Directorio de plantillas; predeterminado: Templates junto al ejecutable |
| assembly.dll / assembly.exe | Uno o varios ensamblados administrados |
| documentation.xml | Uno o varios archivos XML de documentación |
| Patrones * y ? | Seleccionan archivos de entrada; use comillas para que los procese el CLI |

Los códigos de salida son 0 para éxito/ayuda, 2 para argumentos o entradas, 3 para metadatos/resolución/XML y 4 para plantillas o escritura.

## Personalizar la presentación

Copie src/DocuNet.Rendering/Templates a un directorio propio y edite sus archivos .spark, CSS y JavaScript. Desde src:

```shell
dotnet run --project DocuNet.Cli -- --templates=../my-templates --output=../output/custom "/ruta/MiBiblioteca.dll"
```

El directorio de plantillas debe contener una copia completa de las plantillas y recursos necesarios. Puede regenerar el sitio tras editarlos sin recompilar DocuNet. Las plantillas compilan C#: utilice plantillas de confianza.

El contrato de datos, helpers y expansión de nombres está en [arquitectura y plantillas](docs/architecture.md).

## Regeneración y publicación

La salida incluye .docunet-manifest.json, que identifica los archivos administrados. Conserve ese manifiesto: DocuNet rechaza colisiones con archivos ajenos y cambios manuales en archivos generados. Personalice las plantillas para regenerar el sitio.

El render se prepara antes de reemplazar la salida. La copia final de múltiples archivos no es una transacción atómica frente a interrupciones del sistema. Las rutas enlazadas o reparse points no se admiten para plantillas ni salida.

Publique el contenido generado mediante su alojamiento estático preferido. El CLI genera archivos; no hace commits, push ni despliegues.

## Documentación

- [Demo y publicación en GitHub Pages](docs/demo.md).
- [Arquitectura y contrato de plantillas](docs/architecture.md).
- [Organización del código](docs/source-layout.md).
- [Tecnologías](docs/technology.md).
- [Pruebas y cobertura](docs/testing.md).
- [Hoja de ruta](docs/roadmap.md).
- [Cómo contribuir](CONTRIBUTING.md).
- [Política de seguridad](SECURITY.md).
- [Avisos y licencias de terceros](THIRD-PARTY-NOTICES.md).

## Limitaciones actuales

La resolución de herencia usa referencias de .NET Framework 4.8 para entradas Framework y la plataforma .NET del CLI para entradas modernas; la superficie heredada puede diferir de la de otros runtimes. No se expande inheritdoc ni se admiten firmas function-pointer. Consulte los detalles en la documentación de arquitectura.

## Reconocimientos

[Docu](https://github.com/jagregory/docu), creado por James Gregory, es la inspiración de DocuNet y la base de sus plantillas heredadas, adaptadas para este proyecto. Se conserva su crédito y licencia junto con los avisos de los recursos de terceros. DocuNet es un proyecto independiente; este reconocimiento no implica respaldo de los autores originales.

## Licencia

DocuNet se distribuye bajo la [licencia MIT](license.md).

Copyright (c) 2026 Fernando Paz. Puede usar, modificar y redistribuir el código, incluidos usos comerciales, conservando el aviso de copyright y el texto de la licencia. Los componentes de terceros conservan sus propios avisos y términos.