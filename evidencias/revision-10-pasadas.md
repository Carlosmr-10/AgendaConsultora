# Revisión en diez pasadas — 3 de octubre de 2026

Referencia: las dos páginas de «Agenda Consultora - Fase 1.pdf» y los requisitos
adicionales del usuario. Cada pasada tiene un foco distinto; no consiste en repetir
la misma compilación diez veces.

| Pasada | Foco y comprobaciones | Resultado |
|---|---|---|
| 1 | Contraste del PDF: solo personas, menú, altas, listado, búsqueda, modificación, baja, validaciones y evidencia. | Todos tienen implementación; empresas quedan documentadas para otra fase. |
| 2 | Arquitectura: proyecto independiente en Fase1, modelo encapsulado, List privada, gestor sin Console, auxiliares reutilizables. | Conforme. Los resultados ordenados son copias; las propiedades no tienen setters públicos. |
| 3 | Flujo: opciones no numéricas, fuera de rango, negativas y desbordamiento; submenú inválido y fin de entrada. | Se vuelve a preguntar o se cierra limpiamente al terminar la entrada; no hay excepciones sin controlar por estas entradas. |
| 4 | Alta e identidad: campos vacíos, espacios exteriores, datos registrados, Id sucesivos y no reutilización tras baja. | Conforme; altas incompletas no se guardan. Se prueban también 25 altas consecutivas. |
| 5 | Formatos: correos obligatorios y mal formados, puntos en parte local, dominio incorrecto, etiquetas y subdominios; teléfono vacío, letras y longitudes incorrectas. | Se detectó y corrigió la aceptación de ana.@consultora.es. Se reforzaron los límites de las expresiones regulares. |
| 6 | Listado: vacío, orden por apellidos/nombre y datos completos; estado final tras 25 altas y 13 bajas. | Conforme; la secuencia final contiene exactamente los Id pares del 2 al 24. |
| 7 | Búsqueda: Id existente/inexistente, nombre completo, fragmentos, mayúsculas, varias coincidencias y búsqueda vacía. | Conforme; se comprueba específicamente la salida posterior a la consulta, no solo el texto del alta anterior. |
| 8 | Modificación: localización, datos actuales, revalidación, propuesta, confirmación, cancelación y fin de entrada en cada paso. | Conforme; cancelar mantiene el correo original y confirmar muestra el nuevo. |
| 9 | Baja: localización, ficha completa, confirmaciones afirmativas/negativas, respuesta inválida y registro inexistente. | Conforme; solo una respuesta afirmativa válida elimina. |
| 10 | Entrega: compilación Debug y Release con advertencias como errores, documentación, comandos, alcance futuro y demostración reproducible. | Ambas compilaciones con 0 errores y 0 advertencias. La salida Release coincide exactamente con la evidencia Debug. |

## Evidencia reproducible

Desde la raíz del proyecto:

```bash
dotnet build Fase1/AgendaConsultora.csproj --no-restore --nologo -warnaserror
python3 Fase1/pruebas/verificar.py
dotnet build Fase1/AgendaConsultora.csproj --no-restore --configuration Release --nologo -warnaserror
```

En una copia nueva, ejecutar primero `dotnet restore Fase1/AgendaConsultora.csproj`.
El script contiene 64 escenarios con aserciones, límite de tiempo y comprobación del
código de salida. El resultado detallado está en `verificacion.txt`. La demostración
se conserva en `entrada-demo.txt` y `salida-demo.txt`.

## Límites previstos de la entrega

- Los datos y el contador se reinician al cerrar: no hay persistencia en esta fase.
- El teléfono admite nueve dígitos sin prefijos ni separadores, como permite el requisito.
- La validación del correo es de formato básico y no comprueba la existencia del buzón.
- La búsqueda ignora mayúsculas; no promete equivalencia entre letras con y sin tilde.
- La comprobación se ha realizado con .NET 10; no se ha ejecutado bajo .NET 8.

No se han encontrado incumplimientos pendientes en los requisitos revisados.
Las pruebas aportan evidencia del comportamiento comprobado, no una garantía absoluta
de ausencia de cualquier posible defecto.
