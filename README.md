# Agenda Consultora — versión final

Aplicación gráfica de escritorio que reúne los requisitos de las fases 1–5 en un
único proyecto MVC.

## Tecnología

- C# y .NET 10.
- Avalonia 12.1.4 para la interfaz gráfica multiplataforma.
- Extensión de VS Code: `AvaloniaTeam.vscode-avalonia`.
- Datos en memoria, sin base de datos.

Se usa Avalonia porque es la extensión y plantilla instalada en este equipo. .NET
MAUI no está instalado y no ofrece escritorio Linux nativo.

## Estructura MVC

```text
AgendaConsultora/
├── AgendaConsultora.sln
├── README.md
├── src/
│   └── AgendaConsultora.App/
│       ├── Modelos/
│       │   ├── Persona.cs
│       │   ├── Empresa.cs
│       │   ├── DatosPersona.cs
│       │   └── DatosEmpresa.cs
│       ├── Vistas/
│       │   ├── VistaPrincipal.axaml
│       │   ├── VistaPrincipal.axaml.cs
│       │   └── ModelosVista.cs
│       ├── Controladores/
│       │   └── ControladorAgenda.cs
│       ├── Servicios/
│       │   └── AgendaService.cs
│       ├── Validacion/
│       │   └── InputValidator.cs
│       ├── App.axaml
│       ├── App.axaml.cs
│       ├── Program.cs
│       └── AgendaConsultora.App.csproj
├── tests/
│   └── AgendaConsultora.Tests/
├── evidencias/
└── respaldos/
```

- **Modelos:** representan personas, empresas y `IdEmpresa` opcional.
- **Vistas:** muestran formularios, pestañas, listados, mensajes y confirmaciones.
- **Controlador:** recibe eventos de la vista y coordina todas las operaciones.
- **Servicio:** mantiene las colecciones y aplica las reglas de negocio.
- **Validación:** comprueba correo, teléfono y campos obligatorios.

La vista no accede a las colecciones. El controlador no contiene controles de
Avalonia. El servicio no contiene código visual.

## Funcionalidad

- Alta, listado, búsqueda, modificación y baja de personas.
- Alta, listado, búsqueda y modificación de empresas.
- Id autogenerado para personas y empresas.
- Listados ordenados por apellidos o nombre comercial.
- Búsquedas parciales sin distinguir mayúsculas.
- Confirmación antes de modificar, eliminar o cambiar relaciones.
- Una persona puede estar sin empresa o pertenecer a una única empresa.
- Una empresa puede tener cero, una o varias personas.
- Asignación, cambio y desvinculación de empresa.
- Consulta de la empresa de una persona y de la plantilla de una empresa.
- Baja lógica de empresa mediante `Estado = false`; nunca se elimina su registro.
- Bloqueo de baja mientras existan personas asociadas.
- Reactivación mediante `Estado = true` conservando Id y datos.
- Validación de correo, teléfono español y campos obligatorios.
- Mensajes claros ante operaciones incorrectas.
- Los datos permanecen en memoria durante la sesión.

## Ejecutar

Desde esta carpeta:

```bash
dotnet run --project src/AgendaConsultora.App/AgendaConsultora.App.csproj
```

## Compilar y probar

```bash
dotnet build AgendaConsultora.sln --configuration Release -warnaserror
dotnet run --project tests/AgendaConsultora.Tests/AgendaConsultora.Tests.csproj
```

Resultado comprobado: 29 verificaciones de negocio, compilación Debug y Release sin
errores ni advertencias, formato correcto y arranque visual validado.

El cumplimiento por fases está documentado en
`evidencias/cumplimiento-fases-1-a-5.md`.
