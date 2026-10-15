"""Diez escenarios de aceptación contra la aplicación real de Fase 5."""

from pathlib import Path
import subprocess


RAIZ = Path(__file__).resolve().parents[2]
DLL = RAIZ / "Fase5/src/AgendaProfesional.Console/bin/Debug/net10.0/AgendaProfesional.Console.dll"
pruebas: list[str] = []


def ejecutar(entradas: list[str]) -> str:
    resultado = subprocess.run(
        ["dotnet", str(DLL)],
        cwd=RAIZ,
        input="\n".join(entradas) + "\n",
        text=True,
        capture_output=True,
        timeout=10,
    )
    assert resultado.returncode == 0, resultado.stderr
    return resultado.stdout


def contiene(salida: str, *textos: str) -> None:
    for texto in textos:
        assert texto in salida, (texto, salida)


def ok(nombre: str) -> None:
    pruebas.append(nombre)
    print(f"OK - {nombre}")


empresa_alfa = ["1", "Alfa", "B12345678", "912345678", "alfa@consultora.es", "Calle Uno"]
empresa_beta = ["1", "Beta", "B87654321", "923456789", "beta@consultora.es", "Calle Dos"]
persona_ana = ["1", "Ana", "Zamora", "ana@consultora.es", "612345678"]
persona_luis = ["1", "Luis", "Alonso", "luis@consultora.es", "623456789"]

salida = ejecutar(["hola", "9", "1", "0", "2", "0", "0"])
contiene(salida, "Opción no válida.", "=== PERSONAS ===", "=== EMPRESAS ===", "Hasta pronto.")
ok("01 Menús, retorno y opciones inválidas")

salida = ejecutar([
    "1", "1", "", "Ana", "", "Zamora", "correo", "ana@consultora.es",
    "letras", "612345678", "2", "0", "0",
])
contiene(
    salida,
    "El nombre no puede estar vacío.",
    "Los apellidos no pueden estar vacíos.",
    "Correo no válido.",
    "Teléfono no válido.",
    "Alta realizada correctamente.",
    "Nombre: Ana Zamora",
)
ok("02 Alta y validaciones de persona")

salida = ejecutar([
    "2", "1", "", "Alfa", "", "B12345678", "abc", "912345678",
    "correo", "alfa@consultora.es", "", "Calle Uno", "2", "0", "0",
])
contiene(
    salida,
    "El nombre comercial es obligatorio.",
    "El CIF no puede estar vacío.",
    "Teléfono no válido.",
    "Correo no válido.",
    "La dirección es obligatoria.",
    "Estado: Activa",
)
ok("03 Alta, Estado y validaciones de empresa")

salida = ejecutar(["1"] + persona_ana + persona_luis + ["2", "3", "2", "ZAM an", "0", "0"])
listado = salida.split("Selecciona una opción: Id: 2", 1)[1].split("=== PERSONAS ===", 1)[0]
assert listado.index("Nombre: Luis Alonso") < listado.index("Nombre: Ana Zamora")
contiene(salida.split("Nombre o apellidos (admite partes): ")[1], "Nombre: Ana Zamora")
ok("04 Listado ordenado y búsqueda parcial de personas")

salida = ejecutar(["2"] + empresa_beta + empresa_alfa + ["2", "3", "2", "b123", "0", "0"])
listado = salida.split("Selecciona una opción: Id: 2", 1)[1].split("=== EMPRESAS ===", 1)[0]
assert listado.index("Nombre comercial: Alfa") < listado.index("Nombre comercial: Beta")
contiene(salida.split("Texto de búsqueda: ")[1], "Nombre comercial: Alfa")
ok("05 Listado ordenado y búsqueda de empresas")

salida = ejecutar(["2"] + empresa_alfa + ["5", "1", "S", "3", "1", "1", "6", "1", "S", "0", "0"])
contiene(salida, "Estado: Inactiva", "Estado actualizado correctamente.")
assert "Estado: Activa" in salida.rsplit("Estado actualizado correctamente.", 1)[1]
ok("06 Baja lógica, conservación y reactivación")

base = ["2"] + empresa_alfa + empresa_beta + ["0", "1"] + persona_ana
salida = ejecutar(base + ["6", "1", "1", "S", "3", "1", "1", "0", "0"])
contiene(salida, "Empresa asignada correctamente.")
contiene(salida.split("Tipo de búsqueda: ")[1], "Empresa: Alfa (Id: 1)")
ok("07 Asignación y consulta de empresa de una persona")

salida = ejecutar(base + [
    "6", "1", "1", "S", "6", "1", "2", "S", "0",
    "2", "7", "1", "7", "2", "0", "0",
])
contiene(salida.split("Plantilla de Alfa (Id: 1)")[1], "Esta empresa no tiene personas asociadas.")
contiene(salida.split("Plantilla de Beta (Id: 2)")[1], "Nombre: Ana Zamora")
ok("08 Cambio de empresa y consulta de plantillas")

salida = ejecutar(base + [
    "6", "1", "1", "S", "0", "2", "5", "1", "0",
    "1", "7", "1", "S", "0", "2", "5", "1", "S", "0", "0",
])
contiene(
    salida,
    "Baja bloqueada: la empresa tiene personas asociadas.",
    "Persona desvinculada correctamente.",
    "Estado: Inactiva",
)
ok("09 Bloqueo de baja y desbloqueo tras desvincular")

salida = ejecutar(base + ["6", "1", "1", "N", "3", "1", "1", "0", "0"])
contiene(salida, "Cambio de relación cancelado.")
contiene(salida.split("Tipo de búsqueda: ")[1], "Empresa: Sin empresa asignada")
salida_eof = ejecutar(base + ["6", "1", "1"])
contiene(salida_eof, "Fin de entrada.")
assert "Empresa asignada correctamente." not in salida_eof
ok("10 Cancelación y EOF no guardan cambios pendientes")

assert len(pruebas) == 10
print("10/10 escenarios de aceptación superados.")
