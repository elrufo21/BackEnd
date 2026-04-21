ALTER PROCEDURE [dbo].[uspObtenerNotaPedido]
 @Valores VARCHAR(MAX)
AS
BEGIN
 SET NOCOUNT ON;

 DECLARE
 @IdNota INT,
 @Cabecera VARCHAR(MAX),
 @Detalle VARCHAR(MAX),
 @EstadoSunatDocu VARCHAR(80)

 SET @Valores = LTRIM(RTRIM(ISNULL(@Valores,'')))
 SET @IdNota = TRY_CONVERT(INT,@Valores)

 IF ISNULL(@IdNota,0)=0
 BEGIN
  SELECT 'FORMATO_INVALIDO' Resultado
  RETURN
 END

 IF NOT EXISTS(
  SELECT 1
  FROM NotaPedido WITH(NOLOCK)
  WHERE NotaId=@IdNota
 )
 BEGIN
  SELECT '~' Resultado
  RETURN
 END

 /* CABECERA + CLIENTE */

 SELECT
 @Cabecera =
  CONVERT(VARCHAR,np.NotaId)+'|' +
  ISNULL(np.NotaDocu,'')+'|' +
  CONVERT(VARCHAR,np.ClienteId)+'|' +
  CONVERT(VARCHAR,np.NotaFecha,23)+'|' +
  ISNULL(np.NotaUsuario,'')+'|' +
  ISNULL(np.NotaFormaPago,'')+'|' +
  ISNULL(np.NotaCondicion,'')+'|' +
  ISNULL(CONVERT(VARCHAR,np.NotaFechaPago,23),'')+'|' +
  ISNULL(np.NotaDireccion,'')+'|' +
  ISNULL(np.NotaTelefono,'')+'|' +
  CONVERT(VARCHAR(50),CAST(ISNULL(np.NotaSubtotal,0) AS DECIMAL(18,2)))+'|' +
  CONVERT(VARCHAR(50),CAST(ISNULL(np.NotaMovilidad,0) AS DECIMAL(18,2)))+'|' +
  CONVERT(VARCHAR(50),CAST(ISNULL(np.NotaDescuento,0) AS DECIMAL(18,2)))+'|' +
  CONVERT(VARCHAR(50),CAST(ISNULL(np.NotaTotal,0) AS DECIMAL(18,2)))+'|' +
  CONVERT(VARCHAR(50),CAST(ISNULL(np.NotaAcuenta,0) AS DECIMAL(18,2)))+'|' +
  CONVERT(VARCHAR(50),CAST(ISNULL(np.NotaSaldo,0) AS DECIMAL(18,2)))+'|' +
  CONVERT(VARCHAR(50),CAST(ISNULL(np.NotaAdicional,0) AS DECIMAL(18,2)))+'|' +
  CONVERT(VARCHAR(50),CAST(ISNULL(np.NotaTarjeta,0) AS DECIMAL(18,2)))+'|' +
  CONVERT(VARCHAR(50),CAST(ISNULL(np.NotaPagar,0) AS DECIMAL(18,2)))+'|' +
  ISNULL(np.NotaEstado,'')+'|' +
  CONVERT(VARCHAR,np.CompaniaId)+'|' +
  ISNULL(np.NotaEntrega,'')+'|' +
  ISNULL(np.ModificadoPor,'')+'|' +
  ISNULL(np.FechaEdita,'')+'|' +
  ISNULL(np.NotaConcepto,'')+'|' +
  ISNULL(np.NotaSerie,'')+'|' +
  ISNULL(np.NotaNumero,'')+'|' +
  CONVERT(VARCHAR(50),CAST(ISNULL(np.NotaGanancia,0) AS DECIMAL(18,2)))+'|' +
  CONVERT(VARCHAR(50),CAST(ISNULL(np.ICBPER,0) AS DECIMAL(18,2)))+'|' +
  ISNULL(np.CajaId,'')+'|' +
  ISNULL(np.EntidadBancaria,'')+'|' +
  ISNULL(np.NroOperacion,'')+'|' +

  /* CLIENTE */

  CONVERT(VARCHAR,c.ClienteId)+'|' +
  ISNULL(c.ClienteRazon,'')+'|' +
  ISNULL(c.ClienteRuc,'')+'|' +
  ISNULL(c.ClienteDni,'')+'|' +
  ISNULL(c.ClienteDireccion,'')+'|' +
  ISNULL(c.ClienteTelefono,'')+'|' +
  ISNULL(c.ClienteCorreo,'')+'|' +
  ISNULL(c.ClienteEstado,'')+'|' +
  ISNULL(c.ClienteDespacho,'')+'|' +
  ISNULL(c.ClienteUsuario,'')+'|' +
  ISNULL(CONVERT(VARCHAR,c.ClienteFecha,23),'')
 FROM NotaPedido np WITH(NOLOCK)
 LEFT JOIN Cliente c WITH(NOLOCK)
  ON c.ClienteId=np.ClienteId
 WHERE np.NotaId=@IdNota

 SELECT TOP (1)
  @EstadoSunatDocu = ISNULL(dv.EstadoSunat,'')
 FROM DocumentoVenta dv WITH(NOLOCK)
 WHERE dv.NotaId=@IdNota
 ORDER BY dv.DocuId DESC


 /* DETALLE */

 SELECT
 @Detalle = STUFF((
  SELECT
   ';DET|' +
   CONVERT(VARCHAR,d.DetalleId)+'|' +
   CONVERT(VARCHAR,d.NotaId)+'|' +
   CONVERT(VARCHAR,d.IdProducto)+'|' +
   CONVERT(VARCHAR(50),CAST(ISNULL(d.DetalleCantidad,0) AS DECIMAL(18,2)))+'|' +
   ISNULL(d.DetalleUm,'')+'|' +
   ISNULL(d.DetalleDescripcion,'')+'|' +
   CONVERT(VARCHAR(50),CAST(ISNULL(d.DetalleCosto,0) AS DECIMAL(18,2)))+'|' +
   CONVERT(VARCHAR(50),CAST(ISNULL(d.DetallePrecio,0) AS DECIMAL(18,2)))+'|' +
   CONVERT(VARCHAR(50),CAST(ISNULL(d.DetalleImporte,0) AS DECIMAL(18,2)))+'|' +
   ISNULL(d.DetalleEstado,'')+'|' +
   CONVERT(VARCHAR(50),CAST(ISNULL(d.CantidadSaldo,0) AS DECIMAL(18,2)))+'|' +
   REPLACE(ISNULL(@EstadoSunatDocu,''),'|',' ')
  FROM DetallePedido d WITH(NOLOCK)
  WHERE d.NotaId=@IdNota
  ORDER BY d.DetalleId
  FOR XML PATH('')
 ),1,1,'')


 SELECT ISNULL(@Cabecera + '[' + ISNULL(@Detalle,''),'~') Resultado

END
