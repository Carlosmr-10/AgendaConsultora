from pathlib import Path
import subprocess
p = Path(__file__).resolve().parents[1]
dll = p/'bin/Debug/net10.0/AgendaConsultora.dll'
checks = []
def run(name, lines, expected):
    result = subprocess.run(['dotnet',str(dll)],input='\n'.join(lines)+'\n',text=True,capture_output=True,timeout=10)
    assert result.returncode == 0, result.stderr
    for text in expected:
        assert text in result.stdout, (name,text,result.stdout)
    checks.append(name)
    return result.stdout
alta = ['1','Zeta','B12345678','912345678','zeta@consultora.es','Calle Uno']
mod = ['4','1','Alfa','B12345678','912345678','alfa@consultora.es','Calle Dos']
run('Navegación sin cerrar',['1','0','2','0','1','0','0'],['=== PERSONAS ===','=== EMPRESAS ===','Hasta pronto.'])
run('Menús inválidos',['hola','9','2','hola','99','0','0'],['Opción no válida.','Hasta pronto.'])
run('Listado vacío',['2','2','0','0'],['No hay empresas que mostrar.'])
run('Campos obligatorios y formatos',['2','1','','Zeta','','B1','abc','912345678','correo','zeta@consultora.es','','Calle Uno','0','0'],['El nombre comercial es obligatorio.','El CIF no puede estar vacío.','Teléfono no válido.','Correo no válido.','La dirección es obligatoria.','Alta de empresa realizada correctamente.'])
for tipo,texto in [('1','1'),('2','ZET'),('2','b123')]:
    out=run('Buscar '+tipo+': '+texto,['2']+alta+['3',tipo,texto,'0','0'],[])
    assert 'Nombre comercial: Zeta' in out.split('Tipo de búsqueda: ')[1]
for opcion in ['4','5','6']:
    run('Id inexistente '+opcion,['2',opcion,'99','0','0'],['No existe una empresa con ese Id.'])
out=run('Modificar confirmada',['2']+alta+mod+['S','0','0'],['Empresa modificada correctamente.'])
assert 'Nombre comercial: Alfa' in out.split('Empresa modificada correctamente.')[1]
out=run('Modificar cancelada',['2']+alta+mod+['N','2','0','0'],['Modificación cancelada.'])
assert 'Nombre comercial: Zeta' in out.split('Modificación cancelada.')[1]
out=run('Baja cancelada',['2']+alta+['5','1','quizá','N','2','0','0'],['Respuesta no válida.','Cambio de estado cancelado.'])
assert 'Estado: Activa' in out.split('Cambio de estado cancelado.')[1]
out=run('Baja conserva registro y reactivación',['2']+alta+['5','1','S','3','1','1','6','1','SI','0','0'],['Estado: Inactiva','Estado actualizado correctamente.'])
assert 'Estado: Activa' in out.rsplit('Estado actualizado correctamente.',1)[1]
out=run('Ordenación',['2']+alta+['1','Alfa','B2','923456789','alfa@consultora.es','Calle Dos','2','0','0'],[])
listado=out.split('Selecciona una opción: ')[-3]
assert listado.index('Nombre comercial: Alfa') < listado.index('Nombre comercial: Zeta')
persona=['1','Ana','Zamora','ana@consultora.es','612345678']
out=run('Personas CRUD y conservación al cambiar menú',['1']+persona+['0','2','0','1','3','2','ZAM','4','1','Ana','Zamora','nueva@consultora.es','612345678','S','5','1','S','2','0','0'],['Nombre: Ana Zamora','Modificación realizada correctamente.','Baja realizada correctamente.','No hay personas registradas.','Hasta pronto.'])
for i in range(1,len(alta)+1):
    run('EOF alta empresa '+str(i),['2']+alta[:i],['Fin de entrada.'])
demo=['2']+alta+mod+['S','5','1','S','2','6','1','S','0','1']+persona+['2','0','0']
out=run('Demostración completa',demo,['Alta de empresa realizada correctamente.','Empresa modificada correctamente.','Estado: Inactiva','Alta realizada correctamente.','Hasta pronto.'])
(p/'evidencias/entrada-demo.txt').write_text('\n'.join(demo)+'\n')
(p/'evidencias/salida-demo.txt').write_text(out)
(p/'evidencias/verificacion.txt').write_text('\n'.join('OK: '+c for c in checks)+f'\n{len(checks)} escenarios de consola superados.\n')
print(f'{len(checks)} escenarios de consola superados.')
