# Contribuir a DocuNet

Las contribuciones pueden mejorar el lector de metadatos, la documentación, las pruebas o las plantillas del sitio.

## Seguimiento de trabajo

Use GitHub Issues para reportar errores, proponer mejoras y acordar el alcance de cambios. Vincule los pull requests con su issue cuando corresponda. Los expedientes y resultados históricos locales no son necesarios para contribuir ni deben adjuntarse al repositorio público.

Para vulnerabilidades, siga [SECURITY.md](SECURITY.md) y use un canal privado.

## Informar un problema

Describa el comportamiento esperado y el observado, el comando utilizado, el sistema operativo, la versión del SDK y los diagnósticos. Si es posible, incluya un proyecto mínimo compilable que reproduzca el problema. No publique DLL privadas, credenciales ni documentación sensible.

## Preparar un cambio

1. Revise README.md y la documentación de arquitectura.
2. Compile y ejecute las pruebas desde src.
3. Mantenga separado el procesamiento de ensamblados del renderizado.
4. Agregue una regresión de comportamiento cuando corrija un defecto.
5. Actualice la documentación si cambia el contrato del CLI o de las plantillas.

Para cambios de interfaz, adjunte capturas y describa las comprobaciones de navegación, tamaño de pantalla y accesibilidad. Para cambios grandes, acuerde primero su alcance con los mantenedores.

## Comprobaciones

```shell
cd src
dotnet restore --locked-mode
dotnet build
dotnet test
```

Consulte [pruebas y cobertura](docs/testing.md) para medir la cobertura y verificar sus umbrales. No incluya bin/obj, sitios generados, reportes locales ni muestras privadas.

## Convenciones

Use C#, cuatro espacios, referencias anulables, PascalCase para tipos y miembros públicos y camelCase para parámetros y variables. Mantenga proyectos y configuración de desarrollo dentro de src.

Explique qué cambia, por qué y cómo se verificó en la propuesta de contribución. Conserve los avisos de autoría y de componentes de terceros.
