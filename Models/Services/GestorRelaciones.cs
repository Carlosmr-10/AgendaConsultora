using AgendaConsultora.Models;

namespace AgendaConsultora.Models.Services;

public sealed class GestorRelaciones
{
    private readonly GestorPersonas personas;
    private readonly GestorEmpresas empresas;

    public GestorRelaciones(GestorPersonas personas, GestorEmpresas empresas)
    {
        this.personas = personas;
        this.empresas = empresas;
    }

    public Empresa? BuscarEmpresa(int id) => empresas.BuscarPorId(id);

    public Empresa? EmpresaDePersona(int personaId) =>
        personas.BuscarPorId(personaId)?.EmpresaId is int id ? empresas.BuscarPorId(id) : null;

    public bool Asignar(int personaId, int empresaId)
    {
        Empresa? empresa = empresas.BuscarPorId(empresaId);
        return empresa is { Estado: true } && personas.VincularEmpresa(personaId, empresa);
    }

    public bool Desvincular(int personaId) => personas.DesvincularEmpresa(personaId);
}
