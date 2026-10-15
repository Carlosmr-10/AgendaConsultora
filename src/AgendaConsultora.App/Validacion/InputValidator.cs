using System.Net.Mail;
using System.Text.RegularExpressions;

namespace AgendaConsultora.Validacion;

public static partial class InputValidator
{
    public static bool EsTextoObligatorio(string? valor) => !string.IsNullOrWhiteSpace(valor);

    public static bool EsCorreoValido(string? correo)
    {
        if (string.IsNullOrWhiteSpace(correo)) return false;

        try
        {
            var direccion = new MailAddress(correo.Trim());
            string dominio = direccion.Host;
            return direccion.Address == correo.Trim()
                && dominio.Contains('.', StringComparison.Ordinal)
                && !dominio.StartsWith('.')
                && !dominio.EndsWith('.');
        }
        catch (FormatException)
        {
            return false;
        }
    }

    public static bool EsTelefonoValido(string? telefono) =>
        !string.IsNullOrWhiteSpace(telefono) && TelefonoRegex().IsMatch(telefono.Trim());

    [GeneratedRegex(@"^(?:\+34[\s-]?)?(?:\d[\s-]?){8}\d$")]
    private static partial Regex TelefonoRegex();
}
