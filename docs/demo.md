# Demo pública en GitHub Pages

La demo se genera desde src/tests/DocuNet.TestFixtures, que contiene código y comentarios XML sintéticos incluidos en el repositorio. Ejemplifica genéricos, herencia, sobrecargas, miembros y referencias; no representa documentación de un producto privado.

## Automatización

El workflow [Publish demo](../.github/workflows/demo.yml) se ejecuta al recibir cambios en master o manualmente mediante workflow_dispatch, siempre sobre master.

1. Restaura dependencias bloqueadas, compila y ejecuta pruebas y cobertura.
2. Genera el sitio con el CLI y el fixture público.
3. Copia index.htm a index.html para la entrada predeterminada de GitHub Pages.
4. Sube exclusivamente el directorio generado como artefacto de Pages.
5. Publica mediante el entorno github-pages.

El HTML generado no se añade a master ni necesita una rama gh-pages. El artefacto contiene el sitio y sus avisos de terceros; no contiene muestras privadas, ensamblados del fixture, archivos del repositorio ni reportes de cobertura.

Los enlaces relativos funcionan bajo el subdirectorio del repositorio. La URL real se obtiene de la salida del despliegue y aparece en el entorno github-pages y en la página del workflow; no depende de un nombre de usuario o repositorio fijado en código.

## Habilitar la publicación

Una vez creado el repositorio público y subidos los cambios:

1. Abra Settings → Pages.
2. En Build and deployment seleccione Source: GitHub Actions.
3. Revise las reglas del entorno github-pages para permitir despliegues desde master.
4. Ejecute Publish demo desde Actions sobre master, o suba un cambio a esa rama.
5. Consulte la URL emitida por el despliegue y actualice el enlace «Ver demo» del README con esa dirección.

Actualmente la demo remota no está publicada. El workflow preparado y las comprobaciones locales no acreditan ejecución de GitHub Actions. El README enlaza estas instrucciones hasta disponer de la URL real.

Documentación oficial: [workflows de GitHub Pages](https://docs.github.com/en/pages/getting-started-with-github-pages/using-custom-workflows-with-github-pages).

## Generar la demo localmente

Desde src:

```powershell
dotnet restore --locked-mode
dotnet build --configuration Release
dotnet run --no-build --configuration Release --project DocuNet.Cli -- --output=../output/demo tests/DocuNet.TestFixtures/bin/Release/net10.0/DocuNet.TestFixtures.dll
Copy-Item ../output/demo/index.htm ../output/demo/index.html
```

Abra output/demo/index.html. La copia adicional solo adapta la entrada del alojamiento; la salida original y sus enlaces index.htm se conservan.

## Evolución

La demo refleja las plantillas actuales. Las mejoras responsive, de navegación y sustitución de jQuery se incorporarán según la [hoja de ruta](roadmap.md), conservando los contratos de contenido y enlaces.
