# Fase 4 — Organización mediante funciones

Esta entrega parte de la Fase 3, que ya contiene todas las funcionalidades de las
Fases 1 y 2. La refactorización conserva la gestión de personas, la gestión de
empresas, la baja lógica mediante `Estado`, la reactivación y las relaciones entre
personas y empresas. No se ha añadido ni eliminado ninguna operación funcional.

## Ejecución

Desde la carpeta raíz `AgendaConsultora`:

```bash
dotnet run --project Fase4/AgendaConsultora.csproj
```

Requiere .NET 10. Los datos se mantienen en memoria durante la ejecución.

## Reparto de responsabilidades

### Arranque y coordinación general

- `Program.Main`: contiene una única llamada a `ControladorAgenda.Iniciar()`.
- `ControladorAgenda.Iniciar`: crea la aplicación mediante `CrearControlador` y
  comienza su ejecución.
- `CrearControlador`: construye una sola instancia de cada gestor compartido,
  conecta las vistas con sus controladores y devuelve el controlador principal.
- `EjecutarMenuPrincipal`: mantiene el menú activo hasta seleccionar salir.
- `ProcesarOpcionPrincipal`: dirige cada opción al controlador correspondiente y
  devuelve `true` solamente cuando hay que terminar.

### Presentación, lectura y validación

- `VistaAgenda.LeerOpcion`, `VistaPersonas.LeerOpcion` y
  `VistaEmpresas.LeerOpcion`: presentan cada menú y devuelven una opción válida.
- `VistaPersonas.LeerDatos` y `VistaEmpresas.LeerDatos`: solicitan los campos y
  devuelven objetos `DatosPersona` o `DatosEmpresa`, evitando tuplas ambiguas.
- `Entrada.Texto`: repite una pregunta hasta superar la función de validación
  recibida como parámetro.
- `Entrada.Numero`: usa `IntentarLeerNumeroEnRango` para evitar excepciones y
  aceptar únicamente números dentro de los límites indicados.
- `Entrada.Confirmar`: reutiliza `EsRespuestaAfirmativa` y
  `EsRespuestaNegativa` para interpretar S/SI/SÍ y N/NO.
- `Validador`: contiene reglas puras para texto obligatorio, correo y teléfono.
- Las vistas contienen todos los textos y formatos de consola. Los gestores y
  controladores no escriben directamente en `Console`.

### Operaciones reutilizables de los controladores

- `OperacionesControlador.BuscarPorId<T>`: recibe las funciones para leer el Id,
  buscar el elemento y avisar si no existe. Se reutiliza en personas y empresas.
- `OperacionesControlador.MostrarColeccion<T>`: muestra una colección con una
  función recibida o ejecuta el aviso de colección vacía.
- `ProcesarOpcion` en cada controlador: asigna cada opción a una función con un
  nombre descriptivo y devuelve si debe volver al menú principal.
- `BuscarPorId` y `BuscarPorNombre`/`BuscarPorTexto`: separan los dos tipos de
  búsqueda sin duplicar la localización por identificador.
- `DarDeAlta`, `Modificar`, `DarDeBaja`, `CambiarEstado`, `AsignarEmpresa`,
  `DesvincularEmpresa` y `VerPlantilla`: cada función coordina una sola operación.
- `LocalizarEmpresaAsignable`: comprueba existencia, estado y asignación previa
  antes de devolver una empresa válida para crear o cambiar una relación.

### Modelo y gestión de datos

- `GestorPersonas` y `GestorEmpresas`: poseen las colecciones y ejecutan altas,
  búsquedas, modificaciones y bajas. La consola no conoce esas colecciones.
- `NormalizarDatos`: elimina espacios exteriores y devuelve un objeto de datos.
- `CrearPersonaValidada` y `CrearEmpresaValidada`: validan antes de crear.
- `AplicarDatos`: actualiza los campos modificables de una empresa después de
  validar toda la propuesta, evitando cambios parciales.
- `CoincideConTodasLasPalabras`: concentra la regla de búsqueda parcial de personas.
- `PuedeDesactivar`: centraliza la regla que bloquea una empresa relacionada.
- `GestorRelaciones`: asigna, cambia, consulta y elimina la asociación sin duplicar
  listas ni permitir que una persona pertenezca a más de una empresa.

## Parámetros, retornos y estado

- Las lecturas compuestas devuelven `DatosPersona` y `DatosEmpresa`; sus nombres de
  propiedad documentan el significado de cada valor.
- Las búsquedas devuelven el objeto encontrado o `null`.
- Las operaciones que pueden fallar devuelven `bool`.
- Los listados se exponen como `IReadOnlyList<T>` para impedir modificaciones desde
  los controladores.
- No hay variables globales mutables. Las colecciones y contadores son campos
  privados de cada gestor porque representan el estado en memoria de la sesión.
- `GestorPersonas.Orden` es el único campo estático: es un comparador inmutable y
  compartido, sin datos de personas ni empresas.

## Ausencia de duplicación

La localización por Id y la presentación de colecciones se implementan una sola vez
mediante funciones genéricas. Las reglas de entrada numérica y confirmación también
están centralizadas. La vista de personas se comparte al mostrar la plantilla de una
empresa, por lo que el formato de una persona no se repite.

## Verificación

```bash
dotnet build Fase4/AgendaConsultora.csproj -warnaserror
python3 Fase4/pruebas/verificar.py
dotnet run --project Fase4/pruebas/Servicios/Servicios.csproj
dotnet build Fase4/AgendaConsultora.csproj --configuration Release -warnaserror
python3 Fase4/pruebas/revision_10_pasadas.py
dotnet format Fase4/AgendaConsultora.csproj --verify-no-changes --no-restore
```

La aplicación conserva los 23 escenarios de consola y las 31 comprobaciones de
servicios heredadas de Fase 3. También se compara una demostración completa de Fase
3 y Fase 4, ignorando únicamente el título de fase, para confirmar que el flujo y
los resultados funcionales permanecen iguales.

La auditoría final realiza diez pasadas diferenciadas y deja su informe en
`evidencias/revision-10-pasadas.md`.
