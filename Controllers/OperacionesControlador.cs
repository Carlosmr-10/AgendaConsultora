namespace AgendaConsultora.Controllers;

internal static class OperacionesControlador
{
    public static T? BuscarPorId<T>(
        Func<int> leerId,
        Func<int, T?> buscar,
        Action mostrarNoExiste) where T : class
    {
        T? elemento = buscar(leerId());
        if (elemento is null)
            mostrarNoExiste();

        return elemento;
    }

    public static void MostrarColeccion<T>(
        IReadOnlyList<T> elementos,
        Action mostrarVacio,
        Action<T> mostrarElemento)
    {
        if (elementos.Count == 0)
        {
            mostrarVacio();
            return;
        }

        foreach (T elemento in elementos)
            mostrarElemento(elemento);
    }
}
