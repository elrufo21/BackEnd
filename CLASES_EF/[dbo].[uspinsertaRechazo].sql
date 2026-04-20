create procedure [dbo].[uspinsertaRechazo]  
@ListaOrden varchar(Max)  
as  
begin  
Declare @pos int  
Declare @orden varchar(max)  
Declare @detalle varchar(max)  
Set @pos = CharIndex('[',@ListaOrden,0)  
Set @orden = SUBSTRING(@ListaOrden,1,@pos-1)  
Set @detalle = SUBSTRING(@ListaOrden,@pos+1,len(@ListaOrden)-@pos)  
Declare @pos1 int,@pos2 int,@pos3 int,@pos4 int,  
        @pos5 int,@pos6 int,@pos7 int,@pos8 int,  
        @pos9 int,@pos10 int,@pos11 int,@pos12 int,  
        @pos13 int,@pos14 int,@pos15 int,@pos16 int,  
        @pos17 int,@pos18 int,@pos19 int,@pos20 int  
 Declare @CompaniaId int,@NotaId numeric(38),@DocuDocumento varchar(60),  
         @DocuNumero varchar(60),@ClienteId numeric(20),@DocuEmision date,  
         @DocuSubTotal decimal(18,2),@DocuIgv decimal(18,2),@DocuTotal decimal(18,2),  
         @DocuUsuario varchar(60),@DocuSerie char(4),@TipoCodigo char(20),  
         @DocuAdicional decimal(18,2),@DocuAsociado varchar(80),@DocuConcepto varchar(80),  
         @DocuHASH varchar(250),@EstadoSunat varchar(80),@Letras varchar(60),  
         @CodigoSunat varchar(80),@MensajeSunat varchar(max),  
         @DocuId numeric(38),@TraeEstado varchar(80),@NotaEstado varchar(80)  
Set @pos1 = CharIndex('|',@orden,0)  
Set @pos2 = CharIndex('|',@orden,@pos1+1)  
Set @pos3 = CharIndex('|',@orden,@pos2+1)  
Set @pos4 = CharIndex('|',@orden,@pos3+1)  
Set @pos5 = CharIndex('|',@orden,@pos4+1)  
Set @pos6= CharIndex('|',@orden,@pos5+1)  
Set @pos7 = CharIndex('|',@orden,@pos6+1)  
Set @pos8 = CharIndex('|',@orden,@pos7+1)  
Set @pos9 = CharIndex('|',@orden,@pos8+1)  
Set @pos10= CharIndex('|',@orden,@pos9+1)  
Set @pos11= CharIndex('|',@orden,@pos10+1)  
Set @pos12= CharIndex('|',@orden,@pos11+1)  
Set @pos13= CharIndex('|',@orden,@pos12+1)  
Set @pos14= CharIndex('|',@orden,@pos13+1)  
Set @pos15= CharIndex('|',@orden,@pos14+1)  
Set @pos16= CharIndex('|',@orden,@pos15+1)  
Set @pos17= CharIndex('|',@orden,@pos16+1)  
Set @pos18= CharIndex('|',@orden,@pos17+1)  
Set @pos19= CharIndex('|',@orden,@pos18+1)  
Set @pos20= Len(@orden)+1  
Set @CompaniaId=convert(int,SUBSTRING(@orden,1,@pos1-1))  
Set @NotaId=convert(numeric(38),SUBSTRING(@orden,@pos1+1,@pos2-@pos1-1))  
Set @DocuDocumento=SUBSTRING(@orden,@pos2+1,@pos3-@pos2-1)  
Set @DocuNumero=SUBSTRING(@orden,@pos3+1,@pos4-@pos3-1)  
Set @ClienteId=convert(numeric(20),SUBSTRING(@orden,@pos4+1,@pos5-@pos4-1))  
Set @DocuEmision=convert(date,SUBSTRING(@orden,@pos5+1,@pos6-@pos5-1))  
Set @DocuSubTotal=convert(decimal(18,2),SUBSTRING(@orden,@pos6+1,@pos7-@pos6-1))  
Set @DocuIgv=convert(decimal(18,2),SUBSTRING(@orden,@pos7+1,@pos8-@pos7-1))  
Set @DocuTotal=convert(decimal(18,2),SUBSTRING(@orden,@pos8+1,@pos9-@pos8-1))  
Set @DocuUsuario=SUBSTRING(@orden,@pos9+1,@pos10-@pos9-1)  
Set @DocuSerie=SUBSTRING(@orden,@pos10+1,@pos11-@pos10-1)  
Set @TipoCodigo=SUBSTRING(@orden,@pos11+1,@pos12-@pos11-1)  
set @DocuAdicional=convert(decimal(18,2),SUBSTRING(@orden,@pos12+1,@pos13-@pos12-1))  
set @DocuAsociado=SUBSTRING(@orden,@pos13+1,@pos14-@pos13-1)  
set @DocuConcepto=SUBSTRING(@orden,@pos14+1,@pos15-@pos14-1)  
set @DocuHASH=SUBSTRING(@orden,@pos15+1,@pos16-@pos15-1)  
set @EstadoSunat=SUBSTRING(@orden,@pos16+1,@pos17-@pos16-1)  
set @Letras=SUBSTRING(@orden,@pos17+1,@pos18-@pos17-1)  
set @CodigoSunat=SUBSTRING(@orden,@pos18+1,@pos19-@pos18-1)  
set @MensajeSunat=SUBSTRING(@orden,@pos19+1,@pos20-@pos19-1)  
set @TraeEstado=(select top 1 n.NotaEstado from NotaPedido n where n.NotaId=@NotaId)  
if(@TraeEstado='PENDIENTE')set @NotaEstado='EMITIDO'  
else set @NotaEstado=@TraeEstado  
Begin Transaction  
insert into DocumentoVenta values(@CompaniaId,@NotaId,@DocuDocumento,@DocuNumero,  
@ClienteId,GETDATE(),@DocuEmision,'ALCONTADO','CERO CON 00/100 SOLES',0,0,0,0,  
@DocuUsuario,'RECHAZADO',@DocuSerie,@TipoCodigo,0,@DocuAsociado,  
@DocuConcepto,'',@DocuHASH,'RECHAZADO','SERVICIO','',0,  
@CodigoSunat,@MensajeSunat,'EFECTIVO','-','',0,0,'')  
Set @DocuId= @@identity  
update NotaPedido   
set CompaniaId=@CompaniaId,NotaSerie=@DocuSerie,  
NotaNumero=@DocuNumero,NotaEstado=@NotaEstado  
where NotaId=@NotaId  
   Declare Tabla Cursor For Select * From fnSplitString(@detalle,';')   
Open Tabla  
Declare @Columna varchar(max),  
  @IdProducto numeric(20),  
  @Cantidad decimal(18,2),  
  @Precio decimal(18,2),  
  @Importe decimal(18,2),  
  @DetalleNotaId numeric(38),  
  @UM varchar(80),  
  @ValorUM decimal(18,4)  
Declare @p1 int,@p2 int,@p3 int,@p4 int,  
        @p5 int,@p6 int,@p7 int  
Fetch Next From Tabla INTO @Columna  
 While @@FETCH_STATUS = 0  
 Begin  
Set @p1 = CharIndex('|',@Columna,0)  
Set @p2 = CharIndex('|',@Columna,@p1+1)  
Set @p3 = CharIndex('|',@Columna,@p2+1)  
Set @p4 = CharIndex('|',@Columna,@p3+1)  
Set @p5 = CharIndex('|',@Columna,@p4+1)  
Set @p6= CharIndex('|',@Columna,@p5+1)  
Set @p7 = Len(@Columna)+1  
Set @DetalleNotaId=Convert(numeric(38),SUBSTRING(@Columna,1,@p1-1))  
Set @IdProducto=Convert(numeric(20),SUBSTRING(@Columna,@p1+1,@p2-(@p1+1)))  
Set @Cantidad=Convert(decimal(18,2),SUBSTRING(@Columna,@p2+1,@p3-(@p2+1)))  
Set @UM=SUBSTRING(@Columna,@p3+1,@p4-(@p3+1))  
Set @Precio=Convert(decimal(18,2),SUBSTRING(@Columna,@p4+1,@p5-(@p4+1)))  
Set @Importe=Convert(decimal(18,2),SUBSTRING(@Columna,@p5+1,@p6-(@p5+1)))  
Set @ValorUM=Convert(decimal(18,4),SUBSTRING(@Columna,@p6+1,@p7-(@p6+1)))  
insert into DetalleDocumento   
values(@DocuId,@IdProducto,@Cantidad,@Precio,@Importe,@DetalleNotaId,@UM,@ValorUM)  
Fetch Next From Tabla INTO @Columna  
end  
 Close Tabla;  
 Deallocate Tabla;  
 update DetallePedido  
 set DetalleEstado='PENDIENTE'  
 where NotaId=@NotaId  
 Commit Transaction;  
select 'true'  
end  