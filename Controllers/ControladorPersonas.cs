using AgendaConsultora.Views;
using AgendaConsultora.Models;
using AgendaConsultora.Models.Services;

namespace AgendaConsultora.Controllers;

internal sealed class ControladorPersonas
{
    private readonly GestorPersonas Gestor;
    private readonly VistaPersonas vista;
    public ControladorPersonas(GestorPersonas gestor, VistaPersonas vista)
    {
        Gestor = gestor;
        this.vista = vista;
    }

    public void Ejecutar()
    {
        while (true)
        {
            switch (vista.LeerOpcion())
            {
                case 1: Alta(); break;
                case 2: vista.MostrarResultados(Gestor.Listar()); break;
                case 3: Buscar(); break;
                case 4: Modificar(); break;
                case 5: Baja(); break;
                case 0: return;
            }
        }
    }

    private void Alta()
    {
        var datos = vista.LeerDatos();
        Persona persona = Gestor.Alta(datos.Nombre, datos.Apellidos, datos.Correo, datos.Telefono);
        vista.AltaRealizada();
        vista.MostrarPersona(persona);
    }

    private void Buscar()
    {
        if (vista.LeerTipoBusqueda() == 1)
        {
            Persona? persona = Localizar();
            if (persona is not null)
                vista.MostrarPersona(persona);
        }
        else
        {
            string texto = vista.LeerBusqueda();
            vista.MostrarBusqueda(Gestor.BuscarPorNombre(texto));
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
        vista.MostrarPersona(persona);
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
        vista.MostrarPersona(Gestor.BuscarPorId(persona.Id)!);
    }

    private void Baja()
    {
        Persona? persona = Localizar();
        if (persona is null)
            return;
        vista.MostrarPersona(persona);
        if (!vista.ConfirmarBaja())
        {
            vista.BajaCancelada();
            return;
        }
        Gestor.Baja(persona.Id);
        vista.BajaRealizada();
    }

}
