using AgendaConsultora.Models.Services;
using AgendaConsultora.Views;

namespace AgendaConsultora.Controllers;

internal sealed class ControladorAgenda
{
    private readonly VistaAgenda vista;
    private readonly ControladorPersonas personas;
    private readonly ControladorEmpresas empresas;

    private ControladorAgenda(VistaAgenda vista, ControladorPersonas personas, ControladorEmpresas empresas)
    {
        this.vista = vista;
        this.personas = personas;
        this.empresas = empresas;
    }

    public static void Iniciar()
    {
        CrearControlador().Ejecutar();
    }

    private static ControladorAgenda CrearControlador()
    {
        var gestorPersonas = new GestorPersonas();
        var gestorEmpresas = new GestorEmpresas(gestorPersonas);
        var gestorRelaciones = new GestorRelaciones(gestorPersonas, gestorEmpresas);
        var vistaPersonas = new VistaPersonas();

        return new ControladorAgenda(
            new VistaAgenda(),
            new ControladorPersonas(
                gestorPersonas, vistaPersonas, gestorRelaciones, new VistaRelaciones()),
            new ControladorEmpresas(
                gestorEmpresas, new VistaEmpresas(), gestorPersonas, vistaPersonas));
    }

    private void Ejecutar()
    {
        vista.MostrarInicio();
        try
        {
            EjecutarMenuPrincipal();
        }
        catch (EndOfStreamException)
        {
            vista.MostrarFinEntrada();
        }
    }

    private void EjecutarMenuPrincipal()
    {
        while (!ProcesarOpcionPrincipal(vista.LeerOpcion()))
        {
        }

        vista.MostrarDespedida();
    }

    private bool ProcesarOpcionPrincipal(int opcion)
    {
        switch (opcion)
        {
            case 1: personas.Ejecutar(); return false;
            case 2: empresas.Ejecutar(); return false;
            case 0: return true;
            default: return false;
        }
    }
}
