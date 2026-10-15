using Avalonia;

namespace AgendaConsultora;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args) => CrearAplicacion().StartWithClassicDesktopLifetime(args);

    public static AppBuilder CrearAplicacion() => AppBuilder.Configure<App>()
        .UsePlatformDetect()
#if DEBUG
        .WithDeveloperTools()
#endif
        .WithInterFont()
        .LogToTrace();
}
