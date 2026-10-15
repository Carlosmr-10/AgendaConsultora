using AgendaProfesional.Services;
using AgendaProfesional.UI;

namespace AgendaProfesional.ConsoleApp;

internal static class Program
{
    private static void Main() => new ConsoleUI(new AgendaService()).Ejecutar();
}
