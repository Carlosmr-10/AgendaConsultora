using System.Globalization;
using AgendaProfesional.Models;
using AgendaProfesional.Utils;

namespace AgendaProfesional.Services;

public sealed class AgendaService
{
    private readonly List<Persona> personas = new();
    private readonly List<Empresa> empresas = new();
    private int siguienteIdPersona = 1;
    private int siguienteIdEmpresa = 1;

    private static readonly StringComparer OrdenEspanol =
        StringComparer.Create(CultureInfo.GetCultureInfo("es-ES"), true);

    public Persona DarDeAltaPersona(DatosPersona datos)
    {
        DatosPersona datosNormalizados = Normalizar(datos);
        Validar(datosNormalizados);

        var persona = new Persona(
            siguienteIdPersona,
            datosNormalizados.Nombre,
            datosNormalizados.Apellidos,
            datosNormalizados.Correo,
            datosNormalizados.Telefono);

        siguienteIdPersona = checked(siguienteIdPersona + 1);
        personas.Add(persona);
        return persona;
    }

    public IReadOnlyList<Persona> ListarPersonas() => OrdenarPersonas(personas);

    public Persona? BuscarPersonaPorId(int id) => personas.Find(persona => persona.Id == id);

    public IReadOnlyList<Persona> BuscarPersonas(string texto)
    {
        string[] palabras = texto.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (palabras.Length == 0) return Array.Empty<Persona>();

        return OrdenarPersonas(personas.Where(persona =>
            CoincideConTodasLasPalabras(persona, palabras)));
    }

    public bool ModificarPersona(int id, DatosPersona datos)
    {
        Persona? persona = BuscarPersonaPorId(id);
        if (persona is null) return false;

        DatosPersona datosNormalizados = Normalizar(datos);
        Validar(datosNormalizados);
        AplicarDatos(persona, datosNormalizados);
        return true;
    }

    public bool DarDeBajaPersona(int id)
    {
        Persona? persona = BuscarPersonaPorId(id);
        return persona is not null && personas.Remove(persona);
    }

    public Empresa DarDeAltaEmpresa(DatosEmpresa datos)
    {
        DatosEmpresa datosNormalizados = Normalizar(datos);
        Validar(datosNormalizados);

        var empresa = new Empresa(
            siguienteIdEmpresa,
            datosNormalizados.NombreComercial,
            datosNormalizados.Cif,
            datosNormalizados.Telefono,
            datosNormalizados.Correo,
            datosNormalizados.Direccion);

        siguienteIdEmpresa = checked(siguienteIdEmpresa + 1);
        empresas.Add(empresa);
        return empresa;
    }

    public IReadOnlyList<Empresa> ListarEmpresas() => empresas
        .OrderBy(empresa => empresa.NombreComercial, OrdenEspanol)
        .ThenBy(empresa => empresa.Id)
        .ToArray();

    public Empresa? BuscarEmpresaPorId(int id) => empresas.Find(empresa => empresa.Id == id);

    public IReadOnlyList<Empresa> BuscarEmpresas(string texto)
    {
        string consulta = texto.Trim();
        return ListarEmpresas().Where(empresa =>
            empresa.NombreComercial.Contains(consulta, StringComparison.OrdinalIgnoreCase)
            || empresa.Cif.Contains(consulta, StringComparison.OrdinalIgnoreCase)).ToArray();
    }

    public bool ModificarEmpresa(int id, DatosEmpresa datos)
    {
        Empresa? empresa = BuscarEmpresaPorId(id);
        if (empresa is null) return false;

        DatosEmpresa datosNormalizados = Normalizar(datos);
        Validar(datosNormalizados);
        AplicarDatos(empresa, datosNormalizados);
        return true;
    }

    public bool PuedeDesactivarEmpresa(int id)
    {
        Empresa? empresa = BuscarEmpresaPorId(id);
        return empresa is { Estado: true } && !TienePersonasAsociadas(id);
    }

    public bool DesactivarEmpresa(int id)
    {
        Empresa? empresa = BuscarEmpresaPorId(id);
        if (empresa is null || !PuedeDesactivarEmpresa(id)) return false;

        empresa.Estado = false;
        return true;
    }

    public bool ActivarEmpresa(int id)
    {
        Empresa? empresa = BuscarEmpresaPorId(id);
        if (empresa is null || empresa.Estado) return false;

        empresa.Estado = true;
        return true;
    }

    public bool AsignarEmpresa(int idPersona, int idEmpresa)
    {
        Persona? persona = BuscarPersonaPorId(idPersona);
        Empresa? empresa = BuscarEmpresaPorId(idEmpresa);
        if (persona is null || empresa is not { Estado: true }) return false;

        persona.IdEmpresa = empresa.Id;
        return true;
    }

    public bool DesvincularPersona(int idPersona)
    {
        Persona? persona = BuscarPersonaPorId(idPersona);
        if (persona?.IdEmpresa is null) return false;

        persona.IdEmpresa = null;
        return true;
    }

    public Empresa? ObtenerEmpresaDePersona(int idPersona)
    {
        Persona? persona = BuscarPersonaPorId(idPersona);
        return persona?.IdEmpresa is int idEmpresa ? BuscarEmpresaPorId(idEmpresa) : null;
    }

    public IReadOnlyList<Persona> ListarPersonasDeEmpresa(int idEmpresa) =>
        OrdenarPersonas(personas.Where(persona => persona.IdEmpresa == idEmpresa));

    private bool TienePersonasAsociadas(int idEmpresa) =>
        personas.Any(persona => persona.IdEmpresa == idEmpresa);

    private static Persona[] OrdenarPersonas(IEnumerable<Persona> resultado) => resultado
        .OrderBy(persona => persona.Apellidos, OrdenEspanol)
        .ThenBy(persona => persona.Nombre, OrdenEspanol)
        .ThenBy(persona => persona.Id)
        .ToArray();

    private static bool CoincideConTodasLasPalabras(
        Persona persona,
        IEnumerable<string> palabras)
    {
        string nombreCompleto = $"{persona.Nombre} {persona.Apellidos}";
        return palabras.All(palabra =>
            nombreCompleto.Contains(palabra, StringComparison.OrdinalIgnoreCase));
    }

    private static DatosPersona Normalizar(DatosPersona datos) => new(
        datos.Nombre.Trim(),
        datos.Apellidos.Trim(),
        datos.Correo.Trim(),
        datos.Telefono.Trim());

    private static DatosEmpresa Normalizar(DatosEmpresa datos) => new(
        datos.NombreComercial.Trim(),
        datos.Cif.Trim(),
        datos.Telefono.Trim(),
        datos.Correo.Trim(),
        datos.Direccion.Trim());

    private static void Validar(DatosPersona datos)
    {
        if (!InputValidator.EsTextoObligatorio(datos.Nombre)
            || !InputValidator.EsTextoObligatorio(datos.Apellidos)
            || !InputValidator.EsCorreoValido(datos.Correo)
            || !InputValidator.EsTelefonoValido(datos.Telefono))
            throw new ArgumentException("Los datos de la persona no son válidos.");
    }

    private static void Validar(DatosEmpresa datos)
    {
        if (!InputValidator.EsTextoObligatorio(datos.NombreComercial)
            || !InputValidator.EsTextoObligatorio(datos.Cif)
            || !InputValidator.EsTextoObligatorio(datos.Direccion)
            || !InputValidator.EsCorreoValido(datos.Correo)
            || !InputValidator.EsTelefonoValido(datos.Telefono))
            throw new ArgumentException("Los datos de la empresa no son válidos.");
    }

    private static void AplicarDatos(Persona persona, DatosPersona datos)
    {
        persona.Nombre = datos.Nombre;
        persona.Apellidos = datos.Apellidos;
        persona.Correo = datos.Correo;
        persona.Telefono = datos.Telefono;
    }

    private static void AplicarDatos(Empresa empresa, DatosEmpresa datos)
    {
        empresa.NombreComercial = datos.NombreComercial;
        empresa.Cif = datos.Cif;
        empresa.Telefono = datos.Telefono;
        empresa.Correo = datos.Correo;
        empresa.Direccion = datos.Direccion;
    }
}
