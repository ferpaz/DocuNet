# Entradas de pruebas

Los fixtures compilables están en src/tests/DocuNet.TestFixtures y las pruebas xUnit en src/tests/DocuNet.Tests. Este directorio admite datos mínimos adicionales por escenario; no aloja proyectos ni ensamblados privados.

Incluya origen, preparación y pruebas consumidoras al agregar un conjunto. Prefiera fuente compilable a DLL/PDB. No modifique entradas originales durante las pruebas; use directorios temporales.

Las muestras privadas permanecen en samples, ignoradas. Las salidas locales se guardan en tests/results/<codigo>/<ejecucion>/, también ignoradas. [Procedimiento y cobertura](../../docs/testing.md).