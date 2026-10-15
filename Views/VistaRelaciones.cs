using AgendaConsultora.Models;

namespace AgendaConsultora.Views;

internal sealed class VistaRelaciones
{
    public int LeerEmpresaId() => Entrada.Numero("Id de la empresa a asignar: ", 1, int.MaxValue);
    public void EmpresaInexistente() => Console.WriteLine("No existe una empresa con ese Id.");
    public void EmpresaInactiva() => Console.WriteLine("No se puede asignar una empresa inactiva. Reactívala primero.");
    public void YaAsignada() => Console.WriteLine("La persona ya pertenece a esa empresa.");
    public void SinEmpresa() => Console.WriteLine("La persona ya está sin empresa asignada.");
    public void MostrarDestino(Empresa empresa) => Console.WriteLine($"Empresa seleccionada: {empresa.NombreComercial} (Id: {empresa.Id})");
    public bool ConfirmarAsignacion() => Entrada.Confirmar("¿Asignar esta empresa a la persona?");
    public bool ConfirmarDesvinculacion() => Entrada.Confirmar("¿Desvincular la persona conservando sus datos?");
    public void Cancelada() => Console.WriteLine("Cambio de relación cancelado.");
    public void ResultadoAsignacion(bool correcto) => Console.WriteLine(correcto
        ? "Empresa asignada correctamente." : "No se pudo asignar la empresa.");
    public void ResultadoDesvinculacion(bool correcto) => Console.WriteLine(correcto
        ? "Persona desvinculada correctamente." : "No se pudo desvincular la persona.");
}
