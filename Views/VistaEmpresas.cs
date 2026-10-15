using AgendaConsultora.Models.Validation;
using AgendaConsultora.Models;

namespace AgendaConsultora.Views;

internal sealed class VistaEmpresas
{
    public DatosEmpresa LeerDatos()
    {
        Console.WriteLine("Todos los campos son obligatorios.");
        string nombre = Entrada.Texto("Nombre comercial: ", Validador.TextoObligatorio, "El nombre comercial es obligatorio.");
        string cif = Entrada.Texto("CIF: ", Validador.TextoObligatorio, "El CIF no puede estar vacío.");
        string telefono = Entrada.Texto("Teléfono (9 dígitos): ", Validador.TelefonoValido, "Teléfono no válido.");
        string correo = Entrada.Texto("Correo: ", Validador.CorreoValido, "Correo no válido.");
        string direccion = Entrada.Texto("Dirección: ", Validador.TextoObligatorio, "La dirección es obligatoria.");
        return new DatosEmpresa(nombre, cif, telefono, correo, direccion);
    }

    public void MostrarEmpresa(Empresa empresa, bool puedeDesactivar)
    {
        Console.WriteLine($"Id: {empresa.Id}\nNombre comercial: {empresa.NombreComercial}\nCIF: {empresa.Cif}");
        Console.WriteLine($"Teléfono: {empresa.Telefono}\nCorreo: {empresa.Correo}\nDirección: {empresa.Direccion}");
        Console.WriteLine($"Estado: {(empresa.Estado ? "Activa" : "Inactiva")}");
        Console.WriteLine($"Baja disponible: {(puedeDesactivar ? "Sí" : "No")}");
        Console.WriteLine("----------------------------------------");
    }

    public void SinResultados() => Console.WriteLine("No hay empresas que mostrar.");

    public void AltaRealizada() => Console.WriteLine("Alta de empresa realizada correctamente.");

    public void NoExiste() => Console.WriteLine("No existe una empresa con ese Id.");

    public void DatosActuales() => Console.WriteLine("Datos actuales:");

    public void ModificacionCancelada() => Console.WriteLine("Modificación cancelada.");

    public void ModificacionRealizada() => Console.WriteLine("Empresa modificada correctamente.");

    public void BajaBloqueada() => Console.WriteLine("Baja bloqueada: la empresa tiene personas asociadas.");

    public void CambioCancelado() => Console.WriteLine("Cambio de estado cancelado.");

    public void EstadoSinCambios(bool nuevoEstado) => Console.WriteLine(nuevoEstado ? "La empresa ya está activa." : "La empresa ya está inactiva.");

    public void ResultadoEstado(bool realizado) => Console.WriteLine(realizado ? "Estado actualizado correctamente." : "No se pudo cambiar el estado.");

    public int LeerOpcion()
    {
        Console.WriteLine("\n=== EMPRESAS ===");
        Console.WriteLine("1. Alta\n2. Listar (activas e inactivas)\n3. Buscar\n4. Modificar");
        Console.WriteLine("5. Dar de baja (solo sin relaciones)\n6. Reactivar\n7. Ver personas de una empresa\n0. Volver al menú principal");
        return Entrada.Numero("Selecciona una opción: ", 0, 7);
    }

    public int LeerTipoBusqueda()
    {
        Console.WriteLine("1. Por Id\n2. Por nombre comercial o CIF");
        return Entrada.Numero("Tipo de búsqueda: ", 1, 2);
    }

    public int LeerId() => Entrada.Numero("Id de la empresa: ", 1, int.MaxValue);

    public string LeerBusqueda() => Entrada.Texto("Texto de búsqueda: ", Validador.TextoObligatorio, "Introduce un texto.");

    public bool ConfirmarModificacion() => Entrada.Confirmar("¿Guardar los cambios?");

    public bool ConfirmarEstado(bool nuevoEstado) => Entrada.Confirmar(nuevoEstado ? "¿Reactivar esta empresa?" : "¿Desactivar esta empresa sin borrar su registro?");

    public void SolicitarNuevosDatos() => Console.WriteLine("Introduce los datos nuevos, repitiendo los que quieras conservar.");

    public void MostrarPropuesta(DatosEmpresa datos) =>
        Console.WriteLine($"Propuesta: {datos.Nombre} | {datos.Cif} | {datos.Telefono} | {datos.Correo} | {datos.Direccion}");

    public void MostrarEncabezadoPlantilla(Empresa empresa) =>
        Console.WriteLine($"Plantilla de {empresa.NombreComercial} (Id: {empresa.Id})");

    public void MostrarPlantillaVacia() =>
        Console.WriteLine("Esta empresa no tiene personas asociadas.");
}
