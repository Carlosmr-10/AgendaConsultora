# Fase 2 — Gestión de empresas

La entrega es independiente de Fase1 y conserva la gestión de personas.

## Ejecutar

Desde la raíz del proyecto, con .NET 10:

```bash
dotnet run --project Fase2/AgendaConsultora.csproj
```

El menú principal permite entrar en Personas o Empresas. En ambos submenús,
0 vuelve al principal; solo 0 en el principal cierra la aplicación.

## Empresas

- Alta con Id automático, nombre comercial, CIF, teléfono, correo y dirección.
- Todos esos campos son obligatorios. El CIF se comprueba como texto no vacío;
  no se exige una comprobación fiscal ni se consulta un registro oficial.
- Correo y teléfono reutilizan las validaciones de Fase1 (teléfono de 9 dígitos).
- Listado ordenado por nombre comercial y búsquedas por Id, nombre o CIF.
- Modificación con ficha actual, propuesta y confirmación; cancelar no cambia datos.
- `Estado` es booleano: `true` indica activa y `false` indica inactiva.
- Baja con confirmación: cambia exclusivamente `Estado` a `false`. No se elimina
  el objeto de la colección, no se borran personas y no se usa DELETE.
- La ficha muestra si la baja está disponible. Si tiene personas asociadas, no
  se solicita confirmación ni se permite desactivarla. El gestor vuelve a comprobarlo.
- La opción Reactivar cambia `Estado` a `true` tras confirmar, conservando el Id.
- Se muestran y buscan tanto activas como inactivas, con estado explícito.
  Modificar una inactiva no la reactiva automáticamente.

## Relación preparada

`Persona.EmpresaId` y `GestorPersonas.TieneRelacionConEmpresa` permiten comprobar
relaciones reales. `VincularEmpresa` prepara la asociación a una empresa activa
para la siguiente fase; no se ha añadido una opción de vinculación al menú.
Por tanto, las empresas creadas desde esta consola inicialmente no tienen relaciones.
Las pruebas de servicios crean una relación y verifican que bloquea la baja.
Modificar los datos de una persona conserva su EmpresaId.

La baja de personas conserva el comportamiento de Fase1. La regla de conservación
mediante Estado se aplica a empresas, conforme al alcance de este añadido.
Los datos siguen en memoria y se pierden al cerrar la aplicación.

## Estructura MVC de consola

```text
Fase2/
├── Program.cs
├── Models/
│   ├── Persona.cs
│   ├── Empresa.cs
│   ├── Services/
│   │   ├── GestorPersonas.cs
│   │   └── GestorEmpresas.cs
│   └── Validation/Validador.cs
├── Views/
│   ├── VistaAgenda.cs
│   ├── VistaPersonas.cs
│   ├── VistaEmpresas.cs
│   └── Entrada.cs
├── Controllers/
│   ├── ControladorAgenda.cs
│   ├── ControladorPersonas.cs
│   └── ControladorEmpresas.cs
├── pruebas/
└── evidencias/
```

- **Modelo:** entidades, colecciones, validaciones y reglas de negocio, incluido
  el bloqueo de baja y el campo Estado. No accede a la consola.
- **Vista:** todos los textos visibles, preguntas, menús, fichas, confirmaciones,
  formato y lectura de consola. Valida las entradas usando las reglas del modelo.
  No accede a los gestores ni modifica registros.
- **Controlador:** decide qué operación ejecutar, consulta o modifica el modelo
  y pide a la vista que recoja o muestre datos. No contiene textos de interfaz
  ni llamadas a Console.
- **Program:** Main contiene una única llamada a `ControladorAgenda.Iniciar()`.
  Ese método crea los gestores compartidos, las vistas y los controladores, y
  arranca el flujo de navegación. También se controla el fin de entrada.

Las excepciones de argumentos del modelo son diagnósticos internos; los mensajes
que se muestran al usuario están en Views. La Fase 1 conserva su estructura original.

## Verificación

```bash
dotnet build Fase2/AgendaConsultora.csproj -warnaserror
python3 Fase2/pruebas/verificar.py
dotnet run --project Fase2/pruebas/Servicios/Servicios.csproj
dotnet build Fase2/AgendaConsultora.csproj --configuration Release -warnaserror
```

Resultado: 23 escenarios de consola, 21 comprobaciones de servicios y compilación
Debug/Release con 0 errores y 0 advertencias. También se han repetido satisfactoriamente
los 64 escenarios de la entrega Fase1.

| Prueba manual | Resultado esperado |
|---|---|
| Entrar en Personas, volver, entrar en Empresas y volver | La aplicación continúa en el menú principal |
| Alta con nombre o CIF vacío | Se vuelve a pedir el campo |
| Correo o teléfono inválido | Rechazo y nueva solicitud |
| Buscar un Id inexistente | Mensaje claro, sin cambios |
| Modificar y contestar N | Se conserva la ficha anterior |
| Dar de baja sin relaciones y contestar S | Mismo Id y datos, Estado Inactiva |
| Buscar la empresa inactiva | Se encuentra y muestra su estado |
| Reactivar y contestar S | Mismo Id y datos, Estado Activa |
| Baja con relaciones (prueba de servicios) | Bloqueada; Estado sigue siendo true |
| Escribir texto o una opción inexistente en un menú | Mensaje de error y nueva solicitud |

Para reproducir la demostración:

```bash
dotnet run --project Fase2/AgendaConsultora.csproj --no-build < Fase2/evidencias/entrada-demo.txt
```

## Diez pruebas de aceptación de Fase 2

Desde la raíz, después de compilar Fase2 y Fase3:

```bash
python3 pruebas/verificar_10_por_fase.py
```

Las diez pruebas de esta fase cubren navegación y entradas inválidas, validaciones
obligatorias, listado vacío/ordenado, búsqueda, modificación confirmada/cancelada,
baja lógica, reactivación, cancelación de baja y continuidad de la gestión de personas.
El resultado está en `evidencias/aceptacion-10-pruebas.txt`.
