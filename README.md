# Fase 3 — Relación entre personas y empresas

Entrega independiente basada en Fase 2, según los requisitos de la imagen del enunciado.
Se conservan las operaciones de personas y empresas, el patrón MVC de consola y
`Main` con una sola llamada a `ControladorAgenda.Iniciar()`.

## Ejecutar

Desde la raíz `AgendaConsultora`:

```bash
dotnet run --project Fase3/AgendaConsultora.csproj
```

Requiere .NET 10. Los datos se mantienen en memoria durante la sesión.

## Nuevas operaciones

En el menú Personas:

- **6. Asignar o cambiar empresa:** introduce el Id de una persona y el de una
  empresa existentes. Muestra la empresa de destino y pide confirmación. La empresa
  debe estar activa. Asignar otra sustituye la relación anterior: nunca se acumulan
  varias empresas en una persona.
- **7. Desvincular empresa:** muestra la persona y pide confirmación. Su EmpresaId
  pasa a null y conserva el resto de sus datos.
- El listado, las búsquedas y las fichas muestran el nombre e Id de la empresa
  asociada, o indican que no tiene empresa asignada.

En el menú Empresas:

- **7. Ver personas de una empresa:** solicita su Id y muestra su plantilla ordenada
  por apellidos y nombre. Si no hay personas, lo indica expresamente.
- Se mantienen la baja lógica y la reactivación.

En los submenús, 0 vuelve al principal. Solo 0 en el principal termina la aplicación.

## Política para empresas relacionadas

**No se permite desactivar una empresa mientras tenga personas asociadas.**
Se deben desvincular o trasladar antes esas personas. La interfaz informa del bloqueo
sin solicitar confirmación de baja, y el gestor comprueba la misma condición.
Cuando no quedan relaciones, la baja cambia únicamente `Estado = false`.
No se elimina el registro de empresa ni se borran personas en cascada.
La reactivación cambia `Estado = true` y conserva el mismo Id y datos.

## Diseño

- `Models/Persona.cs`: `int? EmpresaId` expresa cero o una empresa por persona.
- `Models/Services/GestorRelaciones.cs`: comprueba ambos identificadores al asignar,
  impide la asignación de empresas inactivas y permite consultar o quitar la relación.
- `GestorPersonas.ListarPorEmpresa`: calcula la plantilla desde las personas, sin
  mantener una segunda lista de relaciones que pudiera desincronizarse.
- La modificación de datos personales conserva EmpresaId. El cambio de nombre de
  empresa aparece en las consultas porque se resuelve su Id al mostrar la ficha.
- `Controllers/`: coordina gestores, confirmaciones y consultas, sin textos ni Console.
- `Views/`: textos, lectura y presentación. `VistaRelaciones` contiene los mensajes
  de asignación y desvinculación; las vistas no acceden a los gestores.
- `Program.cs`: arranque mediante una sola llamada.

## Verificación reproducible

Desde la raíz:

```bash
dotnet build Fase2/AgendaConsultora.csproj -warnaserror
dotnet build Fase3/AgendaConsultora.csproj -warnaserror
python3 pruebas/verificar_10_por_fase.py
python3 Fase2/pruebas/verificar.py
python3 Fase3/pruebas/verificar.py
dotnet run --project Fase2/pruebas/Servicios/Servicios.csproj
dotnet run --project Fase3/pruebas/Servicios/Servicios.csproj
```

Se han ejecutado diez pruebas de aceptación distintas en Fase 2 y diez en Fase 3.
Cada prueba puede contener varias ejecuciones y aserciones para cubrir resultados,
cancelaciones y errores. Los informes están en `evidencias/aceptacion-10-pruebas.txt`
de cada fase; el código reproducible está en `pruebas/verificar_10_por_fase.py` de la raíz.

Además, ambas fases superan sus 23 escenarios heredados de consola; Fase 2 supera
21 comprobaciones de servicios y Fase 3 supera 31, incluidas diez de relaciones.
Las dos fases compilan en Debug y Release sin errores ni advertencias.

| Prueba Fase 3 | Resultado esperado |
|---|---|
| Persona recién creada | Sin empresa asignada |
| Asignación válida | Empresa visible en ficha, búsqueda y listado |
| Cambio de empresa | Desaparece de la plantilla anterior y aparece en la nueva |
| Desvinculación | Mantiene nombre, correo y teléfono; queda sin empresa |
| Empresa con cero o varias personas | Mensaje de plantilla vacía o listado ordenado |
| Baja con personas | Bloqueada, empresa sigue activa |
| Desvincular, desactivar y reactivar | Misma empresa conservada; se puede reasignar |
| Id inválido/inexistente o empresa inactiva | Rechazo sin crear una relación |
| Cancelación o fin de entrada | No guarda la asignación pendiente |
| Modificar datos personales o empresa | Conserva la relación y refleja los datos actualizados |

Las pruebas cubren los requisitos y escenarios indicados, sin prometer ausencia
absoluta de cualquier posible defecto.
