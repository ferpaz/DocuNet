# Protocolo HARNESS

## Alcance y lectura obligatoria

Este es el protocolo de trabajo de IA para DocuNet, adaptado a los acuerdos de este repositorio. Antes de implementar, lea esta guía, [el flujo Git](git-workflow.md) y [la plantilla de requerimientos](requirement-template.md). `docs/` contiene documentación de la aplicación; `docs/harness/`, reglas del arnés; `docs/requirements/`, el historial de requerimientos.

Los roles son Coordinator, Developer y Tester. Pueden desempeñarse por agentes separados o por el mismo agente en fases explícitas. La separación de roles no implica una revisión independiente cuando los ejecuta un solo agente; registre esa limitación. El Coordinator conserva la responsabilidad del flujo y consolida las entregas.

## Estados y aprobaciones

`Borrador → Pendiente de aprobación del alcance → En desarrollo → En validación → Pendiente de aprobación final → Aprobado → Cerrado`.

Para requerimientos con descubrimiento, inserte `En investigación → Pendiente de aprobación del comportamiento/diseño` después de aprobar el alcance y antes de desarrollo. Un fallo devuelve el trabajo a la fase correspondiente. Registre bloqueos sin declarar aprobado o cerrado el requerimiento.

1. **Inicio — Coordinator.** Compruebe limpieza y sincronice según el flujo Git. Si faltan remoto, commits o permisos, registre la limitación; no simule un pull exitoso. Cree el expediente usando la plantilla. Nombre: `YYYY-MM-DD_CODIGO_slug.md`, por ejemplo `2026-10-15_DOCUNET-001_generar-documentacion-por-version.md`. La fecha corresponde al inicio de la solución, no al cierre; use la fecha local del usuario. El código es único y permanece estable.
2. **Definición — Coordinator.** Revise objetivo, descripción, alcance y exclusiones, criterios verificables, trazabilidad, diseño de pruebas unitarias, regresión e integración, branches y aprobaciones. Vincule criterios con pruebas concretas. Resuelva ambigüedades que afecten comportamiento o aceptación. Una sección no aplicable debe justificarlo.
3. **Aprobación humana del alcance.** Presente el expediente y solicite aprobación antes de programar. Registre la decisión real y su referencia a la conversación o revisión; no infiera aprobación del silencio. Cambios sustanciales de alcance requieren nueva aprobación. Esta puerta no impide preparar documentación y análisis solicitados.
4. **Implementación — Developer.** Trabaje en el branch acordado. Implemente únicamente el alcance aprobado, escriba pruebas unitarias y ejecútelas. Entregue al Coordinator archivos modificados, relación criterio/cambio/prueba, comandos y resultados, limitaciones y riesgos. No declare aceptación humana.
5. **Revisión — Coordinator.** Compare la entrega con cada criterio y revise código y pruebas. Devuelva incumplimientos al Developer. Actualice documentación de uso, arquitectura y contrato entre repositorios donde corresponda; entregue al Tester una revisión identificable y el plan de pruebas.
6. **Validación — Tester.** Ejecute las pruebas unitarias y las de regresión e integración definidas, usando entradas de `tests/fixtures/` y salidas de `tests/results/<codigo>/<ejecucion>/`. Registre revisión probada, entorno, comandos, códigos de salida, esperado/obtenido y rutas de evidencia. Cuando el alcance afecte publicación, pruebe integración contra un repositorio temporal y preservación de versiones. Verifique enlaces cuando genere HTML. No use el sitio publicado como fixture. Informe fallos o pruebas omitidas y devuelva el control al Coordinator; una prueba no ejecutada no cuenta como exitosa.
7. **Aprobación humana del resultado.** El Coordinator presenta criterios, cambios, evidencias y limitaciones. Solicita aprobación final y, si corresponde, autorización explícita para merge a `master`. Conserve el trabajo pendiente si no hay respuesta o se rechaza.
8. **Cierre — Coordinator.** Tras aprobación, complete trazabilidad, commits y merge autorizado conforme al flujo Git. Compruebe el árbol limpio. Registre lo pendiente si no puede completar el cierre; nunca borre cambios para aparentar limpieza.

## Evidencia y trazabilidad

El expediente debe conectar código del requerimiento, branch, criterios, pruebas, commits de implementación, documentación y aprobación humana. Use el código en commits y PR. Registre autor/rol, fecha y motivo de decisiones. No invente hashes, ejecuciones o aprobaciones.

Los resultados voluminosos permanecen ignorados; conserve un resumen revisable en el expediente o documentación enlazada. Antes del cierre, compruebe que la evidencia referenciada esté disponible en el repositorio o en un artefacto persistente accesible. El commit de cierre contiene el expediente y se identifica mediante el código; no intente incluir su propio hash dentro del archivo.

## Refinamiento, investigación y relevo entre sesiones

Distinga **refinar** (analizar y editar documentos) de **ejecutar** (investigar artefactos, programar o probar la solución). Una petición de preparar un requerimiento no autoriza su ejecución. Durante refinamiento, conserve la intención original, transforme términos ambiguos en criterios verificables y registre decisiones, propuestas y pendientes por separado. Si el humano excluye este trabajo documental del protocolo, respete esa excepción sin extenderla a futuras sesiones o requerimientos.

Para requisitos con descubrimiento técnico, defina fases: investigación, aprobación del comportamiento/diseño recuperado e implementación. El Coordinator puede realizar la investigación con apoyo especializado y entrega evidencia al humano antes de delegar programación al Developer. No fuerce tests unitarios para un entregable exclusivamente documental: use revisiones y evidencias apropiadas, justificando lo no aplicable.

No exija conocer antes de investigar los comportamientos que la investigación debe descubrir. En cambio, defina preguntas, artefactos, entregables y condiciones para aprobar la fase siguiente. Separe observaciones, inferencias e incógnitas; cambios al contrato aprobado requieren revisión humana.

Cada expediente debe incluir un relevo: estado real, decisiones confirmadas, preguntas abiertas, insumos locales ignorados por Git, rutas y hashes cuando estén disponibles, comandos ejecutados frente a propuestos, entregables persistentes y siguiente acción autorizada. No interprete una aclaración del humano como aprobación global.

La validación se adapta al alcance: un generador aislado no requiere probar Git del sitio publicado; un cambio documental no requiere compilar la aplicación. La integración entre repositorios y preservación de versiones se prueban cuando el requerimiento las afecta. La falta de plataforma o fixture queda como pendiente, no como prueba exitosa.
