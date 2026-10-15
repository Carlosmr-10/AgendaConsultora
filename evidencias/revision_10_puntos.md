# Revisión en 10 puntos de la Fase 5

| Nº | Aspecto revisado | Evidencia | Resultado |
|---:|---|---|---|
| 1 | Alcance acumulado | Los diez escenarios recorren personas, empresas y relaciones; además se comparó una ejecución completa con Fase 4. | Conforme: Fase 5 engloba las fases 1–4. |
| 2 | Estructura profesional | Existen solución, proyecto en `src`, modelos, servicio, UI, utilidades y pruebas separadas. | Conforme con el árbol solicitado. |
| 3 | Modelos orientados a objetos | `Persona` contiene sus datos e `IdEmpresa` anulable; `Empresa` contiene sus datos y `Estado`. | Conforme con la cardinalidad cero o una empresa por persona. |
| 4 | Encapsulación de colecciones | Las dos listas son privadas de `AgendaService`; la UI recibe resultados de solo lectura. | `ConsoleUI` no crea ni modifica las colecciones. |
| 5 | Entradas y validaciones | Se probaron campos obligatorios, correo, teléfono, Id, opciones de menú y confirmaciones. | Las entradas incorrectas se repiten o se rechazan sin cerrar la aplicación. |
| 6 | Gestión completa | Las pruebas cubren alta, listado, búsqueda, modificación y baja de personas y empresas. | Todas las operaciones conservadas. |
| 7 | Relaciones | Se probaron asignación, cambio, consulta en ambos sentidos y desvinculación. | Una persona mantiene como máximo un `IdEmpresa`; una empresa admite varias personas. |
| 8 | Baja lógica y reactivación | Se comprobó el bloqueo con plantilla, la baja sin relaciones y la posterior reactivación. | La empresa permanece en la lista; solo cambia `Estado`. |
| 9 | Calidad técnica | Compilaciones Debug y Release con advertencias tratadas como errores, más `dotnet format --verify-no-changes`. | Cero errores, cero advertencias y formato correcto. |
| 10 | Regresión entre fases | Se recompilaron Fases 1–4 y se ejecutaron sus suites disponibles. | Fase 1: 64; Fases 2–4: 23 escenarios cada una; servicios: 21/31/31; matriz Fase 2–3: 20/20. |

## Resultado específico de Fase 5

- 38 comprobaciones directas de modelos, validadores y `AgendaService`.
- 10/10 escenarios de aceptación ejecutados contra la consola real.
- Compilaciones Debug y Release: 0 advertencias y 0 errores.
- Verificación de formato: sin cambios pendientes.
