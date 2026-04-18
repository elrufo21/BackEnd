create procedure [dbo].[listaDetalleNota]  
@Data varchar(max)  
as  
begin  
DECLARE @NotaId numeric(20),  
        @Estado varchar(80)  
DECLARE @p1 int,@p2 int  
Set @Data = LTRIM(RTrim(@Data))  
Set @p1 = CharIndex('|',@Data,0)  
Set @p2 = Len(@Data)+1  
Set @NotaId=convert(numeric(20),SUBSTRING(@Data,1,@p1-1))  
Set @Estado=SUBSTRING(@Data,@p1+1,@p2-@p1-1)  
select  
'DetalleId|NotaId|IdProducto|Cantidad|UMedida|Descripcion|PrecioCosto|PrecioUni|Importe|Estado|ValorUM|PrecioSunat|IGVPrecio|ImporteSunat|PV|SV|Codigo|CodigoSunat|Linea¬'+  
isnull((select STUFF ((select '¬'+convert(varchar,d.DetalleId)+'|'+
convert(varchar,d.NotaId)+'|'+convert(varchar,d.IdProducto)+'|'+  
convert(varchar(50),cast(d.DetalleCantidad as money),1)+'|'+d.DetalleUm+'|'+d.DetalleDescripcion+'|'+convert(varchar,d.DetalleCosto)+'|'+  
convert(varchar(50),cast(d.DetallePrecio as money),1)+'|'+convert(varchar(50),cast(d.DetalleImporte as money),1)+'|'+  
d.DetalleEstado+'|'+CONVERT(varchar,d.ValorUM)+'|'+  
  
convert(varchar,convert(decimal(18,6),d.DetallePrecio/1.18))+'|'+  
convert(varchar,(convert(decimal(18,6),d.DetallePrecio/1.18)* d.DetalleCantidad)*0.18)+'|'+  
convert(varchar,convert(decimal(18,6),d.DetallePrecio/1.18)* d.DetalleCantidad) +'|'+  
  
convert(varchar(50),CAST(d.DetalleCantidad *p.ProductoPV as money),1)+'|'+  
convert(varchar(50),CAST(d.DetalleCantidad *p.ProductoSV as money),1)+'|'+  
p.ProductoCodigo+'|'+s.CodigoSunat+'|'+s.NombreSublinea  
from DetallePedido d  
inner join Producto p  
on p.IdProducto=d.IdProducto  
inner join Sublinea s  
on s.IdSubLinea=p.IdSubLinea  
where d.NotaId=@NotaId and d.DetalleEstado=@Estado  
order by d.DetalleId asc  
for xml path('')),1,1,'')),'~')  
end  