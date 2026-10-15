# Cumplimiento de las fases 1–5

| Fase | Requisito | Implementación final |
|---|---|---|
| 1 | Gestión de personas | Alta, listado, búsqueda, modificación y baja desde la pestaña Personas. |
| 1 | Robustez y validaciones | Campos obligatorios, correo, teléfono y mensajes visuales; no se realizan conversiones numéricas desde texto. |
| 1 | Clases y colección | `Persona` y `List<Persona>` privada dentro de `AgendaService`. |
| 2 | Segundo menú de empresas | Pestaña Empresas independiente y accesible sin cerrar la aplicación. |
| 2 | CRUD de empresas | Alta, listado, búsqueda y modificación; la baja es lógica. |
| 2 | Estado y reactivación | `Estado` cambia entre `false` y `true`; nunca se elimina una empresa de la colección. |
| 3 | Relación | `Persona.IdEmpresa` es anulable; asignación, cambio y desvinculación implementados. |
| 3 | Consultas cruzadas | La persona muestra su empresa y la empresa muestra su plantilla. |
| 3 | Política de baja | La empresa con personas asociadas no puede desactivarse. |
| 4 | Funciones | Métodos separados para lectura visual, presentación, validación y operaciones de negocio. |
| 4 | Sin duplicación | Validaciones, refrescos, búsquedas y acceso por Id están centralizados. |
| 4 | Entrada breve | `Program` solo configura y arranca Avalonia; `App` inicializa vista, servicio y controlador. |
| 5 | Modelo orientado a objetos | Modelos `Persona`, `Empresa`, `DatosPersona` y `DatosEmpresa`. |
| 5 | Servicio | `AgendaService` contiene altas, búsquedas, asignaciones, bajas y colecciones. |
| 5 | Interfaz | `Vistas/VistaPrincipal` sustituye a `ConsoleUI`. |
| 5 | MVC | La vista publica eventos, el controlador coordina y el servicio aplica las reglas. |

## Verificaciones

- 29 comprobaciones automatizadas de reglas de negocio y validación.
- Compilación Debug: 0 errores y 0 advertencias.
- Compilación Release: 0 errores y 0 advertencias.
- Interfaz iniciada y revisada visualmente.
- Búsqueda estática: ninguna eliminación de empresas mediante `Remove`.
- Las colecciones solo existen en `AgendaService`.
