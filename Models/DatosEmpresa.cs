namespace AgendaConsultora.Models;

public sealed record DatosEmpresa(
    string Nombre,
    string Cif,
    string Telefono,
    string Correo,
    string Direccion);
