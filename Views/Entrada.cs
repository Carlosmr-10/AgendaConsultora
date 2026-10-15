using System;
using System.IO;

namespace AgendaConsultora.Views;

public static class Entrada
{
    public static string Leer(string mensaje)
    {
        Console.Write(mensaje);
        // EOF es distinto de una línea vacía: permite finalizar una entrada redirigida.
        return Console.ReadLine()?.Trim() ?? throw new EndOfStreamException();
    }

    public static string Texto(string mensaje, Func<string, bool> validar, string error)
    {
        while (true)
        {
            string valor = Leer(mensaje);
            if (validar(valor))
                return valor;
            Console.WriteLine(error);
        }
    }

    public static int Numero(string mensaje, int minimo, int maximo)
    {
        while (true)
        {
            string valor = Leer(mensaje);
            if (IntentarLeerNumeroEnRango(valor, minimo, maximo, out int numero))
                return numero;
            Console.WriteLine($"Opción no válida. Introduce un número entre {minimo} y {maximo}.");
        }
    }

    public static bool Confirmar(string mensaje)
    {
        while (true)
        {
            string respuesta = Leer(mensaje + " (S/SI/N/NO): ").ToUpperInvariant();
            if (EsRespuestaAfirmativa(respuesta))
                return true;
            if (EsRespuestaNegativa(respuesta))
                return false;
            Console.WriteLine("Respuesta no válida. Escribe S o N.");
        }
    }

    private static bool IntentarLeerNumeroEnRango(
        string valor,
        int minimo,
        int maximo,
        out int numero) =>
        int.TryParse(valor, out numero) && numero >= minimo && numero <= maximo;

    private static bool EsRespuestaAfirmativa(string respuesta) =>
        respuesta is "S" or "SI" or "SÍ";

    private static bool EsRespuestaNegativa(string respuesta) =>
        respuesta is "N" or "NO";
}
