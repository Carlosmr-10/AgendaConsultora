namespace AgendaConsultora.Modelos;

public sealed class Empresa
{
    public int Id { get; }
    public string NombreComercial { get; internal set; }
    public string Cif { get; internal set; }
    public string Telefono { get; internal set; }
    public string Correo { get; internal set; }
    public string Direccion { get; internal set; }
    public bool Estado { get; internal set; } = true;

    internal Empresa(int id, DatosEmpresa datos)
    {
        Id = id;
        NombreComercial = datos.NombreComercial;
        Cif = datos.Cif;
        Telefono = datos.Telefono;
        Correo = datos.Correo;
        Direccion = datos.Direccion;
    }
}
