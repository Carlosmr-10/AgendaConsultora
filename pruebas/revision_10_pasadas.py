"""Auditoría reproducible de los requisitos de Fase 4."""

from pathlib import Path
import re
import subprocess


RAIZ = Path(__file__).resolve().parents[2]
FASE = RAIZ / "Fase4"
RESULTADOS: list[str] = []


def ejecutar(comando: list[str], entrada: str | None = None) -> str:
    resultado = subprocess.run(
        comando,
        cwd=RAIZ,
        input=entrada,
        text=True,
        capture_output=True,
        timeout=60,
    )
    if resultado.returncode != 0:
        raise AssertionError(
            f"Falló {' '.join(comando)}\n{resultado.stdout}\n{resultado.stderr}"
        )
    return resultado.stdout + resultado.stderr


def registrar(numero: int, nombre: str) -> None:
    RESULTADOS.append(f"{numero}. OK - {nombre}")
    print(RESULTADOS[-1])


def fuentes(carpeta: str) -> list[Path]:
    return sorted((FASE / carpeta).rglob("*.cs"))


# 1. La refactorización debe conservar el comportamiento completo de Fase 3.
entrada = (RAIZ / "Fase3/evidencias/entrada-relaciones.txt").read_text()
salida_fase3 = ejecutar(
    ["dotnet", "Fase3/bin/Debug/net10.0/AgendaConsultora.dll"], entrada
)
salida_fase4 = ejecutar(
    ["dotnet", "Fase4/bin/Debug/net10.0/AgendaConsultora.dll"], entrada
)
salida_fase4_normalizada = salida_fase4.replace(
    "=== AGENDA CONSULTORA — FASE 4 ===", "=== AGENDA CONSULTORA ==="
)
assert salida_fase3 == salida_fase4_normalizada
registrar(1, "Alcance heredado de Fases 1, 2 y 3 sin cambios funcionales")

# 2. Flujos reales de consola, incluidas entradas inválidas, cancelaciones y EOF.
salida = ejecutar(["python3", "Fase4/pruebas/verificar.py"])
assert "23 escenarios de consola superados." in salida
registrar(2, "23 escenarios de consola y manejo robusto de entradas")

# 3. Reglas de negocio del modelo y relaciones.
salida = ejecutar(
    [
        "dotnet",
        "run",
        "--project",
        "Fase4/pruebas/Servicios/Servicios.csproj",
        "--no-restore",
    ]
)
assert "Total: 31 comprobaciones de servicios superadas." in salida
registrar(3, "31 comprobaciones de gestión, Estado y relaciones")

# 4. Compilación Debug tratando cualquier advertencia como error.
salida = ejecutar(
    [
        "dotnet", "build", "Fase4/AgendaConsultora.csproj", "--no-restore",
        "--configuration", "Debug", "--nologo", "-warnaserror",
    ]
)
assert "0 Warning(s)" in salida and "0 Error(s)" in salida
registrar(4, "Compilación Debug con 0 advertencias y 0 errores")

# 5. Compilación Release independiente.
salida = ejecutar(
    [
        "dotnet", "build", "Fase4/AgendaConsultora.csproj", "--no-restore",
        "--configuration", "Release", "--nologo", "-warnaserror",
    ]
)
assert "0 Warning(s)" in salida and "0 Error(s)" in salida
registrar(5, "Compilación Release con 0 advertencias y 0 errores")

# 6. Main breve y coordinación descompuesta en funciones.
programa = (FASE / "Program.cs").read_text()
agenda = (FASE / "Controllers/ControladorAgenda.cs").read_text()
assert re.search(
    r"private static void Main\(\)\s*=>\s*ControladorAgenda\.Iniciar\(\);",
    programa,
)
assert programa.count("ControladorAgenda.Iniciar()") == 1
for metodo in (
    "Iniciar", "CrearControlador", "Ejecutar", "EjecutarMenuPrincipal",
    "ProcesarOpcionPrincipal",
):
    assert metodo in agenda
registrar(6, "Main de una llamada y flujo principal dividido por responsabilidad")

# 7. Presentación, lectura, validación y gestión permanecen separadas.
for archivo in fuentes("Controllers") + fuentes("Models"):
    assert "Console." not in archivo.read_text(), archivo
vistas = "\n".join(archivo.read_text() for archivo in fuentes("Views"))
entrada_cs = (FASE / "Views/Entrada.cs").read_text()
validador = (FASE / "Models/Validation/Validador.cs").read_text()
assert "Console." in vistas
assert "int.TryParse" in entrada_cs
assert "int.Parse" not in entrada_cs and "Convert.ToInt" not in entrada_cs
assert "Console." not in validador
registrar(7, "Menús, entrada, validación y gestión en componentes separados")

# 8. Funciones reutilizables y ausencia de la duplicación señalada.
controladores = "\n".join(archivo.read_text() for archivo in fuentes("Controllers"))
operaciones = (FASE / "Controllers/OperacionesControlador.cs").read_text()
assert "BuscarPorId<T>" in operaciones and "MostrarColeccion<T>" in operaciones
assert controladores.count("OperacionesControlador.BuscarPorId") == 2
assert controladores.count("OperacionesControlador.MostrarColeccion") == 3
for nombre in (
    "DarDeAlta", "BuscarPorId", "Modificar", "DarDeBaja",
    "CambiarEstado", "AsignarEmpresa", "DesvincularEmpresa", "VerPlantilla",
):
    assert nombre in controladores
registrar(8, "Búsqueda/listados reutilizables y operaciones con nombres explicativos")

# 9. Parámetros y retornos coherentes, sin tuplas anónimas ni estado global mutable.
datos_persona = (FASE / "Models/DatosPersona.cs").read_text()
datos_empresa = (FASE / "Models/DatosEmpresa.cs").read_text()
servicios = "\n".join(archivo.read_text() for archivo in fuentes("Models/Services"))
assert "record DatosPersona" in datos_persona
assert "record DatosEmpresa" in datos_empresa
assert "IReadOnlyList<" in servicios and "? BuscarPorId" in servicios
assert "public (" not in vistas
campos_estaticos = re.findall(
    r"^\s*private static (?!readonly)[^(\n;]+;\s*$",
    "\n".join(archivo.read_text() for archivo in fuentes("Models")),
    flags=re.MULTILINE,
)
assert not campos_estaticos
assert "private static readonly StringComparer Orden" in servicios
registrar(9, "Tipos de datos claros, retornos seguros y sin globales mutables")

# 10. Evidencia y políticas esenciales documentadas y presentes en el código.
readme = (FASE / "README.md").read_text()
gestor_empresas = (FASE / "Models/Services/GestorEmpresas.cs").read_text()
persona = (FASE / "Models/Persona.cs").read_text()
for termino in (
    "Reparto de responsabilidades", "Program.Main", "Entrada.Texto",
    "BuscarPorId<T>", "IReadOnlyList<T>", "No hay variables globales mutables",
):
    assert termino in readme
assert "empresa!.Estado = false" in gestor_empresas
assert "empresas.Remove" not in gestor_empresas
assert "int? EmpresaId" in persona
assert all((RAIZ / f"Fase{numero}").is_dir() for numero in range(1, 5))
registrar(10, "README completo, baja lógica y entregas separadas por fases")

assert len(RESULTADOS) == 10
print("10/10 revisiones diferenciadas superadas.")
