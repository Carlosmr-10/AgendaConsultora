namespace AgendaConsultora.Modelos;

public sealed class Persona
{
    public int Id { get; }
    public string Nombre { get; internal set; }
    public string Apellidos { get; internal set; }
    public string Correo { get; internal set; }
    public string Telefono { get; internal set; }
    public int? IdEmpresa { get; internal set; }

    internal Persona(int id, string nombre, string apellidos, string correo, string telefono)
    {
        Id = id;
        Nombre = nombre;
        Apellidos = apellidos;
        Correo = correo;
        Telefono = telefono;
    }
}
