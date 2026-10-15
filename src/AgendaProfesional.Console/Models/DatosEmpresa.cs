namespace AgendaProfesional.Models;

public sealed record DatosEmpresa(
    string NombreComercial,
    string Cif,
    string Telefono,
    string Correo,
    string Direccion);
