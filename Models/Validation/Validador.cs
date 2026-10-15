using System.Net.Mail;
using System.Text.RegularExpressions;

namespace AgendaConsultora.Models.Validation;

public static class Validador
{
    public static bool TextoObligatorio(string valor) => !string.IsNullOrWhiteSpace(valor);

    public static bool CorreoValido(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor) || valor.Length > 254)
            return false;

        return MailAddress.TryCreate(valor, out MailAddress? correo)
            && correo.Address == valor
            && Regex.IsMatch(valor,
                @"\A[^\s@.]+(?:\.[^\s@.]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z]{2,}\z");
    }

    public static bool TelefonoValido(string valor) =>
        Regex.IsMatch(valor, @"\A[0-9]{9}\z");
}
