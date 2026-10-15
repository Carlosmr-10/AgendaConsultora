using System.Text;
using AgendaProfesional.Models;
using AgendaProfesional.Services;
using AgendaProfesional.Utils;

namespace AgendaProfesional.UI;

public sealed class ConsoleUI
{
    private readonly AgendaService agenda;

    public ConsoleUI(AgendaService agenda)
    {
        this.agenda = agenda;
    }

    public void Ejecutar()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("Datos en memoria: se perderán al cerrar la aplicación.");

        try
        {
            EjecutarMenuPrincipal();
        }
        catch (EndOfStreamException)
        {
            Console.WriteLine("\nFin de entrada. Operación pendiente cancelada. Agenda cerrada.");
        }
    }

    private void EjecutarMenuPrincipal()
    {
        while (true)
        {
            MostrarMenuPrincipal();
            switch (InputValidator.LeerOpcion("Selecciona una opción: ", 0, 2))
            {
                case 1: EjecutarMenuPersonas(); break;
                case 2: EjecutarMenuEmpresas(); break;
                case 0: Console.WriteLine("Hasta pronto."); return;
            }
        }
    }

    private static void MostrarMenuPrincipal()
    {
        Console.WriteLine("\n=== AGENDA PROFESIONAL — FASE 5 ===");
        Console.WriteLine("1. Gestión de personas");
        Console.WriteLine("2. Gestión de empresas");
        Console.WriteLine("0. Salir");
    }

    private void EjecutarMenuPersonas()
    {
        while (true)
        {
            MostrarMenuPersonas();
            switch (InputValidator.LeerOpcion("Selecciona una opción: ", 0, 7))
            {
                case 1: DarDeAltaPersona(); break;
                case 2: MostrarPersonas(agenda.ListarPersonas(), false); break;
                case 3: BuscarPersona(); break;
                case 4: ModificarPersona(); break;
                case 5: DarDeBajaPersona(); break;
                case 6: AsignarOCambiarEmpresa(); break;
                case 7: DesvincularPersona(); break;
                case 0: return;
            }
        }
    }

    private static void MostrarMenuPersonas()
    {
        Console.WriteLine("\n=== PERSONAS ===");
        Console.WriteLine("1. Dar de alta una persona");
        Console.WriteLine("2. Listar personas");
        Console.WriteLine("3. Buscar persona");
        Console.WriteLine("4. Modificar persona");
        Console.WriteLine("5. Dar de baja una persona");
        Console.WriteLine("6. Asignar o cambiar empresa");
        Console.WriteLine("7. Desvincular empresa");
        Console.WriteLine("0. Volver al menú principal");
    }

    private void DarDeAltaPersona()
    {
        Persona persona = agenda.DarDeAltaPersona(LeerDatosPersona());
        Console.WriteLine("Alta realizada correctamente.");
        MostrarPersona(persona);
    }

    private void BuscarPersona()
    {
        Console.WriteLine("1. Buscar por Id");
        Console.WriteLine("2. Buscar por nombre y apellidos");
        if (InputValidator.LeerOpcion("Tipo de búsqueda: ", 1, 2) == 1)
        {
            Persona? persona = LocalizarPersona();
            if (persona is not null) MostrarPersona(persona);
            return;
        }

        string texto = InputValidator.LeerTextoObligatorio(
            "Nombre o apellidos (admite partes): ",
            "Introduce un texto de búsqueda.");
        MostrarPersonas(agenda.BuscarPersonas(texto), true);
    }

    private void ModificarPersona()
    {
        Persona? persona = LocalizarPersona();
        if (persona is null) return;

        Console.WriteLine("Datos actuales:");
        MostrarPersona(persona);
        Console.WriteLine("Introduce todos los datos nuevos, repitiendo los que quieras conservar.");
        DatosPersona datos = LeerDatosPersona();
        Console.WriteLine(
            $"Propuesta para Id {persona.Id}: {datos.Nombre} {datos.Apellidos} | {datos.Correo} | {datos.Telefono}");

        if (!InputValidator.LeerConfirmacion("¿Guardar los cambios?"))
        {
            Console.WriteLine("Modificación cancelada.");
            return;
        }

        agenda.ModificarPersona(persona.Id, datos);
        Console.WriteLine("Modificación realizada correctamente.");
        MostrarPersona(persona);
    }

    private void DarDeBajaPersona()
    {
        Persona? persona = LocalizarPersona();
        if (persona is null) return;

        MostrarPersona(persona);
        if (!InputValidator.LeerConfirmacion("¿Eliminar esta persona?"))
        {
            Console.WriteLine("Baja cancelada.");
            return;
        }

        agenda.DarDeBajaPersona(persona.Id);
        Console.WriteLine("Baja realizada correctamente.");
    }

    private void AsignarOCambiarEmpresa()
    {
        Persona? persona = LocalizarPersona();
        if (persona is null) return;

        MostrarPersona(persona);
        int idEmpresa = InputValidator.LeerId("Id de la empresa a asignar: ");
        Empresa? empresa = agenda.BuscarEmpresaPorId(idEmpresa);
        if (empresa is null)
        {
            Console.WriteLine("No existe una empresa con ese Id.");
            return;
        }
        if (!empresa.Estado)
        {
            Console.WriteLine("No se puede asignar una empresa inactiva. Reactívala primero.");
            return;
        }
        if (persona.IdEmpresa == empresa.Id)
        {
            Console.WriteLine("La persona ya pertenece a esa empresa.");
            return;
        }

        Console.WriteLine($"Empresa seleccionada: {empresa.NombreComercial} (Id: {empresa.Id})");
        if (!InputValidator.LeerConfirmacion("¿Asignar esta empresa a la persona?"))
        {
            Console.WriteLine("Cambio de relación cancelado.");
            return;
        }

        agenda.AsignarEmpresa(persona.Id, empresa.Id);
        Console.WriteLine("Empresa asignada correctamente.");
        MostrarPersona(persona);
    }

    private void DesvincularPersona()
    {
        Persona? persona = LocalizarPersona();
        if (persona is null) return;

        MostrarPersona(persona);
        if (persona.IdEmpresa is null)
        {
            Console.WriteLine("La persona ya está sin empresa asignada.");
            return;
        }
        if (!InputValidator.LeerConfirmacion("¿Desvincular la persona conservando sus datos?"))
        {
            Console.WriteLine("Cambio de relación cancelado.");
            return;
        }

        agenda.DesvincularPersona(persona.Id);
        Console.WriteLine("Persona desvinculada correctamente.");
        MostrarPersona(persona);
    }

    private Persona? LocalizarPersona()
    {
        int id = InputValidator.LeerId("Id de la persona: ");
        Persona? persona = agenda.BuscarPersonaPorId(id);
        if (persona is null) Console.WriteLine("No existe una persona con ese Id.");
        return persona;
    }

    private static DatosPersona LeerDatosPersona()
    {
        Console.WriteLine("Todos los campos son obligatorios.");
        return new DatosPersona(
            InputValidator.LeerTextoObligatorio("Nombre: ", "El nombre no puede estar vacío."),
            InputValidator.LeerTextoObligatorio("Apellidos: ", "Los apellidos no pueden estar vacíos."),
            InputValidator.LeerCorreo("Correo: "),
            InputValidator.LeerTelefono("Teléfono (9 dígitos sin separadores): "));
    }

    private void MostrarPersonas(IReadOnlyList<Persona> personas, bool esBusqueda)
    {
        if (personas.Count == 0)
        {
            Console.WriteLine(esBusqueda
                ? "No se encontraron coincidencias."
                : "No hay personas registradas.");
            return;
        }

        foreach (Persona persona in personas) MostrarPersona(persona);
    }

    private void MostrarPersona(Persona persona)
    {
        Empresa? empresa = agenda.ObtenerEmpresaDePersona(persona.Id);
        Console.WriteLine($"Id: {persona.Id}");
        Console.WriteLine($"Nombre: {persona.Nombre} {persona.Apellidos}");
        Console.WriteLine($"Correo: {persona.Correo}");
        Console.WriteLine($"Teléfono: {persona.Telefono}");
        Console.WriteLine(empresa is null
            ? "Empresa: Sin empresa asignada"
            : $"Empresa: {empresa.NombreComercial} (Id: {empresa.Id})");
        Console.WriteLine("----------------------------------------");
    }

    private void EjecutarMenuEmpresas()
    {
        while (true)
        {
            MostrarMenuEmpresas();
            switch (InputValidator.LeerOpcion("Selecciona una opción: ", 0, 7))
            {
                case 1: DarDeAltaEmpresa(); break;
                case 2: MostrarEmpresas(agenda.ListarEmpresas()); break;
                case 3: BuscarEmpresa(); break;
                case 4: ModificarEmpresa(); break;
                case 5: CambiarEstadoEmpresa(false); break;
                case 6: CambiarEstadoEmpresa(true); break;
                case 7: VerPlantilla(); break;
                case 0: return;
            }
        }
    }

    private static void MostrarMenuEmpresas()
    {
        Console.WriteLine("\n=== EMPRESAS ===");
        Console.WriteLine("1. Alta");
        Console.WriteLine("2. Listar (activas e inactivas)");
        Console.WriteLine("3. Buscar");
        Console.WriteLine("4. Modificar");
        Console.WriteLine("5. Dar de baja (solo sin relaciones)");
        Console.WriteLine("6. Reactivar");
        Console.WriteLine("7. Ver personas de una empresa");
        Console.WriteLine("0. Volver al menú principal");
    }

    private void DarDeAltaEmpresa()
    {
        Empresa empresa = agenda.DarDeAltaEmpresa(LeerDatosEmpresa());
        Console.WriteLine("Alta de empresa realizada correctamente.");
        MostrarEmpresa(empresa);
    }

    private void BuscarEmpresa()
    {
        Console.WriteLine("1. Por Id");
        Console.WriteLine("2. Por nombre comercial o CIF");
        if (InputValidator.LeerOpcion("Tipo de búsqueda: ", 1, 2) == 1)
        {
            Empresa? empresa = LocalizarEmpresa();
            if (empresa is not null) MostrarEmpresa(empresa);
            return;
        }

        string texto = InputValidator.LeerTextoObligatorio(
            "Texto de búsqueda: ", "Introduce un texto.");
        MostrarEmpresas(agenda.BuscarEmpresas(texto));
    }

    private void ModificarEmpresa()
    {
        Empresa? empresa = LocalizarEmpresa();
        if (empresa is null) return;

        Console.WriteLine("Datos actuales:");
        MostrarEmpresa(empresa);
        Console.WriteLine("Introduce los datos nuevos, repitiendo los que quieras conservar.");
        DatosEmpresa datos = LeerDatosEmpresa();
        Console.WriteLine(
            $"Propuesta: {datos.NombreComercial} | {datos.Cif} | {datos.Telefono} | {datos.Correo} | {datos.Direccion}");
        if (!InputValidator.LeerConfirmacion("¿Guardar los cambios?"))
        {
            Console.WriteLine("Modificación cancelada.");
            return;
        }

        agenda.ModificarEmpresa(empresa.Id, datos);
        Console.WriteLine("Empresa modificada correctamente.");
        MostrarEmpresa(empresa);
    }

    private void CambiarEstadoEmpresa(bool nuevoEstado)
    {
        Empresa? empresa = LocalizarEmpresa();
        if (empresa is null) return;

        MostrarEmpresa(empresa);
        if (empresa.Estado == nuevoEstado)
        {
            Console.WriteLine(nuevoEstado
                ? "La empresa ya está activa."
                : "La empresa ya está inactiva.");
            return;
        }
        if (!nuevoEstado && !agenda.PuedeDesactivarEmpresa(empresa.Id))
        {
            Console.WriteLine("Baja bloqueada: la empresa tiene personas asociadas.");
            return;
        }

        string pregunta = nuevoEstado
            ? "¿Reactivar esta empresa?"
            : "¿Desactivar esta empresa sin borrar su registro?";
        if (!InputValidator.LeerConfirmacion(pregunta))
        {
            Console.WriteLine("Cambio de estado cancelado.");
            return;
        }

        bool realizado = nuevoEstado
            ? agenda.ActivarEmpresa(empresa.Id)
            : agenda.DesactivarEmpresa(empresa.Id);
        Console.WriteLine(realizado
            ? "Estado actualizado correctamente."
            : "No se pudo cambiar el estado.");
        MostrarEmpresa(empresa);
    }

    private void VerPlantilla()
    {
        Empresa? empresa = LocalizarEmpresa();
        if (empresa is null) return;

        Console.WriteLine($"Plantilla de {empresa.NombreComercial} (Id: {empresa.Id})");
        IReadOnlyList<Persona> plantilla = agenda.ListarPersonasDeEmpresa(empresa.Id);
        if (plantilla.Count == 0)
        {
            Console.WriteLine("Esta empresa no tiene personas asociadas.");
            return;
        }

        foreach (Persona persona in plantilla) MostrarPersona(persona);
    }

    private Empresa? LocalizarEmpresa()
    {
        int id = InputValidator.LeerId("Id de la empresa: ");
        Empresa? empresa = agenda.BuscarEmpresaPorId(id);
        if (empresa is null) Console.WriteLine("No existe una empresa con ese Id.");
        return empresa;
    }

    private static DatosEmpresa LeerDatosEmpresa()
    {
        Console.WriteLine("Todos los campos son obligatorios.");
        return new DatosEmpresa(
            InputValidator.LeerTextoObligatorio(
                "Nombre comercial: ", "El nombre comercial es obligatorio."),
            InputValidator.LeerTextoObligatorio("CIF: ", "El CIF no puede estar vacío."),
            InputValidator.LeerTelefono("Teléfono (9 dígitos): "),
            InputValidator.LeerCorreo("Correo: "),
            InputValidator.LeerTextoObligatorio(
                "Dirección: ", "La dirección es obligatoria."));
    }

    private void MostrarEmpresas(IReadOnlyList<Empresa> empresas)
    {
        if (empresas.Count == 0)
        {
            Console.WriteLine("No hay empresas que mostrar.");
            return;
        }

        foreach (Empresa empresa in empresas) MostrarEmpresa(empresa);
    }

    private void MostrarEmpresa(Empresa empresa)
    {
        Console.WriteLine($"Id: {empresa.Id}");
        Console.WriteLine($"Nombre comercial: {empresa.NombreComercial}");
        Console.WriteLine($"CIF: {empresa.Cif}");
        Console.WriteLine($"Teléfono: {empresa.Telefono}");
        Console.WriteLine($"Correo: {empresa.Correo}");
        Console.WriteLine($"Dirección: {empresa.Direccion}");
        Console.WriteLine($"Estado: {(empresa.Estado ? "Activa" : "Inactiva")}");
        Console.WriteLine(
            $"Baja disponible: {(agenda.PuedeDesactivarEmpresa(empresa.Id) ? "Sí" : "No")}");
        Console.WriteLine("----------------------------------------");
    }
}
