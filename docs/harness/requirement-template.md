# Plantilla de requerimiento

Copie a `docs/requirements/YYYY-MM-DD_CODIGO_slug.md`. Complete antes de programar; justifique cada «No aplica».

# CODIGO — Título

- Inicio: YYYY-MM-DD (zona horaria del usuario)
- Estado: Borrador
- Coordinator / Developer / Tester:
- Modalidad: agentes separados o fases del mismo agente
- Base / branch de desarrollo / destino:
- Repositorios afectados:

## Objetivo y descripción

Problema, usuario beneficiado, resultado esperado, entradas y salidas. Incluya la versión de la biblioteca o del producto cuando corresponda.

## Alcance

Cambios incluidos, exclusiones, dependencias, riesgos y preguntas abiertas.

## Insumos, fases y decisiones

Liste artefactos y rutas, disponibilidad, sensibilidad y si están ignorados por Git. Registre identidad/hashes cuando se inspeccionen. Distinga decisiones confirmadas, propuestas y preguntas abiertas.

Si requiere investigación, defina preguntas y entregables, evidencia persistente y aprobación humana de resultados antes de implementar. Especifique equivalencia observable y diferencias permitidas cuando sustituya una herramienta existente.

## Criterios de aceptación

| ID | Escenario y resultado verificable | Pruebas asociadas |
| --- | --- | --- |
| CA-01 | Completar | UT-01, IT-01 |

## Diseño de pruebas

| ID | Tipo (unitaria/regresión/integración) | Entrada o fixture | Comando/procedimiento | Resultado esperado |
| --- | --- | --- | --- | --- |
| UT-01 | Unitaria | Completar | Completar | Completar |
| IT-01 | Integración | Completar | Completar | Completar |

Defina regresión, plataformas a validar, preparación/limpieza del entorno y ubicación de resultados. Incluya casos negativos y conservación de versiones existentes si aplica. No registre comandos propuestos como ejecutados.

## Aprobaciones humanas

| Puerta | Estado | Persona, fecha y referencia | Condiciones |
| --- | --- | --- | --- |
| Alcance antes de ejecutar | Pendiente | — | — |
| Comportamiento/diseño tras investigación, si aplica | Pendiente | — | Antes de programar |
| Resultado validado | Pendiente | — | — |
| Merge al destino | Pendiente | — | — |
| Push/despliegue, si aplica | Pendiente | — | — |

## Entrega del Developer y revisión del Coordinator

Archivos, decisiones, criterios satisfechos, pruebas unitarias ejecutadas, documentación actualizada y pendientes.

## Informe del Tester

Revisión probada, entorno, comandos realmente ejecutados, resultados y evidencia persistente. Enumere fallos y omisiones. Indique a quién devuelve el control.

## Trazabilidad y cierre

Commits de implementación, documentación, PR, merge, revisión integrada validada y estado final de cada repositorio. Registre excepciones de arranque, conflictos y decisiones humanas. Cierre únicamente tras aprobación y comprobación del árbol limpio.

## Relevo entre sesiones

Estado real, decisiones confirmadas, bloqueos, insumos que debe aportar el humano, documentos/evidencias para leer y próxima acción autorizada. Identifique pruebas ejecutadas, propuestas y pendientes. No dé por disponibles datos locales solo porque existían en otra sesión.
