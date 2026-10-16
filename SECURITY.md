# Política de seguridad

## Versiones atendidas

Se evalúan reportes sobre la revisión más reciente de master. El proyecto no mantiene actualmente ramas LTS ni garantiza correcciones retroactivas para versiones antiguas.

## Reportar una vulnerabilidad

No publique detalles explotables, credenciales, DLL privadas ni pruebas de concepto sensibles en Issues o pull requests públicos.

Si el repositorio tiene habilitado el reporte privado de vulnerabilidades de GitHub, utilice Security → Report a vulnerability. Si no está disponible, escriba al mantenedor a [ferpaz64@gmail.com](mailto:ferpaz64@gmail.com) con el asunto “DocuNet: reporte de seguridad”. Use ese primer contacto para acordar cómo compartir material sensible; no adjunte ensamblados confidenciales.

Incluya:

- Revisión o versión afectada, sistema operativo y SDK/runtime.
- Componente y comando implicados.
- Pasos mínimos de reproducción con datos sintéticos.
- Impacto y condiciones necesarias, distinguiendo observaciones de suposiciones.
- Si el problema afecta al CLI, las plantillas o el sitio generado.

La revisión se realiza según disponibilidad; no se promete un plazo fijo. El mantenedor coordinará reproducción, corrección y divulgación con quien reporte el problema. Los errores sin impacto de seguridad pueden registrarse en GitHub Issues.

## Límites de confianza

DocuNet lee metadatos sin ejecutar código ni constructores de atributos de la biblioteca documentada. Esto no equivale a un sandbox para entradas malformadas: limite permisos y recursos al procesar archivos de origen desconocido.

Las plantillas Spark compilan y ejecutan C# de quien las edita; use únicamente plantillas de confianza. No acepte plantillas arbitrarias de usuarios en un servicio público sin aislamiento externo.

El contenido XML se escapa en las plantillas suministradas. Las personalizaciones deben preservar ese escape y comprobar enlaces y rutas. El manifiesto de salida debe conservarse para proteger archivos administrados y contenido ajeno.

Las plantillas actuales incluyen jQuery y ScrollTo heredados. Su sustitución figura en la [hoja de ruta](docs/roadmap.md); no se afirma que sean dependencias modernas o libres de vulnerabilidades. Comunique cualquier impacto reproducible sobre DocuNet por el canal privado anterior.

## Publicación y dependencias

Conserve avisos y licencias de terceros en las distribuciones. Revise alertas de dependencias y cambios de seguridad antes de publicar versiones. La existencia de esta política no activa por sí sola el reporte privado de GitHub ni otras opciones de seguridad del repositorio.
