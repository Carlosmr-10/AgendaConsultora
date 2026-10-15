# Fase 1 — Gestión de personas

## Alcance

Aplicación de consola basada en las dos páginas de «Agenda Consultora - Fase 1.pdf».
Incluye alta, listado ordenado, búsqueda por Id o nombre/apellidos, modificación,
baja y salida. Todos los datos son obligatorios: nombre, apellidos, correo y teléfono.
La gestión de empresas y la relación persona–empresa quedan para fases posteriores.

## Diseño para explicar en clase

- `Models/Persona.cs`: representa una persona con Id y propiedades de solo lectura.
- `Services/GestorPersonas.cs`: mantiene una `List<Persona>` privada y realiza el CRUD.
  Los listados son copias ordenadas por apellidos, nombre e Id, con ordenación española.
- `Helpers/Validador.cs`: reúne reglas reutilizables. El correo combina `MailAddress`
  con comprobación del dominio con punto y rechazo de puntos iniciales, finales o consecutivos en la parte local; es una validación básica de formato, no de existencia.
  El teléfono exige exactamente nueve dígitos ASCII; no admite prefijos ni separadores.
- `Helpers/Entrada.cs`: repite las preguntas incorrectas y usa `int.TryParse`.
- `Program.cs`: muestra el menú, recoge los datos y solicita confirmaciones.

El contador empieza en 1, solo aumenta en altas válidas y no reutiliza Id eliminados.
Se reinicia junto con la colección al abrir otra sesión. No hay persistencia.
La modificación recoge una propuesta en variables locales. Solo después de confirmar
se reemplaza el objeto en la lista conservando su Id; cancelar no altera el registro.
El modelo no expone setters para impedir cambios desde fuera del gestor.

Las entradas numéricas fuera de rango, vacías, no numéricas o demasiado grandes
se rechazan sin excepciones. Las confirmaciones aceptan S/SI/SÍ y N/NO, sin distinguir
mayúsculas. Una respuesta distinta vuelve a solicitarse. EOF cierra limpiamente
y cancela una operación pendiente, evitando un bucle infinito con entrada redirigida.

## Ejecución

Desde la raíz de esta entrega:

```bash
dotnet build Fase1/AgendaConsultora.csproj
dotnet run --project Fase1/AgendaConsultora.csproj
```

Para crear una copia desde cero con el SDK de .NET 10:

```bash
dotnet new console -n AgendaConsultora -o AgendaConsultora/Fase1 --framework net10.0
cd AgendaConsultora/Fase1
mkdir -p Models Services Helpers
```

Copiar los seis archivos del proyecto respetando sus rutas y ejecutar `dotnet run`.
Para .NET 8, cambiar `TargetFramework` a `net8.0` y disponer de su SDK/runtime;
la compilación comprobada de esta entrega corresponde a .NET 10.

## Prueba y demostración

Se ha compilado con cero errores y cero advertencias. Se han ejecutado 64 escenarios
de consola, incluyendo campos vacíos, formatos inválidos, búsquedas, ordenación,
confirmaciones, cancelación sin cambios, Id no reutilizados, desbordamiento numérico
y fin de entrada. El resumen está en `evidencias/verificacion.txt`. Las diez revisiones y su trazabilidad
se detallan en `evidencias/revision-10-pasadas.md`.

Repetir la evidencia completa, desde `Fase1/`:

```bash
dotnet run --no-build < evidencias/entrada-demo.txt
```

`evidencias/salida-demo.txt` contiene la salida real (la entrada redirigida no se muestra
como eco). Para clase, introducir manualmente los valores de `entrada-demo.txt`:
crear Ana Zamora y Luis Alonso, listar, buscar ALON, modificar el correo de Ana,
eliminar a Luis confirmando y salir. El listado coloca a Luis antes de Ana.

## Evolución

Crear una carpeta `Fase2/` que conserve el comportamiento de esta versión. Añadir
`Models/Empresa.cs` con Id, nombre comercial, CIF, teléfono, correo y dirección;
otros datos del contexto son opcionales. Añadir `Services/GestorEmpresas.cs`
con una colección privada `List<Empresa>`. Incorporar `int? EmpresaId` a Persona
y validar que la empresa exista antes de asociarla. Para mostrar sus personas,
añadir `ListarPorEmpresa(int empresaId)` y filtrar la colección de personas.
Decidir explícitamente qué ocurre al eliminar una empresa con personas asociadas.
No hay clases, datos ni opciones funcionales de empresa en esta primera fase.

## Repetir las pruebas automáticas

Desde la raíz del proyecto, con Python 3 instalado:

```bash
dotnet build Fase1/AgendaConsultora.csproj
python3 Fase1/pruebas/verificar.py
```

El script utiliza únicamente la biblioteca estándar de Python, ejecuta la aplicación
real y comprueba su salida y código de terminación. No añade dependencias al programa C#.
