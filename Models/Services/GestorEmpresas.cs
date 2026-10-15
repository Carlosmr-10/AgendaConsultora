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
        DatosEmpresa datos = NormalizarDatos(nombre, cif, telefono, correo, direccion);
        Empresa empresa = CrearEmpresaValidada(siguienteId, datos);
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

        DatosEmpresa datos = NormalizarDatos(nombre, cif, telefono, correo, direccion);
        ValidarDatos(datos);
        AplicarDatos(empresa, datos);
        return true;
    }

    public bool PuedeDesactivar(int id) => PuedeDesactivar(BuscarPorId(id));

    public bool Desactivar(int id)
    {
        Empresa? empresa = BuscarPorId(id);
        if (!PuedeDesactivar(empresa)) return false;

        empresa!.Estado = false;
        return true;
    }

    public bool Activar(int id)
    {
        Empresa? empresa = BuscarPorId(id);
        if (empresa is null || empresa.Estado) return false;
        empresa.Estado = true;
        return true;
    }

    private bool PuedeDesactivar(Empresa? empresa) =>
        empresa is { Estado: true } && !personas.TieneRelacionConEmpresa(empresa.Id);

    private static DatosEmpresa NormalizarDatos(
        string nombre,
        string cif,
        string telefono,
        string correo,
        string direccion) => new(
            nombre.Trim(),
            cif.Trim(),
            telefono.Trim(),
            correo.Trim(),
            direccion.Trim());

    private static Empresa CrearEmpresaValidada(int id, DatosEmpresa datos)
    {
        ValidarDatos(datos);
        return new Empresa(
            id, datos.Nombre, datos.Cif, datos.Telefono, datos.Correo, datos.Direccion);
    }

    private static void ValidarDatos(DatosEmpresa datos)
    {
        if (!Validador.TextoObligatorio(datos.Nombre)
            || !Validador.TextoObligatorio(datos.Cif)
            || !Validador.TextoObligatorio(datos.Direccion)
            || !Validador.TelefonoValido(datos.Telefono)
            || !Validador.CorreoValido(datos.Correo))
            throw new ArgumentException("Los datos de la empresa no son válidos.");
    }

    private static void AplicarDatos(Empresa empresa, DatosEmpresa datos)
    {
        empresa.NombreComercial = datos.Nombre;
        empresa.Cif = datos.Cif;
        empresa.Telefono = datos.Telefono;
        empresa.Correo = datos.Correo;
        empresa.Direccion = datos.Direccion;
    }
}
