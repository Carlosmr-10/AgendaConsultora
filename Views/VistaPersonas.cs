using AgendaConsultora.Models.Validation;
using AgendaConsultora.Models;

namespace AgendaConsultora.Views;

internal sealed class VistaPersonas
{
    public (string Nombre, string Apellidos, string Correo, string Telefono) LeerDatos()
    {
        Console.WriteLine("Todos los campos son obligatorios.");
        string nombre = Entrada.Texto("Nombre: ", Validador.TextoObligatorio, "El nombre no puede estar vacío.");
        string apellidos = Entrada.Texto("Apellidos: ", Validador.TextoObligatorio, "Los apellidos no pueden estar vacíos.");
        string correo = Entrada.Texto("Correo: ", Validador.CorreoValido, "Correo no válido. Ejemplo: ana@consultora.es");
        string telefono = Entrada.Texto("Teléfono (9 dígitos sin separadores): ", Validador.TelefonoValido,
            "Teléfono no válido. Introduce exactamente 9 dígitos, sin letras ni separadores.");
        return (nombre, apellidos, correo, telefono);
    }

    public void MostrarPersona(Persona persona, Empresa? empresa)
    {
        Console.WriteLine($"Id: {persona.Id}\nNombre: {persona.Nombre} {persona.Apellidos}");
        Console.WriteLine($"Correo: {persona.Correo}\nTeléfono: {persona.Telefono}");
        Console.WriteLine(empresa is null ? "Empresa: Sin empresa asignada"
            : $"Empresa: {empresa.NombreComercial} (Id: {empresa.Id})");
        Console.WriteLine("----------------------------------------");
    }

    public void SinResultados(bool busqueda) => Console.WriteLine(busqueda
        ? "No se encontraron coincidencias." : "No hay personas registradas.");

    public void AltaRealizada() => Console.WriteLine("Alta realizada correctamente.");

    public void NoExiste() => Console.WriteLine("No existe una persona con ese Id.");

    public void DatosActuales() => Console.WriteLine("Datos actuales:");

    public void ModificacionCancelada() => Console.WriteLine("Modificación cancelada.");

    public void ModificacionRealizada() => Console.WriteLine("Modificación realizada correctamente.");

    public void BajaCancelada() => Console.WriteLine("Baja cancelada.");

    public void BajaRealizada() => Console.WriteLine("Baja realizada correctamente.");

    public int LeerOpcion()
    {
        Console.WriteLine("\n=== PERSONAS ===");
        Console.WriteLine("1. Dar de alta una persona\n2. Listar personas\n3. Buscar persona");
        Console.WriteLine("4. Modificar persona\n5. Dar de baja una persona\n6. Asignar o cambiar empresa\n7. Desvincular empresa\n0. Volver al menú principal");
        return Entrada.Numero("Selecciona una opción: ", 0, 7);
    }

    public int LeerTipoBusqueda()
    {
        Console.WriteLine("1. Buscar por Id\n2. Buscar por nombre y apellidos");
        return Entrada.Numero("Tipo de búsqueda: ", 1, 2);
    }

    public int LeerId() => Entrada.Numero("Id de la persona: ", 1, int.MaxValue);

    public string LeerBusqueda() => Entrada.Texto("Nombre o apellidos (admite partes): ",
                Validador.TextoObligatorio, "Introduce un texto de búsqueda.");

    public bool ConfirmarModificacion() => Entrada.Confirmar("¿Guardar los cambios?");

    public bool ConfirmarBaja() => Entrada.Confirmar("¿Eliminar esta persona?");

    public void SolicitarNuevosDatos() => Console.WriteLine("Introduce todos los datos nuevos, repitiendo los que quieras conservar.");

    public void MostrarPropuesta(int id, (string Nombre, string Apellidos, string Correo, string Telefono) datos) => Console.WriteLine($"Propuesta para Id {id}: {datos.Nombre} {datos.Apellidos} | {datos.Correo} | {datos.Telefono}");
}
