CREATE procedure [dbo].[uspListaBajas]  
@Data varchar(max)  
as  
begin  
Declare @p1 int  
Declare @CompaniaId int  
Set @Data = LTRIM(RTrim(@Data))  
set @CompaniaId=@Data  
select  
'DocuId|Compania|NotaId|FechaEmision|Documento|Numero|RazonSocial|DNI|SubTotal|IGV|ICBPER|Total|Usuario|Estado¬100|80|100|115|95|130|350|90|115|115|100|115|160|125¬String|String|String|String|String|String|String|String|String|String|String|String|String|String|String¬'+  
isnull((select STUFF((select '¬'+convert(varchar,d.DocuId)+'|'+convert(varchar,d.CompaniaId)+'|'+convert(varchar,d.NotaId)+'|'+  
(Convert(char(10),d.DocuEmision,103))+'|'+d.DocuDocumento+'|'+d.docuSerie+'-'+d.DocuNumero+'|'+  
c.ClienteRazon+'|'+c.ClienteDni+'|'+  
(convert(varchar(50), CAST(d.DocuSubTotal as money), -1))+'|'+  
(convert(varchar(50), CAST(d.DocuIgv as money), -1))+'|'+  
(convert(varchar(50), CAST(d.ICBPER as money), -1))+'|'+  
(convert(varchar(50), CAST(d.DocuTotal as money), -1))+'|'+  
d.DocuUsuario+'|'+d.EstadoSunat  
from DocumentoVenta d  
inner join Cliente c  
on c.ClienteId=d.ClienteId  
where d.TipoCodigo='03'and((d.CompaniaId=@CompaniaId and DocuEstado='ANULADO' and EstadoSunat='ENVIADO'))  
order by d.DocuSerie,d.DocuNumero asc  
FOR XML path ('')),1,1,'')),'~')  
end  
GO
