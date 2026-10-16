# Repository Guidelines

## Propósito y arquitectura

DocuNet es un CLI de .NET 10 que genera documentación HTML estática desde ensamblados .NET Framework o .NET moderno y sus comentarios XML. El soporte validado actual cubre Windows y Linux.

Separe lectura de metadatos, interpretación XML, modelo de documentación y presentación. Inspeccione ensamblados sin ejecutar su código ni constructores de atributos. Conserve identificadores XML completos para resolver miembros, referencias y sobrecargas.

El contrato actual acepta ensamblados/XML, --output y --templates. No añadir parámetros obligatorios de un producto específico, integración Git, backend o despliegue sin definir antes el cambio de alcance.

## Estructura

Mantenga solución, proyectos y configuración de desarrollo dentro de src:

- DocuNet.Cli: argumentos y diagnósticos.
- DocuNet.Core: lectura DLL/XML y modelo.
- DocuNet.Rendering: plantillas y generación de archivos estáticos.
- tests/DocuNet.Tests: pruebas xUnit.
- tests/DocuNet.TestFixtures: ensamblados sintéticos compilables.

Documentación pública en docs; datos adicionales de pruebas en tests/fixtures y resultados regenerables en tests/results. Los archivos históricos locales de análisis/requerimientos y las muestras privadas están ignorados; no son necesarios para contribuir.

Consulte [organización del código](docs/source-layout.md). No incluir DLL privadas, credenciales, documentación sensible ni salidas de compilación en commits.

## Desarrollo

Ejecute desde src para aplicar global.json:

```shell
dotnet restore --locked-mode
dotnet build
dotnet test
dotnet run --project DocuNet.Cli -- --help
```

Use C#, cuatro espacios, referencias anulables, PascalCase para tipos/miembros públicos y camelCase para parámetros/variables. Respete .editorconfig y las APIs de rutas multiplataforma.

## Pruebas y archivos

Use xUnit.net y NSubstitute para colaboradores simulados; fixtures y archivos reales para comprobar metadatos y renderizado. Mantenga al menos 90% de líneas y 80% de ramas por proyecto de producción; consulte [pruebas](docs/testing.md).

Cubra genéricos, sobrecargas, XML ausente/inválido, dependencias faltantes, escape HTML y destinos de enlaces. Verifique regeneración determinista y conservación de archivos ajenos o modificados manualmente. No escriba fuera del destino solicitado ni modifique muestras originales.

## Presentación y licencias

Las plantillas son código C# de confianza y deben permanecer editables externamente. Nuevas plantillas deben preservar IDs/contenido y probar navegación, responsive y accesibilidad. jQuery y el diseño heredado se modernizarán según la [hoja de ruta](docs/roadmap.md).

Conserve el crédito a Docu y los avisos de terceros. Si cambia dependencias o recursos copiados, actualice [avisos](THIRD-PARTY-NOTICES.md) y las licencias que se distribuyen con el CLI y el sitio. La licencia MIT de DocuNet no reemplaza licencias de terceros.

## Colaboración

Siga [CONTRIBUTING.md](CONTRIBUTING.md) y [SECURITY.md](SECURITY.md). Use GitHub Issues para errores/propuestas y vincule PR cuando corresponda. Los mensajes de commit deben describir el cambio; no exija códigos de expedientes locales.

Para cambios importantes, acuerde alcance y criterios antes de implementar. Describa cambios y validación; adjunte capturas para presentación. No haga push, publique ni despliegue sin autorización. El protocolo local de IA de docs/harness solo se aplica cuando se solicita expresamente; no es un requisito para contribuciones públicas.
