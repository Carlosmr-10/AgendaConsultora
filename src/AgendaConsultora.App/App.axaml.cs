using AgendaConsultora.Controladores;
using AgendaConsultora.Servicios;
using AgendaConsultora.Vistas;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace AgendaConsultora;

public partial class App : Application
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime escritorio)
        {
            var vista = new VistaPrincipal();
            _ = new ControladorAgenda(vista, new AgendaService());
            escritorio.MainWindow = vista;
        }

        base.OnFrameworkInitializationCompleted();
    }
}
