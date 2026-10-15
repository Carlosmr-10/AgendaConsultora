using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AgendaConsultora.Helpers;
using AgendaConsultora.Models;

namespace AgendaConsultora.Services;

public sealed class GestorPersonas
{
    private readonly List<Persona> personas = new();
    private int siguienteId = 1;
    private static readonly StringComparer Orden =
        StringComparer.Create(CultureInfo.GetCultureInfo("es-ES"), true);

    public Persona Alta(string nombre, string apellidos, string correo, string telefono)
    {
        Persona persona = CrearPersona(siguienteId, nombre, apellidos, correo, telefono);
        siguienteId = checked(siguienteId + 1);
        personas.Add(persona);
        return persona;
    }

    public IReadOnlyList<Persona> Listar() => Ordenar(personas);
    public Persona? BuscarPorId(int id) => personas.Find(p => p.Id == id);

    public IReadOnlyList<Persona> BuscarPorNombre(string texto)
    {
        string[] palabras = texto.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (palabras.Length == 0)
            return Array.Empty<Persona>();

        return Ordenar(personas.Where(p => palabras.All(palabra =>
            $"{p.Nombre} {p.Apellidos}".Contains(palabra, StringComparison.OrdinalIgnoreCase))));
    }

    public bool Modificar(int id, string nombre, string apellidos, string correo, string telefono)
    {
        int indice = personas.FindIndex(p => p.Id == id);
        if (indice < 0)
            return false;
        personas[indice] = CrearPersona(id, nombre, apellidos, correo, telefono);
        return true;
    }

    public bool Baja(int id) => personas.RemoveAll(p => p.Id == id) > 0;

    private static Persona[] Ordenar(IEnumerable<Persona> resultado) => resultado
        .OrderBy(p => p.Apellidos, Orden).ThenBy(p => p.Nombre, Orden).ThenBy(p => p.Id).ToArray();

    private static Persona CrearPersona(int id, string nombre, string apellidos,
        string correo, string telefono)
    {
        nombre = nombre.Trim();
        apellidos = apellidos.Trim();
        correo = correo.Trim();
        telefono = telefono.Trim();
        if (!Validador.TextoObligatorio(nombre) || !Validador.TextoObligatorio(apellidos)
            || !Validador.CorreoValido(correo) || !Validador.TelefonoValido(telefono))
            throw new ArgumentException("Los datos de la persona no son válidos.");
        return new Persona(id, nombre, apellidos, correo, telefono);
    }
}
