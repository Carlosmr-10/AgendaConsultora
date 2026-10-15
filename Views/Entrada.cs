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
            if (int.TryParse(valor, out int numero) && numero >= minimo && numero <= maximo)
                return numero;
            Console.WriteLine($"Opción no válida. Introduce un número entre {minimo} y {maximo}.");
        }
    }

    public static bool Confirmar(string mensaje)
    {
        while (true)
        {
            string respuesta = Leer(mensaje + " (S/SI/N/NO): ").ToUpperInvariant();
            if (respuesta is "S" or "SI" or "SÍ")
                return true;
            if (respuesta is "N" or "NO")
                return false;
            Console.WriteLine("Respuesta no válida. Escribe S o N.");
        }
    }
}
