namespace AgendaConsultora.Models;

public sealed class Persona
{
    public int Id { get; }
    public string Nombre { get; }
    public string Apellidos { get; }
    public string Correo { get; }
    public string Telefono { get; }

    internal Persona(int id, string nombre, string apellidos, string correo, string telefono)
    {
        Id = id;
        Nombre = nombre;
        Apellidos = apellidos;
        Correo = correo;
        Telefono = telefono;
    }
}
