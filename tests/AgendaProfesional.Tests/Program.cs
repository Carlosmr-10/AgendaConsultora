using AgendaProfesional.Models;
using AgendaProfesional.Services;
using AgendaProfesional.Utils;

int comprobaciones = 0;

void Verificar(bool condicion, string descripcion)
{
    if (!condicion) throw new InvalidOperationException(descripcion);
    comprobaciones++;
    Console.WriteLine($"OK - {descripcion}");
}

void VerificarArgumentoInvalido(Action accion, string descripcion)
{
    bool rechazada = false;
    try
    {
        accion();
    }
    catch (ArgumentException)
    {
        rechazada = true;
    }
    Verificar(rechazada, descripcion);
}

var agenda = new AgendaService();

Persona ana = agenda.DarDeAltaPersona(
    new DatosPersona(" Ana ", " Zamora ", " ana@consultora.es ", " 612345678 "));
Persona luis = agenda.DarDeAltaPersona(
    new DatosPersona("Luis", "Alonso", "luis@consultora.es", "623456789"));
Verificar(ana.Id == 1 && luis.Id == 2, "Id de personas autoincremental");
Verificar(ana.Nombre == "Ana" && ana.Correo == "ana@consultora.es", "Normalización de datos personales");
Verificar(agenda.ListarPersonas().Select(p => p.Id).SequenceEqual([2, 1]), "Personas ordenadas por apellidos");
Verificar(agenda.BuscarPersonas("ZAM an").Single().Id == ana.Id, "Búsqueda parcial sin distinguir mayúsculas");
Verificar(agenda.BuscarPersonaPorId(99) is null, "Búsqueda de persona inexistente controlada");
VerificarArgumentoInvalido(
    () => agenda.DarDeAltaPersona(new DatosPersona("", "Apellido", "correo", "letras")),
    "Alta personal inválida rechazada por el servicio");

Empresa alfa = agenda.DarDeAltaEmpresa(
    new DatosEmpresa(" Alfa ", " B12345678 ", " 912345678 ", " alfa@consultora.es ", " Calle Uno "));
Empresa beta = agenda.DarDeAltaEmpresa(
    new DatosEmpresa("Beta", "B87654321", "923456789", "beta@consultora.es", "Calle Dos"));
Verificar(alfa.Id == 1 && beta.Id == 2 && alfa.Estado, "Id de empresas y Estado inicial activo");
Verificar(alfa.NombreComercial == "Alfa" && alfa.Cif == "B12345678", "Normalización de datos empresariales");
Verificar(agenda.ListarEmpresas().Select(e => e.Id).SequenceEqual([1, 2]), "Empresas ordenadas por nombre comercial");
Verificar(agenda.BuscarEmpresas("b123").Single().Id == alfa.Id, "Búsqueda de empresa por CIF");
VerificarArgumentoInvalido(
    () => agenda.DarDeAltaEmpresa(new DatosEmpresa("", "", "1", "correo", "")),
    "Alta empresarial inválida rechazada por el servicio");

Verificar(agenda.DesactivarEmpresa(beta.Id), "Empresa sin relaciones desactivada");
Verificar(!beta.Estado && ReferenceEquals(agenda.BuscarEmpresaPorId(beta.Id), beta), "Baja lógica conserva el objeto");
Verificar(!agenda.AsignarEmpresa(ana.Id, beta.Id), "Empresa inactiva no asignable");
Verificar(agenda.ActivarEmpresa(beta.Id) && beta.Estado, "Empresa inactiva reactivada");
Verificar(!agenda.ActivarEmpresa(beta.Id), "Doble activación rechazada");

Verificar(agenda.AsignarEmpresa(ana.Id, alfa.Id), "Empresa asignada a persona existente");
Verificar(ReferenceEquals(agenda.ObtenerEmpresaDePersona(ana.Id), alfa), "Empresa de una persona consultable");
Verificar(agenda.AsignarEmpresa(luis.Id, alfa.Id), "Una empresa admite varias personas");
Verificar(agenda.ListarPersonasDeEmpresa(alfa.Id).Select(p => p.Id).SequenceEqual([2, 1]), "Plantilla ordenada por apellidos");
Verificar(!agenda.DesactivarEmpresa(alfa.Id) && alfa.Estado, "Baja de empresa relacionada bloqueada");
Verificar(!agenda.AsignarEmpresa(999, alfa.Id) && !agenda.AsignarEmpresa(ana.Id, 999), "Asignación exige ambos Id existentes");

Verificar(agenda.AsignarEmpresa(ana.Id, beta.Id), "Cambio de empresa permitido");
Verificar(agenda.ListarPersonasDeEmpresa(alfa.Id).Single().Id == luis.Id
    && agenda.ListarPersonasDeEmpresa(beta.Id).Single().Id == ana.Id,
    "Cambio mantiene una única empresa por persona");
Verificar(agenda.ModificarPersona(
    ana.Id,
    new DatosPersona("Ana María", "Zamora", "ana.nueva@consultora.es", "634567890")),
    "Persona modificada");
Verificar(ana.IdEmpresa == beta.Id && ana.Nombre == "Ana María", "Modificar persona conserva su relación");
Verificar(agenda.ModificarEmpresa(
    beta.Id,
    new DatosEmpresa("Beta Nueva", "B87654321", "923456789", "beta@consultora.es", "Calle Tres")),
    "Empresa modificada");
Verificar(agenda.ObtenerEmpresaDePersona(ana.Id)?.NombreComercial == "Beta Nueva", "Consulta refleja empresa modificada");

Verificar(agenda.DesvincularPersona(ana.Id), "Persona desvinculada");
Verificar(ana.IdEmpresa is null && ana.Correo == "ana.nueva@consultora.es", "Desvinculación conserva datos personales");
Verificar(!agenda.DesvincularPersona(ana.Id), "Doble desvinculación rechazada");
Verificar(agenda.DesactivarEmpresa(beta.Id) && !beta.Estado, "Desvincular desbloquea la baja lógica");
Verificar(agenda.ListarEmpresas().Count == 2, "Empresas inactivas permanecen en la colección");

Verificar(agenda.DarDeBajaPersona(luis.Id), "Baja de persona existente");
Verificar(agenda.BuscarPersonaPorId(luis.Id) is null, "Persona eliminada deja de encontrarse");
Verificar(!agenda.DarDeBajaPersona(999), "Baja de persona inexistente controlada");

Verificar(InputValidator.EsCorreoValido("nombre@empresa.es")
    && !InputValidator.EsCorreoValido("nombre@empresa")
    && !InputValidator.EsCorreoValido("nombre.@empresa.es"),
    "Validación de correo");
Verificar(InputValidator.EsTelefonoValido("612345678")
    && !InputValidator.EsTelefonoValido("61234A678")
    && !InputValidator.EsTelefonoValido("123"),
    "Validación de teléfono");

Console.WriteLine($"Total: {comprobaciones} comprobaciones superadas.");
