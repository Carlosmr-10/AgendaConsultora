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
        var gestorPersonas = new GestorPersonas();
        var gestorEmpresas = new GestorEmpresas(gestorPersonas);
        var controlador = new ControladorAgenda(
            new VistaAgenda(),
            new ControladorPersonas(gestorPersonas, new VistaPersonas(),
                new GestorRelaciones(gestorPersonas, gestorEmpresas), new VistaRelaciones()),
            new ControladorEmpresas(gestorEmpresas, new VistaEmpresas(), gestorPersonas));
        controlador.Ejecutar();
    }

    private void Ejecutar()
    {
        vista.MostrarInicio();
        try
        {
            while (true)
            {
                switch (vista.LeerOpcion())
                {
                    case 1: personas.Ejecutar(); break;
                    case 2: empresas.Ejecutar(); break;
                    case 0: vista.MostrarDespedida(); return;
                }
            }
        }
        catch (EndOfStreamException)
        {
            vista.MostrarFinEntrada();
        }
    }
}
