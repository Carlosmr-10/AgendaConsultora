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

var rp = new GestorPersonas();
var re = new GestorEmpresas(rp);
var relaciones = new GestorRelaciones(rp, re);
var ana = rp.Alta("Ana", "Zamora", "ana@consultora.es", "612345678");
var luis = rp.Alta("Luis", "Alonso", "luis@consultora.es", "623456789");
var alfa = re.Alta("Alfa", "B1", "912345678", "alfa@consultora.es", "Calle Uno");
var beta = re.Alta("Beta", "B2", "923456789", "beta@consultora.es", "Calle Dos");
Verificar(relaciones.EmpresaDePersona(ana.Id) is null, "Persona inicialmente sin empresa");
Verificar(!relaciones.Asignar(999, alfa.Id) && !relaciones.Asignar(ana.Id, 999), "Asignación exige ambos Id existentes");
Verificar(relaciones.Asignar(ana.Id, alfa.Id) && ReferenceEquals(relaciones.EmpresaDePersona(ana.Id), alfa), "Asignación consultable");
Verificar(relaciones.Asignar(luis.Id, alfa.Id) && rp.ListarPorEmpresa(alfa.Id).Count == 2, "Una empresa admite varias personas");
Verificar(!re.Desactivar(alfa.Id) && alfa.Estado, "Relaciones impiden baja lógica");
Verificar(relaciones.Asignar(ana.Id, beta.Id) && rp.ListarPorEmpresa(alfa.Id).Count == 1 && rp.ListarPorEmpresa(beta.Id).Count == 1, "Cambio no duplica asociación");
Verificar(relaciones.Desvincular(ana.Id) && ana.EmpresaId is null && ana.Correo == "ana@consultora.es", "Desvincular conserva datos");
Verificar(!relaciones.Desvincular(ana.Id) && !relaciones.Desvincular(999), "Desvinculación inexistente controlada");
Verificar(re.Desactivar(beta.Id) && !relaciones.Asignar(ana.Id, beta.Id) && ana.EmpresaId is null, "No asociar empresa desactivada");
Verificar(re.Activar(beta.Id) && relaciones.Asignar(ana.Id, beta.Id), "Reactivar permite nueva asignación");
Console.WriteLine($"Total: {pruebas} comprobaciones de servicios superadas.");
