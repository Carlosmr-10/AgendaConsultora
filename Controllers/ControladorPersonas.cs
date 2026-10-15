using AgendaConsultora.Views;
using AgendaConsultora.Models;
using AgendaConsultora.Models.Services;

namespace AgendaConsultora.Controllers;

internal sealed class ControladorPersonas
{
    private readonly GestorPersonas Gestor;
    private readonly VistaPersonas vista;
    private readonly GestorRelaciones relaciones;
    private readonly VistaRelaciones vistaRelaciones;
    public ControladorPersonas(GestorPersonas gestor, VistaPersonas vista,
        GestorRelaciones relaciones, VistaRelaciones vistaRelaciones)
    {
        Gestor = gestor;
        this.vista = vista;
        this.relaciones = relaciones;
        this.vistaRelaciones = vistaRelaciones;
    }

    public void Ejecutar()
    {
        while (true)
        {
            switch (vista.LeerOpcion())
            {
                case 1: Alta(); break;
                case 2: MostrarResultados(Gestor.Listar()); break;
                case 3: Buscar(); break;
                case 4: Modificar(); break;
                case 5: Baja(); break;
                case 6: AsignarEmpresa(); break;
                case 7: DesvincularEmpresa(); break;
                case 0: return;
            }
        }
    }

    private void Alta()
    {
        var datos = vista.LeerDatos();
        Persona persona = Gestor.Alta(datos.Nombre, datos.Apellidos, datos.Correo, datos.Telefono);
        vista.AltaRealizada();
        MostrarPersona(persona);
    }

    private void Buscar()
    {
        if (vista.LeerTipoBusqueda() == 1)
        {
            Persona? persona = Localizar();
            if (persona is not null)
                MostrarPersona(persona);
        }
        else
        {
            string texto = vista.LeerBusqueda();
            MostrarResultados(Gestor.BuscarPorNombre(texto), true);
        }
    }

    private Persona? Localizar()
    {
        int id = vista.LeerId();
        Persona? persona = Gestor.BuscarPorId(id);
        if (persona is null)
            vista.NoExiste();
        return persona;
    }

    private void Modificar()
    {
        Persona? persona = Localizar();
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
        Gestor.Modificar(persona.Id, datos.Nombre, datos.Apellidos, datos.Correo, datos.Telefono);
        vista.ModificacionRealizada();
        MostrarPersona(Gestor.BuscarPorId(persona.Id)!);
    }

    private void Baja()
    {
        Persona? persona = Localizar();
        if (persona is null)
            return;
        MostrarPersona(persona);
        if (!vista.ConfirmarBaja())
        {
            vista.BajaCancelada();
            return;
        }
        Gestor.Baja(persona.Id);
        vista.BajaRealizada();
    }


    private void MostrarPersona(Persona persona) =>
        vista.MostrarPersona(persona, relaciones.EmpresaDePersona(persona.Id));

    private void MostrarResultados(IReadOnlyList<Persona> personas, bool busqueda = false)
    {
        if (personas.Count == 0) vista.SinResultados(busqueda);
        foreach (Persona persona in personas) MostrarPersona(persona);
    }

    private void AsignarEmpresa()
    {
        Persona? persona = Localizar();
        if (persona is null) return;
        MostrarPersona(persona);
        int empresaId = vistaRelaciones.LeerEmpresaId();
        Empresa? empresa = relaciones.BuscarEmpresa(empresaId);
        if (empresa is null) { vistaRelaciones.EmpresaInexistente(); return; }
        if (!empresa.Estado) { vistaRelaciones.EmpresaInactiva(); return; }
        if (persona.EmpresaId == empresaId) { vistaRelaciones.YaAsignada(); return; }
        vistaRelaciones.MostrarDestino(empresa);
        if (!vistaRelaciones.ConfirmarAsignacion()) { vistaRelaciones.Cancelada(); return; }
        vistaRelaciones.ResultadoAsignacion(relaciones.Asignar(persona.Id, empresaId));
        MostrarPersona(persona);
    }

    private void DesvincularEmpresa()
    {
        Persona? persona = Localizar();
        if (persona is null) return;
        MostrarPersona(persona);
        if (persona.EmpresaId is null) { vistaRelaciones.SinEmpresa(); return; }
        if (!vistaRelaciones.ConfirmarDesvinculacion()) { vistaRelaciones.Cancelada(); return; }
        vistaRelaciones.ResultadoDesvinculacion(relaciones.Desvincular(persona.Id));
        MostrarPersona(persona);
    }
}
