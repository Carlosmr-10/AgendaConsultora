# Revisión en diez pasadas — 13 de octubre de 2026

Cada pasada revisa un aspecto distinto de Fase 4. La auditoría es reproducible con:

```bash
python3 Fase4/pruebas/revision_10_pasadas.py
```

| Pasada | Aspecto comprobado | Evidencia y resultado |
|---:|---|---|
| 1 | Conservación de todas las fases | Se ejecuta la misma demostración completa en Fase 3 y Fase 4. Tras normalizar exclusivamente el título de fase, las salidas coinciden exactamente. |
| 2 | Comportamiento de consola | Los 23 escenarios heredados superan altas, búsquedas, modificaciones, bajas, reactivación, relaciones, cancelaciones, entradas inválidas y fin de entrada. |
| 3 | Reglas del modelo | Las 31 comprobaciones de servicios superan validación, Id, orden, Estado, baja lógica, relación, cambio, desvinculación y bloqueo de empresa relacionada. |
| 4 | Compilación Debug | `dotnet build` con advertencias tratadas como errores finaliza con 0 advertencias y 0 errores. |
| 5 | Compilación Release | La compilación optimizada con advertencias tratadas como errores finaliza con 0 advertencias y 0 errores. |
| 6 | Main y flujo principal | `Main` contiene una única llamada. Creación, ejecución, menú principal y procesamiento de opciones están en funciones con nombres propios. |
| 7 | Separación de responsabilidades | Solo las vistas acceden a `Console`. `Entrada` lee de forma segura, `Validador` contiene reglas puras, los controladores coordinan y los gestores operan sobre los datos. |
| 8 | Reutilización y nombres | `BuscarPorId<T>` se comparte entre personas y empresas; `MostrarColeccion<T>` se reutiliza en tres listados. Cada operación tiene una función descriptiva. |
| 9 | Parámetros, retornos y estado | `DatosPersona` y `DatosEmpresa` reemplazan tuplas; búsquedas anulables, operaciones booleanas y listas de solo lectura expresan sus contratos. No existe estado global mutable. |
| 10 | README, fases y políticas | El README explica las funciones principales. Las cuatro fases están separadas. Las empresas usan `Estado = false`, nunca se eliminan de la colección, y `EmpresaId` mantiene la relación. |

## Resultado

Las diez pasadas terminaron correctamente. No se detectó ningún incumplimiento del
enunciado de Fase 4 ni ninguna regresión funcional respecto a Fase 3.

Como comprobación adicional, `dotnet format --verify-no-changes` terminó sin
diagnósticos: el código también cumple el formato esperado por las herramientas de
.NET.

Durante la preparación se corrigió una regla del propio script de auditoría que
confundía métodos estáticos con variables globales. Esa corrección solo afecta a la
precisión del comprobador; el código de la aplicación ya cumplía la condición.

Las pruebas reducen de forma importante el riesgo de defectos, aunque ninguna
revisión de software puede demostrar matemáticamente la ausencia absoluta de todos
los errores posibles.
