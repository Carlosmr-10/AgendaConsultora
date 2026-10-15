using AgendaConsultora.Modelos;
using AgendaConsultora.Servicios;
using AgendaConsultora.Vistas;

namespace AgendaConsultora.Controladores;

public sealed class ControladorAgenda
{
    private readonly VistaPrincipal vista;
    private readonly AgendaService agenda;
    private int? personaSeleccionadaId;
    private int? empresaSeleccionadaId;

    public ControladorAgenda(VistaPrincipal vista, AgendaService agenda)
    {
        this.vista = vista;
        this.agenda = agenda;
        ConectarEventos();
        RefrescarTodo();
    }

    private void ConectarEventos()
    {
        vista.GuardarPersonaSolicitado += GuardarPersonaAsync;
        vista.NuevaPersonaSolicitada += NuevaPersona;
        vista.EliminarPersonaSolicitado += EliminarPersonaAsync;
        vista.AsignarEmpresaSolicitado += AsignarEmpresaAsync;
        vista.DesvincularPersonaSolicitado += DesvincularPersonaAsync;
        vista.BuscarPersonaCambiado += RefrescarPersonas;
        vista.PersonaSeleccionada += CargarPersona;
        vista.GuardarEmpresaSolicitado += GuardarEmpresaAsync;
        vista.NuevaEmpresaSolicitada += NuevaEmpresa;
        vista.DesactivarEmpresaSolicitado += DesactivarEmpresaAsync;
        vista.ActivarEmpresaSolicitado += ActivarEmpresaAsync;
        vista.BuscarEmpresaCambiado += RefrescarEmpresas;
        vista.EmpresaSeleccionada += CargarEmpresa;
    }

    private async Task GuardarPersonaAsync()
    {
        try
        {
            if (personaSeleccionadaId is not null
                && !await vista.ConfirmarAsync("Guardar modificación", "¿Quieres guardar los nuevos datos de esta persona?"))
            {
                vista.MostrarEstado("Modificación de persona cancelada.", false);
                return;
            }

            bool esAlta = personaSeleccionadaId is null;
            Persona persona = agenda.GuardarPersona(personaSeleccionadaId, vista.DatosPersonaIntroducidos);
            personaSeleccionadaId = persona.Id;
            RefrescarTodo();
            vista.SeleccionarPersona(persona.Id);
            vista.MostrarEstado(esAlta
                ? $"Persona creada correctamente con Id {persona.Id}."
                : "Persona modificada correctamente.", true);
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            vista.MostrarEstado(ex.Message, false);
        }
    }

    private void NuevaPersona()
    {
        personaSeleccionadaId = null;
        vista.LimpiarPersona();
    }

    private async Task EliminarPersonaAsync()
    {
        Persona? persona = PersonaSeleccionada();
        if (persona is null)
        {
            vista.MostrarEstado("Selecciona primero la persona que quieres dar de baja.", false);
            return;
        }

        if (!await vista.ConfirmarAsync(
                "Dar de baja a una persona",
                $"¿Quieres eliminar a {persona.Nombre} {persona.Apellidos}? Esta operación no se puede deshacer."))
        {
            vista.MostrarEstado("Baja de persona cancelada.", false);
            return;
        }

        agenda.EliminarPersona(persona.Id);
        NuevaPersona();
        RefrescarTodo();
        vista.MostrarEstado("Persona dada de baja correctamente.", true);
    }

    private async Task AsignarEmpresaAsync()
    {
        Persona? persona = PersonaSeleccionada();
        int? idEmpresa = vista.EmpresaElegidaId;
        Empresa? empresa = idEmpresa is int id ? agenda.BuscarEmpresaPorId(id) : null;
        if (persona is null || empresa is null)
        {
            vista.MostrarEstado("Selecciona una persona y una empresa activa.", false);
            return;
        }

        if (persona.IdEmpresa == empresa.Id)
        {
            vista.MostrarEstado("La persona ya pertenece a esa empresa.", false);
            return;
        }

        if (!await vista.ConfirmarAsync(
                "Asignar empresa",
                $"¿Quieres vincular a {persona.Nombre} {persona.Apellidos} con {empresa.NombreComercial}?"))
        {
            vista.MostrarEstado("Asignación cancelada.", false);
            return;
        }

        if (!agenda.AsignarEmpresa(persona.Id, empresa.Id))
        {
            vista.MostrarEstado("No se pudo realizar la asignación. Comprueba que la empresa siga activa.", false);
            return;
        }

        RefrescarTodo();
        vista.SeleccionarPersona(persona.Id);
        vista.MostrarEstado("Empresa asignada correctamente.", true);
    }

    private async Task DesvincularPersonaAsync()
    {
        Persona? persona = PersonaSeleccionada();
        if (persona?.IdEmpresa is null)
        {
            vista.MostrarEstado("La persona seleccionada no tiene una empresa asignada.", false);
            return;
        }

        if (!await vista.ConfirmarAsync(
                "Desvincular persona",
                $"¿Quieres dejar a {persona.Nombre} {persona.Apellidos} sin empresa?"))
        {
            vista.MostrarEstado("Desvinculación cancelada.", false);
            return;
        }

        agenda.DesvincularPersona(persona.Id);
        RefrescarTodo();
        vista.SeleccionarPersona(persona.Id);
        vista.MostrarEstado("Persona desvinculada. Sus datos se han conservado.", true);
    }

    private async Task GuardarEmpresaAsync()
    {
        try
        {
            if (empresaSeleccionadaId is not null
                && !await vista.ConfirmarAsync("Guardar modificación", "¿Quieres guardar los nuevos datos de esta empresa?"))
            {
                vista.MostrarEstado("Modificación de empresa cancelada.", false);
                return;
            }

            bool esAlta = empresaSeleccionadaId is null;
            Empresa empresa = agenda.GuardarEmpresa(empresaSeleccionadaId, vista.DatosEmpresaIntroducidos);
            empresaSeleccionadaId = empresa.Id;
            RefrescarTodo();
            vista.SeleccionarEmpresa(empresa.Id);
            vista.MostrarEstado(esAlta
                ? $"Empresa creada correctamente con Id {empresa.Id}."
                : "Empresa modificada correctamente.", true);
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            vista.MostrarEstado(ex.Message, false);
        }
    }

    private void NuevaEmpresa()
    {
        empresaSeleccionadaId = null;
        vista.LimpiarEmpresa();
    }

    private async Task DesactivarEmpresaAsync()
    {
        Empresa? empresa = EmpresaSeleccionada();
        if (empresa is null)
        {
            vista.MostrarEstado("Selecciona primero una empresa.", false);
            return;
        }

        if (!empresa.Estado)
        {
            vista.MostrarEstado("La empresa ya está inactiva.", false);
            return;
        }

        if (agenda.ListarPersonasDeEmpresa(empresa.Id).Count > 0)
        {
            vista.MostrarEstado("No se puede desactivar: desvincula o traslada antes a todas las personas.", false);
            return;
        }

        if (!await vista.ConfirmarAsync(
                "Desactivar empresa",
                $"¿Quieres cambiar el Estado de {empresa.NombreComercial} a inactiva? El registro se conservará."))
        {
            vista.MostrarEstado("Baja lógica cancelada.", false);
            return;
        }

        agenda.DesactivarEmpresa(empresa.Id);
        RefrescarTodo();
        vista.SeleccionarEmpresa(empresa.Id);
        vista.MostrarEstado("Empresa desactivada. El registro permanece guardado.", true);
    }

    private async Task ActivarEmpresaAsync()
    {
        Empresa? empresa = EmpresaSeleccionada();
        if (empresa is null)
        {
            vista.MostrarEstado("Selecciona primero una empresa.", false);
            return;
        }

        if (empresa.Estado)
        {
            vista.MostrarEstado("La empresa ya está activa.", false);
            return;
        }

        if (!await vista.ConfirmarAsync("Reactivar empresa", $"¿Quieres volver a activar {empresa.NombreComercial}?"))
        {
            vista.MostrarEstado("Reactivación cancelada.", false);
            return;
        }

        agenda.ActivarEmpresa(empresa.Id);
        RefrescarTodo();
        vista.SeleccionarEmpresa(empresa.Id);
        vista.MostrarEstado("Empresa reactivada correctamente.", true);
    }

    private void CargarPersona(int id)
    {
        Persona? persona = agenda.BuscarPersonaPorId(id);
        if (persona is null) return;

        personaSeleccionadaId = id;
        Empresa? empresa = agenda.ObtenerEmpresaDePersona(persona);
        vista.CargarPersona(persona, empresa is null
            ? "Sin empresa asignada"
            : $"Actualmente: {empresa.NombreComercial}");
    }

    private void CargarEmpresa(int id)
    {
        Empresa? empresa = agenda.BuscarEmpresaPorId(id);
        if (empresa is null) return;

        empresaSeleccionadaId = id;
        string[] plantilla = agenda.ListarPersonasDeEmpresa(id)
            .Select(persona => $"{persona.Apellidos}, {persona.Nombre} · Id {persona.Id}")
            .ToArray();
        vista.CargarEmpresa(empresa, plantilla);
    }

    private void RefrescarTodo()
    {
        RefrescarPersonas();
        RefrescarEmpresas();
        RefrescarEmpresasDisponibles();
    }

    private void RefrescarPersonas()
    {
        PersonaFila[] filas = agenda.ListarPersonas(vista.FiltroPersonas).Select(persona =>
        {
            Empresa? empresa = agenda.ObtenerEmpresaDePersona(persona);
            return new PersonaFila(
                persona.Id,
                ObtenerIniciales(persona),
                $"{persona.Apellidos}, {persona.Nombre}",
                $"{persona.Correo} · {persona.Telefono}",
                empresa is null ? "Sin empresa asignada" : empresa.NombreComercial,
                $"Id {persona.Id}");
        }).ToArray();
        vista.MostrarPersonas(filas);
    }

    private void RefrescarEmpresas()
    {
        EmpresaFila[] filas = agenda.ListarEmpresas(vista.FiltroEmpresas).Select(empresa =>
        {
            int total = agenda.ListarPersonasDeEmpresa(empresa.Id).Count;
            return new EmpresaFila(
                empresa.Id,
                empresa.NombreComercial,
                $"{empresa.Cif} · {empresa.Correo}",
                total == 1 ? "1 persona asociada" : $"{total} personas asociadas",
                empresa.Estado ? "Activa" : "Inactiva",
                empresa.Estado ? "#DCFCE7" : "#FEE2E2",
                empresa.Estado ? "#166534" : "#991B1B",
                $"Id {empresa.Id}");
        }).ToArray();
        vista.MostrarEmpresas(filas);
    }

    private void RefrescarEmpresasDisponibles()
    {
        EmpresaOpcion[] opciones = agenda.ListarEmpresas()
            .Where(empresa => empresa.Estado)
            .Select(empresa => new EmpresaOpcion(empresa.Id, $"{empresa.NombreComercial} · {empresa.Cif}"))
            .ToArray();
        vista.MostrarEmpresasDisponibles(opciones);
    }

    private Persona? PersonaSeleccionada() =>
        personaSeleccionadaId is int id ? agenda.BuscarPersonaPorId(id) : null;

    private Empresa? EmpresaSeleccionada() =>
        empresaSeleccionadaId is int id ? agenda.BuscarEmpresaPorId(id) : null;

    private static string ObtenerIniciales(Persona persona) =>
        $"{persona.Nombre.FirstOrDefault()}{persona.Apellidos.FirstOrDefault()}".ToUpperInvariant();
}
