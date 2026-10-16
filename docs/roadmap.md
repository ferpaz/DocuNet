# Hoja de ruta

Estas líneas describen mejoras propuestas. No representan funciones disponibles ni fechas de entrega comprometidas.

## Presentación y navegación

Evolucionar las plantillas externas hacia una presentación moderna, responsive y orientada al desarrollador que consulta una API:

- Lectura cómoda en escritorio, tablet y móvil, incluidas firmas largas y ejemplos de código.
- Navegación por namespaces, tipos y miembros, con contexto de ubicación y enlaces directos a sobrecargas.
- Búsqueda y filtrado en el sitio estático, conservando el contexto durante la navegación.
- Acceso por teclado, foco visible, contraste y estructura semántica.
- Renovación de recursos heredados conforme al diseño.
- Pruebas visuales, de viewport y de accesibilidad.

Mantener separado el modelo de documentación de su presentación. El nuevo diseño debe preservar contenido, identificadores y destinos de enlaces, sin quedar ligado a snapshots de las plantillas actuales.

## Renovación de JavaScript y recursos heredados

Las plantillas actuales incluyen jQuery 1.3.2 (con Sizzle) y jQuery.ScrollTo 1.4.2. Se conservan por compatibilidad; su actualización no está implementada ni se presenta como resuelta.

Como parte de la modernización:

- Evaluar sustituir las funciones usadas por APIs nativas del navegador o dependencias mantenidas.
- Revisar avisos de seguridad y eliminar bibliotecas o funciones que no sean necesarias.
- Preservar navegación, desplazamiento, ejemplos de código y enlaces directos.
- Probar tamaños de pantalla, teclado, foco, historial y funcionamiento sin backend.
- Actualizar avisos/licencias y validar los sitios generados antes de retirar los recursos actuales.

La renovación del diseño y de JavaScript debe tratarse como una mejora con regresiones de comportamiento; no se realizará como una actualización automática de paquetes.

## Validación macOS

Ejecutar restauración, compilación, suite de pruebas, cobertura y generación con fixtures propios en macOS. Registrar versión del sistema, arquitectura y SDK/runtime; comprobar rutas, permisos, recursos y enlaces.

Incorporar macOS a CI cuando exista evidencia de ejecución allí. La matriz actual incluye Windows y Linux.
