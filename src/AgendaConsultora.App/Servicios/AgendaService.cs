using System.Globalization;
using AgendaConsultora.Modelos;
using AgendaConsultora.Validacion;

namespace AgendaConsultora.Servicios;

public sealed class AgendaService
{
    private readonly List<Persona> personas = new();
    private readonly List<Empresa> empresas = new();
    private int siguienteIdPersona = 1;
    private int siguienteIdEmpresa = 1;

    private static readonly StringComparer OrdenEspanol =
        StringComparer.Create(CultureInfo.GetCultureInfo("es-ES"), true);

    public Persona GuardarPersona(int? id, DatosPersona datos)
    {
        DatosPersona normalizados = Normalizar(datos);
        Validar(normalizados);

        if (id is null)
        {
            var nueva = new Persona(
                siguienteIdPersona++,
                normalizados.Nombre,
                normalizados.Apellidos,
                normalizados.Correo,
                normalizados.Telefono);
            personas.Add(nueva);
            return nueva;
        }

        Persona persona = BuscarPersonaPorId(id.Value)
            ?? throw new InvalidOperationException("La persona ya no existe.");
        persona.Nombre = normalizados.Nombre;
        persona.Apellidos = normalizados.Apellidos;
        persona.Correo = normalizados.Correo;
        persona.Telefono = normalizados.Telefono;
        return persona;
    }

    public bool EliminarPersona(int id)
    {
        Persona? persona = BuscarPersonaPorId(id);
        return persona is not null && personas.Remove(persona);
    }

    public IReadOnlyList<Persona> ListarPersonas(string? filtro = null)
    {
        IEnumerable<Persona> consulta = personas;
        if (!string.IsNullOrWhiteSpace(filtro))
        {
            string[] palabras = filtro.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            consulta = consulta.Where(persona => palabras.All(palabra =>
                $"{persona.Nombre} {persona.Apellidos}".Contains(
                    palabra,
                    StringComparison.OrdinalIgnoreCase)
                || persona.Correo.Contains(palabra, StringComparison.OrdinalIgnoreCase)));
        }

        return consulta
            .OrderBy(persona => persona.Apellidos, OrdenEspanol)
            .ThenBy(persona => persona.Nombre, OrdenEspanol)
            .ThenBy(persona => persona.Id)
            .ToArray();
    }

    public Persona? BuscarPersonaPorId(int id) => personas.Find(persona => persona.Id == id);

    public Empresa GuardarEmpresa(int? id, DatosEmpresa datos)
    {
        DatosEmpresa normalizados = Normalizar(datos);
        Validar(normalizados);

        if (id is null)
        {
            var nueva = new Empresa(siguienteIdEmpresa++, normalizados);
            empresas.Add(nueva);
            return nueva;
        }

        Empresa empresa = BuscarEmpresaPorId(id.Value)
            ?? throw new InvalidOperationException("La empresa ya no existe.");
        empresa.NombreComercial = normalizados.NombreComercial;
        empresa.Cif = normalizados.Cif;
        empresa.Telefono = normalizados.Telefono;
        empresa.Correo = normalizados.Correo;
        empresa.Direccion = normalizados.Direccion;
        return empresa;
    }

    public IReadOnlyList<Empresa> ListarEmpresas(string? filtro = null)
    {
        IEnumerable<Empresa> consulta = empresas;
        if (!string.IsNullOrWhiteSpace(filtro))
        {
            string texto = filtro.Trim();
            consulta = consulta.Where(empresa =>
                empresa.NombreComercial.Contains(texto, StringComparison.OrdinalIgnoreCase)
                || empresa.Cif.Contains(texto, StringComparison.OrdinalIgnoreCase));
        }

        return consulta
            .OrderBy(empresa => empresa.NombreComercial, OrdenEspanol)
            .ThenBy(empresa => empresa.Id)
            .ToArray();
    }

    public Empresa? BuscarEmpresaPorId(int id) => empresas.Find(empresa => empresa.Id == id);

    public bool DesactivarEmpresa(int id)
    {
        Empresa? empresa = BuscarEmpresaPorId(id);
        if (empresa is null || !empresa.Estado || TienePersonasAsociadas(id)) return false;
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

    public IReadOnlyList<Persona> ListarPersonasDeEmpresa(int idEmpresa) =>
        ListarPersonas().Where(persona => persona.IdEmpresa == idEmpresa).ToArray();

    public Empresa? ObtenerEmpresaDePersona(Persona persona) =>
        persona.IdEmpresa is int idEmpresa ? BuscarEmpresaPorId(idEmpresa) : null;

    private bool TienePersonasAsociadas(int idEmpresa) =>
        personas.Any(persona => persona.IdEmpresa == idEmpresa);

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
            || !InputValidator.EsTextoObligatorio(datos.Apellidos))
            throw new ArgumentException("Nombre y apellidos son obligatorios.");
        if (!InputValidator.EsCorreoValido(datos.Correo))
            throw new ArgumentException("Introduce un correo electrónico válido.");
        if (!InputValidator.EsTelefonoValido(datos.Telefono))
            throw new ArgumentException("Introduce un teléfono español válido de 9 dígitos.");
    }

    private static void Validar(DatosEmpresa datos)
    {
        if (!InputValidator.EsTextoObligatorio(datos.NombreComercial)
            || !InputValidator.EsTextoObligatorio(datos.Cif)
            || !InputValidator.EsTextoObligatorio(datos.Direccion))
            throw new ArgumentException("Nombre comercial, CIF y dirección son obligatorios.");
        if (!InputValidator.EsCorreoValido(datos.Correo))
            throw new ArgumentException("Introduce un correo electrónico válido.");
        if (!InputValidator.EsTelefonoValido(datos.Telefono))
            throw new ArgumentException("Introduce un teléfono español válido de 9 dígitos.");
    }
}
