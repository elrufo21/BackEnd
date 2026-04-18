create procedure [dbo].[uspTraerDV]  
@Valores varchar(max)  
as  
begin  
Declare @NotaId numeric(38),@DocuIdA numeric(38)  
Declare @a1 int,@a2 int  
Set @Valores= LTRIM(RTrim(@Valores))  
Set @a1 = CharIndex('|',@Valores,0)  
Set @a2= Len(@Valores)+1  
  
set @NotaId=SUBSTRING(@Valores,1,@a1-1)  
set @DocuIdA=SUBSTRING(@Valores,@a1+1,@a2-@a1-1)  
  
Declare @EstadoSunat varchar(40)  
  
set @EstadoSunat=isnull((select top 1 EstadoSunat  
from DocumentoVenta  
where DocuId=@DocuIdA),'')  
  
IF EXISTS(select top 1 NotaId   
from DocumentoVenta   
where NotaId=@NotaId and TipoCodigo='01') --(TipoCodigo<>'07' and TipoCodigo <>'00'))  
begin  
Declare @lista varchar(max)  
Declare @Estado varchar(20),@Asociado varchar(40),  
@TipoCodigo char(2),@Serie char(4)  
Declare @DocuId numeric(38)  
declare @1 int,@2 int,@3 int,@4 int  
set @lista=(select top 1 d.DocuEstado+'|'+d.DocuAsociado+'|'+convert(char(2),d.TipoCodigo)+'|'+convert(varchar,d.DocuId)   
from DocumentoVenta d   
where NotaId=@NotaId and TipoCodigo='01')  
Set @lista = LTRIM(RTrim(@lista))  
Set @1 = CharIndex('|',@lista,0)  
Set @2 = CharIndex('|',@lista,@1+1)  
Set @3 = CharIndex('|',@lista,@2+1)  
Set @4 = Len(@lista)+1  
set @Estado=SUBSTRING(@lista,1,@1-1)  
set @Asociado=SUBSTRING(@lista,@1+1,@2-@1-1)  
set @TipoCodigo=SUBSTRING(@lista,@2+1,@3-@2-1)  
set @DocuId=convert(numeric(38),SUBSTRING(@lista,@3+1,@4-@3-1))  
  
set @Serie=isnull((select top 1 m.SerieNC    
from MAQUINAS m   
where m.SerieNC<>''),'')  
  
if(len(@Asociado)>0 and @EstadoSunat='ENVIADO')  
select 'CANJEADO'  
else  
begin  
Declare @Data varchar(max)  
Declare @NotaConcepto varchar(20)  
Declare @Entrega varchar(40)  
Declare @FormaPago varchar(40)  
Declare @NotaEstado varchar(40)  
Declare @ConceptoOBS varchar(80)  
declare @p1 int,@p2 int,@p3 int,@p4 int,@p5 int  
set @Data=(select top 1 NotaConcepto+'|'+n.NotaEntrega+'|'+  
n.NotaFormaPago+'|'+n.NotaEstado+'|'+n.ConceptoOBS   
from NotaPedido n where n.NotaId=@NotaId)  
Set @Data = LTRIM(RTrim(@Data))  
Set @p1 = CharIndex('|',@Data,0)  
Set @p2 = CharIndex('|',@Data,@p1+1)  
Set @p3= CharIndex('|',@Data,@p2+1)  
Set @p4= CharIndex('|',@Data,@p3+1)  
Set @p5 = Len(@Data)+1  
set @NotaConcepto=SUBSTRING(@Data,1,@p1-1)  
set @Entrega=SUBSTRING(@Data,@p1+1,@p2-@p1-1)  
set @FormaPago=SUBSTRING(@Data,@p2+1,@p3-@p2-1)  
set @NotaEstado=SUBSTRING(@Data,@p3+1,@p4-@p3-1)  
set @ConceptoOBS=SUBSTRING(@Data,@p4+1,@p5-@p4-1)  
select  
isnull((select STUFF((select top 1'¬'+d.DocuCondicion+'|'+d.EstadoSunat+'|'+d.DocuDocumento+'|'+  
d.DocuSerie+'-'+d.DocuNumero+'|'+convert(varchar,d.ClienteId)+'|'+  
c.ClienteRazon+'|'+c.ClienteRuc+'|'+c.ClienteDni+'|'+c.ClienteDireccion+'|'+  
(Convert(char(10),d.DocuEmision,103))+'|'+d.DocuUsuario+'|'+  
CONVERT(VarChar(50), cast(d.DocuTotal as money ), 1)+'|'+convert(varchar,d.CompaniaId)+'|'+  
(select dbo.genenerarNroFactura(@Serie,d.CompaniaId,'NOTA DE CREDITO'))+'|'+  
@Entrega+'|'+@FormaPago+'|'+@NotaEstado+'|'+convert(varchar,d.NotaId)+'|'+  
convert(varchar,d.DocuId)+'|'+@Serie+'|'+co.CompaniaRazonSocial+'|'+co.CompaniaComercial+'|'+  
co.CompaniaRUC+'|'+co.CompaniaUserSecun+'|'+co.ComapaniaPWD+'|'+co.CompaniaPFX+'|'+  
co.CompaniaClave+'|'+co.CompaniaEmail+'|'+co.CompaniaDireccion+'|'+co.CompaniaTelefono+'|'+  
co.CompaniaNomUBG+'|'+co.CompaniaCodigoUBG+'|'+co.CompaniaDistrito+'|'+co.CompaniaDirecSunat+'|'+  
d.DocuOperacion+'|'+c.ClienteCodigo+'|'+d.DocuTransaccion+'|'+  
CONVERT(VarChar(50), cast(d.DocuSaldo as money ), 1)+'|'+  
CONVERT(VarChar(50), cast(d.DocuAdicional as money ), 1)+'|'+  
CONVERT(VarChar(50), cast(d.DocuSubTotal as money ), 1)+'|'+  
CONVERT(VarChar(50), cast(d.DocuIgv as money ), 1)+'|'+@ConceptoOBS+'|'+d.EntidadBancaria+'|'+  
CONVERT(VarChar(50), cast(d.Efectivo as money ), 1)+'|'+  
CONVERT(VarChar(50), cast(d.Deposito as money ), 1)  
from DocumentoVenta d  
inner join Cliente c  
on c.ClienteId=d.ClienteId  
inner join Compania co  
on co.CompaniaId=d.CompaniaId  
where d.NotaId=@NotaId and TipoCodigo='01'  
for xml path('')),1,1,'')),'~')+'['+  
'Cantidad|UM|Descripcion|Precio|Importe|DetalleId|IdProducto|valorUM|PrecioSunat|IGVPrecio|ImporteSunat|Codigo|Linea|CodSunat¬103|100|350|110|115|100|100|100|100|100|100|100|100|100¬String|String|String|String|String|String|String|String|String|String|Str
ing|String|String|String¬'+  
isnull((select STUFF((select '¬'+CONVERT(VarChar(50), cast(d.DetalleCantidad as money ), 1)+'|'+  
d.DetalleUM+'|'+p.ProductoNombre+' '+p.ProductoMarca+'|'+  
CONVERT(VarChar(50), cast(d.DetallPrecio as money ), 1)+'|'+  
CONVERT(VarChar(50), cast(d.DetalleImporte as money ), 1)+'|'+  
convert(varchar,d.DetalleNotaId)+'|'+convert(varchar,d.IdProducto)+'|'+  
convert(varchar,d.ValorUM)+'|'+  
  
convert(varchar,convert(decimal(18,6),d.DetallPrecio/1.18))+'|'+  
convert(varchar,(convert(decimal(18,6),d.DetallPrecio/1.18)* d.DetalleCantidad)*0.18)+'|'+  
convert(varchar,convert(decimal(18,6),d.DetallPrecio/1.18)* d.DetalleCantidad) +'|'+  
  
p.ProductoCodigo+'|'+s.NombreSublinea+'|'+s.CodigoSUNAT  
from DetalleDocumento d  
inner join Producto p  
on p.IdProducto=d.IdProducto  
inner join Sublinea s  
on s.IdSubLinea=p.IdSubLinea  
where DocuId=@DocuId  
order by d.DetalleId asc  
for xml path('')),1,1,'')),'~')  
end  
end  
else  
begin  
select 'NO EXISTE'  
end  
end  