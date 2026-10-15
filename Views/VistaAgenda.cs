using System.Text;

namespace AgendaConsultora.Views;

internal sealed class VistaAgenda
{
    public void MostrarInicio()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("Datos en memoria: se perderán al cerrar la aplicación.");
    }

    public int LeerOpcion()
    {
        Console.WriteLine("\n=== AGENDA CONSULTORA ===");
        Console.WriteLine("1. Gestión de personas\n2. Gestión de empresas\n0. Salir");
        return Entrada.Numero("Selecciona una opción: ", 0, 2);
    }

    public void MostrarDespedida() => Console.WriteLine("Hasta pronto.");

    public void MostrarFinEntrada() =>
        Console.WriteLine("\nFin de entrada. Operación pendiente cancelada. Agenda cerrada.");
}
