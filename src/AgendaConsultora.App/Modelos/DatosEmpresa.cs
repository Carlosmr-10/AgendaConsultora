namespace AgendaConsultora.Modelos;

public sealed record DatosEmpresa(
    string NombreComercial,
    string Cif,
    string Telefono,
    string Correo,
    string Direccion);
