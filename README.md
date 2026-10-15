# Fase 5 — Modelo orientado a objetos y estructura profesional

Esta entrega reorganiza la aplicación completa de las fases anteriores. Mantiene
personas, empresas, altas, listados, búsquedas, modificaciones, bajas, reactivación,
asignación o cambio de empresa, desvinculación y consulta de plantillas.

Los datos continúan en memoria. No se ha añadido una base de datos porque el
enunciado no la exige.

## Estructura

```text
Fase5/
├── README.md
├── AgendaProfesional.sln
├── src/
│   └── AgendaProfesional.Console/
│       ├── AgendaProfesional.Console.csproj
│       ├── Program.cs
│       ├── Models/
│       │   ├── Persona.cs
│       │   ├── Empresa.cs
│       │   ├── DatosPersona.cs
│       │   └── DatosEmpresa.cs
│       ├── Services/
│       │   └── AgendaService.cs
│       ├── UI/
│       │   └── ConsoleUI.cs
│       └── Utils/
│           └── InputValidator.cs
├── tests/
│   ├── AgendaProfesional.Tests/
│   │   ├── AgendaProfesional.Tests.csproj
│   │   └── Program.cs
│   └── verificar_fase5.py
└── evidencias/
    ├── revision_10_puntos.md
    └── verificacion.txt
```

La solución contiene el proyecto de aplicación propuesto. Las pruebas permanecen
como un proyecto auxiliar independiente y referencian la aplicación sin modificarla.

## Reparto de responsabilidades

| Componente | Responsabilidad |
|---|---|
| `Persona` | Representa una persona. `IdEmpresa` es `int?`: puede ser `null` o contener el Id de una única empresa. |
| `Empresa` | Representa una empresa y conserva su registro mediante el booleano `Estado`. |
| `DatosPersona` y `DatosEmpresa` | Agrupan los datos recibidos en altas y modificaciones con nombres de propiedad claros. |
| `AgendaService` | Es propietario de `List<Persona>` y `List<Empresa>`. Ejecuta altas, búsquedas, modificaciones, bajas, colecciones, asignaciones y consultas de relaciones. |
| `ConsoleUI` | Muestra menús, solicita datos y presenta resultados. Solo usa los métodos públicos de `AgendaService`; no accede ni modifica directamente sus colecciones. |
| `InputValidator` | Lee texto, opciones, Id y confirmaciones de manera segura. Centraliza las reglas de correo, teléfono y campos obligatorios. |
| `Program` | Punto de entrada e inicialización: crea `AgendaService`, lo inyecta en `ConsoleUI` e inicia la interfaz con una sola instrucción. |

## Reglas conservadas

- Los Id se generan automáticamente y no se reutilizan durante la sesión.
- Los listados se devuelven como `IReadOnlyList<T>` y están ordenados.
- Una persona puede tener cero o una empresa mediante `IdEmpresa`.
- Una empresa puede tener cero, una o varias personas.
- Cambiar la empresa sustituye el Id anterior; no crea relaciones duplicadas.
- Desvincular conserva todos los datos personales.
- Una empresa inactiva no puede asignarse.
- Una empresa relacionada no puede darse de baja hasta trasladar o desvincular su plantilla.
- La baja de empresa cambia `Estado` a `false`; nunca elimina el objeto de la colección.
- Reactivar cambia `Estado` a `true` y conserva el mismo Id y datos.
- Una modificación se aplica después de validar todos los datos, evitando estados parciales.
- Las opciones no numéricas, fuera de rango y las confirmaciones inválidas no cierran la aplicación.

## Ejecución

Desde la raíz `AgendaConsultora`:

```bash
dotnet run --project Fase5/src/AgendaProfesional.Console/AgendaProfesional.Console.csproj
```

Desde `Fase5/`:

```bash
dotnet run --project src/AgendaProfesional.Console/AgendaProfesional.Console.csproj
```

La aplicación y las pruebas usan .NET 10, la versión instalada y comprobada en este
entorno.

## Compilación y pruebas

Desde la raíz `AgendaConsultora`:

```bash
dotnet build Fase5/AgendaProfesional.sln -warnaserror
dotnet run --project Fase5/tests/AgendaProfesional.Tests/AgendaProfesional.Tests.csproj
python3 Fase5/tests/verificar_fase5.py
dotnet build Fase5/AgendaProfesional.sln --configuration Release -warnaserror
```

Resultados comprobados:

- 38 comprobaciones directas de `AgendaService`, modelos y validadores.
- 10 escenarios completos contra la aplicación real de consola.
- Ejecución completa equivalente a Fase 4 después de normalizar únicamente el título.
- Compilación Debug y Release con cero errores y cero advertencias.
- Revisión documentada en diez puntos, incluida la regresión de Fases 1–4.

El resumen se conserva en `evidencias/verificacion.txt` y el detalle de la revisión
en `evidencias/revision_10_puntos.md`.

## Persistencia futura

Si se incorpora un archivo o base de datos, debe añadirse una capa independiente de
infraestructura. `ConsoleUI` seguirá comunicándose con el servicio y no contendrá
consultas, rutas de archivos ni instrucciones de base de datos.
