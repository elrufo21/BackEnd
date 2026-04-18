alter procedure [dbo].[LDdocumentos]
    @Mes int = null,
    @ANNO int = null,
    @Data varchar(64) = null,
    @FechaInicio date = null,
    @FechaFin date = null
as
begin
    set nocount on;

    declare @cabecera varchar(max) = 'Fecha|Documento|NroDoc|Cliente|RUC|DNI|SubTotal|IGV|ICBPER|Total|Usuario|Estado|Referencia|Codigo|Mensaje|Condicion|FormaPago|Entidad|NroOperacion|Efectivo|Deposito';
    declare @anchos varchar(max) = '85|90|110|250|80|80|115|115|90|115|150|150|110|0|0|0|0|0|0|0|0';

    declare @fi date;
    declare @ff date;
    declare @sep int;
    declare @sIni varchar(32);
    declare @sFin varchar(32);
    declare @detalle varchar(max);

    -- Prioridad 1: rango explícito
    if @FechaInicio is not null and @FechaFin is not null
    begin
        set @fi = @FechaInicio;
        set @ff = @FechaFin;
    end
    -- Prioridad 2: formato legado "@Data" => "fechaInicio|fechaFin"
    else if @Data is not null and ltrim(rtrim(@Data)) <> ''
    begin
        set @sep = charindex('|', @Data);
        set @sIni = ltrim(rtrim(case when @sep > 0 then left(@Data, @sep - 1) else @Data end));
        set @sFin = ltrim(rtrim(case when @sep > 0 then substring(@Data, @sep + 1, 8000) else @sIni end));

        set @fi = coalesce(
            try_convert(date, @sIni, 101), -- MM/dd/yyyy
            try_convert(date, @sIni, 103), -- dd/MM/yyyy
            try_convert(date, @sIni, 23),  -- yyyy-MM-dd
            try_convert(date, @sIni)
        );
        set @ff = coalesce(
            try_convert(date, @sFin, 101),
            try_convert(date, @sFin, 103),
            try_convert(date, @sFin, 23),
            try_convert(date, @sFin)
        );
    end
    -- Prioridad 3: compatibilidad previa Mes/Año
    else if @Mes is not null and @ANNO is not null
    begin
        set @fi = datefromparts(@ANNO, @Mes, 1);
        set @ff = eomonth(@fi);
    end

    if @fi is null or @ff is null
    begin
        select @cabecera + '¬' + @anchos;
        return;
    end

    if @fi > @ff
    begin
        declare @tmp date = @fi;
        set @fi = @ff;
        set @ff = @tmp;
    end

    set @detalle = (
        select STUFF((select '¬'+(convert(char(10), d.DocuEmision, 103))+'|'+
        d.DocuDocumento+'|'+
        convert(varchar, d.DocuSerie+'-'+d.DocuNumero)+'|'+
        d.ClienteRazon+'|'+isnull(d.ClienteRuc, '')+'|'+isnull(d.ClienteDni, '')+'|'+
        case when (d.TipoCodigo = '07') then
            '-'+convert(varchar(50), cast(d.DocuSubTotal as money), 1)
        else
            convert(varchar(50), cast(d.DocuSubTotal as money), 1) end+'|'+
        case when (d.TipoCodigo = '07') then
            '-'+convert(varchar(50), cast(d.DocuIgv as money), 1)
        else
            convert(varchar(50), cast(d.DocuIgv as money), 1) end+'|'+
        case when (d.TipoCodigo = '07') then
            '-'+convert(varchar(50), cast(d.ICBPER as money), 1)
        else
            convert(varchar(50), cast(d.ICBPER as money), 1) end+'|'+
        case when (d.TipoCodigo = '07') then
            '-'+convert(varchar(50), cast(d.DocuTotal as money), 1)
        else
            convert(varchar(50), cast(d.DocuTotal as money), 1) end+'|'+
        d.DocuUsuario+'|'+d.DocuEstado+'|'+d.DocuNroGuia+'|'+d.CodigoSunat+'|'+replace(d.MensajeSunat, '|', ' ')+'|'+
        d.DocuCondicion+'|'+d.FormaPago+'|'+d.EntidadBancaria+'|'+d.NroOperacion+'|'+
        convert(varchar(50), cast(d.Efectivo as money), 1)+'|'+
        convert(varchar(50), cast(d.Deposito as money), 1)
        from DocumentoVenta d
        where d.DocuEmision >= @fi
          and d.DocuEmision < dateadd(day, 1, @ff)
          and d.DocuDocumento <> 'PROFORMA V'
          and d.DocuDocumento <> 'PROFORMA'
        order by d.DocuEmision asc, d.DocuSerie+'-'+d.DocuNumero asc
        FOR XML PATH('')), 1, 1, '')
    );

    if @detalle is null or ltrim(rtrim(@detalle)) = ''
    begin
        select @cabecera + '¬' + @anchos;
    end
    else
    begin
        select @cabecera + '¬' + @anchos + '¬' + @detalle;
    end
end
