using System.Net.Mail;
using System.Text.RegularExpressions;

namespace AgendaProfesional.Utils;

public static class InputValidator
{
    public static string LeerTextoObligatorio(string mensaje, string error)
    {
        return LeerHastaValidar(mensaje, EsTextoObligatorio, error);
    }

    public static string LeerCorreo(string mensaje)
    {
        return LeerHastaValidar(
            mensaje,
            EsCorreoValido,
            "Correo no válido. Ejemplo: nombre@empresa.es");
    }

    public static string LeerTelefono(string mensaje)
    {
        return LeerHastaValidar(
            mensaje,
            EsTelefonoValido,
            "Teléfono no válido. Introduce exactamente 9 dígitos.");
    }

    public static int LeerOpcion(string mensaje, int minimo, int maximo)
    {
        while (true)
        {
            string valor = LeerLinea(mensaje);
            if (int.TryParse(valor, out int numero) && numero >= minimo && numero <= maximo)
                return numero;

            Console.WriteLine($"Opción no válida. Introduce un número entre {minimo} y {maximo}.");
        }
    }

    public static int LeerId(string mensaje) => LeerOpcion(mensaje, 1, int.MaxValue);

    public static bool LeerConfirmacion(string mensaje)
    {
        while (true)
        {
            string respuesta = LeerLinea($"{mensaje} (S/SI/N/NO): ").ToUpperInvariant();
            if (respuesta is "S" or "SI" or "SÍ") return true;
            if (respuesta is "N" or "NO") return false;
            Console.WriteLine("Respuesta no válida. Escribe S o N.");
        }
    }

    public static bool EsTextoObligatorio(string valor) => !string.IsNullOrWhiteSpace(valor);

    public static bool EsCorreoValido(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor) || valor.Length > 254)
            return false;

        return MailAddress.TryCreate(valor, out MailAddress? correo)
            && correo.Address == valor
            && Regex.IsMatch(
                valor,
                @"\A[^\s@.]+(?:\.[^\s@.]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z]{2,}\z");
    }

    public static bool EsTelefonoValido(string valor) =>
        Regex.IsMatch(valor, @"\A[0-9]{9}\z");

    private static string LeerHastaValidar(
        string mensaje,
        Func<string, bool> validar,
        string error)
    {
        while (true)
        {
            string valor = LeerLinea(mensaje);
            if (validar(valor)) return valor;
            Console.WriteLine(error);
        }
    }

    private static string LeerLinea(string mensaje)
    {
        Console.Write(mensaje);
        return Console.ReadLine()?.Trim() ?? throw new EndOfStreamException();
    }
}
