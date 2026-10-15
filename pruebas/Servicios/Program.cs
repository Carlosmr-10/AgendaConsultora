using AgendaConsultora.Models.Services;

int pruebas = 0;
void Verificar(bool condicion, string descripcion)
{
    if (!condicion) throw new Exception(descripcion);
    Console.WriteLine("OK: " + descripcion);
    pruebas++;
}
var personas = new GestorPersonas();
var empresas = new GestorEmpresas(personas);
var empresa = empresas.Alta("Zeta", "B12345678", "912345678", "zeta@consultora.es", "Calle Uno");
Verificar(empresa.Estado && empresa.Id == 1, "Alta activa con Id automático");
Verificar(empresas.Desactivar(1), "Baja sin relaciones permitida");
Verificar(!empresa.Estado && empresas.Listar().Count == 1 && ReferenceEquals(empresas.BuscarPorId(1), empresa), "La baja conserva el mismo registro");
Verificar(empresas.Buscar("b123").Single().Id == 1, "Búsqueda por CIF de inactiva");
Verificar(!empresas.Desactivar(1), "Doble baja rechazada");
Verificar(empresas.Modificar(1, "Alfa", "B12345678", "912345678", "alfa@consultora.es", "Calle Dos") && !empresa.Estado, "Modificar no reactiva");
var persona = personas.Alta("Ana", "Zamora", "ana@consultora.es", "612345678");
Verificar(!personas.VincularEmpresa(persona.Id, empresa), "No asociar una empresa inactiva");
Verificar(empresas.Activar(1) && empresa.Estado, "Reactivar conserva identidad");
Verificar(!empresas.Activar(1), "Doble activación rechazada");
Verificar(personas.VincularEmpresa(persona.Id, empresa), "Preparación de relación con empresa activa");
Verificar(!empresas.PuedeDesactivar(1) && !empresas.Desactivar(1) && empresa.Estado, "Bloqueo de baja con relaciones en el gestor");
personas.Modificar(persona.Id, "Ana María", "Zamora", "ana@consultora.es", "612345678");
Verificar(personas.TieneRelacionConEmpresa(1) && !empresas.Desactivar(1), "Modificar persona conserva la relación y el bloqueo");
var segunda = empresas.Alta("Beta", "B87654321", "923456789", "beta@consultora.es", "Calle Tres");
Verificar(segunda.Id == 2 && empresas.Desactivar(2), "Baja solo afecta a la empresa seleccionada");
Verificar(empresa.Estado && personas.BuscarPorId(persona.Id) is not null, "No se alteran empresa vinculada ni personas");
foreach (var datos in new[] {
    ("", "B1", "912345678", "a@dominio.es", "Calle"),
    ("Nombre", " ", "912345678", "a@dominio.es", "Calle"),
    ("Nombre", "B1", "letras", "a@dominio.es", "Calle"),
    ("Nombre", "B1", "912345678", "correo", "Calle"),
    ("Nombre", "B1", "912345678", "a@dominio.es", "") })
{
    bool rechazado = false;
    try { empresas.Modificar(1, datos.Item1, datos.Item2, datos.Item3, datos.Item4, datos.Item5); }
    catch (ArgumentException) { rechazado = true; }
    Verificar(rechazado && empresa.NombreComercial == "Alfa" && empresa.Estado, "Modificación inválida no deja cambios parciales");
}
Verificar(!empresas.Desactivar(99) && !empresas.Activar(99) && !empresas.Modificar(99,"N","B","912345678","a@dominio.es","C"), "Id inexistente controlado");
Verificar(empresas.Listar().Select(e => e.NombreComercial).SequenceEqual(new[]{"Alfa","Beta"}), "Listado ordenado con estados conservados");
Console.WriteLine($"{pruebas} comprobaciones de servicios superadas.");
