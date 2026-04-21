# Respuestas De Falla - Endpoints De Emision (NotaController)

Fecha de referencia: `2026-04-20`  
Base URL: `http://{host}/api/v1/Nota`

## 1) Convenciones Generales (muy importante para frontend)

En `factura/boleta/credito/resumen`:

- `ok` significa `flg_rta == "1"` (respuesta tecnica del gateway).
- `aceptado` significa aceptacion real de SUNAT/OSE.
- Puede pasar: `ok = true` y `aceptado = false`.
- Siempre revisar `aceptado` para decidir exito funcional.

En `boleta/anular-individual`:

- El backend responde `409` cuando SUNAT/OSE rechaza la NC.
- Revisar `status HTTP` + `ok`.

---

## 2) POST `/factura/enviar`

### Errores HTTP

- `400 BadRequest`
  - `Payload requerido.`
  - Campos faltantes/invalidos en validacion.
  - `TIPO_PROCESO` invalido.
- `500 InternalServerError`
  - Error interno del backend al enviar.

### Falla funcional (SUNAT/OSE) con `200 OK`

```json
{
  "ok": true,
  "flg_rta": "1",
  "mensaje": "...",
  "cod_sunat": "1033",
  "msj_sunat": "...",
  "hash_cpe": "...",
  "hash_cdr": "",
  "cdr_recibido": false,
  "cdr_base64": "",
  "aceptado": false,
  "ticket": "",
  "registro_bd": {
    "ok": true,
    "accion_bd": "registrar_rechazo",
    "estado_sunat": "RECHAZADO",
    "cod_sunat": "1033",
    "msj_sunat": "...",
    "mensaje": "Se registró el documento como RECHAZADO y la nota volvió a estado PENDIENTE."
  }
}
```

### Regla frontend

- Exito: `aceptado == true`.
- Falla: `aceptado != true`.

---

## 3) POST `/boleta/enviar`

Mismo contrato de error que factura.

### Errores HTTP

- `400` por payload/validacion/`TIPO_PROCESO`.
- `500` por excepcion interna.

### Falla funcional con `200`

Misma estructura de `factura/enviar`, cambiando `cod_sunat/msj_sunat/registro_bd` segun caso.

### Regla frontend

- Exito: `aceptado == true`.
- Falla: `aceptado != true`.

---

## 4) POST `/credito/enviar`

### Errores HTTP

- `400` por payload/validacion/`TIPO_PROCESO`.
- `500` por excepcion interna.

### Falla funcional (SUNAT/OSE) con `200`

```json
{
  "ok": true,
  "flg_rta": "1",
  "mensaje": "...",
  "cod_sunat": "2116",
  "msj_sunat": "...",
  "hash_cpe": "...",
  "hash_cdr": "",
  "cdr_recibido": false,
  "cdr_base64": "",
  "aceptado": false,
  "ticket": "",
  "registro_bd": {
    "ok": false,
    "accion_bd": "sin_documento_pendiente",
    "mensaje": "No se encontró DocumentoVenta pendiente para ...",
    "cod_sunat": "2116",
    "msj_sunat": "..."
  }
}
```

### Regla frontend

- Exito: `aceptado == true`.
- Falla: `aceptado != true`.

---

## 5) POST `/boleta/anular-individual`

### Errores HTTP

- `400 BadRequest`
  - Falta `DOCU_ID` y `NRO_DOCUMENTO_MODIFICA`.
  - No se pudo construir NC valida.
  - `TIPO_PROCESO` invalido.
- `404 NotFound`
  - No se encontro boleta.
- `409 Conflict`
  - Boleta ya anulada, o rechazo SUNAT/OSE de la NC.
- `500 InternalServerError`
  - Error interno del proceso de anulacion.

### Rechazo SUNAT/OSE (caso mas importante) con `409`

```json
{
  "ok": false,
  "mensaje": "SUNAT/OSE rechazó la nota de crédito.",
  "docu_id_boleta": 4,
  "boleta": "BA01-00000003",
  "nota_credito": "BN01-00000001",
  "tipo_comprobante_modifica": "03",
  "referencia_modifica": "BA01-00000003",
  "cod_sunat": "2116",
  "msj_sunat": "...",
  "sunat": {
    "ok": true,
    "flg_rta": "1",
    "cod_sunat": "2116",
    "aceptado": false,
    "registro_bd": { "...": "..." }
  }
}
```

### Regla frontend

- Exito solo si `HTTP 200` y `ok == true`.
- Si `409`, tratar como rechazo funcional.

---

## 6) POST `/resumen/enviar` (lote boletas)

## 7) POST `/resumen/enviar-baja` (baja; si son boletas deriva a resumen)

Ambos usan el mismo contrato de respuesta normalizada.

### Errores HTTP

- `400`
  - Payload faltante.
  - Validacion de campos.
  - `TIPO_PROCESO` invalido.
  - En `enviar-baja`: mezcla no soportada de boletas y RA.
- `500`
  - Excepcion interna en envio.

### Falla funcional con `200`

```json
{
  "ok": true,
  "flg_rta": "1",
  "mensaje": "...",
  "cod_sunat": "1033",
  "msj_sunat": "...",
  "hash_cpe": "...",
  "hash_cdr": "",
  "aceptado": false,
  "ticket": "",
  "registro_bd": {
    "ok": true,
    "accion_bd": "registrar_rechazo_lote",
    "documentos_actualizados": 5,
    "estado_sunat": "RECHAZADO",
    "cod_sunat": "1033",
    "msj_sunat": "...",
    "mensaje": "Se registró el lote como RECHAZADO en DocumentoVenta."
  }
}
```

### Regla frontend

- Exito: `aceptado == true`.
- Falla: `aceptado != true`.

---

## 8) `registro_bd` en fallas (resumen)

En fallas, `registro_bd` puede devolver:

- `ok: false` + `mensaje` (no pudo registrar en BD).
- `ok: false` + `accion_bd: "sin_documento_pendiente"` (sin DV pendiente).
- `ok: false` + `accion_bd: "sin_documentos_pendientes"` (lote sin DV en estado actualizable).
- `ok: true` + `accion_bd`:
  - `mantener_pendiente`
  - `registrar_rechazo`
  - `registrar_rechazo_documento`
  - `mantener_pendiente_lote`
  - `registrar_rechazo_lote`

---

## 9) Matriz corta para frontend

1. Si `HTTP >= 500`: error tecnico backend.
2. Si endpoint = `boleta/anular-individual` y `HTTP == 409`: rechazo funcional.
3. Si respuesta trae campo `aceptado`:
   - `aceptado == true` => exito.
   - `aceptado != true` => falla funcional.
4. Mostrar siempre:
   - `mensaje`
   - `cod_sunat`
   - `msj_sunat`
   - `registro_bd.mensaje` (si existe).

---

## 10) Forzar errores (modo prueba temporal)

Se habilitó forzado por header HTTP:

- Header: `X-Force-Error`
- Endpoints soportados:
  - `/factura/enviar`
  - `/boleta/enviar`
  - `/credito/enviar`
  - `/boleta/anular-individual`
  - `/resumen/enviar`
  - `/resumen/enviar-baja`

Valores soportados:

- `http_400`
- `http_500`
- `envio_fallido`
- `sunat_1033`
- `sunat_2116`
- `sunat_2325`
- `sunat_0109`

Notas:

- En forzado `sunat_*` y `envio_fallido` **sí se ejecuta registro en BD** (igual que un rechazo/error real).
- En forzado `http_400` y `http_500` es respuesta directa de API (sin persistencia).
- Para volver a normalidad, enviar sin ese header.

Ejemplo `curl`:

```bash
curl -X POST "http://localhost:5000/api/v1/Nota/boleta/enviar" \
  -H "Content-Type: application/json" \
  -H "X-Force-Error: sunat_1033" \
  -d "{ ... }"
```
