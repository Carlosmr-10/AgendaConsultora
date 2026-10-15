using AgendaConsultora.Modelos;
using AgendaConsultora.Servicios;
using AgendaConsultora.Validacion;

var agenda = new AgendaService();
int correctas = 0;

void Comprobar(bool condicion, string nombre)
{
    if (!condicion) throw new InvalidOperationException($"FALLO: {nombre}");
    correctas++;
    Console.WriteLine($"OK - {nombre}");
}

void DebeFallar(Action accion, string nombre)
{
    try
    {
        accion();
        throw new InvalidOperationException($"FALLO: {nombre}");
    }
    catch (ArgumentException)
    {
        correctas++;
        Console.WriteLine($"OK - {nombre}");
    }
}

Comprobar(InputValidator.EsCorreoValido("ana@empresa.es"), "Correo válido aceptado");
Comprobar(!InputValidator.EsCorreoValido("correo@"), "Correo inválido rechazado");
Comprobar(InputValidator.EsTelefonoValido("+34 600 123 123"), "Teléfono con prefijo aceptado");
Comprobar(!InputValidator.EsTelefonoValido("60012A123"), "Teléfono con letras rechazado");

Persona zoe = agenda.GuardarPersona(null, new("  Zoe ", " Zeta ", "zoe@empresa.es", "600 100 100"));
Persona ana = agenda.GuardarPersona(null, new("Ana", "Álvarez", "ana@empresa.es", "600-200-200"));
Comprobar(zoe.Id == 1 && ana.Id == 2, "Id de persona autogenerado");
Comprobar(zoe.Nombre == "Zoe", "Datos personales normalizados");
Comprobar(agenda.ListarPersonas()[0].Id == ana.Id, "Personas ordenadas por apellidos");
Comprobar(agenda.ListarPersonas("ÁLVA").Single().Id == ana.Id, "Búsqueda parcial sin distinguir mayúsculas");
DebeFallar(
    () => agenda.GuardarPersona(null, new("", "Apellido", "correo@empresa.es", "600000000")),
    "Nombre obligatorio validado por el servicio");

Empresa beta = agenda.GuardarEmpresa(null, new("Beta SL", "B22222222", "910 222 222", "beta@empresa.es", "Madrid"));
Empresa alfa = agenda.GuardarEmpresa(null, new("Alfa SL", "B11111111", "910 111 111", "alfa@empresa.es", "Sevilla"));
Comprobar(beta.Id == 1 && alfa.Id == 2, "Id de empresa autogenerado");
Comprobar(beta.Estado && alfa.Estado, "Empresas nuevas activas");
Comprobar(agenda.ListarEmpresas()[0].Id == alfa.Id, "Empresas ordenadas por nombre");
Comprobar(agenda.ListarEmpresas("b111").Single().Id == alfa.Id, "Búsqueda de empresa por CIF");
DebeFallar(
    () => agenda.GuardarEmpresa(null, new("", "", "910000000", "x@empresa.es", "Madrid")),
    "Nombre y CIF obligatorios validados");

Comprobar(agenda.AsignarEmpresa(ana.Id, alfa.Id), "Asignación de empresa realizada");
Comprobar(ana.IdEmpresa == alfa.Id, "Persona conserva una única empresa");
Comprobar(agenda.ListarPersonasDeEmpresa(alfa.Id).Single().Id == ana.Id, "Plantilla consultable desde empresa");
Comprobar(!agenda.DesactivarEmpresa(alfa.Id), "Baja bloqueada cuando existen relaciones");
Comprobar(agenda.AsignarEmpresa(ana.Id, beta.Id), "Cambio de empresa realizado");
Comprobar(agenda.ListarPersonasDeEmpresa(alfa.Id).Count == 0, "Cambio elimina la relación anterior");
Comprobar(agenda.DesactivarEmpresa(alfa.Id), "Empresa sin relaciones desactivada");
Comprobar(!alfa.Estado && agenda.BuscarEmpresaPorId(alfa.Id) == alfa, "Baja lógica conserva el registro");
Comprobar(!agenda.AsignarEmpresa(zoe.Id, alfa.Id), "Empresa inactiva no asignable");
Comprobar(agenda.ActivarEmpresa(alfa.Id), "Empresa reactivada");
Comprobar(agenda.DesvincularPersona(ana.Id), "Persona desvinculada");
Comprobar(ana.IdEmpresa is null && ana.Correo == "ana@empresa.es", "Desvinculación conserva sus datos");

agenda.GuardarPersona(ana.Id, new("Ana María", "Álvarez", "ana.nueva@empresa.es", "611111111"));
Comprobar(ana.Nombre == "Ana María" && ana.IdEmpresa is null, "Modificación de persona conservada");
agenda.GuardarEmpresa(beta.Id, new("Beta Norte", "B22222222", "910222222", "norte@empresa.es", "Bilbao"));
Comprobar(beta.NombreComercial == "Beta Norte" && beta.Estado, "Modificación de empresa conserva Estado");
Comprobar(agenda.EliminarPersona(zoe.Id) && agenda.BuscarPersonaPorId(zoe.Id) is null, "Baja de persona realizada");

Console.WriteLine($"Total: {correctas} comprobaciones superadas.");
