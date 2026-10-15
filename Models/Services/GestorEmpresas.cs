using System.Globalization;
using AgendaConsultora.Models.Validation;
using AgendaConsultora.Models;

namespace AgendaConsultora.Models.Services;

public sealed class GestorEmpresas
{
    private readonly List<Empresa> empresas = new();
    private readonly GestorPersonas personas;
    private int siguienteId = 1;

    public GestorEmpresas(GestorPersonas personas) => this.personas = personas;

    public Empresa Alta(string nombre, string cif, string telefono, string correo, string direccion)
    {
        Empresa empresa = Validar(siguienteId, nombre, cif, telefono, correo, direccion);
        siguienteId = checked(siguienteId + 1);
        empresas.Add(empresa);
        return empresa;
    }

    // Se incluyen las inactivas para consultar y recuperar los registros conservados.
    public IReadOnlyList<Empresa> Listar() => empresas
        .OrderBy(e => e.NombreComercial, StringComparer.Create(CultureInfo.GetCultureInfo("es-ES"), true))
        .ThenBy(e => e.Id).ToArray();

    public Empresa? BuscarPorId(int id) => empresas.Find(e => e.Id == id);

    public IReadOnlyList<Empresa> Buscar(string texto) => Listar().Where(e =>
        e.NombreComercial.Contains(texto.Trim(), StringComparison.OrdinalIgnoreCase)
        || e.Cif.Contains(texto.Trim(), StringComparison.OrdinalIgnoreCase)).ToArray();

    public bool Modificar(int id, string nombre, string cif, string telefono, string correo, string direccion)
    {
        Empresa? empresa = BuscarPorId(id);
        if (empresa is null) return false;
        Empresa propuesta = Validar(id, nombre, cif, telefono, correo, direccion);
        empresa.NombreComercial = propuesta.NombreComercial;
        empresa.Cif = propuesta.Cif;
        empresa.Telefono = propuesta.Telefono;
        empresa.Correo = propuesta.Correo;
        empresa.Direccion = propuesta.Direccion;
        return true;
    }

    public bool PuedeDesactivar(int id) => BuscarPorId(id) is { Estado: true }
        && !personas.TieneRelacionConEmpresa(id);

    public bool Desactivar(int id)
    {
        if (!PuedeDesactivar(id)) return false;
        BuscarPorId(id)!.Estado = false;
        return true;
    }

    public bool Activar(int id)
    {
        Empresa? empresa = BuscarPorId(id);
        if (empresa is null || empresa.Estado) return false;
        empresa.Estado = true;
        return true;
    }

    private static Empresa Validar(int id, string nombre, string cif, string telefono, string correo, string direccion)
    {
        nombre = nombre.Trim();
        cif = cif.Trim();
        telefono = telefono.Trim();
        correo = correo.Trim();
        direccion = direccion.Trim();
        if (!Validador.TextoObligatorio(nombre) || !Validador.TextoObligatorio(cif)
            || !Validador.TextoObligatorio(direccion) || !Validador.TelefonoValido(telefono)
            || !Validador.CorreoValido(correo))
            throw new ArgumentException("Los datos de la empresa no son válidos.");
        return new Empresa(id, nombre, cif, telefono, correo, direccion);
    }
}
