namespace AgendaConsultora.Models;

public sealed class Empresa
{
    public int Id { get; }
    public string NombreComercial { get; internal set; }
    public string Cif { get; internal set; }
    public string Telefono { get; internal set; }
    public string Correo { get; internal set; }
    public string Direccion { get; internal set; }
    public bool Estado { get; internal set; } = true;

    internal Empresa(int id, string nombre, string cif, string telefono, string correo, string direccion)
    {
        Id = id;
        NombreComercial = nombre;
        Cif = cif;
        Telefono = telefono;
        Correo = correo;
        Direccion = direccion;
    }
}
