using AgendaConsultora.Views;
using AgendaConsultora.Models;
using AgendaConsultora.Models.Services;

namespace AgendaConsultora.Controllers;

internal sealed class ControladorEmpresas
{
    private readonly GestorEmpresas gestor;
    private readonly VistaEmpresas vista;
    private readonly GestorPersonas personas;
    private readonly VistaPersonas vistaPersonas;

    public ControladorEmpresas(
        GestorEmpresas gestor,
        VistaEmpresas vista,
        GestorPersonas personas,
        VistaPersonas vistaPersonas)
    {
        this.gestor = gestor;
        this.vista = vista;
        this.personas = personas;
        this.vistaPersonas = vistaPersonas;
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
            case 5: CambiarEstado(false); return false;
            case 6: CambiarEstado(true); return false;
            case 7: VerPlantilla(); return false;
            case 0: return true;
            default: return false;
        }
    }

    private void DarDeAlta()
    {
        var d = vista.LeerDatos();
        Empresa empresa = gestor.Alta(d.Nombre, d.Cif, d.Telefono, d.Correo, d.Direccion);
        vista.AltaRealizada();
        MostrarEmpresa(empresa);
    }

    private Empresa? LocalizarPorId()
    {
        return OperacionesControlador.BuscarPorId(
            vista.LeerId, gestor.BuscarPorId, vista.NoExiste);
    }

    private void Buscar()
    {
        if (vista.LeerTipoBusqueda() == 1) BuscarPorId();
        else BuscarPorTexto();
    }

    private void BuscarPorId()
    {
        Empresa? empresa = LocalizarPorId();
        if (empresa is not null) MostrarEmpresa(empresa);
    }

    private void BuscarPorTexto()
    {
        MostrarResultados(gestor.Buscar(vista.LeerBusqueda()));
    }

    private void Modificar()
    {
        Empresa? empresa = LocalizarPorId();
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
        Empresa? empresa = LocalizarPorId();
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

    private void MostrarResultados(IReadOnlyList<Empresa> empresas)
    {
        OperacionesControlador.MostrarColeccion(
            empresas,
            vista.SinResultados,
            MostrarEmpresa);
    }

    private void MostrarEmpresa(Empresa empresa) =>
        vista.MostrarEmpresa(empresa, gestor.PuedeDesactivar(empresa.Id));

    private void VerPlantilla()
    {
        Empresa? empresa = LocalizarPorId();
        if (empresa is null) return;
        IReadOnlyList<Persona> plantilla = personas.ListarPorEmpresa(empresa.Id);
        vista.MostrarEncabezadoPlantilla(empresa);
        OperacionesControlador.MostrarColeccion(
            plantilla,
            vista.MostrarPlantillaVacia,
            persona => vistaPersonas.MostrarPersona(persona, empresa));
    }
}
