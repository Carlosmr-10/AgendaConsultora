using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AgendaConsultora.Models.Validation;
using AgendaConsultora.Models;

namespace AgendaConsultora.Models.Services;

public sealed class GestorPersonas
{
    private readonly List<Persona> personas = new();
    private int siguienteId = 1;
    private static readonly StringComparer Orden =
        StringComparer.Create(CultureInfo.GetCultureInfo("es-ES"), true);

    public Persona Alta(string nombre, string apellidos, string correo, string telefono)
    {
        DatosPersona datos = NormalizarDatos(nombre, apellidos, correo, telefono);
        Persona persona = CrearPersonaValidada(siguienteId, datos);
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

        return Ordenar(personas.Where(persona => CoincideConTodasLasPalabras(persona, palabras)));
    }

    public bool Modificar(int id, string nombre, string apellidos, string correo, string telefono)
    {
        int indice = personas.FindIndex(p => p.Id == id);
        if (indice < 0)
            return false;
        DatosPersona datos = NormalizarDatos(nombre, apellidos, correo, telefono);
        Persona propuesta = CrearPersonaValidada(id, datos);
        propuesta.EmpresaId = personas[indice].EmpresaId;
        personas[indice] = propuesta;
        return true;
    }

    public bool TieneRelacionConEmpresa(int empresaId) => personas.Any(p => p.EmpresaId == empresaId);

    // La asignación por identificadores se coordina desde GestorRelaciones.
    public bool VincularEmpresa(int personaId, Empresa empresa)
    {
        Persona? persona = BuscarPorId(personaId);
        if (persona is null || !empresa.Estado)
            return false;
        persona.EmpresaId = empresa.Id;
        return true;
    }

    public IReadOnlyList<Persona> ListarPorEmpresa(int empresaId) =>
        Ordenar(personas.Where(p => p.EmpresaId == empresaId));

    public bool DesvincularEmpresa(int personaId)
    {
        Persona? persona = BuscarPorId(personaId);
        if (persona is null || persona.EmpresaId is null) return false;
        persona.EmpresaId = null;
        return true;
    }

    public bool Baja(int id) => personas.RemoveAll(p => p.Id == id) > 0;

    private static Persona[] Ordenar(IEnumerable<Persona> resultado) => resultado
        .OrderBy(p => p.Apellidos, Orden).ThenBy(p => p.Nombre, Orden).ThenBy(p => p.Id).ToArray();

    private static bool CoincideConTodasLasPalabras(Persona persona, IEnumerable<string> palabras)
    {
        string nombreCompleto = $"{persona.Nombre} {persona.Apellidos}";
        return palabras.All(palabra =>
            nombreCompleto.Contains(palabra, StringComparison.OrdinalIgnoreCase));
    }

    private static DatosPersona NormalizarDatos(
        string nombre,
        string apellidos,
        string correo,
        string telefono) => new(
            nombre.Trim(),
            apellidos.Trim(),
            correo.Trim(),
            telefono.Trim());

    private static Persona CrearPersonaValidada(int id, DatosPersona datos)
    {
        if (!Validador.TextoObligatorio(datos.Nombre)
            || !Validador.TextoObligatorio(datos.Apellidos)
            || !Validador.CorreoValido(datos.Correo)
            || !Validador.TelefonoValido(datos.Telefono))
            throw new ArgumentException("Los datos de la persona no son válidos.");

        return new Persona(id, datos.Nombre, datos.Apellidos, datos.Correo, datos.Telefono);
    }
}
