# Hoja de ruta

Estas líneas describen mejoras propuestas. No representan funciones disponibles ni fechas de entrega comprometidas.

## Prioridad y orden de ejecución

Priorizar la funcionalidad del CLI y la utilidad del sitio generado. Abordar la distribución y la instalación después de estabilizar las opciones, las plantillas y la gestión de recursos.

| Orden | Incidentes | Resultado esperado y dependencias |
| --- | --- | --- |
| 1 | Modernización de parámetros y ayuda del CLI | Mejorar el uso del CLI y preparar la incorporación consistente de las opciones de framework visual y recursos. |
| 2 | Dependencias CSS y JavaScript: CDN o archivos locales | Definir cómo declarar, resolver y publicar los recursos que utilizarán las nuevas plantillas. |
| 3 | Presentación y navegación; Bootstrap y Tailwind CSS; renovación de JavaScript y recursos heredados | Ejecutar estos incidentes de forma coordinada para modernizar la experiencia de consulta y evitar rehacer navegación y estilos. |
| 4 | Gestión de plantillas personalizadas | Definir su personalización, compatibilidad y conservación durante actualizaciones a partir de las nuevas plantillas. |
| 5 | Distribución por plataforma y arquitectura, incluido macOS | Empaquetar y validar la herramienta con opciones y estructura de recursos consolidadas. |
| 6 | Instalación de Windows mediante WinGet | Utilizar los paquetes de Windows y las políticas de actualización y conservación de archivos definidas previamente. |

Definir el diseño y la navegación deseados al comenzar la ejecución del roadmap, aunque su implementación corresponda al bloque 3. Este diseño inicial orientará las decisiones sobre recursos del bloque 2.

Entregar la modernización visual por incrementos: acordar una navegación común, implementar una variante de las plantillas y después la segunda. El bloque 3 se completará con soporte para Bootstrap y Tailwind CSS seleccionable por el usuario, con las mismas funciones y contenido y con el JavaScript heredado renovado conforme al diseño.

Crear los incidentes de actualización de Bootstrap, Tailwind CSS y sus dependencias al completar su integración. Revisarlos en cada futura versión del CLI.

## Modernización de parámetros y ayuda del CLI

Modernizar la presentación de los parámetros, la ayuda y el procesamiento de argumentos mediante bibliotecas NuGet mantenidas y compatibles con .NET 10. Seleccionar las bibliotecas durante el análisis del incidente, considerando su mantenimiento, licencia, dependencias y compatibilidad con la distribución por plataforma y arquitectura.

Alcance propuesto:

- Sustituir el procesamiento manual de argumentos por una biblioteca que permita declarar opciones, tipos, valores predeterminados, parámetros obligatorios y reglas de validación de forma consistente.
- Generar la ayuda desde la definición de los parámetros para mantener sincronizados nombres, descripciones, valores permitidos y ejemplos de uso. Mejorar la legibilidad de `--help` y la presentación de opciones y errores.
- Presentar diagnósticos claros para opciones desconocidas, valores ausentes o inválidos y combinaciones incompatibles, con códigos de salida estables para automatización.
- Evaluar una biblioteca de presentación de consola para mejorar el formato, adaptándose al ancho de la terminal y permitiendo salida legible sin colores ni secuencias de control cuando se redirige a archivos o se ejecuta en CI.
- Conservar la compatibilidad de los comandos y parámetros existentes; documentar cualquier cambio necesario y su migración para los scripts de los usuarios.
- Cubrir con pruebas xUnit el procesamiento, la validación, la ayuda y los códigos de salida, incluidos argumentos entre comillas, rutas con espacios y ejecución en las plataformas soportadas. Usar NSubstitute cuando se requieran mocks.
- Actualizar la documentación y los ejemplos de uso con el contrato final del CLI.

## Dependencias CSS y JavaScript: CDN o archivos locales

Permitir que las plantillas utilicen las dependencias CSS y JavaScript desde CDN oficiales o recomendados por sus proyectos, o que sus archivos se copien dentro del sitio generado. Definir cómo seleccionar el modo de recursos durante la generación y cómo declararlo en las plantillas.

Alcance propuesto:

- Ofrecer un modo CDN, con referencias HTTPS a versiones explícitas de las dependencias, y un modo local, con archivos copiados al sitio y enlaces relativos para su publicación estática y consulta sin conexión.
- Identificar los CDN oficiales o recomendados de cada dependencia; cuando no exista una fuente adecuada, conservar la alternativa local. Evitar referencias a versiones flotantes.
- Mantener las versiones de CSS y JavaScript equivalentes entre ambos modos y contemplar los recursos asociados que necesiten, como fuentes e imágenes.
- Evaluar integridad de subrecursos (SRI), configuración de `crossorigin` y compatibilidad con políticas CSP para las referencias externas, según lo admitido por cada proveedor.
- Definir el modo predeterminado y documentar los requisitos de conexión tanto para generar como para consultar el sitio. Determinar si los recursos locales se distribuyen con las plantillas o se obtienen durante la generación.
- Conservar las licencias y avisos de terceros correspondientes en ambos modos y copiar únicamente los recursos necesarios para el modo seleccionado.
- Verificar generación repetible, enlaces y navegación en ambos modos; comprobar el funcionamiento sin conexión del modo local y documentar el comportamiento del modo CDN cuando sus recursos no estén disponibles.

Coordinar este incidente con la renovación de JavaScript y recursos heredados, y con la gestión de plantillas personalizadas.

## Presentación y navegación

Evolucionar las plantillas externas hacia una presentación moderna, responsive y orientada al desarrollador que consulta una API:

- Lectura cómoda en escritorio, tablet y móvil, incluidas firmas largas y ejemplos de código.
- Navegación por namespaces, tipos y miembros, con contexto de ubicación y enlaces directos a sobrecargas.
- Búsqueda y filtrado en el sitio estático, conservando el contexto durante la navegación.
- Acceso por teclado, foco visible, contraste y estructura semántica.
- Renovación de recursos heredados conforme al diseño.
- Pruebas visuales, de viewport y de accesibilidad.

Mantener separado el modelo de documentación de su presentación. El nuevo diseño debe preservar contenido, identificadores y destinos de enlaces, sin quedar ligado a snapshots de las plantillas actuales.

## Bootstrap y Tailwind CSS en las plantillas estándar

Incorporar soporte para Bootstrap y Tailwind CSS mediante variantes de las plantillas estándar, para mejorar la apariencia visual, la consistencia y la adaptación a distintos tamaños de pantalla del sitio generado. El usuario podrá elegir cuál de los dos frameworks utilizar al generar el sitio; el CLI deberá soportar ambos y documentar la opción de selección y su valor predeterminado.

Alcance propuesto:

- Utilizar la versión estable más reciente de cada framework disponible al implementar y completar este requerimiento. Verificar las versiones en sus fuentes oficiales y registrar las versiones concretas soportadas por la entrega del CLI.
- Modernizar con ambas variantes la estructura visual de las páginas de namespaces, tipos y miembros, con estilos coherentes para navegación, tablas, firmas, ejemplos de código y búsqueda, manteniendo las mismas funciones y contenido.
- Integrar los componentes de Bootstrap y los estilos utilitarios de Tailwind CSS, junto con los componentes adicionales que se requieran para cada variante; definir sus dependencias CSS y JavaScript y publicar únicamente los recursos de la variante seleccionada.
- Mantener las plantillas externas y editables, y documentar cómo seleccionar el framework y personalizar estilos y componentes sin modificar el ejecutable.
- Definir cómo generar y distribuir los estilos finales de cada variante. Si se requiere compilar recursos, evaluar hacerlo al preparar las plantillas estándar y documentar el proceso para quienes las personalicen.
- Coordinar la entrega de recursos con el incidente de CDN o archivos locales, según las capacidades de cada framework, y evitar incluir dependencias o estilos sin uso.
- Preservar el contenido, los identificadores de miembros, los enlaces relativos y la navegación del sitio; validar ambas variantes en escritorio, tablet y móvil, así como teclado y accesibilidad.
- Incorporar ejemplos y comprobaciones visuales para ambas variantes que permitan revisar el resultado y detectar regresiones en las plantillas estándar.
- Crear incidentes de seguimiento para mantener Bootstrap, Tailwind CSS y sus dependencias actualizados en futuras versiones del CLI. Cada actualización deberá revisar versiones estables disponibles, compatibilidad y migraciones de plantillas personalizadas, licencias y regresiones visuales y funcionales; documentar las versiones adoptadas en cada entrega.

Este incidente forma parte de la modernización de presentación y navegación y debe coordinarse con la renovación de los recursos JavaScript heredados.

## Renovación de JavaScript y recursos heredados

Las plantillas actuales incluyen jQuery 1.3.2 (con Sizzle) y jQuery.ScrollTo 1.4.2. Se conservan por compatibilidad; su actualización no está implementada ni se presenta como resuelta.

Como parte de la modernización:

- Evaluar sustituir las funciones usadas por APIs nativas del navegador o dependencias mantenidas.
- Revisar avisos de seguridad y eliminar bibliotecas o funciones que no sean necesarias.
- Preservar navegación, desplazamiento, ejemplos de código y enlaces directos.
- Probar tamaños de pantalla, teclado, foco, historial y funcionamiento sin backend.
- Actualizar avisos/licencias y validar los sitios generados antes de retirar los recursos actuales.

La renovación del diseño y de JavaScript debe tratarse como una mejora con regresiones de comportamiento; no se realizará como una actualización automática de paquetes.

## Gestión de plantillas personalizadas

Definir en un incidente independiente cómo administrar las plantillas modificadas o creadas por el usuario durante la instalación, actualización y desinstalación del CLI, para todos los métodos de distribución.

Decisiones pendientes:

- Determinar dónde guardar las plantillas personalizadas y cómo distinguirlas de las plantillas incluidas en la distribución, conservando la selección mediante `--templates`.
- Definir cómo incorporar mejoras de las plantillas predeterminadas sin sobrescribir personalizaciones y cómo informar incompatibilidades entre versiones.
- Establecer qué se conserva al desinstalar y cómo permitir que el usuario elimine explícitamente sus personalizaciones.
- Documentar el flujo acordado y comprobar que las actualizaciones preserven las modificaciones del usuario.

## Distribución por plataforma y arquitectura

Evaluar paquetes o instaladores diferenciados para Windows y Linux en x64 y ARM64. Este mismo trabajo incorporará el soporte de macOS en Intel y Apple Silicon, con publicación y validación propias. Cada combinación de sistema operativo y arquitectura requiere un artefacto específico.

| Variante propuesta | Runtime identifier |
| --- | --- |
| Windows x64 | win-x64 |
| Linux x64 | linux-x64 |
| Windows ARM64 | win-arm64 |
| Linux ARM64 | linux-arm64 |
| macOS Intel | osx-x64 |
| macOS Apple Silicon | osx-arm64 |

Mantener una única solución y código fuente; variar la publicación, el empaquetado y las pruebas por destino.

El soporte de macOS se abordará dentro de esta iniciativa de distribución. Su implementación, validación e incorporación a CI no son una tarea previa ni un requisito para las mejoras anteriores del roadmap. Hasta completar este trabajo, el soporte validado se limita a Windows y Linux.

Requisitos y opciones a evaluar:

- **Incluir siempre las referencias de .NET Framework 4.8** en ReferenceAssemblies/net48 para todos los destinos, incluidos Linux, Windows ARM64 y macOS. Permiten interpretar bibliotecas Framework sin ejecutar sus ensamblados y no deben confundirse con el runtime del CLI.
- **Conservar Templates como carpeta externa y editable**, con sus plantillas y recursos, para personalizar la presentación sin cambiar ni recompilar el ejecutable. Mantener --templates para seleccionar otra ubicación.
- **Evaluar agrupar las DLL de ejecución referenciadas** mediante publicación single-file. Es empaquetado de dependencias, no fusión de sus assemblies; la compatibilidad debe comprobarse antes de adoptarlo.
- **Preferir, si es viable, una distribución sin el runtime de .NET 10 incluido**: publicación framework-dependent, SelfContained=false. Esto exige instalar previamente el runtime de .NET 10 adecuado al sistema y arquitectura; no requiere el SDK para usar la CLI.
- Comprobar especialmente Spark/Roslyn, compilación dinámica de plantillas y lectura de metadatos. Si necesitan referencias físicas, definir qué archivos deben permanecer externos o extraerse; no asumir que activar single-file basta.
- Conservar licencias y avisos de terceros en cada paquete y verificar rutas, permisos, funcionamiento sin conexión y generación con entradas Framework y modernas.
- Probar el ejecutable publicado en cada destino y automatizar los archivos descargables y sus comprobaciones en CI. No contar compilación cruzada como ejecución validada.
- Implementar y validar macOS en este mismo trabajo: restauración, compilación, suite, cobertura y generación con fixtures propios en Intel y Apple Silicon. Comprobar rutas, mayúsculas/minúsculas, permisos, recursos, enlaces y referencias net48; registrar versión del sistema, arquitectura y SDK/runtime. Incorporar sus jobs de CI y anunciar soporte solo después de la validación correspondiente.

La evaluación deberá comparar tamaño, requisitos de instalación y experiencia de uso. El agrupamiento de dependencias y la exclusión del runtime son objetivos sujetos a compatibilidad; las referencias net48 y la personalización externa de Templates se conservan.

Referencia: [publicación single-file de .NET](https://learn.microsoft.com/en-us/dotnet/core/deploying/single-file/overview).

## Instalación de Windows mediante WinGet

Preparar la distribución para instalar DocuNet con WinGet en Windows x64 y ARM64:

- Publicar los paquetes y manifiestos necesarios para su incorporación al catálogo de WinGet, con identificador estable, versiones, URLs de descarga, hashes y metadatos de licencia.
- Admitir instalación en modo usuario (`winget install --id <identificador-del-paquete> --exact --scope user`), sin requerir permisos de administrador. El identificador se definirá al preparar la publicación.
- Registrar el comando `docunet` en el `PATH` del usuario, directamente o mediante el mecanismo de alias de WinGet que corresponda al paquete, para ejecutarlo desde cualquier carpeta. Preservar las entradas existentes del `PATH` y documentar cuándo es necesario abrir una terminal nueva.
- Instalar el paquete completo, incluidas las referencias `ReferenceAssemblies/net48`, la carpeta externa `Templates` y las licencias. Resolver sus rutas con independencia del directorio de trabajo del usuario.
- Definir y comprobar cómo satisfacer el requisito del runtime de .NET 10 si se mantiene la distribución framework-dependent, sin incorporar el runtime al paquete ni asumir que instalar sus prerrequisitos admite siempre el modo usuario.
- Validar los manifiestos con `winget validate` y probar instalación, actualización y desinstalación en cuentas sin privilegios, para ambas arquitecturas. Verificar el comando desde PowerShell y CMD en carpetas distintas a la de instalación, incluidas rutas con espacios.
- Hacer que la actualización y la desinstalación gestionen únicamente las entradas de `PATH`, alias y archivos propios. La política para las plantillas personalizadas se definirá en el incidente [Gestión de plantillas personalizadas](#gestión-de-plantillas-personalizadas).

Referencias: [instalación con WinGet y selección de ámbito](https://learn.microsoft.com/en-us/windows/package-manager/winget/install) y [manifiestos de paquetes](https://learn.microsoft.com/en-us/windows/package-manager/package/manifest).
