namespace AgendaConsultora.Vistas;

public sealed record PersonaFila(
    int Id,
    string Iniciales,
    string NombreCompleto,
    string Contacto,
    string Empresa,
    string IdTexto);

public sealed record EmpresaFila(
    int Id,
    string Nombre,
    string Contacto,
    string Plantilla,
    string Estado,
    string EstadoFondo,
    string EstadoColor,
    string IdTexto);

public sealed record EmpresaOpcion(int Id, string Nombre);
