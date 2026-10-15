using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AgendaConsultora.Helpers;
using AgendaConsultora.Models;
using AgendaConsultora.Services;

namespace AgendaConsultora;

internal static class Program
{
    private static readonly GestorPersonas Gestor = new();

    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("Datos en memoria: se perderán al cerrar la aplicación.");
        try
        {
            bool salir = false;
            while (!salir)
            {
                Console.WriteLine("\n=== AGENDA CONSULTORA — FASE 1 ===");
                Console.WriteLine("1. Dar de alta una persona\n2. Listar personas\n3. Buscar persona");
                Console.WriteLine("4. Modificar persona\n5. Dar de baja una persona\n0. Salir");
                switch (Entrada.Numero("Selecciona una opción: ", 0, 5))
                {
                    case 1: Alta(); break;
                    case 2: MostrarResultados(Gestor.Listar()); break;
                    case 3: Buscar(); break;
                    case 4: Modificar(); break;
                    case 5: Baja(); break;
                    case 0: salir = true; break;
                }
            }
            Console.WriteLine("Hasta pronto.");
        }
        catch (EndOfStreamException)
        {
            Console.WriteLine("\nFin de entrada. Operación pendiente cancelada. Agenda cerrada.");
        }
    }

    private static (string Nombre, string Apellidos, string Correo, string Telefono) LeerDatos()
    {
        Console.WriteLine("Todos los campos son obligatorios.");
        string nombre = Entrada.Texto("Nombre: ", Validador.TextoObligatorio, "El nombre no puede estar vacío.");
        string apellidos = Entrada.Texto("Apellidos: ", Validador.TextoObligatorio, "Los apellidos no pueden estar vacíos.");
        string correo = Entrada.Texto("Correo: ", Validador.CorreoValido, "Correo no válido. Ejemplo: ana@consultora.es");
        string telefono = Entrada.Texto("Teléfono (9 dígitos sin separadores): ", Validador.TelefonoValido,
            "Teléfono no válido. Introduce exactamente 9 dígitos, sin letras ni separadores.");
        return (nombre, apellidos, correo, telefono);
    }

    private static void Alta()
    {
        var datos = LeerDatos();
        Persona persona = Gestor.Alta(datos.Nombre, datos.Apellidos, datos.Correo, datos.Telefono);
        Console.WriteLine("Alta realizada correctamente.");
        MostrarPersona(persona);
    }

    private static void Buscar()
    {
        Console.WriteLine("1. Buscar por Id\n2. Buscar por nombre y apellidos");
        if (Entrada.Numero("Tipo de búsqueda: ", 1, 2) == 1)
        {
            Persona? persona = Localizar();
            if (persona is not null)
                MostrarPersona(persona);
        }
        else
        {
            string texto = Entrada.Texto("Nombre o apellidos (admite partes): ",
                Validador.TextoObligatorio, "Introduce un texto de búsqueda.");
            MostrarResultados(Gestor.BuscarPorNombre(texto), "No se encontraron coincidencias.");
        }
    }

    private static Persona? Localizar()
    {
        int id = Entrada.Numero("Id de la persona: ", 1, int.MaxValue);
        Persona? persona = Gestor.BuscarPorId(id);
        if (persona is null)
            Console.WriteLine("No existe una persona con ese Id.");
        return persona;
    }

    private static void Modificar()
    {
        Persona? persona = Localizar();
        if (persona is null)
            return;
        Console.WriteLine("Datos actuales:");
        MostrarPersona(persona);
        Console.WriteLine("Introduce todos los datos nuevos, repitiendo los que quieras conservar.");
        var datos = LeerDatos();
        Console.WriteLine($"Propuesta para Id {persona.Id}: {datos.Nombre} {datos.Apellidos} | {datos.Correo} | {datos.Telefono}");
        if (!Entrada.Confirmar("¿Guardar los cambios?"))
        {
            Console.WriteLine("Modificación cancelada.");
            return;
        }
        Gestor.Modificar(persona.Id, datos.Nombre, datos.Apellidos, datos.Correo, datos.Telefono);
        Console.WriteLine("Modificación realizada correctamente.");
        MostrarPersona(Gestor.BuscarPorId(persona.Id)!);
    }

    private static void Baja()
    {
        Persona? persona = Localizar();
        if (persona is null)
            return;
        MostrarPersona(persona);
        if (!Entrada.Confirmar("¿Eliminar esta persona?"))
        {
            Console.WriteLine("Baja cancelada.");
            return;
        }
        Gestor.Baja(persona.Id);
        Console.WriteLine("Baja realizada correctamente.");
    }

    private static void MostrarResultados(IReadOnlyList<Persona> personas,
        string mensajeVacio = "No hay personas registradas.")
    {
        if (personas.Count == 0)
        {
            Console.WriteLine(mensajeVacio);
            return;
        }
        foreach (Persona persona in personas)
            MostrarPersona(persona);
    }

    private static void MostrarPersona(Persona persona)
    {
        Console.WriteLine($"Id: {persona.Id}\nNombre: {persona.Nombre} {persona.Apellidos}");
        Console.WriteLine($"Correo: {persona.Correo}\nTeléfono: {persona.Telefono}");
        Console.WriteLine("----------------------------------------");
    }
}
