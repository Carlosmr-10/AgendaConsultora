using AgendaConsultora.Views;
using AgendaConsultora.Models;
using AgendaConsultora.Models.Services;

namespace AgendaConsultora.Controllers;

internal sealed class ControladorEmpresas
{
    private readonly GestorEmpresas gestor;
    private readonly VistaEmpresas vista;
    public ControladorEmpresas(GestorEmpresas gestor, VistaEmpresas vista)
    {
        this.gestor = gestor;
        this.vista = vista;
    }

    public void Ejecutar()
    {
        while (true)
        {
            switch (vista.LeerOpcion())
            {
                case 1: Alta(); break;
                case 2: Mostrar(gestor.Listar()); break;
                case 3: Buscar(); break;
                case 4: Modificar(); break;
                case 5: CambiarEstado(false); break;
                case 6: CambiarEstado(true); break;
                case 0: return;
            }
        }
    }

    private void Alta()
    {
        var d = vista.LeerDatos();
        Empresa empresa = gestor.Alta(d.Nombre, d.Cif, d.Telefono, d.Correo, d.Direccion);
        vista.AltaRealizada();
        MostrarEmpresa(empresa);
    }

    private Empresa? Localizar()
    {
        Empresa? empresa = gestor.BuscarPorId(vista.LeerId());
        if (empresa is null) vista.NoExiste();
        return empresa;
    }

    private void Buscar()
    {
        if (vista.LeerTipoBusqueda() == 1)
        {
            Empresa? empresa = Localizar();
            if (empresa is not null) MostrarEmpresa(empresa);
        }
        else
        {
            string texto = vista.LeerBusqueda();
            Mostrar(gestor.Buscar(texto));
        }
    }

    private void Modificar()
    {
        Empresa? empresa = Localizar();
        if (empresa is null) return;
        vista.DatosActuales();
        MostrarEmpresa(empresa);
        vista.SolicitarNuevosDatos();
        var d = vista.LeerDatos();
        vista.MostrarPropuesta(d);
        if (!vista.ConfirmarModificacion())
        {
            vista.ModificacionCancelada();
            return;
        }
        gestor.Modificar(empresa.Id, d.Nombre, d.Cif, d.Telefono, d.Correo, d.Direccion);
        vista.ModificacionRealizada();
        MostrarEmpresa(empresa);
    }

    private void CambiarEstado(bool nuevoEstado)
    {
        Empresa? empresa = Localizar();
        if (empresa is null) return;
        MostrarEmpresa(empresa);
        if (empresa.Estado == nuevoEstado)
        {
            vista.EstadoSinCambios(nuevoEstado);
            return;
        }
        if (!nuevoEstado && !gestor.PuedeDesactivar(empresa.Id))
        {
            vista.BajaBloqueada();
            return;
        }
        if (!vista.ConfirmarEstado(nuevoEstado))
        {
            vista.CambioCancelado();
            return;
        }
        bool realizado = nuevoEstado ? gestor.Activar(empresa.Id) : gestor.Desactivar(empresa.Id);
        vista.ResultadoEstado(realizado);
        MostrarEmpresa(empresa);
    }

    private void Mostrar(IReadOnlyList<Empresa> empresas)
    {
        if (empresas.Count == 0) vista.SinResultados();
        foreach (Empresa empresa in empresas) MostrarEmpresa(empresa);
    }

    private void MostrarEmpresa(Empresa empresa) =>
        vista.MostrarEmpresa(empresa, gestor.PuedeDesactivar(empresa.Id));
}
