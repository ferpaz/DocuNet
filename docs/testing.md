# Pruebas y cobertura

Las pruebas usan xUnit.net y NSubstitute. La suite normal compila fixtures propios y no requiere muestras privadas. Los proyectos de pruebas y la configuración de cobertura están en src.

## Ejecutar la suite

Desde src:

```shell
dotnet restore --locked-mode
dotnet build
dotnet test --no-build
```

## Medir cobertura

Desde src, usando PowerShell 7 para comprobar los umbrales:

```powershell
dotnet test --no-build --coverage --coverage-settings coverage.config --coverage-output-format cobertura --coverage-output coverage.cobertura.xml --results-directory ../tests/results/coverage
./Check-Coverage.ps1 -Report ../tests/results/coverage/coverage.cobertura.xml
```

Para Release, agregue --configuration Release tanto a build como a test.

El nombre relativo de --coverage-output se resuelve bajo el directorio de resultados. La configuración incluye únicamente DocuNet.Core, DocuNet.Rendering y DocuNet.Cli; excluye pruebas, fixtures, dependencias y C# generado dinámicamente por Spark. No se excluyen archivos propios de producción para mejorar porcentajes.

Microsoft.Testing.Extensions.CodeCoverage integra la medición con Microsoft.Testing.Platform. Véase la [documentación oficial](https://learn.microsoft.com/en-us/dotnet/core/testing/microsoft-testing-platform-code-coverage).

Check-Coverage.ps1 exige al menos **90% de líneas y 80% de ramas en cada proyecto** y falla si falta alguno de los tres proyectos. La cobertura mide ejecución: debe acompañar aserciones de resultados, errores y preservación de datos.

## Comportamientos protegidos

| Área | Casos principales |
| --- | --- |
| Metadatos | Lectura sin ejecutar módulos/atributos; visibilidad; identidad por clase; genéricos; herencia; referencias Framework/modernas; enums; eventos; tipos anidados; namespace global; duplicados y dependencias faltantes |
| XML | IDs exactos; ref/out, indexadores y arrays; comentarios ausentes; estructura inválida; miembros sin nombre, duplicados y DTD rechazados |
| HTML | Escape; párrafos, listas y referencias; sobrecargas con anclas únicas; enlaces y fragmentos |
| Plantillas | Expansión de nombres; parciales; edición sin recompilar; recursos; fallos sin reemplazar el sitio previo |
| Archivos | Regeneración determinista; preservación de archivos ajenos; colisiones, ediciones manuales, traversal, solapamiento y raíz de filesystem |
| CLI | Ayuda; argumentos; patrones; deduplicación; XML compañero; diagnósticos y códigos de salida |

PresentationContractTests verifica el contenido mediante DOM y destinos de enlaces. No fija el HTML completo ni el CSS, permitiendo evolucionar la presentación.

SampleValidationTests es una prueba explícita para comparar sitios y entradas adicionales mediante variables de entorno. Su omisión en la suite normal es deliberada. La suite pública debe seguir siendo reproducible sin esos archivos.

## Añadir regresiones

Compruebe el resultado observable que distingue el comportamiento correcto del defecto. Use archivos y ensamblados sintéticos para probar procesamiento real; reserve NSubstitute para colaboradores simulados. No sustituya aserciones por ejecución superficial ni oculte código para cumplir umbrales.

Las plantillas nuevas requieren además pruebas de viewport, teclado, accesibilidad y navegación en navegador. Guarde los reportes locales bajo tests/results, que está ignorado por Git.

La matriz de CI incluye Windows y Linux. La ampliación a otros destinos se validará como parte de la iniciativa de [distribución por plataforma y arquitectura](roadmap.md#distribución-por-plataforma-y-arquitectura). Configurar un workflow o realizar compilación cruzada no acredita ejecución en un destino nuevo.
