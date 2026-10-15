from pathlib import Path
import subprocess
p=Path(__file__).resolve().parents[1]
dll=p/'bin/Debug/net10.0/AgendaConsultora.dll'
checks=[]
def run(name, lines, includes, excludes=()):
    result=subprocess.run(['dotnet',str(dll)],input='\n'.join(lines)+'\n',text=True,capture_output=True,timeout=10)
    assert result.returncode==0,(name,result.stderr)
    for text in includes: assert text in result.stdout,(name,text,result.stdout)
    for text in excludes: assert text not in result.stdout,(name,text)
    checks.append(name)
    return result.stdout
alta=['1','Ana','Zamora','ana@consultora.es','612345678']
run('Listado vacío',['2','0'],['No hay personas registradas.'])
run('Alta válida',alta+['0'],['Alta realizada correctamente.','Id: 1','Nombre: Ana Zamora'])
for correo in ['', 'correo','correo@','@dominio.com','correo@dominio','a@@dominio.es','Ana <ana@consultora.es>', 'ana.@consultora.es', '.ana@consultora.es', 'a..b@consultora.es', 'ana@-consultora.es', 'ana@consultora-.es', 'ana@consultora..es']:
    run('Correo rechazado: '+repr(correo),['1','Ana','Zamora',correo,'ana@consultora.es','612345678','0'],['Correo no válido.','Alta realizada correctamente.'])
for telefono in ['', 'abcdefghi','12345678','1234567890','12345678a']:
    run('Teléfono rechazado: '+repr(telefono),['1','Ana','Zamora','ana@consultora.es',telefono,'612345678','0'],['Teléfono no válido.','Alta realizada correctamente.'])
run('Nombres obligatorios',['1','','Ana','','Zamora','ana@consultora.es','612345678','0'],['El nombre no puede estar vacío.','Los apellidos no pueden estar vacíos.'])
otra=['1','Luis','Alonso','luis@consultora.es','623456789']
out=run('Listado ordenado',alta+otra+['2','0'],['Id: 2','Id: 1'])
last=out.split('Selecciona una opción: ')[-2]
assert last.index('Id: 2')<last.index('Id: 1')
run('Búsqueda por Id',alta+['3','1','1','0'],['Id: 1'])
run('Búsqueda parcial y mayúsculas',alta+['3','2','ZAM an','0'],['Nombre: Ana Zamora'])
run('Búsqueda sin resultados',['3','2','nadie','0'],['No se encontraron coincidencias.'])
for opcion in ['3','4','5']:
    run('Id inexistente en '+opcion,[opcion]+(['1'] if opcion=='3' else [])+['99','0'],['No existe una persona con ese Id.'])
mod=['4','1','Ana','Zamora','ana.nueva@consultora.es','634567890']
out=run('Modificación confirmada',alta+mod+['s','2','0'],['Modificación realizada correctamente.'])
assert 'Correo: ana.nueva@consultora.es' in out.split('Modificación realizada correctamente.')[1]
out=run('Modificación cancelada',alta+mod+['no','2','0'],['Modificación cancelada.'])
assert 'Correo: ana@consultora.es' in out.split('Modificación cancelada.')[1]
assert 'Correo: ana.nueva@consultora.es' not in out.split('Modificación cancelada.')[1]
run('Baja confirmada',alta+['5','1','SI','2','0'],['Baja realizada correctamente.','No hay personas registradas.'])
out=run('Baja cancelada',alta+['5','1','N','2','0'],['Baja cancelada.'])
assert 'Id: 1' in out.split('Baja cancelada.')[1]
run('Confirmación inválida',alta+['5','1','tal vez','','N','0'],['Respuesta no válida.','Baja cancelada.'],['Baja realizada correctamente.'])
run('Menú robusto',['abc','99','999999999999999999999','-1','0'],['Opción no válida.','Hasta pronto.'])
run('Id robusto',['3','1','abc','0','-1','9999999999999999','7','0'],['Opción no válida.','No existe una persona con ese Id.'])
out=run('Id no reutilizado',alta+['5','1','S']+otra+['0'],['Id: 2'])
run('EOF durante alta',['1','Ana'],['Fin de entrada.'],['Alta realizada correctamente.'])
run('EOF en confirmación',alta+mod,['Fin de entrada.'],['Modificación realizada correctamente.'])
demo=alta+otra+['2','3','2','ALON','4','1','Ana','Zamora','ana.nueva@consultora.es','612345678','S','5','2','SI','0']
(p/'evidencias/entrada-demo.txt').write_text('\n'.join(demo)+'\n')
(p/'evidencias/salida-demo.txt').write_text(run('Demostración completa',demo,['Alta realizada correctamente.','Modificación realizada correctamente.','Baja realizada correctamente.','Hasta pronto.']))
# Comprobar los resultados posteriores a la operación evita confundirlos con el alta.
for query in ['ANA', 'zam', 'Ana Zamora', 'ZAM an']:
    out=run('Coincidencia comprobada: '+query,alta+['3','2',query,'0'],[])
    result=out.split('Nombre o apellidos (admite partes): ')[1]
    assert 'Nombre: Ana Zamora' in result and 'No se encontraron coincidencias.' not in result
out=run('Id encontrado tras la consulta',alta+['3','1','1','0'],[])
assert 'Nombre: Ana Zamora' in out.split('Id de la persona: ')[1]
out=run('Varios resultados parciales',alta+['1','Ana','Alonso','otra@consultora.es','623456789','3','2','ana','0'],[])
result=out.split('Nombre o apellidos (admite partes): ')[1]
assert result.index('Nombre: Ana Alonso') < result.index('Nombre: Ana Zamora')
out=run('Revalidar correo y teléfono al modificar',alta+['4','1','Ana','Zamora','correo','ana.nueva@consultora.es','letras','634567890','S','0'],['Correo no válido.','Teléfono no válido.','Modificación realizada correctamente.'])
result=out.split('Modificación realizada correctamente.')[1]
assert 'Correo: ana.nueva@consultora.es' in result and 'Teléfono: 634567890' in result
for answer in ['s','si','Sí','  SI  ']:
    run('Confirmación afirmativa '+repr(answer),alta+['5','1',answer,'2','0'],['Baja realizada correctamente.','No hay personas registradas.'])
for answer in ['n','no','  No  ']:
    out=run('Confirmación negativa '+repr(answer),alta+['5','1',answer,'2','0'],['Baja cancelada.'])
    assert 'Nombre: Ana Zamora' in out.split('Baja cancelada.')[1]
run('Correo con subdominio y etiqueta',['1','Ana','Zamora','ana.nombre+agenda@equipo.consultora.es','612345678','0'],['Alta realizada correctamente.'])
out=run('Normalización de espacios',['1','  Ana  ',' Zamora ',' ana@consultora.es ',' 612345678 ','2','0'],['Nombre: Ana Zamora','Correo: ana@consultora.es'])
run('Submenú y búsqueda vacía',['3','hola','9','2','','   ','nadie','0'],['Opción no válida.','Introduce un texto de búsqueda.','No se encontraron coincidencias.'])
run('Estado nuevo por sesión',['2','0'],['No hay personas registradas.'])
# EOF en cada punto de una operación debe cerrar sin bloquearse ni guardar parcialmente.
for index in range(1,len(mod)+1):
    run('EOF durante modificación '+str(index),alta+mod[:index],['Fin de entrada.'],['Modificación realizada correctamente.'])
# Múltiples operaciones con duplicados permitidos y borrados intercalados.
lines=[]
for i in range(25):
    lines += ['1','Persona',f'Apellido{i:02d}',f'p{i}@consultora.es','612345678']
for id in range(1,26,2):
    lines += ['5',str(id),'S']
out=run('Secuencia de 25 altas y 13 bajas',lines+['2','0'],[])
result=out.split('Selecciona una opción: ')[-2]
ids=[int(line[4:]) for line in result.splitlines() if line.startswith('Id: ')]
assert ids==list(range(2,26,2)),ids
(p/'evidencias/verificacion.txt').write_text('Pruebas automáticas de consola mediante entrada redirigida:\n'+'\n'.join('OK - '+name for name in checks)+f'\nTotal: {len(checks)} escenarios superados.\n')
print(f'{len(checks)} escenarios superados.')
