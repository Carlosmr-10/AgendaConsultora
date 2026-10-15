using AgendaConsultora.Views;
using AgendaConsultora.Models;
using AgendaConsultora.Models.Services;

namespace AgendaConsultora.Controllers;

internal sealed class ControladorPersonas
{
    private readonly GestorPersonas gestor;
    private readonly VistaPersonas vista;
    private readonly GestorRelaciones relaciones;
    private readonly VistaRelaciones vistaRelaciones;
    public ControladorPersonas(GestorPersonas gestor, VistaPersonas vista,
        GestorRelaciones relaciones, VistaRelaciones vistaRelaciones)
    {
        this.gestor = gestor;
        this.vista = vista;
        this.relaciones = relaciones;
        this.vistaRelaciones = vistaRelaciones;
    }

    public void Ejecutar()
    {
        while (!ProcesarOpcion(vista.LeerOpcion()))
        {
        }
    }

    private bool ProcesarOpcion(int opcion)
    {
        switch (opcion)
        {
            case 1: DarDeAlta(); return false;
            case 2: MostrarResultados(gestor.Listar()); return false;
            case 3: Buscar(); return false;
            case 4: Modificar(); return false;
            case 5: DarDeBaja(); return false;
            case 6: AsignarEmpresa(); return false;
            case 7: DesvincularEmpresa(); return false;
            case 0: return true;
            default: return false;
        }
    }

    private void DarDeAlta()
    {
        var datos = vista.LeerDatos();
        Persona persona = gestor.Alta(datos.Nombre, datos.Apellidos, datos.Correo, datos.Telefono);
        vista.AltaRealizada();
        MostrarPersona(persona);
    }

    private void Buscar()
    {
        if (vista.LeerTipoBusqueda() == 1) BuscarPorId();
        else BuscarPorNombre();
    }

    private void BuscarPorId()
    {
        Persona? persona = LocalizarPorId();
        if (persona is not null) MostrarPersona(persona);
    }

    private void BuscarPorNombre()
    {
        MostrarResultados(gestor.BuscarPorNombre(vista.LeerBusqueda()), true);
    }

    private Persona? LocalizarPorId()
    {
        return OperacionesControlador.BuscarPorId(
            vista.LeerId, gestor.BuscarPorId, vista.NoExiste);
    }

    private void Modificar()
    {
        Persona? persona = LocalizarPorId();
        if (persona is null)
            return;
        vista.DatosActuales();
        MostrarPersona(persona);
        vista.SolicitarNuevosDatos();
        var datos = vista.LeerDatos();
        vista.MostrarPropuesta(persona.Id, datos);
        if (!vista.ConfirmarModificacion())
        {
            vista.ModificacionCancelada();
            return;
        }
        gestor.Modificar(persona.Id, datos.Nombre, datos.Apellidos, datos.Correo, datos.Telefono);
        vista.ModificacionRealizada();
        MostrarPersona(gestor.BuscarPorId(persona.Id)!);
    }

    private void DarDeBaja()
    {
        Persona? persona = LocalizarPorId();
        if (persona is null)
            return;
        MostrarPersona(persona);
        if (!vista.ConfirmarBaja())
        {
            vista.BajaCancelada();
            return;
        }
        gestor.Baja(persona.Id);
        vista.BajaRealizada();
    }


    private void MostrarPersona(Persona persona) =>
        vista.MostrarPersona(persona, relaciones.EmpresaDePersona(persona.Id));

    private void MostrarResultados(IReadOnlyList<Persona> personas, bool busqueda = false)
    {
        OperacionesControlador.MostrarColeccion(
            personas,
            () => vista.SinResultados(busqueda),
            MostrarPersona);
    }

    private void AsignarEmpresa()
    {
        Persona? persona = LocalizarPorId();
        if (persona is null) return;
        MostrarPersona(persona);
        Empresa? empresa = LocalizarEmpresaAsignable(persona);
        if (empresa is null) return;
        vistaRelaciones.MostrarDestino(empresa);
        if (!vistaRelaciones.ConfirmarAsignacion()) { vistaRelaciones.Cancelada(); return; }
        vistaRelaciones.ResultadoAsignacion(relaciones.Asignar(persona.Id, empresa.Id));
        MostrarPersona(persona);
    }

    private Empresa? LocalizarEmpresaAsignable(Persona persona)
    {
        int empresaId = vistaRelaciones.LeerEmpresaId();
        Empresa? empresa = relaciones.BuscarEmpresa(empresaId);

        if (empresa is null)
            vistaRelaciones.EmpresaInexistente();
        else if (!empresa.Estado)
            vistaRelaciones.EmpresaInactiva();
        else if (persona.EmpresaId == empresaId)
            vistaRelaciones.YaAsignada();
        else
            return empresa;

        return null;
    }

    private void DesvincularEmpresa()
    {
        Persona? persona = LocalizarPorId();
        if (persona is null) return;
        MostrarPersona(persona);
        if (persona.EmpresaId is null) { vistaRelaciones.SinEmpresa(); return; }
        if (!vistaRelaciones.ConfirmarDesvinculacion()) { vistaRelaciones.Cancelada(); return; }
        vistaRelaciones.ResultadoDesvinculacion(relaciones.Desvincular(persona.Id));
        MostrarPersona(persona);
    }
}
