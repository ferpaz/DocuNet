# Flujo Git del HARNESS

## Inicio limpio y actualizado

1. Confirme raíz, branch, remotos y upstream: `git rev-parse --show-toplevel`, `git status --short --branch`, `git remote -v`, `git branch -vv`.
2. Si hay cambios previos, identifique su origen y detenga las modificaciones del requerimiento. Solicite al humano decidir cómo preservarlos. No haga stash, reset, clean, descarte ni commits de trabajo ajeno automáticamente.
3. Con árbol limpio y upstream configurado, ejecute `git pull --ff-only`. Ante divergencia, error de permisos o autenticación, informe el bloqueo; no fuerce la sincronización.
4. En un repositorio sin commits o remoto, documente la excepción de arranque. La preparación documental puede continuar; acuerde la base y configuración Git antes de implementar. No invente una URL remota.

## Branch de trabajo

Registre base y branch en el expediente. Use `feature/<codigo>-<slug>` o `fix/<codigo>-<slug>` en minúsculas. `master` es el destino previsto; si no existe o el repositorio usa otro, acuerde el destino antes de integrar.

Después de aprobar el alcance, cree el branch desde la base sincronizada. Mantenga el expediente preparado junto al trabajo del requerimiento. Use commits enfocados como `DOCUNET-001: Agregar catálogo de versiones`. Revise el diff y agregue archivos explícitos; evite incluir cambios ajenos o secretos.

## Revisión y merge

La aprobación del resultado y el permiso de merge son decisiones registradas por separado, aunque el humano puede otorgarlas juntas. Push y despliegue requieren autorización explícita; no se deducen del permiso de merge local.

Antes del merge, asegure commits completos y árbol limpio. Actualice el destino con `git pull --ff-only`; incorpore la base actualizada al branch de trabajo y vuelva a validar si cambió el código o las condiciones probadas. No reescriba historia compartida ni use force push. Si existe protección por PR, respétela.

Resuelva conflictos solo cuando la intención sea inequívoca. Si afectan alcance o comportamiento, solicite ayuda humana indicando archivos, alternativas e impacto. No elija automáticamente una versión completa del archivo. Después de resolver, ejecute las pruebas afectadas y registre los resultados. Un cambio sustancial posterior a la aprobación exige renovar esa aprobación.

Con autorización, integre al destino. Verifique que contiene los commits del requerimiento (`git merge-base --is-ancestor <commit> master`), revise el resultado y ejecute la validación acordada sobre la revisión integrada. Si falla, informe el fallo y mantenga el requerimiento abierto; no anuncie éxito ni revierta historia compartida sin autorización.

## Cierre

Actualice el expediente con aprobación, revisiones probadas, commits, PR o merge y resultados. Si ese registro se añade después del merge, hágalo mediante un commit documental trazable siguiendo las protecciones del destino. Compruebe `git diff --check` y `git status --porcelain`: este último debe quedar vacío. Verifique por separado cualquier repositorio de publicación modificado. Limpieza local no equivale a push ni despliegue.

## Expediente preparado antes de crear el branch

Refinar un requerimiento puede dejar documentos modificados antes de iniciar la ejecución. No intente hacer pull con esos cambios ni los clasifique automáticamente como trabajo ajeno. Identifique su origen y acuerde preservarlos: por ejemplo, un commit documental autorizado antes de sincronizar, o una base limpia preparada por el humano. Después sincronice y cree el branch de implementación. No descarte ni guarde en stash automáticamente el expediente para satisfacer el inicio limpio.
