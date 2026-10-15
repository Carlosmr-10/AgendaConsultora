using AgendaConsultora.Modelos;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace AgendaConsultora.Vistas;

public partial class VistaPrincipal : Window
{
    public event Func<Task>? GuardarPersonaSolicitado;
    public event Action? NuevaPersonaSolicitada;
    public event Func<Task>? EliminarPersonaSolicitado;
    public event Func<Task>? AsignarEmpresaSolicitado;
    public event Func<Task>? DesvincularPersonaSolicitado;
    public event Action? BuscarPersonaCambiado;
    public event Action<int>? PersonaSeleccionada;
    public event Func<Task>? GuardarEmpresaSolicitado;
    public event Action? NuevaEmpresaSolicitada;
    public event Func<Task>? DesactivarEmpresaSolicitado;
    public event Func<Task>? ActivarEmpresaSolicitado;
    public event Action? BuscarEmpresaCambiado;
    public event Action<int>? EmpresaSeleccionada;

    public VistaPrincipal() => InitializeComponent();

    public DatosPersona DatosPersonaIntroducidos => new(
        PersonaNombre.Text ?? string.Empty,
        PersonaApellidos.Text ?? string.Empty,
        PersonaCorreo.Text ?? string.Empty,
        PersonaTelefono.Text ?? string.Empty);

    public DatosEmpresa DatosEmpresaIntroducidos => new(
        EmpresaNombre.Text ?? string.Empty,
        EmpresaCif.Text ?? string.Empty,
        EmpresaTelefono.Text ?? string.Empty,
        EmpresaCorreo.Text ?? string.Empty,
        EmpresaDireccion.Text ?? string.Empty);

    public string FiltroPersonas => BuscarPersona.Text ?? string.Empty;
    public string FiltroEmpresas => BuscarEmpresa.Text ?? string.Empty;
    public int? EmpresaElegidaId => (EmpresaParaPersona.SelectedItem as EmpresaOpcion)?.Id;

    public void MostrarPersonas(IReadOnlyList<PersonaFila> filas)
    {
        ListaPersonas.ItemsSource = filas;
        PersonasContador.Text = filas.Count == 1 ? "1 persona" : $"{filas.Count} personas";
    }

    public void MostrarEmpresas(IReadOnlyList<EmpresaFila> filas)
    {
        ListaEmpresas.ItemsSource = filas;
        EmpresasContador.Text = filas.Count == 1 ? "1 empresa" : $"{filas.Count} empresas";
    }

    public void MostrarEmpresasDisponibles(IReadOnlyList<EmpresaOpcion> opciones) =>
        EmpresaParaPersona.ItemsSource = opciones;

    public void CargarPersona(Persona persona, string empresaActual)
    {
        PersonaModoText.Text = $"Editando Id {persona.Id}";
        PersonaNombre.Text = persona.Nombre;
        PersonaApellidos.Text = persona.Apellidos;
        PersonaCorreo.Text = persona.Correo;
        PersonaTelefono.Text = persona.Telefono;
        PersonaEmpresaActual.Text = empresaActual;
        EmpresaParaPersona.SelectedItem = (EmpresaParaPersona.ItemsSource as IEnumerable<EmpresaOpcion>)?
            .FirstOrDefault(opcion => opcion.Id == persona.IdEmpresa);
    }

    public void CargarEmpresa(Empresa empresa, IReadOnlyList<string> plantilla)
    {
        EmpresaModoText.Text = $"Editando Id {empresa.Id}";
        EmpresaNombre.Text = empresa.NombreComercial;
        EmpresaCif.Text = empresa.Cif;
        EmpresaCorreo.Text = empresa.Correo;
        EmpresaTelefono.Text = empresa.Telefono;
        EmpresaDireccion.Text = empresa.Direccion;
        EmpresaEstadoActual.Text = empresa.Estado ? "Activa" : "Inactiva";
        ListaPlantilla.ItemsSource = plantilla.Count == 0 ? ["Sin personas asociadas"] : plantilla;
    }

    public void LimpiarPersona()
    {
        ListaPersonas.SelectedItem = null;
        PersonaModoText.Text = "Nueva persona";
        PersonaNombre.Text = string.Empty;
        PersonaApellidos.Text = string.Empty;
        PersonaCorreo.Text = string.Empty;
        PersonaTelefono.Text = string.Empty;
        PersonaEmpresaActual.Text = "Selecciona una persona";
        EmpresaParaPersona.SelectedItem = null;
        PersonaNombre.Focus();
    }

    public void LimpiarEmpresa()
    {
        ListaEmpresas.SelectedItem = null;
        EmpresaModoText.Text = "Nueva empresa";
        EmpresaNombre.Text = string.Empty;
        EmpresaCif.Text = string.Empty;
        EmpresaCorreo.Text = string.Empty;
        EmpresaTelefono.Text = string.Empty;
        EmpresaDireccion.Text = string.Empty;
        EmpresaEstadoActual.Text = "Selecciona una empresa";
        ListaPlantilla.ItemsSource = Array.Empty<string>();
        EmpresaNombre.Focus();
    }

    public void SeleccionarPersona(int id)
    {
        ListaPersonas.SelectedItem = (ListaPersonas.ItemsSource as IEnumerable<PersonaFila>)?
            .FirstOrDefault(fila => fila.Id == id);
    }

    public void SeleccionarEmpresa(int id)
    {
        ListaEmpresas.SelectedItem = (ListaEmpresas.ItemsSource as IEnumerable<EmpresaFila>)?
            .FirstOrDefault(fila => fila.Id == id);
    }

    public void MostrarEstado(string mensaje, bool correcto)
    {
        MensajeEstado.Text = mensaje;
        BarraEstado.Background = new SolidColorBrush(Color.Parse(correcto ? "#DCFCE7" : "#FEE2E2"));
        MensajeEstado.Foreground = new SolidColorBrush(Color.Parse(correcto ? "#166534" : "#991B1B"));
    }

    public async Task<bool> ConfirmarAsync(string titulo, string mensaje)
    {
        var aceptar = new Button { Content = "Confirmar", MinWidth = 110 };
        aceptar.Classes.Add("primary");
        var cancelar = new Button { Content = "Cancelar", MinWidth = 100 };
        var dialogo = new Window
        {
            Title = titulo,
            Width = 440,
            Height = 220,
            CanResize = false,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Content = new Border
            {
                Padding = new Avalonia.Thickness(24),
                Child = new StackPanel
                {
                    Spacing = 18,
                    Children =
                    {
                        new TextBlock { Text = titulo, FontSize = 20, FontWeight = FontWeight.Bold },
                        new TextBlock { Text = mensaje, TextWrapping = TextWrapping.Wrap },
                        new StackPanel
                        {
                            Orientation = Avalonia.Layout.Orientation.Horizontal,
                            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                            Spacing = 10,
                            Children = { cancelar, aceptar }
                        }
                    }
                }
            }
        };

        aceptar.Click += (_, _) => dialogo.Close(true);
        cancelar.Click += (_, _) => dialogo.Close(false);
        return await dialogo.ShowDialog<bool>(this);
    }

    private async void OnGuardarPersona(object? sender, RoutedEventArgs e) => await InvocarAsync(GuardarPersonaSolicitado);
    private void OnNuevaPersona(object? sender, RoutedEventArgs e) => NuevaPersonaSolicitada?.Invoke();
    private async void OnEliminarPersona(object? sender, RoutedEventArgs e) => await InvocarAsync(EliminarPersonaSolicitado);
    private async void OnAsignarEmpresa(object? sender, RoutedEventArgs e) => await InvocarAsync(AsignarEmpresaSolicitado);
    private async void OnDesvincularPersona(object? sender, RoutedEventArgs e) => await InvocarAsync(DesvincularPersonaSolicitado);
    private void OnBuscarPersona(object? sender, TextChangedEventArgs e) => BuscarPersonaCambiado?.Invoke();

    private void OnSeleccionPersona(object? sender, SelectionChangedEventArgs e)
    {
        if (ListaPersonas.SelectedItem is PersonaFila fila) PersonaSeleccionada?.Invoke(fila.Id);
    }

    private async void OnGuardarEmpresa(object? sender, RoutedEventArgs e) => await InvocarAsync(GuardarEmpresaSolicitado);
    private void OnNuevaEmpresa(object? sender, RoutedEventArgs e) => NuevaEmpresaSolicitada?.Invoke();
    private async void OnDesactivarEmpresa(object? sender, RoutedEventArgs e) => await InvocarAsync(DesactivarEmpresaSolicitado);
    private async void OnActivarEmpresa(object? sender, RoutedEventArgs e) => await InvocarAsync(ActivarEmpresaSolicitado);
    private void OnBuscarEmpresa(object? sender, TextChangedEventArgs e) => BuscarEmpresaCambiado?.Invoke();

    private void OnSeleccionEmpresa(object? sender, SelectionChangedEventArgs e)
    {
        if (ListaEmpresas.SelectedItem is EmpresaFila fila) EmpresaSeleccionada?.Invoke(fila.Id);
    }

    private static async Task InvocarAsync(Func<Task>? accion)
    {
        if (accion is not null) await accion();
    }
}
