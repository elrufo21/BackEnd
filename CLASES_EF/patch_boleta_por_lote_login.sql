CREATE PROCEDURE [dbo].[listaNotaPedido]    
@FechaInicio DATE,    
@FechaFin DATE    
AS    
BEGIN    
SET NOCOUNT ON;    
    
SELECT    
ISNULL((    
SELECT STUFF((    
SELECT    
'¬'+    
CONVERT(VARCHAR,n.NotaId)+'|'+    
ISNULL(n.NotaDocu,'')+'|'+    
    
-- CLIENTE    
CONVERT(VARCHAR,c.ClienteId)+'|'+    
ISNULL(c.ClienteRazon,'')+'|'+    
ISNULL(c.ClienteRuc,'')+'|'+    
ISNULL(c.ClienteDni,'')+'|'+    
ISNULL(c.ClienteDireccion,'')+'|'+    
ISNULL(c.ClienteTelefono,'')+'|'+    
ISNULL(c.ClienteCorreo,'')+'|'+    
ISNULL(c.ClienteEstado,'')+'|'+    
ISNULL(c.ClienteDespacho,'')+'|'+    
ISNULL(c.ClienteUsuario,'')+'|'+    
CONVERT(VARCHAR,c.ClienteFecha,103)+'|'+    
    
-- NOTA    
CONVERT(VARCHAR, n.NotaFecha, 103) + ' ' + CONVERT(VARCHAR, n.NotaFecha, 108)+'|'+    
ISNULL(n.NotaUsuario,'')+'|'+    
ISNULL(n.NotaFormaPago,'')+'|'+    
ISNULL(n.NotaCondicion,'')+'|'+    
CONVERT(VARCHAR,n.NotaFechaPago,103)+'|'+    
ISNULL(n.NotaDireccion,'')+'|'+    
ISNULL(n.NotaTelefono,'')+'|'+    
CONVERT(VARCHAR(50),CAST(n.NotaSubtotal AS MONEY),1)+'|'+    
CONVERT(VARCHAR(50),CAST(n.NotaMovilidad AS MONEY),1)+'|'+    
CONVERT(VARCHAR(50),CAST(n.NotaDescuento AS MONEY),1)+'|'+    
CONVERT(VARCHAR(50),CAST(n.NotaTotal AS MONEY),1)+'|'+    
CONVERT(VARCHAR(50),CAST(n.NotaAcuenta AS MONEY),1)+'|'+    
CONVERT(VARCHAR(50),CAST(n.NotaSaldo AS MONEY),1)+'|'+    
CONVERT(VARCHAR(50),CAST(n.NotaAdicional AS MONEY),1)+'|'+    
CONVERT(VARCHAR(50),CAST(n.NotaTarjeta AS MONEY),1)+'|'+    
CONVERT(VARCHAR(50),CAST(n.NotaPagar AS MONEY),1)+'|'+    
ISNULL(n.NotaEstado,'')+'|'+    
CONVERT(VARCHAR,n.CompaniaId)+'|'+    
ISNULL(n.NotaEntrega,'')+'|'+    
ISNULL(n.ModificadoPor,'')+'|'+    
ISNULL(n.FechaEdita,'')+'|'+    
ISNULL(n.NotaConcepto,'')+'|'+    
ISNULL(n.NotaSerie,'')+'|'+    
ISNULL(n.NotaNumero,'')+'|'+    
CONVERT(VARCHAR(50),CAST(n.NotaGanancia AS MONEY),1)+'|'+    
CONVERT(VARCHAR(50),CAST(n.ICBPER AS MONEY),1)+'|'+    
ISNULL(n.CajaId,'')+'|'+    
ISNULL(n.EntidadBancaria,'')+'|'+    
ISNULL(n.NroOperacion,'')+'|'+    
CONVERT(VARCHAR(50),CAST(n.Efectivo AS MONEY),1)+'|'+    
CONVERT(VARCHAR(50),CAST(n.Deposito AS MONEY),1)+'|'+    
ISNULL((
    SELECT TOP (1) dv.EstadoSunat
    FROM DocumentoVenta dv WITH(NOLOCK)
    WHERE dv.NotaId = n.NotaId
    ORDER BY dv.DocuId DESC
),'')    
    
FROM NotaPedido n WITH(NOLOCK)    
LEFT JOIN Cliente c WITH(NOLOCK)    
ON c.ClienteId = n.ClienteId    
    
WHERE    
n.NotaFecha >= @FechaInicio    
AND n.NotaFecha < DATEADD(DAY,1,@FechaFin)    
    
ORDER BY n.NotaId DESC    
FOR XML PATH('')    
),1,1,'')    
),'~') AS Resultado;    
    
END    
  
  
