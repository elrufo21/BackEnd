USE [master]
GO
/****** Object:  Database [BD_SGOV]    Script Date: 20/04/2026 16:11:07 ******/
CREATE DATABASE [BD_SGOV]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'BD_SGOV_Data', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS01\MSSQL\DATA\BD_SGOV.mdf' , SIZE = 8192KB , MAXSIZE = 3072000KB , FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'BD_SGOV_Logs', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS01\MSSQL\DATA\BD_SGOV.ldf' , SIZE = 8192KB , MAXSIZE = 1024000KB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [BD_SGOV] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [BD_SGOV].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [BD_SGOV] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [BD_SGOV] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [BD_SGOV] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [BD_SGOV] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [BD_SGOV] SET ARITHABORT OFF 
GO
ALTER DATABASE [BD_SGOV] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [BD_SGOV] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [BD_SGOV] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [BD_SGOV] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [BD_SGOV] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [BD_SGOV] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [BD_SGOV] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [BD_SGOV] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [BD_SGOV] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [BD_SGOV] SET  DISABLE_BROKER 
GO
ALTER DATABASE [BD_SGOV] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [BD_SGOV] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [BD_SGOV] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [BD_SGOV] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [BD_SGOV] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [BD_SGOV] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [BD_SGOV] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [BD_SGOV] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [BD_SGOV] SET  MULTI_USER 
GO
ALTER DATABASE [BD_SGOV] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [BD_SGOV] SET DB_CHAINING OFF 
GO
ALTER DATABASE [BD_SGOV] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [BD_SGOV] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [BD_SGOV] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [BD_SGOV] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [BD_SGOV] SET QUERY_STORE = OFF
GO
USE [BD_SGOV]
GO
/****** Object:  User [scot]    Script Date: 20/04/2026 16:11:07 ******/
CREATE USER [scot] WITHOUT LOGIN WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [scot]
GO
/****** Object:  Schema [scot]    Script Date: 20/04/2026 16:11:07 ******/
CREATE SCHEMA [scot]
GO
/****** Object:  UserDefinedFunction [dbo].[CalcularEdad]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create function [dbo].[CalcularEdad]
(
    @FecNac             date
)
RETURNS int
AS
BEGIN
declare 
 @fechaActual date,
 @anioNacimiento int,
 @mesNacimiento int,
 @diaNacimiento int,
 @añoActual int,
 @mesActual int,
 @diaActual int,
 @anios int

set @fechaActual=getdate()
set @anioNacimiento = year(@FecNac)
set @mesNacimiento = month(@FecNac)
set @diaNacimiento = day(@FecNac)

set @añoActual = CONVERT(int,year(@fechaActual))
set @mesActual = CONVERT(int,month(@fechaActual))
set @diaActual = CONVERT(int,day(@fechaActual))



set @anios = @añoActual - @anioNacimiento

if ((@mesActual - @mesNacimiento)<0)
begin
if (@anioNacimiento<@añoActual)
   set @anios=@anios-1 
end

if ((@mesActual = @mesNacimiento))
begin
   if (@diaNacimiento>@diaActual)
   set @anios=@anios-1 
end

RETURN @anios
END
GO
/****** Object:  UserDefinedFunction [dbo].[desincrectar]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create function [dbo].[desincrectar]
( @clave varbinary(500))

 returns varchar(100)
 as
 begin
 declare @pass as varchar(50)
 set @pass=DECRYPTBYPASSPHRASE('clave',@clave)
 return @pass
 end
GO
/****** Object:  UserDefinedFunction [dbo].[diaNombre]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE function [dbo].[diaNombre]
(@fecha datetime)
returns nvarchar(20)
as
begin
declare @NomDia nvarchar(20)
	 --if (DATEPART(dw,@fecha)=0)set @NomDia='Domingo'
	 --if (DATEPART(dw,@fecha)=1)set @NomDia='Lunes'
	 --if (DATEPART(dw,@fecha)=2)set @NomDia='Martes'
	 --if (DATEPART(dw,@fecha)=3)set @NomDia='Miercoles'
	 --if (DATEPART(dw,@fecha)=4)set @NomDia='Jueves'
	 --if (DATEPART(dw,@fecha)=5)set @NomDia='Viernes'
	 --if (DATEPART(dw,@fecha)=6)set @NomDia='Sabado'

	 if (DATEPART(dw,@fecha)=1)set @NomDia='Domingo'
	 if (DATEPART(dw,@fecha)=2)set @NomDia='Lunes'
	 if (DATEPART(dw,@fecha)=3)set @NomDia='Martes'
	 if (DATEPART(dw,@fecha)=4)set @NomDia='Miercoles'
	 if (DATEPART(dw,@fecha)=5)set @NomDia='Jueves'
	 if (DATEPART(dw,@fecha)=6)set @NomDia='Viernes'
	 if (DATEPART(dw,@fecha)=7)set @NomDia='Sabado'


 return @Nomdia
end
GO
/****** Object:  UserDefinedFunction [dbo].[encriptar]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create function [dbo].[encriptar]
( @clave varchar(50))
 returns varbinary(500)
 as
 begin
 declare @pass as varbinary(500)
 set @pass=ENCRYPTBYPASSPHRASE('clave',@clave)
 return @pass
 end
GO
/****** Object:  UserDefinedFunction [dbo].[fnSplitString]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[fnSplitString] 
( 
    @string VARCHAR(MAX), 
    @delimiter CHAR(1) 
) 
RETURNS @output TABLE(splitdata VARCHAR(MAX) 
) 
BEGIN 
    DECLARE @start INT, @end INT 
    SELECT @start = 1, @end = CHARINDEX(@delimiter, @string) 
    WHILE @start < LEN(@string) + 1 BEGIN 
        IF @end = 0  
            SET @end = LEN(@string) + 1
       
        INSERT INTO @output (splitdata)  
        VALUES(SUBSTRING(@string, @start, @end - @start)) 
        SET @start = @end + 1 
        SET @end = CHARINDEX(@delimiter, @string, @start)
    END 
    RETURN 
END
GO
/****** Object:  UserDefinedFunction [dbo].[geneneraIdLiquida]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create function [dbo].[geneneraIdLiquida] 
(@dato varchar(20))    
returns varchar(13)    
begin     
declare @autoincremento int,@numero varchar(8),@codigo varchar(12)    
set @codigo=SUBSTRING(@dato,1,4)    
select @autoincremento =ISNULL(MAX(CONVERT(INT,RIGHT(LiquidacionNumero,8))),0)FROM Liquidacion   
SET @autoincremento=@autoincremento + 1    
SELECT @numero=right('0000000' + convert(varchar,@autoincremento),8)    
set @codigo=RTRIM(@codigo)+RTRIM(@numero)    
return @codigo     
end
GO
/****** Object:  UserDefinedFunction [dbo].[geneneraIdLiVenta]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create function [dbo].[geneneraIdLiVenta]
(@dato varchar(20))  
returns varchar(13)  
begin   
declare @autoincremento int,@numero varchar(8),@codigo varchar(12)  
set @codigo=SUBSTRING(@dato,1,4)  
select @autoincremento =ISNULL(MAX(CONVERT(INT,RIGHT(LiquidacionNumero,8))),0)FROM LiquidacionVenta  
SET @autoincremento=@autoincremento + 1  
SELECT @numero=right('0000000' + convert(varchar,@autoincremento),8)  
set @codigo=RTRIM(@codigo)+RTRIM(@numero)  
return @codigo  
end
GO
/****** Object:  UserDefinedFunction [dbo].[genenerarNroFactura]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE function [dbo].[genenerarNroFactura](@dato varchar(20),@CompaniaId int,@DocuDocumento varchar(40))
returns varchar(13)
begin 
declare @autoincremento int,
@numero varchar(8),
@codigo varchar(11)
set @codigo=SUBSTRING(@dato,1,4)
select @autoincremento =ISNULL(MAX(CONVERT(INT,RIGHT(DocuNumero,8))),0)FROM DocumentoVenta
where CompaniaId=@CompaniaId and (DocuDocumento=@DocuDocumento and DocuSerie=@dato)
SET @autoincremento=@autoincremento + 1
SELECT @numero=right('0000000' + convert(varchar,@autoincremento),8)
set @codigo=RTRIM(@numero)
return @codigo
end
GO
/****** Object:  UserDefinedFunction [dbo].[genenerarNroGuia]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create function [dbo].[genenerarNroGuia] (@dato varchar(20))
returns varchar(11)
begin 
declare @autoincremento int,@numero varchar(8),@codigo varchar(11)
set @codigo=SUBSTRING(@dato,1,5)
select @autoincremento =ISNULL(MAX(CONVERT(INT,RIGHT(GuiaNumero,6))),0)FROM GuiaRemision
SET @autoincremento=@autoincremento + 1
SELECT @numero=right('00000' + convert(varchar,@autoincremento),6)
set @codigo=RTRIM(@codigo)+RTRIM(@numero)
return @codigo
end
GO
/****** Object:  UserDefinedFunction [dbo].[genenerarNroGuiaSI]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create function [dbo].[genenerarNroGuiaSI]  
(@serie varchar(10),@Concepto nvarchar(1))  
returns varchar(13)  
begin   
declare @autoincremento int,  
@numero varchar(8),  
@codigo varchar(11)  
set @codigo=SUBSTRING(@serie,1,4)  
select @autoincremento =ISNULL(MAX(CONVERT(INT,RIGHT(g.Numero,8))),0)FROM GuiaInternaSI g  
where(g.Concepto=@Concepto and g.Serie=@serie)  
SET @autoincremento=@autoincremento + 1  
SELECT @numero=right('0000000' + convert(varchar,@autoincremento),8)  
set @codigo=RTRIM(@numero)  
return @codigo  
end
GO
/****** Object:  UserDefinedFunction [dbo].[Letras]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[Letras]
(
    @Numero             Decimal(18,2),
    @Moneda             varchar(60)
)
RETURNS Varchar(180)
AS
BEGIN
    DECLARE @RespLetra Varchar(180)
        DECLARE @lnEntero INT,
                        @lcRetorno VARCHAR(512),
                        @lnTerna INT,
                        @lcMiles VARCHAR(512),
                        @lcCadena VARCHAR(512),
                        @lnUnidades INT,
                        @lnDecenas INT,
                        @lnCentenas INT,
                        @lnFraccion INT
        SELECT  @lnEntero = CAST(@Numero AS INT),
                        @lnFraccion = (@Numero - @lnEntero) * 100,
                        @lcRetorno = '',
                        @lnTerna = 1
  WHILE @lnEntero > 0
  BEGIN /* WHILE */
            -- Recorro terna por terna
            SELECT @lcCadena = ''
            SELECT @lnUnidades = @lnEntero % 10
            SELECT @lnEntero = CAST(@lnEntero/10 AS INT)
            SELECT @lnDecenas = @lnEntero % 10
            SELECT @lnEntero = CAST(@lnEntero/10 AS INT)
            SELECT @lnCentenas = @lnEntero % 10
            SELECT @lnEntero = CAST(@lnEntero/10 AS INT)
            -- Analizo las unidades
            SELECT @lcCadena =
            CASE /* UNIDADES */
              WHEN @lnUnidades = 1 THEN 'UN ' + @lcCadena
              WHEN @lnUnidades = 2 THEN 'DOS ' + @lcCadena
              WHEN @lnUnidades = 3 THEN 'TRES ' + @lcCadena
              WHEN @lnUnidades = 4 THEN 'CUATRO ' + @lcCadena
              WHEN @lnUnidades = 5 THEN 'CINCO ' + @lcCadena
              WHEN @lnUnidades = 6 THEN 'SEIS ' + @lcCadena
              WHEN @lnUnidades = 7 THEN 'SIETE ' + @lcCadena
              WHEN @lnUnidades = 8 THEN 'OCHO ' + @lcCadena
              WHEN @lnUnidades = 9 THEN 'NUEVE ' + @lcCadena
              ELSE @lcCadena
            END /* UNIDADES */
            -- Analizo las decenas
            SELECT @lcCadena =
            CASE /* DECENAS */
              WHEN @lnDecenas = 1 THEN
                CASE @lnUnidades
                  WHEN 0 THEN 'DIEZ '
                  WHEN 1 THEN 'ONCE '
                  WHEN 2 THEN 'DOCE '
                  WHEN 3 THEN 'TRECE '
                  WHEN 4 THEN 'CATORCE '
                  WHEN 5 THEN 'QUINCE '
                  WHEN 6 THEN 'DIEZ Y SEIS '
                  WHEN 7 THEN 'DIEZ Y SIETE '
                  WHEN 8 THEN 'DIEZ Y OCHO '
                  WHEN 9 THEN 'DIEZ Y NUEVE '
                END
              WHEN @lnDecenas = 2 THEN
              CASE @lnUnidades
                WHEN 0 THEN 'VEINTE '
                ELSE 'VEINTI' + @lcCadena
              END
              WHEN @lnDecenas = 3 THEN
              CASE @lnUnidades
                WHEN 0 THEN 'TREINTA '
                ELSE 'TREINTA Y ' + @lcCadena
              END
              WHEN @lnDecenas = 4 THEN
                CASE @lnUnidades
                    WHEN 0 THEN 'CUARENTA'
                    ELSE 'CUARENTA Y ' + @lcCadena
                END
              WHEN @lnDecenas = 5 THEN
                CASE @lnUnidades
                    WHEN 0 THEN 'CINCUENTA '
                    ELSE 'CINCUENTA Y ' + @lcCadena
                END
              WHEN @lnDecenas = 6 THEN
                CASE @lnUnidades
                    WHEN 0 THEN 'SESENTA '
                    ELSE 'SESENTA Y ' + @lcCadena
                END
              WHEN @lnDecenas = 7 THEN
                 CASE @lnUnidades
                    WHEN 0 THEN 'SETENTA '
                    ELSE 'SETENTA Y ' + @lcCadena
                 END
              WHEN @lnDecenas = 8 THEN
                CASE @lnUnidades
                    WHEN 0 THEN 'OCHENTA '
                    ELSE  'OCHENTA Y ' + @lcCadena
                END
              WHEN @lnDecenas = 9 THEN
                CASE @lnUnidades
                    WHEN 0 THEN 'NOVENTA '
                    ELSE 'NOVENTA Y ' + @lcCadena
                END
              ELSE @lcCadena
            END /* DECENAS */
            -- Analizo las centenas
            SELECT @lcCadena =
            CASE /* CENTENAS */
			WHEN @lnCentenas = 1 AND @lnTerna = 3 THEN 'CIEN ' + @lcCadena
WHEN @lnCentenas = 1 AND @lnUnidades = 0 AND @lnDecenas = 0 THEN 'CIEN ' + @lcCadena
WHEN @lnCentenas = 1 AND @lnTerna <> 3 THEN 'CIENTO ' + @lcCadena
              WHEN @lnCentenas = 1 THEN 'CIENTO ' + @lcCadena
              WHEN @lnCentenas = 2 THEN 'DOSCIENTOS ' + @lcCadena
              WHEN @lnCentenas = 3 THEN 'TRESCIENTOS ' + @lcCadena
              WHEN @lnCentenas = 4 THEN 'CUATROCIENTOS ' + @lcCadena
              WHEN @lnCentenas = 5 THEN 'QUINIENTOS ' + @lcCadena
              WHEN @lnCentenas = 6 THEN 'SEISCIENTOS ' + @lcCadena
              WHEN @lnCentenas = 7 THEN 'SETECIENTOS ' + @lcCadena
              WHEN @lnCentenas = 8 THEN 'OCHOCIENTOS ' + @lcCadena
              WHEN @lnCentenas = 9 THEN 'NOVECIENTOS ' + @lcCadena
              ELSE @lcCadena
            END /* CENTENAS */
            -- Analizo la terna
            SELECT @lcCadena =
            CASE /* TERNA */
              WHEN @lnTerna = 1 THEN @lcCadena
              WHEN @lnTerna = 2 THEN @lcCadena + 'MIL '
              WHEN @lnTerna = 3 THEN @lcCadena + 'MILLONES '
              WHEN @lnTerna = 4 THEN @lcCadena + 'MIL '
              ELSE ''
            END /* TERNA */
            -- Armo el retorno terna a terna
            SELECT @lcRetorno = @lcCadena  + @lcRetorno
            SELECT @lnTerna = @lnTerna + 1
   END /* WHILE */
   IF @lnTerna = 1
       SELECT @lcRetorno = 'CERO'
   DECLARE @sFraccion VARCHAR(15)
   SET @sFraccion = '00' + LTRIM(CAST(@lnFraccion AS varchar))
   SELECT @RespLetra = RTRIM(@lcRetorno) + ' CON ' + SUBSTRING(@sFraccion,LEN(@sFraccion)-1,2) + '/100 '+@Moneda
   RETURN @RespLetra
END
GO
/****** Object:  UserDefinedFunction [dbo].[MesNombre]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create function [dbo].[MesNombre]
(@NroMes int)
returns nvarchar(20)
as
begin
declare @NomMes nvarchar(20)
--set @NroMes=12
	 if (@NroMes=1)set @NomMes='Enero'
	 if (@NroMes=2)set @NomMes='Febreo'
	 if (@NroMes=3)set @NomMes='Marzo'
	 if (@NroMes=4)set @NomMes='Abril'
	 if (@NroMes=5)set @NomMes='Mayo'
	 if (@NroMes=6)set @NomMes='Junio'
	 if (@NroMes=7)set @NomMes='Julio'
	 if (@NroMes=8)set @NomMes='Agosto'
	 if (@NroMes=9)set @NomMes='Septiembre'
	 if (@NroMes=10)set @NomMes='Octubre'
	 if (@NroMes=11)set @NomMes='Noviembre'
	 if (@NroMes=12)set @NomMes='Diciembre'
 return @NomMes
end
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Addresses]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Addresses](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Direccion] [nvarchar](max) NULL,
	[Ciudad] [nvarchar](max) NULL,
	[Departamento] [nvarchar](max) NULL,
	[CodigoPostal] [nvarchar](max) NULL,
	[Username] [nvarchar](max) NULL,
	[Pais] [nvarchar](max) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[LastModifiedDate] [datetime2](7) NULL,
	[LastModifiedBy] [nvarchar](max) NULL,
 CONSTRAINT [PK_Addresses] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Almacen]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Almacen](
	[AlmacenId] [numeric](20, 0) IDENTITY(1,1) NOT NULL,
	[AlmacenNombre] [varchar](80) NULL,
	[AlmacenDepartamento] [varchar](80) NULL,
	[AlmacenProvincia] [varchar](80) NULL,
	[AlmacenDistrito] [varchar](80) NULL,
	[AlmacenDireccion] [varchar](300) NULL,
	[AlmacenEstado] [varchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[AlmacenId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Area]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Area](
	[AreaId] [int] IDENTITY(1,1) NOT NULL,
	[AreaNombre] [varchar](80) NULL,
PRIMARY KEY CLUSTERED 
(
	[AreaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Caja]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Caja](
	[CajaId] [numeric](38, 0) IDENTITY(1,1) NOT NULL,
	[CajaFecha] [datetime] NULL,
	[CajaCierre] [varchar](40) NULL,
	[MontoIniSOl] [decimal](18, 2) NULL,
	[CajaEncargado] [varchar](60) NULL,
	[CajaUsuario] [varchar](60) NULL,
	[CajaEstado] [varchar](40) NULL,
	[CajaIngresos] [decimal](18, 2) NULL,
	[CajaDeposito] [decimal](18, 2) NULL,
	[CajaSalidas] [decimal](18, 2) NULL,
	[CajaTotal] [decimal](18, 2) NULL,
	[UsuarioId] [int] NULL,
	[Observacion] [varchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[CajaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CajaDetalle]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CajaDetalle](
	[DetalleId] [numeric](38, 0) IDENTITY(1,1) NOT NULL,
	[CajaId] [numeric](38, 0) NULL,
	[DetalleFecha] [datetime] NULL,
	[NotaId] [numeric](38, 0) NULL,
	[DetalleMovimiento] [varchar](80) NULL,
	[DetalleReferencia] [varchar](80) NULL,
	[DetalleConcepto] [varchar](250) NULL,
	[DetalleMonto] [decimal](18, 2) NULL,
	[DetalleEfectivo] [decimal](18, 2) NULL,
	[DetalleVuelto] [decimal](18, 2) NULL,
	[RutaImagen] [varchar](max) NULL,
	[Estado] [nvarchar](1) NULL,
	[Vista] [nvarchar](1) NULL,
	[Usuario] [varchar](80) NULL,
	[GastoId] [varchar](40) NULL,
	[LiquidaId] [nvarchar](40) NULL,
PRIMARY KEY CLUSTERED 
(
	[DetalleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CajaGeneral]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CajaGeneral](
	[IdGeneral] [numeric](38, 0) IDENTITY(1,1) NOT NULL,
	[FechaCierre] [datetime] NULL,
	[Usuario] [varchar](80) NULL,
	[Ingresos] [decimal](18, 2) NULL,
	[Salidas] [decimal](18, 2) NULL,
	[Total] [decimal](18, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[IdGeneral] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CajaPincipal]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CajaPincipal](
	[IdCaja] [numeric](38, 0) IDENTITY(1,1) NOT NULL,
	[CajaConcepto] [varchar](80) NULL,
	[CajaFecha] [datetime] NULL,
	[CajaId] [numeric](38, 0) NULL,
	[CajaDescripcion] [varchar](250) NULL,
	[CajaMonto] [decimal](18, 2) NULL,
	[CajaUsuario] [varchar](20) NULL,
	[IdGeneral] [numeric](38, 0) NULL,
	[Referencia] [nvarchar](40) NULL,
	[GastoId] [nvarchar](40) NULL,
	[RutaImagen] [varchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[IdCaja] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Cliente]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Cliente](
	[ClienteId] [numeric](20, 0) IDENTITY(1,1) NOT NULL,
	[ClienteRazon] [varchar](140) NULL,
	[ClienteRuc] [varchar](40) NULL,
	[ClienteDni] [varchar](40) NULL,
	[ClienteDireccion] [varchar](max) NULL,
	[ClienteTelefono] [varchar](80) NULL,
	[ClienteCorreo] [varchar](80) NULL,
	[ClienteEstado] [varchar](40) NULL,
	[ClienteDespacho] [varchar](max) NULL,
	[ClienteUsuario] [varchar](80) NULL,
	[ClienteFecha] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[ClienteId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Compania]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Compania](
	[CompaniaId] [int] IDENTITY(1,1) NOT NULL,
	[CompaniaRazonSocial] [varchar](140) NULL,
	[CompaniaRUC] [varchar](20) NULL,
	[CompaniaDireccion] [varchar](max) NULL,
	[CompaniaTelefono] [varchar](80) NULL,
	[CompaniaEmail] [varchar](100) NULL,
	[CompaniaIniFecha] [varchar](100) NULL,
	[CompaniaComercial] [varchar](250) NULL,
	[CompaniaUserSecun] [varchar](250) NULL,
	[ComapaniaPWD] [varchar](250) NULL,
	[CompaniaPFX] [varchar](max) NULL,
	[CompaniaClave] [varchar](250) NULL,
	[CompaniaNomUBG] [varchar](40) NULL,
	[CompaniaCodigoUBG] [varchar](10) NULL,
	[CompaniaDistrito] [varchar](40) NULL,
	[CompaniaDirecSunat] [varchar](250) NULL,
	[ICBPER] [decimal](18, 2) NULL,
	[TokenApi] [varchar](max) NULL,
	[ClienIdToken] [varchar](max) NULL,
	[DescuentoMax] [decimal](18, 2) NULL,
	[RenovacionOSE] [date] NULL,
	[RenovacionFirma] [date] NULL,
	[RenovacionSome] [date] NULL,
	[CorreoSGO] [varchar](80) NULL,
	[PasswordCorreo] [varchar](80) NULL,
	[CorreosAdmin] [varchar](max) NULL,
	[TIPO_PROCESO] [int] NULL,
	[BoletaPorLote] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[CompaniaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Compras]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Compras](
	[CompraId] [numeric](38, 0) IDENTITY(1,1) NOT NULL,
	[CompaniaId] [int] NULL,
	[CompraCorrelativo] [varchar](80) NULL,
	[ProveedorId] [numeric](38, 0) NULL,
	[CompraRegistro] [datetime] NULL,
	[CompraEmision] [date] NULL,
	[CompraComputo] [date] NULL,
	[TipoCodigo] [char](20) NULL,
	[CompraSerie] [varchar](60) NULL,
	[CompraNumero] [varchar](80) NULL,
	[CompraCondicion] [varchar](60) NULL,
	[CompraMoneda] [varchar](60) NULL,
	[CompraTipoCambio] [decimal](18, 3) NULL,
	[CompraDias] [int] NULL,
	[CompraFechaPago] [date] NULL,
	[CompraUsuario] [varchar](80) NULL,
	[CompraTipoIgv] [varchar](60) NULL,
	[CompraValorVenta] [decimal](18, 2) NULL,
	[CompraDescuento] [decimal](18, 2) NULL,
	[CompraSubtotal] [decimal](18, 2) NULL,
	[CompraIgv] [decimal](18, 2) NULL,
	[CompraTotal] [decimal](18, 2) NULL,
	[CompraEstado] [varchar](60) NULL,
	[CompraAsociado] [varchar](60) NULL,
	[CompraSaldo] [decimal](18, 2) NULL,
	[CompraOBS] [varchar](max) NULL,
	[CompraTipoSunat] [decimal](18, 3) NULL,
	[CompraConcepto] [varchar](60) NULL,
	[CompraPercepcion] [decimal](18, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[CompraId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Countries]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Countries](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Iso2] [nvarchar](max) NULL,
	[Iso3] [nvarchar](max) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[LastModifiedDate] [datetime2](7) NULL,
	[LastModifiedBy] [nvarchar](max) NULL,
 CONSTRAINT [PK_Countries] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CuentaProveedor]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CuentaProveedor](
	[CuentaId] [numeric](38, 0) IDENTITY(1,1) NOT NULL,
	[ProveedorId] [numeric](38, 0) NULL,
	[Entidad] [varchar](80) NULL,
	[TipoCuenta] [varchar](80) NULL,
	[Moneda] [varchar](80) NULL,
	[NroCuenta] [varchar](80) NULL,
PRIMARY KEY CLUSTERED 
(
	[CuentaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DetaLiquidaVenta]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DetaLiquidaVenta](
	[DetalleId] [numeric](38, 0) IDENTITY(1,1) NOT NULL,
	[LiquidacionId] [numeric](38, 0) NULL,
	[DocuId] [numeric](38, 0) NULL,
	[NotaId] [numeric](38, 0) NULL,
	[SaldoDocu] [decimal](18, 2) NULL,
	[EfectivoSoles] [decimal](18, 2) NULL,
	[EfectivoDolar] [decimal](18, 2) NULL,
	[DepositoSoles] [decimal](18, 2) NULL,
	[DepositoDolar] [decimal](18, 2) NULL,
	[TipoCambio] [decimal](18, 3) NULL,
	[EntidadBanco] [varchar](80) NULL,
	[NroOperacion] [varchar](80) NULL,
	[AcuentaGeneral] [decimal](18, 2) NULL,
	[SaldoActual] [decimal](18, 2) NULL,
	[FechaPago] [varchar](60) NULL,
	[Documento] [varchar](80) NULL,
	[Cliente] [varchar](350) NULL,
PRIMARY KEY CLUSTERED 
(
	[DetalleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DetalleCompra]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DetalleCompra](
	[DetalleId] [numeric](38, 0) IDENTITY(1,1) NOT NULL,
	[CompraId] [numeric](38, 0) NULL,
	[IdProducto] [numeric](20, 0) NULL,
	[DetalleCodigo] [varchar](80) NULL,
	[Descripcion] [varchar](255) NULL,
	[DetalleUM] [varchar](60) NULL,
	[DetalleCantidad] [decimal](18, 2) NULL,
	[PrecioCosto] [decimal](18, 4) NULL,
	[DetalleImporte] [decimal](18, 4) NULL,
	[DetalleDescuento] [decimal](18, 4) NULL,
	[DetalleEstado] [varchar](40) NULL,
	[DescuentoB] [decimal](18, 4) NULL,
	[EstadoB] [char](1) NULL,
	[ValorUM] [decimal](18, 4) NULL,
PRIMARY KEY CLUSTERED 
(
	[DetalleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DetalleDocumento]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DetalleDocumento](
	[DetalleId] [numeric](38, 0) IDENTITY(1,1) NOT NULL,
	[DocuId] [numeric](38, 0) NULL,
	[IdProducto] [numeric](20, 0) NULL,
	[DetalleCantidad] [decimal](18, 2) NULL,
	[DetallPrecio] [decimal](18, 2) NULL,
	[DetalleImporte] [decimal](18, 2) NULL,
	[DetalleNotaId] [numeric](38, 0) NULL,
	[DetalleUM] [varchar](80) NULL,
	[ValorUM] [decimal](18, 4) NULL,
	[DetalleDescripcion] [varchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[DetalleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DetalleGuia]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DetalleGuia](
	[DetalleId] [numeric](38, 0) IDENTITY(1,1) NOT NULL,
	[GuiaId] [numeric](38, 0) NULL,
	[IdProducto] [numeric](20, 0) NULL,
	[DetalleCantidad] [decimal](18, 2) NULL,
	[DetalleCosto] [decimal](18, 4) NULL,
	[DetallePrecio] [decimal](18, 2) NULL,
	[DetalleImporte] [decimal](18, 2) NULL,
	[DetalleEstado] [varchar](60) NULL,
	[IdDetalle] [numeric](38, 0) NULL,
	[ValorUM] [decimal](18, 4) NULL,
	[UniMedida] [varchar](40) NULL,
PRIMARY KEY CLUSTERED 
(
	[DetalleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DetalleGuiaInterna]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DetalleGuiaInterna](
	[DetalleId] [numeric](38, 0) IDENTITY(1,1) NOT NULL,
	[GuiaId] [numeric](38, 0) NULL,
	[IdProducto] [numeric](20, 0) NULL,
	[Cantidad] [decimal](18, 2) NULL,
	[UnidadM] [varchar](80) NULL,
	[Descripcion] [varchar](max) NULL,
	[Costo] [decimal](18, 4) NULL,
	[PrecioVenta] [decimal](18, 2) NULL,
	[Importe] [decimal](18, 2) NULL,
	[Estado] [nvarchar](1) NULL,
	[ValorUM] [decimal](18, 4) NULL,
PRIMARY KEY CLUSTERED 
(
	[DetalleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DetalleLiquida]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DetalleLiquida](
	[DetalleId] [numeric](38, 0) IDENTITY(1,1) NOT NULL,
	[LiquidacionId] [numeric](38, 0) NULL,
	[CompraId] [numeric](38, 0) NULL,
	[SaldoDocu] [decimal](18, 2) NULL,
	[EfectivoSoles] [decimal](18, 2) NULL,
	[EfectivoDolar] [decimal](18, 2) NULL,
	[DepositoSoles] [decimal](18, 2) NULL,
	[DepositoDolar] [decimal](18, 2) NULL,
	[TipoCambio] [decimal](18, 3) NULL,
	[EntidadBanco] [varchar](80) NULL,
	[NroOperacion] [varchar](80) NULL,
	[AcuentaGeneral] [decimal](18, 2) NULL,
	[SaldoActual] [decimal](18, 2) NULL,
	[FechaPago] [varchar](60) NULL,
	[Numero] [varchar](60) NULL,
	[Proveedor] [varchar](255) NULL,
	[Moneda] [varchar](20) NULL,
	[Concepto] [varchar](40) NULL,
PRIMARY KEY CLUSTERED 
(
	[DetalleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DetallePedido]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DetallePedido](
	[DetalleId] [numeric](38, 0) IDENTITY(1,1) NOT NULL,
	[NotaId] [numeric](38, 0) NULL,
	[IdProducto] [numeric](20, 0) NULL,
	[DetalleCantidad] [decimal](18, 2) NULL,
	[DetalleUm] [varchar](40) NULL,
	[DetalleDescripcion] [varchar](max) NULL,
	[DetalleCosto] [decimal](18, 4) NULL,
	[DetallePrecio] [decimal](18, 2) NULL,
	[DetalleImporte] [decimal](18, 2) NULL,
	[DetalleEstado] [varchar](60) NULL,
	[CantidadSaldo] [decimal](18, 2) NULL,
	[ValorUM] [decimal](18, 4) NULL,
PRIMARY KEY CLUSTERED 
(
	[DetalleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DocumentoVenta]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DocumentoVenta](
	[DocuId] [numeric](38, 0) IDENTITY(1,1) NOT NULL,
	[CompaniaId] [int] NULL,
	[NotaId] [numeric](38, 0) NULL,
	[DocuDocumento] [varchar](60) NULL,
	[DocuNumero] [varchar](60) NULL,
	[ClienteId] [numeric](20, 0) NULL,
	[DocuRegistro] [datetime] NULL,
	[DocuEmision] [date] NULL,
	[DocuCondicion] [varchar](60) NULL,
	[DocuLetras] [varchar](60) NULL,
	[DocuSubTotal] [decimal](18, 2) NULL,
	[DocuIgv] [decimal](18, 2) NULL,
	[DocuTotal] [decimal](18, 2) NULL,
	[DocuSaldo] [decimal](18, 2) NULL,
	[DocuUsuario] [varchar](60) NULL,
	[DocuEstado] [varchar](60) NULL,
	[DocuSerie] [char](4) NULL,
	[TipoCodigo] [varchar](10) NULL,
	[DocuAdicional] [decimal](18, 2) NULL,
	[DocuAsociado] [varchar](80) NULL,
	[DocuConcepto] [varchar](80) NULL,
	[DocuNroGuia] [varchar](80) NULL,
	[DocuHash] [varchar](250) NULL,
	[EstadoSunat] [varchar](80) NULL,
	[ICBPER] [decimal](18, 2) NULL,
	[CodigoSunat] [varchar](80) NULL,
	[MensajeSunat] [varchar](max) NULL,
	[DocuGravada] [decimal](18, 2) NULL,
	[DocuDescuento] [decimal](18, 2) NULL,
	[EnvioCorreo] [nvarchar](1) NULL,
	[FormaPago] [varchar](80) NULL,
	[EntidadBancaria] [varchar](80) NULL,
	[NroOperacion] [varchar](80) NULL,
	[Efectivo] [decimal](18, 2) NULL,
	[Deposito] [decimal](18, 2) NULL,
	[ClienteRazon] [varchar](140) NULL,
	[ClienteRuc] [varchar](40) NULL,
	[ClienteDni] [varchar](40) NULL,
	[DireccionFiscal] [varchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[DocuId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Feriados]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Feriados](
	[IdFeriado] [int] IDENTITY(1,1) NOT NULL,
	[Fecha] [date] NULL,
	[Motivo] [varchar](300) NULL,
PRIMARY KEY CLUSTERED 
(
	[IdFeriado] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GastosFijos]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GastosFijos](
	[GastoId] [int] IDENTITY(1,1) NOT NULL,
	[GastoFecha] [date] NULL,
	[GsstoDesc] [varchar](max) NULL,
	[GstoMonto] [decimal](18, 2) NULL,
	[GastoReg] [datetime] NULL,
	[GastoUsuario] [varchar](80) NULL,
	[Estado] [nvarchar](1) NULL,
PRIMARY KEY CLUSTERED 
(
	[GastoId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GuiaCanje]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GuiaCanje](
	[CanjeId] [numeric](38, 0) IDENTITY(1,1) NOT NULL,
	[CompraId] [numeric](38, 0) NULL,
	[CompaniaId] [int] NULL,
	[CanjeFecha] [date] NULL,
	[CanjeRegistro] [datetime] NULL,
	[CanjeSerie] [varchar](80) NULL,
	[CanjeNumero] [varchar](80) NULL,
	[CanjeEmision] [date] NULL,
	[CanjeComputo] [date] NULL,
	[CanjeCorrelativo] [varchar](80) NULL,
	[CanjeTipo] [varchar](80) NULL,
	[CanjeOBS] [varchar](max) NULL,
	[TCSunat] [decimal](18, 3) NULL,
	[GCompania] [int] NULL,
	[GSerie] [varchar](80) NULL,
	[GNumero] [varchar](80) NULL,
	[GEmision] [date] NULL,
	[GCanjeComputo] [date] NULL,
	[GCanjeCorrelativo] [varchar](80) NULL,
	[GCanjeTipo] [varchar](80) NULL,
	[GCanjeOBS] [varchar](max) NULL,
	[GTCSunat] [decimal](18, 3) NULL,
	[CanjeUsuario] [varchar](60) NULL,
PRIMARY KEY CLUSTERED 
(
	[CanjeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GuiaInterna]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GuiaInterna](
	[GuiaId] [numeric](38, 0) IDENTITY(1,1) NOT NULL,
	[FechaRegistro] [datetime] NULL,
	[Concepto] [nvarchar](1) NULL,
	[Motivo] [varchar](300) NULL,
	[Origen] [varchar](300) NULL,
	[Destino] [varchar](300) NULL,
	[Observacion] [varchar](max) NULL,
	[Usuario] [varchar](80) NULL,
PRIMARY KEY CLUSTERED 
(
	[GuiaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GuiaInternaSI]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GuiaInternaSI](
	[GuiaId] [numeric](38, 0) IDENTITY(1,1) NOT NULL,
	[FechaRegistro] [datetime] NULL,
	[Concepto] [nvarchar](1) NULL,
	[Serie] [nvarchar](4) NULL,
	[Numero] [nvarchar](8) NULL,
	[Motivo] [varchar](300) NULL,
	[Origen] [varchar](300) NULL,
	[Destino] [varchar](300) NULL,
	[ClienteId] [varchar](20) NULL,
	[Observacion] [varchar](max) NULL,
	[Total] [decimal](18, 2) NULL,
	[Usuario] [varchar](80) NULL,
	[Estado] [nvarchar](1) NULL,
PRIMARY KEY CLUSTERED 
(
	[GuiaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GuiaRelacion]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GuiaRelacion](
	[DetalleId] [numeric](38, 0) IDENTITY(1,1) NOT NULL,
	[GuiaId] [numeric](38, 0) NULL,
	[NotaId] [numeric](38, 0) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GuiaRemision]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GuiaRemision](
	[GuiaId] [numeric](38, 0) IDENTITY(1,1) NOT NULL,
	[GuiaNumero] [varchar](60) NULL,
	[GuiaMotivo] [varchar](80) NULL,
	[GuiaRegistro] [datetime] NULL,
	[GuiaFechaTraslado] [datetime] NULL,
	[GuiaDestinatario] [varchar](250) NULL,
	[GuiaRucDes] [varchar](60) NULL,
	[GuiaAlmacen] [varchar](80) NULL,
	[GuiaPartida] [varchar](max) NULL,
	[GuiaLLegada] [varchar](max) NULL,
	[GuiaTramsporte] [varchar](80) NULL,
	[GuiaTransporteRuc] [varchar](20) NULL,
	[GuiaChofer] [varchar](80) NULL,
	[GuiaPlaca] [varchar](80) NULL,
	[GuiaConstancia] [varchar](80) NULL,
	[GuiaLicencia] [varchar](80) NULL,
	[GuiaUsuario] [varchar](80) NULL,
	[GuiaTotal] [decimal](18, 2) NULL,
	[GuiaConcepto] [varchar](40) NULL,
	[ClienteId] [numeric](20, 0) NULL,
	[GuiaEstado] [varchar](60) NULL,
	[GuiaTelefono] [varchar](80) NULL,
PRIMARY KEY CLUSTERED 
(
	[GuiaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Images]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Images](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Url] [nvarchar](4000) NULL,
	[ProductId] [int] NOT NULL,
	[PublicCode] [nvarchar](max) NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[LastModifiedDate] [datetime2](7) NULL,
	[LastModifiedBy] [nvarchar](max) NULL,
 CONSTRAINT [PK_Images] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Kardex]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Kardex](
	[KardexId] [numeric](38, 0) IDENTITY(1,1) NOT NULL,
	[IdProducto] [numeric](20, 0) NULL,
	[KardexFecha] [datetime] NULL,
	[KardexMotivo] [varchar](60) NULL,
	[KardexDocumento] [varchar](60) NULL,
	[StockInicial] [decimal](18, 2) NULL,
	[CantidadIngreso] [decimal](18, 2) NULL,
	[CantidadSalida] [decimal](18, 2) NULL,
	[PrecioCosto] [decimal](18, 4) NULL,
	[StockFinal] [decimal](18, 2) NULL,
	[KadexConcepto] [varchar](40) NULL,
	[Usuario] [varchar](60) NULL,
PRIMARY KEY CLUSTERED 
(
	[KardexId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Letra]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Letra](
	[LetraId] [numeric](38, 0) IDENTITY(1,1) NOT NULL,
	[ProveedorId] [numeric](38, 0) NULL,
	[LetraFechaReg] [datetime] NULL,
	[LetraFechaGiro] [date] NULL,
	[LetraMoneda] [varchar](40) NULL,
	[LetraSaldo] [decimal](18, 2) NULL,
	[LetraTotal] [decimal](18, 2) NULL,
	[LetraUsuario] [varchar](60) NULL,
	[LetraEstado] [varchar](60) NULL,
	[CompaniaId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[LetraId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Liquidacion]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Liquidacion](
	[LiquidacionId] [numeric](38, 0) IDENTITY(1,1) NOT NULL,
	[LiquidacionNumero] [varchar](80) NULL,
	[LiquidacionRegistro] [datetime] NULL,
	[LiquidacionFecha] [date] NULL,
	[LiquidacionDescripcion] [varchar](250) NULL,
	[LiquidacionCambio] [decimal](18, 3) NULL,
	[LiquidaEfectivoSol] [decimal](18, 2) NULL,
	[LiquidaDepositoSol] [decimal](18, 2) NULL,
	[LiquidaTotalSol] [decimal](18, 2) NULL,
	[LiquidaEfectivoDol] [decimal](18, 2) NULL,
	[LiquidaDepositoDol] [decimal](18, 2) NULL,
	[LiquidaTotalDol] [decimal](18, 2) NULL,
	[LiquidaUsuario] [varchar](60) NULL,
PRIMARY KEY CLUSTERED 
(
	[LiquidacionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LiquidacionVenta]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LiquidacionVenta](
	[LiquidacionId] [numeric](38, 0) IDENTITY(1,1) NOT NULL,
	[LiquidacionNumero] [varchar](80) NULL,
	[LiquidacionRegistro] [datetime] NULL,
	[LiquidacionFecha] [date] NULL,
	[LiquidacionDescripcion] [varchar](250) NULL,
	[LiquidacionCambio] [decimal](18, 3) NULL,
	[LiquidaEfectivoSol] [decimal](18, 2) NULL,
	[LiquidaDepositoSol] [decimal](18, 2) NULL,
	[LiquidaTotalSol] [decimal](18, 2) NULL,
	[LiquidaEfectivoDol] [decimal](18, 2) NULL,
	[LiquidaDepositoDol] [decimal](18, 2) NULL,
	[LiquidaTotalDol] [decimal](18, 2) NULL,
	[LiquidaUsuario] [varchar](60) NULL,
PRIMARY KEY CLUSTERED 
(
	[LiquidacionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MAQUINAS]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MAQUINAS](
	[IdMaquina] [int] IDENTITY(1,1) NOT NULL,
	[Maquina] [varchar](140) NULL,
	[Registro] [datetime] NULL,
	[SerieFactura] [nvarchar](4) NULL,
	[SerieNC] [nvarchar](4) NULL,
	[SerieBoleta] [nvarchar](4) NULL,
	[Tiketera] [varchar](300) NULL,
PRIMARY KEY CLUSTERED 
(
	[IdMaquina] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Monedas]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Monedas](
	[MonedaId] [numeric](38, 0) IDENTITY(1,1) NOT NULL,
	[ConteoId] [numeric](38, 0) NULL,
	[Efectivo] [int] NULL,
	[Billete] [varchar](80) NULL,
	[Monto] [decimal](18, 2) NULL,
	[Concepto] [char](1) NULL,
	[CajaId] [numeric](38, 0) NULL,
PRIMARY KEY CLUSTERED 
(
	[MonedaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NotaPedido]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NotaPedido](
	[NotaId] [numeric](38, 0) IDENTITY(1,1) NOT NULL,
	[NotaDocu] [varchar](60) NULL,
	[ClienteId] [numeric](20, 0) NULL,
	[NotaFecha] [datetime] NULL,
	[NotaUsuario] [varchar](60) NULL,
	[NotaFormaPago] [varchar](60) NULL,
	[NotaCondicion] [varchar](60) NULL,
	[NotaFechaPago] [date] NULL,
	[NotaDireccion] [varchar](max) NULL,
	[NotaTelefono] [varchar](250) NULL,
	[NotaSubtotal] [decimal](18, 2) NULL,
	[NotaMovilidad] [decimal](18, 2) NULL,
	[NotaDescuento] [decimal](18, 2) NULL,
	[NotaTotal] [decimal](18, 2) NULL,
	[NotaAcuenta] [decimal](18, 2) NULL,
	[NotaSaldo] [decimal](18, 2) NULL,
	[NotaAdicional] [decimal](18, 2) NULL,
	[NotaTarjeta] [decimal](18, 2) NULL,
	[NotaPagar] [decimal](18, 2) NULL,
	[NotaEstado] [varchar](60) NULL,
	[CompaniaId] [int] NULL,
	[NotaEntrega] [varchar](40) NULL,
	[ModificadoPor] [varchar](60) NULL,
	[FechaEdita] [varchar](60) NULL,
	[NotaConcepto] [varchar](60) NULL,
	[NotaSerie] [varchar](60) NULL,
	[NotaNumero] [varchar](60) NULL,
	[NotaGanancia] [decimal](18, 2) NULL,
	[ICBPER] [decimal](18, 2) NULL,
	[CajaId] [varchar](40) NULL,
	[EntidadBancaria] [varchar](80) NULL,
	[NroOperacion] [varchar](80) NULL,
	[Efectivo] [decimal](18, 2) NULL,
	[Deposito] [decimal](18, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[NotaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Personal]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Personal](
	[PersonalId] [int] IDENTITY(1,1) NOT NULL,
	[PersonalNombres] [varchar](140) NULL,
	[PersonalApellidos] [varchar](140) NULL,
	[AreaId] [int] NULL,
	[PersonalCodigo] [varchar](80) NULL,
	[PersonalNacimiento] [date] NULL,
	[PersonalIngreso] [varchar](20) NULL,
	[PersonalDNI] [varchar](20) NULL,
	[PersonalDireccion] [varchar](140) NULL,
	[PersonalTelefono] [varchar](40) NULL,
	[PersonalEmail] [varchar](100) NULL,
	[PersonalEstado] [varchar](60) NULL,
	[PersonalImagen] [varchar](max) NULL,
	[CompaniaId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[PersonalId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Producto]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Producto](
	[IdProducto] [numeric](20, 0) IDENTITY(1,1) NOT NULL,
	[IdSubLinea] [numeric](20, 0) NULL,
	[ProductoCodigo] [varchar](300) NULL,
	[ProductoNombre] [varchar](300) NULL,
	[ProductoUM] [varchar](60) NULL,
	[ProductoCosto] [decimal](18, 4) NULL,
	[ProductoVenta] [decimal](18, 2) NULL,
	[ProductoVentaB] [decimal](18, 2) NULL,
	[ProductoCantidad] [decimal](18, 2) NULL,
	[ProductoEstado] [varchar](60) NULL,
	[ProductoUsuario] [varchar](60) NULL,
	[ProductoFecha] [datetime] NULL,
	[ProductoImagen] [varchar](max) NULL,
	[ValorCritico] [decimal](18, 2) NULL,
	[AplicaINV] [nvarchar](1) NULL,
PRIMARY KEY CLUSTERED 
(
	[IdProducto] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductoUnion]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductoUnion](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdProducto] [numeric](20, 0) NULL,
	[IdProductoB] [numeric](20, 0) NULL,
	[Cantidad] [decimal](18, 2) NULL,
	[UM] [varchar](80) NULL,
	[Precio] [decimal](18, 2) NULL,
	[ValorUM] [decimal](18, 4) NULL,
	[Estado] [nvarchar](1) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Proveedor]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Proveedor](
	[ProveedorId] [numeric](38, 0) IDENTITY(1,1) NOT NULL,
	[ProveedorRazon] [varchar](250) NULL,
	[ProveedorRuc] [varchar](20) NULL,
	[ProveedorContacto] [varchar](140) NULL,
	[ProveedorCelular] [varchar](140) NULL,
	[ProveedorTelefono] [varchar](140) NULL,
	[ProveedorCorreo] [varchar](140) NULL,
	[ProveedorDireccion] [varchar](140) NULL,
	[ProveedorEstado] [varchar](40) NULL,
PRIMARY KEY CLUSTERED 
(
	[ProveedorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ResumenBoletas]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ResumenBoletas](
	[ResumenId] [numeric](38, 0) IDENTITY(1,1) NOT NULL,
	[CompaniaId] [int] NULL,
	[ResumenSerie] [varchar](250) NULL,
	[Secuencia] [numeric](38, 0) NULL,
	[FechaReferencia] [date] NULL,
	[FechaEnvio] [datetime] NULL,
	[SubTotal] [decimal](18, 2) NULL,
	[IGV] [decimal](18, 2) NULL,
	[Total] [decimal](18, 2) NULL,
	[ResumenTiket] [varchar](250) NULL,
	[CodigoSunat] [varchar](80) NULL,
	[HASHCDR] [varchar](max) NULL,
	[MensajeSunat] [varchar](max) NULL,
	[Usuario] [varchar](80) NULL,
	[ESTADO] [char](1) NULL,
	[RangoNumero] [varchar](80) NULL,
	[ICBPER] [decimal](18, 2) NULL,
	[CDRBase64] [varchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[ResumenId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Reviews]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Reviews](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [nvarchar](100) NULL,
	[Rating] [int] NOT NULL,
	[Comentario] [nvarchar](4000) NULL,
	[ProductId] [int] NOT NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[LastModifiedDate] [datetime2](7) NULL,
	[LastModifiedBy] [nvarchar](max) NULL,
 CONSTRAINT [PK_Reviews] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ShoppingCartItems]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShoppingCartItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Producto] [nvarchar](max) NULL,
	[Precio] [decimal](10, 2) NOT NULL,
	[Cantidad] [int] NOT NULL,
	[Imagen] [nvarchar](max) NULL,
	[Categoria] [nvarchar](max) NULL,
	[ShoppingCartMasterId] [uniqueidentifier] NULL,
	[ShoppingCartId] [int] NOT NULL,
	[ProductId] [int] NOT NULL,
	[Stock] [int] NOT NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[LastModifiedDate] [datetime2](7) NULL,
	[LastModifiedBy] [nvarchar](max) NULL,
 CONSTRAINT [PK_ShoppingCartItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ShoppingCarts]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShoppingCarts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ShoppingCartMasterId] [uniqueidentifier] NULL,
	[CreatedDate] [datetime2](7) NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[LastModifiedDate] [datetime2](7) NULL,
	[LastModifiedBy] [nvarchar](max) NULL,
 CONSTRAINT [PK_ShoppingCarts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Sublinea]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sublinea](
	[IdSubLinea] [numeric](20, 0) IDENTITY(1,1) NOT NULL,
	[NombreSublinea] [varchar](300) NULL,
	[CodigoSunat] [varchar](40) NULL,
PRIMARY KEY CLUSTERED 
(
	[IdSubLinea] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TemporalCanje]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TemporalCanje](
	[temporalId] [numeric](38, 0) IDENTITY(1,1) NOT NULL,
	[temporalCanje] [varchar](80) NULL,
	[temporalDias] [int] NULL,
	[temporalVencimiento] [varchar](20) NULL,
	[temporalMonto] [decimal](18, 2) NULL,
	[usuarioId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[temporalId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TemporalCompra]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TemporalCompra](
	[TemporalId] [numeric](38, 0) IDENTITY(1,1) NOT NULL,
	[UsuarioID] [int] NULL,
	[IdProducto] [numeric](20, 0) NULL,
	[DetalleCodigo] [varchar](80) NULL,
	[Descripcion] [varchar](255) NULL,
	[DetalleUM] [varchar](60) NULL,
	[DetalleCantidad] [decimal](18, 2) NULL,
	[PrecioCosto] [decimal](18, 4) NULL,
	[DetalleImporte] [decimal](18, 2) NULL,
	[DetalleDescuento] [decimal](18, 4) NULL,
	[DetalleEstado] [varchar](40) NULL,
	[ValorUM] [decimal](18, 4) NULL,
	[Posicion] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[TemporalId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TemporalGuia]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TemporalGuia](
	[TemporalId] [numeric](38, 0) IDENTITY(1,1) NOT NULL,
	[UsuarioID] [int] NULL,
	[IdProducto] [numeric](20, 0) NULL,
	[cantidad] [decimal](18, 2) NULL,
	[precioventa] [decimal](18, 2) NULL,
	[importe] [decimal](18, 2) NULL,
	[Concepto] [varchar](60) NULL,
	[CantidadSaldo] [decimal](18, 2) NULL,
	[ClienteId] [numeric](20, 0) NULL,
	[DetalleId] [numeric](38, 0) NULL,
	[DetalleUM] [varchar](40) NULL,
	[ValorUM] [decimal](18, 4) NULL,
PRIMARY KEY CLUSTERED 
(
	[TemporalId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TemporalGuiaB]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TemporalGuiaB](
	[TemporalId] [numeric](38, 0) IDENTITY(1,1) NOT NULL,
	[UsuarioID] [int] NULL,
	[IdProducto] [numeric](20, 0) NULL,
	[Cantidad] [decimal](18, 2) NULL,
	[UnidadM] [varchar](80) NULL,
	[PrecioVenta] [decimal](18, 2) NULL,
	[Importe] [decimal](18, 2) NULL,
	[Concepto] [nvarchar](1) NULL,
	[ValorUM] [decimal](18, 4) NULL,
PRIMARY KEY CLUSTERED 
(
	[TemporalId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[temporalLetra]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[temporalLetra](
	[TemporalId] [numeric](38, 0) IDENTITY(1,1) NOT NULL,
	[CompraId] [numeric](38, 0) NULL,
	[ProveedorId] [numeric](38, 0) NULL,
	[TemporalDocumento] [varchar](60) NULL,
	[TemporalMoneda] [varchar](20) NULL,
	[TemporalMonto] [decimal](18, 2) NULL,
	[UsuarioId] [int] NULL,
	[TemporalCanje] [varchar](80) NULL,
PRIMARY KEY CLUSTERED 
(
	[TemporalId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TemporalLiquida]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TemporalLiquida](
	[TemporalId] [numeric](38, 0) IDENTITY(1,1) NOT NULL,
	[IdDeuda] [numeric](38, 0) NULL,
	[Numero] [varchar](60) NULL,
	[Proveedor] [varchar](255) NULL,
	[SaldoDocu] [decimal](18, 2) NULL,
	[Moneda] [varchar](20) NULL,
	[TipoCambio] [decimal](18, 3) NULL,
	[EfectivoSoles] [decimal](18, 2) NULL,
	[EfectivoDolar] [decimal](18, 2) NULL,
	[DepositoSoles] [decimal](18, 2) NULL,
	[DepositoDolar] [decimal](18, 2) NULL,
	[EntidadBanco] [varchar](80) NULL,
	[NroOperacion] [varchar](80) NULL,
	[AcuentaGeneral] [decimal](18, 2) NULL,
	[TemporalFecha] [varchar](60) NULL,
	[UsuarioId] [int] NULL,
	[Concepto] [varchar](40) NULL,
PRIMARY KEY CLUSTERED 
(
	[TemporalId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TemporalLiVenta]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TemporalLiVenta](
	[TemporalId] [numeric](38, 0) IDENTITY(1,1) NOT NULL,
	[DocuId] [numeric](38, 0) NULL,
	[NotaId] [numeric](38, 0) NULL,
	[UsuarioId] [int] NULL,
	[SaldoDocu] [decimal](18, 2) NULL,
	[TipoCambio] [decimal](18, 3) NULL,
	[EfectivoSoles] [decimal](18, 2) NULL,
	[EfectivoDolar] [decimal](18, 2) NULL,
	[DepositoSoles] [decimal](18, 2) NULL,
	[DepositoDolar] [decimal](18, 2) NULL,
	[EntidadBanco] [varchar](80) NULL,
	[NroOperacion] [varchar](80) NULL,
	[AcuentaGeneral] [decimal](18, 2) NULL,
	[TemporalFecha] [varchar](60) NULL,
PRIMARY KEY CLUSTERED 
(
	[TemporalId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TemporalServicio]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TemporalServicio](
	[TemporalId] [numeric](38, 0) IDENTITY(1,1) NOT NULL,
	[UsuarioId] [int] NULL,
	[TemporalDetalle] [varchar](max) NULL,
	[TemporalUm] [varchar](80) NULL,
	[TemporalCantidad] [decimal](18, 2) NULL,
	[TemporalCosto] [decimal](18, 4) NULL,
	[TemporalDescuento] [decimal](18, 4) NULL,
	[TemporalImporte] [decimal](18, 2) NULL,
	[TemporalEstado] [varchar](80) NULL,
PRIMARY KEY CLUSTERED 
(
	[TemporalId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TemporalVenta]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TemporalVenta](
	[temporalId] [numeric](38, 0) IDENTITY(1,1) NOT NULL,
	[UsuarioID] [int] NULL,
	[IdProducto] [numeric](20, 0) NULL,
	[cantidad] [decimal](18, 2) NULL,
	[precioventa] [decimal](18, 2) NULL,
	[importe] [decimal](18, 2) NULL,
	[ValorUM] [decimal](18, 4) NULL,
	[UniMedida] [varchar](40) NULL,
	[Descripcion] [varchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TipoCambio]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TipoCambio](
	[IdTipo] [numeric](38, 0) IDENTITY(1,1) NOT NULL,
	[TipoFecha] [date] NULL,
	[TipoCompra] [decimal](18, 3) NULL,
	[TipoVenta] [decimal](18, 3) NULL,
	[TipoEmpresa] [decimal](18, 3) NULL,
PRIMARY KEY CLUSTERED 
(
	[IdTipo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TipoComprobante]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TipoComprobante](
	[TipoId] [int] IDENTITY(1,1) NOT NULL,
	[TipoCodigo] [char](20) NULL,
	[TipoDescripcion] [varchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[TipoId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Ubigeo]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Ubigeo](
	[UgeoId] [int] IDENTITY(1,1) NOT NULL,
	[IdDepa] [varchar](20) NULL,
	[IdProv] [varchar](20) NULL,
	[IdDist] [varchar](20) NULL,
	[Nombre] [varchar](140) NULL,
PRIMARY KEY CLUSTERED 
(
	[UgeoId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UnidadMedida]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UnidadMedida](
	[IdUm] [int] IDENTITY(1,1) NOT NULL,
	[IdProducto] [numeric](20, 0) NULL,
	[UMDescripcion] [varchar](80) NULL,
	[ValorUM] [decimal](18, 4) NULL,
	[PrecioVenta] [decimal](18, 2) NULL,
	[PrecioVentaB] [decimal](18, 2) NULL,
	[PrecioCosto] [decimal](18, 4) NULL,
	[unidadImagen] [varchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[IdUm] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Usuarios]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Usuarios](
	[UsuarioID] [int] IDENTITY(1,1) NOT NULL,
	[PersonalId] [int] NULL,
	[UsuarioAlias] [varchar](60) NULL,
	[UsuarioClave] [varbinary](500) NULL,
	[UsuarioFechaReg] [datetime] NULL,
	[UsuarioEstado] [varchar](40) NULL,
	[FechaVencimientoClave] [date] NULL,
PRIMARY KEY CLUSTERED 
(
	[UsuarioID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Compania] ADD  CONSTRAINT [DF_Compania_BoletaPorLote]  DEFAULT ((1)) FOR [BoletaPorLote]
GO
ALTER TABLE [dbo].[CajaDetalle]  WITH CHECK ADD FOREIGN KEY([CajaId])
REFERENCES [dbo].[Caja] ([CajaId])
GO
ALTER TABLE [dbo].[Compras]  WITH CHECK ADD FOREIGN KEY([CompaniaId])
REFERENCES [dbo].[Compania] ([CompaniaId])
GO
ALTER TABLE [dbo].[Compras]  WITH CHECK ADD FOREIGN KEY([ProveedorId])
REFERENCES [dbo].[Proveedor] ([ProveedorId])
GO
ALTER TABLE [dbo].[DetaLiquidaVenta]  WITH CHECK ADD FOREIGN KEY([LiquidacionId])
REFERENCES [dbo].[LiquidacionVenta] ([LiquidacionId])
GO
ALTER TABLE [dbo].[DetaLiquidaVenta]  WITH CHECK ADD FOREIGN KEY([NotaId])
REFERENCES [dbo].[NotaPedido] ([NotaId])
GO
ALTER TABLE [dbo].[DetalleCompra]  WITH CHECK ADD FOREIGN KEY([CompraId])
REFERENCES [dbo].[Compras] ([CompraId])
GO
ALTER TABLE [dbo].[DetalleDocumento]  WITH CHECK ADD FOREIGN KEY([DocuId])
REFERENCES [dbo].[DocumentoVenta] ([DocuId])
GO
ALTER TABLE [dbo].[DetalleDocumento]  WITH CHECK ADD FOREIGN KEY([IdProducto])
REFERENCES [dbo].[Producto] ([IdProducto])
GO
ALTER TABLE [dbo].[DetalleGuia]  WITH CHECK ADD FOREIGN KEY([GuiaId])
REFERENCES [dbo].[GuiaRemision] ([GuiaId])
GO
ALTER TABLE [dbo].[DetalleGuia]  WITH CHECK ADD FOREIGN KEY([IdProducto])
REFERENCES [dbo].[Producto] ([IdProducto])
GO
ALTER TABLE [dbo].[DetalleGuiaInterna]  WITH CHECK ADD FOREIGN KEY([GuiaId])
REFERENCES [dbo].[GuiaInternaSI] ([GuiaId])
GO
ALTER TABLE [dbo].[DetalleLiquida]  WITH CHECK ADD FOREIGN KEY([LiquidacionId])
REFERENCES [dbo].[Liquidacion] ([LiquidacionId])
GO
ALTER TABLE [dbo].[DetallePedido]  WITH CHECK ADD FOREIGN KEY([IdProducto])
REFERENCES [dbo].[Producto] ([IdProducto])
GO
ALTER TABLE [dbo].[DetallePedido]  WITH CHECK ADD FOREIGN KEY([NotaId])
REFERENCES [dbo].[NotaPedido] ([NotaId])
GO
ALTER TABLE [dbo].[DocumentoVenta]  WITH CHECK ADD FOREIGN KEY([ClienteId])
REFERENCES [dbo].[Cliente] ([ClienteId])
GO
ALTER TABLE [dbo].[DocumentoVenta]  WITH CHECK ADD FOREIGN KEY([CompaniaId])
REFERENCES [dbo].[Compania] ([CompaniaId])
GO
ALTER TABLE [dbo].[DocumentoVenta]  WITH CHECK ADD FOREIGN KEY([NotaId])
REFERENCES [dbo].[NotaPedido] ([NotaId])
GO
ALTER TABLE [dbo].[GuiaCanje]  WITH CHECK ADD FOREIGN KEY([CompraId])
REFERENCES [dbo].[Compras] ([CompraId])
GO
ALTER TABLE [dbo].[Letra]  WITH CHECK ADD FOREIGN KEY([ProveedorId])
REFERENCES [dbo].[Proveedor] ([ProveedorId])
GO
ALTER TABLE [dbo].[NotaPedido]  WITH CHECK ADD FOREIGN KEY([ClienteId])
REFERENCES [dbo].[Cliente] ([ClienteId])
GO
ALTER TABLE [dbo].[NotaPedido]  WITH CHECK ADD FOREIGN KEY([CompaniaId])
REFERENCES [dbo].[Compania] ([CompaniaId])
GO
ALTER TABLE [dbo].[Personal]  WITH CHECK ADD FOREIGN KEY([CompaniaId])
REFERENCES [dbo].[Compania] ([CompaniaId])
GO
ALTER TABLE [dbo].[Personal]  WITH CHECK ADD  CONSTRAINT [FK_Personal_Area] FOREIGN KEY([AreaId])
REFERENCES [dbo].[Area] ([AreaId])
GO
ALTER TABLE [dbo].[Personal] CHECK CONSTRAINT [FK_Personal_Area]
GO
ALTER TABLE [dbo].[Producto]  WITH CHECK ADD FOREIGN KEY([IdSubLinea])
REFERENCES [dbo].[Sublinea] ([IdSubLinea])
GO
ALTER TABLE [dbo].[ShoppingCartItems]  WITH CHECK ADD  CONSTRAINT [FK_ShoppingCartItems_ShoppingCarts_ShoppingCartId] FOREIGN KEY([ShoppingCartId])
REFERENCES [dbo].[ShoppingCarts] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ShoppingCartItems] CHECK CONSTRAINT [FK_ShoppingCartItems_ShoppingCarts_ShoppingCartId]
GO
ALTER TABLE [dbo].[TemporalCompra]  WITH CHECK ADD FOREIGN KEY([IdProducto])
REFERENCES [dbo].[Producto] ([IdProducto])
GO
ALTER TABLE [dbo].[TemporalGuia]  WITH CHECK ADD FOREIGN KEY([IdProducto])
REFERENCES [dbo].[Producto] ([IdProducto])
GO
ALTER TABLE [dbo].[TemporalGuia]  WITH CHECK ADD FOREIGN KEY([UsuarioID])
REFERENCES [dbo].[Usuarios] ([UsuarioID])
GO
ALTER TABLE [dbo].[temporalLetra]  WITH CHECK ADD FOREIGN KEY([CompraId])
REFERENCES [dbo].[Compras] ([CompraId])
GO
ALTER TABLE [dbo].[temporalLetra]  WITH CHECK ADD FOREIGN KEY([CompraId])
REFERENCES [dbo].[Compras] ([CompraId])
GO
ALTER TABLE [dbo].[TemporalLiVenta]  WITH CHECK ADD FOREIGN KEY([NotaId])
REFERENCES [dbo].[NotaPedido] ([NotaId])
GO
ALTER TABLE [dbo].[TemporalVenta]  WITH CHECK ADD FOREIGN KEY([IdProducto])
REFERENCES [dbo].[Producto] ([IdProducto])
GO
ALTER TABLE [dbo].[TemporalVenta]  WITH CHECK ADD FOREIGN KEY([UsuarioID])
REFERENCES [dbo].[Usuarios] ([UsuarioID])
GO
ALTER TABLE [dbo].[Usuarios]  WITH CHECK ADD  CONSTRAINT [FK_Usuarios_Personal] FOREIGN KEY([PersonalId])
REFERENCES [dbo].[Personal] ([PersonalId])
GO
ALTER TABLE [dbo].[Usuarios] CHECK CONSTRAINT [FK_Usuarios_Personal]
GO
/****** Object:  StoredProcedure [dbo].[AcuentaPedido]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[AcuentaPedido]  
@NotaId varchar(38)  
as  
begin  
select  
'NroCaja|Fecha|Movimiento|Efectivo|Monto|Vuelto¬100|140|110|120|120|120¬String|String|String|String|String|String¬'+  
isnull((select STUFF((select '¬'+convert(varchar,c.CajaId)+'|'+  
Convert(char(10),c.DetalleFecha,103)+' '+Convert(char(8),c.DetalleFecha,114)+'|'+  
c.DetalleMovimiento+'|'+CONVERT(VarChar(50),cast(c.DetalleEfectivo as money ), 1)+'|'+  
CONVERT(VarChar(50),cast(c.DetalleMonto as money ), 1)+'|'+  
CONVERT(VarChar(50),cast(c.DetalleVuelto as money ), 1)  
from CajaDetalle c  
where c.NotaId=@NotaId  
order by DetalleId asc  
FOR XML PATH('')),1,1,'')),'~')+'['+  
'FechaPago|Liquidacion|Documento|SaldoDocu|Acuenta|SaldoActual¬110|125|120|120|120|120¬String|String|String|String|String|String¬'+  
isnull((select stuff((select '¬'+ Convert(char(10),d.FechaPago,103)+'|'+'LQ '+l.LiquidacionNumero+'|'+  
d.Documento+'|'+  
CONVERT(VarChar(50),cast(d.SaldoDocu as money ), 1)+'|'+  
CONVERT(VarChar(50), cast(d.AcuentaGeneral as money ), 1)+'|'+  
CONVERT(VarChar(50), cast(d.SaldoActual as money ), 1)  
from DetaLiquidaVenta d  
inner join LiquidacionVenta l  
on l.LiquidacionId=d.LiquidacionId  
where d.NotaId=@NotaId  
order by d.DetalleId asc
for xml path('')),1,1,'')),'~')  
end
GO
/****** Object:  StoredProcedure [dbo].[anularDocumento]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[anularDocumento]              
@ListaOrden varchar(Max)              
as              
begin    
             
Declare @posA1 int,@posA2 int,@posA3 int      
Declare @orden varchar(max),      
        @detalle varchar(max),      
        @Guia varchar(max)      
Set @posA1 = CharIndex('[',@ListaOrden,0)      
Set @posA2 = CharIndex('[',@ListaOrden,@posA1+1)      
Set @posA3 =Len(@ListaOrden)+1      
Set @orden = SUBSTRING(@ListaOrden,1,@posA1-1)      
Set @detalle = SUBSTRING(@ListaOrden,@posA1+1,@posA2-@posA1-1)      
Set @Guia=SUBSTRING(@ListaOrden,@posA2+1,@posA3-@posA2-1)    
              
declare @p1 int,@p2 int,              
        @p3 int,@p4 int              
declare @DocuId numeric(38),              
@NotaId numeric(38),              
@DocuUsuario varchar(80),              
--@DetalleId numeric(38),              
@Documento varchar(40)              
Set @orden= LTRIM(RTrim(@orden))              
Set @p1 = CharIndex('|',@orden,0)              
Set @p2 = CharIndex('|',@orden,@p1+1)              
Set @p3 = CharIndex('|',@orden,@p2+1)              
Set @p4 = Len(@orden)+1              
Set @DocuId=convert(numeric(38),SUBSTRING(@orden,1,@p1-1))              
Set @NotaId=convert(numeric(38),SUBSTRING(@orden,@p1+1,@p2-@p1-1))              
Set @DocuUsuario=SUBSTRING(@orden,@p2+1,@p3-@p2-1)              
set @Documento=SUBSTRING(@orden,@p3+1,@p4-@p3-1)              
      
--set @DetalleId=isnull((select top 1 d.DetalleId from CajaDetalle d              
--where d.NotaId=@NotaId               
--order by d.DetalleId desc),0)              
      
Begin Transaction              
update DocumentoVenta              
set DocuEstado='ANULADO'              
where DocuId=@DocuId              
      
update NotaPedido set ModificadoPor=@DocuUsuario,              
FechaEdita=(IsNull(convert(varchar,GETDATE(),103),'')+' '+ IsNull(SUBSTRING(convert(varchar,GETDATE(),114),1,8),'')),              
NotaEstado='ANULADO',              
NotaSaldo=NotaPagar,NotaAcuenta=0               
where NotaId=@NotaId      
              
delete from CajaDetalle              
where NotaId=@NotaId             
      
Declare Tabla Cursor For Select * From fnSplitString(@detalle,';')               
Open Tabla              
Declare @Columna varchar(max),              
  @IdProducto numeric(20),            
  @CodigoPro varchar(200),              
  @Cantidad decimal(18,2),              
  @Costo decimal(18,4),              
  @Precio decimal(18,2),              
  @IniciaStock decimal(18,2),              
  @StockFinal decimal(18,2),        
  @ValorUM decimal(18,4),  
  @AplicaINV nvarchar(1)              
Declare @d1 int,@d2 int,        
        @d3 int,@d4 int,  
        @d5 int,@d6 int,@d7 int              
Fetch Next From Tabla INTO @Columna              
 While @@FETCH_STATUS = 0              
 Begin              
Set @d1 = CharIndex('|',@Columna,0)              
Set @d2 = CharIndex('|',@Columna,@d1+1)              
Set @d3 = CharIndex('|',@Columna,@d2+1)            
Set @d4 = CharIndex('|',@Columna,@d3+1)        
Set @d5 = CharIndex('|',@Columna,@d4+1)  
Set @d6 = CharIndex('|',@Columna,@d5+1)                  
Set @d7 = Len(@Columna)+1              
            
Set @IdProducto=Convert(numeric(38),SUBSTRING(@Columna,1,@d1-1))            
Set @CodigoPro=SUBSTRING(@Columna,@d1+1,@d2-(@d1+1))               
Set @Cantidad=Convert(decimal(18,2),SUBSTRING(@Columna,@d2+1,@d3-(@d2+1)))              
Set @Precio=Convert(decimal(18,2),SUBSTRING(@Columna,@d3+1,@d4-(@d3+1)))              
Set @Costo=Convert(decimal(18,4),SUBSTRING(@Columna,@d4+1,@d5-(@d4+1)))        
Set @ValorUM=Convert(decimal(18,4),SUBSTRING(@Columna,@d5+1,@d6-(@d5+1)))  
Set @AplicaINV=SUBSTRING(@Columna,@d6+1,@d7-(@d6+1))                 
             
   if(@AplicaINV='S')            
   BEGIN        
           
   Declare @CantidadING decimal(18,2)        
   set @CantidadING=@Cantidad * @ValorUM         
                
   set @IniciaStock=(select top 1 ProductoCantidad from Producto where IdProducto=@IdProducto)              
   set @StockFinal=@IniciaStock+@CantidadING            
              
   insert into Kardex values(@IdProducto,GETDATE(),'Anulacion por Venta',@Documento,@IniciaStock,              
   @CantidadING,0,@Costo,@StockFinal,'INGRESO',@DocuUsuario)              
              
   update producto               
   set  ProductoCantidad =ProductoCantidad + @CantidadING              
   where IDProducto=@IdProducto        
                 
   END              
            
Fetch Next From Tabla INTO @Columna              
end              
 Close Tabla;              
 Deallocate Tabla;                 
 Commit Transaction;      
 select 'true'                  

end  
GO
/****** Object:  StoredProcedure [dbo].[ap_insertarCanje]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[ap_insertarCanje]
@LetraId  numeric(38),
@CompraId numeric(38),
@Documento varchar(60),
@Moneda varchar(60),
@Monto    varchar(80)
as
begin
insert into DocumentoCanje values(@LetraId,@CompraId,@Documento,@Moneda,@Monto)
end
GO
/****** Object:  StoredProcedure [dbo].[ap_Reimprimir]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[ap_Reimprimir] 
@NotaId numeric(38),
@Usuario varchar(60)
as
begin
begin
update DetallePedido
set DetalleEstado='PENDIENTE'
where NotaId=@NotaId
end
begin
update NotaPedido
set NotaDocu='PROFORMA V',NotaEstado='PENDIENTE',
NotaSerie='',NotaNumero='',ModificadoPor=@Usuario
where NotaId=@NotaId
end
end
GO
/****** Object:  StoredProcedure [dbo].[ap_xEntregar]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[ap_xEntregar]
as
begin
select 
'Codigo|RazonSocial|Direccion|Telefono¬80|355|80|80¬String|String|String|String¬'+
isnull((select STUFF((select '¬'+convert(varchar,c.ClienteId)+'|'+c.ClienteRazon+'|'+c.ClienteDespacho+'|'+c.ClienteTelefono
from DetallePedido d
inner join NotaPedido n
on n.NotaId=d.NotaId
inner join cliente c
on c.ClienteId=n.ClienteId
where d.cantidadSaldo>0 and (n.NotaEstado<>'ANULADO' and n.NotaEntrega='POR ENTREGAR')
group by c.ClienteId,c.ClienteRazon,c.ClienteDespacho,c.ClienteTelefono
order by c.ClienteRazon asc
for xml path('')),1,1,'')),'~')
end
GO
/****** Object:  StoredProcedure [dbo].[aumentarStockCompra]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[aumentarStockCompra]
@IdProducto numeric(38),
@Cantidad decimal(18,2),
@Costo decimal(18,4),
@Estado varchar(40),
@Documento varchar(80),
@usuario varchar(80)
as
begin
declare @IniciaStock decimal(18,2),@stockFinal decimal(18,2)
set @IniciaStock=(select top 1 p.ProductoCantidad from Producto p where p.IdProducto=@IdProducto)
set @stockFinal=@IniciaStock+@Cantidad
if(@Estado='BONIFICACION')
begin
update Producto 
set ProductoCantidad=ProductoCantidad+@Cantidad
where IdProducto=@IdProducto 
end
else
begin
update Producto 
set ProductoCantidad=ProductoCantidad+@Cantidad,ProductoCosto=@Costo
where IdProducto=@IdProducto 
end
insert into Kardex values(@IdProducto,GETDATE(),'Ingreso por Compra',@Documento,@IniciaStock,
@Cantidad,0,@Costo,@StockFinal,'INGRESO',@Usuario)
end
GO
/****** Object:  StoredProcedure [dbo].[aumentaSaldo]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[aumentaSaldo]
@Cantidad decimal(18,2),
@IdDetalle numeric(38)
as
update DetallePedido
set CantidadSaldo=CantidadSaldo+@Cantidad
where DetalleId=@IdDetalle
GO
/****** Object:  StoredProcedure [dbo].[buscarProducto]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[buscarProducto]
@Descripcion varchar(250) 
as                
begin                
select                 
'IdPro|Categoria|Codigo|Descripcion|Cantidad|Pre_Venta|Pre_VentaB|Stock|U_Medida|Pre_Costo|ValorUM|ValorCritico|AplicaINV¬100|100|100|100|100|100|100|100|100|100|100|100|100¬String|String|String|String|String|String|String|String|String|String|String|String|String¬'+                
isnull((select STUFF((select top 70 '¬'+      
convert(varchar,p.IdProducto)+'|'+      
s.NombreSublinea+'|'+                
p.ProductoCodigo+'|'+      
p.ProductoNombre+'||'+                
CONVERT(VarChar(50), cast(p.ProductoVenta as money ), 1)+'|'+                
CONVERT(VarChar(50), cast(p.ProductoVentaB as money ), 1)+'|'+                
CONVERT(VarChar(50), cast(p.ProductoCantidad as money ), 1)+'|'+                
p.ProductoUM+'|'+                
convert(varchar,p.ProductoCosto)+'|1|'+                
convert(varchar,p.ValorCritico)+'|'+p.AplicaINV                
FROM Producto p (nolock)                
INNER JOIN Sublinea s (nolock)                
ON p.IdSubLinea =s.IdSubLinea                 
where (p.ProductoNombre like'%'+@Descripcion+'%')and p.ProductoEstado='BUENO'                
order by p.ProductoNombre asc                
FOR XML path ('')),1,1,'')),'~')+'¬'+                
isnull((select STUFF((select top 70'¬'+      
convert(varchar,p.IdProducto)+'|'+                
s.NombreSublinea+'|'+                
p.ProductoCodigo+'|'+      
p.ProductoNombre+'||'+                
CONVERT(VarChar(50), cast(u.PrecioVenta as money ), 1)+'|'+                
CONVERT(VarChar(50), cast(u.PrecioVentaB as money ), 1)+'|'+                
CONVERT(VarChar(50),cast((p.ProductoCantidad/u.ValorUM)as money ), 1)+'|'+                
u.UMDescripcion+'|'+convert(varchar,u.PrecioCosto)+'|'+      
convert(varchar,u.ValorUM)+'|'+      
convert(varchar,p.ValorCritico)+'|'+p.AplicaINV                
from UnidadMedida u (nolock)                
inner join Producto p (nolock)                
on p.IdProducto=u.IdProducto                
INNER JOIN Sublinea s (nolock)                
ON p.IdSubLinea =s.IdSubLinea                 
where (p.ProductoNombre like'%'+@Descripcion+'%') and p.ProductoEstado='BUENO'                
order by p.ProductoNombre asc                
FOR XML path ('')),1,1,'')),'~')                
end 
GO
/****** Object:  StoredProcedure [dbo].[buscarSubLinea]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[buscarSubLinea] 
@IdSubLinea numeric(20)
as
begin
select p.IdProducto,l.NombreLinea,s.NombreSublinea,p.ProductoCodigo,p.ProductoNombre,
p.ProductoMarca,p.ProductoNombre+' '+p.ProductoMarca as Descripcion,CONVERT(VarChar(50), cast(p.ProductoCantidad as money ), 1) as ProductoCantidad, 
p.ProductoUM,CONVERT(VarChar(50), cast(p.ProductoVenta as money ), 1)as ProductoVenta,CONVERT(VarChar(50), cast(p.ProductoVentaB as money ), 1)as ProductoVentaB,p.ProductoCosto as PrecioCosto,p.ProductoCostoDolar as CostoDolar,p.ProductoTipoCambio as TipoCambio, 
a.AlmacenNombre,p.ProductoUbicacion,p.ProductoObs,p.ProductoEstado,p.ProductoUsuario,'1' as ValorUM,p.ProductoImagen,p.ValorCritico
FROM Producto p
INNER JOIN Sublinea s
ON p.IdSubLinea =s.IdSubLinea 
INNER JOIN Linea l
ON s.IdLinea =l.IdLinea 
INNER JOIN Almacen a
ON p.AlmacenId =a.AlmacenId
where p.IdSubLinea=@IdSubLinea
union all(
select p.IdProducto,l.NombreLinea,s.NombreSublinea,p.ProductoCodigo,p.ProductoNombre,
p.ProductoMarca,p.ProductoNombre+' '+p.ProductoMarca as Descripcion,CONVERT(VarChar(50), cast((p.ProductoCantidad/u.ValorUM)as money ), 1) as ProductoCantidad, 
u.UMDescripcion,CONVERT(VarChar(50), cast(u.PrecioVenta as money ), 1)as ProductoVenta,CONVERT(VarChar(50), cast(u.PrecioVentaB as money ), 1)as ProductoVentaB,u.PrecioCosto,'0' as CostoDolar,'0' as TipoCambio, 
a.AlmacenNombre,p.ProductoUbicacion,p.ProductoObs,p.ProductoEstado,p.ProductoUsuario,u.ValorUM,p.ProductoImagen,p.ValorCritico
from UnidadMedida u
inner join Producto p
on p.IdProducto=u.IdProducto
INNER JOIN Sublinea s
ON p.IdSubLinea =s.IdSubLinea 
INNER JOIN Linea l
ON s.IdLinea =l.IdLinea 
INNER JOIN Almacen a
ON p.AlmacenId =a.AlmacenId
where p.IdSubLinea=@IdSubLinea)
order by 7 asc
end
GO
/****** Object:  StoredProcedure [dbo].[cajaPrincipal]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[cajaPrincipal]
as
begin
select
'ID|Concepto|CajaId|Fecha|Descripcion|Monto|Usuario|Referencia|GastoId¬90|100|80|136|212|120|100|100|100¬String|String|String|String|String|String|String|String|String¬'+
isnull((select STUFF((select '¬'+ convert(varchar,c.IdCaja)+'|'+c.CajaConcepto+'|'+convert(varchar,c.CajaId)+'|'+
Convert(char(10),c.CajaFecha,103)+' '+Convert(char(8),c.CajaFecha,114) 
+'|'+c.CajaDescripcion+'|'+CONVERT(VarChar(50),cast(c.CajaMonto as money), 1)+'|'+
c.CajaUsuario+'|'+c.Referencia+'|'+c.GastoId 
from CajaPincipal c 
where c.CajaConcepto='INGRESO' and c.IdGeneral=0
order by c.IdCaja desc
for xml path('')),1,1,'')),'~')+'['+
'ID|Concepto|CajaId|Fecha|Descripcion|Monto|Usuario|Referencia|GastoId¬90|100|80|135|290|125|100|100|100¬String|String|String|String|String|String|String|String|String¬'+
isnull((select STUFF((select '¬'+ convert(varchar,c.IdCaja)+'|'+c.CajaConcepto+'|'+convert(varchar,c.CajaId)+'|'+
Convert(char(10),c.CajaFecha,103)+' '+Convert(char(8),c.CajaFecha,114) 
+'|'+c.CajaDescripcion+'|'+CONVERT(VarChar(50),cast(c.CajaMonto as money), 1)+'|'+
c.CajaUsuario+'|'+c.Referencia+'|'+c.GastoId  
from CajaPincipal c 
where c.CajaConcepto='SALIDA' and c.IdGeneral=0
order by c.IdCaja desc
for xml path('')),1,1,'')),'~')+'['+
'Codigo|FechaCierre|Usuario|Ingresos|Salidas|Total¬100|140|150|130|130|130¬String|String|String|String|String|String¬'+
isnull((select STUFF ((select '¬'+ CONVERT(varchar,c.IdGeneral)+'|'+
(IsNull(convert(varchar,c.FechaCierre,103),'')+' '+ IsNull(SUBSTRING(convert(varchar,c.FechaCierre,114),1,8),''))+'|'+c.Usuario+'|'+
CONVERT(varchar(50),cast(c.Ingresos as money),1)+'|'+CONVERT(varchar(50),cast(c.Salidas as money),1)+'|'+
CONVERT(varchar(50),cast(c.Total as money),1)
from CajaGeneral c
order by c.IdGeneral desc
for xml path('')),1,1,'')),'~')
end
GO
/****** Object:  StoredProcedure [dbo].[canjearGuia]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[canjearGuia] 
@ProveedorId numeric(38)
as
begin
select
'CompraId|FechaEmision|Documento|Moneda|Saldo|Monto|Estado¬100|110|150|90|120|120|150¬String|String|String|String|String|String|String¬'+
isnull((select STUFF((select '¬'+ convert(varchar,c.CompraId)+'|'+(Convert(char(10),c.CompraEmision,103))+'|'+
SUBSTRING(t.TipoDescripcion,1,1)+'C '+ c.CompraSerie+'-'+c.CompraNumero+'|'+c.CompraMoneda+'|'+
(convert(varchar(50), CAST(c.CompraSaldo as money), -1))+'|'+
(convert(varchar(50), CAST(c.CompraTotal as money), -1))+'|'+
c.CompraEstado
from Compras c
inner join TipoComprobante t
on t.TipoCodigo=c.TipoCodigo
where c.ProveedorId=@ProveedorId and c.TipoCodigo='09'
order by c.CompraEmision desc
for xml path('')),1,1,'')),'~')	
end
GO
/****** Object:  StoredProcedure [dbo].[CanjeFacturaFecha]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[CanjeFacturaFecha]
@fechainicio date,
@fechafin date
as
begin
SELECT dbo.GuiaCanje.*, dbo.Compras.CompraMoneda as Moneda,(convert(varchar(50), CAST(dbo.Compras.CompraValorVenta as money), -1))as Total,
(SUBSTRING(dbo.Compras.CompraMoneda,1,1)+'/.  '+(convert(varchar(50), CAST(dbo.Compras.CompraTotal as money), -1)))as Monto,dbo.Proveedor.ProveedorRazon as Proveedor
FROM dbo.GuiaCanje INNER JOIN dbo.Compras ON dbo.GuiaCanje.CompraId = dbo.Compras.CompraId inner join dbo.Proveedor on dbo.Proveedor.ProveedorId=dbo.Compras.ProveedorId 
where (Convert(char(10),dbo.GuiaCanje.CanjeFecha,103) BETWEEN @fechainicio AND @fechafin) 
order by dbo.GuiaCanje.CanjeId desc
end
GO
/****** Object:  StoredProcedure [dbo].[cargaPrincipal]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[cargaPrincipal]
as
begin
select
isnull((select STUFF((select '¬'+ convert(varchar,c.CompaniaId)+'|'+
c.CompaniaRazonSocial
from Compania c 
order by c.CompaniaId asc 
FOR XML PATH('')),1,1,'')),'~')+'['+
isnull((select STUFF((select '¬'+ convert(varchar,s.IdSubLinea)+'|'+
s.NombreSublinea 
from Sublinea s 
where s.NombreSublinea<>''
order by s.NombreSublinea asc
FOR XML PATH('')),1,1,'')),'~')+'['+
isnull((select STUFF((select '¬'+ t.TipoCodigo+'|'+
t.TipoDescripcion
from TipoComprobante t
order by t.TipoCodigo asc
FOR XML PATH('')),1,1,'')),'~')+'['+
isnull((select STUFF((select '¬'+ convert(varchar,a.AreaId)+'|'+
a.AreaNombre 
from Area a
order by a.AreaNombre asc
FOR XML PATH('')),1,1,'')),'~')
end
GO
/****** Object:  StoredProcedure [dbo].[ClientesAtendidos]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[ClientesAtendidos]
@ANNO INT,
@VENDEDOR VARCHAR(40)
as
begin
select MONTH(N.NotaFecha)as Numero,
(DATENAME(month,n.NotaFecha)) as Mes,n.NotaUsuario as Usuario,
COUNT(ClienteId) as Clientes
from NotaPedido n
where YEAR(n.NotaFecha)=@ANNO and (n.NotaUsuario=@VENDEDOR and n.NotaEstado='CANCELADO')
group by MONTH(N.NotaFecha),(DATENAME(month,n.NotaFecha)),n.NotaUsuario
order by 1 asc
end
GO
/****** Object:  StoredProcedure [dbo].[consultarActivacion]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[consultarActivacion]
@IdPersonal int
as
begin
declare @data varchar(80)
set @data=isnull((select top 1 a.Usuario as Cant
from Desbloqueo d
inner join AdministraBL a
on a.IdAdmin=d.IdAdmin
where IdPersonal=@IdPersonal and convert(date,d.Fecha)=convert(date,GETDATE())),'~')
select @data
end
GO
/****** Object:  StoredProcedure [dbo].[CorrelativoCompra]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[CorrelativoCompra]
@CompaniaId int,@anno int,@mes int
as
begin
select top 1 c.CompraCorrelativo as Correlativo from Compras c
where CompaniaId=@CompaniaId and (year(CompraComputo)=@anno and MONTH(CompraComputo)=@mes)
order by c.CompraCorrelativo desc
end
GO
/****** Object:  StoredProcedure [dbo].[CorrelativoLiVenta]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[CorrelativoLiVenta]
as
begin
declare @cod varchar(12)
select @cod=dbo.geneneraIdLiVenta('001-')
SELECT TOP 1 @cod  AS ID FROM LiquidacionVenta
end
GO
/****** Object:  StoredProcedure [dbo].[correlativoNroFactura]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[correlativoNroFactura]@dato varchar(20),@CompaniaId int,@DocuDocumento varchar(40)
as
begin
declare @cod varchar(13)
select @cod=dbo.genenerarNroFactura(@dato,@CompaniaId,@DocuDocumento)
SELECT TOP 1 @cod  AS ID FROM DocumentoVenta
end
GO
/****** Object:  StoredProcedure [dbo].[correlativoNroGuia]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[correlativoNroGuia] @dato varchar(20)
as
begin
declare @cod varchar(11)
select @cod=dbo.genenerarNroGuia(@dato)
SELECT TOP 1 @cod  AS ID FROM GuiaRemision
end
GO
/****** Object:  StoredProcedure [dbo].[CuentaCorrienteCliente]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[CuentaCorrienteCliente]
as
begin
select
'ClienteId|Cliente|SaldoSol¬100|525|140¬String|String|String¬'+
isnull((select stuff((select '¬'+ convert(varchar,c.ClienteId)+'|'+c.ClienteRazon+'|'+
CONVERT(VarChar(50), cast(sum(n.NotaSaldo)as money ), 1)
from NotaPedido n
inner join Cliente c
on c.ClienteId=n.ClienteId
where (n.NotaSaldo>0 and n.NotaEstado<>'CANCELADO') and n.NotaCondicion='CREDITO'
group by c.ClienteId,c.ClienteRazon
order by c.ClienteRazon asc
for xml path('')),1,1,'')),'~')
end
GO
/****** Object:  StoredProcedure [dbo].[CuentaCorrienteProCom]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[CuentaCorrienteProCom] 
@CompaniaId varchar(40)
as
begin
select isnull(SC.ProveedorId,ISNULL(DC.ProveedorId,ISNULL(LS.ProveedorId,LD.ProveedorId))) as ProveedorId
,isnull(SC.RazonSocial,ISNULL(DC.RazonSocial,ISNULL(LS.RazonSocial,LD.RazonSocial))) as ProveedorRazon,
convert(varchar(50),cast((isnull(Sum(DC.SaldoDol),0)+ isnull(sum(LD.SaldoDolLe),0))as money),1)as SaldoDol,
convert(varchar(50),cast((isnull(Sum(SC.SaldoSol),0)+ isnull(sum(LS.SaldoSolLe),0))as money),1)as SaldoSol
from
(
    select p.ProveedorId,p.ProveedorRazon as RazonSocial,Sum(c.CompraSaldo)as SaldoSol
	from Proveedor p
	inner join Compras c
	on c.ProveedorId=p.ProveedorId
	where c.CompaniaId=@CompaniaId and (c.CompraMoneda='SOLES' and c.CompraEstado='PENDIENTE DE PAGO')
	group by p.ProveedorId,p.ProveedorRazon
) SC
full join(
  select p.ProveedorId,p.ProveedorRazon as RazonSocial,Sum(c.CompraSaldo)as SaldoDol
	from Proveedor p
	inner join Compras c
	on c.ProveedorId=p.ProveedorId
	where c.CompaniaId=@CompaniaId and (c.CompraMoneda='DOLARES' and c.CompraEstado='PENDIENTE DE PAGO')
	group by p.ProveedorId,p.ProveedorRazon
)DC ON SC.ProveedorId=DC.ProveedorId
full join(
select p.ProveedorId,p.ProveedorRazon as RazonSocial,
		Sum(d.DetalleSaldo)as SaldoSolLe
	from Proveedor p
	inner join Letra l
	on l.ProveedorId=p.ProveedorId
	inner join DetalleLetra d
	on d.LetraId=l.LetraId
	where l.CompaniaId=@CompaniaId and(l.LetraMoneda='SOLES' and d.DetalleEstado='PENDIENTE')
group by p.ProveedorId,p.ProveedorRazon
)LS ON LS.ProveedorId=SC.ProveedorId
full join(
select p.ProveedorId,p.ProveedorRazon as RazonSocial,
		Sum(d.DetalleSaldo)as SaldoDolLe
	from Proveedor p
	inner join Letra l
	on l.ProveedorId=p.ProveedorId
	inner join DetalleLetra d
	on d.LetraId=l.LetraId
	where l.CompaniaId=@CompaniaId and (l.LetraMoneda='DOLARES' and d.DetalleEstado='PENDIENTE')
group by p.ProveedorId,p.ProveedorRazon
)LD ON LS.ProveedorId=LD.ProveedorId
GROUP BY SC.ProveedorId,DC.ProveedorId,LS.ProveedorId,LD.ProveedorId,
		 SC.RazonSocial,DC.RazonSocial,LS.RazonSocial,LD.RazonSocial
order by 2 asc
end
GO
/****** Object:  StoredProcedure [dbo].[CuentaCorrienteProveedor]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[CuentaCorrienteProveedor]  
as  
begin  
select 
isnull(SC.ProveedorId,ISNULL(DC.ProveedorId,ISNULL(LS.ProveedorId,LD.ProveedorId))) as ProveedorId  
,isnull(SC.RazonSocial,ISNULL(DC.RazonSocial,ISNULL(LS.RazonSocial,LD.RazonSocial))) as ProveedorRazon,  
convert(varchar(50),cast((isnull(Sum(DC.SaldoDol),0)+ isnull(sum(LD.SaldoDolLe),0))as money),1)as SaldoDol,  
convert(varchar(50),cast((isnull(Sum(SC.SaldoSol),0)+ isnull(sum(LS.SaldoSolLe),0))as money),1)as SaldoSol  
from  
(  
 select p.ProveedorId,p.ProveedorRazon as RazonSocial,Sum(c.CompraSaldo)as SaldoSol  
 from Proveedor p  
 inner join Compras c  
 on c.ProveedorId=p.ProveedorId  
 where c.CompraMoneda='SOLES' and c.CompraEstado='PENDIENTE DE PAGO'  
 group by p.ProveedorId,p.ProveedorRazon  
) SC  
full join(  
  select p.ProveedorId,p.ProveedorRazon as RazonSocial,Sum(c.CompraSaldo)as SaldoDol  
 from Proveedor p  
 inner join Compras c  
 on c.ProveedorId=p.ProveedorId  
 where c.CompraMoneda='DOLARES' and c.CompraEstado='PENDIENTE DE PAGO'  
 group by p.ProveedorId,p.ProveedorRazon  
)DC ON SC.ProveedorId=DC.ProveedorId  
full join(  
select p.ProveedorId,p.ProveedorRazon as RazonSocial,  
  Sum(d.DetalleSaldo)as SaldoSolLe  
 from Proveedor p  
 inner join Letra l  
 on l.ProveedorId=p.ProveedorId  
 inner join DetalleLetra d  
 on d.LetraId=l.LetraId  
 where l.LetraMoneda='SOLES' and d.DetalleEstado='PENDIENTE'  
group by p.ProveedorId,p.ProveedorRazon  
)LS ON LS.ProveedorId=SC.ProveedorId  
full join(  
select p.ProveedorId,p.ProveedorRazon as RazonSocial,  
  Sum(d.DetalleSaldo)as SaldoDolLe  
 from Proveedor p  
 inner join Letra l  
 on l.ProveedorId=p.ProveedorId  
 inner join DetalleLetra d  
 on d.LetraId=l.LetraId  
 where l.LetraMoneda='DOLARES' and d.DetalleEstado='PENDIENTE'  
group by p.ProveedorId,p.ProveedorRazon  
)LD ON LS.ProveedorId=LD.ProveedorId  
GROUP BY SC.ProveedorId,DC.ProveedorId,LS.ProveedorId,LD.ProveedorId,  
SC.RazonSocial,DC.RazonSocial,LS.RazonSocial,LD.RazonSocial  
order by 2 asc  
end
GO
/****** Object:  StoredProcedure [dbo].[CuentasCorreienteCompania]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[CuentasCorreienteCompania]
as
begin
select 
isnull(SC.CompaniaId,ISNULL(DC.CompaniaId,ISNULL(LS.CompaniaId,LD.CompaniaId))) as CompaniaId
,isnull(SC.RazonSocial,ISNULL(DC.RazonSocial,ISNULL(LS.RazonSocial,LD.RazonSocial))) as RazonSocial,
convert(varchar(50),cast((isnull(Sum(DC.SaldoDol),0)+ isnull(sum(LD.SaldoDolLe),0))as money),1)as SaldoDol,
convert(varchar(50),cast((isnull(Sum(SC.SaldoSol),0)+ isnull(sum(LS.SaldoSolLe),0))as money),1)as SaldoSol
from
(
select co.CompaniaId,co.CompaniaRazonSocial as RazonSocial,
sum(c.CompraSaldo)SaldoSol
from Compania co
inner join Compras c
on c.CompaniaId=co.CompaniaId
where c.CompraMoneda='SOLES' AND c.CompraEstado='PENDIENTE DE PAGO'
group by co.CompaniaId,co.CompaniaRazonSocial
) SC
FULL JOIN 
(
select co.CompaniaId,co.CompaniaRazonSocial as RazonSocial,
sum(c.CompraSaldo)as SaldoDol
from Compania co
inner join Compras c
on c.CompaniaId=co.CompaniaId
where c.CompraMoneda='DOLARES' AND c.CompraEstado='PENDIENTE DE PAGO'
group by co.CompaniaId,co.CompaniaRazonSocial
)DC ON DC.CompaniaId=SC.CompaniaId
full join
(
select l.CompaniaId,co.CompaniaRazonSocial as RazonSocial,SUM(d.DetalleSaldo) as SaldoSolLe
from DetalleLetra d
inner join Letra l
on l.LetraId=d.LetraId
inner join Compania co
on co.CompaniaId=l.CompaniaId
where d.DetalleEstado='PENDIENTE' and l.LetraMoneda='SOLES'
group by l.CompaniaId,co.CompaniaRazonSocial
)LS on LS.CompaniaId=SC.CompaniaId
full join(
select l.CompaniaId,co.CompaniaRazonSocial as RazonSocial,SUM(d.DetalleSaldo) as SaldoDolLe
from DetalleLetra d
inner join Letra l
on l.LetraId=d.LetraId
inner join Compania co
on co.CompaniaId=l.CompaniaId
where d.DetalleEstado='PENDIENTE' and l.LetraMoneda='DOLARES'
group by l.CompaniaId,co.CompaniaRazonSocial
)LD on LD.CompaniaId=LS.CompaniaId
GROUP BY SC.CompaniaId,DC.CompaniaId,LS.CompaniaId,LD.CompaniaId,
		 SC.RazonSocial,DC.RazonSocial,LS.RazonSocial,LD.RazonSocial
order by 2 asc
end
GO
/****** Object:  StoredProcedure [dbo].[DeudaCliente]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[DeudaCliente] 
@Cliente varchar(20)  
as  
begin  
select  
'ClienteId|FechaEmision|Documento|Vencimiento|Moneda|SaldoDocu|MontoDocu¬100|105|140|105|90|120|120¬String|String|String|String|String|String|String¬'+  
isnull((select STUFF((select '¬'+convert(varchar,n.ClienteId)+'|'+(Convert(char(10),n.NotaFecha,103))+'|'+  
n.NotaSerie+'-'+n.NotaNumero+'|'+  
(Convert(char(10),n.NotaFechaPago,103))+'|'+'SOLES'+'|'+convert(varchar(50),cast(n.NotaSaldo as money),1)+'|'+  
convert(varchar(50),cast(n.NotaPagar as money),1)   
from NotaPedido n  
where n.notadocu<>'PROFORMA' and (n.ClienteId=@Cliente and ((n.NotaSaldo>0 and n.NotaEstado<>'CANCELADO') and n.NotaCondicion='CREDITO'))  
order by n.NotaId desc  
for xml path('')),1,1,'')),'~')  
end
GO
/****** Object:  StoredProcedure [dbo].[DeudasProveedor]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[DeudasProveedor] 
@ProveedorId varchar(20)  
as  
begin  
select 
'Id|FechaEmision|Documento|Vencimiento|Moneda|TipoCambio|SaldoDocu|MontoDocu¬90|90|90|90|90|90|90|90¬String|String|String|String|String|String|String|String¬'+
isnull((select STUFF((select '¬'+convert(varchar,c.ProveedorId)+'|'+
convert(varchar,c.CompraId)+'|'+  
(Convert(char(10),c.CompraEmision,103))+'|'+  
substring(t.TipoDescripcion,1,1)+'C '+c.CompraSerie+'-'+c.CompraNumero+'|'+  
(Convert(char(10),c.CompraFechaPago,103))+'|'+  
c.CompraMoneda+'|'+  
convert(varchar,c.CompraTipoCambio)+'|'+  
convert(varchar(50),cast(c.CompraSaldo as money),1)+'|'+  
convert(varchar(50),cast(c.CompraTotal as money),1)  
from Compras c  
inner join TipoComprobante t  
on t.TipoCodigo=c.TipoCodigo  
where c.ProveedorId=@ProveedorId and c.CompraEstado='PENDIENTE DE PAGO'  
FOR XML path ('')),1,1,'')),'~')+'¬'+
isnull((select STUFF((select '¬'+
convert(varchar,l.ProveedorId)+'|'+
convert(varchar,d.LetraId)+'|'+
(Convert(char(10),l.LetraFechaGiro,103))+'|'+  
'LT '+d.LetraCanje+'|'+
(Convert(char(10),d.LetraVencimiento,103))+'|'+  
l.LetraMoneda+'|'+'3.276|'+  
convert(varchar(50),cast(d.DetalleSaldo as money),1)+'|'+  
convert(varchar(50),cast(d.DetalleMonto as money),1)  
from DetalleLetra d  
inner join Letra l  
on l.LetraId=d.LetraId  
where l.ProveedorId=@ProveedorId and d.DetalleEstado='PENDIENTE'
FOR XML path ('')),1,1,'')),'~')  
end
GO
/****** Object:  StoredProcedure [dbo].[DeudasProveedorA]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[DeudasProveedorA]
as
begin
select c.CompraId,(Convert(char(10),c.CompraEmision,103)) as CompraEmision,substring(t.TipoDescripcion,1,1)+'C '+c.CompraSerie+'-'+c.CompraNumero as Documento,
(Convert(char(10),c.CompraFechaPago,103)) as Vencimiento,c.CompraMoneda as Moneda,c.CompraTipoCambio as TipoCambio,
CONVERT(VarChar(50),cast(c.CompraSaldo as money ), 1) as SaldoDoc,CONVERT(VarChar(50),cast(c.CompraTotal as money ), 1) as MontoDoc
from Compras c
inner join TipoComprobante t
on t.TipoCodigo=c.TipoCodigo
where c.CompraEstado='PENDIENTE DE PAGO'
order by c.CompraFechaPago asc
end
GO
/****** Object:  StoredProcedure [dbo].[DeudasProveedorC]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[DeudasProveedorC]   
@Data varchar(max) 
as  
begin
Declare @p1 int,@p2 int

Declare @CompaniaId int,  
        @ProveedorId numeric(20)
Set @Data = LTRIM(RTrim(@Data))    
Set @p1 = CharIndex('|',@Data,0)    
Set @p2 =Len(@Data)+1 
Set @CompaniaId =convert(int,SUBSTRING(@Data,1,@p1-1))    
Set @ProveedorId=convert(numeric(20),SUBSTRING(@Data,@p1+1,@p2-@p1-1))  
select 
'Id|FechaEmision|Documento|Vencimiento|Moneda|TipoCambio|SaldoDocu|MontoDocu¬90|90|90|90|90|90|90|90¬String|String|String|String|String|String|String|String¬'+
isnull((select STUFF((select '¬'+convert(varchar,c.ProveedorId)+'|'+
convert(varchar,c.CompraId)+'|'+  
(Convert(char(10),c.CompraEmision,103))+'|'+  
substring(t.TipoDescripcion,1,1)+'C '+c.CompraSerie+'-'+c.CompraNumero+'|'+  
(Convert(char(10),c.CompraFechaPago,103))+'|'+  
c.CompraMoneda+'|'+  
convert(varchar,c.CompraTipoCambio)+'|'+  
convert(varchar(50),cast(c.CompraSaldo as money),1)+'|'+  
convert(varchar(50),cast(c.CompraTotal as money),1)  
from Compras c  
inner join TipoComprobante t  
on t.TipoCodigo=c.TipoCodigo  
where (c.CompaniaId=@CompaniaId and c.ProveedorId=@ProveedorId) and c.CompraEstado='PENDIENTE DE PAGO'
FOR XML path ('')),1,1,'')),'~')+'¬'+
isnull((select STUFF((select '¬'+
convert(varchar,l.ProveedorId)+'|'+
convert(varchar,d.LetraId)+'|'+
(Convert(char(10),l.LetraFechaGiro,103))+'|'+  
'LT '+d.LetraCanje+'|'+
(Convert(char(10),d.LetraVencimiento,103))+'|'+  
l.LetraMoneda+'|'+'3.276|'+  
convert(varchar(50),cast(d.DetalleSaldo as money),1)+'|'+  
convert(varchar(50),cast(d.DetalleMonto as money),1)  
from DetalleLetra d  
inner join Letra l  
on l.LetraId=d.LetraId  
where (l.CompaniaId=@CompaniaId and l.ProveedorId=@ProveedorId) and d.DetalleEstado='PENDIENTE' 
FOR XML path ('')),1,1,'')),'~') 
end
GO
/****** Object:  StoredProcedure [dbo].[editaDetaCompra]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[editaDetaCompra]  
@Data varchar(max)  
as  
begin  
Declare @pos1 int,@pos2 int,
        @pos3 int,@pos4 int,
        @pos5 int,@pos6 int  
declare @Id numeric(38),  
@cantidad decimal(18,2),  
@precioCosto decimal(18,4),  
@Descuento decimal(18,4),  
@importe decimal(18,2),  
@CompraId numeric(38)  
Set @Data = LTRIM(RTrim(@Data))  
Set @pos1 = CharIndex('|',@Data,0)
Set @pos2 = CharIndex('|',@Data,@pos1+1)
Set @pos3 = CharIndex('|',@Data,@pos2+1)
Set @pos4 = CharIndex('|',@Data,@pos3+1)
Set @pos5= CharIndex('|',@Data,@pos4+1)
Set @pos6 =Len(@Data)+1    
Set @Id =convert(numeric(38),SUBSTRING(@Data,1,@pos1-1))  
Set @cantidad=convert(decimal(18,2),SUBSTRING(@Data,@pos1+1,@pos2-@pos1-1))  
Set @precioCosto=convert(decimal(18,4),SUBSTRING(@Data,@pos2+1,@pos3-@pos2-1))   
Set @Descuento=convert(decimal(18,4),SUBSTRING(@Data,@pos3+1,@pos4-@pos3-1))  
Set @importe=convert(decimal(18,4),SUBSTRING(@Data,@pos4+1,@pos5-@pos4-1))  
Set @CompraId=convert(numeric(38),SUBSTRING(@Data,@pos5+1,@pos6-@pos5-1))  
update DetalleCompra  
set DetalleCantidad=@cantidad,PrecioCosto=@precioCosto,  
DetalleDescuento=@Descuento,DetalleImporte=@importe  
where DetalleId=@Id  
select isnull((select STUFF ((select '¬'+convert(varchar,u.IdUm)+'|'+convert(varchar,u.IdProducto)+'|'+  
u.UMDescripcion+'|'+CONVERT(VarChar(50), cast(u.ValorUM as money ), 1)+'|'+  
convert(varchar,d.PrecioCosto)  
from UnidadMedida u  
inner join DetalleCompra d  
on d.IdProducto=u.IdProducto  
where d.CompraId=@CompraId  
order by u.ValorUM asc  
for xml path('')),1,1,'')),'true')  
end
GO
/****** Object:  StoredProcedure [dbo].[editaDetaLiVenta]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[editaDetaLiVenta]
@DetalleId numeric(38),
@EntidadBanco varchar(80),
@NroOperacion varchar(80),
@FechaPago varchar(60)
as
begin
update DetaLiquidaVenta
set EntidadBanco=@EntidadBanco,NroOperacion=@NroOperacion,FechaPago=@FechaPago
where DetalleId=@DetalleId
end
GO
/****** Object:  StoredProcedure [dbo].[editaGuiacanje]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[editaGuiacanje]
@CanjeId numeric(38),
@CompraId numeric(38),
@CompaniaId int,
@CanjeFecha date,
@CanjeRegistro datetime,
@CanjeSerie varchar(80),
@CanjeNumero varchar(80),
@CanjeEmision date,
@CanjeComputo date,
@CanjeCorrelativo varchar(80),
@CanjeTipo varchar(80),
@CanjeOBS varchar(max),
@TCSunat decimal(18,3),
@Usuario varchar(80),
@Subtotal decimal(18,2),
@Igv decimal(18,2),
@Total decimal(18,2)
as
begin
update GuiaCanje
set CompaniaId=@CompaniaId,CanjeFecha=@CanjeFecha,CanjeRegistro=@CanjeRegistro,
CanjeSerie=@CanjeSerie,CanjeNumero=@CanjeNumero,CanjeEmision=@CanjeEmision,CanjeComputo=@CanjeComputo,
CanjeCorrelativo=@CanjeCorrelativo,CanjeTipo=@CanjeTipo,CanjeOBS=@CanjeOBS,TCSunat=@TCSunat,CanjeUsuario=@Usuario
where CanjeId=@CanjeId
begin
update Compras
set CompaniaId=@CompaniaId,CompraSerie=@CanjeSerie,CompraNumero=@CanjeNumero,CompraEmision=@CanjeEmision,
CompraComputo=@CanjeComputo,CompraCorrelativo=@CanjeCorrelativo,CompraTipoIgv=@CanjeTipo,CompraOBS=@CanjeOBS,
CompraTipoSunat=@TCSunat,CompraUsuario=@Usuario,CompraSubtotal=@Subtotal,CompraIgv=@Igv,CompraTotal=@Total
where CompraId=@CompraId
end
end
GO
/****** Object:  StoredProcedure [dbo].[editaNotaLD]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[editaNotaLD]  
@Data varchar(max)  
as  
begin  
declare @p0 int,@p1 int,  
  @p2 int,@p3 int,  
  @p4 int,@p5 int,  
  @p6 int,@p7 int,
  @p8 int  
declare @DetalleId numeric(38),  
  @Cantidad decimal(18,2),  
  @Costo decimal(18,2),  
  @PrecioUni decimal(18,2),  
  @Importe decimal(18,2),  
  @Ganancia decimal(18,2),  
  @UM varchar(80),  
  @IdProducto numeric(20),  
  @NotaId numeric(38),
  @Descripcion varchar(max) 
Set @Data= LTRIM(RTrim(@Data))  
set @p0 = CharIndex('|',@Data,0)  
Set @p1 = CharIndex('|',@Data,@p0+1)  
Set @p2 = CharIndex('|',@Data,@p1+1)  
Set @p3 = CharIndex('|',@Data,@p2+1)  
Set @p4 = CharIndex('|',@Data,@p3+1)  
Set @p5= CharIndex('|',@Data,@p4+1)  
Set @p6= CharIndex('|',@Data,@p5+1)
Set @p7= CharIndex('|',@Data,@p6+1)  
Set @p8=Len(@Data)+1  
Set @DetalleId=Convert(numeric(38),SUBSTRING(@Data,1,@p0-1))  
Set @Cantidad=Convert(decimal(18,2),SUBSTRING(@Data,@p0+1,@p1-(@p0+1)))  
Set @Costo= Convert(decimal(18,2),SUBSTRING(@Data,@p1+1,@p2-(@p1+1)))  
Set @PrecioUni= Convert(decimal(18,2),SUBSTRING(@Data,@p2+1,@p3-(@p2+1)))  
Set @Importe= Convert(decimal(18,2),SUBSTRING(@Data,@p3+1,@p4-(@p3+1)))  
Set @Ganancia= Convert(decimal(18,2),SUBSTRING(@Data,@p4+1,@p5-@p4-1))  
Set @UM=SUBSTRING(@Data,@p5+1,@p6-@p5-1)  
Set @IdProducto=Convert(numeric(20),SUBSTRING(@Data,@p6+1,@p7-@p6-1))
Set @Descripcion=SUBSTRING(@Data,@p7+1,@p8-@p7-1)
    
set @NotaId=(select top 1 NotaId from DetallePedido   
where DetalleId=@DetalleId) 
 
begin  
 update DetallePedido   
 set DetalleCantidad=@Cantidad,DetalleCosto=@Costo,  
 DetallePrecio=@PrecioUni,DetalleImporte=@Importe,DetalleDescripcion=@Descripcion
 where DetalleId=@DetalleId  
 
 update NotaPedido  
 set NotaGanancia=@Ganancia  
 where NotaId=@NotaId  
 
 select 'true'  
end  
end
GO
/****** Object:  StoredProcedure [dbo].[editaPrecioB]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[editaPrecioB]   
@detalle varchar(Max)  
as
begin
Begin Transaction  
Declare Tabla Cursor For Select * From fnSplitString(@detalle,';')     
Open Tabla  
Declare @Columna varchar(max),
        @IdTabla numeric(38),@IdProducto numeric(20),
        @Aviso varchar(20),@Accion varchar(20),
        @UM varchar(80),@valor decimal(18,4)
        
Declare @productoventaA decimal(18,2),
        @productoventaB decimal(18,2)
 
Declare @p1 int,@p2 int,@p3 int,@p4 int,
        @p5 int,@p6 int    
Fetch Next From Tabla INTO @Columna    
 While @@FETCH_STATUS = 0    
 Begin    
Set @p1 = CharIndex('|',@Columna,0)    
Set @p2 = CharIndex('|',@Columna,@p1+1)    
Set @p3 = CharIndex('|',@Columna,@p2+1)    
Set @p4 = CharIndex('|',@Columna,@p3+1)    
Set @p5 = CharIndex('|',@Columna,@p4+1)
Set @p6= Len(@Columna)+1

Set @IdTabla=convert(numeric(38),SUBSTRING(@Columna,1,@p1-1))
Set @IdProducto=convert(numeric(20),SUBSTRING(@Columna,@p1+1,@p2-@p1-1))    
Set @Aviso=SUBSTRING(@Columna,@p2+1,@p3-@p2-1)    
set @Accion=SUBSTRING(@Columna,@p3+1,@p4-@p3-1)    
set @UM=SUBSTRING(@Columna,@p4+1,@p5-@p4-1)  
set @valor=convert(decimal(18,4),SUBSTRING(@Columna,@p5+1,@p6-@p5-1)) 
  
if(@valor=1)  
begin  
	set @productoventaA=(select top 1 p.ProductoVenta 
	from Producto p 
	where p.IdProducto=@IdProducto)  
	set @productoventaB=(select top 1 p.ProductoVentaB 
	from Producto p where p.IdProducto=@IdProducto)  
end
else  
begin  
	set @productoventaA=isnull((select top 1 u.PrecioVenta 
	from UnidadMedida u 
	where u.IdProducto=@IdProducto and u.UMDescripcion=@UM),0)  
	set @productoventaB=isnull((select top 1 u.PrecioVentaB 
	from UnidadMedida u 
	where u.IdProducto=@IdProducto and u.UMDescripcion=@UM),0)  
end
  
if(@Accion='T')  
begin  
if @Aviso='B'  
begin
if(@productoventaB>0)
begin 
	update TemporalVenta  
	set precioventa=@productoventaB,importe=cantidad*@productoventaB  
	where temporalId=@IdTabla
end  
end  
else  
begin
if(@productoventaA>0)
begin   
	update TemporalVenta  
	set precioventa=@productoventaA,importe=cantidad*@productoventaA  
	where temporalId=@IdTabla
end 
end  
end
else  
begin  
if @Aviso='B'  
begin
if(@productoventaB>0)
begin  
	update DetallePedido  
	set DetallePrecio=@productoventaB,DetalleImporte=DetalleCantidad*@productoventaB  
	where DetalleId=@IdTabla  
end
end  
else  
begin
if(@productoventaA>0)
begin   
	update DetallePedido  
	set DetallePrecio=@productoventaA,DetalleImporte=DetalleCantidad*@productoventaA  
	where DetalleId=@IdTabla  
end
end  
end 
Fetch Next From Tabla INTO @Columna    
end   
 Close Tabla;    
 Deallocate Tabla;    
 Commit Transaction;    
 Select 'true'  
end
GO
/****** Object:  StoredProcedure [dbo].[editaprueba]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[editaprueba]  
@ListaOrden varchar(Max)  
as  
begin  
Declare @detalle varchar(max)  
Set @detalle =@ListaOrden  
Begin Transaction  
Declare Tabla Cursor For Select * From fnSplitString(@detalle,';')   
Open Tabla  
        Declare @Columna varchar(max)  
  declare @Id numeric(38)  
     declare @Descripcion varchar(max)  
  Declare @p1 int  
  declare @p2 int  
Fetch Next From Tabla INTO @Columna  
While @@FETCH_STATUS = 0  
Begin  
     Set @p1 = CharIndex('|',@Columna,0)  
  Set @p2 =Len(@Columna)+1  
        Set @Id=Convert(numeric(38),SUBSTRING(@Columna,1,@p1-1))  
        Set @Descripcion=SUBSTRING(@Columna,@p1+1,@p2-(@p1+1))  
  update detalledocumento
  set DetalleDescripcion=@Descripcion
  where DetalleId=@Id
Fetch Next From Tabla INTO @Columna  
End  
 Close Tabla;  
 Deallocate Tabla;  
 Commit Transaction;  
 Select 'true';  
End
GO
/****** Object:  StoredProcedure [dbo].[editapruebaB]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[editapruebaB]
@ListaOrden varchar(Max)
as
begin
Declare @detalle varchar(max)
Set @detalle =@ListaOrden
Begin Transaction
Declare Tabla Cursor For Select * From fnSplitString(@detalle,';')	
Open Tabla
        Declare @Columna varchar(max)
		declare @DetalleId numeric(38)
	    declare @UM varchar(40)
		Declare @p1 int
		declare @p2 int
Fetch Next From Tabla INTO @Columna
While @@FETCH_STATUS = 0
Begin
	    Set @p1 = CharIndex('|',@Columna,0)
		Set @p2 =Len(@Columna)+1
        Set @DetalleId=Convert(numeric(38),SUBSTRING(@Columna,1,@p1-1))
		Set @UM=SUBSTRING(@Columna,@p1+1,@p2-(@p1+1))
		update DetalleGuia
		set UniMedida=@UM
		where DetalleId=@DetalleId
Fetch Next From Tabla INTO @Columna
End
	Close Tabla;
	Deallocate Tabla;
	Commit Transaction;
	Select 'true';
End
GO
/****** Object:  StoredProcedure [dbo].[editarCajaPri]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[editarCajaPri]
@Data varchar(max)
as
begin
Declare @p1 int,@p2 int,
		@p3 int,@p4 int,
		@p5 int,@p6 int
declare @CajaConcepto varchar(80),
		@CajaDescripcion varchar(250),
		@CajaMonto decimal(18,2),
		@CajaUsuario varchar(20),
		@RutaImagen varchar(max),
		@IdCaja numeric(38)
Set @Data = LTRIM(RTrim(@Data))
		Set @p1 = CharIndex('|',@Data,0)
		Set @p2 = CharIndex('|',@Data,@p1+1)
		Set @p3 = CharIndex('|',@Data,@p2+1)
		Set @p4 = CharIndex('|',@Data,@p3+1)
		Set @p5= CharIndex('|',@Data,@p4+1)
		Set @p6= Len(@Data)+1
		Set @CajaConcepto=SUBSTRING(@Data,1,@p1-1)
		Set @CajaDescripcion=SUBSTRING(@Data,@p1+1,@p2-@p1-1)
		Set @CajaMonto=convert(decimal(18,2),SUBSTRING(@Data,@p2+1,@p3-@p2-1))
		Set @CajaUsuario=SUBSTRING(@Data,@p3+1,@p4-@p3-1)
		Set @RutaImagen=SUBSTRING(@Data,@p4+1,@p5-@p4-1)
		Set @IdCaja=convert(numeric(38),SUBSTRING(@Data,@p5+1,@p6-@p5-1))		
update CajaPincipal
set CajaConcepto=@CajaConcepto,CajaFecha=GETDATE(),
CajaDescripcion=@CajaDescripcion,CajaMonto=@CajaMonto,
RutaImagen=@RutaImagen,CajaUsuario=@CajaUsuario
where IdCaja=@IdCaja
select isnull((select STUFF((select '¬'+ convert(varchar,c.IdCaja)+'|'+c.CajaConcepto+'|'+convert(varchar,c.CajaId)+'|'+
Convert(char(10),c.CajaFecha,103)+' '+Convert(char(8),c.CajaFecha,114) 
+'|'+c.CajaDescripcion+'|'+CONVERT(VarChar(50),cast(c.CajaMonto as money), 1)+'|'+
c.CajaUsuario+'|'+c.RutaImagen  
from CajaPincipal c 
where c.CajaConcepto='INGRESO' and c.IdGeneral=0
order by c.IdCaja desc
for xml path('')),1,1,'')),'~')+'['+
isnull((select STUFF((select '¬'+ convert(varchar,c.IdCaja)+'|'+c.CajaConcepto+'|'+convert(varchar,c.CajaId)+'|'+
Convert(char(10),c.CajaFecha,103)+' '+Convert(char(8),c.CajaFecha,114) 
+'|'+c.CajaDescripcion+'|'+CONVERT(VarChar(50),cast(c.CajaMonto as money), 1)+'|'+
c.CajaUsuario+'|'+c.RutaImagen  
from CajaPincipal c 
where c.CajaConcepto='SALIDA' and c.IdGeneral=0
order by c.IdCaja desc
for xml path('')),1,1,'')),'~')
end
GO
/****** Object:  StoredProcedure [dbo].[editarCanje]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[editarCanje]
@temporalId numeric(38),
@temporalCanje varchar(80),
@temporalDias int,
@temporalVencimiento varchar(20),
@temporalMonto decimal(18,2)
as
begin
update TemporalCanje
set temporalCanje=@temporalCanje,temporalDias=@temporalDias,
temporalVencimiento=@temporalVencimiento,temporalMonto=@temporalMonto
where temporalId=@temporalId
end
GO
/****** Object:  StoredProcedure [dbo].[editarCompania]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[editarCompania]
@CompaniaId int,
@CompaniaRazonSocial varchar(140),
@CompaniaRUC varchar(20),
@CompaniaDireccion varchar(max),
@CompaniaTelefono varchar(80),
@CompaniaEmail varchar(100),
@CompaniaIniFecha varchar(100)
as
begin
update Compania
set CompaniaRazonSocial=@CompaniaRazonSocial,
CompaniaRUC=@CompaniaRUC,CompaniaDireccion=@CompaniaDireccion,
CompaniaTelefono=@CompaniaTelefono,CompaniaEmail=@CompaniaEmail,
CompaniaIniFecha=@CompaniaIniFecha
where CompaniaId=@CompaniaId
end
GO
/****** Object:  StoredProcedure [dbo].[editarCompra]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[editarCompra]      
@ListaOrden varchar(Max)        
as        
begin        
Declare @pos int        
Declare @orden varchar(max)        
Declare @detalle varchar(max)        
Set @pos = CharIndex('[',@ListaOrden,0)        
Set @orden = SUBSTRING(@ListaOrden,1,@pos-1)        
Set @detalle = SUBSTRING(@ListaOrden,@pos+1,len(@ListaOrden)-@pos)       
Declare @c1 int,@c2 int,@c3 int,@c4 int,        
        @c5 int,@c6 int,@c7 int,@c8 int,        
        @c9 int,@c10 int,@c11 int,@c12 int,        
        @c13 int,@c14 int,@c15 int,@c16 int,        
        @c17 int,@c18 int,@c19 int,@c20 int,        
        @c21 int,@c22 int,@c23 int,@c24 int,        
        @c25 int,@c26 int,@c27 int,@c28 int        
Declare @CompraId numeric(38),@CompaniaId int,        
  @CompraCorrelativo varchar(80),@ProveedorId numeric(38),        
  @CompraEmision date,@CompraComputo date,        
  @TipoCodigo char(20),@CompraSerie varchar(60),        
  @CompraNumero varchar(80),@CompraCondicion varchar(60),        
  @CompraMoneda varchar(60),@CompraTipoCambio decimal(18,3),        
  @CompraDias int,@CompraFechaPago date,        
  @CompraUsuario varchar(80),@CompraTipoIgv varchar(60),        
  @CompraValorVenta decimal(18,2),@CompraDescuento decimal(18,2),        
  @CompraSubtotal decimal(18,2),@CompraIgv decimal(18,2),        
  @CompraTotal decimal(18,2),@CompraEstado varchar(60),        
  @CompraAsociado varchar(60),@compraSaldo decimal(18,2),        
  @CompraOBS varchar(max),@CompraTipoSunat decimal(18,3),        
  @CompraPercepcion decimal(18,2),@CompraConcepto varchar(40)      
Set @c1 = CharIndex('|',@orden,0)        
Set @c2 = CharIndex('|',@orden,@c1+1)        
Set @c3 = CharIndex('|',@orden,@c2+1)        
Set @c4 = CharIndex('|',@orden,@c3+1)        
Set @c5 = CharIndex('|',@orden,@c4+1)        
Set @c6= CharIndex('|',@orden,@c5+1)        
Set @c7 = CharIndex('|',@orden,@c6+1)        
Set @c8 = CharIndex('|',@orden,@c7+1)        
Set @c9 = CharIndex('|',@orden,@c8+1)        
Set @c10= CharIndex('|',@orden,@c9+1)        
Set @c11= CharIndex('|',@orden,@c10+1)        
Set @c12= CharIndex('|',@orden,@c11+1)        
Set @c13= CharIndex('|',@orden,@c12+1)        
Set @c14= CharIndex('|',@orden,@c13+1)        
Set @c15= CharIndex('|',@orden,@c14+1)        
Set @c16= CharIndex('|',@orden,@c15+1)        
Set @c17= CharIndex('|',@orden,@c16+1)        
Set @c18 = CharIndex('|',@orden,@c17+1)        
Set @c19 = CharIndex('|',@orden,@c18+1)        
Set @c20= CharIndex('|',@orden,@c19+1)        
Set @c21= CharIndex('|',@orden,@c20+1)        
Set @c22= CharIndex('|',@orden,@c21+1)        
Set @c23= CharIndex('|',@orden,@c22+1)        
Set @c24= CharIndex('|',@orden,@c23+1)        
Set @c25= CharIndex('|',@orden,@c24+1)        
Set @c26= CharIndex('|',@orden,@c25+1)      
Set @c27= CharIndex('|',@orden,@c26+1)          
Set @c28= Len(@orden)+1           
set @CompaniaId=convert(int,SUBSTRING(@orden,1,@c1-1))        
set @CompraCorrelativo=SUBSTRING(@orden,@c1+1,@c2-@c1-1)        
set @ProveedorId=convert(numeric(38),SUBSTRING(@orden,@c2+1,@c3-@c2-1))        
set @CompraEmision=convert(date,SUBSTRING(@orden,@c3+1,@c4-@c3-1))        
set @CompraComputo=convert(date,SUBSTRING(@orden,@c4+1,@c5-@c4-1))        
set @TipoCodigo=SUBSTRING(@orden,@c5+1,@c6-@c5-1)        
set @CompraSerie=SUBSTRING(@orden,@c6+1,@c7-@c6-1)        
set @CompraNumero=SUBSTRING(@orden,@c7+1,@c8-@c7-1)        
set @CompraCondicion=SUBSTRING(@orden,@c8+1,@c9-@c8-1)        
set @CompraMoneda=SUBSTRING(@orden,@c9+1,@c10-@c9-1)        
set @CompraTipoCambio=convert(decimal(18,3),SUBSTRING(@orden,@c10+1,@c11-@c10-1))        
set @CompraDias=convert(int,SUBSTRING(@orden,@c11+1,@c12-@c11-1))        
set @CompraFechaPago=convert(date,SUBSTRING(@orden,@c12+1,@c13-@c12-1))        
set @CompraUsuario=SUBSTRING(@orden,@c13+1,@c14-@c13-1)        
set @CompraTipoIgv=SUBSTRING(@orden,@c14+1,@c15-@c14-1)        
set @CompraValorVenta=convert(decimal(18,2),SUBSTRING(@orden,@c15+1,@c16-@c15-1))         
set @CompraDescuento=convert(decimal(18,2),SUBSTRING(@orden,@c16+1,@c17-@c16-1))        
set @CompraSubtotal=convert(decimal(18,2),SUBSTRING(@orden,@c17+1,@c18-@c17-1))        
set @CompraIgv=convert(decimal(18,2),SUBSTRING(@orden,@c18+1,@c19-@c18-1))        
set @CompraTotal=convert(decimal(18,2),SUBSTRING(@orden,@c19+1,@c20-@c19-1))        
set @CompraEstado=SUBSTRING(@orden,@c20+1,@c21-@c20-1)        
set @CompraAsociado=SUBSTRING(@orden,@c21+1,@c22-@c21-1)        
set @compraSaldo=convert(decimal(18,2),SUBSTRING(@orden,@c22+1,@c23-@c22-1))        
set @CompraOBS=SUBSTRING(@orden,@c23+1,@c24-@c23-1)        
set @CompraTipoSunat =convert(decimal(18,3),SUBSTRING(@orden,@c24+1,@c25-@c24-1))         
set @CompraPercepcion=convert(decimal(18,2),SUBSTRING(@orden,@c25+1,@c26-@c25-1))        
set @CompraId=convert(numeric(38),SUBSTRING(@orden,@c26+1,@c27-@c26-1))       
set @CompraConcepto=SUBSTRING(@orden,@c27+1,@c28-@c27-1)      
      
Begin Transaction      
        
update Compras        
set CompaniaId=@CompaniaId,CompraCorrelativo=@CompraCorrelativo,ProveedorId=@ProveedorId,      
CompraEmision=@CompraEmision,CompraComputo=@CompraComputo,        
TipoCodigo=@TipoCodigo,CompraSerie=@CompraSerie,CompraNumero=@CompraNumero,CompraCondicion=@CompraCondicion,        
CompraMoneda=@CompraMoneda,CompraTipoCambio=@CompraTipoCambio,CompraDias=@CompraDias,CompraFechaPago=@CompraFechaPago,        
CompraUsuario=@CompraUsuario,CompraTipoIgv=@CompraTipoIgv,CompraValorVenta=@CompraValorVenta,        
CompraDescuento=@CompraDescuento,CompraSubtotal=@CompraSubtotal,CompraIgv=@CompraIgv,CompraTotal=@CompraTotal,        
CompraEstado=@CompraEstado,CompraAsociado=@CompraAsociado,CompraSaldo=@compraSaldo,CompraOBS=@CompraOBS,        
CompraTipoSunat=@CompraTipoSunat,CompraPercepcion=@CompraPercepcion        
where CompraId=@CompraId       
  
if(@CompraConcepto='MERCADERIA')        
BEGIN               
Declare Tabla Cursor For Select * From fnSplitString(@detalle,';')         
Open Tabla        
Declare @Columna varchar(max),      
        @DetalleId numeric(38),        
        @IdProducto numeric(20),        
        @DetalleCodigo varchar(80),        
        @Descripcion varchar(255),        
        @DetalleUM   varchar(60),        
        @DetalleCantidad decimal(18,2),        
        @PrecioCosto  decimal(18,4),      
        @DetalleDescuento decimal(18,4),          
        @DetalleImprte decimal(18,4),        
        @DetalleEstado varchar(60),        
        @CostoReal decimal(18,4),
        @AplicaINV nvarchar(1)       
Declare @p1 int,@p2 int,@p3 int,@p4 int,        
        @p5 int,@p6 int,@p7 int,@p8 int,
        @p9 int,@p10 int,@p11 int,@p12 int        

Fetch Next From Tabla INTO @Columna        
While @@FETCH_STATUS = 0        
Begin        
Set @p1=CharIndex('|',@Columna,0)        
Set @p2=CharIndex('|',@Columna,@p1+1)        
Set @p3=CharIndex('|',@Columna,@p2+1)        
Set @p4=CharIndex('|',@Columna,@p3+1)        
Set @p5=CharIndex('|',@Columna,@p4+1)        
Set @p6=CharIndex('|',@Columna,@p5+1)        
Set @p7=CharIndex('|',@Columna,@p6+1)        
Set @p8=CharIndex('|',@Columna,@p7+1)        
Set @p9=CharIndex('|',@Columna,@p8+1)      
Set @p10=CharIndex('|',@Columna,@p9+1)
Set @p11=CharIndex('|',@Columna,@p10+1)         
Set @p12=Len(@Columna)+1      
      
set @DetalleId=Convert(numeric(38),SUBSTRING(@Columna,1,@p1-1))          
set @IdProducto=Convert(numeric(20),SUBSTRING(@Columna,@p1+1,@p2-(@p1+1)))        
Set @DetalleCodigo=SUBSTRING(@Columna,@p2+1,@p3-(@p2+1))        
Set @Descripcion=SUBSTRING(@Columna,@p3+1,@p4-(@p3+1))        
Set @DetalleUM=SUBSTRING(@Columna,@p4+1,@p5-(@p4+1))        
Set @DetalleCantidad=convert(decimal(18,2),SUBSTRING(@Columna,@p5+1,@p6-(@p5+1)))        
Set @PrecioCosto=convert(decimal(18,4),SUBSTRING(@Columna,@p6+1,@p7-(@p6+1)))        
Set @DetalleDescuento=convert(decimal(18,4),SUBSTRING(@Columna,@p7+1,@p8-(@p7+1)))        
Set @DetalleImprte=convert(decimal(18,4),SUBSTRING(@Columna,@p8+1,@p9-(@p8+1)))        
Set @DetalleEstado=SUBSTRING(@Columna,@p9+1,@p10-(@p9+1))        
Set @CostoReal=convert(decimal(18,4),SUBSTRING(@Columna,@p10+1,@p11-(@p10+1)))
Set @AplicaINV=SUBSTRING(@Columna,@p11+1,@p12-(@p11+1))         
          
update DetalleCompra       
set PrecioCosto=@PrecioCosto,DetalleImporte=@DetalleImprte,
DetalleEstado='EMITIDO'      
where DetalleId=@DetalleId      

if(@DetalleEstado='PENDIENTE')       
begin      

if(@AplicaINV='S')
Begin

declare @IniciaStock decimal(18,2),@stockFinal decimal(18,2)        

set @IniciaStock=(select top 1 p.ProductoCantidad   
from Producto p 
where p.IdProducto=@IdProducto)        
set @stockFinal=@IniciaStock+@DetalleCantidad 
    
update Producto         
set ProductoCantidad=ProductoCantidad+@DetalleCantidad,ProductoCosto=@CostoReal        
where IdProducto=@IdProducto       
        
insert into Kardex values(@IdProducto,GETDATE(),'Ingreso por Compra',@CompraSerie+'-'+@CompraNumero,@IniciaStock,        
@DetalleCantidad,0,@CostoReal,@StockFinal,'INGRESO',@CompraUsuario)      

END
ELSE
BEGIN
update Producto         
set    ProductoCosto=@CostoReal        
where  IdProducto=@IdProducto 
END
END
      
ELSE      
BEGIN      
      
if(@DetalleEstado='EMITIDO')       
begin      
      
update Producto         
set ProductoCosto=@CostoReal        
where IdProducto=@IdProducto       

end     

END  
Fetch Next From Tabla INTO @Columna        
end       
Close Tabla;        
Deallocate Tabla;  
end       
Commit Transaction;       
select 'true'        
end
GO
/****** Object:  StoredProcedure [dbo].[editarDetaLiquida]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[editarDetaLiquida]
@DetalleId numeric(38),
@EntidadBanco varchar(80),
@NroOperacion varchar(80),
@FechaPago varchar(60)
as
begin
update DetalleLiquida
set EntidadBanco=@EntidadBanco,NroOperacion=@NroOperacion,FechaPago=@FechaPago
where DetalleId=@DetalleId
end
GO
/****** Object:  StoredProcedure [dbo].[editarGuia]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[editarGuia]
@GuiaId numeric(38),
@GuiaNumero varchar(60),
@GuiaMotivo varchar(80),
@GuiaFechaTraslado datetime,
@GuiaDestinatario varchar(250),
@GuiaRucDes varchar(60),
@GuiaAlmacen varchar(80),
@GuiaPartida varchar(max),
@GuiaLLegada varchar(max),
@GuiaTramsporte varchar(80),
@GuiaTransporteRuc varchar(20),
@GuiaChofer varchar(80),
@GuiaPlaca varchar(80),
@GuiaConstancia varchar(80),
@GuiaLicencia varchar(80),
@GuiaUsuario varchar(80),
@GuiaTotal decimal(18,2),
@ClienteId numeric(20),
@GuiaTelefono varchar(80)
as
begin
update GuiaRemision
set GuiaNumero=@GuiaNumero,GuiaMotivo=@GuiaMotivo,GuiaFechaTraslado=@GuiaFechaTraslado,
GuiaDestinatario=@GuiaDestinatario,GuiaRucDes=@GuiaRucDes,GuiaAlmacen=@GuiaAlmacen,
GuiaPartida=@GuiaPartida,GuiaLLegada=@GuiaLLegada,GuiaTramsporte=@GuiaTramsporte,
GuiaTransporteRuc=@GuiaTransporteRuc,GuiaChofer=@GuiaChofer,GuiaPlaca=@GuiaPlaca,
GuiaConstancia=@GuiaConstancia,GuiaLicencia=@GuiaLicencia,GuiaUsuario=@GuiaUsuario,GuiaTotal=@GuiaTotal,
ClienteId=@ClienteId,GuiaTelefono=@GuiaTelefono
where GuiaId=@GuiaId
end
GO
/****** Object:  StoredProcedure [dbo].[editarLiquida]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[editarLiquida]  
@ListaOrden varchar(Max)  
as  
begin  
Declare @pos1 int,@pos2 int          
Declare @orden varchar(max),          
        @detalle varchar(max)          
Set @pos1 = CharIndex('[',@ListaOrden,0)          
Set @pos2 =Len(@ListaOrden)+1          
Set @orden = SUBSTRING(@ListaOrden,1,@pos1-1)          
Set @detalle = SUBSTRING(@ListaOrden,@pos1+1,@pos2-@pos1-1)
Declare @c1 int,@c2 int,@c3 int,@c4 int,          
        @c5 int,@c6 int,@c7 int,@c8 int,          
        @c9 int,@c10 int,@c11 int
Declare @Id numeric(38),  
		@Fecha date,  
		@Descripcion varchar(250),  
		@Cambio decimal(18,3),  
		@EfectivoSol decimal(18,2),  
		@DepositoSol decimal(18,2),  
		@TotalSol decimal(18,2),  
		@EfectivoDol decimal(18,2),  
		@DepositoDol decimal(18,2),  
		@TotalDol decimal(18,2),  
		@Usuario varchar(60)  
Set @c1 = CharIndex('|',@orden,0)          
Set @c2 = CharIndex('|',@orden,@c1+1)          
Set @c3 = CharIndex('|',@orden,@c2+1)          
Set @c4 = CharIndex('|',@orden,@c3+1)          
Set @c5 = CharIndex('|',@orden,@c4+1)          
Set @c6= CharIndex('|',@orden,@c5+1)          
Set @c7 = CharIndex('|',@orden,@c6+1)          
Set @c8 = CharIndex('|',@orden,@c7+1)          
Set @c9 = CharIndex('|',@orden,@c8+1)          
Set @c10= CharIndex('|',@orden,@c9+1)          
Set @c11=Len(@orden)+1
set @Id=convert(numeric(38),SUBSTRING(@orden,1,@c1-1))          
set @Fecha=SUBSTRING(@orden,@c1+1,@c2-@c1-1)          
set @Descripcion=SUBSTRING(@orden,@c2+1,@c3-@c2-1)          
set @Cambio=convert(decimal(18,3),SUBSTRING(@orden,@c3+1,@c4-@c3-1))          
set @EfectivoSol=convert(decimal(18,2),SUBSTRING(@orden,@c4+1,@c5-@c4-1))          
set @DepositoSol=convert(decimal(18,2),SUBSTRING(@orden,@c5+1,@c6-@c5-1))          
set @TotalSol=convert(decimal(18,2),SUBSTRING(@orden,@c6+1,@c7-@c6-1))          
set @EfectivoDol=convert(decimal(18,2),SUBSTRING(@orden,@c7+1,@c8-@c7-1))          
set @DepositoDol=convert(decimal(18,2),SUBSTRING(@orden,@c8+1,@c9-@c8-1))          
set @TotalDol=convert(decimal(18,2),SUBSTRING(@orden,@c9+1,@c10-@c9-1))          
set @Usuario=SUBSTRING(@orden,@c10+1,@c11-@c10-1)   
Begin Transaction    

update Liquidacion  
set LiquidacionRegistro=GETDATE(),LiquidacionFecha=@Fecha,  
LiquidacionDescripcion=@Descripcion,LiquidacionCambio=@Cambio,  
LiquidaEfectivoSol=@EfectivoSol,LiquidaDepositoSol=@DepositoSol,  
LiquidaTotalSol=@TotalSol,LiquidaEfectivoDol=@EfectivoDol,  
LiquidaDepositoDol=@DepositoDol,LiquidaTotalDol=@TotalDol,  
LiquidaUsuario=@Usuario  
where LiquidacionId=@Id

Declare Tabla Cursor For Select * From fnSplitString(@detalle,';')           
Open Tabla          
Declare @Columna varchar(max),  
  @DetalleId numeric(38),    
  @EntidadBanco varchar(80),    
  @NroOperacion varchar(80),    
  @FechaPago varchar(60)  
Declare @p1 int,@p2 int,@p3 int,@p4 int  
Fetch Next From Tabla INTO @Columna          
 While @@FETCH_STATUS = 0          
 Begin          
Set @p1 = CharIndex('|',@Columna,0)          
Set @p2 = CharIndex('|',@Columna,@p1+1)          
Set @p3 = CharIndex('|',@Columna,@p2+1)          
Set @p4 =Len(@Columna)+1  
  
set @DetalleId =Convert(numeric(38),SUBSTRING(@Columna,1,@p1-1))      
Set @EntidadBanco=SUBSTRING(@Columna,@p1+1,@p2-(@p1+1))              
Set @NroOperacion=SUBSTRING(@Columna,@p2+1,@p3-(@p2+1))          
Set @FechaPago=SUBSTRING(@Columna,@p3+1,@p4-(@p3+1)) 

update DetalleLiquida  
set EntidadBanco=@EntidadBanco,NroOperacion=@NroOperacion,FechaPago=@FechaPago  
where DetalleId=@DetalleId

Fetch Next From Tabla INTO @Columna          
end          
 Close Tabla;          
 Deallocate Tabla;          
 Commit Transaction;  
 select 'true'   
end
GO
/****** Object:  StoredProcedure [dbo].[editarLiquidaVenta]    Script Date: 20/04/2026 16:11:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[editarLiquidaVenta]
@ListaOrden varchar(Max)
as
begin
Declare @pos1 int,@pos2 int        
Declare @orden varchar(max),        
        @detalle varchar(max)        
Set @pos1 = CharIndex('[',@ListaOrden,0)        
Set @pos2 =Len(@ListaOrden)+1        
Set @orden = SUBSTRING(@ListaOrden,1,@pos1-1)        
Set @detalle = SUBSTRING(@ListaOrden,@pos1+1,@pos2-@pos1-1)
Declare @Id numeric(38),@Fecha date,  
		@Descripcion varchar(250),@Cambio decimal(18,3),  
		@EfectivoSol decimal(18,2),@DepositoSol decimal(18,2),  
		@TotalSol decimal(18,2),@EfectivoDol decimal(18,2),  
		@DepositoDol decimal(18,2),@TotalDol decimal(18,2),  
		@Usuario varchar(60)
Declare @c1 int,@c2 int,@c3 int,@c4 int,        
        @c5 int,@c6 int,@c7 int,@c8 int,        
        @c9 int,@c10 int,@c11 int
Set @c1 = CharIndex('|',@orden,0)        
Set @c2 = CharIndex('|',@orden,@c1+1)        
Set @c3 = CharIndex('|',@orden,@c2+1)        
Set @c4 = CharIndex('|',@orden,@c3+1)        
Set @c5 = CharIndex('|',@orden,@c4+1)        
Set @c6= CharIndex('|',@orden,@c5+1)        
Set @c7 = CharIndex('|',@orden,@c6+1)        
Set @c8 = CharIndex('|',@orden,@c7+1)        
Set @c9 = CharIndex('|',@orden,@c8+1)        
Set @c10= CharIndex('|',@orden,@c9+1)        
Set @c11=Len(@orden)+1		 
set @Id=convert(numeric(38),SUBSTRING(@orden,1,@c1-1))        
set @Fecha=SUBSTRING(@orden,@c1+1,@c2-@c1-1)        
set @Descripcion=SUBSTRING(@orden,@c2+1,@c3-@c2-1)        
set @Cambio=convert(decimal(18,3),SUBSTRING(@orden,@c3+1,@c4-@c3-1))        
set @EfectivoSol=convert(decimal(18,2),SUBSTRING(@orden,@c4+1,@c5-@c4-1))        
set @DepositoSol=convert(decimal(18,2),SUBSTRING(@orden,@c5+1,@c6-@c5-1))        
set @TotalSol=convert(decimal(18,2),SUBSTRING(@orden,@c6+1,@c7-@c6-1))        
set @EfectivoDol=convert(decimal(18,2),SUBSTRING(@orden,@c7+1,@c8-@c7-1))        
set @DepositoDol=convert(decimal(18,2),SUBSTRING(@orden,@c8+1,@c9-@c8-1))        
set @TotalDol=convert(decimal(18,2),SUBSTRING(@orden,@c9+1,@c10-@c9-1))        
set @Usuario=SUBSTRING(@orden,@c10+1,@c11-@c10-1)
Begin Transaction  
update LiquidacionVenta  
set LiquidacionRegistro=GETDATE(),LiquidacionFecha=@Fecha,  
LiquidacionDescripcion=@Descripcion,LiquidacionCambio=@Cambio,  
LiquidaEfectivoSol=@EfectivoSol,LiquidaDepositoSol=@DepositoSol,  
LiquidaTotalSol=@TotalSol,LiquidaEfectivoDol=@EfectivoDol,  
LiquidaDepositoDol=@DepositoDol,LiquidaTotalDol=@TotalDol,  
LiquidaUsuario=@Usuario  
where LiquidacionId=@Id 
Declare Tabla Cursor For Select * From fnSplitString(@detalle,';')         
Open Tabla        
Declare @Columna varchar(max),
		@DetalleId numeric(38),  
		@EntidadBanco varchar(80),  
		@NroOperacion varchar(80),  
		@FechaPago varchar(60)
Declare @p1 int,@p2 int,@p3 int,@p4 int
Fetch Next From Tabla INTO @Columna        
 While @@FETCH_STATUS = 0        
 Begin        
Set @p1 = CharIndex('|',@Columna,0)        
Set @p2 = CharIndex('|',@Columna,@p1+1)        
Set @p3 = CharIndex('|',@Columna,@p2+1)        
Set @p4 =Len(@Columna)+1

set @DetalleId =Convert(numeric(38),SUBSTRING(@Columna,1,@p1-1))    
Set @EntidadBanco=SUBSTRING(@Columna,@p1+1,@p2-(@p1+1))            
Set @NroOperacion=SUBSTRING(@Columna,@p2+1,@p3-(@p2+1))        
Set @FechaPago=SUBSTRING(@Columna,@p3+1,@p4-(@p3+1))   

update DetaLiquidaVenta  
set EntidadBanco=@EntidadBanco,NroOperacion=@NroOperacion,FechaPago=@FechaPago  
where DetalleId=@DetalleId 
 
Fetch Next From Tabla INTO @Columna        
end        
 Close Tabla;        
 Deallocate Tabla;        
 Commit Transaction;
 select 'true' 
end
GO
/****** Object:  StoredProcedure [dbo].[editarNOta]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[editarNOta]
@NotaId numeric(38),
@NotaDocu varchar(60),
@ClienteId numeric(20),
@NotaFecha datetime,
@NotaUsuario varchar(60),
@NotaSubtotal decimal(8,2),
@NotaDescuento decimal(18,2),
@NotaTotal decimal(18,2),
@NotaEstado varchar(60)
as
begin
update NotaPedido
set NotaDocu=@NotaDocu,ClienteId=@ClienteId,NotaFecha=@NotaFecha,NotaUsuario=@NotaUsuario,NotaSubtotal=@NotaSubtotal,NotaDescuento=@NotaDescuento,NotaTotal=@NotaTotal,NotaEstado=@NotaEstado
where NotaId=@NotaId
end
GO
/****** Object:  StoredProcedure [dbo].[editarPersonal]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[editarPersonal]
@Id numeric(20),
@PersonalNombres varchar(140),
@PersonalApellidos varchar(140),
@AreaId numeric(20),
@PersonalCodigo varchar (80),
@PersonalNacimiento date,
@PersonalIngreso varchar(20),
@PersonalDNI varchar(20),
@PersonalDireccion varchar(140),
@PersonalTelefono varchar(40),
@PersonalEmail varchar(100),
@PersonalEstado varchar(60),
@PersonalImagen varchar(max),
@CompaniaId int
as
begin
update Personal
set PersonalNombres=@PersonalNombres,PersonalApellidos=@PersonalApellidos,AreaId=@AreaId,PersonalCodigo=@PersonalCodigo,PersonalNacimiento=@PersonalNacimiento,
PersonalIngreso=@PersonalIngreso,PersonalDNI=@PersonalDNI,PersonalDireccion=@PersonalDireccion,PersonalTelefono=@PersonalTelefono,
PersonalEmail=@PersonalEmail,PersonalEstado=@PersonalEstado,PersonalImagen=@PersonalImagen,CompaniaId=@CompaniaId
where PersonalId=@Id
end
GO
/****** Object:  StoredProcedure [dbo].[editarTemLiquida]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[editarTemLiquida]
@orden varchar(max) 
as
begin
Declare @c1 int,@c2 int,@c3 int,@c4 int,    
        @c5 int,@c6 int,@c7 int,@c8 int,
        @c9 int,@c10 int   
Declare @TemporalId numeric(38),  
        @EfectivoSoles decimal(18,2),  
		@EfectivoDolar decimal(18,2),  
		@DepositoSoles decimal(18,2),  
		@DepositoDolar decimal(18,2),  
		@TipoCambio decimal(18,3),  
		@EntidadBanco varchar(80),  
		@NroOperacion varchar(80),  
		@AcuentaGeneral decimal(18,2),  
		@TemporalFecha varchar(60)  

Set @c1 = CharIndex('|',@orden,0)    
Set @c2 = CharIndex('|',@orden,@c1+1)    
Set @c3 = CharIndex('|',@orden,@c2+1)    
Set @c4 = CharIndex('|',@orden,@c3+1)    
Set @c5 = CharIndex('|',@orden,@c4+1)
Set @c6 = CharIndex('|',@orden,@c5+1)    
Set @c7 = CharIndex('|',@orden,@c6+1)    
Set @c8 = CharIndex('|',@orden,@c7+1)    
Set @c9 = CharIndex('|',@orden,@c8+1) 
Set @c10 =Len(@orden)+1

set @TemporalId=Convert(numeric(38),SUBSTRING(@orden,1,@c1-1))    
set @EfectivoSoles=Convert(decimal(18,2),SUBSTRING(@orden,@c1+1,@c2-@c1-1))    
set @EfectivoDolar=Convert(decimal(18,2),SUBSTRING(@orden,@c2+1,@c3-@c2-1))    
set @DepositoSoles=Convert(decimal(18,2),SUBSTRING(@orden,@c3+1,@c4-@c3-1))    
set @DepositoDolar=Convert(decimal(18,2),SUBSTRING(@orden,@c4+1,@c5-@c4-1))  
set @TipoCambio=Convert(decimal(18,3),SUBSTRING(@orden,@c5+1,@c6-@c5-1))
set @EntidadBanco=SUBSTRING(@orden,@c6+1,@c7-@c6-1)  
set @NroOperacion=SUBSTRING(@orden,@c7+1,@c8-@c7-1)    
set @AcuentaGeneral=Convert(decimal(18,2),SUBSTRING(@orden,@c8+1,@c9-@c8-1))    
set @TemporalFecha=SUBSTRING(@orden,@c9+1,@c10-@c9-1)

update TemporalLiquida  
set EfectivoSoles=@EfectivoSoles,EfectivoDolar=@EfectivoDolar,  
DepositoSoles=@DepositoSoles,DepositoDolar=@DepositoDolar,  
TipoCambio=@TipoCambio,EntidadBanco=@EntidadBanco,NroOperacion=@NroOperacion,  
AcuentaGeneral=@AcuentaGeneral,TemporalFecha=@TemporalFecha  
where TemporalId=@TemporalId  

select 'true'

end
GO
/****** Object:  StoredProcedure [dbo].[editarTemLiVenta]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[editarTemLiVenta]
@orden varchar(max)
as  
begin
Declare @c1 int,@c2 int,@c3 int,@c4 int,    
        @c5 int,@c6 int,@c7 int,@c8 int,    
        @c9 int,@c10 int 
Declare @TemporalId numeric(38), @EfectivoSoles decimal(18,2),  
		@EfectivoDolar decimal(18,2),@DepositoSoles decimal(18,2),  
		@DepositoDolar decimal(18,2),@TipoCambio decimal(18,3),  
		@EntidadBanco varchar(80),@NroOperacion varchar(80),  
		@AcuentaGeneral decimal(18,2),@TemporalFecha varchar(60)  
Set @c1 = CharIndex('|',@orden,0)    
Set @c2 = CharIndex('|',@orden,@c1+1)    
Set @c3 = CharIndex('|',@orden,@c2+1)    
Set @c4 = CharIndex('|',@orden,@c3+1)    
Set @c5 = CharIndex('|',@orden,@c4+1) 
Set @c6 = CharIndex('|',@orden,@c5+1)    
Set @c7 = CharIndex('|',@orden,@c6+1)    
Set @c8 = CharIndex('|',@orden,@c7+1)    
Set @c9 = CharIndex('|',@orden,@c8+1)    
Set @c10= Len(@orden)+1  

set @TemporalId=Convert(numeric(38),SUBSTRING(@orden,1,@c1-1))    
set @EfectivoSoles=Convert(decimal(18,2),SUBSTRING(@orden,@c1+1,@c2-@c1-1))    
set @EfectivoDolar=Convert(decimal(18,2),SUBSTRING(@orden,@c2+1,@c3-@c2-1))    
set @DepositoSoles=Convert(decimal(18,2),SUBSTRING(@orden,@c3+1,@c4-@c3-1))    
set @DepositoDolar=convert(decimal(18,2),SUBSTRING(@orden,@c4+1,@c5-@c4-1))  
set @TipoCambio=Convert(decimal(18,3),SUBSTRING(@orden,@c5+1,@c6-@c5-1))
set @EntidadBanco=SUBSTRING(@orden,@c6+1,@c7-@c6-1)    
set @NroOperacion=SUBSTRING(@orden,@c7+1,@c8-@c7-1)    
set @AcuentaGeneral=convert(decimal(18,2),SUBSTRING(@orden,@c8+1,@c9-@c8-1))  
set @TemporalFecha=SUBSTRING(@orden,@c9+1,@c10-@c9-1)   
 
update TemporalLiVenta  
set EfectivoSoles=@EfectivoSoles,EfectivoDolar=@EfectivoDolar,  
DepositoSoles=@DepositoSoles,DepositoDolar=@DepositoDolar,  
TipoCambio=@TipoCambio,EntidadBanco=@EntidadBanco,NroOperacion=@NroOperacion,  
AcuentaGeneral=@AcuentaGeneral,TemporalFecha=@TemporalFecha  
where TemporalId=@TemporalId

select 'true'

end
GO
/****** Object:  StoredProcedure [dbo].[editarUsuario]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[editarUsuario] 
@UsuarioId int,
@UsuarioAlias varchar(60),
@UsuarioClave varchar(40),
@UsuarioEstado varchar(40)
as
begin
update Usuarios 
set UsuarioAlias=@UsuarioAlias,
UsuarioClave=dbo.encriptar(@UsuarioClave),UsuarioFechaReg=GETDATE(),
Usuarioestado=@UsuarioEstado
where UsuarioID=@UsuarioId
end
GO
/****** Object:  StoredProcedure [dbo].[eliminaBloqueB]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[eliminaBloqueB]
@ListaOrden varchar(Max),
@PKardex varchar(max)
as
begin
Declare @pos int
	Set @pos = CharIndex('_',@ListaOrden,0)
	Declare @BloqueId varchar(max)
	Declare @detalle varchar(max)
	Set @BloqueId=SUBSTRING(@ListaOrden,1,@pos-1)
	Set @detalle =SUBSTRING(@ListaOrden,@pos+1,len(@ListaOrden)-@pos)
Begin Transaction
Declare Tabla Cursor For Select * From fnSplitString(@detalle,';')	
Open Tabla
        Declare @Columna varchar(max)
		declare @NotaId numeric(38)
		Declare @ps1 int
Fetch Next From Tabla INTO @Columna
While @@FETCH_STATUS = 0
Begin
	    Set @NotaId=@Columna
		update NotaPedido
        set NotaEstado='PENDIENTE',NotaSaldo=NotaPagar,NotaAcuenta=0
        where NotaId=@NotaId
        delete from CajaDetalle
        where NotaId=@NotaId
Fetch Next From Tabla INTO @Columna
End
	Close Tabla;
	Deallocate Tabla;
	begin
	DECLARE @Kardex VARCHAR(MAX)
    Set @Kardex =@PKardex
	Declare TablaB Cursor For Select * From fnSplitString(@Kardex,';')	
Open TablaB
		Declare @ColumnaB varchar(max),
		@IdProducto numeric(20),
		@Documento varchar(150),
		@CantIngreso decimal(18,2),
		@PrecioCosto decimal(18,4),
		@Usuario varchar(80)
		Declare @p1 int
		Declare @p2 int
		Declare @p3 int
		declare @p4 int
		declare @p5 int
		declare @IniciaStock decimal(18,2),@StockFinal decimal(18,2)
Fetch Next From TablaB INTO @ColumnaB
	While @@FETCH_STATUS = 0
	Begin
		Set @p1 = CharIndex('|',@ColumnaB,0)
		Set @p2 = CharIndex('|',@ColumnaB,@p1+1)
		Set @p3 = CharIndex('|',@ColumnaB,@p2+1)
		Set @p4 = CharIndex('|',@ColumnaB,@p3+1)
		Set @p5 =Len(@ColumnaB)+1
        Set @IdProducto=Convert(numeric(20),SUBSTRING(@ColumnaB,1,@p1-1))
		Set @Documento= Convert(varchar(150),SUBSTRING(@ColumnaB,@p1+1,@p2-(@p1+1)))
		Set @CantIngreso= Convert(varchar(80),SUBSTRING(@ColumnaB,@p2+1,@p3-(@p2+1)))
		Set @PrecioCosto= Convert(varchar(80),SUBSTRING(@ColumnaB,@p3+1,@p4-(@p3+1)))
		Set @Usuario= Convert(varchar(80),SUBSTRING(@ColumnaB,@p4+1,@p5-@p4-1))
		set @IniciaStock=(select top 1 ProductoCantidad from Producto (nolock) where IdProducto=@IdProducto)
		set @StockFinal=@IniciaStock+@CantIngreso
		insert into Kardex values(@IdProducto,GETDATE(),'Anulacion por Venta',@Documento,@IniciaStock,
		@CantIngreso,0,@PrecioCosto,@StockFinal,'INGRESO',@Usuario)
		update producto 
	    set  ProductoCantidad =ProductoCantidad+@CantIngreso
	    where IDProducto=@IdProducto
		Fetch Next From TablaB INTO @ColumnaB
	End
	Close TablaB;
	Deallocate TablaB;
	delete from DetalleBloque
	where BloqueId=@BloqueId
	delete from BLOQUE
	where BloqueId=@BloqueId
end
	Commit Transaction;
	Select 'true';
End
GO
/****** Object:  StoredProcedure [dbo].[eliminaCuenta]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[eliminaCuenta]
@Data varchar(max)
as
begin
    Set @Data = LTRIM(RTrim(@Data))
	Declare @pos1 int,@pos2 int
	declare @CuentaId numeric(38),@ProveedorId numeric(38)
	declare @contador int
Set @pos1 = CharIndex('|',@Data,0)
Set @CuentaId=convert(numeric(38),SUBSTRING(@Data,1,@pos1-1))
Set @pos2 =Len(@Data)+1
Set @ProveedorId=convert(numeric(38),SUBSTRING(@Data,@pos1+1,@pos2-@pos1-1))
	delete from CuentaProveedor
	where CuentaId=@CuentaId
set @contador=(select COUNT(*) from CuentaProveedor where ProveedorId=@ProveedorId)	
if 	@contador<=0
begin
	select 'true'
end
else
begin
	select isnull((select STUFF ((select '¬'+ CONVERT(varchar,c.CuentaId)+'|'+c.Entidad+'|'+
	c.TipoCuenta+'|'+c.Moneda+'|'+c.NroCuenta
	from CuentaProveedor c
	where c.ProveedorId=@ProveedorId
	order by c.CuentaId desc
	for xml path('')),1,1,'')),'~')
end
end
GO
/****** Object:  StoredProcedure [dbo].[eliminaDetaCaja]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[eliminaDetaCaja]
@DetalleId numeric(38),
@NotaId numeric(38),
@Monto decimal(18,2)
as
begin
declare @Acuenta decimal(18,2),@Documento varchar(40),@EstadoDocu varchar(80)
declare @Data varchar(60)
declare @p1 int,@p2 int
update NotaPedido
set NotaSaldo=NotaSaldo + @Monto,NotaAcuenta=NotaAcuenta-@Monto
where NotaId=@NotaId
set @Acuenta=(select NotaAcuenta from NotaPedido where NotaId=@NotaId)
set @Data=isnull((select top 1 d.DocuDocumento+'¬'+d.DocuEstado from DocumentoVenta d where d.NotaId=@NotaId order by DocuId desc),'0¬0')
Set @Data = LTRIM(RTrim(@Data))
Set @p1 = CharIndex('¬',@Data,0)
Set @p2 = Len(@Data)+1
Set @Documento=SUBSTRING(@Data,1,@p1-1)
Set @EstadoDocu=SUBSTRING(@Data,@p1+1,@p2-@p1-1)
if @EstadoDocu='ANULADO'
begin
update NotaPedido 
set NotaEstado='ANULADO'
where NotaId=@NotaId
end
else
begin
if(@Documento='FACTURA' or @Documento='BOLETA')
begin
if @Acuenta<=0
begin
update NotaPedido 
set NotaEstado='EMITIDO'
where NotaId=@NotaId
end
else
begin
update NotaPedido 
set NotaEstado='ACUENTA'
where NotaId=@NotaId
end
END
else
begin
if @Acuenta<=0
begin
update NotaPedido 
set NotaEstado='PENDIENTE'
where NotaId=@NotaId
end
else
begin
update NotaPedido 
set NotaEstado='ACUENTA'
where NotaId=@NotaId
end
end
end
delete from CajaDetalle 
where DetalleId=@DetalleId
end
GO
/****** Object:  StoredProcedure [dbo].[eliminaDetaLiquida]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[eliminaDetaLiquida] 
@DetalleId numeric(38),
@CompraId numeric(18,2),
@Acuenta decimal(18,2),
@Concepto varchar(40)
as
begin
if(@Concepto='LETRA')
begin
update DetalleLetra
set DetalleSaldo=DetalleSaldo+@Acuenta,DetalleEstado='PENDIENTE DE PAGO'
where DetalleId=@CompraId
end
else
begin
update Compras
set CompraSaldo=CompraSaldo+@Acuenta,CompraEstado='PENDIENTE DE PAGO'
where CompraId=@CompraId
end
begin
delete from DetalleLiquida
where DetalleId=@DetalleId
end
end
GO
/****** Object:  StoredProcedure [dbo].[eliminaDetaNota]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[eliminaDetaNota]
@Data varchar(max)
as
begin
declare @p0 int, 
        @p1 int
declare @DetalleId numeric(38),
        @Ganancia decimal(18,2),
        @NotaId numeric(38)
Set @Data= LTRIM(RTrim(@Data))
set @p0 = CharIndex('|',@Data,0)
Set @p1 = Len(@Data)+1
Set @DetalleId=Convert(numeric(38),SUBSTRING(@Data,1,@p0-1))
Set @Ganancia= Convert(decimal(18,2),SUBSTRING(@Data,@p0+1,@p1-@p0-1))
set @NotaId=(select NotaId from DetallePedido where DetalleId=@DetalleId)
begin
	delete from DetallePedido 
	where DetalleId=@DetalleId
	update NotaPedido
	set NotaGanancia=NotaGanancia-@Ganancia
	where NotaId=@NotaId
	select 'true'
end
end
GO
/****** Object:  StoredProcedure [dbo].[eliminaGuiaRe]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create proc [dbo].[eliminaGuiaRe]
@GuiaId numeric(38),
@NotaId numeric(38)
as
begin
begin
update GuiaRemision
set GuiaEstado=''
where GuiaId=@GuiaId
end
begin
delete from GuiaRelacion
where NotaId=@NotaId
end
end
GO
/****** Object:  StoredProcedure [dbo].[eliminaliquiVenta]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[eliminaliquiVenta] 
@LiquidacionId numeric(38),
@NotaId numeric(38),
@Acuenta decimal(18,2)
as
update NotaPedido
set NotaSaldo=NotaSaldo + @Acuenta,NotaEstado='EMITIDO'
where NotaId=@NotaId
delete from DetaLiquidaVenta
where LiquidacionId=@LiquidacionId
delete from LiquidacionVenta
where LiquidacionId=@LiquidacionId
GO
/****** Object:  StoredProcedure [dbo].[eliminarCajaPrin]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[eliminarCajaPrin]
@Data varchar(max)
as
begin
Declare  @p1 int,@p2 int
Declare @IdCaja numeric(38),
        @GastoId nvarchar(40)
Set @Data = LTRIM(RTrim(@Data))
Set @p1 = CharIndex('|',@Data,0)
Set @p2 =Len(@Data)+1
Set @IdCaja=convert(numeric(38),SUBSTRING(@Data,1,@p1-1))
Set @GastoId=SUBSTRING(@Data,@p1+1,@p2-@p1-1)
begin
delete from GastosFijos
where GastoId=@GastoId
delete from CajaPincipal 
where IdCaja=@IdCaja
select isnull((select STUFF((select '¬'+ convert(varchar,c.IdCaja)+'|'+c.CajaConcepto+'|'+convert(varchar,c.CajaId)+'|'+
Convert(char(10),c.CajaFecha,103)+' '+Convert(char(8),c.CajaFecha,114) 
+'|'+c.CajaDescripcion+'|'+CONVERT(VarChar(50),cast(c.CajaMonto as money), 1)+'|'+
c.CajaUsuario+'|'+c.Referencia+'|'+c.GastoId 
from CajaPincipal c 
where c.CajaConcepto='INGRESO' and c.IdGeneral=0
order by c.IdCaja desc
for xml path('')),1,1,'')),'~')+'['+
isnull((select STUFF((select '¬'+ convert(varchar,c.IdCaja)+'|'+c.CajaConcepto+'|'+convert(varchar,c.CajaId)+'|'+
Convert(char(10),c.CajaFecha,103)+' '+Convert(char(8),c.CajaFecha,114) 
+'|'+c.CajaDescripcion+'|'+CONVERT(VarChar(50),cast(c.CajaMonto as money), 1)+'|'+
c.CajaUsuario+'|'+c.Referencia+'|'+c.GastoId 
from CajaPincipal c 
where c.CajaConcepto='SALIDA' and c.IdGeneral=0
order by c.IdCaja desc
for xml path('')),1,1,'')),'~')
end
end
GO
/****** Object:  StoredProcedure [dbo].[eliminarCanje]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[eliminarCanje]
@CanjeId numeric(38),@CompraId numeric(38),@GTCSunat decimal(18,3),
@GCompania int,@GSerie varchar(80),
@GNumero varchar(80),@GEmision date,
@GComputo date,@GCorrelativo varchar(80),
@GTipo varchar(80),@GOBS varchar(max),
@Usuario varchar(60),@Monto decimal(18,2)
as
declare @Subtotal decimal(18,2),@Igv decimal(18,2),@Total decimal(18,2)
IF @GTipo ='DISGREGADO'
begin
set @Subtotal=@Monto
set @Igv=@Subtotal * 0.18
set @Total=@Subtotal + @Igv
end   
ELSE If @GTipo='INCLUIDO'
begin
set @Subtotal=@Monto/1.18
set @Igv=@Monto-(@Monto/1.18)
set @Total=@Monto
end
Else
begin
set @Subtotal=@Monto
set @Igv=0
set @Total=@Monto
end
begin
update Compras
set CompaniaId=@GCompania,CompraTipoSunat=@GTCSunat,CompraSerie=@GSerie,CompraNumero=@GNumero,CompraEmision=@GEmision,
CompraComputo=@GComputo,CompraCorrelativo=@GCorrelativo,CompraTipoIgv=@GTipo,CompraOBS=@GOBS,TipoCodigo='09',CompraUsuario=@Usuario,
CompraSubtotal=@Subtotal,CompraIgv=@Igv,CompraTotal=@Total
where CompraId=@CompraId
begin
delete from GuiaCanje
where CanjeId=@CanjeId
end
end
GO
/****** Object:  StoredProcedure [dbo].[eliminarCompra]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[eliminarCompra] 
@CompraId numeric(38)
as
begin
delete from DetalleCompra 
where CompraId=@CompraId
delete from Compras
where CompraId=@CompraId
select 'true'
end
GO
/****** Object:  StoredProcedure [dbo].[eliminarDocumento]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[eliminarDocumento]
@DocuId numeric(38)
as
begin
delete from DetalleDocumento 
where DocuId=@DocuId
end
begin
delete from DocumentoVenta
where DocuId=@DocuId
end
GO
/****** Object:  StoredProcedure [dbo].[eliminarGeneral]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[eliminarGeneral]
@Data varchar(max)
as
begin
Declare @IdGeneral numeric(38)
set @IdGeneral=@Data
	delete from CajaGeneral
	where IdGeneral=@IdGeneral
	update CajaPincipal
	set IdGeneral=0
	where IdGeneral=@IdGeneral
end
begin
select isnull((select STUFF((select '¬'+ convert(varchar,c.IdCaja)+'|'+c.CajaConcepto+'|'+convert(varchar,c.CajaId)+'|'+
Convert(char(10),c.CajaFecha,103)+' '+Convert(char(8),c.CajaFecha,114) 
+'|'+c.CajaDescripcion+'|'+CONVERT(VarChar(50),cast(c.CajaMonto as money), 1)+'|'+
c.CajaUsuario+'|'+c.Referencia+'|'+c.GastoId
from CajaPincipal c 
where c.CajaConcepto='INGRESO' and c.IdGeneral=0
order by c.IdCaja desc
for xml path('')),1,1,'')),'~')+'['+
isnull((select STUFF((select '¬'+ convert(varchar,c.IdCaja)+'|'+c.CajaConcepto+'|'+convert(varchar,c.CajaId)+'|'+
Convert(char(10),c.CajaFecha,103)+' '+Convert(char(8),c.CajaFecha,114) 
+'|'+c.CajaDescripcion+'|'+CONVERT(VarChar(50),cast(c.CajaMonto as money), 1)+'|'+
c.CajaUsuario+'|'+c.Referencia+'|'+c.GastoId
from CajaPincipal c 
where c.CajaConcepto='SALIDA' and c.IdGeneral=0
order by c.IdCaja desc
for xml path('')),1,1,'')),'~')+'['+
isnull((select STUFF ((select '¬'+ CONVERT(varchar,c.IdGeneral)+'|'+
(IsNull(convert(varchar,c.FechaCierre,103),'')+' '+ IsNull(SUBSTRING(convert(varchar,c.FechaCierre,114),1,8),''))+'|'+c.Usuario+'|'+
CONVERT(varchar(50),cast(c.Ingresos as money),1)+'|'+CONVERT(varchar(50),cast(c.Salidas as money),1)+'|'+
CONVERT(varchar(50),cast(c.Total as money),1)
from CajaGeneral c
order by c.IdGeneral desc
for xml path('')),1,1,'')),'~')
end
GO
/****** Object:  StoredProcedure [dbo].[eliminarGuia]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[eliminarGuia] 
@GuiaId numeric(38)
as
begin
delete from DetalleGuia
where GuiaId=@GuiaId
end
begin
delete from GuiaRemision
where GuiaId=@GuiaId
end
GO
/****** Object:  StoredProcedure [dbo].[eliminarletra]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[eliminarletra] @LetraId numeric(38)
as
begin
delete from DocumentoCanje
where LetraId=@LetraId
begin
delete from DetalleLetra
where LetraId=@LetraId
begin
delete from Letra
where LetraId=@LetraId
end
end
end
GO
/****** Object:  StoredProcedure [dbo].[eliminarliquida]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[eliminarliquida] 
@ListaOrden varchar(Max)    
as  
begin   
Declare @pos1 int,@pos2 int    
Declare @orden varchar(max),    
        @detalle varchar(max)    
Set @pos1 = CharIndex('[',@ListaOrden,0)    
Set @pos2 =Len(@ListaOrden)+1    
Set @orden = SUBSTRING(@ListaOrden,1,@pos1-1)    
Set @detalle = SUBSTRING(@ListaOrden,@pos1+1,@pos2-@pos1-1) 

Declare @LiquidacionId numeric(38)    
set @LiquidacionId=@orden 
		
Begin Transaction    
Declare Tabla Cursor For Select * From fnSplitString(@detalle,';')     
Open Tabla 
Declare @Columna varchar(max),
        @DetalleId nvarchar(38),
        @CompraId numeric(18,2),  
		@Acuenta decimal(18,2),  
		@Concepto varchar(40)        
Declare @p1 int,@p2 int,@p3 int,@p4 int  
Fetch Next From Tabla INTO @Columna    
 While @@FETCH_STATUS = 0    
 Begin    
Set @p1= CharIndex('|',@Columna,0)    
Set @p2= CharIndex('|',@Columna,@p1+1)
Set @p3= CharIndex('|',@Columna,@p2+1)    
Set @p4=Len(@Columna)+1

set @DetalleId=SUBSTRING(@Columna,1,@p1-1)  
Set @CompraId=SUBSTRING(@Columna,@p1+1,@p2-(@p1+1))      
Set @Acuenta=Convert(decimal(18,2),SUBSTRING(@Columna,@p2+1,@p3-(@p2+1)))    
Set @Concepto=SUBSTRING(@Columna,@p3+1,@p4-(@p3+1))   

if(@Concepto='LETRA')  
begin  
	update DetalleLetra  
	set DetalleSaldo=DetalleSaldo+@Acuenta,DetalleEstado='PENDIENTE DE PAGO'  
	where DetalleId=@CompraId  
end  
else  
begin  
	update Compras  
	set CompraSaldo=CompraSaldo+@Acuenta,CompraEstado='PENDIENTE DE PAGO'  
	where CompraId=@CompraId  
end

delete from CajaDetalle    
where LiquidaId='LP-'+@DetalleId  
  
Fetch Next From Tabla INTO @Columna    
end    
  Close Tabla;    
  Deallocate Tabla; 
  delete from DetalleLiquida  
  where LiquidacionId=@LiquidacionId   
  delete from Liquidacion  
  where LiquidacionId=@LiquidacionId 
  Commit Transaction;
  Select 'true' 
 END
GO
/****** Object:  StoredProcedure [dbo].[eliminarNotaPedido]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[eliminarNotaPedido] 
@NotaId numeric(38)
as
begin
Begin Transaction
delete from DetallePedido
where NotaId=@NotaId
delete from NotaPedido
where NotaId=@NotaId
Commit Transaction;
select 'true'
end
GO
/****** Object:  StoredProcedure [dbo].[eliminarRenta]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[eliminarRenta] 
@Data varchar(max)
as
begin
Declare @RentaId numeric(38),
@Cantidad int
Set @Data = LTRIM(RTrim(@Data))
Set @RentaId=convert(numeric(38),@Data)
delete from RentaMensual
where RentaId=@RentaId
set @Cantidad=(select COUNT(r.RentaId) from RentaMensual r)
if @Cantidad<=0
begin
select 'true'
end
else
begin
(select STUFF((select '¬'+convert(varchar,r.RentaId)+'|'+convert(varchar,r.CompaniaId)+'|'+convert(varchar,r.RentaANNO)+'|'+
convert(varchar,r.RentaMes)+'|'+dbo.MesNombre(r.RentaMes)+' '+convert(varchar,r.RentaANNO)+'|'+
CONVERT(VarChar(50), cast((r.IGV) as money ), 1)+'|'+CONVERT(VarChar(50), cast((r.Renta) as money ), 1)+'|'+
CONVERT(VarChar(50), cast((r.SaldoIGV) as money ), 1)+'|'+CONVERT(VarChar(50), cast((r.SaldoRenta) as money ), 1)+'|'+
CONVERT(VarChar(50), cast((r.InteresIgv) as money ), 1)+'|'+CONVERT(VarChar(50), cast((r.InteresRenta) as money ), 1)+'|'+
CONVERT(VarChar(50), cast((r.TributoIgv) as money ), 1)+'|'+CONVERT(VarChar(50), cast((r.TributoRenta) as money ), 1)+'|'+
CONVERT(char(1),r.FormaPago)+'|'+convert(varchar,r.FechaCancelacion,103)+'|'+r.EntidadBancaria+'|'+r.NroOperacion+'|'+
CONVERT(VarChar(50), cast((r.PagoTotal) as money ), 1)
from RentaMensual r
where year(r.FechaCancelacion)=year(getdate())
order by r.RentaId desc
for xml path('')),1,1,''))
end
end
GO
/****** Object:  StoredProcedure [dbo].[eliminartemporales]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[eliminartemporales] @usuarioId int
as
begin
delete from temporalLetra
where UsuarioId=@usuarioId
begin
delete from TemporalCanje
where UsuarioId=@usuarioId
end
end
GO
/****** Object:  StoredProcedure [dbo].[eliminarUM]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[eliminarUM]
@Data varchar(max)
as
begin
    Set @Data = LTRIM(RTrim(@Data))
	Declare @pos1 int,@pos2 int
	declare @IdUm int,@IdProducto numeric(20)
	declare @contador int
Set @pos1 = CharIndex('|',@Data,0)
Set @IdUm =convert(int,SUBSTRING(@Data,1,@pos1-1))
Set @pos2 =Len(@Data)+1
Set @IdProducto=convert(numeric,SUBSTRING(@Data,@pos1+1,@pos2-@pos1-1))
	delete from UnidadMedida
	where IdUm=@IdUm
set @contador=(select COUNT(*) from UnidadMedida where IdProducto=@IdProducto)	
if 	@contador<=0
begin
	select 'true'
end
else
begin
	(select STUFF ((select '¬'+convert(varchar,m.IdUm)+'|'+CONVERT(varchar,m.IdProducto)+'|'+m.UMDescripcion+'|'+
	CONVERT(VarChar(50), cast(m.ValorUM as money ),2)+'|'+CONVERT(VarChar(50),cast(m.PrecioVenta as money ), 1)+'|'+CONVERT(VarChar(50), cast(m.PrecioVentaB as money ), 1)+'|'+
	CONVERT(varchar(50),m.PrecioCosto)
	from UnidadMedida m
	where m.IdProducto=@IdProducto
	order by m.ValorUM asc
	for xml path('')),1,1,''))
end
end
GO
/****** Object:  StoredProcedure [dbo].[equivalenteProducto]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[equivalenteProducto]
as
begin
select 'IdPro|Descripcion|UM|Valor|UB|PrecioVenta|PrecioVentaB|PrecioCosto¬100|450|100|100|100|100|100|100¬String|String|String|Decimal|String|Decimal|Decimal|Decimal¬'+
isnull((select STUFF ((select '¬'+convert(varchar,p.IdProducto)+'|'+
p.ProductoNombre+'|'+u.UMDescripcion+'|'+
convert(varchar,u.ValorUM)+'|'+p.ProductoUM+'|'+
convert(varchar,u.PrecioVenta)+'|'+
convert(varchar,u.PrecioVentaB)+'|'+
convert(varchar,u.PrecioCosto)
from UnidadMedida u
inner join Producto p
on p.IdProducto=u.IdProducto
order by p.ProductoNombre asc
for xml path('')),1,1,'')),'~')
end
GO
/****** Object:  StoredProcedure [dbo].[ingresarCompra]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[ingresarCompra]    
@ListaOrden varchar(Max)    
as    
begin    
Declare @pos int    
Declare @orden varchar(max)    
Declare @detalle varchar(max)    
Set @pos = CharIndex('[',@ListaOrden,0)    
Set @orden = SUBSTRING(@ListaOrden,1,@pos-1)    
Set @detalle = SUBSTRING(@ListaOrden,@pos+1,len(@ListaOrden)-@pos)    
Declare @c1 int,@c2 int,@c3 int,@c4 int,    
        @c5 int,@c6 int,@c7 int,@c8 int,    
        @c9 int,@c10 int,@c11 int,@c12 int,    
        @c13 int,@c14 int,@c15 int,@c16 int,    
        @c17 int,@c18 int,@c19 int,@c20 int,    
        @c21 int,@c22 int,@c23 int,@c24 int,    
        @c25 int,@c26 int,@c27 int,@c28 int    
Declare @CompaniaId int,@CompraId numeric(38),@UsuarioId int,    
@CompraCorrelativo varchar(80),    
@ProveedorId numeric(38),    
@CompraEmision date,    
@CompraComputo date,    
@TipoCodigo char(20),    
@CompraSerie varchar(60),    
@CompraNumero varchar(80),    
@CompraCondicion varchar(60),    
@CompraMoneda varchar(60),    
@CompraTipoCambio decimal(18,3),    
@CompraDias int,    
@CompraFechaPago date,    
@CompraUsuario varchar(80),    
@CompraTipoIgv varchar(60),    
@CompraValorVenta decimal(18,2),    
@CompraDescuento decimal(18,2),    
@CompraSubtotal decimal(18,2),    
@CompraIgv decimal(18,2),    
@CompraTotal decimal(18,2),    
@CompraEstado varchar(60),    
@CompraAsociado varchar(60),    
@compraSaldo decimal(18,2),    
@CompraOBS varchar(max),    
@CompraTipoSunat decimal(18,3),    
@CompraConcepto varchar(60),    
@CompraPercepcion decimal(18,2)    
Set @c1 = CharIndex('|',@orden,0)    
Set @c2 = CharIndex('|',@orden,@c1+1)    
Set @c3 = CharIndex('|',@orden,@c2+1)    
Set @c4 = CharIndex('|',@orden,@c3+1)    
Set @c5 = CharIndex('|',@orden,@c4+1)    
Set @c6= CharIndex('|',@orden,@c5+1)    
Set @c7 = CharIndex('|',@orden,@c6+1)    
Set @c8 = CharIndex('|',@orden,@c7+1)    
Set @c9 = CharIndex('|',@orden,@c8+1)    
Set @c10= CharIndex('|',@orden,@c9+1)    
Set @c11= CharIndex('|',@orden,@c10+1)    
Set @c12= CharIndex('|',@orden,@c11+1)    
Set @c13= CharIndex('|',@orden,@c12+1)    
Set @c14= CharIndex('|',@orden,@c13+1)    
Set @c15= CharIndex('|',@orden,@c14+1)    
Set @c16= CharIndex('|',@orden,@c15+1)    
Set @c17= CharIndex('|',@orden,@c16+1)    
Set @c18 = CharIndex('|',@orden,@c17+1)    
Set @c19 = CharIndex('|',@orden,@c18+1)    
Set @c20= CharIndex('|',@orden,@c19+1)    
Set @c21= CharIndex('|',@orden,@c20+1)    
Set @c22= CharIndex('|',@orden,@c21+1)    
Set @c23= CharIndex('|',@orden,@c22+1)    
Set @c24= CharIndex('|',@orden,@c23+1)    
Set @c25= CharIndex('|',@orden,@c24+1)    
Set @c26= CharIndex('|',@orden,@c25+1)    
Set @c27= CharIndex('|',@orden,@c26+1)    
Set @c28= Len(@orden)+1    
set @CompaniaId=convert(int,SUBSTRING(@orden,1,@c1-1))    
set @CompraCorrelativo=SUBSTRING(@orden,@c1+1,@c2-@c1-1)    
set @ProveedorId=convert(numeric(38),SUBSTRING(@orden,@c2+1,@c3-@c2-1))    
set @CompraEmision=convert(date,SUBSTRING(@orden,@c3+1,@c4-@c3-1))    
set @CompraComputo=convert(date,SUBSTRING(@orden,@c4+1,@c5-@c4-1))    
set @TipoCodigo=SUBSTRING(@orden,@c5+1,@c6-@c5-1)    
set @CompraSerie=SUBSTRING(@orden,@c6+1,@c7-@c6-1)    
set @CompraNumero=SUBSTRING(@orden,@c7+1,@c8-@c7-1)    
set @CompraCondicion=SUBSTRING(@orden,@c8+1,@c9-@c8-1)    
set @CompraMoneda=SUBSTRING(@orden,@c9+1,@c10-@c9-1)    
set @CompraTipoCambio=convert(decimal(18,3),SUBSTRING(@orden,@c10+1,@c11-@c10-1))    
set @CompraDias=convert(int,SUBSTRING(@orden,@c11+1,@c12-@c11-1))    
set @CompraFechaPago=convert(date,SUBSTRING(@orden,@c12+1,@c13-@c12-1))    
set @CompraUsuario=SUBSTRING(@orden,@c13+1,@c14-@c13-1)        
set @CompraTipoIgv=SUBSTRING(@orden,@c14+1,@c15-@c14-1)    
set @CompraValorVenta=convert(decimal(18,2),SUBSTRING(@orden,@c15+1,@c16-@c15-1))        
set @CompraDescuento=convert(decimal(18,2),SUBSTRING(@orden,@c16+1,@c17-@c16-1))    
set @CompraSubtotal=convert(decimal(18,2),SUBSTRING(@orden,@c17+1,@c18-@c17-1))    
set @CompraIgv=convert(decimal(18,2),SUBSTRING(@orden,@c18+1,@c19-@c18-1))    
set @CompraTotal=convert(decimal(18,2),SUBSTRING(@orden,@c19+1,@c20-@c19-1))    
set @CompraEstado=SUBSTRING(@orden,@c20+1,@c21-@c20-1)    
set @CompraAsociado=SUBSTRING(@orden,@c21+1,@c22-@c21-1)    
set @compraSaldo=convert(decimal(18,2),SUBSTRING(@orden,@c22+1,@c23-@c22-1))    
set @CompraOBS=SUBSTRING(@orden,@c23+1,@c24-@c23-1)    
set @CompraTipoSunat =convert(decimal(18,3),SUBSTRING(@orden,@c24+1,@c25-@c24-1))    
set @CompraConcepto =SUBSTRING(@orden,@c25+1,@c26-@c25-1)    
set @CompraPercepcion=convert(decimal(18,2),SUBSTRING(@orden,@c26+1,@c27-@c26-1))    
set @UsuarioId=convert(int,SUBSTRING(@orden,@c27+1,@c28-@c27-1))    
Begin Transaction    
insert into Compras values(@CompaniaId,@CompraCorrelativo,@ProveedorId,GETDATE(),    
@CompraEmision,@CompraComputo,@TipoCodigo,@CompraSerie,@CompraNumero,@CompraCondicion,    
@CompraMoneda,@CompraTipoCambio,@CompraDias,@CompraFechaPago,@CompraUsuario,@CompraTipoIgv,    
@CompraValorVenta,@CompraDescuento,@CompraSubtotal,@CompraIgv,@CompraTotal,@CompraEstado,    
@CompraAsociado,@compraSaldo,@CompraOBS,@CompraTipoSunat,@CompraConcepto,@CompraPercepcion)    
set @CompraId=(select @@IDENTITY)    
Declare Tabla Cursor For Select * From fnSplitString(@detalle,';')     
Open Tabla    
Declare @Columna varchar(max),    
        @IdProducto numeric(20),    
        @DetalleCodigo varchar(80),    
        @Descripcion varchar(255),    
        @DetalleUM   varchar(60),    
        @DetalleCantidad decimal(18,2),    
        @PrecioCosto  decimal(18,4),    
        @DetalleImprte decimal(18,4),    
        @DetalleDescuento decimal(18,4),    
        @DetalleEstado varchar(60),    
        @CostoReal decimal(18,4),
        @AplicaINV nvarchar(1)    
Declare @p1 int,@p2 int,@p3 int,@p4 int,    
        @p5 int,@p6 int,@p7 int,@p8 int,
        @p9 int,@p10 int,@p11 int    
        Fetch Next From Tabla INTO @Columna    
While @@FETCH_STATUS = 0    
Begin    
Set @p1=CharIndex('|',@Columna,0)    
Set @p2=CharIndex('|',@Columna,@p1+1)    
Set @p3=CharIndex('|',@Columna,@p2+1)    
Set @p4=CharIndex('|',@Columna,@p3+1)    
Set @p5=CharIndex('|',@Columna,@p4+1)    
Set @p6=CharIndex('|',@Columna,@p5+1)    
Set @p7=CharIndex('|',@Columna,@p6+1)    
Set @p8=CharIndex('|',@Columna,@p7+1)    
Set @p9=CharIndex('|',@Columna,@p8+1)
Set @p10=CharIndex('|',@Columna,@p9+1) 
Set @p11=Len(@Columna)+1    
set @IdProducto=Convert(numeric(20),SUBSTRING(@Columna,1,@p1-1))    
Set @DetalleCodigo=SUBSTRING(@Columna,@p1+1,@p2-(@p1+1))    
Set @Descripcion=SUBSTRING(@Columna,@p2+1,@p3-(@p2+1))    
Set @DetalleUM=SUBSTRING(@Columna,@p3+1,@p4-(@p3+1))    
Set @DetalleCantidad=convert(decimal(18,2),SUBSTRING(@Columna,@p4+1,@p5-(@p4+1)))    
Set @PrecioCosto=convert(decimal(18,4),SUBSTRING(@Columna,@p5+1,@p6-(@p5+1)))    
Set @DetalleDescuento=convert(decimal(18,4),SUBSTRING(@Columna,@p6+1,@p7-(@p6+1)))    
Set @DetalleImprte=convert(decimal(18,4),SUBSTRING(@Columna,@p7+1,@p8-(@p7+1)))    
Set @DetalleEstado=SUBSTRING(@Columna,@p8+1,@p9-(@p8+1))    
Set @CostoReal=convert(decimal(18,4),SUBSTRING(@Columna,@p9+1,@p10-(@p9+1)))
Set @AplicaINV=SUBSTRING(@Columna,@p10+1,@p11-(@p10+1))       
    
insert into DetalleCompra values(@CompraId,@IdProducto,@DetalleCodigo,    
@Descripcion,@DetalleUM,@DetalleCantidad,@PrecioCosto,@DetalleImprte,    
@DetalleDescuento,@DetalleEstado,0,'',1)    

if(@CompraConcepto='MERCADERIA')    
BEGIN    

if(@AplicaINV='S')
BEGIN

declare @IniciaStock decimal(18,2),@stockFinal decimal(18,2)    
set @IniciaStock=(select top 1 p.ProductoCantidad from Producto p where p.IdProducto=@IdProducto)    
set @stockFinal=@IniciaStock+@DetalleCantidad    
if(@DetalleEstado='BONIFICACION')    
begin    
update Producto     
set ProductoCantidad=ProductoCantidad+@DetalleCantidad    
where IdProducto=@IdProducto     
end    
else    
begin    
update Producto     
set ProductoCantidad=ProductoCantidad+@DetalleCantidad,ProductoCosto=@CostoReal    
where IdProducto=@IdProducto     
end    
insert into Kardex values(@IdProducto,DATEADD(HOUR, 1, GETDATE()),'Ingreso por Compra',@CompraSerie+'-'+@CompraNumero,@IniciaStock,    
@DetalleCantidad,0,@CostoReal,@StockFinal,'INGRESO',@CompraUsuario)
   
END
END    
    
Fetch Next From Tabla INTO @Columna    
end    
 Close Tabla;    
 Deallocate Tabla;    
    Commit Transaction;    
    if(@CompraConcepto='MERCADERIA')    
    begin    
    delete from TemporalCompra 
    where UsuarioID=@UsuarioId    
    end    
    else    
    begin    
    delete from TemporalServicio 
    where UsuarioId=@UsuarioId    
    end    
    select 'true'    
end
GO
/****** Object:  StoredProcedure [dbo].[ingresarDetaCajaB]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[ingresarDetaCajaB]    
@Data varchar(max)    
as    
begin    
Declare @p1 int,@p2 int,    
  @p3 int,@p4 int,    
  @p5 int,@p6 int,    
  @p7 int,@p8 int,    
  @p9 int,@p10 int,    
  @p11 int    
Declare @CajaId numeric(38),@NotaId numeric(38),    
  @Movimiento varchar(80),    
  @Concepto varchar(250),@Monto decimal(18,2),    
  @Efectivo decimal(18,2),@Vuelto decimal(18,2),    
  @DetalleId numeric(38),@RutaImagen varchar(max),    
  @Usuario VARCHAR(80),@GastoIdB varchar(40)    
Set @Data = LTRIM(RTrim(@Data))    
Set @p1 = CharIndex('|',@Data,0)    
Set @p2 = CharIndex('|',@Data,@p1+1)    
Set @p3 = CharIndex('|',@Data,@p2+1)    
Set @p4 = CharIndex('|',@Data,@p3+1)    
Set @p5 = CharIndex('|',@Data,@p4+1)    
Set @p6 =CharIndex('|',@Data,@p5+1)    
Set @p7 = CharIndex('|',@Data,@p6+1)    
Set @p8 = CharIndex('|',@Data,@p7+1)    
Set @p9 = CharIndex('|',@Data,@p8+1)    
Set @p10 = CharIndex('|',@Data,@p9+1)    
Set @p11= Len(@Data)+1    
Set @CajaId =convert(numeric(38),SUBSTRING(@Data,1,@p1-1))    
Set @NotaId=convert(numeric(38),SUBSTRING(@Data,@p1+1,@p2-@p1-1))    
Set @Movimiento=SUBSTRING(@Data,@p2+1,@p3-@p2-1)    
Set @Concepto=SUBSTRING(@Data,@p3+1,@p4-@p3-1)    
Set @Monto=convert(decimal(18,2),SUBSTRING(@Data,@p4+1,@p5-@p4-1))    
Set @Efectivo=convert(decimal(18,2),SUBSTRING(@Data,@p5+1,@p6-@p5-1))    
Set @Vuelto=convert(decimal(18,2),SUBSTRING(@Data,@p6+1,@p7-@p6-1))    
Set @DetalleId=convert(numeric(38),SUBSTRING(@Data,@p7+1,@p8-@p7-1))    
Set @RutaImagen=SUBSTRING(@Data,@p8+1,@p9-@p8-1)    
Set @Usuario=SUBSTRING(@Data,@p9+1,@p10-@p9-1)    
Set @GastoIdB=SUBSTRING(@Data,@p10+1,@p11-@p10-1)

set @CajaId=(select isnull((select stuff((select '¬'+ convert(varchar,c.CajaId)
from Caja c where c.CajaEstado='ACTIVO' order by c.CajaId desc for xml path('')),1,1,'')),'0'))

if(@CajaId='0')
begin
select 'existe'
end
else
begin   
Declare @Referencia varchar(80)    
set @Referencia=@Movimiento    
if(@Movimiento='INGRESO')SET @Movimiento='INGRESO'    
else set @Movimiento='SALIDA'    
if(@DetalleId=0)    
begin    
Declare @GastoId numeric(38)    
if(@Referencia='GASTO INTERNO')    
begin    
 insert into GastosFijos values(GETDATE(),@Concepto,@Monto,GETDATE(),@Usuario,'C')    
 Set @GastoId= @@identity    
 insert into CajaDetalle values(@CajaId,GETDATE(),@NotaId,@Movimiento,@Referencia,    
 @Concepto,@Monto,@Efectivo,@Vuelto,@RutaImagen,'T','',@Usuario,CONVERT(varchar,@GastoId),'')    
 select 'true'  
end    
else    
begin    
 insert into CajaDetalle values(@CajaId,GETDATE(),@NotaId,@Movimiento,@Referencia,    
 @Concepto,@Monto,@Efectivo,@Vuelto,@RutaImagen,'T','',@Usuario,'','')    
 select 'true'    
end    
END   
else    
begin    
if(@GastoIdB='')    
begin  
if(@Referencia='GASTO INTERNO')    
begin    
 insert into GastosFijos values(GETDATE(),@Concepto,@Monto,GETDATE(),@Usuario,'C')    
 Set @GastoId= @@identity    
 update CajaDetalle    
 set DetalleFecha=getdate(),DetalleMovimiento=@Movimiento,    
 DetalleReferencia=@Referencia,    
 DetalleConcepto=@Concepto,DetalleMonto=@Monto,DetalleEfectivo=@Efectivo,    
 RutaImagen=@RutaImagen,Usuario=@Usuario,GastoId=convert(varchar,@GastoId)    
 where DetalleId=@DetalleId    
 select 'true'    
end    
else    
begin    
delete from GastosFijos     
where GastoId=@GastoIdB    
update CajaDetalle    
set DetalleFecha=getdate(),DetalleMovimiento=@Movimiento,    
DetalleReferencia=@Referencia,    
DetalleConcepto=@Concepto,DetalleMonto=@Monto,DetalleEfectivo=@Efectivo,    
RutaImagen=@RutaImagen,Usuario=@Usuario,GastoId=''    
where DetalleId=@DetalleId    
select 'true'    
end    
end    
else    
begin    
if(@Referencia='GASTO INTERNO')    
begin    
 update GastosFijos    
 set GastoFecha=GETDATE(),GsstoDesc=@Concepto,GstoMonto=@Monto,    
 GastoReg=GETDATE(),GastoUsuario=@Usuario,Estado='C'    
 where GastoId=@GastoIdB    
 update CajaDetalle    
 set DetalleFecha=GETDATE(),DetalleMovimiento=@Movimiento,DetalleReferencia=@Referencia,    
 DetalleConcepto=@Concepto,DetalleMonto=@Monto,DetalleEfectivo=@Efectivo,    
 RutaImagen=@RutaImagen,Usuario=@Usuario,GastoId=@GastoIdB    
 where DetalleId=@DetalleId    
 select 'true'    
end    
else    
begin    
 delete from GastosFijos     
 where GastoId=@GastoIdB    
 update CajaDetalle    
 set DetalleFecha=GETDATE(),DetalleMovimiento=@Movimiento,DetalleReferencia=@Referencia,    
 DetalleConcepto=@Concepto,DetalleMonto=@Monto,DetalleEfectivo=@Efectivo,    
 RutaImagen=@RutaImagen,Usuario=@Usuario,GastoId=''    
 where DetalleId=@DetalleId    
 select 'true'    
end    
end    
end
end    
end
GO
/****** Object:  StoredProcedure [dbo].[ingresarProveedor]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[ingresarProveedor]  
@Data varchar(max)  
as  
begin  
Declare @p1 int,@p2 int,  
  @p3 int,@p4 int,  
  @p5 int,@p6 int,  
  @p7 int,@p8 int,  
  @p9 int   
Declare @ProveedorId numeric(38),@Razon varchar(250),  
		@Ruc varchar(20),@Contacto varchar(140),  
		@Celular varchar(140),@Telefono varchar(140),  
		@Correo varchar(140),@Direccion varchar(140),  
		@Estado varchar(40)  
Set @Data = LTRIM(RTrim(@Data))  
Set @p1 = CharIndex('|',@Data,0)  
Set @p2 = CharIndex('|',@Data,@p1+1)  
Set @p3 = CharIndex('|',@Data,@p2+1)  
Set @p4 = CharIndex('|',@Data,@p3+1)  
Set @p5 = CharIndex('|',@Data,@p4+1)  
Set @p6 = CharIndex('|',@Data,@p5+1)  
Set @p7 = CharIndex('|',@Data,@p6+1)  
Set @p8 = CharIndex('|',@Data,@p7+1)  
Set @p9 =Len(@Data)+1  
set @ProveedorId=convert(numeric(38),SUBSTRING(@Data,1,@p1-1))  
set @Razon=SUBSTRING(@Data,@p1+1,@p2-@p1-1)  
set @Ruc=SUBSTRING(@Data,@p2+1,@p3-@p2-1)  
set @Contacto=SUBSTRING(@Data,@p3+1,@p4-@p3-1)  
set @Celular=SUBSTRING(@Data,@p4+1,@p5-@p4-1)  
set @Telefono=SUBSTRING(@Data,@p5+1,@p6-@p5-1)  
set @Correo=SUBSTRING(@Data,@p6+1,@p7-@p6-1)  
set @Direccion=SUBSTRING(@Data,@p7+1,@p8-@p7-1)  
set @Estado=SUBSTRING(@Data,@p8+1,@p9-@p8-1)  
if (@ProveedorId=0)  
begin
 IF EXISTS(select top 1 p.ProveedorRuc 
 from Proveedor p
 where p.ProveedorRuc=@Ruc)
 begin
 select 'RUC'
 end
 else
 begin
 insert into Proveedor values(@Razon,@Ruc,@Contacto,@Celular,@Telefono,@Correo,@Direccion,@Estado)
 select
 isnull((select stuff((SELECT '¬'+ CONVERT(varchar,p.ProveedorId)+'|'+p.ProveedorRazon+'|'+p.ProveedorRuc+'|'+  
 p.ProveedorContacto+'|'+p.ProveedorCelular+'|'+p.ProveedorTelefono+'|'+p.ProveedorCorreo+'|'+  
 p.ProveedorDireccion+'|'+p.ProveedorEstado  
 from Proveedor p  
 order by p.ProveedorId desc  
 for xml path('')),1,1,'')),'~')  
 end
end  
else  
begin  
update Proveedor  
set ProveedorRazon=@Razon,ProveedorRuc=@Ruc,ProveedorContacto=@Contacto,  
ProveedorCelular=@Celular,ProveedorTelefono=@Telefono,ProveedorCorreo=@Correo,  
ProveedorDireccion=@Direccion,ProveedorEstado=@Estado  
where ProveedorId=@ProveedorId
 select
 isnull((select stuff((SELECT '¬'+ CONVERT(varchar,p.ProveedorId)+'|'+p.ProveedorRazon+'|'+p.ProveedorRuc+'|'+  
 p.ProveedorContacto+'|'+p.ProveedorCelular+'|'+p.ProveedorTelefono+'|'+p.ProveedorCorreo+'|'+  
 p.ProveedorDireccion+'|'+p.ProveedorEstado  
 from Proveedor p  
 order by p.ProveedorId desc  
 for xml path('')),1,1,'')),'~')
end
end
GO
/****** Object:  StoredProcedure [dbo].[insertaGuiaCanje]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[insertaGuiaCanje]
@CompraId numeric(38),
@CompaniaId int,
@CanjeFecha date,
@CanjeRegistro datetime,
@CanjeSerie varchar(80),
@CanjeNumero varchar(80),
@CanjeEmision date,
@CanjeComputo date,
@CanjeCorrelativo varchar(80),
@CanjeTipo varchar(80),
@CanjeOBS varchar(max),
@TCSunat decimal(18,3),
@GCompania int,
@GSerie varchar(80),
@GNumero varchar(80),
@GEmision date,
@GCanjeComputo date,
@GCanjeCorrelativo varchar(80),
@GCanjeTipo varchar(80),
@GCanjeOBS varchar(max),
@GTCSunat decimal(18,3),
@CanjeUsuario varchar(60),
@Subtotal decimal(18,2),
@Igv decimal(18,2),
@Total decimal(18,2)
as
begin
insert into GuiaCanje values(@CompraId,@CompaniaId,@CanjeFecha,@CanjeRegistro,@CanjeSerie,@CanjeNumero,
@CanjeEmision,@CanjeComputo,@CanjeCorrelativo,@CanjeTipo,@CanjeOBS,@TCSunat,@GCompania,@GSerie,@GNumero,@GEmision,
@GCanjeComputo,@GCanjeCorrelativo,@GCanjeTipo,@GCanjeOBS,@GTCSunat,@CanjeUsuario)
begin
update Compras
set CompaniaId=@CompaniaId,CompraTipoSunat=@TCSunat,CompraSerie=@CanjeSerie,CompraNumero=@CanjeNumero,CompraEmision=@CanjeEmision,
CompraComputo=@CanjeComputo,CompraCorrelativo=@CanjeCorrelativo,CompraTipoIgv=@CanjeTipo,CompraOBS=@CanjeOBS,TipoCodigo='01',
CompraSubtotal=@Subtotal,CompraIgv=@Igv,CompraTotal=@Total
where CompraId=@CompraId
end
end
GO
/****** Object:  StoredProcedure [dbo].[insertaLiquidaVenta]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[insertaLiquidaVenta]
@LiquidacionNumero varchar(80),
@LiquidacionRegistro datetime,
@LiquidacionFecha date,
@LiquidacionDescripcion varchar(250),
@LiquidacionCambio decimal(18,3),
@LiquidaEfectivoSol decimal(18,2),
@LiquidaDepositoSol decimal(18,2),
@LiquidaTotalSol decimal(18,2),
@LiquidaEfectivoDol decimal(18,2),
@LiquidaDepositoDol decimal(18,2),
@LiquidaTotalDol decimal(18,2),
@LiquidaUsuario varchar(60)
as
begin
insert into LiquidacionVenta values(@LiquidacionNumero,
@LiquidacionRegistro,@LiquidacionFecha,@LiquidacionDescripcion,
@LiquidacionCambio,@LiquidaEfectivoSol,@LiquidaDepositoSol,
@LiquidaTotalSol,@LiquidaEfectivoDol,@LiquidaDepositoDol,
@LiquidaTotalDol,@LiquidaUsuario)
select @@identity
end
GO
/****** Object:  StoredProcedure [dbo].[insertarAlmacen]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[insertarAlmacen]
@Data varchar(max)
as
begin
Declare @pos1 int
Declare @pos2 int
Declare @pos3 int
Declare @pos4 int
Declare @pos5 int
Declare @pos6 int
Declare @pos7 int
Declare @AlmacenId numeric(20)
Declare @AlmacenNombre varchar(80)
Declare @AlmacenDepartamento varchar(80)
Declare @AlmacenProvincia varchar(80)
Declare @AlmacenDistrito varchar(80)
Declare @AlmacenDireccion varchar(300)
Declare @AlmacenEstado varchar(20)
Declare @AlmacenBD varchar(80)
Set @Data = LTRIM(RTrim(@Data))
Set @pos1 = CharIndex('|',@Data,0)
Set @AlmacenId =convert(numeric,SUBSTRING(@Data,1,@pos1-1))
Set @pos2 = CharIndex('|',@Data,@pos1+1)
Set @AlmacenNombre = SUBSTRING(@Data,@pos1+1,@pos2-@pos1-1)
Set @pos3 = CharIndex('|',@Data,@pos2+1)
Set @AlmacenDepartamento=SUBSTRING(@Data,@pos2+1,@pos3-@pos2-1)
Set @pos4 = CharIndex('|',@Data,@pos3+1)
Set @AlmacenProvincia=SUBSTRING(@Data,@pos3+1,@pos4-@pos3-1)
Set @pos5 = CharIndex('|',@Data,@pos4+1)
Set @AlmacenDistrito=SUBSTRING(@Data,@pos4+1,@pos5-@pos4-1)
Set @pos6 =CharIndex('|',@Data,@pos5+1)
Set @AlmacenDireccion=SUBSTRING(@Data,@pos5+1,@pos6-@pos5-1)
Set @pos7 = Len(@Data)+1
Set @AlmacenEstado=SUBSTRING(@Data,@pos6+1,@pos7-@pos6-1)
set @AlmacenBD=(select top 1 a.AlmacenNombre from Almacen a where AlmacenNombre=@AlmacenNombre)
if @AlmacenId=0
begin
if(@AlmacenBD=@AlmacenNombre)
begin
select 'existe'
end
else
begin
insert into Almacen values(@AlmacenNombre,@AlmacenDepartamento,@AlmacenProvincia,@AlmacenDistrito,@AlmacenDireccion,@AlmacenEstado)
(select STUFF((select '¬'+ convert(varchar,a.AlmacenId)+'|'+a.AlmacenNombre+'|'+a.AlmacenDepartamento+'|'+
a.AlmacenProvincia+'|'+a.AlmacenDistrito+'|'+a.AlmacenDireccion+'|'+a.AlmacenEstado
from Almacen a
order by AlmacenId desc
for xml path('')),1,1,''))
end
end
else
begin
update Almacen
set AlmacenNombre=@AlmacenNombre,AlmacenDepartamento=@AlmacenDepartamento,AlmacenProvincia=@AlmacenProvincia,AlmacenDistrito=@AlmacenDistrito,AlmacenDireccion=@AlmacenDireccion,AlmacenEstado=@AlmacenEstado
where AlmacenId=@AlmacenId
(select STUFF((select '¬'+ convert(varchar,a.AlmacenId)+'|'+a.AlmacenNombre+'|'+a.AlmacenDepartamento+'|'+
a.AlmacenProvincia+'|'+a.AlmacenDistrito+'|'+a.AlmacenDireccion+'|'+a.AlmacenEstado
from Almacen a
order by AlmacenId desc
for xml path('')),1,1,''))
end
end
GO
/****** Object:  StoredProcedure [dbo].[insertarCanje]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[insertarCanje]
@temporalCanje varchar(80),
@temporalDias int,
@temporalVencimiento varchar(20),
@temporalMonto decimal(18,2),
@usuarioId int
as
begin
insert into TemporalCanje values(@temporalCanje,
@temporalDias,@temporalVencimiento,@temporalMonto,@usuarioId)
end
GO
/****** Object:  StoredProcedure [dbo].[insertarDetaBonificacion]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[insertarDetaBonificacion]
@CompraId numeric(38),
@IdProducto numeric(20),
@DetalleCodigo varchar(80),
@Descripcion varchar(255),
@DetalleUM   varchar(60),
@DetalleCantidad decimal(18,2),
@PrecioCosto  decimal(18,4),
@DetalleImprte decimal(18,4),
@DetalleDescuento decimal(18,4),
@DetalleEstado varchar(60),
@KardexDocumento varchar(80),
@Usuario varchar(80)
as
Begin Transaction
insert into DetalleCompra values(@CompraId,@IdProducto,@DetalleCodigo,
@Descripcion,@DetalleUM,@DetalleCantidad,@PrecioCosto,@DetalleImprte,
@DetalleDescuento,@DetalleEstado,0,'',1)
declare @IniciaStock decimal(18,2),@StockFinal decimal(18,2),@Concepto varchar(40)
set @IniciaStock=(select top 1 ProductoCantidad from Producto where IdProducto=@IdProducto)
set @StockFinal=@IniciaStock+@DetalleCantidad
set @concepto='INGRESO'
insert into Kardex values(@IdProducto,GETDATE(),'Ingreso por Compra',@KardexDocumento,@IniciaStock,
@DetalleCantidad,0,@PrecioCosto,@StockFinal,@Concepto,@Usuario)
update producto 
set ProductoCantidad =ProductoCantidad + @DetalleCantidad
where IDProducto=@IdProducto
Commit Transaction;
GO
/****** Object:  StoredProcedure [dbo].[insertarDetaGuia]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[insertarDetaGuia]
@GuiaId numeric(38),
@IdProducto numeric(20),
@DetalleCantidad decimal(18,2),
@DetalleCosto decimal(18,4),
@DetallePrecio decimal(18, 2),
@DetalleImporte decimal(18, 2),
@DetalleEstado varchar(60),
@flac int,
@IdDetalle numeric(38),
@Documento varchar(80),
@Usuario varchar(80),
@Concepto varchar(80),
@ValorUM decimal(18,4),
@UniMedida varchar(40)
as
declare @Inicial decimal(18,2),@final decimal(18,2),@cantidad decimal(18,2)
set @Inicial=(select p.ProductoCantidad from Producto p where p.IdProducto=@IdProducto)
set @cantidad=(@DetalleCantidad * @ValorUM)
if(@Concepto='INGRESO')
begin
set @final=@Inicial+@cantidad
end
else
begin
set @final=@Inicial-@cantidad
end
begin
begin
insert into DetalleGuia values(@GuiaId,@IdProducto,@DetalleCantidad,@DetalleCosto,
@DetallePrecio,@DetalleImporte,@DetalleEstado,@IdDetalle,@ValorUM,@UniMedida)
if(@flac=1)
update DetallePedido
set CantidadSaldo=CantidadSaldo-@DetalleCantidad
where DetalleId=@IdDetalle
end
begin
update producto 
set ProductoCantidad =@final
where IDProducto=@IDProducto
end
begin
if(@Concepto='INGRESO')
begin
insert into Kardex values(@IdProducto,GETDATE(),
'Ingreso por Guia',@Documento,@inicial,@Cantidad,0,@DetalleCosto,@final,'INGRESO',@Usuario)
end
else
begin
insert into Kardex values(@IdProducto,GETDATE(),
'Salida por Guia',@Documento,@inicial,0,@cantidad,@DetalleCosto,@final,'SALIDA',@Usuario)
end
end
end
GO
/****** Object:  StoredProcedure [dbo].[insertarDetaLetra]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[insertarDetaLetra]
@LetraId  numeric(38),
@LetraCanje varchar(80),
@LetraDias int,
@LetraVencimiento date,
@DetalleSaldo decimal(18,2),
@DetalleMonto decimal(18,2),
@DetalleEstado varchar(60)
as
begin
insert into DetalleLetra values(@LetraId,@LetraCanje,
@LetraDias,@LetraVencimiento,@DetalleMonto,
@DetalleSaldo,@DetalleEstado)
end
GO
/****** Object:  StoredProcedure [dbo].[insertarDetaLiquida]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[insertarDetaLiquida]
@LiquidacionId numeric(38),
@CompraId numeric(38),
@SaldoDocu decimal(18,2),
@EfectivoSoles decimal(18, 2),
@EfectivoDolar decimal(18, 2),
@DepositoSoles decimal(18, 2),
@DepositoDolar decimal(18, 2),
@TipoCambio decimal(18, 3),
@EntidadBanco varchar(80),
@NroOperacion varchar(80),
@AcuentaGeneral decimal(18, 2),
@SaldoActual decimal(18, 2),
@FechaPago varchar(60),
@Numero varchar(60),
@Proveedor varchar(255),
@Moneda varchar(20),
@Concepto varchar(40),
@CompraEstado varchar(60)
as
begin
insert into DetalleLiquida values(
@LiquidacionId,@CompraId,@SaldoDocu,@EfectivoSoles,@EfectivoDolar,@DepositoSoles,
@DepositoDolar,@TipoCambio,@EntidadBanco,@NroOperacion,@AcuentaGeneral,@SaldoActual,@FechaPago,@Numero,
@Proveedor,@Moneda,@Concepto
)
begin
if(@Concepto='COMPRA')
begin
update Compras
set CompraSaldo=CompraSaldo - @AcuentaGeneral,CompraEstado=@CompraEstado
where CompraId=@CompraId
end
else
begin
update DetalleLetra
set DetalleSaldo=DetalleSaldo-@AcuentaGeneral,DetalleEstado=@CompraEstado
where DetalleId=@CompraId
end
end
end
GO
/****** Object:  StoredProcedure [dbo].[insertarDetalleCompra]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[insertarDetalleCompra] 
@Data varchar(max)  
as
begin
Declare @pos1 int,@pos2 int,          
        @pos3 int,@pos4 int,          
		@pos5 int,@pos6 int,    
		@pos7 int,@pos8 int,
		@pos9 int,@pos10 int     
Declare @CompraId numeric(38),@IdProducto numeric(20),  
		@Codigo varchar(80),@Descripcion varchar(255),  
		@UM varchar(60),@Cantidad decimal(18,2),  
		@PrecioCosto  decimal(18,4),@Importe decimal(18,4),  
		@Descuento decimal(18,4),@Estado varchar(60)
		
Set @Data = LTRIM(RTrim(@Data))          
Set @pos1 = CharIndex('|',@Data,0)        
Set @pos2 = CharIndex('|',@Data,@pos1+1)        
Set @pos3 = CharIndex('|',@Data,@pos2+1)            
Set @pos4 = CharIndex('|',@Data,@pos3+1)    
Set @pos5 = CharIndex('|',@Data,@pos4+1)            
Set @pos6 = CharIndex('|',@Data,@pos5+1)
Set @pos7 = CharIndex('|',@Data,@pos6+1)    
Set @pos8 = CharIndex('|',@Data,@pos7+1)            
Set @pos9 = CharIndex('|',@Data,@pos8+1)        
Set @pos10 = Len(@Data)+1

Set @CompraId=convert(numeric(38),SUBSTRING(@Data,1,@pos1-1))          
Set @IdProducto=convert(numeric(20),SUBSTRING(@Data,@pos1+1,@pos2-@pos1-1))          
Set @Codigo=SUBSTRING(@Data,@pos2+1,@pos3-@pos2-1)         
Set @Descripcion=SUBSTRING(@Data,@pos3+1,@pos4-@pos3-1)    
Set @UM=SUBSTRING(@Data,@pos4+1,@pos5-@pos4-1)    
Set @Cantidad=convert(decimal(18,2),SUBSTRING(@Data,@pos5+1,@pos6-@pos5-1))    
Set @PrecioCosto=convert(decimal(18,4),SUBSTRING(@Data,@pos6+1,@pos7-@pos6-1))

Set @Importe=convert(decimal(18,4),SUBSTRING(@Data,@pos7+1,@pos8-@pos7-1))    
Set @Descuento=convert(decimal(18,4),SUBSTRING(@Data,@pos8+1,@pos9-@pos8-1))    
Set @Estado=SUBSTRING(@Data,@pos9+1,@pos10-@pos9-1)        

insert into DetalleCompra values(@CompraId,@IdProducto,@Codigo,  
@Descripcion,@UM,@Cantidad,@PrecioCosto,@Importe,  
@Descuento,@Estado,0,'',1)
select 'true'

end
GO
/****** Object:  StoredProcedure [dbo].[insertarDetalleNota]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[insertarDetalleNota]  
@Data varchar(max)  
as  
begin  
Declare @p1 int,@p2 int,@p3 int,@p4 int,  
        @p5 int,@p6 int,@p7 int,@p8 int,  
        @p9 int,@p10 int,@p11 int,@p12 int  
Declare @NotaId numeric(38),  
  @IdProducto numeric(20),  
  @Cantidad decimal(18,2),  
  @DetalleUm varchar(40),  
  @Descripcion varchar(140),  
  @Costo decimal(18,2),   
  @Precio decimal(18,2),  
  @Importe decimal(18,2),  
  @Estado varchar(60),  
  @CantidadSaldo decimal(18,2),  
  @ValorUM decimal(18,4),  
  @DocuId numeric(38)=0  
Declare @DetalleNotaId numeric(38),  
        @Aviso varchar(max),  
        @Stock decimal(18,2),  
        @Existe int  
Set @Data= LTRIM(RTrim(@Data))  
set @p1=CharIndex('|',@Data,0)  
Set @p2=CharIndex('|',@Data,@p1+1)  
Set @p3=CharIndex('|',@Data,@p2+1)  
Set @p4=CharIndex('|',@Data,@p3+1)  
Set @p5=CharIndex('|',@Data,@p4+1)  
Set @p6=CharIndex('|',@Data,@p5+1)  
Set @p7=CharIndex('|',@Data,@p6+1)  
Set @p8=CharIndex('|',@Data,@p7+1)  
Set @p9=CharIndex('|',@Data,@p8+1)  
Set @p10=CharIndex('|',@Data,@p9+1)  
Set @p11=CharIndex('|',@Data,@p10+1)  
Set @p12=Len(@Data)+1  
  
Set @NotaId=Convert(numeric(38),SUBSTRING(@Data,1,@p1-1))  
Set @IdProducto=Convert(numeric(20),SUBSTRING(@Data,@p1+1,@p2-(@p1+1)))  
Set @Cantidad= Convert(decimal(18,2),SUBSTRING(@Data,@p2+1,@p3-(@p2+1)))  
Set @DetalleUm=SUBSTRING(@Data,@p3+1,@p4-(@p3+1))  
Set @Descripcion=SUBSTRING(@Data,@p4+1,@p5-(@p4+1))  
Set @Costo=Convert(decimal(18,2),SUBSTRING(@Data,@p5+1,@p6-(@p5+1)))  
Set @Precio= Convert(decimal(18,2),SUBSTRING(@Data,@p6+1,@p7-(@p6+1)))  
Set @Importe= Convert(decimal(18,2),SUBSTRING(@Data,@p7+1,@p8-(@p7+1)))  
Set @Estado=SUBSTRING(@Data,@p8+1,@p9-(@p8+1))  
Set @CantidadSaldo=Convert(decimal(18,2),SUBSTRING(@Data,@p9+1,@p10-(@p9+1)))  
Set @ValorUM=Convert(decimal(18,4),SUBSTRING(@Data,@p10+1,@p11-(@p10+1)))  
Set @DocuId=Convert(numeric(38),SUBSTRING(@Data,@p11+1,@p12-(@p11+1)))  
  
insert into DetallePedido values(@NotaId,@IdProducto,@Cantidad,  
@DetalleUm,@Descripcion,@Costo, @Precio,  
@Importe,@Estado,@CantidadSaldo,@ValorUM)  
set @DetalleNotaId=(select @@IDENTITY)  
  
if(@DocuId<>'0')  
begin  

insert into DetalleDocumento values  
(@DocuId,@IdProducto,@Cantidad,@Precio,@Importe,  
@DetalleNotaId,@DetalleUm,@ValorUM,@Descripcion)  

end  
  
select 'true'  
  
end
GO
/****** Object:  StoredProcedure [dbo].[insertarGeneral]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[insertarGeneral]
@Data varchar(max)
as
begin
Declare @p1 int,@p2 int,
		@p3 int,@p4 int,
		@p5 int
Declare
		@IdGeneral numeric(38),
		@Usuario varchar(80),
		@Ingresos decimal(18,2),
		@Salidas decimal(18,2),
		@Total decimal(18,2),
		@Codigo numeric(38)
Set @Data = LTRIM(RTrim(@Data))
Set @p1 = CharIndex('|',@Data,0)
Set @p2 = CharIndex('|',@Data,@p1+1)
Set @p3 = CharIndex('|',@Data,@p2+1)
Set @p4 = CharIndex('|',@Data,@p3+1)
Set @p5 =Len(@Data)+1
set @IdGeneral=convert(numeric(38),SUBSTRING(@Data,1,@p1-1))
set @Usuario=SUBSTRING(@Data,@p1+1,@p2-@p1-1)
set @Ingresos=convert(decimal(18,2),SUBSTRING(@Data,@p2+1,@p3-@p2-1))
set @Salidas=convert(decimal(18,2),SUBSTRING(@Data,@p3+1,@p4-@p3-1))
set @Total=convert(decimal(18,2),SUBSTRING(@Data,@p4+1,@p5-@p4-1))
if(@IdGeneral=0)
begin
insert into CajaGeneral values(GETDATE(),@Usuario,@Ingresos,@Salidas,@Total)
set @Codigo=(select @@IDENTITY)
update CajaPincipal
set IdGeneral=@Codigo
where IdGeneral=0
end
else
begin
update CajaGeneral
set FechaCierre=GETDATE(),Ingresos=@Ingresos,Salidas=@Salidas,Total=@Total,Usuario=@Usuario
where IdGeneral=@IdGeneral
end
begin
select isnull((select STUFF((select '¬'+ convert(varchar,c.IdCaja)+'|'+c.CajaConcepto+'|'+convert(varchar,c.CajaId)+'|'+
Convert(char(10),c.CajaFecha,103)+' '+Convert(char(8),c.CajaFecha,114) 
+'|'+c.CajaDescripcion+'|'+CONVERT(VarChar(50),cast(c.CajaMonto as money), 1)+'|'+
c.CajaUsuario+'|'+c.Referencia+'|'+c.GastoId
from CajaPincipal c 
where c.CajaConcepto='INGRESO' and c.IdGeneral=0
order by c.IdCaja desc
for xml path('')),1,1,'')),'~')+'['+
isnull((select STUFF((select '¬'+ convert(varchar,c.IdCaja)+'|'+c.CajaConcepto+'|'+convert(varchar,c.CajaId)+'|'+
Convert(char(10),c.CajaFecha,103)+' '+Convert(char(8),c.CajaFecha,114) 
+'|'+c.CajaDescripcion+'|'+CONVERT(VarChar(50),cast(c.CajaMonto as money), 1)+'|'+
c.CajaUsuario+'|'+c.Referencia+'|'+c.GastoId
from CajaPincipal c 
where c.CajaConcepto='SALIDA' and c.IdGeneral=0
order by c.IdCaja desc
for xml path('')),1,1,'')),'~')+'['+
isnull((select STUFF ((select '¬'+ CONVERT(varchar,c.IdGeneral)+'|'+
(IsNull(convert(varchar,c.FechaCierre,103),'')+' '+ IsNull(SUBSTRING(convert(varchar,c.FechaCierre,114),1,8),''))+'|'+c.Usuario+'|'+
CONVERT(varchar(50),cast(c.Ingresos as money),1)+'|'+CONVERT(varchar(50),cast(c.Salidas as money),1)+'|'+
CONVERT(varchar(50),cast(c.Total as money),1)
from CajaGeneral c
order by c.IdGeneral desc
for xml path('')),1,1,'')),'~')
end
end
GO
/****** Object:  StoredProcedure [dbo].[insertarGR]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[insertarGR]
@GuiaId numeric(38),
@NotaId numeric(38)
as
begin
begin
insert into GuiaRelacion values(@GuiaId,@NotaId)
end
begin
update GuiaRemision
set GuiaEstado='CANJEADO'
where GuiaId=@GuiaId
end
end
GO
/****** Object:  StoredProcedure [dbo].[insertarGuia]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[insertarGuia]
@GuiaNumero varchar(60),
@GuiaMotivo varchar(80),
@GuiaRegistro datetime,
@GuiaFechaTraslado datetime,
@GuiaDestinatario varchar(250),
@GuiaRucDes varchar(60),
@GuiaAlmacen varchar(80),
@GuiaPartida varchar(max),
@GuiaLLegada varchar(max),
@GuiaTramsporte varchar(80),
@GuiaTransporteRuc varchar(20),
@GuiaChofer varchar(80),
@GuiaPlaca varchar(80),
@GuiaConstancia varchar(80),
@GuiaLicencia varchar(80),
@GuiaUsuario varchar(80),
@GuiaTotal decimal(18,2),
@GuiaConcepto varchar(40),
@ClienteId numeric(20),
@GuiaEstado varchar(60),
@GuiaTelefono varchar(80)
as
begin
insert into GuiaRemision values(@GuiaNumero,@GuiaMotivo,
@GuiaRegistro,@GuiaFechaTraslado,@GuiaDestinatario,@GuiaRucDes,@GuiaAlmacen,@GuiaPartida,
@GuiaLLegada,@GuiaTramsporte,@GuiaTransporteRuc,@GuiaChofer,@GuiaPlaca,@GuiaConstancia,
@GuiaLicencia,@GuiaUsuario,@GuiaTotal,@GuiaConcepto,@ClienteId,@GuiaEstado,@GuiaTelefono
)													
select @@identity
end
GO
/****** Object:  StoredProcedure [dbo].[insertarKardexB]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[insertarKardexB]
	 @IdProducto numeric(20),
	 @KardexMotivo  varchar(60),
	 @KardexDocumento varchar(60),
	 @CantidadIngreso decimal(18, 2),
	 @CantidadSalida decimal(18, 2),
	 @PrecioCosto decimal(18,4),
	 @Usuario varchar(60),
	 @Aviso char(1)
	as
	begin
	begin
	declare @IniciaStock decimal(18,2),@StockFinal decimal(18,2),@Concepto varchar(40)
	set @IniciaStock=(select top 1 ProductoCantidad from Producto where IdProducto=@IdProducto)
	if @Aviso='S'
	begin
	set @StockFinal=@IniciaStock-@CantidadSalida
	set @concepto='SALIDA'
	end
	else if @Aviso='I'
	begin
	set @StockFinal=@IniciaStock+@CantidadIngreso
	set @concepto='INGRESO'
	end
	else
	begin
	set @StockFinal=@IniciaStock
	set @concepto='INGRESO'
	end
	insert into Kardex values(@IdProducto,GETDATE(),@KardexMotivo,@KardexDocumento,@IniciaStock,
	@CantidadIngreso,@CantidadSalida,@PrecioCosto,@StockFinal,@Concepto,@Usuario)
	end
	begin
	if @Aviso='S'
	begin
	update producto 
	set  ProductoCantidad =ProductoCantidad - @CantidadSalida
	where IDProducto=@IdProducto
	end
	else if @Aviso='I'
	begin
	update producto
	set ProductoCantidad =ProductoCantidad + @CantidadIngreso
	where IDProducto=@IdProducto
	end
	end
	end
GO
/****** Object:  StoredProcedure [dbo].[insertarLetra]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[insertarLetra]
@ProveedorId numeric(38),
@LetraFechaReg datetime,
@LetraFechaGiro date,
@LetraMoneda varchar(40),
@LetraSaldo decimal(18,2),
@LetraTotal decimal(18,2),
@letraUsuario varchar(60),
@LetraEstado varchar(60),
@CompaniaId INT 
as
begin
insert into Letra values(@ProveedorId,@LetraFechaReg,@LetraFechaGiro,
@LetraMoneda,@LetraSaldo,@LetraTotal,@letraUsuario,@LetraEstado,@CompaniaId)
select @@identity
end
GO
/****** Object:  StoredProcedure [dbo].[insertarLiquida]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[insertarLiquida]
@ListaOrden varchar(Max)      
as      
begin      
Declare @pos1 int,@pos2 int      
Declare @orden varchar(max),      
        @detalle varchar(max)      
Set @pos1 = CharIndex('[',@ListaOrden,0)      
Set @pos2 =Len(@ListaOrden)+1      
Set @orden = SUBSTRING(@ListaOrden,1,@pos1-1)      
Set @detalle = SUBSTRING(@ListaOrden,@pos1+1,@pos2-@pos1-1)      
Declare @c1 int,@c2 int,@c3 int,@c4 int,      
        @c5 int,@c6 int,@c7 int,@c8 int,      
        @c9 int,@c10 int,@c11 int      
Declare      
@LiquidacionNumero varchar(80),      
@LiquidacionFecha date,      
@LiquidacionDescripcion varchar(250),      
@LiquidacionCambio decimal(18,3),      
@LiquidaEfectivoSol decimal(18,2),      
@LiquidaDepositoSol decimal(18,2),      
@LiquidaTotalSol decimal(18,2),      
@LiquidaEfectivoDol decimal(18,2),      
@LiquidaDepositoDol decimal(18,2),      
@LiquidaTotalDol decimal(18,2),      
@LiquidaUsuario varchar(60)  
     
Set @c1 = CharIndex('|',@orden,0)      
Set @c2 = CharIndex('|',@orden,@c1+1)      
Set @c3 = CharIndex('|',@orden,@c2+1)      
Set @c4 = CharIndex('|',@orden,@c3+1)      
Set @c5 = CharIndex('|',@orden,@c4+1)      
Set @c6= CharIndex('|',@orden,@c5+1)      
Set @c7 = CharIndex('|',@orden,@c6+1)      
Set @c8 = CharIndex('|',@orden,@c7+1)      
Set @c9 = CharIndex('|',@orden,@c8+1)      
Set @c10= CharIndex('|',@orden,@c9+1)      
Set @c11= Len(@orden)+1       
  
set @LiquidacionNumero=SUBSTRING(@orden,1,@c1-1)      
set @LiquidacionFecha=convert(date,SUBSTRING(@orden,@c1+1,@c2-@c1-1))      
set @LiquidacionDescripcion=SUBSTRING(@orden,@c2+1,@c3-@c2-1)      
set @LiquidacionCambio=Convert(decimal(18,3),SUBSTRING(@orden,@c3+1,@c4-@c3-1))      
set @LiquidaEfectivoSol=convert(decimal(18,2),SUBSTRING(@orden,@c4+1,@c5-@c4-1))      
set @LiquidaDepositoSol=convert(decimal(18,2),SUBSTRING(@orden,@c5+1,@c6-@c5-1))      
set @LiquidaTotalSol=convert(decimal(18,2),SUBSTRING(@orden,@c6+1,@c7-@c6-1))      
set @LiquidaEfectivoDol=convert(decimal(18,2),SUBSTRING(@orden,@c7+1,@c8-@c7-1))      
set @LiquidaDepositoDol=convert(decimal(18,2),SUBSTRING(@orden,@c8+1,@c9-@c8-1))      
set @LiquidaTotalDol=convert(decimal(18,2),SUBSTRING(@orden,@c9+1,@c10-@c9-1))      
set @LiquidaUsuario=SUBSTRING(@orden,@c10+1,@c11-@c10-1)      
Declare @LiquidacionId numeric(38)  
  
Declare @CajaIdB numeric(38)  
set @CajaIdB=(select isnull((select stuff((select '¬'+ convert(varchar,c.CajaId)  
from Caja c where c.CajaEstado='ACTIVO' order by c.CajaId desc for xml path('')),1,1,'')),'0'))  
  
if(@CajaIdB='0')  
begin  
select 'existe'  
end  
else  
begin
Begin Transaction
 
declare @cod varchar(13)              
SET @cod=ISNULL((select TOP 1 dbo.geneneraIdLiquida('0001') AS ID FROM Liquidacion),'0001-00000001')  
     
insert into Liquidacion values(@cod,  
Getdate(),@LiquidacionFecha,@LiquidacionDescripcion,  
@LiquidacionCambio,@LiquidaEfectivoSol,@LiquidaDepositoSol,  
@LiquidaTotalSol,@LiquidaEfectivoDol,@LiquidaDepositoDol,  
@LiquidaTotalDol,@LiquidaUsuario)  
set @LiquidacionId=(select @@identity)


Declare Tabla Cursor For Select * From fnSplitString(@detalle,';')       
Open Tabla      
Declare @Columna varchar(max),

		@CompraId numeric(38),  
		@SaldoDocu decimal(18,2),  
		@EfectivoSoles decimal(18,2),  
		@EfectivoDolar decimal(18,2),  
		@DepositoSoles decimal(18,2),  
		@DepositoDolar decimal(18,2),  
		@TipoCambio decimal(18,3),  
		@EntidadBanco varchar(80),  
		@NroOperacion varchar(80),  
		@AcuentaGeneral decimal(18,2),  
		@SaldoActual decimal(18,2),  
		@FechaPago varchar(60),  
		@Numero varchar(60),  
		@Proveedor varchar(255),  
		@Moneda varchar(20),  
		@Concepto varchar(40),  
		@CompraEstado varchar(60),
		
		@DetalleId numeric(38) 
		      
Declare @p1 int,@p2 int,@p3 int,@p4 int,      
        @p5 int,@p6 int,@p7 int,@p8 int,      
        @p9 int,@p10 int,@p11 int,      
        @p12 int,@p13 int,@p14 int,@p15 int,
        @p16 int,@p17 int
        
Fetch Next From Tabla INTO @Columna      
 While @@FETCH_STATUS = 0      
 Begin      Set @p1 = CharIndex('|',@Columna,0)      
Set @p2 = CharIndex('|',@Columna,@p1+1)      
Set @p3 = CharIndex('|',@Columna,@p2+1)      
Set @p4 = CharIndex('|',@Columna,@p3+1)      
Set @p5 = CharIndex('|',@Columna,@p4+1)      
Set @p6= CharIndex('|',@Columna,@p5+1)      
Set @p7= CharIndex('|',@Columna,@p6+1)      
Set @p8 = CharIndex('|',@Columna,@p7+1)      
Set @p9 = CharIndex('|',@Columna,@p8+1)      
Set @p10 = CharIndex('|',@Columna,@p9+1)      
Set @p11= CharIndex('|',@Columna,@p10+1)      
Set @p12= CharIndex('|',@Columna,@p11+1)      
Set @p13= CharIndex('|',@Columna,@p12+1)
Set @p14= CharIndex('|',@Columna,@p13+1)      
Set @p15= CharIndex('|',@Columna,@p14+1)      
Set @p16= CharIndex('|',@Columna,@p15+1)      
Set @p17=Len(@Columna)+1
      
set @CompraId=Convert(numeric(38),SUBSTRING(@Columna,1,@p1-1))      
Set @SaldoDocu=Convert(decimal(18,2),SUBSTRING(@Columna,@p1+1,@p2-(@p1+1)))      
Set @EfectivoSoles=convert(decimal(18,2),SUBSTRING(@Columna,@p2+1,@p3-(@p2+1)))      
Set @EfectivoDolar=convert(decimal(18,2),SUBSTRING(@Columna,@p3+1,@p4-(@p3+1)))      
Set @DepositoSoles=convert(decimal(18,2),SUBSTRING(@Columna,@p4+1,@p5-(@p4+1)))      
Set @DepositoDolar=convert(decimal(18,2),SUBSTRING(@Columna,@p5+1,@p6-(@p5+1)))
Set @TipoCambio=convert(decimal(18,3),SUBSTRING(@Columna,@p6+1,@p7-(@p6+1)))      
Set @EntidadBanco=SUBSTRING(@Columna,@p7+1,@p8-(@p7+1))      
Set @NroOperacion=SUBSTRING(@Columna,@p8+1,@p9-(@p8+1))      
Set @AcuentaGeneral=convert(decimal(18,2),SUBSTRING(@Columna,@p9+1,@p10-(@p9+1)))      
Set @SaldoActual=Convert(decimal(18,2),SUBSTRING(@Columna,@p10+1,@p11-(@p10+1)))
Set @FechaPago=SUBSTRING(@Columna,@p11+1,@p12-(@p11+1))
Set @Numero=SUBSTRING(@Columna,@p12+1,@p13-(@p12+1))
Set @Proveedor=SUBSTRING(@Columna,@p13+1,@p14-(@p13+1))      
Set @Moneda=SUBSTRING(@Columna,@p14+1,@p15-(@p14+1))
Set @Concepto=SUBSTRING(@Columna,@p15+1,@p16-(@p15+1))
Set @CompraEstado=SUBSTRING(@Columna,@p16+1,@p17-(@p16+1))
  

insert into DetalleLiquida values(  
@LiquidacionId,@CompraId,@SaldoDocu,@EfectivoSoles,@EfectivoDolar,@DepositoSoles,  
@DepositoDolar,@TipoCambio,@EntidadBanco,@NroOperacion,@AcuentaGeneral,@SaldoActual,@FechaPago,@Numero,  
@Proveedor,@Moneda,@Concepto)
set @DetalleId=(select @@IDENTITY)

if(@Concepto='COMPRA')  
begin  
	update Compras  
	set CompraSaldo=CompraSaldo - @AcuentaGeneral,CompraEstado=@CompraEstado  
	where CompraId=@CompraId  
END  
else  
begin  
	update DetalleLetra  
	set DetalleSaldo=DetalleSaldo-@AcuentaGeneral,DetalleEstado=@CompraEstado  
	where DetalleId=@CompraId  
end
      
if(@DepositoSoles>0)      
begin      
	insert into CajaDetalle values(@CajaIdB,GETDATE(),'0','DEPOSITO','LIQUIDACION',      
	'LIQUIDACION AL PROVEEDOR '+@Proveedor+' DOC NRO '+@Numero,@DepositoSoles,      
	@DepositoSoles,0,'','T','',@LiquidaUsuario,'','LP-'+CONVERT(varchar,@DetalleId))      
end      
if(@EfectivoSoles>0)      
BEGIN      
	insert into CajaDetalle values(@CajaIdB,GETDATE(),'0','SALIDA','LIQUIDACION',      
	'LIQUIDACION AL PROVEEDOR '+@Proveedor+' DOC NRO '+@Numero,@EfectivoSoles,      
	@EfectivoSoles,0,'','T','',@LiquidaUsuario,'','LP-'+CONVERT(varchar,@DetalleId))      
END      

Fetch Next From Tabla INTO @Columna      
end      
 Close Tabla;      
 Deallocate Tabla;      
 Commit Transaction;      
 SELECT 'true'      
end
END
GO
/****** Object:  StoredProcedure [dbo].[insertarRenta]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[insertarRenta] 
@Data varchar(max)
as
declare @existe int
		Declare @pos1 int,@pos2 int,@pos3 int,@pos4 int,
		@pos5 int,@pos6 int,@pos7 int,@pos8 int,@pos9 int,
		@pos10 int,@pos11 int,@pos12 int,@pos13 int,@pos14 int,
		@pos15 int,@pos16 int,@pos17 int,@pos18 int
Declare @RentaId numeric(38),@CompaniaId int,@RentaUsuario varchar(80),
		@RentaANNO int,@RentaMes int,@IGV decimal(18,2),@Renta decimal(18,2),
		@SaldoIGV decimal(18,2),@SaldoRenta decimal(18,2),@InteresIgv decimal(18,2),
		@InteresRenta decimal(18,2),@TributoIgv decimal(18,2),@TributoRenta decimal(18,2),
		@FormaPago bit,@FechaCancelacion datetime,@EntidadBancaria varchar(80),
		@NroOperacion varchar(80),@PagoTotal decimal(18,2)
Set @Data = LTRIM(RTrim(@Data))
Set @pos1 = CharIndex('|',@Data,0)
Set @RentaId=convert(numeric(38),SUBSTRING(@Data,1,@pos1-1))
Set @pos2 = CharIndex('|',@Data,@pos1+1)
Set @CompaniaId= convert(int,SUBSTRING(@Data,@pos1+1,@pos2-@pos1-1))
Set @pos3 = CharIndex('|',@Data,@pos2+1)
Set @RentaUsuario=SUBSTRING(@Data,@pos2+1,@pos3-@pos2-1)
Set @pos4 = CharIndex('|',@Data,@pos3+1)
Set @RentaANNO=convert(int,SUBSTRING(@Data,@pos3+1,@pos4-@pos3-1))
Set @pos5 = CharIndex('|',@Data,@pos4+1)
Set @RentaMes=convert(int,SUBSTRING(@Data,@pos4+1,@pos5-@pos4-1))
Set @pos6 =CharIndex('|',@Data,@pos5+1)
Set @IGV=convert(decimal(18,2),SUBSTRING(@Data,@pos5+1,@pos6-@pos5-1))
Set @pos7 =CharIndex('|',@Data,@pos6+1)
Set @Renta=convert(decimal(18,2),SUBSTRING(@Data,@pos6+1,@pos7-@pos6-1))
Set @pos8 =CharIndex('|',@Data,@pos7+1)
Set @SaldoIGV=convert(decimal(18,2),SUBSTRING(@Data,@pos7+1,@pos8-@pos7-1))
Set @pos9 =CharIndex('|',@Data,@pos8+1)
Set @SaldoRenta=convert(decimal(18,2),SUBSTRING(@Data,@pos8+1,@pos9-@pos8-1))
Set @pos10=CharIndex('|',@Data,@pos9+1)
Set @InteresIgv=convert(decimal(18,2),SUBSTRING(@Data,@pos9+1,@pos10-@pos9-1))
Set @pos11=CharIndex('|',@Data,@pos10+1)
Set @InteresRenta=convert(decimal(18,2),SUBSTRING(@Data,@pos10+1,@pos11-@pos10-1))
Set @pos12=CharIndex('|',@Data,@pos11+1)
Set @TributoIgv=convert(decimal(18,2),SUBSTRING(@Data,@pos11+1,@pos12-@pos11-1))
Set @pos13=CharIndex('|',@Data,@pos12+1)
Set @TributoRenta=convert(decimal(18,2),SUBSTRING(@Data,@pos12+1,@pos13-@pos12-1))
Set @pos14=CharIndex('|',@Data,@pos13+1)
Set @FormaPago=convert(bit,SUBSTRING(@Data,@pos13+1,@pos14-@pos13-1))
Set @pos15=CharIndex('|',@Data,@pos14+1)
Set @FechaCancelacion=convert(date,SUBSTRING(@Data,@pos14+1,@pos15-@pos14-1))
Set @pos16=CharIndex('|',@Data,@pos15+1)
Set @EntidadBancaria=SUBSTRING(@Data,@pos15+1,@pos16-@pos15-1)
Set @pos17=CharIndex('|',@Data,@pos16+1)
Set @NroOperacion=SUBSTRING(@Data,@pos16+1,@pos17-@pos16-1)
Set @pos18= Len(@Data)+1
Set @PagoTotal=convert(decimal(18,2),SUBSTRING(@Data,@pos17+1,@pos18-@pos17-1))
set @existe=(select count(RentaId)as Codigo from RentaMensual
             where CompaniaId=@CompaniaId and(RentaANNO=@RentaANNO and RentaMes=@RentaMes))
begin
if @RentaId=0
begin  
if @existe=0
begin
insert into RentaMensual values
(@CompaniaId,@RentaUsuario,
@RentaANNO,@RentaMes,@IGV,@Renta,@SaldoIGV,
@SaldoRenta,@InteresIgv,@InteresRenta,@TributoIgv,@TributoRenta,@FormaPago,@FechaCancelacion,
@EntidadBancaria,@NroOperacion,@PagoTotal
)
(select STUFF((select '¬'+convert(varchar,r.RentaId)+'|'+convert(varchar,r.CompaniaId)+'|'+convert(varchar,r.RentaANNO)+'|'+
convert(varchar,r.RentaMes)+'|'+dbo.MesNombre(r.RentaMes)+' '+convert(varchar,r.RentaANNO)+'|'+
CONVERT(VarChar(50), cast((r.IGV) as money ), 1)+'|'+CONVERT(VarChar(50), cast((r.Renta) as money ), 1)+'|'+
CONVERT(VarChar(50), cast((r.SaldoIGV) as money ), 1)+'|'+CONVERT(VarChar(50), cast((r.SaldoRenta) as money ), 1)+'|'+
CONVERT(VarChar(50), cast((r.InteresIgv) as money ), 1)+'|'+CONVERT(VarChar(50), cast((r.InteresRenta) as money ), 1)+'|'+
CONVERT(VarChar(50), cast((r.TributoIgv) as money ), 1)+'|'+CONVERT(VarChar(50), cast((r.TributoRenta) as money ), 1)+'|'+
CONVERT(char(1),r.FormaPago)+'|'+convert(varchar,r.FechaCancelacion,103)+'|'+r.EntidadBancaria+'|'+r.NroOperacion+'|'+
CONVERT(VarChar(50), cast((r.PagoTotal) as money ), 1)
from RentaMensual r
where year(r.FechaCancelacion)=year(getdate())
order by r.RentaId desc
for xml path('')),1,1,''))
end
else
begin
select 'existe'
end
end
else
begin
update RentaMensual
set IGV=@IGV,Renta=@Renta,SaldoIGV=@SaldoIGV,SaldoRenta=@SaldoRenta,InteresIgv=@InteresIgv,
InteresRenta=@InteresRenta,TributoIgv=@TributoIgv,TributoRenta=@TributoRenta,FormaPago=@FormaPago,
FechaCancelacion=@FechaCancelacion,EntidadBancaria=@EntidadBancaria,NroOperacion=@NroOperacion,PagoTotal=@PagoTotal
where RentaId=@RentaId
(select STUFF((select '¬'+convert(varchar,r.RentaId)+'|'+convert(varchar,r.CompaniaId)+'|'+convert(varchar,r.RentaANNO)+'|'+
convert(varchar,r.RentaMes)+'|'+dbo.MesNombre(r.RentaMes)+' '+convert(varchar,r.RentaANNO)+'|'+
CONVERT(VarChar(50), cast((r.IGV) as money ), 1)+'|'+CONVERT(VarChar(50), cast((r.Renta) as money ), 1)+'|'+
CONVERT(VarChar(50), cast((r.SaldoIGV) as money ), 1)+'|'+CONVERT(VarChar(50), cast((r.SaldoRenta) as money ), 1)+'|'+
CONVERT(VarChar(50), cast((r.InteresIgv) as money ), 1)+'|'+CONVERT(VarChar(50), cast((r.InteresRenta) as money ), 1)+'|'+
CONVERT(VarChar(50), cast((r.TributoIgv) as money ), 1)+'|'+CONVERT(VarChar(50), cast((r.TributoRenta) as money ), 1)+'|'+
CONVERT(char(1),r.FormaPago)+'|'+convert(varchar,r.FechaCancelacion,103)+'|'+r.EntidadBancaria+'|'+r.NroOperacion+'|'+
CONVERT(VarChar(50), cast((r.PagoTotal) as money ), 1)
from RentaMensual r
where year(r.FechaCancelacion)=year(getdate())
order by r.RentaId desc
for xml path('')),1,1,''))
end
end
GO
/****** Object:  StoredProcedure [dbo].[insertartemLetra]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[insertartemLetra]
@CompraId numeric(38),
@ProveedorId numeric(38),
@TemporalDocumento varchar(60),
@TemporalMoneda varchar(20),
@TemporalMonto decimal(18,2),
@UsuarioId int,
@TemporalCanje varchar(80)
as
begin
insert into temporalLetra values(@CompraId,@ProveedorId,@TemporalDocumento,@TemporalMoneda,
@TemporalMonto,@UsuarioId,@TemporalCanje)
end
GO
/****** Object:  StoredProcedure [dbo].[insertarTempCompra]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[insertarTempCompra]
@UsuarioID int,
@IdProducto numeric(20),
@DetalleCodigo varchar(80),
@Descripcion varchar(255),
@DetalleUM   varchar(60),
@DetalleCantidad decimal(18,2),
@PrecioCosto  decimal(18,4),
@DetalleImporte decimal(18,2),
@DetalleDescuento decimal(18,4),
@DetalleEstado varchar(40),
@Posicion int
as
begin
insert into TemporalCompra values(@UsuarioID,@IdProducto,@DetalleCodigo,
@Descripcion,@DetalleUM,@DetalleCantidad,@PrecioCosto,@DetalleImporte,
@DetalleDescuento,@DetalleEstado,1,@Posicion)
end
GO
/****** Object:  StoredProcedure [dbo].[insertarTempCompraB]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[insertarTempCompraB]  
@Data varchar(max)  
as  
begin  
Declare @p1 int,@p2 int,  
        @p3 int,@p4 int,  
        @p5 int,@p6 int,  
        @p7 int,@p8 int,  
        @p9 int,@p10 int,  
        @p11 int,@p12 int  
Declare @UsuarioID int,@IdProducto numeric(20),  
  @DetalleCodigo varchar(80),@Descripcion varchar(255),  
  @DetalleUM varchar(60),@DetalleCantidad decimal(18,2),  
  @PrecioCosto  decimal(18,4),@DetalleImporte decimal(18,2),  
  @DetalleDescuento decimal(18,4),@DetalleEstado varchar(40),  
  @ValorUM decimal(18,4),@Posicion int  
Set @Data = LTRIM(RTrim(@Data))  
Set @p1 = CharIndex('|',@Data,0)  
Set @p2 = CharIndex('|',@Data,@p1+1)  
Set @p3= CharIndex('|',@Data,@p2+1)  
Set @p4= CharIndex('|',@Data,@p3+1)  
Set @p5= CharIndex('|',@Data,@p4+1)  
Set @p6= CharIndex('|',@Data,@p5+1)  
Set @p7= CharIndex('|',@Data,@p6+1)  
Set @p8= CharIndex('|',@Data,@p7+1)  
Set @p9= CharIndex('|',@Data,@p8+1)  
Set @p10= CharIndex('|',@Data,@p9+1)  
Set @p11= CharIndex('|',@Data,@p10+1)  
Set @p12= Len(@Data)+1  
Set @UsuarioID =convert(int,SUBSTRING(@Data,1,@p1-1))  
Set @IdProducto=convert(numeric(20),SUBSTRING(@Data,@p1+1,@p2-@p1-1))  
Set @DetalleCodigo=SUBSTRING(@Data,@p2+1,@p3-@p2-1)  
Set @Descripcion=SUBSTRING(@Data,@p3+1,@p4-@p3-1)  
Set @DetalleUM=SUBSTRING(@Data,@p4+1,@p5-@p4-1)  
Set @DetalleCantidad=convert(decimal(18,2),SUBSTRING(@Data,@p5+1,@p6-@p5-1))  
Set @PrecioCosto=convert(decimal(18,4),SUBSTRING(@Data,@p6+1,@p7-@p6-1))  
Set @DetalleImporte=convert(decimal(18,2),SUBSTRING(@Data,@p7+1,@p8-@p7-1))  
Set @DetalleDescuento=convert(decimal(18,4),SUBSTRING(@Data,@p8+1,@p9-@p8-1))  
Set @DetalleEstado=SUBSTRING(@Data,@p9+1,@p10-@p9-1)  
Set @ValorUM=convert(decimal(18,4),SUBSTRING(@Data,@p10+1,@p11-@p10-1))  
Set @Posicion=convert(int,SUBSTRING(@Data,@p11+1,@p12-@p11-1))  
insert into TemporalCompra values(@UsuarioID,@IdProducto,@DetalleCodigo,  
@Descripcion,@DetalleUM,@DetalleCantidad,@PrecioCosto,@DetalleImporte,  
@DetalleDescuento,@DetalleEstado,@ValorUM,@Posicion)  
select  
isnull((select STUFF ((select '¬'+convert(varchar,t.TemporalId)+'|'+convert(varchar,t.IdProducto)+'|'+  
t.DetalleCodigo+'|'+t.Descripcion+'|'+t.DetalleUM+'|'+  
CONVERT(VarChar(50),cast(t.DetalleCantidad as money ), 1)+'|'+  
convert(varchar,t.PrecioCosto)+'|'+convert(varchar,t.DetalleDescuento)+'|'+
convert(varchar,t.DetalleImporte)+'|'+CONVERT(varchar,t.ValorUM)+'|'+  
t.DetalleEstado+'|'+p.AplicaINV
from TemporalCompra t   
inner join Producto p   
on p.IdProducto=t.IdProducto   
where t.UsuarioID=@UsuarioID  
order by t.Posicion asc  
for xml path('')),1,1,'')),'~')+'['+  
isnull((select STUFF ((select '¬'+convert(varchar,u.IdUm)+'|'+convert(varchar,u.IdProducto)+'|'+  
u.UMDescripcion+'|'+CONVERT(VarChar(50), cast(u.ValorUM as money ), 1)+'|'+  
convert(varchar,t.PrecioCosto)  
from UnidadMedida u  
inner join TemporalCompra t  
on t.IdProducto=u.IdProducto  
where t.UsuarioID=@UsuarioID  
order by u.ValorUM asc  
for xml path('')),1,1,'')),'~')  
end
GO
/****** Object:  StoredProcedure [dbo].[insertarTempoGuia]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[insertarTempoGuia]
@UsuarioID int,
@IdProducto numeric(20),
@cantidad decimal(18,2),
@precioventa decimal(18,2),
@importe decimal(18,2),
@Concepto varchar(60),
@CantidadSaldo decimal(18,2),
@ClienteId numeric(20),
@DetalleId numeric(38),
@DetalleUM varchar(40),
@ValorUM decimal(18,4)
as
begin
insert into TemporalGuia values(@UsuarioID,@IdProducto,@cantidad,
@precioventa,@importe,@Concepto,@CantidadSaldo,@ClienteId,@DetalleId,@DetalleUM,@ValorUM)
end
GO
/****** Object:  StoredProcedure [dbo].[insertarTempoVenta]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[insertarTempoVenta]  
@Data varchar(max)  
as  
begin  
Declare @pos1 int,@pos2 int,  
  @pos3 int,@pos4 int,  
  @pos5 int,@pos6 int,  
  @pos7 int,@pos8 int  
Declare @UsuarioID int,  
  @IdProducto numeric(20),  
  @cantidad decimal(18,2),  
  @precioventa decimal(18,2),  
  @importe decimal(18,2),  
  @ValorUM decimal(18,4),  
  @Unidad varchar(40),
  @Descripcion varchar(max)   
Set @Data = LTRIM(RTrim(@Data))  
Set @pos1 = CharIndex('|',@Data,0)  
Set @pos2 = CharIndex('|',@Data,@pos1+1)  
Set @pos3 = CharIndex('|',@Data,@pos2+1)  
Set @pos4 = CharIndex('|',@Data,@pos3+1)  
Set @pos5= CharIndex('|',@Data,@pos4+1)  
Set @pos6= CharIndex('|',@Data,@pos5+1)
Set @pos7= CharIndex('|',@Data,@pos6+1)   
Set @pos8=Len(@Data)+1  
Set @UsuarioID=convert(int,SUBSTRING(@Data,1,@pos1-1))  
Set @IdProducto=convert(numeric(20),SUBSTRING(@Data,@pos1+1,@pos2-@pos1-1))  
Set @cantidad=convert(decimal(18,2),SUBSTRING(@Data,@pos2+1,@pos3-@pos2-1))  
Set @precioventa=convert(decimal(18,2),SUBSTRING(@Data,@pos3+1,@pos4-@pos3-1))  
Set @importe=convert(decimal(18,2),SUBSTRING(@Data,@pos4+1,@pos5-@pos4-1))  
Set @ValorUM=convert(decimal(18,4),SUBSTRING(@Data,@pos5+1,@pos6-@pos5-1))  
Set @Unidad=SUBSTRING(@Data,@pos6+1,@pos7-@pos6-1)
Set @Descripcion=SUBSTRING(@Data,@pos7+1,@pos8-@pos7-1) 
insert into TemporalVenta values(@UsuarioID,@IdProducto,@cantidad,@precioventa,@importe,  
@ValorUM,@Unidad,@Descripcion)  
select 'true'  
end
GO
/****** Object:  StoredProcedure [dbo].[insertarTemUMGuia]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[insertarTemUMGuia]
@Data varchar(max)
as
	Declare @pos1 int
	Declare @pos2 int
	Declare @pos3 int
	Declare @pos4 int
	Declare @pos5 int
	Declare @pos6 int
	Declare @pos7 int
	Declare @pos8 int
Declare 
@UsuarioID int,
@IdProducto numeric(20),
@cantidad decimal(18,2),
@precioventa decimal(18,2),
@importe decimal(18,2),
@Concepto varchar(60),
@DetalleUM varchar(40),
@ValorUM decimal(18,4)
Set @Data = LTRIM(RTrim(@Data))
Set @pos1 = CharIndex('|',@Data,0)
Set @UsuarioID=convert(int,SUBSTRING(@Data,1,@pos1-1))
Set @pos2 = CharIndex('|',@Data,@pos1+1)
Set @IdProducto=convert(numeric(20),SUBSTRING(@Data,@pos1+1,@pos2-@pos1-1))
Set @pos3 = CharIndex('|',@Data,@pos2+1)
Set @cantidad=convert(decimal(18,2),SUBSTRING(@Data,@pos2+1,@pos3-@pos2-1))
Set @pos4 = CharIndex('|',@Data,@pos3+1)
Set @precioventa=convert(decimal(18,2),SUBSTRING(@Data,@pos3+1,@pos4-@pos3-1))
Set @pos5 = CharIndex('|',@Data,@pos4+1)
Set @importe=convert(decimal(18,2),SUBSTRING(@Data,@pos4+1,@pos5-@pos4-1))
Set @pos6 =CharIndex('|',@Data,@pos5+1)
Set @Concepto=SUBSTRING(@Data,@pos5+1,@pos6-@pos5-1)
Set @pos7=CharIndex('|',@Data,@pos6+1)
Set @DetalleUM=SUBSTRING(@Data,@pos6+1,@pos7-@pos6-1)
Set @pos8= Len(@Data)+1
Set @ValorUM=convert(decimal(18,4),SUBSTRING(@Data,@pos7+1,@pos8-@pos7-1))
IF EXISTS(select t.DetalleUM from TemporalGuia t where (t.IdProducto=@IdProducto and t.DetalleUM=@DetalleUM)and t.UsuarioID=@UsuarioID)
begin
select 'UM'
end
else
begin
insert into TemporalGuia values(@UsuarioID,@IdProducto,@cantidad,
@precioventa,@importe,@Concepto,0,0,0,@DetalleUM,@ValorUM)
select 'true'
end
GO
/****** Object:  StoredProcedure [dbo].[InsertarUM]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[InsertarUM]
@Data varchar(max)
as
begin
Declare @pos1 int
Declare @pos2 int
Declare @pos3 int
Declare @pos4 int
Declare @pos5 int
Declare @pos6 int
Declare @pos7 int
declare @IdUm int,
@IdProducto numeric(20),
@UMDescripcion varchar(80),
@ValorUM decimal(18,4),
@PrecioVenta decimal(18,2),
@PrecioVentaB decimal(18,2),
@PrecioCosto decimal(18,4)
Set @Data = LTRIM(RTrim(@Data))
Set @pos1 = CharIndex('|',@Data,0)
Set @IdUm =convert(int,SUBSTRING(@Data,1,@pos1-1))
Set @pos2 = CharIndex('|',@Data,@pos1+1)
Set @IdProducto=convert(numeric,SUBSTRING(@Data,@pos1+1,@pos2-@pos1-1))
Set @pos3 = CharIndex('|',@Data,@pos2+1)
Set @UMDescripcion=SUBSTRING(@Data,@pos2+1,@pos3-@pos2-1)
Set @pos4 = CharIndex('|',@Data,@pos3+1)
Set @ValorUM=convert(decimal(18,4),SUBSTRING(@Data,@pos3+1,@pos4-@pos3-1))
Set @pos5 = CharIndex('|',@Data,@pos4+1)
Set @PrecioVenta=convert(decimal(18,2),SUBSTRING(@Data,@pos4+1,@pos5-@pos4-1))
Set @pos6 =CharIndex('|',@Data,@pos5+1)
Set @PrecioVentaB=convert(decimal(18,2),SUBSTRING(@Data,@pos5+1,@pos6-@pos5-1))
Set @pos7 = Len(@Data)+1
Set @PrecioCosto=convert(decimal(18,4),SUBSTRING(@Data,@pos6+1,@pos7-@pos6-1))
declare @CostoPro decimal(18,4),@costoTotal decimal(18,4)
set @CostoPro=(select top 1 p.ProductoCosto from Producto p where p.IdProducto=@IdProducto)
set @costoTotal=@ValorUM * @CostoPro
if @IdUm=0
begin
IF EXISTS(select u.UMDescripcion from UnidadMedida u where u.IdProducto=@IdProducto and u.UMDescripcion=@UMDescripcion)
select 'UM'
else IF EXISTS(select u.ValorUM from UnidadMedida u where u.IdProducto=@IdProducto and u.ValorUM=@ValorUM)
select 'VALOR'
else
begin
insert into UnidadMedida values(@IdProducto,@UMDescripcion,@ValorUM,@PrecioVenta,@PrecioVentaB,@costoTotal)
(select STUFF ((select '¬'+convert(varchar,m.IdUm)+'|'+CONVERT(varchar,m.IdProducto)+'|'+m.UMDescripcion+'|'+
CONVERT(VarChar(50),cast(m.ValorUM as money ),2)+'|'+CONVERT(VarChar(50),cast(m.PrecioVenta as money ), 1)+'|'+CONVERT(VarChar(50), cast(m.PrecioVentaB as money ), 1)+'|'+
CONVERT(varchar(50),m.PrecioCosto)
from UnidadMedida m
where m.IdProducto=@IdProducto
order by m.ValorUM asc
for xml path('')),1,1,''))
end
end
else
begin
update UnidadMedida
set PrecioVenta=@PrecioVenta,PrecioVentaB=@PrecioVentaB,PrecioCosto=@PrecioCosto
where IdUm=@IdUm
select 'true'
end
end
GO
/****** Object:  StoredProcedure [dbo].[insertaTemLiVenta]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[insertaTemLiVenta]  
@orden varchar(max)  
as  
begin   
Declare @c1 int,@c2 int,@c3 int,@c4 int,    
        @c5 int,@c6 int   
Declare @DocuId numeric(38),@NotaId numeric(38),    
  @UsuarioId int,@SaldoDocu decimal(18,2),    
  @TipoCambio decimal(18,3),@TemporalFecha varchar(60)  
      
Set @c1 = CharIndex('|',@orden,0)    
Set @c2 = CharIndex('|',@orden,@c1+1)    
Set @c3 = CharIndex('|',@orden,@c2+1)    
Set @c4 = CharIndex('|',@orden,@c3+1)    
Set @c5 = CharIndex('|',@orden,@c4+1)     
Set @c6= Len(@orden)+1  
  
set @DocuId=Convert(numeric(38),SUBSTRING(@orden,1,@c1-1))    
set @NotaId=Convert(numeric(38),SUBSTRING(@orden,@c1+1,@c2-@c1-1))    
set @UsuarioId=SUBSTRING(@orden,@c2+1,@c3-@c2-1)    
set @SaldoDocu=Convert(decimal(18,2),SUBSTRING(@orden,@c3+1,@c4-@c3-1))    
set @TipoCambio=convert(decimal(18,3),SUBSTRING(@orden,@c4+1,@c5-@c4-1))  
set @TemporalFecha=SUBSTRING(@orden,@c5+1,@c6-@c5-1)     
     
insert into TemporalLiVenta values(@DocuId,@NotaId,@UsuarioId,@SaldoDocu,@TipoCambio,0,    
0,0,0,'','',0,@TemporalFecha)  
  
select 'true'  
  
end
GO
/****** Object:  StoredProcedure [dbo].[insertaTempoLiquida]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[insertaTempoLiquida]  
@orden varchar(max)  
as  
begin   
Declare @c1 int,@c2 int,@c3 int,@c4 int,    
        @c5 int,@c6 int,@c7 int,@c8 int,
        @c9 int,@c10 int,@c11 int,@c12 int,    
        @c13 int,@c14 int,@c15 int,@c16 int  
Declare @IdDeuda numeric(38),  
		@Numero varchar(60),  
		@Proveedor varchar(255),  
		@SaldoDocu decimal(18,2),  
		@Moneda varchar(20),  
		@TipoCambio decimal(18,3),  
		@EfectivoSoles decimal(18,2),  
		@EfectivoDolar decimal(18,2),  
		@DepositoSoles decimal(18,2),  
		@DepositoDolar decimal(18,2),  
		@EntidadBanco varchar(80),  
		@NroOperacion varchar(80),  
		@AcuentaGeneral decimal(18,2),  
		@TemporalFecha varchar(60),  
		@UsuarioId int,  
		@Concepto varchar(40)
Set @c1 = CharIndex('|',@orden,0)    
Set @c2 = CharIndex('|',@orden,@c1+1)    
Set @c3 = CharIndex('|',@orden,@c2+1)    
Set @c4 = CharIndex('|',@orden,@c3+1)    
Set @c5 = CharIndex('|',@orden,@c4+1)
Set @c6 = CharIndex('|',@orden,@c5+1)    
Set @c7 = CharIndex('|',@orden,@c6+1)    
Set @c8 = CharIndex('|',@orden,@c7+1)    
Set @c9 = CharIndex('|',@orden,@c8+1) 
Set @c10 = CharIndex('|',@orden,@c9+1)    
Set @c11= CharIndex('|',@orden,@c10+1)    
Set @c12= CharIndex('|',@orden,@c11+1)    
Set @c13= CharIndex('|',@orden,@c12+1)
Set @c14= CharIndex('|',@orden,@c13+1)    
Set @c15= CharIndex('|',@orden,@c14+1)     
Set @c16= Len(@orden)+1

set @IdDeuda=Convert(numeric(38),SUBSTRING(@orden,1,@c1-1))    
set @Numero=SUBSTRING(@orden,@c1+1,@c2-@c1-1)    
set @Proveedor=SUBSTRING(@orden,@c2+1,@c3-@c2-1)    
set @SaldoDocu=Convert(decimal(18,2),SUBSTRING(@orden,@c3+1,@c4-@c3-1))    
set @Moneda=SUBSTRING(@orden,@c4+1,@c5-@c4-1)  
set @TipoCambio=Convert(decimal(18,3),SUBSTRING(@orden,@c5+1,@c6-@c5-1))

set @EfectivoSoles=Convert(decimal(18,2),SUBSTRING(@orden,@c6+1,@c7-@c6-1))   
set @EfectivoDolar=Convert(decimal(18,2),SUBSTRING(@orden,@c7+1,@c8-@c7-1))    
set @DepositoSoles=Convert(decimal(18,2),SUBSTRING(@orden,@c8+1,@c9-@c8-1))    
set @DepositoDolar=Convert(decimal(18,2),SUBSTRING(@orden,@c9+1,@c10-@c9-1))
    
set @EntidadBanco=SUBSTRING(@orden,@c10+1,@c11-@c10-1)  
set @NroOperacion=SUBSTRING(@orden,@c11+1,@c12-@c11-1)

set @AcuentaGeneral=Convert(decimal(18,2),SUBSTRING(@orden,@c12+1,@c13-@c12-1))    
set @TemporalFecha=SUBSTRING(@orden,@c13+1,@c14-@c13-1)    
set @UsuarioId=convert(int,SUBSTRING(@orden,@c14+1,@c15-@c14-1))  
set @Concepto=SUBSTRING(@orden,@c15+1,@c16-@c15-1)    
		 
insert into TemporalLiquida values  
(@IdDeuda,@Numero,@Proveedor,@SaldoDocu,@Moneda,@TipoCambio,  
@EfectivoSoles,@EfectivoDolar,@DepositoSoles,@DepositoDolar,  
@EntidadBanco,@NroOperacion,@AcuentaGeneral,@TemporalFecha,@UsuarioId,@Concepto)
select 'true'  

end
GO
/****** Object:  StoredProcedure [dbo].[KardeProveedor]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[KardeProveedor]
@IdProducto numeric(20),
@fechainicio date,
@fechafin date
as
begin
select p.ProveedorId,p.ProveedorRazon,(Convert(char(10),c.CompraEmision,103)) as FechaEmision,
substring(t.TipoDescripcion,1,1)+'-C  '+c.CompraSerie+'-'+c.CompraNumero as Numero,
c.CompraTipoCambio as TipoCambio,CONVERT(VarChar(50), cast(d.DetalleCantidad as money ), 1)as Cantidad,
substring(d.DetalleUM,1,3) as UM,	
case when(CompraMoneda='DOLARES')then 
case when(CompraTipoIgv='DISGREGADO')then
cast((((((d.DetalleImporte-d.DetalleDescuento)/d.DetalleCantidad)*1)*1.18)- d.DescuentoB) as decimal(18,4))
else
cast(((cast(((d.DetalleImporte-d.DetalleDescuento)/d.DetalleCantidad)as decimal(18,4))-d.DescuentoB)*1) as decimal(18,4))
end
else
case when(CompraTipoIgv='DISGREGADO') then
cast(((((d.DetalleImporte-d.DetalleDescuento)/d.DetalleCantidad)*1.18)-d.DescuentoB) as decimal(18,4))
else 
cast((((d.DetalleImporte-d.DetalleDescuento)/d.DetalleCantidad)-d.DescuentoB)as decimal(18,4)) 
end end as CostoSoles,
------
case when(CompraMoneda='DOLARES')then 
case when(CompraTipoIgv='DISGREGADO')then
cast(((((d.DetalleImporte-d.DetalleDescuento)/d.DetalleCantidad)*1.18)-d.DescuentoB) as decimal(18,4))
else 
cast((((d.DetalleImporte-d.DetalleDescuento)/d.DetalleCantidad)-d.DescuentoB) as decimal(18,4))
end
else 
case when(CompraTipoIgv='DISGREGADO')then 
cast((((((d.DetalleImporte-d.DetalleDescuento)/d.DetalleCantidad)/1)*1.18)-d.DescuentoB) as decimal(18,4))
else 
cast(((((d.DetalleImporte-d.DetalleDescuento)/d.DetalleCantidad)/1)-d.DescuentoB) as decimal(18,4))
end end as CostoDolar
from DetalleCompra d
inner join Compras c
on c.CompraId=d.CompraId
inner join Proveedor p
on p.ProveedorId=c.ProveedorId
inner join TipoComprobante t
on t.TipoCodigo=c.TipoCodigo
where(Convert(char(10),c.CompraEmision,101) BETWEEN @fechainicio AND @fechafin) and d.IdProducto=@IdProducto
order by 1 desc,c.CompraEmision desc
end
GO
/****** Object:  StoredProcedure [dbo].[kardexCompra]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[kardexCompra]   
@orden varchar(max)
as  
begin
Declare @c1 int,@c2 int,@c3 int 
Declare @ProveedorId numeric(38),  
		@Asociado varchar(60),  
		@CompraId numeric(38)
Set @c1 = CharIndex('|',@orden,0)          
Set @c2 = CharIndex('|',@orden,@c1+1)          
Set @c3 = Len(@orden)+1		
set @ProveedorId=convert(numeric(38),SUBSTRING(@orden,1,@c1-1))          
set @Asociado=SUBSTRING(@orden,@c1+1,@c2-@c1-1)          
set @CompraId=SUBSTRING(@orden,@c2+1,@c3-@c2-1)		
select
'FechaPago|CompraId|Documento|Moneda|Acuenta|CompraTotal¬90|90|90|90|90¬String|String|String|String|String¬'+
isnull((select STUFF((select '¬'+(Convert(char(10),c.CompraEmision,103))+'|'+
convert(varchar,c.CompraId)+'|'+  
'NC '+c.CompraSerie+'-'+c.CompraNumero+'|'+
c.CompraMoneda+'|'+  
CONVERT(VarChar(50), cast(c.CompraTotal as money ), 1)+'|'+
convert(varchar,c.CompraTotal)  
from Compras c  
where (c.ProveedorId=@ProveedorId and (c.CompraAsociado)=@Asociado)
order by c.CompraTotal desc  
FOR XML path ('')),1,1,'')),'~')+'¬'+   
isnull((select STUFF((select '¬'+d.FechaPago+'|'+
convert(varchar,d.CompraId)+'|'+
'LQ '+l.LiquidacionNumero+'|'+
c.CompraMoneda+'|'+
CONVERT(VarChar(50), cast(d.AcuentaGeneral as money ), 1)+'|'+
convert(varchar,d.AcuentaGeneral)  
from DetalleLiquida d  
inner join Liquidacion l  
on l.LiquidacionId=d.LiquidacionId  
inner join Compras c  
on c.CompraId=d.CompraId  
where c.CompraId=@CompraId  
order by d.AcuentaGeneral desc
FOR XML path ('')),1,1,'')),'~')  
end
GO
/****** Object:  StoredProcedure [dbo].[Ld_listaAlmacen]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[Ld_listaAlmacen]
as
begin
select 
'Id|Almacen|Departamento|Provincia|Distrito|Direccion|Estado¬80|435|100|100|100|100|100¬String|String|String|String|String|String|String¬'+
isnull((select STUFF((select '¬'+ convert(varchar,a.AlmacenId)+'|'+a.AlmacenNombre+'|'+a.AlmacenDepartamento+'|'+
a.AlmacenProvincia+'|'+a.AlmacenDistrito+'|'+a.AlmacenDireccion+'|'+a.AlmacenEstado
from Almacen a
order by AlmacenId desc
for xml path('')),1,1,'')),'~')
end
GO
/****** Object:  StoredProcedure [dbo].[ldBloques]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[ldBloques]
as
begin
declare @fechaReferencia date
set @fechaReferencia=(select top 1 n.NotaFecha from NotaPedido n
where (n.NotaCondicion='ALCONTADO' and n.NotaEntrega='INMEDIATA' and n.NotaFormaPago='EFECTIVO')and
(n.NotaEstado<>'ANULADO'and(n.NotaConcepto='MERCADERIA' and(((n.NotaEstado<>'CANCELADO' and n.NotaAcuenta<=0) AND n.NotaDocu <>'PROFORMA'))))
group by n.NotaFecha
order by n.NotaFecha asc)
select
'NotaId|Usuario|FechaEmision|Documento|ClienteRazon|Saldo|Total¬100|150|150|135|400|120|120¬String|String|String|String|String|String|String¬'+
isnull((select STUFF((select '¬'+convert(varchar,n.NotaId)+'|'+n.NotaUsuario+'|'+
(IsNull(convert(varchar,n.NotaFecha,103),'')+' '+ IsNull(SUBSTRING(convert(varchar,n.NotaFecha,114),1,8),''))+'|'+
n.NotaDocu+'|'+c.ClienteRazon+'|'+
CONVERT(VarChar(50), cast(n.NotaSaldo as money ), 1)+'|'+
CONVERT(VarChar(50), cast(n.NotaPagar as money ), 1)
from NotaPedido n
inner join Cliente c
on c.ClienteId=n.ClienteId
where convert(date,n.NotaFecha)=@fechaReferencia and(n.NotaCondicion='ALCONTADO' and n.NotaEntrega='INMEDIATA' and n.NotaFormaPago='EFECTIVO')and
(n.NotaEstado<>'ANULADO'and(n.NotaConcepto='MERCADERIA' and(((n.NotaEstado<>'CANCELADO' and n.NotaAcuenta<=0) AND n.NotaDocu <>'PROFORMA'))))
order by n.NotaId asc
FOR XML path ('')),1,1,'')),'~')+'_'+
'NotaId|FechaEmision|Documento|Vendedor|IdPro|Cantidad|UM|Descripcion|PrecioVenta|PrecioCosto|Importe|ValorUM¬95|153|105|150|70|100|60|330|100|100|110|100¬String|String|String|String|String|String|String|String|String|String|String|String¬'+
isnull((select STUFF(( select '¬'+ convert(varchar,d.NotaId)+'|'+
(IsNull(convert(varchar,n.NotaFecha,103),'')+' '+ IsNull(SUBSTRING(convert(varchar,n.NotaFecha,114),1,8),''))+'|'+
n.NotaDocu+'|'+n.NotaUsuario+'|'+
convert(varchar,d.IdProducto)+'|'+
CONVERT(VarChar(50), cast(d.DetalleCantidad as money ), 1)+'|'+
d.DetalleUm+'|'+d.DetalleDescripcion+'|'+
CONVERT(VarChar(50), cast(d.DetallePrecio as money ), 1)+'|'+
CONVERT(VarChar(50), cast(d.DetalleCosto as money ), 1)+'|'+
CONVERT(VarChar(50), cast(d.DetalleImporte as money ), 1)+'|'+
CONVERT(varchar,d.ValorUM)
from DetallePedido d
inner join NotaPedido n
on n.NotaId=d.NotaId
where convert(date,n.NotaFecha)=@fechaReferencia and(n.NotaCondicion='ALCONTADO' and n.NotaEntrega='INMEDIATA' and n.NotaFormaPago='EFECTIVO')and
(n.NotaEstado<>'ANULADO'and(n.NotaConcepto='MERCADERIA' and(((n.NotaEstado<>'CANCELADO' and n.NotaAcuenta<=0) AND n.NotaDocu <>'PROFORMA'))))
order by n.NotaId asc
FOR XML PATH('')), 1, 1, '')),'~')+'_'+
isnull((select STUFF((select '¬'+CONVERT(varchar,c.CajaId)
from Caja c
where CajaEstado='ACTIVO'
FOR XML path ('')),1,1,'')),'0')+'_'+
isnull((select top 15 STUFF((select top 15 '¬'+convert(varchar,b.BloqueId)+'|'+
(IsNull(convert(varchar,b.BloqueFecha,103),'')+' '+ IsNull(SUBSTRING(convert(varchar,b.BloqueFecha,114),1,8),''))
from Bloque b
order by b.BloqueId desc
FOR XML path ('')),1,1,'')),'')
end
GO
/****** Object:  StoredProcedure [dbo].[LDdocumentos]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[LDdocumentos]
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
GO
/****** Object:  StoredProcedure [dbo].[LdGanancia]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[LdGanancia] 
@NotaId numeric(38)
as
begin
declare @Estado varchar(80)
set @Estado=(select top 1 n.NotaEstado from NotaPedido n where n.NotaId=@NotaId)
select 
'FechaEmision|Vendedor|Descripcion|Cantidad|UM|PrecioUni|PreCosto|GXUnidad|Importe|Ganancia¬150|150|385|110|70|110|110|110|0|120¬'+
(select STUFF((select '¬'+(IsNull(convert(varchar,n.NotaFecha,103),'')+' '+ IsNull(SUBSTRING(convert(varchar,n.NotaFecha,114),1,8),''))+'|'+
n.NotaUsuario+'|'+d.DetalleDescripcion+'|'+
CONVERT(VarChar(50), cast((d.DetalleCantidad) as money ), 1)+'|'+d.DetalleUm+'|'+
CONVERT(VarChar(50), cast(d.DetallePrecio as money ), 1)+'|'+
CONVERT(VarChar(50), cast(d.DetalleCosto as money ), 1)+'|'+
CONVERT(VarChar(50), cast((d.DetallePrecio-d.DetalleCosto) as money ), 1)+'|'+
CONVERT(VarChar(50), cast((d.DetalleImporte) as money ), 1)+'|'+
CONVERT(VarChar(50), cast(((d.DetallePrecio-d.DetalleCosto)* d.DetalleCantidad) as money ), 1)
from DetallePedido d (noLOCK) 
inner join NotaPedido n (noLOCK)
on n.NotaId=d.NotaId
where d.NotaId=@NotaId
order by d.DetalleId asc
for xml path('')),1,1,''))
end
GO
/****** Object:  StoredProcedure [dbo].[LDrptCompra]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[LDrptCompra]
@Mes int,
@ANNO int
as
begin
select 'Compania|FechaEmision|Documento|RUC|RazonSocial|Tipo|BaseImp|IGV|Total|Moneda|TipoSunat|Monto|Referencia¬
68|95|110|90|330|45|105|105|105|85|75|105|110¬'+
(select stuff((select '¬'+CONVERT(varchar,c.CompaniaId)+'|'+(Convert(char(10),c.CompraEmision,103))+'|'+
(c.CompraSerie+'-'+c.CompraNumero)+'|'+
p.ProveedorRuc+'|'+p.ProveedorRazon+'|'+c.TipoCodigo+'|'+
case when c.CompraMoneda='DOLARES' THEN
case when c.TipoCodigo='07' then
'-'+CONVERT(VarChar(50), cast((c.CompraTotal/1.18)*c.CompraTipoSunat as money ), 1)
else
 CONVERT(VarChar(50), cast((c.CompraTotal/1.18)*c.CompraTipoSunat as money ), 1)end
else  
case when c.TipoCodigo='07' then
'-'+CONVERT(VarChar(50), cast((c.CompraTotal/1.18) as money ), 1)
else
CONVERT(VarChar(50), cast((c.CompraTotal/1.18) as money ), 1)end
end+'|'+
case when c.CompraMoneda='DOLARES' then
case when c.TipoCodigo='07' then
'-'+CONVERT(VarChar(50), cast((c.CompraTotal-(c.CompraTotal/1.18))*c.CompraTipoSunat as money ), 1)
else
 CONVERT(VarChar(50), cast((c.CompraTotal-(c.CompraTotal/1.18))*c.CompraTipoSunat as money ), 1)end
else 
case when c.TipoCodigo='07' then
'-'+CONVERT(VarChar(50), cast((c.CompraTotal-(c.CompraTotal/1.18))as money ), 1)
else
CONVERT(VarChar(50), cast((c.CompraTotal-(c.CompraTotal/1.18))as money ), 1)end
end+'|'+
case when c.CompraMoneda='DOLARES' then
case when c.TipoCodigo='07' then
'-'+CONVERT(VarChar(50), cast((c.CompraTotal *c.CompraTipoSunat) as money ), 1)
else
CONVERT(VarChar(50), cast((c.CompraTotal *c.CompraTipoSunat) as money ), 1) end
else 
case when c.TipoCodigo='07' then
'-'+CONVERT(VarChar(50), cast(c.CompraTotal as money ), 1)
else
CONVERT(VarChar(50), cast(c.CompraTotal as money ), 1)end
end+'|'+
c.CompraMoneda+'|'+convert(varchar,c.CompraTipoSunat)+'|'+
case when c.TipoCodigo='07' then
'-'+CONVERT(VarChar(50), cast(c.CompraTotal as money ), 1)
else
CONVERT(VarChar(50), cast(c.CompraTotal as money ), 1)
end+'|'+CompraAsociado
from Compras c
inner join Proveedor p
on p.ProveedorId=c.ProveedorId
where (Month(c.CompraComputo)=@Mes and YEAR(c.CompraComputo)=@ANNO) and(c.TipoCodigo='01' or c.TipoCodigo='07')
order by c.CompraEmision asc
FOR XML PATH('')), 1, 1, ''))
end
GO
/****** Object:  StoredProcedure [dbo].[ldTraerDetalle]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[ldTraerDetalle]
@Data varchar(max)
as
begin
declare @p0 int, 
        @p1 int
declare @IdProducto numeric(20),
		@NotaId numeric(38),
		@Ganancia decimal(18,2)
		set @p0 = CharIndex('|',@Data,0)
        Set @p1 = Len(@Data)+1
	Set @IdProducto=Convert(numeric(20),SUBSTRING(@Data,1,@p0-1))
	Set @NotaId= Convert(numeric(38),SUBSTRING(@Data,@p0+1,@p1-@p0-1))
	set @Ganancia=(select (d.DetallePrecio - d.DetalleCosto) 
	from DetallePedido d where d.IdProducto=@IdProducto and d.NotaId=@NotaId)
begin
	update DetallePedido 
	set DetalleCantidad=DetalleCantidad + 1,
	DetalleImporte=((DetalleCantidad + 1)* DetallePrecio) 
	where IdProducto=@IdProducto and NotaId=@NotaId
	update NotaPedido
	set NotaGanancia=NotaGanancia+@Ganancia
	where NotaId=@NotaId
	select 'true'
end
end
GO
/****** Object:  StoredProcedure [dbo].[LetrasVencidas]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[LetrasVencidas]     
as    
begin  
select  
'RazonSocial|Documento|SaldoDoc|Vencimiento|FinVencimiento|Estado¬90|90|90|90|90|90¬String|String|String|String|String|String¬'+  
isnull((select STUFF((select '¬'+  
p.ProveedorRazon+'|'+  
'LT '+d.LetraCanje+'|'+    
substring(l.LetraMoneda,1,1)+'/  '+CONVERT(VarChar(50),cast(d.DetalleSaldo as money ), 1)+'|'+    
(Convert(char(10),d.LetraVencimiento,103))+'|'+    
convert(char(10),(dateadd(DAY,6,d.LetraVencimiento)),103)+'|'+    
case when ((dateadd(DAY,6,d.LetraVencimiento))<= CONVERT(date,GETDATE())) then    
'VENCIDO'    
else    
case when (CONVERT(date,GETDATE())>=(d.LetraVencimiento)) then    
'POR VENCER'    
else    
'PENDIENTE'    
end end    
from DetalleLetra d    
inner join Letra l    
on l.LetraId=d.LetraId    
inner join Proveedor p    
on p.ProveedorId=l.ProveedorId    
where (d.DetalleEstado<>'TOTALMENTE PAGADO') and ((dateadd(DAY,-6,d.LetraVencimiento))<= CONVERT(date,GETDATE()))  
FOR XML path ('')),1,1,'')),'~') +'¬'+   
isnull((select STUFF((select '¬'+  
p.ProveedorRazon+'|'+  
substring(t.TipoDescripcion,1,1)+'C '+C.CompraSerie+' '+c.CompraNumero+'|'+    
substring(c.CompraMoneda,1,1)+'/  '+CONVERT(VarChar(50),cast(c.CompraSaldo as money ), 1)+'|'+  
(Convert(char(10),c.CompraFechaPago,103))+'|'+  
(Convert(char(10),c.CompraFechaPago,103))+'|'+    
case when (CONVERT(date,GETDATE())>=(c.CompraFechaPago)) then    
'VENCIDO'    
else    
case when ((dateadd(DAY,-2,c.CompraFechaPago))<= CONVERT(date,GETDATE())) then    
'POR VENCER'    
else    
'PENDIENTE'    
end end     
from Compras c    
inner join Proveedor p    
on c.ProveedorId=p.ProveedorId    
inner join TipoComprobante t    
on t.TipoCodigo=c.TipoCodigo    
where c.CompraEstado='PENDIENTE DE PAGO' and ((dateadd(DAY,-6,c.CompraFechaPago))<= CONVERT(date,GETDATE()))  
FOR XML path ('')),1,1,'')),'~')    
end
GO
/****** Object:  StoredProcedure [dbo].[LetrasVencidasR]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[LetrasVencidasR]
as
begin
select Row_number() over(order by d.LetraVencimiento asc)as Item,p.ProveedorRazon as RazonSocial,'LT '+d.LetraCanje as LetraCanje,
l.LetraMoneda as Moneda,CONVERT(VarChar(50),cast(d.DetalleSaldo as money ), 1) as SaldoDoc,
(Convert(char(10),d.LetraVencimiento,103)) as Vencimiento,
convert(char(10),(dateadd(DAY,6,d.LetraVencimiento)),103) as FinVencimiento,
case when ((dateadd(DAY,6,d.LetraVencimiento))<= CONVERT(date,GETDATE())) then
'VENCIDO'
else
case when ((dateadd(DAY,-6,d.LetraVencimiento))<= CONVERT(date,GETDATE())) then
'POR VENCER'
else 
'PENDIENTE'
end end as Estado
from DetalleLetra d
inner join Letra l
on l.LetraId=d.LetraId
inner join Proveedor p
on p.ProveedorId=l.ProveedorId
where (d.DetalleEstado<>'TOTALMENTE PAGADO')
order by d.LetraVencimiento asc
end
GO
/****** Object:  StoredProcedure [dbo].[listaBloque]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[listaBloque]
@BloqueId numeric(38)
as
begin
select
'NotaId|Usuario|FechaEmision|Documento|ClienteRazon|Saldo|Total¬100|150|150|135|400|120|120¬String|String|String|String|String|String|String¬'+
isnull((select STUFF((select '¬'+convert(varchar,b.NotaId)+'|'+
n.NotaUsuario+'|'+
(IsNull(convert(varchar,n.NotaFecha,103),'')+' '+ IsNull(SUBSTRING(convert(varchar,n.NotaFecha,114),1,8),''))+'|'+
n.NotaDocu+'|'+
c.ClienteRazon+'|'+
CONVERT(VarChar(50), cast(n.NotaSaldo as money ), 1)+'|'+
CONVERT(VarChar(50), cast(n.NotaPagar as money ), 1)+'|'+
convert(varchar,b.BloqueId)
from DetalleBloque b
inner join NotaPedido n
on  n.NotaId=b.NotaId
inner join Cliente c
on c.ClienteId=n.ClienteId
where b.BloqueId=@BloqueId
FOR XML path ('')),1,1,'')),'~')+'_'+
'NotaId|FechaEmision|Documento|Vendedor|IdPro|Cantidad|UM|Descripcion|PrecioVenta|PrecioCosto|Importe|ValorUM¬95|153|105|150|70|100|60|330|100|100|110|100¬String|String|String|String|String|String|String|String|String|String|String|String¬'+
isnull((select STUFF(( select '¬'+ convert(varchar,d.NotaId)+'|'+
(IsNull(convert(varchar,n.NotaFecha,103),'')+' '+ IsNull(SUBSTRING(convert(varchar,n.NotaFecha,114),1,8),''))+'|'+
n.NotaDocu+'|'+n.NotaUsuario+'|'+
convert(varchar,d.IdProducto)+'|'+
CONVERT(VarChar(50), cast(d.DetalleCantidad as money ), 1)+'|'+
d.DetalleUm+'|'+d.DetalleDescripcion+'|'+
CONVERT(VarChar(50), cast(d.DetallePrecio as money ), 1)+'|'+
CONVERT(VarChar(50), cast(d.DetalleCosto as money ), 1)+'|'+
CONVERT(VarChar(50), cast(d.DetalleImporte as money ), 1)+'|'+
CONVERT(varchar,d.ValorUm)
from DetalleBloque b
inner join DetallePedido d
on d.NotaId=b.NotaId
inner join NotaPedido n
on n.NotaId=d.NotaId
where b.BloqueId=@BloqueId
FOR XML PATH('')), 1, 1, '')),'~')
end
GO
/****** Object:  StoredProcedure [dbo].[listaCanjeFactura]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[listaCanjeFactura]
as
begin
SELECT dbo.GuiaCanje.*, dbo.Compras.CompraMoneda as Moneda,(convert(varchar(50), CAST(dbo.Compras.CompraValorVenta as money), -1))as Total,
(SUBSTRING(dbo.Compras.CompraMoneda,1,1)+'/.  '+(convert(varchar(50), CAST(dbo.Compras.CompraTotal as money), -1)))as Monto,dbo.Proveedor.ProveedorRazon as Proveedor
FROM dbo.GuiaCanje INNER JOIN dbo.Compras ON dbo.GuiaCanje.CompraId = dbo.Compras.CompraId inner join dbo.Proveedor on dbo.Proveedor.ProveedorId=dbo.Compras.ProveedorId 
where year(dbo.GuiaCanje.CanjeFecha)=YEAR(GETDATE())
order by dbo.GuiaCanje.CanjeId desc
end
GO
/****** Object:  StoredProcedure [dbo].[listaCompraEmision]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[listaCompraEmision]   
@fechainicio date,  
@fechafin date  
as  
begin  
select   
'CompraId|Correlativo|Compania|FechaRegistro|Computo|Emision|RazonSocial|RUC|Documento|Serie|Numero|Condicion|Moneda|TipoCambio|DiasPlazo|FechaPago|TipoIgv|ValorVenta|Descuento|SubTotal|Igv|Total|SaldoDoc|Usuario|CompaniaNombre|Estado|ProveedorId|DocumentoId|Asociado|OBS|TipoSunat|Concepto|Percepcion¬100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100¬String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String¬'+  
isnull((select STUFF ((select '¬'+convert(varchar,c.CompraId)+'|'+  
c.CompraCorrelativo+'|'+  
convert(varchar,c.CompaniaId)+'|'+  
(IsNull(convert(varchar,c.CompraRegistro,103),'')+' '+ IsNull(SUBSTRING(convert(varchar,c.CompraRegistro,114),1,8),''))+'|'+  
Convert(char(10),c.CompraComputo,103)+'|'+  
Convert(char(10),c.CompraEmision,103)+'|'+  
p.ProveedorRazon+'|'+  
p.ProveedorRuc+'|'+  
c.TipoCodigo+'|'+  
c.CompraSerie+'|'+  
c.CompraNumero+'|'+  
c.CompraCondicion+'|'+  
c.CompraMoneda+'|'+  
convert(varchar,CompraTipoCambio)+'|'+  
convert(varchar,c.CompraDias)+'|'+  
Convert(char(10),c.CompraFechaPago,103)+'|'+  
c.CompraTipoIgv+'|'+  
CONVERT(VarChar(50), cast(c.CompraValorVenta as money ), 1)+'|'+  
CONVERT(VarChar(50), cast(c.CompraDescuento as money ), 1)+'|'+  
CONVERT(VarChar(50), cast(c.CompraSubtotal as money ), 1)+'|'+  
CONVERT(VarChar(50), cast(c.CompraIgv as money ), 1)+'|'+  
CONVERT(VarChar(50), cast(c.CompraTotal as money ), 1)+'|'+  
CONVERT(VarChar(50), cast(c.compraSaldo as money ), 1)+'|'+  
c.CompraUsuario+'|'+  
co.CompaniaRazonSocial+'|'+  
c.CompraEstado+'|'+  
convert(varchar,c.ProveedorId)+'|'+  
t.TipoDescripcion+'|'+  
c.CompraAsociado+'|'+  
CompraOBS+'|'+  
convert(varchar,CompraTipoSunat)+'|'+  
CompraConcepto+'|'+  
convert(varchar,c.CompraPercepcion)  
from Compras c  
inner join Proveedor p  
on p.ProveedorId=c.ProveedorId  
inner join Compania co  
on co.CompaniaId=c.CompaniaId  
inner join TipoComprobante t  
on t.TipoCodigo=c.TipoCodigo  
where c.TipoCodigo<>'07' and(Convert(char(10),c.CompraEmision, 101) BETWEEN @fechainicio AND @fechafin)  
order by c.CompraEmision asc  
for xml path('')),1,1,'')),'~')  
end
GO
/****** Object:  StoredProcedure [dbo].[listaDetaGeneral]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[listaDetaGeneral] 
@IdGeneral numeric(38)
as
select
'ID|Concepto|CajaId|Fecha|Descripcion|Monto|Usuario|Referencia|GastoId¬90|100|80|136|212|120|100|100|100¬String|String|String|String|String|String|String|String|String¬'+
isnull((select STUFF((select '¬'+ convert(varchar,c.IdCaja)+'|'+c.CajaConcepto+'|'+convert(varchar,c.CajaId)+'|'+
Convert(char(10),c.CajaFecha,103)+' '+Convert(char(8),c.CajaFecha,114) 
+'|'+c.CajaDescripcion+'|'+CONVERT(VarChar(50),cast(c.CajaMonto as money), 1)+'|'+
c.CajaUsuario+'|'+c.Referencia+'|'+c.GastoId
from CajaPincipal c 
where c.CajaConcepto='INGRESO' and c.IdGeneral=@IdGeneral
order by c.IdCaja desc
for xml path('')),1,1,'')),'~')+'['+
'ID|Concepto|CajaId|Fecha|Descripcion|Monto|Usuario|Referencia|GastoId¬90|100|80|135|290|125|100|100|100¬String|String|String|String|String|String|String|String|String¬'+
isnull((select STUFF((select '¬'+ convert(varchar,c.IdCaja)+'|'+c.CajaConcepto+'|'+convert(varchar,c.CajaId)+'|'+
Convert(char(10),c.CajaFecha,103)+' '+Convert(char(8),c.CajaFecha,114) 
+'|'+c.CajaDescripcion+'|'+CONVERT(VarChar(50),cast(c.CajaMonto as money), 1)+'|'+
c.CajaUsuario+'|'+c.Referencia+'|'+c.GastoId
from CajaPincipal c 
where c.CajaConcepto='SALIDA' and c.IdGeneral=@IdGeneral
order by c.IdCaja desc
for xml path('')),1,1,'')),'~')
GO
/****** Object:  StoredProcedure [dbo].[listaDetaliquiVenta]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[listaDetaliquiVenta]       
@LiquidacionId varchar(38)      
as      
begin      
select    
'ID|DocuId|Documento|RazonSocial|Saldo|Moneda|TipoCambio|EfectivoSoles|EfectivoDolar|DepositoSoles|DepositoDolar|EntidadBancaria|NroOperacion|Acuenta|Usuario|FechaOperacion|SaldoActual|NotaId¬90|90|90|90|90|90|90|90|90|90|90|90|90|90|90|90|90|90¬String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String¬'+     
isnull((select STUFF((select '¬'+convert(varchar,d.DetalleId)+'|'+  
convert(varchar,d.NotaId)+'|'+d.Documento+'|'+    
d.Cliente+'|'+    
CONVERT(VarChar(50),cast(d.SaldoDocu as money ), 1) +'|SOLES|'+  
convert(varchar,d.TipoCambio)+'|'+  
convert(varchar,d.EfectivoSoles)+'|'+      
convert(varchar,d.EfectivoDolar)+'|'+    
convert(varchar,d.DepositoSoles)+'|'+    
convert(varchar,d.DepositoDolar)+'|'+   
d.EntidadBanco+'|'+    
d.NroOperacion+'|'+      
CONVERT(VarChar(50), cast(d.AcuentaGeneral as money ), 1)+'||'+      
d.FechaPago+'|'+    
CONVERT(VarChar(50), cast(d.SaldoActual as money ), 1)+'|'+    
convert(varchar,d.NotaId)       
from DetaLiquidaVenta d 
where d.LiquidacionId=@LiquidacionId      
order by d.DetalleId asc     
for xml path('')),1,1,'')),'~')    
end
GO
/****** Object:  StoredProcedure [dbo].[listaDetalleCompra]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[listaDetalleCompra]  
@CompraId varchar(60)  
as  
begin  
select  
'DetalleId|IdProducto|DetalleCodigo|Descripcion|UM|Cantidad|PrecioCosto|Descuento|Importe|ValorUM|Estado|AplicaINV¬100|100|100|420|80|90|100|100|110|100|100|100¬String|String|String|String|String|String|String|String|String|String|String|String¬'+  
isnull((select STUFF((select '¬'+convert(varchar,d.DetalleId)+'|'+convert(varchar,d.IdProducto)+'|'+  
d.DetalleCodigo+'|'+d.Descripcion+'|'+d.DetalleUM+'|'+CONVERT(VarChar(50),cast(d.DetalleCantidad as money ),1)+'|'+  
convert(varchar,d.PrecioCosto)+'|'+convert(varchar,d.detalleDescuento)+'|'+  
CONVERT(VarChar(50),cast(d.DetalleImporte as money ), 2)+'|'+
CONVERT(varchar,d.ValorUM)+'|'+d.DetalleEstado+'|'+p.AplicaINV  
from DetalleCompra d
inner join Producto p
on p.IdProducto=d.IdProducto  
where d.CompraId=@CompraId  
order by d.DetalleId asc  
for xml path('')),1,1,'')),'~')+'['+  
'IdUm|IdProducto|UNIDAD M|Valor|Costo¬100|100|100|100|100¬String|String|String|String|String¬'+  
isnull((select STUFF ((select '¬'+convert(varchar,u.IdUm)+'|'+convert(varchar,u.IdProducto)+'|'+  
u.UMDescripcion+'|'+CONVERT(VarChar(50), cast(u.ValorUM as money ), 1)+'|'+  
convert(varchar,d.PrecioCosto)  
from UnidadMedida u  
inner join DetalleCompra d  
on d.IdProducto=u.IdProducto  
where d.CompraId=@CompraId  
order by u.ValorUM asc  
for xml path('')),1,1,'')),'~')  
end
GO
/****** Object:  StoredProcedure [dbo].[listaDocuCompania]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[listaDocuCompania]        
@fechainicio date,        
@fechafin date        
as        
begin              
select         
'Id|Compania|NroNota|Documento|NroDocumento|ClienteId|Cliente|RUC|DNI|Direcccion|Numero|FechaEmision|Condicion|Serie|DocuLetras|SubTotal|Igv|ICBPER|Total|Usuario|EstadoDocu|CompaniaNombre|Saldo|Estado|DocuAdiconal|DocuHash|RucCompania|Correo|Referencia|Gravada|Descuento|EnvioCorreo|FormaPago|EntidadBancaria|NroOperacion¬100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100¬String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String¬'+         
isnull((select STUFF ((select '¬'+convert(varchar,d.DocuId)+'|'+        
convert(varchar,d.CompaniaId)+'|'+        
convert(varchar,d.NotaId)+'|'+        
d.DocuDocumento+'|'+        
d.docuSerie+'-'+d.DocuNumero+'|'+        
convert(varchar,d.ClienteId)+'|'+        
d.ClienteRazon+'|'+        
d.ClienteRuc+'|'+        
d.ClienteDni+'|'+        
d.DireccionFiscal+'|'+        
d.DocuNumero+'|'+        
(Convert(char(10),d.DocuEmision,103))+'|'+        
d.DocuCondicion+'|'+        
d.DocuSerie+'|'+        
d.DocuLetras+'|'+
case when(d.TipoCodigo='07')then  
'-'+(convert(varchar(50), CAST(d.DocuSubTotal as money), -1))        
else (convert(varchar(50), CAST(d.DocuSubTotal as money), -1))end+'|'+
case when(d.TipoCodigo='07')then 
'-'+(convert(varchar(50), CAST(d.DocuIgv as money), -1))
else +(convert(varchar(50), CAST(d.DocuIgv as money), -1))end+'|'+
case when(d.TipoCodigo='07')then      
'-'+(convert(varchar(50), CAST(d.ICBPER as money), -1))
else (convert(varchar(50), CAST(d.ICBPER as money), -1))end +'|'+
case when(d.TipoCodigo='07')then        
'-'+(convert(varchar(50), CAST(d.DocuTotal as money), -1))       
else (convert(varchar(50), CAST(d.DocuTotal as money), -1))end +'|'+
d.DocuUsuario+'|'+        
d.DocuEstado+'|'+        
co.CompaniaRazonSocial+'|'+        
convert(varchar,d.DocuSaldo)+'|'+        
d.EstadoSunat+'|'+        
convert(varchar,d.DocuAdicional)+'|'+        
d.DocuHash+'|'+        
co.CompaniaRUC+'|'+        
c.ClienteCorreo+'|'+        
d.DocuNroGuia+'|'+ 
case when(d.TipoCodigo='07')then      
'-'+convert(varchar(50), CAST(d.DocuGravada as money),1)
else convert(varchar(50), CAST(d.DocuGravada as money),1)end+'|'+        
case when(d.TipoCodigo='07')then  
'-'+convert(varchar(50), CAST(d.DocuDescuento as money),1)
else convert(varchar(50), CAST(d.DocuDescuento as money),1)end+'|'+
d.EnvioCorreo+'|'+      
d.FormaPago+'|'+  
d.EntidadBancaria+'|'+  
d.NroOperacion       
from DocumentoVenta d        
inner join Compania co        
on co.CompaniaId=d.CompaniaId        
inner join Cliente c        
on c.ClienteId=d.ClienteId         
where d.CompaniaId='1'and(Convert(char(10),d.DocuEmision,101) BETWEEN @fechainicio AND @fechafin)        
order by d.DocuId desc        
for xml path('')),1,1,'')),'~')        
end
GO
/****** Object:  StoredProcedure [dbo].[listaDocumentos]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[listaDocumentos]        
as        
begin        
select         
'Id|Compania|NroNota|Documento|NroDocumento|ClienteId|Cliente|RUC|DNI|Direcccion|Numero|FechaEmision|Condicion|Serie|DocuLetras|SubTotal|Igv|ICBPER|Total|Usuario|EstadoDocu|CompaniaNombre|Saldo|Estado|DocuAdiconal|DocuHash|RucCompania|Correo|Referencia|Gravada|Descuento|EnvioCorreo|FormaPago|EntidadBancaria|NroOperacion¬100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100¬String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String¬'+         
isnull((select STUFF ((select '¬'+convert(varchar,d.DocuId)+'|'+        
convert(varchar,d.CompaniaId)+'|'+        
convert(varchar,d.NotaId)+'|'+        
d.DocuDocumento+'|'+        
d.docuSerie+'-'+d.DocuNumero+'|'+        
convert(varchar,d.ClienteId)+'|'+        
d.ClienteRazon+'|'+        
d.ClienteRuc+'|'+        
d.ClienteDni+'|'+        
d.DireccionFiscal+'|'+        
d.DocuNumero+'|'+        
(Convert(char(10),d.DocuEmision,103))+'|'+        
d.DocuCondicion+'|'+        
d.DocuSerie+'|'+        
d.DocuLetras+'|'+
case when(d.TipoCodigo='07')then  
'-'+(convert(varchar(50), CAST(d.DocuSubTotal as money), -1))        
else (convert(varchar(50), CAST(d.DocuSubTotal as money), -1))end+'|'+
case when(d.TipoCodigo='07')then 
'-'+(convert(varchar(50), CAST(d.DocuIgv as money), -1))
else +(convert(varchar(50), CAST(d.DocuIgv as money), -1))end+'|'+
case when(d.TipoCodigo='07')then      
'-'+(convert(varchar(50), CAST(d.ICBPER as money), -1))
else (convert(varchar(50), CAST(d.ICBPER as money), -1))end +'|'+
case when(d.TipoCodigo='07')then        
'-'+(convert(varchar(50), CAST(d.DocuTotal as money), -1))       
else (convert(varchar(50), CAST(d.DocuTotal as money), -1))end +'|'+
d.DocuUsuario+'|'+        
d.DocuEstado+'|'+        
co.CompaniaRazonSocial+'|'+        
convert(varchar,d.DocuSaldo)+'|'+        
d.EstadoSunat+'|'+        
convert(varchar,d.DocuAdicional)+'|'+        
d.DocuHash+'|'+        
co.CompaniaRUC+'|'+        
c.ClienteCorreo+'|'+        
d.DocuNroGuia+'|'+ 
case when(d.TipoCodigo='07')then      
'-'+convert(varchar(50), CAST(d.DocuGravada as money),1)
else convert(varchar(50), CAST(d.DocuGravada as money),1)end+'|'+        
case when(d.TipoCodigo='07')then  
'-'+convert(varchar(50), CAST(d.DocuDescuento as money),1)
else convert(varchar(50), CAST(d.DocuDescuento as money),1)end+'|'+
d.EnvioCorreo+'|'+      
d.FormaPago+'|'+  
d.EntidadBancaria+'|'+  
d.NroOperacion       
from DocumentoVenta d        
inner join Compania co        
on co.CompaniaId=d.CompaniaId        
inner join Cliente c        
on c.ClienteId=d.ClienteId      
where Month(d.DocuEmision)=Month(GETDATE())and year(d.DocuEmision)=YEAR(Getdate())        
order by d.DocuId desc        
for xml path('')),1,1,'')),'~')        
end
GO
/****** Object:  StoredProcedure [dbo].[listaGeneralFecha]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[listaGeneralFecha]
@fechainicio date,
@fechafin date
as
begin 
select
isnull((select STUFF ((select '¬'+ CONVERT(varchar,c.IdGeneral)+'|'+
(IsNull(convert(varchar,c.FechaCierre,103),'')+' '+ IsNull(SUBSTRING(convert(varchar,c.FechaCierre,114),1,8),''))+'|'+c.Usuario+'|'+
CONVERT(varchar(50),cast(c.Ingresos as money),1)+'|'+CONVERT(varchar(50),cast(c.Salidas as money),1)+'|'+
CONVERT(varchar(50),cast(c.Total as money),1)
from CajaGeneral c
where (Convert(char(10),c.FechaCierre,103) BETWEEN @fechainicio AND @fechafin) 
order by c.IdGeneral desc
for xml path('')),1,1,'')),'~')
end
GO
/****** Object:  StoredProcedure [dbo].[listaLiquida]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[listaLiquida]  
as  
begin  
select 
'ID|Documento|Registro|Fecha|Descripcion|TipoCambio|EfectivoSol|DepositoSol|EfectivoDol|DepositoDol|TotalDol|TotalSol|Usuario¬90|90|90|90|90|90|90|90|90|90|90|90|90¬String|String|String|String|String|String|String|String|String|String|String|String|String¬'+  
isnull((select STUFF ((select '¬'+convert(varchar,l.LiquidacionId)+'|'+  
l.LiquidacionNumero+'|'+  
convert(varchar,l.LiquidacionRegistro,103)+' '+SUBSTRING(convert(varchar,l.LiquidacionRegistro,114),1,8)+'|'+   
(Convert(char(10),l.LiquidacionFecha,103))+'|'+    
l.LiquidacionDescripcion+'|'+  
convert(varchar,l.LiquidacionCambio)+'|'+    
CONVERT(VarChar(50), cast(l.LiquidaEfectivoSol as money ), 1)+'|'+    
CONVERT(VarChar(50), cast(l.LiquidaDepositoSol as money ), 1)+'|'+    
CONVERT(VarChar(50), cast(l.LiquidaEfectivoDol as money ), 1)+'|'+    
CONVERT(VarChar(50), cast(l.LiquidaDepositoDol as money ), 1)+'|'+   
CONVERT(VarChar(50), cast(l.LiquidaTotalDol as money ), 1)+'|'+    
CONVERT(VarChar(50), cast(l.LiquidaTotalSol as money ), 1)+'|'+    
l.LiquidaUsuario
from liquidacion l
where(month(l.LiquidacionFecha)=MONTH(GETDATE()) and YEAR(l.LiquidacionFecha)=YEAR(GETDATE()))  
order by l.LiquidacionId desc
for xml path('')),1,1,'')),'~')  
end
GO
/****** Object:  StoredProcedure [dbo].[listaliquidafecha]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[listaliquidafecha] 
@fechainicio date,
@fechafin date  
as  
begin
select
'ID|Documento|Registro|Fecha|Descripcion|TipoCambio|EfectivoSol|DepositoSol|EfectivoDol|DepositoDol|TotalDol|TotalSol|Usuario¬90|90|90|90|90|90|90|90|90|90|90|90|90¬String|String|String|String|String|String|String|String|String|String|String|String|String¬'+  
isnull((select STUFF ((select '¬'+convert(varchar,l.LiquidacionId)+'|'+  
l.LiquidacionNumero+'|'+  
convert(varchar,l.LiquidacionRegistro,103)+' '+SUBSTRING(convert(varchar,l.LiquidacionRegistro,114),1,8)+'|'+   
(Convert(char(10),l.LiquidacionFecha,103))+'|'+    
l.LiquidacionDescripcion+'|'+  
convert(varchar,l.LiquidacionCambio)+'|'+    
CONVERT(VarChar(50), cast(l.LiquidaEfectivoSol as money ), 1)+'|'+    
CONVERT(VarChar(50), cast(l.LiquidaDepositoSol as money ), 1)+'|'+    
CONVERT(VarChar(50), cast(l.LiquidaEfectivoDol as money ), 1)+'|'+    
CONVERT(VarChar(50), cast(l.LiquidaDepositoDol as money ), 1)+'|'+   
CONVERT(VarChar(50), cast(l.LiquidaTotalDol as money ), 1)+'|'+    
CONVERT(VarChar(50), cast(l.LiquidaTotalSol as money ), 1)+'|'+    
l.LiquidaUsuario
from liquidacion l
where (Convert(char(10),l.LiquidacionFecha,101) BETWEEN @fechainicio AND @fechafin)  
order by l.LiquidacionId desc
for xml path('')),1,1,'')),'~') 
end
GO
/****** Object:  StoredProcedure [dbo].[listaliquidafechaB]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[listaliquidafechaB]   
@fechainicio date,
@fechafin date  
as  
begin  
select 
'ID|Documento|Registro|Fecha|Descripcion|TipoCambio|EfectivoSol|DepositoSol|EfectivoDol|DepositoDol|TotalDol|TotalSol|Usuario¬90|90|90|90|90|90|90|90|90|90|90|90|90¬String|String|String|String|String|String|String|String|String|String|String|String|String¬'+
isnull((select STUFF ((select '¬'+convert(varchar,l.LiquidacionId)+'|'+
l.LiquidacionNumero+'|'+
convert(varchar,l.LiquidacionRegistro,103)+' '+SUBSTRING(convert(varchar,l.LiquidacionRegistro,114),1,8)+'|'+ 
(Convert(char(10),l.LiquidacionFecha,103))+'|'+  
l.LiquidacionDescripcion+'|'+
convert(varchar,l.LiquidacionCambio)+'|'+  
CONVERT(VarChar(50), cast(l.LiquidaEfectivoSol as money ), 1)+'|'+  
CONVERT(VarChar(50), cast(l.LiquidaDepositoSol as money ), 1)+'|'+  
CONVERT(VarChar(50), cast(l.LiquidaEfectivoDol as money ), 1)+'|'+  
CONVERT(VarChar(50), cast(l.LiquidaDepositoDol as money ), 1)+'|'+ 
CONVERT(VarChar(50), cast(l.LiquidaTotalDol as money ), 1)+'|'+  
CONVERT(VarChar(50), cast(l.LiquidaTotalSol as money ), 1)+'|'+  
l.LiquidaUsuario
from LiquidacionVenta l  
where (Convert(char(10),l.LiquidacionFecha,101) BETWEEN @fechainicio AND @fechafin)  
order by l.LiquidacionId desc
for xml path('')),1,1,'')),'~')  
end
GO
/****** Object:  StoredProcedure [dbo].[listaLiquidaVenta]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[listaLiquidaVenta]  
as  
begin  
select 
'ID|Documento|Registro|Fecha|Descripcion|TipoCambio|EfectivoSol|DepositoSol|EfectivoDol|DepositoDol|TotalDol|TotalSol|Usuario¬90|90|90|90|90|90|90|90|90|90|90|90|90¬String|String|String|String|String|String|String|String|String|String|String|String|String¬'+
isnull((select STUFF ((select '¬'+convert(varchar,l.LiquidacionId)+'|'+
l.LiquidacionNumero+'|'+
convert(varchar,l.LiquidacionRegistro,103)+' '+SUBSTRING(convert(varchar,l.LiquidacionRegistro,114),1,8)+'|'+ 
(Convert(char(10),l.LiquidacionFecha,103))+'|'+  
l.LiquidacionDescripcion+'|'+
convert(varchar,l.LiquidacionCambio)+'|'+  
CONVERT(VarChar(50), cast(l.LiquidaEfectivoSol as money ), 1)+'|'+  
CONVERT(VarChar(50), cast(l.LiquidaDepositoSol as money ), 1)+'|'+  
CONVERT(VarChar(50), cast(l.LiquidaEfectivoDol as money ), 1)+'|'+  
CONVERT(VarChar(50), cast(l.LiquidaDepositoDol as money ), 1)+'|'+ 
CONVERT(VarChar(50), cast(l.LiquidaTotalDol as money ), 1)+'|'+  
CONVERT(VarChar(50), cast(l.LiquidaTotalSol as money ), 1)+'|'+  
l.LiquidaUsuario
from LiquidacionVenta l  
where(month(l.LiquidacionFecha)=MONTH(GETDATE()) and YEAR(l.LiquidacionFecha)=YEAR(GETDATE()))  
order by l.LiquidacionId desc
for xml path('')),1,1,'')),'~')
end
GO
/****** Object:  StoredProcedure [dbo].[listaNotaComC]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[listaNotaComC] @f1 date,@f2 date
as
begin
select c.CompraId,c.CompraCorrelativo,c.CompaniaId,c.CompraRegistro,Convert(char(10),c.CompraComputo,103)as CompraComputo,Convert(char(10),c.CompraEmision,103)as CompraEmision,p.ProveedorRazon,
p.ProveedorRuc,c.TipoCodigo,c.CompraSerie,c.CompraNumero,c.CompraCondicion,c.CompraMoneda,CompraTipoCambio,c.CompraDias,Convert(char(10),c.CompraFechaPago,103) as CompraFechaPago,
c.CompraTipoIgv,CONVERT(VarChar(50), cast(c.CompraValorVenta as money ), 1) as ValorVenta,CONVERT(VarChar(50), cast(c.CompraDescuento as money ), 1)as Descuento,CONVERT(VarChar(50), 
cast(c.CompraSubtotal as money ), 1) as Subtotal,CONVERT(VarChar(50), cast(c.CompraIgv as money ), 1) as Igv,CONVERT(VarChar(50), cast(c.CompraTotal as money ), 1) as Total,
CONVERT(VarChar(50), cast(c.compraSaldo as money ), 1) as CompraSaldo,c.CompraUsuario,co.CompaniaRazonSocial,
c.CompraEstado,c.ProveedorId,t.TipoDescripcion,c.CompraAsociado as Asociado,CompraOBS,CompraTipoSunat as TipoSunat
from Compras c
inner join Proveedor p
on p.ProveedorId=c.ProveedorId
inner join Compania co
on co.CompaniaId=c.CompaniaId
inner join TipoComprobante t
on t.TipoCodigo=c.TipoCodigo
where (c.TipoCodigo='07' or c.TipoCodigo='101') and(Convert(char(10),c.CompraComputo, 103) BETWEEN @f1 AND @f2)
order by c.CompraId desc
end
GO
/****** Object:  StoredProcedure [dbo].[listaNotaComE]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[listaNotaComE] @f1 date,@f2 date
as
begin
select c.CompraId,c.CompraCorrelativo,c.CompaniaId,c.CompraRegistro,Convert(char(10),c.CompraComputo,103)as CompraComputo,Convert(char(10),c.CompraEmision,103)as CompraEmision,p.ProveedorRazon,
p.ProveedorRuc,c.TipoCodigo,c.CompraSerie,c.CompraNumero,c.CompraCondicion,c.CompraMoneda,CompraTipoCambio,c.CompraDias,Convert(char(10),c.CompraFechaPago,103) as CompraFechaPago,
c.CompraTipoIgv,CONVERT(VarChar(50), cast(c.CompraValorVenta as money ), 1) as ValorVenta,CONVERT(VarChar(50), cast(c.CompraDescuento as money ), 1)as Descuento,CONVERT(VarChar(50), 
cast(c.CompraSubtotal as money ), 1) as Subtotal,CONVERT(VarChar(50), cast(c.CompraIgv as money ), 1) as Igv,CONVERT(VarChar(50), cast(c.CompraTotal as money ), 1) as Total,
CONVERT(VarChar(50), cast(c.compraSaldo as money ), 1) as CompraSaldo,c.CompraUsuario,co.CompaniaRazonSocial,
c.CompraEstado,c.ProveedorId,t.TipoDescripcion,c.CompraAsociado as Asociado,CompraOBS,CompraTipoSunat as TipoSunat
from Compras c
inner join Proveedor p
on p.ProveedorId=c.ProveedorId
inner join Compania co
on co.CompaniaId=c.CompaniaId
inner join TipoComprobante t
on t.TipoCodigo=c.TipoCodigo
where (c.TipoCodigo='07' or c.TipoCodigo='101') and(Convert(char(10),c.CompraEmision,103) BETWEEN @f1 AND @f2)
order by c.CompraId desc
end
GO
/****** Object:  StoredProcedure [dbo].[listaNotaCompra]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[listaNotaCompra] 
as
begin
select c.CompraId,c.CompraCorrelativo,c.CompaniaId,c.CompraRegistro,Convert(char(10),c.CompraComputo,103)as CompraComputo,Convert(char(10),c.CompraEmision,103)as CompraEmision,p.ProveedorRazon,
p.ProveedorRuc,c.TipoCodigo,c.CompraSerie,c.CompraNumero,c.CompraCondicion,c.CompraMoneda,CompraTipoCambio,c.CompraDias,Convert(char(10),c.CompraFechaPago,103) as CompraFechaPago,
c.CompraTipoIgv,CONVERT(VarChar(50), cast(c.CompraValorVenta as money ), 1) as ValorVenta,CONVERT(VarChar(50), cast(c.CompraDescuento as money ), 1)as Descuento,CONVERT(VarChar(50), 
cast(c.CompraSubtotal as money ), 1) as Subtotal,CONVERT(VarChar(50), cast(c.CompraIgv as money ), 1) as Igv,CONVERT(VarChar(50), cast(c.CompraTotal as money ), 1) as Total,
CONVERT(VarChar(50), cast(c.compraSaldo as money ), 1) as CompraSaldo,c.CompraUsuario,co.CompaniaRazonSocial,
c.CompraEstado,c.ProveedorId,t.TipoDescripcion,c.CompraAsociado as Asociado,CompraOBS,CompraTipoSunat as TipoSunat
from Compras c
inner join Proveedor p
on p.ProveedorId=c.ProveedorId
inner join Compania co
on co.CompaniaId=c.CompaniaId
inner join TipoComprobante t
on t.TipoCodigo=c.TipoCodigo
where (c.TipoCodigo='07' or c.TipoCodigo='101')and(Month(c.CompraComputo)=Month(GETDATE()) and year(c.CompraComputo)=year(GETDATE()))
order by c.CompraId desc
end
GO
/****** Object:  StoredProcedure [dbo].[listaNotaPedido]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
CONVERT(VARCHAR(50),CAST(n.Deposito AS MONEY),1)  
  
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


GO
/****** Object:  StoredProcedure [dbo].[listaProveedor]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[listaProveedor]
 as
 begin
 select
 'Codigo|RazonSocial|RUC|Contacto|Celular|Telefono|Correo|Direccion|Estado¬90|400|105|200|150|150|150|250|100¬String|String|String|String|String|String|String|String|String¬'+
 isnull((select stuff((SELECT '¬'+ CONVERT(varchar,p.ProveedorId)+'|'+p.ProveedorRazon+'|'+p.ProveedorRuc+'|'+
 p.ProveedorContacto+'|'+p.ProveedorCelular+'|'+p.ProveedorTelefono+'|'+p.ProveedorCorreo+'|'+
 p.ProveedorDireccion+'|'+p.ProveedorEstado
 from Proveedor p
 order by p.ProveedorId desc
 for xml path('')),1,1,'')),'~')
 end
GO
/****** Object:  StoredProcedure [dbo].[listarCaja]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[listarCaja]
as
begin
select
'CajaId|FechaApertura|FechaCierre|MontoIniSol|Ingresos|Tarjetas|Salidas|Total|Encargado|Usuario|Estado|Observaciones¬100|100|100|100|100|100|100|100|100|100|100|100¬String|String|String|String|String|String|String|String|String|String|String|String¬'+ 
isnull((select STUFF((select '¬'+convert(varchar,c.CajaId)+'|'+
(Convert(char(10),c.CajaFecha,103))+' '+ IsNull(SUBSTRING(convert(varchar,c.CajaFecha,114),1,8),'')+'|'+
--(Convert(char(10),c.CajaCierre,103))+' '+ IsNull(SUBSTRING(convert(varchar,c.CajaCierre,114),1,8),'')+'|'+
c.CajaCierre+'|'+
CONVERT(VarChar(50), cast(c.MontoIniSOl as money ), 1)+'|'+
CONVERT(VarChar(50), cast(c.CajaIngresos as money ), 1)+'|'+
CONVERT(VarChar(50), cast(c.CajaDeposito as money ), 1)+'|'+
CONVERT(VarChar(50), cast(c.CajaSalidas as money ), 1)+'|'+
CONVERT(VarChar(50), cast(c.CajaTotal as money ), 1)+'|'+
c.CajaEncargado+'|'+
c.CajaUsuario+'|'+
c.CajaEstado+'|'+
c.Observacion
from Caja c
where Month(c.CajaFecha)=Month(GETDATE()) and year(c.CajaFecha)=year(GETDATE())
order by c.CajaId desc
FOR XML path ('')),1,1,'')),'~')
end
GO
/****** Object:  StoredProcedure [dbo].[listarCajaFecha]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[listarCajaFecha]
@fechainicio date,
@fechafin date
as
begin
select
'CajaId|FechaApertura|FechaCierre|MontoIniSol|Ingresos|Tarjetas|Salidas|Total|Encargado|Usuario|Estado|Observaciones¬100|100|100|100|100|100|100|100|100|100|100|100¬String|String|String|String|String|String|String|String|String|String|String|String¬'+ 
isnull((select STUFF((select '¬'+convert(varchar,c.CajaId)+'|'+
(Convert(char(10),c.CajaFecha,103))+' '+ IsNull(SUBSTRING(convert(varchar,c.CajaFecha,114),1,8),'')+'|'+
(Convert(char(10),c.CajaCierre,103))+' '+ IsNull(SUBSTRING(convert(varchar,c.CajaCierre,114),1,8),'')+'|'+
CONVERT(VarChar(50), cast(c.MontoIniSOl as money ), 1)+'|'+
CONVERT(VarChar(50), cast(c.CajaIngresos as money ), 1)+'|'+
CONVERT(VarChar(50), cast(c.CajaDeposito as money ), 1)+'|'+
CONVERT(VarChar(50), cast(c.CajaSalidas as money ), 1)+'|'+
CONVERT(VarChar(50), cast(c.CajaTotal as money ), 1)+'|'+
c.CajaEncargado+'|'+
c.CajaUsuario+'|'+
c.CajaEstado+'|'+
c.Observacion
from Caja c
where (Convert(char(10),c.CajaFecha,101) BETWEEN @fechainicio AND @fechafin)
order by c.CajaId desc
FOR XML path ('')),1,1,'')),'~')
end
GO
/****** Object:  StoredProcedure [dbo].[listarCompras]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[listarCompras]   
as  
begin  
select   
'CompraId|Correlativo|Compania|FechaRegistro|Computo|Emision|RazonSocial|RUC|Documento|Serie|Numero|Condicion|Moneda|TipoCambio|DiasPlazo|FechaPago|TipoIgv|ValorVenta|Descuento|SubTotal|Igv|Total|SaldoDoc|Usuario|CompaniaNombre|Estado|ProveedorId|DocumentoId|Asociado|OBS|TipoSunat|Concepto|Percepcion¬100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100¬String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String¬'+  
isnull((select STUFF ((select '¬'+convert(varchar,c.CompraId)+'|'+  
c.CompraCorrelativo+'|'+  
convert(varchar,c.CompaniaId)+'|'+  
(IsNull(convert(varchar,c.CompraRegistro,103),'')+' '+ IsNull(SUBSTRING(convert(varchar,c.CompraRegistro,114),1,8),''))+'|'+  
Convert(char(10),c.CompraComputo,103)+'|'+  
Convert(char(10),c.CompraEmision,103)+'|'+  
p.ProveedorRazon+'|'+  
p.ProveedorRuc+'|'+  
c.TipoCodigo+'|'+  
c.CompraSerie+'|'+  
c.CompraNumero+'|'+  
c.CompraCondicion+'|'+  
c.CompraMoneda+'|'+  
convert(varchar,CompraTipoCambio)+'|'+  
convert(varchar,c.CompraDias)+'|'+  
Convert(char(10),c.CompraFechaPago,103)+'|'+  
c.CompraTipoIgv+'|'+  
CONVERT(VarChar(50), cast(c.CompraValorVenta as money ), 1)+'|'+  
CONVERT(VarChar(50), cast(c.CompraDescuento as money ), 1)+'|'+  
CONVERT(VarChar(50), cast(c.CompraSubtotal as money ), 1)+'|'+  
CONVERT(VarChar(50), cast(c.CompraIgv as money ), 1)+'|'+  
CONVERT(VarChar(50), cast(c.CompraTotal as money ), 1)+'|'+  
CONVERT(VarChar(50), cast(c.compraSaldo as money ), 1)+'|'+  
c.CompraUsuario+'|'+  
co.CompaniaRazonSocial+'|'+  
c.CompraEstado+'|'+  
convert(varchar,c.ProveedorId)+'|'+  
t.TipoDescripcion+'|'+  
c.CompraAsociado+'|'+  
CompraOBS+'|'+  
convert(varchar,CompraTipoSunat)+'|'+  
CompraConcepto+'|'+  
convert(varchar,c.CompraPercepcion)  
from Compras c  
inner join Proveedor p  
on p.ProveedorId=c.ProveedorId  
inner join Compania co  
on co.CompaniaId=c.CompaniaId  
inner join TipoComprobante t  
on t.TipoCodigo=c.TipoCodigo  
where c.TipoCodigo<>'07' and(Month(c.CompraComputo)=Month(GETDATE()) and year(c.CompraComputo)=year(GETDATE()))  
order by c.CompraId desc  
for xml path('')),1,1,'')),'~')  
end
GO
/****** Object:  StoredProcedure [dbo].[listarDetaLetra]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[listarDetaLetra] @LetraId numeric(38)
as
begin
select d.DetalleId,d.LetraId,d.LetraCanje,d.LetraDias,(Convert(char(10),d.LetraVencimiento,103)) as Vencimeinto,
CONVERT(VarChar(50), cast(d.DetalleSaldo as money ), 1) as SaldoLetra,
CONVERT(VarChar(50), cast(d.DetalleMonto as money ), 1) as DetalleMonto,d.DetalleEstado
from DetalleLetra d
where d.LetraId=@LetraId 
order by 1 asc
end
GO
/****** Object:  StoredProcedure [dbo].[listarDetaliquida]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[listarDetaliquida] 
@LiquidacionId varchar(38)  
as  
begin  
select
'Id|IdDeuda|Documento|RazonSocial|Saldo|Moneda|TipoCambio|EfectivoSoles|EfectivoDolar|DepositoSoles|DepositoDolar|EntidadBancaria|NroOperacion|Acuenta|Usuario|FechaOperacion|SaldoActual|Concepto¬90|90|90|90|90|90|90|90|90|90|90|90|90|90|90|90|90|90¬String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String¬'+  
isnull((select STUFF((select '¬'+convert(varchar,d.DetalleId)+'|'+
convert(varchar,d.CompraId)+'|'+
d.Numero+'|'+  
d.Proveedor+'|'+
CONVERT(VarChar(50), cast(d.SaldoDocu as money ), 1)+'|'+
d.Moneda+'|'+
convert(varchar,d.TipoCambio)+'|'+
convert(varchar,d.EfectivoSoles)+'|'+  
convert(varchar,d.EfectivoDolar)+'|'+
convert(varchar,d.DepositoSoles)+'|'+
convert(varchar,d.DepositoDolar)+'|'+
d.EntidadBanco+'|'+
d.NroOperacion+'|'+
CONVERT(VarChar(50), cast(d.AcuentaGeneral as money ), 1)+'||'+ 
d.FechaPago+'|'+
CONVERT(VarChar(50), cast(d.SaldoActual as money ), 1)+'|'+
d.Concepto  
from DetalleLiquida d  
where d.LiquidacionId=@LiquidacionId  
order by d.DetalleId asc
FOR XML path ('')),1,1,'')),'~') 
end
GO
/****** Object:  StoredProcedure [dbo].[listarGuia]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[listarGuia] 
@Concepto varchar(60)
as
begin
select g.GuiaId,g.GuiaNumero,g.GuiaMotivo,g.GuiaRegistro,g.GuiaFechaTraslado,
g.GuiaDestinatario,g.GuiaRucDes,g.GuiaAlmacen,g.GuiaPartida,g.GuiaLLegada,g.GuiaTramsporte,g.GuiaTransporteRuc,g.GuiaChofer,
g.GuiaPlaca,g.GuiaConstancia,g.GuiaLicencia,g.GuiaUsuario,CONVERT(VarChar(50),cast(g.GuiaTotal as money ), 1) as Total,g.GuiaConcepto as Concepto,
g.ClienteId,g.GuiaEstado,g.GuiaTelefono as Telefono
from GuiaRemision g
where g.GuiaConcepto=@Concepto and (Month(g.GuiaRegistro)=Month(GETDATE()) and YEAR(g.GuiaRegistro)=YEAR(GETDATE()))
order by 1 desc
end
GO
/****** Object:  StoredProcedure [dbo].[listarGuiaFecha]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[listarGuiaFecha]  @fechainicio date,@fechafin date,@Concepto varchar(60)
as
begin
select g.GuiaId,g.GuiaNumero,g.GuiaMotivo,g.GuiaRegistro,g.GuiaFechaTraslado,
g.GuiaDestinatario,g.GuiaRucDes,g.GuiaAlmacen,g.GuiaPartida,g.GuiaLLegada,g.GuiaTramsporte,g.GuiaTransporteRuc,g.GuiaChofer,
g.GuiaPlaca,g.GuiaConstancia,g.GuiaLicencia,g.GuiaUsuario,CONVERT(VarChar(50),cast(g.GuiaTotal as money ), 1) as Total,g.GuiaConcepto as Concepto,
g.ClienteId,g.GuiaEstado
from GuiaRemision g
where (Convert(char(10),g.GuiaFechaTraslado,103) BETWEEN @fechainicio AND @fechafin) and g.GuiaConcepto=@Concepto
order by 1 desc
end
GO
/****** Object:  StoredProcedure [dbo].[listarKardex]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[listarKardex]   
@IdProducto numeric(20)  
as  
begin  
 select   
 'KardexId|IdProducto|FechaMovimiento|Motivo|Documento|StockInicial|CantidadIngre|CantidadSali|PrecioCosto|StockFinal|Concepto|Responsable¬100|100|145|200|145|115|115|115|115|115|100|160¬String|String|String|String|String|String|String|String|String|String|String|String¬'+  
 isnull((select STUFF ((select '¬'+convert(varchar,k.KardexId)+'|'+CONVERT(varchar,k.IdProducto)+'|'+  
 (IsNull(convert(varchar,k.KardexFecha,103),'')+' '+ IsNull(SUBSTRING(convert(varchar,k.KardexFecha,114),1,8),''))+'|'+  
 k.KardexMotivo+'|'+k.KardexDocumento+'|'+  
 CONVERT(VarChar(50), cast(k.StockInicial as money ), 1)+'|'+  
 CONVERT(VarChar(50), cast(k.CantidadIngreso as money ), 1)+'|'+  
 CONVERT(VarChar(50), cast(k.CantidadSalida as money ), 1)+'|'+  
 CONVERT(VarChar(50), cast(k.PrecioCosto as money ), 1)+'|'+  
 CONVERT(VarChar(50), cast(k.StockFinal as money ), 1)+'|'+  
 K.KadexConcepto+'|'+k.Usuario  
 from Kardex k with(nolock)  
 where k.IdProducto=@IdProducto and (Month(k.KardexFecha)=Month(GETDATE()) and YEAR(k.kardexFecha)=year(getdate()))  
 order by k.KardexId desc  
 for xml path('')),1,1,'')),'~')  
end
GO
/****** Object:  StoredProcedure [dbo].[listarKardexFecha]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[listarKardexFecha]   
@Id numeric(20),  
@fechainicio date,
@fechafin date  
as  
begin  
select
 'KardexId|IdProducto|FechaMovimiento|Motivo|Documento|StockInicial|CantidadIngre|CantidadSali|PrecioCosto|StockFinal|Concepto|Responsable¬100|100|145|200|145|115|115|115|115|115|100|160¬String|String|String|String|String|String|String|String|String|String|String|String¬'+ 
 isnull((select STUFF ((select '¬'+convert(varchar,k.KardexId)+'|'+CONVERT(varchar,k.IdProducto)+'|'+  
 (IsNull(convert(varchar,k.KardexFecha,103),'')+' '+ IsNull(SUBSTRING(convert(varchar,k.KardexFecha,114),1,8),''))+'|'+  
 k.KardexMotivo+'|'+k.KardexDocumento+'|'+  
 CONVERT(VarChar(50), cast(k.StockInicial as money ), 1)+'|'+  
 CONVERT(VarChar(50), cast(k.CantidadIngreso as money ), 1)+'|'+  
 CONVERT(VarChar(50), cast(k.CantidadSalida as money ), 1)+'|'+  
 CONVERT(VarChar(50), cast(k.PrecioCosto as money ), 1)+'|'+  
 CONVERT(VarChar(50), cast(k.StockFinal as money ), 1)+'|'+  
 K.KadexConcepto+'|'+k.Usuario  
 from Kardex k  
 where k.IdProducto=@Id and (Convert(char(10),k.KardexFecha,101) BETWEEN @fechainicio AND @fechafin)  
 order by k.KardexId desc  
 for xml path('')),1,1,'')),'~')  
end
GO
/****** Object:  StoredProcedure [dbo].[listarLetraFecha]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[listarLetraFecha] @fechainicio date,@fechafin date
as
begin
select l.LetraId, l.ProveedorId,p.ProveedorRazon,l.LetraFechaReg,(Convert(char(10),l.LetraFechaGiro,103)) as FechaGiro,
l.LetraMoneda as Moneda,CONVERT(VarChar(50), cast(l.LetraSaldo as money ), 1)as SaldoLetras,CONVERT(VarChar(50), cast(l.LetraTotal as money ), 1)as TotalLetras,l.LetraUsuario,l.LetraEstado as Estado
from Letra l
inner join Proveedor p
on p.ProveedorId=l.ProveedorId
where (Convert(char(10),l.LetraFechaGiro,103) BETWEEN @fechainicio AND @fechafin)
order by 1 desc
end
GO
/****** Object:  StoredProcedure [dbo].[listarLetras]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[listarLetras]
as
begin
select l.LetraId, l.ProveedorId,p.ProveedorRazon,l.LetraFechaReg,(Convert(char(10),l.LetraFechaGiro,103)) as FechaGiro,l.LetraMoneda as Moneda,
CONVERT(VarChar(50), cast(l.LetraSaldo as money ), 1)as SaldoLetras,CONVERT(VarChar(50), cast(l.LetraTotal as money ), 1)as TotalLetras,
l.LetraUsuario,l.LetraEstado as Estado,l.CompaniaId
from Letra l
inner join Proveedor p
on p.ProveedorId=l.ProveedorId
where year(LetraFechaGiro)=YEAR(GETDATE())
order by 1 desc
end
GO
/****** Object:  StoredProcedure [dbo].[listarPersonal]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[listarPersonal]
@Estado varchar(20) = 'ACTIVO' 
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        p.PersonalId,
        p.PersonalNombres,
        p.PersonalApellidos,
        p.AreaId,
        p.PersonalCodigo,
        p.PersonalNacimiento,
        p.PersonalIngreso,
        p.PersonalDNI,
        p.PersonalDireccion,
        p.PersonalTelefono,
        p.PersonalEmail,
        p.PersonalEstado,
        p.PersonalImagen,
        p.CompaniaId
    FROM Personal p WITH (NOLOCK)
    WHERE (@Estado IS NULL) OR (p.PersonalEstado = @Estado)
    ORDER BY p.PersonalApellidos ASC;
END
GO
/****** Object:  StoredProcedure [dbo].[listarRenta]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[listarRenta]
as
begin
select 
'ID|Compania|Anno|Mes|Declaracion|Igv|Renta|SaldoIgv|SaldoRenta|InteresIgv|InteresRenta|TotalIgv|TotalRenta|FormaPago|FechaPago|Entidad|NroOperacion|PagoTotal¬
80|90|80|80|145|120|120|120|120|120|120|120|110|70|120|100|100|120¬String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String¬'+
isnull((select STUFF((select '¬'+convert(varchar,r.RentaId)+'|'+convert(varchar,r.CompaniaId)+'|'+convert(varchar,r.RentaANNO)+'|'+
convert(varchar,r.RentaMes)+'|'+dbo.MesNombre(r.RentaMes)+' '+convert(varchar,r.RentaANNO)+'|'+
CONVERT(VarChar(50), cast((r.IGV) as money ), 1)+'|'+CONVERT(VarChar(50), cast((r.Renta) as money ), 1)+'|'+
CONVERT(VarChar(50), cast((r.SaldoIGV) as money ), 1)+'|'+CONVERT(VarChar(50), cast((r.SaldoRenta) as money ), 1)+'|'+
CONVERT(VarChar(50), cast((r.InteresIgv) as money ), 1)+'|'+CONVERT(VarChar(50), cast((r.InteresRenta) as money ), 1)+'|'+
CONVERT(VarChar(50), cast((r.TributoIgv) as money ), 1)+'|'+CONVERT(VarChar(50), cast((r.TributoRenta) as money ), 1)+'|'+
CONVERT(char(1),r.FormaPago)+'|'+convert(varchar,r.FechaCancelacion,103)+'|'+r.EntidadBancaria+'|'+r.NroOperacion+'|'+
CONVERT(VarChar(50), cast((r.PagoTotal) as money ), 1)
from RentaMensual r
where year(r.FechaCancelacion)=year(getdate())
order by r.RentaId desc
for xml path('')),1,1,'')),'~')
end
GO
/****** Object:  StoredProcedure [dbo].[listarRentaFecha]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[listarRentaFecha] 
@fechainicio date,@fechafin date
as
begin
select 
'ID|Compania|Anno|Mes|Declaracion|Igv|Renta|SaldoIgv|SaldoRenta|InteresIgv|InteresRenta|TotalIgv|TotalRenta|FormaPago|FechaPago|Entidad|NroOperacion|PagoTotal¬
80|90|80|80|145|120|120|120|120|120|120|120|110|70|120|100|100|120¬String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String¬'+
isnull((select STUFF((select '¬'+convert(varchar,r.RentaId)+'|'+convert(varchar,r.CompaniaId)+'|'+convert(varchar,r.RentaANNO)+'|'+
convert(varchar,r.RentaMes)+'|'+dbo.MesNombre(r.RentaMes)+' '+convert(varchar,r.RentaANNO)+'|'+
CONVERT(VarChar(50), cast((r.IGV) as money ), 1)+'|'+CONVERT(VarChar(50), cast((r.Renta) as money ), 1)+'|'+
CONVERT(VarChar(50), cast((r.SaldoIGV) as money ), 1)+'|'+CONVERT(VarChar(50), cast((r.SaldoRenta) as money ), 1)+'|'+
CONVERT(VarChar(50), cast((r.InteresIgv) as money ), 1)+'|'+CONVERT(VarChar(50), cast((r.InteresRenta) as money ), 1)+'|'+
CONVERT(VarChar(50), cast((r.TributoIgv) as money ), 1)+'|'+CONVERT(VarChar(50), cast((r.TributoRenta) as money ), 1)+'|'+
CONVERT(char(1),r.FormaPago)+'|'+convert(varchar,r.FechaCancelacion,103)+'|'+r.EntidadBancaria+'|'+r.NroOperacion+'|'+
CONVERT(VarChar(50), cast((r.PagoTotal) as money ), 1)
from RentaMensual r
where (Convert(char(10),r.FechaCancelacion,103) BETWEEN @fechainicio AND @fechafin)
order by r.RentaMes desc
for xml path('')),1,1,'')),'~')
end
GO
/****** Object:  StoredProcedure [dbo].[listarSaldos]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[listarSaldos] 
@ClienteId numeric(20)
as
begin
select
'DetalleId|NroNota|Idproducto|Codigo|Descripcion|Cantidad|Saldo|UM|Stock|UnidadM|CantInicial|critico|ClienteId|PrecioVenta|valorUM¬100|90|100|100|450|100|100|90|100|100|100|100|100|100|100¬String|String|String|String|String|String|String|String|String|String|String|String|String|String|String¬'+
isnull((select STUFF ((select '¬'+convert(varchar,d.DetalleId)+'|'+convert(varchar,d.NotaId)+'|'+
convert(varchar,d.IdProducto)+'|'+p.ProductoCodigo+'|'+d.DetalleDescripcion+'|'+''+'|'+
convert(varchar(50),cast(d.CantidadSaldo as money),1)+'|'+d.DetalleUm+'|'+
convert(varchar(50),cast(p.ProductoCantidad as money),1)+'|'+p.ProductoUM+'|'+
convert(varchar(50),cast(d.DetalleCantidad as money),1)+'|'+
convert(varchar,p.ValorCritico)+'|'+convert(varchar,n.ClienteId)+'|'+
convert(varchar,d.DetallePrecio)+'|'+convert(varchar,d.ValorUM)
from DetallePedido d
inner join NotaPedido n
on n.NotaId=d.NotaId
inner join Producto p
on p.IdProducto=d.IdProducto
where n.ClienteId=@ClienteId and d.cantidadSaldo>0
order by n.NotaId desc,d.DetalleId asc
for xml path('')),1,1,'')),'~')+'_'+
isnull((select STUFF ((select '¬' +CONVERT(VarChar(50), cast(sum(n.NotaSaldo) as money ), 1)
from NotaPedido n
where n.ClienteId=@ClienteId and n.NotaEntrega='POR ENTREGAR'
for xml path('')),1,1,'')),'0')
end
GO
/****** Object:  StoredProcedure [dbo].[listarSaldosB]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[listarSaldosB] @NotaId numeric(38)
as
begin
select d.DetalleId,d.NotaId,d.IdProducto,p.ProductoCodigo as Codigo,d.DetalleDescripcion as Descripcion,
d.CantidadSaldo as CantidadSaldo,p.ProductoCantidad as Stock,substring(p.ProductoUM,1,3) as UM,d.DetalleCantidad as CantidadInicial,
p.ValorCritico,n.ClienteId,d.DetallePrecio as PrecioCosto
from DetallePedido d
inner join NotaPedido n
on n.NotaId=d.NotaId
inner join Producto p
on p.IdProducto=d.IdProducto
where d.NotaId=@NotaId and d.cantidadSaldo>0
order by 1 asc
end
GO
/****** Object:  StoredProcedure [dbo].[listarSublinea]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[listarSublinea]  
as  
begin  
select 
'Id|Categoria|CodigoSunat¬90|100|100¬String|String|String¬'+
isnull((select STUFF ((select '¬'+convert(varchar,s.IdSubLinea)+'|'+
s.NombreSublinea+'|'+s.CodigoSunat  
from Sublinea s  
order by s.NombreSublinea asc
for xml path('')),1,1,'')),'~')  
end
GO
/****** Object:  StoredProcedure [dbo].[listarUM]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[listarUM] 
@IdProducto numeric(20)
as
begin
select
'IdUm|IdProducto|UNIDAD M|Valor|PreVenta|PreVentaB|PreCosto¬80|80|110|100|100|100|100¬String|String|String|Decimal|String|String|String¬'+
isnull((select STUFF ((select '¬'+convert(varchar,m.IdUm)+'|'+CONVERT(varchar,m.IdProducto)+'|'+m.UMDescripcion+'|'+
convert(varchar,m.ValorUM)+'|'+CONVERT(VarChar(50),cast(m.PrecioVenta as money ), 1)+'|'+CONVERT(VarChar(50), cast(m.PrecioVentaB as money ), 1)+'|'+
CONVERT(varchar(50),m.PrecioCosto)
from UnidadMedida m
where m.IdProducto=@IdProducto
order by m.ValorUM asc
for xml path('')),1,1,'')),'~')
end
GO
/****** Object:  StoredProcedure [dbo].[ListarUsuario]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ListarUsuario]  
@Estado varchar(20) = 'ACTIVO'  
AS  
BEGIN  
    SET NOCOUNT ON;  
    SELECT  
        U.UsuarioID,  
        U.PersonalId,  
        CONCAT(P.PersonalNombres, ' ', P.PersonalApellidos) AS Nombre,  
        U.UsuarioAlias,  
        dbo.desincrectar(U.UsuarioClave) as UsuarioClave,  
        A.AreaNombre AS Area,  
        U.UsuarioFechaReg AS Fecha,  
        U.UsuarioEstado AS Estado  
    FROM Usuarios U  
    INNER JOIN Personal P ON U.PersonalId = P.PersonalId  
    INNER JOIN Area A ON P.AreaId = A.AreaId  
    WHERE (@Estado IS NULL) OR (U.UsuarioEstado = @Estado)  
    ORDER BY U.UsuarioID;  
END  
GO
/****** Object:  StoredProcedure [dbo].[listasMarca]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[listasMarca]
as
begin
select
isnull((select STUFF((select '¬'+ p.ProductoUM
from Producto p
group by ProductoUM
order by p.ProductoUM asc
FOR XML PATH('')),1,1,'')),'~')
end
GO
/****** Object:  StoredProcedure [dbo].[listaTempoCompra]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[listaTempoCompra]  
@UsuarioID int  
as  
begin  
select  
'Id|IdProducto|Codigo|Descripcion|UM|Cantidad|PrecioCosto|Descuento|Importe|ValorUM|Estado|AplicaINV¬100|100|100|420|80|90|100|100|110|100|100|100¬String|String|String|String|String|String|String|String|String|String|String|String¬'+  
isnull((select STUFF ((select '¬'+convert(varchar,t.TemporalId)+'|'+convert(varchar,t.IdProducto)+'|'+  
t.DetalleCodigo+'|'+t.Descripcion+'|'+t.DetalleUM+'|'+  
CONVERT(VarChar(50),cast(t.DetalleCantidad as money ), 1)+'|'+  
convert(varchar,t.PrecioCosto)+'|'+convert(varchar,t.DetalleDescuento)  
+'|'+convert(varchar,t.DetalleImporte)+'|'+CONVERT(varchar,t.ValorUM)+'|'+  
t.DetalleEstado+'|'+p.AplicaINV  
from TemporalCompra t   
inner join Producto p   
on p.IdProducto=t.IdProducto   
where t.UsuarioID=@UsuarioID  
order by t.Posicion asc  
for xml path('')),1,1,'')),'~')+'['+  
'IdUm|IdProducto|UnidadM|Valor|Costo¬100|100|100|100|100¬String|String|String|String|String¬'+  
isnull((select STUFF ((select '¬'+convert(varchar,u.IdUm)+'|'+convert(varchar,u.IdProducto)+'|'+  
u.UMDescripcion+'|'+CONVERT(VarChar(50), cast(u.ValorUM as money ), 1)+'|'+  
convert(varchar,t.PrecioCosto)  
from UnidadMedida u  
inner join TemporalCompra t  
on t.IdProducto=u.IdProducto  
where t.UsuarioID=@UsuarioID  
order by u.ValorUM asc  
for xml path('')),1,1,'')),'~')  
end
GO
/****** Object:  StoredProcedure [dbo].[listaTempoLiquida]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[listaTempoLiquida] 
@UsuarioId varchar(20)  
as  
begin  
select
'Id|IdDeuda|Documento|RazonSocial|Saldo|Moneda|TipoCambio|EfectivoSoles|EfectivoDolar|DepositoSoles|DepositoDolar|EntidadBancaria|NroOperacion|Acuenta|Usuario|FechaOperacion|SaldoActual|Concepto¬90|90|90|90|90|90|90|90|90|90|90|90|90|90|90|90|90|90¬String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String¬'+
isnull((select STUFF((select '¬'+convert(varchar,t.TemporalId)+'|'+
convert(varchar,t.IdDeuda)+'|'+
t.Numero+'|'+
t.Proveedor+'|'+ 
CONVERT(VarChar(50), cast(t.SaldoDocu as money ), 1)+'|'+
t.Moneda+'|'+
convert(varchar,t.TipoCambio)+'|'+
convert(varchar,t.EfectivoSoles)+'|'+
convert(varchar,t.EfectivoDolar)+'|'+
convert(varchar,t.DepositoSoles)+'|'+
convert(varchar,t.DepositoDolar)+'|'+
t.EntidadBanco+'|'+
t.NroOperacion+'|'+
CONVERT(VarChar(50), cast(t.AcuentaGeneral as money ), 1)+'|'+ 
convert(varchar,t.UsuarioId)+'|'+
t.TemporalFecha+'|'+
CONVERT(VarChar(50), cast(t.SaldoDocu - t.AcuentaGeneral as money ), 1)+'|'+
t.Concepto  
from TemporalLiquida t  
where UsuarioId=@UsuarioId  
order by t.TemporalId asc
FOR XML path ('')),1,1,'')),'~')     
end
GO
/****** Object:  StoredProcedure [dbo].[listaTempoLiVenta]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[listaTempoLiVenta]     
@UsuarioId Int    
as    
begin    
select  
'ID|DocuId|Documento|RazonSocial|Saldo|Moneda|TipoCambio|EfectivoSoles|EfectivoDolar|DepositoSoles|DepositoDolar|EntidadBancaria|NroOperacion|Acuenta|Usuario|FechaOperacion|SaldoActual|NotaId¬90|90|90|90|90|90|90|90|90|90|90|90|90|90|90|90|90|90¬String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String¬'+  
isnull((select STUFF((select '¬'+convert(varchar,t.TemporalId)+'|'+  
convert(varchar,t.DocuId)+'|'+   
n.NotaSerie+'-'+n.NotaNumero+'|'+    
c.ClienteRazon+'|'+    
CONVERT(VarChar(50),cast(t.SaldoDocu as money ), 1)+'|SOLES|'+  
convert(varchar,t.TipoCambio)+'|'+    
convert(varchar,t.EfectivoSoles)+'|'+  
convert(varchar,t.EfectivoDolar)+'|'+  
convert(varchar,t.DepositoSoles)+'|'+  
convert(varchar,t.DepositoDolar)+'|'+  
convert(varchar,t.EntidadBanco)+'|'+    
convert(varchar,t.NroOperacion)+'|'+  
CONVERT(VarChar(50),cast(t.AcuentaGeneral as money ), 1)+'|'+    
convert(varchar,t.UsuarioId)+'|'+t.TemporalFecha+'|'+  
CONVERT(VarChar(50),cast(t.SaldoDocu - t.AcuentaGeneral as money ), 1)+'|'+  
convert(varchar,t.NotaId)    
from TemporalLiVenta t    
inner join NotaPedido n (nolock)    
on n.NotaId=t.NotaId    
inner join Cliente c (nolock)  
on c.ClienteId=n.ClienteId    
where t.UsuarioId=@UsuarioId    
order by t.TemporalId asc  
for xml path('')),1,1,'')),'~')   
end
GO
/****** Object:  StoredProcedure [dbo].[listaTipoCambio]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[listaTipoCambio]
as
begin
select
'ID|Fecha|COMPRA|VENTA|EMPRESA¬90|110|108|108|117¬String|String|String|String|String¬'+
isnull((select STUFF((select '¬'+ convert(varchar,t.IdTipo),+'|'+
(Convert(char(10),t.TipoFecha,103))+'|'+convert(varchar,t.TipoCompra)+'|'+
convert(varchar,t.TipoVenta)+'|'+
convert(varchar,t.TipoEmpresa) 
from TipoCambio t 
where MONTH(t.TipoFecha)=MONTH(GETDATE()) and YEAR(t.TipoFecha)=YEAR(GETDATE()) 
order by t.TipoFecha desc
for xml path('')),1,1,'')),'~')
end
GO
/****** Object:  StoredProcedure [dbo].[LuisDuenas]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[LuisDuenas]    
as    
begin    
select 'Categoria|Descipcion|Stock|Unidad|Costo¬295|470|105|105|105¬'+    
isnull((select STUFF((select'¬'+s.NombreSublinea+'|'+p.ProductoNombre+'|'+    
CONVERT(VarChar(50), cast(p.ProductoCantidad as money ), 1)+'|'+    
p.ProductoUM+'|'+CONVERT(VarChar(50), cast(p.ProductoCosto as money ), 1)    
from Producto p    
inner join Sublinea s    
on s.IdSubLinea=p.IdSubLinea    
where p.ProductoEstado='BUENO' and  p.ProductoCantidad < = p.ValorCritico and p.AplicaINV='S'   
order by s.NombreSublinea,p.ProductoNombre asc    
for xml path('')),1,1,'')),'~')    
end  
GO
/****** Object:  StoredProcedure [dbo].[LuisDuenasB]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[LuisDuenasB] 
@fechainicio date,
@fechafin date
as
begin
select 'Fecha|Vendedor|Descripcion|UM|Cantidad|PrecioUni|Costo|GXUnidad|Ganancia¬130|150|400|65|110|110|110|110|115¬'+
(select STUFF((select '¬'+
(IsNull(convert(varchar,n.NotaFecha,103),'')+' '+ IsNull(SUBSTRING(convert(varchar,n.NotaFecha,114),1,8),''))
+'|'+n.NotaUsuario+'|'+
d.DetalleDescripcion+'|'+d.DetalleUm+'|'+
CONVERT(VarChar(50), cast((d.DetalleCantidad) as money ), 1)+'|'+
CONVERT(VarChar(50), cast(d.DetallePrecio as money ), 1)+'|'+
CONVERT(VarChar(50), cast(d.DetalleCosto as money ), 1) +'|'+
CONVERT(VarChar(50), cast((d.DetallePrecio-d.DetalleCosto) as money ), 1)+'|'+
CONVERT(VarChar(50), cast(((d.DetallePrecio-d.DetalleCosto)* d.DetalleCantidad) as money ), 1)
	 from DetallePedido d (noLOCK) 
	 inner join NotaPedido n (noLOCK)
	 on n.NotaId=d.NotaId
	 where (Convert(char(10),n.NotaFecha,101) BETWEEN @fechainicio AND @fechafin)  
	 and n.NotaEstado='CANCELADO'
	 order by n.NotaFecha desc
	 for xml path('')),1,1,''))
 end
GO
/****** Object:  StoredProcedure [dbo].[MRDuenas]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[MRDuenas]
as
begin
select 'SubLineas|Productos_'+
isnull((select STUFF((select '¬'+ convert(varchar,s.IdSubLinea)+'|'+s.NombreSublinea
from Sublinea s
for XMl path('')),1,1,'')),'~')+'_'+
'Descripcion|Cantidad|UM|PreVenta|PreVentaB|PreCosto¬400|115|80|115|115|115¬'+
isnull((select STUFF((select '¬'+p.ProductoNombre+'|'+
CONVERT(VarChar(max), cast(p.ProductoCantidad as money ), 1)
+'|'+p.ProductoUM+'|'+CONVERT(varchar,p.ProductoVenta)+'|'+
CONVERT(varchar,p.ProductoVentaB)+'|'+
CONVERT(varchar,p.ProductoCosto)+'|'+
convert(varchar,p.IdSubLinea)
from Producto p
for XMl path('')),1,1,'')),'~')
end
GO
/****** Object:  StoredProcedure [dbo].[MRDuenasB]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[MRDuenasB]  
as  
begin  
select 'Id|SubLinea|Selec¬0|235|50¬String|String|Boolean¬'+  
isnull((select STUFF((select '¬'+ convert(varchar,s.IdSubLinea)+'|'+s.NombreSublinea  
+'|'+convert(char(1),0)  
from Sublinea s  
for XMl path('')),1,1,'')),'~')+'['+  
'ID|Codigo|Descripcion|Inventario|UM|Cantidad|PreVenta|PreCosto¬90|90|90|80|90|90|90|90¬String|String|String|String|String|String|String|String¬'+  
isnull((select STUFF((select '¬'+convert(varchar,p.IdProducto)+'|'+  
p.ProductoCodigo+'|'+p.ProductoNombre+'||'+p.ProductoUM+'|'+  
CONVERT(varchar,p.ProductoCantidad)+'|'+  
CONVERT(varchar,p.ProductoVenta)+'|'+  
CONVERT(varchar,p.ProductoCosto)+'|'+  
convert(varchar,p.IdSubLinea)  
from Producto p  
where p.ProductoEstado='BUENO'  
order by p.IdSubLinea asc,p.ProductoNombre asc  
for XMl path('')),1,1,'')),'~')  
end
GO
/****** Object:  StoredProcedure [dbo].[permisoElimina]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[permisoElimina]
@Codigo varchar(60)
as
begin
select top 1 (((SUBSTRING(p.PersonalNombres+' ',1,CHARINDEX(' ',p.PersonalNombres+' ')-1)))+' '+ ((SUBSTRING(p.PersonalApellidos+' ',1,CHARINDEX(' ',p.PersonalApellidos+' ')-1))))as USUARIO 
from Personal p
where PersonalCodigo=@Codigo and (AreaId=6 or AreaId=7 or AreaId=12)
end
GO
/****** Object:  StoredProcedure [dbo].[pruebaB]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[pruebaB]
as
begin
select
'DetalleId|UM¬100|120¬String|String¬'+
(select STUFF((select '¬'+convert(varchar,d.DetalleId)+'|'+
p.ProductoUM
from DetalleGuia d
inner join Producto p
on p.IdProducto=d.IdProducto
for XML path('')),1,1,''))
end
GO
/****** Object:  StoredProcedure [dbo].[reporteGanancia]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[reporteGanancia]   
@anno int  
as  
begin  
SET LANGUAGE Spanish;select 'Numero|Mes|Ventas|G_Ventas|Gastos|G_Liquida¬80|100|110|110|105|110¬String|String|String|String|String|String¬'+  
isnull((select STUFF ((select '¬'+convert(varchar,isnull(a.Numero,g.Numero))+'|'+convert(varchar,ISNULL(a.Mes,g.Mes))+'|'+  
 CONVERT(VarChar(50), cast(isnull(a.Ventas,0) as money ), 1)+'|'+  
 CONVERT(VarChar(50), cast(isnull(a.Ganancia,0)as money ), 1)+'|'+  
 CONVERT(VarChar(50), cast(isnull(g.Gastos,0) as money ), 1)+'|'+  
 CONVERT(VarChar(50), cast((isnull(a.Ganancia,0)-isnull(g.Gastos,0)) as money ), 1)  
 from   
(select month(n.NotaFecha) as Numero,DATENAME(month,n.NotaFecha) as Mes,sum(n.NotaPagar) as Ventas,  
sum(n.NotaGanancia)- SUM(n.NotaDescuento)as Ganancia --GANANCIA  
from   
 NotaPedido n(noLOCK)   
 where n.NotaEstado='CANCELADO' and YEAR(n.NotaFecha)=@anno  
 group by month(n.NotaFecha),DATENAME(month,n.NotaFecha))a  
full join(  
 select month(g.GastoFecha) as Numero,DATENAME(month,g.GastoFecha) as Mes,SUM(g.GstoMonto) as Gastos   
 from GastosFijos g (noLOCK) --GASTOS  
 where YEAR(g.GastoFecha)=@anno  
 group by month(g.GastoFecha),DATENAME(month,g.GastoFecha)  
)g on a.Numero=g.Numero  
order by a.Numero desc,g.Numero desc  
for xml path('')),1,1,'')),'~')  
end
GO
/****** Object:  StoredProcedure [dbo].[reporteGananciaB]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[reporteGananciaB]   
@Mes int,  
@anno int  
as  
begin  
SET LANGUAGE Spanish;select isnull(a.Numero,g.Numero) as Numero,ISNULL(a.Mes,g.Mes) as Mes,  
CONVERT(VarChar(50), cast(isnull(v.TotalVenta,0) as money ), 1) as TotalVenta,  
CONVERT(VarChar(50), cast((isnull(a.Ganancia,0))as money ), 1) as G_Ventas,  
CONVERT(VarChar(50), cast(isnull(g.Gastos,0) as money ), 1) as Gatos,  
CONVERT(VarChar(50), cast((isnull(a.Ganancia,0)-isnull(g.Gastos,0)) as money ), 1) as G_Liquida  
from  
(select month(n.NotaFecha) as Numero,DATENAME(month,n.NotaFecha) as Mes,  
sum(n.NotaGanancia)- SUM(n.NotaDescuento) as Ganancia--ganancia  
from   
NotaPedido n  
where n.NotaEstado='CANCELADO' and (MONTH(n.NotaFecha)=@Mes and YEAR(n.NotaFecha)=@anno)  
group by month(n.NotaFecha),DATENAME(month,n.NotaFecha))a  
full join(  
select month(g.GastoFecha) as Numero,DATENAME(month,g.GastoFecha) as Mes,SUM(g.GstoMonto) as Gastos   
from GastosFijos g--gastos  
where(Month(g.GastoFecha)=@Mes and YEAR(g.GastoFecha)=@anno)  
group by month(g.GastoFecha),DATENAME(month,g.GastoFecha)  
)g on a.Numero=g.Numero  
full join(select month(n.NotaFecha) as Numero,  
DATENAME(month,n.NotaFecha) as Mes,SUM(n.NotaPagar) as TotalVenta   
from NotaPedido n--total venta  
where (Month(n.NotaFecha)=@Mes and YEAR(n.NotaFecha)=@anno) and n.NotaEstado='CANCELADO'  
group by month(n.NotaFecha),DATENAME(month,n.NotaFecha)  
)v on a.Numero=v.Numero  
group by a.Numero,g.Numero,a.Mes,g.Mes,v.TotalVenta,a.Ganancia,g.Gastos  
order by 1 desc  
end
GO
/****** Object:  StoredProcedure [dbo].[reportePDT]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[reportePDT]
@CompaniaId int,
@Mes int,
@Anno int
as
begin
select isnull(b.CompaniaId,isnull(S.CompaniaId,isnull(d.CompaniaId,isnull(x.CompaniaId,z.CompaniaId)))) as CompaniaId,
convert(varchar(50),cast((ISNULL(b.Monto,0))as money),1) as Ventas,
convert(varchar(50),cast((ISNULL(s.Monto,0)+ISNULL(d.Monto,0))-(ISNULL(x.Monto,0)+ISNULL(z.Monto,0))as money),1) as Compras
from
(
select d.CompaniaId,sum(d.DocuTotal) as Monto--VENTASSS
from DocumentoVenta d
where d.CompaniaId=@companiaId and(month(d.DocuEmision)=@Mes and year(d.DocuEmision)=@Anno)and (d.DocuDocumento<>'PROFORMA V' AND d.DocuDocumento<>'NOTA PEDIDO') and d.DocuEstado<>'ANULADO'
group by d.CompaniaId
)b
full join(
select c.CompaniaId,sum(c.CompraTotal) as Monto
from Compras c--FACTURAS EN SOLES
where c.CompaniaId=@companiaId and(month(c.CompraComputo)=@Mes and year(c.CompraComputo)=@Anno)AND(c.TipoCodigo='01' and c.CompraMoneda='SOLES')
group by c.CompaniaId
)s on b.CompaniaId=s.CompaniaId
full join
(select c.CompaniaId,cast(sum(c.CompraTotal*c.CompraTipoSunat)as decimal(18,2)) as Monto
from Compras c--FACTURAS EN DOLARES
where c.CompaniaId=@companiaId and (month(c.CompraComputo)=@Mes and year(c.CompraComputo)=@Anno)AND(c.TipoCodigo='01' and c.CompraMoneda='DOLARES')
group by c.CompaniaId
)d on b.CompaniaId=d.CompaniaId
full join(
select c.CompaniaId,cast(sum(c.CompraTotal*c.CompraTipoSunat)as decimal(18,2)) as Monto
from Compras c--nota de credito en dolares
where c.CompaniaId=@companiaId and(month(c.CompraComputo)=@Mes and year(c.CompraComputo)=@Anno)AND(c.TipoCodigo='07' and c.CompraMoneda='DOLARES')
group by c.CompaniaId
)x on b.CompaniaId=x.CompaniaId
full join (
select c.CompaniaId,sum(c.CompraTotal) as Monto
from Compras c--nota de credito en soles
where c.CompaniaId=@companiaId and(month(c.CompraComputo)=@Mes and year(c.CompraComputo)=@Anno)AND(c.TipoCodigo='07' and c.CompraMoneda='SOLES')
group by c.CompaniaId
)z on b.CompaniaId=z.CompaniaId
end
GO
/****** Object:  StoredProcedure [dbo].[reporteVentaCompania]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[reporteVentaCompania]  
@Mes int,  
@Anno int  
as  
begin  
select top 2 isnull(b.CompaniaId,isnull(S.CompaniaId,isnull(d.CompaniaId,isnull(x.CompaniaId,z.CompaniaId)))) as CompaniaId,  
ISNULL(b.RazonSocial,isnull(S.RazonSocial,isnull(d.RazonSocial,isnull(x.RazonSocial,z.RazonSocial))))as RazonSocial,  
convert(varchar(50),cast((ISNULL(b.Monto,0))as money),1) as Ventas,  
convert(varchar(50),cast(((ISNULL(s.Monto,0)+ISNULL(d.Monto,0))-(ISNULL(x.Monto,0)+ISNULL(z.Monto,0)))as money),1) as Compras  
from  
(  
select top 2 c.CompaniaId,c.CompaniaRazonSocial as RazonSocial,  
sum(d.DocuTotal) as Monto--VENTASSS  
from DocumentoVenta d  
inner join Compania c  
on c.CompaniaId=d.CompaniaId  
where (month(d.DocuEmision)=@Mes and year(d.DocuEmision)=@Anno)and 
(d.DocuDocumento<>'PROFORMA V' AND d.DocuDocumento<>'NOTA DE CREDITO') and d.DocuAsociado=''  
group by c.CompaniaId,c.CompaniaRazonSocial  
)b  
full join(  
select TOP 2 co.CompaniaId,co.CompaniaRazonSocial as RazonSocial,sum(c.CompraTotal) as Monto  
from Compras c--FACTURAS EN SOLES  
inner join Compania co  
on co.CompaniaId=c.CompaniaId  
where (month(c.CompraComputo)=@Mes and year(c.CompraComputo)=@Anno)AND(c.TipoCodigo='01' and c.CompraMoneda='SOLES')  
group by co.CompaniaId,co.CompaniaRazonSocial  
)s on b.CompaniaId=s.CompaniaId  
full join  
(select TOP 2 co.CompaniaId,co.CompaniaRazonSocial as RazonSocial,cast(sum(c.CompraTotal*c.CompraTipoSunat)as decimal(18,2)) as Monto  
from Compras c--FACTURAS EN DOLARES  
inner join Compania co  
on co.CompaniaId=c.CompaniaId  
where(month(c.CompraComputo)=@Mes and year(c.CompraComputo)=@Anno)AND(c.TipoCodigo='01' and c.CompraMoneda='DOLARES')  
group by co.CompaniaId,co.CompaniaRazonSocial  
)d on b.CompaniaId=d.CompaniaId  
full join(  
select TOP 2 co.CompaniaId,co.CompaniaRazonSocial as RazonSocial,cast(sum(c.CompraTotal*c.CompraTipoSunat)as decimal(18,2)) as Monto  
from Compras c--nota de credito en dolares  
inner join Compania co  
on co.CompaniaId=c.CompaniaId  
where(month(c.CompraComputo)=@Mes and year(c.CompraComputo)=@Anno)AND(c.TipoCodigo='07' and c.CompraMoneda='DOLARES')  
group by co.CompaniaId,co.CompaniaRazonSocial  
)x on b.CompaniaId=x.CompaniaId  
full join (  
select TOP 2 co.CompaniaId,co.CompaniaRazonSocial as RazonSocial,sum(c.CompraTotal) as Monto  
from Compras c--nota de credito en soles  
inner join Compania co  
on co.CompaniaId=c.CompaniaId  
where(month(c.CompraComputo)=@Mes and year(c.CompraComputo)=@Anno)AND(c.TipoCodigo='07' and c.CompraMoneda='SOLES')  
group by co.CompaniaId,co.CompaniaRazonSocial  
)z on b.CompaniaId=z.CompaniaId  
end
GO
/****** Object:  StoredProcedure [dbo].[respaldoBD]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[respaldoBD]
as
begin
declare @fecha varchar(max)
declare @hora varchar(max)
declare @archivo varchar(max)

set @fecha=CONVERT(Varchar(10),GETDATE(),105)
set @hora=REPLACE(CONVERT(varchar(10), GETDATE(), 108),':','-')
set @archivo='C:\Users\HP\OneDrive\Bakup\ROSITA-'+@fecha+'-'+@hora+'.bak'--'D:\Archivo_Sistema\Backup\ROSITA-'+@fecha+'-'+@hora+'.bak'

BACKUP DATABASE ROSITA TO DISK=@archivo
WITH FORMAT,
NAME='ROSITA';
end
GO
/****** Object:  StoredProcedure [dbo].[rptCompraA]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[rptCompraA]
as
begin
select c.CompraId,Convert(char(10),c.CompraEmision,103) as FechaEmision,c.CompraSerie+'-'+c.CompraNumero as Documento,
p.ProveedorRuc as RUC,p.ProveedorRazon as RazonSocial,c.TipoCodigo as TipoCodigo,
case when c.CompraMoneda='DOLARES' THEN
CONVERT(VarChar(50), cast((c.CompraTotal/1.18)*c.CompraTipoSunat as money ), 1)
else  CONVERT(VarChar(50), cast((c.CompraTotal/1.18) as money ), 1)
end as SubTotal,
case when c.CompraMoneda='DOLARES' then
CONVERT(VarChar(50), cast((c.CompraTotal-(c.CompraTotal/1.18))*c.CompraTipoSunat as money ), 1)
else CONVERT(VarChar(50), cast((c.CompraTotal-(c.CompraTotal/1.18))as money ), 1)
end as IGV,
case when c.CompraMoneda='DOLARES' then
CONVERT(VarChar(50), cast((c.CompraTotal *c.CompraTipoSunat) as money ), 1)
else CONVERT(VarChar(50), cast(c.CompraTotal as money ), 1)
end as Total,c.CompraMoneda as Moneda,c.CompraTipoSunat as TipoSunat,CONVERT(VarChar(50), cast(c.CompraTotal as money ), 1) as Monto
from Compras c
inner join Proveedor p
on p.ProveedorId=c.ProveedorId
end
GO
/****** Object:  StoredProcedure [dbo].[rptCompraComputo]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[rptCompraComputo] @fechainicio date,@fechafin date,@CompaniaId int
as
begin
select c.CompraId,Convert(char(10),c.CompraEmision,103) as FechaEmision,c.CompraSerie+'-'+c.CompraNumero as Documento,
p.ProveedorRuc as RUC,p.ProveedorRazon as RazonSocial,c.TipoCodigo as TipoCodigo,
case when c.CompraMoneda='DOLARES' THEN
CONVERT(VarChar(50), cast((c.CompraTotal/1.18)*c.CompraTipoSunat as money ), 1)
else  CONVERT(VarChar(50), cast((c.CompraTotal/1.18) as money ), 1)
end as SubTotal,
case when c.CompraMoneda='DOLARES' then
CONVERT(VarChar(50), cast((c.CompraTotal-(c.CompraTotal/1.18))*c.CompraTipoSunat as money ), 1)
else CONVERT(VarChar(50), cast((c.CompraTotal-(c.CompraTotal/1.18))as money ), 1)
end as IGV,
case when c.CompraMoneda='DOLARES' then
CONVERT(VarChar(50), cast((c.CompraTotal *c.CompraTipoSunat) as money ), 1)
else CONVERT(VarChar(50), cast(c.CompraTotal as money ), 1)
end as Total,c.CompraMoneda as Moneda,c.CompraTipoSunat as TipoSunat,CONVERT(VarChar(50), cast(c.CompraTotal as money ), 1) as Monto
from Compras c
inner join Proveedor p
on p.ProveedorId=c.ProveedorId
where (Convert(char(10),c.CompraComputo,103) BETWEEN @fechainicio AND @fechafin) and (c.TipoCodigo='01' or c.TipoCodigo='07') and c.CompaniaId=@CompaniaId
order by c.CompraEmision asc
end
GO
/****** Object:  StoredProcedure [dbo].[rptCompraEmision]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[rptCompraEmision] @fechainicio date,@fechafin date,@CompaniaId int
as
begin
select c.CompraId,Convert(char(10),c.CompraEmision,103) as FechaEmision,c.CompraSerie+'-'+c.CompraNumero as Documento,
p.ProveedorRuc as RUC,p.ProveedorRazon as RazonSocial,c.TipoCodigo as TipoCodigo,
case when c.CompraMoneda='DOLARES' THEN
CONVERT(VarChar(50), cast((c.CompraTotal/1.18)*c.CompraTipoSunat as money ), 1)
else  CONVERT(VarChar(50), cast((c.CompraTotal/1.18) as money ), 1)
end as SubTotal,
case when c.CompraMoneda='DOLARES' then
CONVERT(VarChar(50), cast((c.CompraTotal-(c.CompraTotal/1.18))*c.CompraTipoSunat as money ), 1)
else CONVERT(VarChar(50), cast((c.CompraTotal-(c.CompraTotal/1.18))as money ), 1)
end as IGV,
case when c.CompraMoneda='DOLARES' then
CONVERT(VarChar(50), cast((c.CompraTotal *c.CompraTipoSunat) as money ), 1)
else CONVERT(VarChar(50), cast(c.CompraTotal as money ), 1)
end as Total,c.CompraMoneda as Moneda,c.CompraTipoSunat as TipoSunat,CONVERT(VarChar(50), cast(c.CompraTotal as money ), 1) as Monto
from Compras c
inner join Proveedor p
on p.ProveedorId=c.ProveedorId
where (Convert(char(10),c.CompraEmision,103) BETWEEN @fechainicio AND @fechafin) and (c.TipoCodigo='01' or c.TipoCodigo='07') and c.CompaniaId=@CompaniaId
order by c.CompraEmision asc
end
GO
/****** Object:  StoredProcedure [dbo].[rptMes]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[rptMes]
@Mes int,
@Anno int
as 
begin
select
'Dia|Fecha|Venta|Ganancia|Gastos|GananciaLQ|FechaExacta¬80|105|103|103|103|103|100¬String|String|String|String|String|String|String¬'+
isnull((select STUFF((select '¬'+
convert(varchar,isnull(a.Dia,b.Dia))+'|'+convert(varchar,isnull(a.Fecha,b.Fecha))+'|'+
convert(varchar(50),cast(isnull(a.VentaTotal,0)as money),1)+'|'+
convert(varchar(50),cast(isnull(a.GananciaTotal,0)as money),1)+'|'+
convert(varchar(50),cast(isnull(b.Gastos,0)as money),1)+'|'+
convert(varchar(50),cast(isnull(a.GananciaTotal,0)-isnull(b.Gastos,0)as money),1)+'|'+
convert(varchar,isnull(a.FechaExacta,b.FechaExacta))
from
(select DAY(n.NotaFecha) as Dia,
dbo.diaNombre(n.NotaFecha)+' '+convert(nvarchar,DAY(n.NotaFecha)) as Fecha,
SUM(n.NotaPagar)as VentaTotal,
SUM(NotaGanancia)- SUM(n.NotaDescuento) as GananciaTotal,convert(varchar,n.NotaFecha,101) as FechaExacta
from NotaPedido n
where (month(n.NotaFecha)=@Mes and year(n.NotaFecha)=@Anno) and n.NotaEstado='CANCELADO'
group by DAY(n.NotaFecha),dbo.diaNombre(n.NotaFecha),convert(varchar,n.NotaFecha,101))a
full join(
	select DAY(g.GastoFecha) as Dia,
	dbo.diaNombre(g.GastoFecha)+' '+convert(nvarchar,DAY(g.GastoFecha)) as Fecha,
	SUM(g.GstoMonto) as Gastos,convert(varchar,g.GastoFecha,101) as FechaExacta
	from GastosFijos g (noLOCK) 
	where (month(g.GastoFecha)=@Mes and year(g.GastoFecha)=@Anno)
	group by DAY(g.GastoFecha),dbo.diaNombre(g.GastoFecha),convert(varchar,g.GastoFecha,101)
)b on a.Dia=b.Dia
order by a.Dia DESC
for xml path('')),1,1,'')),'~')
end
GO
/****** Object:  StoredProcedure [dbo].[rptSemanal]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[rptSemanal]
@Fecha date,
@Anno int
as
begin
declare @NumSemana int
set @NumSemana=(select DATEPART(WK,@Fecha))
select
'Dia|Fecha|Venta|Ganancia|Gastos|GananciaLQ|FechaExacta¬80|105|103|103|103|103|100¬String|String|String|String|String|String|String¬'+
isnull((select STUFF((select '¬'+
convert(varchar,isnull(a.Dia,b.Dia))+'|'+convert(varchar,isnull(a.Fecha,b.Fecha))+'|'+
convert(varchar(50),cast(isnull(a.VentaTotal,0)as money),1)+'|'+
convert(varchar(50),cast(isnull(a.GananciaTotal,0)as money),1)+'|'+
convert(varchar(50),cast(isnull(b.Gastos,0)as money),1)+'|'+
convert(varchar(50),cast(isnull(a.GananciaTotal,0)-isnull(b.Gastos,0)as money),1)+'|'+
convert(varchar,isnull(a.FechaExacta,b.FechaExacta))
from
(select DAY(n.NotaFecha) as Dia,
dbo.diaNombre(n.NotaFecha)+' '+convert(nvarchar,DAY(n.NotaFecha)) as Fecha,
SUM(n.NotaPagar)as VentaTotal,
SUM(NotaGanancia)- SUM(n.NotaDescuento) as GananciaTotal,
convert(varchar,n.NotaFecha,101) as FechaExacta
from NotaPedido n
where ((DATEPART(WK,n.NotaFecha)=@NumSemana)and year(n.NotaFecha)=@Anno) and n.NotaEstado='CANCELADO'
group by DAY(n.NotaFecha),dbo.diaNombre(n.NotaFecha),convert(varchar,n.NotaFecha,101))a
full join(
	select DAY(g.GastoFecha) as Dia,
	dbo.diaNombre(g.GastoFecha)+' '+convert(nvarchar,DAY(g.GastoFecha)) as Fecha,
	SUM(g.GstoMonto) as Gastos,convert(varchar,g.GastoFecha,101) as FechaExacta 
	from GastosFijos g (noLOCK) 
	where((DATEPART(WK,g.GastoFecha)=@NumSemana) and YEAR(g.GastoFecha)=@Anno)
    group by DAY(g.GastoFecha),dbo.diaNombre(g.GastoFecha),convert(varchar,g.GastoFecha,101)
)b on a.Dia=b.Dia
order by a.Dia ASC
for xml path('')),1,1,'')),'~')
end
GO
/****** Object:  StoredProcedure [dbo].[rptVendedor]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[rptVendedor]   
@Mes INT,  
@ANNO INT  
as  
begin 
select
'Personal|Clientes|Ventas|SubTotal|IGV|Ganancia|ImpRenta|Descuento|DesTotal|GLiquida¬185|105|125|125|125|125|125|125|125|125¬String|String|String|String|String|String|String|String|String|String¬'+
isnull((select STUFF ((select '¬'+isnull(a.Usuario,b.Usuario)+'|'+  
convert(varchar,ISNULL(a.Cliente,0))+'|'+  
convert(varchar(50),cast((isnull(a.Venta,0)) as money),1)+'|'+--converiertes a moneda y despues conviertes a texto
convert(varchar(50),cast(((isnull(b.Ganancia,0)/1.18))as money),1)+'|'+
convert(varchar(50),cast((isnull(b.Ganancia,0)-(cast((isnull(b.Ganancia,0)/1.18)as decimal(18,2))))as money),1)+'|'+   
convert(varchar(50),cast((isnull(b.Ganancia,0))as money),1)+'|'+ 
convert(varchar(50),cast((cast((isnull(a.Venta,0)* 0.01) as decimal(18,2)))as money),1)+'|'+   
convert(varchar(50),cast((isnull(a.Descuento,0))as money),1)+'|'+   
convert(varchar(50),cast(((cast((isnull(b.Ganancia,0)-(cast((isnull(b.Ganancia,0)/1.18)as decimal(18,2))))as decimal(18,2))+  
cast((isnull(a.Venta,0)* 0.01) as decimal(18,2)))+isnull(a.Descuento,0))as money),1)+'|'+   
convert(varchar(50),cast((isnull(b.Ganancia,0)-((cast((isnull(b.Ganancia,0)-(cast((isnull(b.Ganancia,0)/1.18)as decimal(18,2))))as decimal(18,2))+cast((isnull(a.Venta,0)* 0.01) as decimal(18,2)))+isnull(a.Descuento,0)))as money),1)
from   
(  
	select n.NotaUsuario as Usuario,COUNT(ClienteId) as Cliente,SUM(n.NotaPagar) as Venta,SUM(n.NotaDescuento) as Descuento  
	from NotaPedido n (NOLOCK) 
	where (
		month(n.NotaFecha)=@Mes and
		YEAR(n.NotaFecha)=@ANNO) and
		n.NotaEstado='CANCELADO'
	group by n.NotaUsuario)a  
	FULL join(
	select n.NotaUsuario as Usuario,sum(n.NotaGanancia) as Ganancia--cast(Sum((d.DetallePrecio - d.DetalleCosto) * d.DetalleCantidad)as decimal(18,2)) as Ganancia  --ok
	--from DetallePedido d (NOLOCK) 
	--inner join 
	from NotaPedido n  (NOLOCK) 
	--on n.NotaId=d.NotaId
	where (month(n.NotaFecha)=@Mes and 
	YEAR(n.NotaFecha)=@ANNO) and 
	n.NotaEstado='CANCELADO'  
	group by n.NotaUsuario  
)b on a.Usuario=b.Usuario  
group by a.Usuario,b.Usuario,a.Cliente,a.Venta,a.Descuento,b.Ganancia  
order by a.Cliente desc 
for xml path('')),1,1,'')),'~') 
end
GO
/****** Object:  StoredProcedure [dbo].[TipoCambioFecha]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[TipoCambioFecha] 
@fechainicio date,
@fechafin date
as
begin
select
'ID|Fecha|COMPRA|VENTA|EMPRESA¬90|110|108|108|117¬String|String|String|String|String¬'+
isnull((select STUFF((select '¬'+ convert(varchar,t.IdTipo),+'|'+
(Convert(char(10),t.TipoFecha,103))+'|'+convert(varchar,t.TipoCompra)+'|'+
convert(varchar,t.TipoVenta)+'|'+
convert(varchar,t.TipoEmpresa) 
from TipoCambio t 
where (Convert(char(10),t.TipoFecha,101) BETWEEN @fechainicio AND @fechafin) 
order by t.TipoFecha asc
for xml path('')),1,1,'')),'~')
end
GO
/****** Object:  StoredProcedure [dbo].[totalLetras]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create proc [dbo].[totalLetras] @numero decimal(18,2),@Moneda varchar(60)
as
begin
select dbo.letras(@numero,@Moneda) as letras
end
GO
/****** Object:  StoredProcedure [dbo].[upsEliminaGuiaInterna]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[upsEliminaGuiaInterna]                    
@ListaOrden varchar(Max)                    
as                    
begin  
                    
Declare @posA1 int,@posA2 int,@posA3 int      
Declare @orden varchar(max),      
        @detalle varchar(max),      
        @Guia varchar(max)  
             
Set @posA1 = CharIndex('[',@ListaOrden,0)      
Set @posA2 = CharIndex('[',@ListaOrden,@posA1+1)      
Set @posA3 =Len(@ListaOrden)+1      
Set @orden = SUBSTRING(@ListaOrden,1,@posA1-1)      
Set @detalle = SUBSTRING(@ListaOrden,@posA1+1,@posA2-@posA1-1)      
Set @Guia=SUBSTRING(@ListaOrden,@posA2+1,@posA3-@posA2-1)  
  
declare @p1 int,@p2 int,@p3 int,            
        @p4 int,@p5 int             
                   
declare @GuiaId numeric(38),@Usuario varchar(80),                  
        @Concepto nvarchar(1),@Numero nvarchar(20),      
        @Serie nvarchar(4)            
                    
Set @orden= LTRIM(RTrim(@orden))                    
Set @p1 = CharIndex('|',@orden,0)                  
Set @p2 = CharIndex('|',@orden,@p1+1)            
Set @p3 = CharIndex('|',@orden,@p2+1)            
Set @p4 = CharIndex('|',@orden,@p3+1)            
Set @p5 =Len(@orden)+1          
                   
Set @GuiaId=convert(numeric(38),SUBSTRING(@orden,1,@p1-1))                    
Set @Usuario=SUBSTRING(@orden,@p1+1,@p2-@p1-1)                  
Set @Concepto=SUBSTRING(@orden,@p2+1,@p3-@p2-1)            
Set @Numero=SUBSTRING(@orden,@p3+1,@p4-@p3-1)             
Set @Serie=SUBSTRING(@orden,@p4+1,@p5-@p4-1)                    
                   
Begin Transaction                    
                  
Declare Tabla Cursor For Select * From fnSplitString(@detalle,';')                     
Open Tabla                    
Declare @Columna varchar(max),                    
  @IdProducto numeric(20),                    
  @Cantidad decimal(18,2),                    
  @Costo decimal(18,4),                           
  @ValorUM decimal(18,4),    
  @AplicaINV nvarchar(1),
                 
  @StockInicial decimal(18,2),                          
  @StockFinal decimal(18,2),@CantidadIng decimal(18,2)     
                    
Declare @pos1 int,@pos2 int,
        @pos3 int,@pos4 int,
        @pos5 int
                        
                   
Fetch Next From Tabla INTO @Columna                    
 While @@FETCH_STATUS = 0                    
 Begin                
                     
Set @pos1 = CharIndex('|',@Columna,0)                    
Set @pos2 = CharIndex('|',@Columna,@pos1+1)    
Set @pos3 = CharIndex('|',@Columna,@pos2+1)
Set @pos4 = CharIndex('|',@Columna,@pos3+1)                      
Set @pos5 = Len(@Columna)+1                  
                  
Set @IdProducto=Convert(numeric(38),SUBSTRING(@Columna,1,@pos1-1))                    
Set @Cantidad=Convert(decimal(18,2),SUBSTRING(@Columna,@pos1+1,@pos2-(@pos1+1)))                    
Set @Costo=Convert(decimal(18,2),SUBSTRING(@Columna,@pos2+1,@pos3-(@pos2+1)))    
Set @ValorUM=Convert(decimal(18,4),SUBSTRING(@Columna,@pos3+1,@pos4-(@pos3+1)))
Set @AplicaINV=SUBSTRING(@Columna,@pos4+1,@pos5-(@pos4+1))             
 
 if(@AplicaINV='S')
 BEGIN
                 
 if(@Concepto='S')                  
 begin                  
                   
 set @StockInicial=(select top 1 ProductoCantidad     
 from Producto(nolock)                           
 where IdProducto=@IdProducto)                          
                         
 set @CantidadIng=(@Cantidad*@ValorUM)                          
 set @StockFinal=@StockInicial-@CantidadIng                          
                         
 update Producto                          
 set ProductoCantidad=ProductoCantidad-@CantidadIng                          
 where IdProducto=@IdProducto       
      
 insert into Kardex values(@IdProducto,GETDATE(),'Salida Por Anulacion Guia Interna',@Serie+'-'+@Numero,@StockInicial,                    
 0,@CantidadIng,@Costo,@StockFinal,'SALIDA',@Usuario)                   
                                                 
 end                  
 else                  
 begin                  
                   
 set @StockInicial=(select top 1 ProductoCantidad     
 from Producto(nolock)                           
 where IdProducto=@IdProducto)                          
                        
 set @CantidadIng=(@Cantidad*@ValorUM)                          
 set @StockFinal=@StockInicial+@CantidadIng                          
                        
 update Producto                          
 set ProductoCantidad=ProductoCantidad+@CantidadIng                          
 where IdProducto=@IdProducto      
                                   
 insert into Kardex values(@IdProducto,GETDATE(),'Ingreso Por Anulacion Guia Interna',@Serie+'-'+@Numero,@StockInicial,                    
 @CantidadIng,0,@Costo,@StockFinal,'INGRESO',@Usuario)                 
                                     
 END
 END              
                
Fetch Next From Tabla INTO @Columna                    
end                    
 Close Tabla;                    
 Deallocate Tabla;          
 delete from DetalleGuiaInterna                  
 where GuiaId=@GuiaId                   
 delete from GuiaInternaSI                  
 where GuiaId=@GuiaId                           
 --Commit Transaction;                    
 --select 'true'  
  if(len(@Guia)>0)      
begin      
Declare TablaB Cursor For Select * From fnSplitString(@Guia,';')       
Open TablaB      
Declare @ColumnaB varchar(max)    
Declare @g1 int,@g2 int,    
        @g3 int,@g4 int,@g5 int    
    
Declare @CantidadA decimal(18,2),     
        @IdProductoU numeric(20),                     
        @CantidadU decimal(18,2),                        
        @UmU varchar(40),                                                       
        @ValorUMU decimal(18,4)    
    
Declare @IniciaStockB decimal(18,2),    
        @StockFinalB decimal(18,2)    
              
Fetch Next From TablaB INTO @ColumnaB      
 While @@FETCH_STATUS = 0      
 Begin      
Set @g1 = CharIndex('|',@ColumnaB,0)                       
Set @g2 = CharIndex('|',@ColumnaB,@g1+1)                        
Set @g3 = CharIndex('|',@ColumnaB,@g2+1)                        
Set @g4 = CharIndex('|',@ColumnaB,@g3+1)                        
Set @g5=Len(@ColumnaB)+1       
     
set @CantidadA=Convert(decimal(18,2),SUBSTRING(@ColumnaB,1,@g1-1))    
Set @IdProductoU=Convert(numeric(20),SUBSTRING(@ColumnaB,@g1+1,@g2-(@g1+1)))    
Set @CantidadU=Convert(decimal(18,2),SUBSTRING(@ColumnaB,@g2+1,@g3-(@g2+1)))      
Set @UmU=SUBSTRING(@ColumnaB,@g3+1,@g4-(@g3+1))      
Set @ValorUMU=Convert(decimal(18,4),SUBSTRING(@ColumnaB,@g4+1,@g5-(@g4+1)))          
    
 Declare @CantidadSalB decimal(18,2)     
    
 set @CantidadSalB=(@CantidadA * @CantidadU)* @ValorUMU                
                    
 set @IniciaStockB=(select top 1 p.ProductoCantidad     
 from Producto p where p.IdProducto=@IdProductoU)  
   
                         
 if(@Concepto='I')  
 begin   
     
 set @StockFinalB=@IniciaStockB + @CantidadSalB                                      
   
 insert into Kardex values(@IdProductoU,getdate(),'Ingreso Por Anulacion Guia Interna',@Serie+'-'+@Numero,@IniciaStockB,                          
 @CantidadSalB,0,0,@StockFinalB,'INGRESO',@Usuario)   
                                           
 update producto                         
 set  ProductoCantidad =ProductoCantidad + @CantidadSalB                       
 where IDProducto=@IdProductoU   
        
 end  
 else  
 begin  
   
 set @StockFinalB=@IniciaStockB - @CantidadSalB                                      
   
 insert into Kardex values(@IdProductoU,getdate(),'Salida Por Anulacion Guia Interna',@Serie+'-'+@Numero,@IniciaStockB,                          
 0,@CantidadSalB,0,@StockFinalB,'SALIDA',@Usuario)   
                                         
 update producto                         
 set  ProductoCantidad =ProductoCantidad - @CantidadSalB                       
 where IDProducto=@IdProductoU    
   
 end  
  
Fetch Next From TablaB INTO @ColumnaB      
end      
    Close TablaB;      
    Deallocate TablaB;      
    Commit Transaction;      
    select 'true'  
end      
else      
begin      
    Commit Transaction;      
    select 'true'  
end  
               
end
GO
/****** Object:  StoredProcedure [dbo].[upsInsertaTemGuiaB]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[upsInsertaTemGuiaB]        
@Data varchar(max)          
as          
begin          
Declare @pos1 int,@pos2 int,          
        @pos3 int,@pos4 int,          
        @pos5 int,@pos6 int,    
        @pos7 int,@pos8 int         
          
Declare @UsuarioID int,@IdProducto numeric(20),        
@UnidadM varchar(80),@Cantidad decimal(18,2),        
@PrecioVenta decimal(18,2),@Importe decimal(18,2),    
@Concepto nvarchar(1),@ValorUM decimal(18,4)   
        
Set @Data = LTRIM(RTrim(@Data))          
Set @pos1 = CharIndex('|',@Data,0)        
Set @pos2 = CharIndex('|',@Data,@pos1+1)        
Set @pos3 = CharIndex('|',@Data,@pos2+1)            
Set @pos4 = CharIndex('|',@Data,@pos3+1)    
Set @pos5 = CharIndex('|',@Data,@pos4+1)            
Set @pos6 = CharIndex('|',@Data,@pos5+1)
Set @pos7 = CharIndex('|',@Data,@pos6+1)          
Set @pos8 = Len(@Data)+1         
        
Set @UsuarioID=convert(int,SUBSTRING(@Data,1,@pos1-1))          
Set @IdProducto=convert(numeric(20),SUBSTRING(@Data,@pos1+1,@pos2-@pos1-1))          
Set @Cantidad=convert(decimal(18,2),SUBSTRING(@Data,@pos2+1,@pos3-@pos2-1))         
Set @UnidadM=SUBSTRING(@Data,@pos3+1,@pos4-@pos3-1)    
Set @PrecioVenta=convert(decimal(18,2),SUBSTRING(@Data,@pos4+1,@pos5-@pos4-1))    
Set @Importe=convert(decimal(18,2),SUBSTRING(@Data,@pos5+1,@pos6-@pos5-1))    
Set @Concepto=SUBSTRING(@Data,@pos6+1,@pos7-@pos6-1)
Set @ValorUM=convert(decimal(18,4),SUBSTRING(@Data,@pos7+1,@pos8-@pos7-1))         
        
insert into TemporalGuiaB values(@UsuarioID,@IdProducto,        
@Cantidad,@UnidadM,@PrecioVenta,@Importe,@Concepto,@ValorUM)        
         
select 'true'          
          
end
GO
/****** Object:  StoredProcedure [dbo].[usp_DeleteOldBackupFiles]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[usp_DeleteOldBackupFiles] 
    @path NVARCHAR(256),--RUTA DEL ARCHIVO
	@extension NVARCHAR(10),--EXTENSION DEL ARCHIVO
	@age_hrs INT--el número de horas que tiene que envejecer 
	--un archivo de respaldo para ser eliminado.
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @DeleteDate NVARCHAR(50)
	DECLARE @DeleteDateTime DATETIME

	SET @DeleteDateTime = DateAdd(hh, - @age_hrs, GetDate())
    SET @DeleteDate = (Select Replace(Convert(nvarchar, @DeleteDateTime, 111), '/', '-') 
    + 'T' + Convert(nvarchar, @DeleteDateTime, 108))

	EXECUTE master.dbo.xp_delete_file 0,
		@path,
		@extension,
		@DeleteDate,--
		1
END
GO
/****** Object:  StoredProcedure [dbo].[uspAnularNC]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspAnularNC]
@ListaOrden varchar(Max)
as
begin
Declare @pos int
Declare @orden varchar(max)
Declare @detalle varchar(max)
Set @pos = CharIndex('[',@ListaOrden,0)
Set @orden = SUBSTRING(@ListaOrden,1,@pos-1)
Set @detalle = SUBSTRING(@ListaOrden,@pos+1,len(@ListaOrden)-@pos)
declare @1 int,@2 int,@3 int,@4 int,@5 int
declare @DocuId numeric(38),@NotaId numeric(38),
@DocuUsuario varchar(80),@DocuAsociado varchar(80),@KardexDocu varchar(80)
Set @orden= LTRIM(RTrim(@orden))
Set @1 = CharIndex('|',@orden,0)
Set @2 = CharIndex('|',@orden,@1+1)
Set @3 = CharIndex('|',@orden,@2+1)
Set @4 = CharIndex('|',@orden,@3+1)
Set @5 = Len(@orden)+1
Set @DocuId=convert(numeric(38),SUBSTRING(@orden,1,@1-1))
Set @NotaId=convert(numeric(38),SUBSTRING(@orden,@1+1,@2-@1-1))
Set @DocuUsuario=SUBSTRING(@orden,@2+1,@3-@2-1)
Set @DocuAsociado=SUBSTRING(@orden,@3+1,@4-@3-1)
Set @KardexDocu=SUBSTRING(@orden,@4+1,@5-@4-1)
Begin Transaction
update DocumentoVenta
set DocuSubTotal=0,DocuIgv=0,DocuTotal=0,DocuSaldo=0,DocuUsuario=@DocuUsuario,DocuEstado='ANULADO'
where DocuId=@DocuId
update DocumentoVenta
set DocuAsociado=''
where DocuId=@DocuAsociado
 Declare Tabla Cursor For Select * From fnSplitString(@detalle,';')	
Open Tabla
Declare @Columna varchar(max),
		@IdProducto numeric(20),
		@Cantidad decimal(18,2),
		@Precio decimal(18,2),
		@Importe decimal(18,2),
		@DetalleNotaId numeric(38),
		@UM varchar(80),
		@ValorUM decimal(18,4),
		@StockInicial decimal(18,2),
		@StockFinal decimal(18,2),@CantidadSal decimal(18,2)
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
Set @Cantidad=Convert(decimal(18,2),SUBSTRING(@Columna,1,@p1-1))
Set @UM=SUBSTRING(@Columna,@p1+1,@p2-(@p1+1))
Set @Precio=Convert(decimal(18,2),SUBSTRING(@Columna,@p2+1,@p3-(@p2+1)))
Set @Importe=Convert(decimal(18,2),SUBSTRING(@Columna,@p3+1,@p4-(@p3+1)))
Set @DetalleNotaId=Convert(numeric(38),SUBSTRING(@Columna,@p4+1,@p5-(@p4+1)))
Set @IdProducto=Convert(numeric(20),SUBSTRING(@Columna,@p5+1,@p6-(@p5+1)))
Set @ValorUM=Convert(decimal(18,4),SUBSTRING(@Columna,@p6+1,@p7-(@p6+1)))
set @StockInicial=(select top 1 ProductoCantidad from Producto where IdProducto=@IdProducto)
set @CantidadSal=(@Cantidad*@ValorUM)
set @StockFinal=@StockInicial-@CantidadSal
update Producto
set ProductoCantidad=ProductoCantidad-@CantidadSal
where IdProducto=@IdProducto
insert into Kardex
values(@IdProducto,GETDATE(),'Anulacion por Nota Credito',@KardexDocu,@StockInicial,0,@CantidadSal,@Precio,@StockFinal,'SALIDA',@DocuUsuario)
Fetch Next From Tabla INTO @Columna
end
	Close Tabla;
	Deallocate Tabla;
	Commit Transaction;
select
isnull((select STUFF ((select '¬'+convert(varchar,d.DocuId)+'|'+convert(varchar,d.CompaniaId)+'|'+
convert(varchar,d.NotaId)+'|'+(Convert(char(10),d.DocuEmision,103))+'|'+
d.DocuDocumento+'|'+d.docuSerie+'-'+d.DocuNumero+'|'+c.ClienteRazon+'|'+c.ClienteRuc+'|'+
c.ClienteDni+'|'+d.DocuNumero+'|'+d.DocuSerie+'|'+
(convert(varchar(50), CAST(d.DocuSubTotal as money),1))+'|'+
(convert(varchar(50), CAST(d.DocuIgv as money),1))+'|'+
(convert(varchar(50), CAST(d.DocuTotal as money),1))+'|'+
d.DocuUsuario+'|'+d.DocuEstado+'|'+c.ClienteDireccion+'|'+d.DocuAsociado
from DocumentoVenta d
inner join Cliente c
on c.ClienteId=d.ClienteId
where d.TipoCodigo='07'and (Month(d.DocuEmision)=Month(GETDATE())and year(d.DocuEmision)=YEAR(Getdate()))
order by d.DocuId desc
for xml path('')),1,1,'')),'~')
end
GO
/****** Object:  StoredProcedure [dbo].[uspCajaInsertaCsv]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspCajaInsertaCsv]  
@Data varchar(max)  
as  
Begin  
Declare @p1 int,@p2 int,@p3 int,  
        @p4 int,@p5 int,@p6 int,  
        @p7 int,@p8 int,@p9 int,  
        @p10 int,@p11 int,@p12 int,  
        @p13 int  
Declare @CajaId  numeric(38),@CajaCierre  varchar(40),  
        @MontoIniSOl  decimal(18,2),@CajaEncargado  varchar(60),  
        @CajaUsuario  varchar(60),@CajaEstado  varchar(40),@CajaIngresos  decimal(18,2),  
        @CajaDeposito  decimal(18,2),@CajaSalidas  decimal(18,2),@CajaTotal  decimal(18,2),  
        @UsuarioId  int,@CantCajas int,@SerieFactura varchar(10),@Asistencia int,  
        @Observacion varchar(max),@Monedas decimal(18,2)  
Set @Data = LTRIM(RTrim(@Data))  
Set @p1 = CharIndex('|',@Data,0)  
Set @p2=CharIndex('|',@Data,@p1+1)  
Set @p3=CharIndex('|',@Data,@p2+1)  
Set @p4=CharIndex('|',@Data,@p3+1)  
Set @p5=CharIndex('|',@Data,@p4+1)  
Set @p6=CharIndex('|',@Data,@p5+1)  
Set @p7=CharIndex('|',@Data,@p6+1)  
Set @p8=CharIndex('|',@Data,@p7+1)  
Set @p9=CharIndex('|',@Data,@p8+1)  
Set @p10=CharIndex('|',@Data,@p9+1)  
Set @p11=CharIndex('|',@Data,@p10+1)  
Set @p12=CharIndex('|',@Data,@p11+1)  
Set @p13= Len(@Data)+1  
Set @CajaId=convert(numeric(38),SUBSTRING(@Data,1,@p1-1))  
Set @CajaCierre=SUBSTRING(@Data,@p1+1,@p2-@p1-1)  
Set @MontoIniSOl=convert(decimal(18,2),SUBSTRING(@Data,@p2+1,@p3-@p2-1))  
Set @CajaEncargado=SUBSTRING(@Data,@p3+1,@p4-@p3-1)  
Set @CajaUsuario=SUBSTRING(@Data,@p4+1,@p5-@p4-1)  
Set @CajaEstado=SUBSTRING(@Data,@p5+1,@p6-@p5-1)  
Set @CajaIngresos=convert(decimal(18,2),SUBSTRING(@Data,@p6+1,@p7-@p6-1))  
Set @CajaDeposito=convert(decimal(18,2),SUBSTRING(@Data,@p7+1,@p8-@p7-1))  
Set @CajaSalidas=convert(decimal(18,2),SUBSTRING(@Data,@p8+1,@p9-@p8-1))  
Set @CajaTotal=convert(decimal(18,2),SUBSTRING(@Data,@p9+1,@p10-@p9-1))  
Set @UsuarioId=convert(int,SUBSTRING(@Data,@p10+1,@p11-@p10-1))  
Set @Observacion=SUBSTRING(@Data,@p11+1,@p12-@p11-1)  
Set @Monedas=SUBSTRING(@Data,@p12+1,@p13-@p12-1)  
if(@CajaId=0)  
begin  
IF EXISTS(select top 1 CajaId from Caja   
where CajaEstado='ACTIVO' order by 1 desc) --and UsuarioId=@UsuarioId  
begin  
select 'existe'  
end  
else  
begin  
insert into Caja values(GETDATE(),@CajaCierre,@MontoIniSOl,  
@CajaEncargado,@CajaUsuario,@CajaEstado,@CajaIngresos,@CajaDeposito,  
@CajaSalidas,@CajaTotal,@UsuarioId,@Observacion)  
set @CajaId=@@identity  
insert into CajaPincipal values('SALIDA',GETDATE(),@CajaId,'SENCILLO PARA LA CAJA NRO '+CONVERT(varchar,@CajaId),  
@MontoIniSOl,@CajaUsuario,0,'SENCILLO','','')  
insert into CajaPincipal values('INGRESO',GETDATE(),@CajaId,'INGRESO DE CAJA CHICA',  
0,@CajaUsuario,0,'INGRESO','','')  
insert into CajaDetalle values(@CajaId,GETDATE(),0,'INGRESO','','TOTAL EFECTIVO',0,0,0,'','T','V',@CajaUsuario,'','')  
insert into CajaDetalle values(@CajaId,GETDATE(),0,'INGRESO','','SENCILLO',0,0,0,'','T','V',@CajaUsuario,'','')  
insert into Monedas values(0,0,'200.00',0,'B',@CajaId)  
insert into Monedas values(0,0,'100.00',0,'B',@CajaId)  
insert into Monedas values(0,0,'50.00',0,'B',@CajaId)  
insert into Monedas values(0,0,'20.00',0,'B',@CajaId)  
insert into Monedas values(0,0,'10.00',0,'B',@CajaId)  
insert into Monedas values(0,0,'5.00',0,'M',@CajaId)  
insert into Monedas values(0,0,'2.00',0,'M',@CajaId)  
insert into Monedas values(0,0,'1.00',0,'M',@CajaId)  
insert into Monedas values(0,0,'0.50',0,'M',@CajaId)  
insert into Monedas values(0,0,'0.20',0,'M',@CajaId)  
insert into Monedas values(0,0,'0.10',0,'M',@CajaId)  
Select 'true'  
end  
end  
else  
begin  
UPDATE CajaPincipal  
SET CajaMonto=@MontoIniSOl  
WHERE CajaId=@CajaId AND Referencia='SENCILLO'  
UPDATE CajaPincipal  
SET CajaMonto=@Monedas  
WHERE CajaId=@CajaId AND Referencia='INGRESO'  
update Caja  
set CajaCierre=@CajaCierre,MontoIniSOl=@MontoIniSOl,  
CajaEncargado=@CajaEncargado,CajaUsuario=@CajaUsuario,  
CajaEstado=@CajaEstado,CajaIngresos=@CajaIngresos,CajaDeposito=@CajaDeposito,  
CajaSalidas=@CajaSalidas,CajaTotal=@CajaTotal,UsuarioId=@UsuarioId,  
observacion=@Observacion  
where CajaId=@CajaId  
Select 'true'  
end  
end
GO
/****** Object:  StoredProcedure [dbo].[uspComboUsuarios]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspComboUsuarios]
as
select
isnull((select STUFF ((select '¬'+
p.Nombres+'-'+convert(varchar,p.IdPersonal)
from PersonalBL p
order by p.IdPersonal desc
for xml path('')),1,1,'')),'~')
GO
/****** Object:  StoredProcedure [dbo].[uspCuentaProve]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspCuentaProve]
@ProveedorId numeric(38)
as
select 
'Id|EntidadBancaria|TipoCuenta|Moneda|NroCuenta¬100|250|140|95|250¬String|String|String|String|String¬'+
isnull((select STUFF ((select '¬'+ CONVERT(varchar,c.CuentaId)+'|'+c.Entidad+'|'+
c.TipoCuenta+'|'+c.Moneda+'|'+c.NroCuenta
from CuentaProveedor c
where c.ProveedorId=@ProveedorId
order by c.CuentaId desc
for xml path('')),1,1,'')),'~')
GO
/****** Object:  StoredProcedure [dbo].[uspDesanular]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspDesanular]
@ListaOrden varchar(Max)
as
begin
Declare @pos int
Declare @orden varchar(max)
Declare @detalle varchar(max)
Set @pos = CharIndex('[',@ListaOrden,0)
Set @orden = SUBSTRING(@ListaOrden,1,@pos-1)
Set @detalle = SUBSTRING(@ListaOrden,@pos+1,len(@ListaOrden)-@pos)
Declare @p1 int,@p2 int,
        @p3 int,@p4 int,
        @p5 int
Declare @DocuId numeric(38),
@NotaId numeric(38),@Usuario varchar(40),
@Estado varchar(40),@CajaId numeric(38),
@Total decimal(18,2),@Documento varchar(40),
@NotaFormaPago varchar(60),@Movimiento varchar(40)
Set @orden = LTRIM(RTrim(@orden))
Set @p1 = CharIndex('|',@orden,0)
Set @p2 = CharIndex('|',@orden,@p1+1)
Set @p3 = CharIndex('|',@orden,@p2+1)
set @p4=CharIndex('|',@orden,@p3+1)
Set @p5=Len(@orden)+1
Set @DocuId=convert(numeric(38),SUBSTRING(@orden,1,@p1-1))
Set @NotaId=convert(numeric(38),SUBSTRING(@orden,@p1+1,@p2-@p1-1))
Set @Usuario=SUBSTRING(@orden,@p2+1,@p3-@p2-1)
Set @Total=convert(decimal(18,2),SUBSTRING(@orden,@p3+1,@p4-@p3-1))
Set @Documento=SUBSTRING(@orden,@p4+1,@p5-@p4-1)
set @CajaId=isnull((select top 1 CajaId from Caja
where CajaEstado='ACTIVO'order by 1 desc),'0')
set @NotaFormaPago=(select top 1 NotaFormaPago from NotaPedido
where NotaId=@NotaId order by NotaId desc)
Begin Transaction
update DocumentoVenta
set DocuUsuario=@Usuario,DocuEstado='EMITIDO'
where DocuId=@DocuId
update NotaPedido
set NotaUsuario=@Usuario,NotaEstado='CANCELADO',NotaAcuenta=@Total,
NotaSaldo=0
Where NotaId=@NotaId
if(@CajaId<>0)
begin
if(@NotaFormaPago='EFECTIVO')set @Movimiento='INGRESO'
else if(@NotaFormaPago='DEPOSITO')set @Movimiento='DEPOSITO'
else set @Movimiento='TARJETA'
insert into CajaDetalle values(@CajaId,GETDATE(),@NotaId,@Movimiento,'',
'Transacción con '+@NotaFormaPago,@Total,@Total,0,'','T','',@Usuario,'','')
end
Declare Tabla Cursor For Select * From fnSplitString(@detalle,';')	
Open Tabla
Declare @Columna varchar(max),
		@IdProducto numeric(20),
		@Costo decimal(18,4),
		@Cantidad decimal(18,2),
		@Precio decimal(18,2),
		@IniciaStock decimal(18,2),
		@StockFinal decimal(18,2)
Declare @d1 int,@d2 int,@d3 int,@d4 int
Fetch Next From Tabla INTO @Columna
	While @@FETCH_STATUS = 0
	Begin
Set @d1 = CharIndex('|',@Columna,0)
Set @d2 = CharIndex('|',@Columna,@d1+1)
Set @d3 = CharIndex('|',@Columna,@d2+1)
Set @d4= Len(@Columna)+1
Set @IdProducto=Convert(numeric(38),SUBSTRING(@Columna,1,@d1-1))
Set @Cantidad=Convert(decimal(18,2),SUBSTRING(@Columna,@d1+1,@d2-(@d1+1)))
Set @Precio=Convert(decimal(18,2),SUBSTRING(@Columna,@d2+1,@d3-(@d2+1)))
Set @Costo=Convert(decimal(18,4),SUBSTRING(@Columna,@d3+1,@d4-(@d3+1)))
	set @IniciaStock=(select top 1 ProductoCantidad from Producto where IdProducto=@IdProducto)
	set @StockFinal=@IniciaStock-@Cantidad
    insert into Kardex values(@IdProducto,GETDATE(),'Salida por DesaAnulacion',@Documento,@IniciaStock,
	0,@Cantidad,@Costo,@StockFinal,'SALIDA',@Usuario)
	update producto 
	set  ProductoCantidad =ProductoCantidad - @Cantidad
	where IDProducto=@IdProducto
Fetch Next From Tabla INTO @Columna
end
	Close Tabla;
	Deallocate Tabla;
	Commit Transaction;
select 'true'
end
GO
/****** Object:  StoredProcedure [dbo].[uspDescuento]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspDescuento]
@detalle varchar(Max)
as
begin
Begin Transaction
Declare Tabla Cursor For Select * From fnSplitString(@detalle,';')	
Open Tabla
Declare @Columna varchar(max),
        @TemporalId numeric(38),
		@Descuento decimal(18,4)
Declare @p1 int,@p2 int
Fetch Next From Tabla INTO @Columna
	While @@FETCH_STATUS = 0
	Begin
Set @p1 = CharIndex('|',@Columna,0)
Set @p2 = Len(@Columna)+1
Set @TemporalId=Convert(numeric(38),SUBSTRING(@Columna,1,@p1-1))
Set @Descuento=Convert(decimal(18,4),SUBSTRING(@Columna,@p1+1,@p2-(@p1+1)))
update TemporalCompra 
set DetalleDescuento=@Descuento 
where TemporalId=@TemporalId		
Fetch Next From Tabla INTO @Columna
end
	Close Tabla;
	Deallocate Tabla;
	Commit Transaction;
select 'true'
end
GO
/****** Object:  StoredProcedure [dbo].[uspDescuentoB]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspDescuentoB]
@detalle varchar(Max)
as
begin
Begin Transaction
Declare Tabla Cursor For Select * From fnSplitString(@detalle,';')	
Open Tabla
Declare @Columna varchar(max),
        @DetalleId numeric(38),
		@Descuento decimal(18,4)
Declare @p1 int,@p2 int
Fetch Next From Tabla INTO @Columna
	While @@FETCH_STATUS = 0
	Begin
Set @p1 = CharIndex('|',@Columna,0)
Set @p2 = Len(@Columna)+1
Set @DetalleId=Convert(numeric(38),SUBSTRING(@Columna,1,@p1-1))
Set @Descuento=Convert(decimal(18,4),SUBSTRING(@Columna,@p1+1,@p2-(@p1+1)))
update DetalleCompra 
set DetalleDescuento=@Descuento
where DetalleId=@DetalleId	
Fetch Next From Tabla INTO @Columna
end
	Close Tabla;
	Deallocate Tabla;
	Commit Transaction;
select 'true'
end
GO
/****** Object:  StoredProcedure [dbo].[uspDetalleNC]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspDetalleNC]
@DocuId numeric(38)
as
begin
select
'Cantidad|UM|Descripcion|Precio|Importe|DetalleId|IdProducto|valorUM|PrecioSunat|IGVPrecio|ImporteSunat|Codigo¬103|100|350|110|115|100|100|100|100|100|100|100¬String|String|String|String|String|String|String|String|String|String|String|String¬'+
isnull((select STUFF((select '¬'+CONVERT(VarChar(50), cast(d.DetalleCantidad as money ), 1)+'|'+
d.DetalleUM+'|'+p.ProductoNombre+'|'+
CONVERT(VarChar(50), cast(d.DetallPrecio as money ), 1)+'|'+
CONVERT(VarChar(50), cast(d.DetalleImporte as money ), 1)+'|'+
convert(varchar,d.DetalleNotaId)+'|'+convert(varchar,d.IdProducto)+'|'+
convert(varchar,d.ValorUM)+'|'+
convert(varchar,convert(decimal(18,2),d.DetallPrecio/1.18))+'|'+
convert(varchar,(d.DetalleImporte - convert(decimal(18,2),d.DetalleImporte/1.18)))+'|'+
convert(varchar,convert(decimal(18,2),d.DetalleImporte/1.18))+'|'+
P.ProductoCodigo
from DetalleDocumento d
inner join Producto p
on p.IdProducto=d.IdProducto
where DocuId=@DocuId
order by d.DetalleId asc
for xml path('')),1,1,'')),'~')
end
GO
/****** Object:  StoredProcedure [dbo].[uspDetallePuntosListaCsv]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create procedure [dbo].[uspDetallePuntosListaCsv]
@IdPuntos varchar(80)
as
Begin
select 
'DetalleId|Codigo|Cantidad|Unidad|Descripcion|PrecioUni|PV|SV|Importe¬100|100|100|100|100|100|100|100|100¬String|String|String|String|String|String|String|String|String¬'+
isnull((select STUFF((select '¬'+Convert(varchar,d.DetalleId)+'|'+
d.Codigo+'|'+CONVERT(VarChar(50), cast(d.Cantidad as money ), 1)+'|'+
d.Unidad+'|'+d.Descripcion+'|'+CONVERT(VarChar(50), cast(d.PrecioUni as money ), 1)+'|'+
CONVERT(VarChar(50), cast(d.PV as money ), 1)+'|'+CONVERT(VarChar(50), cast(d.SV as money ), 1)+
'|'+CONVERT(VarChar(50), cast(d.Importe as money ), 1)
from DetallePuntos d
where d.IdPuntos=@IdPuntos
order by d.DetalleId asc
for XMl path('')),1,1,'')),'~')
End
GO
/****** Object:  StoredProcedure [dbo].[uspEditaArea]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspEditaArea]
@detalle varchar(max)
as
Begin
Begin Transaction
Declare Tabla Cursor For Select * From fnSplitString(@detalle,';')	
Open Tabla
Declare @Columna varchar(max),
        @Id numeric(20),
        @Area varchar(80)
Declare @p1 int,@p2 int
Fetch Next From Tabla INTO @Columna
	While @@FETCH_STATUS = 0
	Begin
Set @p1 = CharIndex('|',@Columna,0)	
Set @p2 = Len(@Columna)+1

Set @Id=Convert(numeric(20),SUBSTRING(@Columna,1,@p1-1))
Set @Area=SUBSTRING(@Columna,@p1+1,@p2-@p1-1)

update Area
set AreaNombre=@Area
where AreaId=@Id

Fetch Next From Tabla INTO @Columna
end
	Close Tabla;
	Deallocate Tabla;
	Commit Transaction;
Select 'true'
end
GO
/****** Object:  StoredProcedure [dbo].[uspEditaBonificacion]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspEditaBonificacion]
@Data varchar(max)
as
begin
Declare @p1 int
Declare @p2 int
Declare @p3 int
declare @TemporalId numeric(38),
@Estado varchar(20),
@UsuarioID int
Set @Data = LTRIM(RTrim(@Data))
Set @p1 = CharIndex('|',@Data,0)
Set @p2 = CharIndex('|',@Data,@p1+1)
Set @p3 =Len(@Data)+1
Set @TemporalId =convert(numeric(38),SUBSTRING(@Data,1,@p1-1))
Set @Estado=SUBSTRING(@Data,@p1+1,@p2-@p1-1)
Set @UsuarioID=convert(int,SUBSTRING(@Data,@p2+1,@p3-@p2-1))
update TemporalCompra 
set PrecioCosto=0,DetalleImporte=0,DetalleDescuento=0,
DetalleEstado=@Estado 
where TemporalId=@TemporalId
select
isnull((select STUFF ((select '¬'+convert(varchar,t.TemporalId)+'|'+convert(varchar,t.IdProducto)+'|'+
t.DetalleCodigo+'|'+t.Descripcion+'|'+t.DetalleUM+'|'+
CONVERT(VarChar(50),cast(t.DetalleCantidad as money ), 1)+'|'+
convert(varchar,t.PrecioCosto)+'|'+convert(varchar,t.DetalleDescuento)
+'|'+convert(varchar,t.DetalleImporte)+'|'+CONVERT(varchar,t.ValorUM)+'|'+
t.DetalleEstado
from TemporalCompra t 
inner join Producto p 
on p.IdProducto=t.IdProducto 
where t.UsuarioID=@UsuarioID
order by t.TemporalId asc
for xml path('')),1,1,'')),'~')+'['+
isnull((select STUFF ((select '¬'+convert(varchar,u.IdUm)+'|'+convert(varchar,u.IdProducto)+'|'+
u.UMDescripcion+'|'+CONVERT(VarChar(50), cast(u.ValorUM as money ), 1)+'|'+
convert(varchar,t.PrecioCosto)
from UnidadMedida u
inner join TemporalCompra t
on t.IdProducto=u.IdProducto
where t.UsuarioID=@UsuarioID
order by u.ValorUM asc
for xml path('')),1,1,'')),'~')
end
GO
/****** Object:  StoredProcedure [dbo].[uspEditaDocNro]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspEditaDocNro]
@Data varchar(max)
as
begin
Declare @p1 int,@p2 int,@p3 int,@p4 int
Declare @DocuId numeric(38),@DocuNumero varchar(80),
@DocuEmision date,@DocuUsuario varchar(80)
Set @Data = LTRIM(RTrim(@Data))
Set @p1 = CharIndex('|',@Data,0)
Set @p2 = CharIndex('|',@Data,@p1+1)
Set @p3 = CharIndex('|',@Data,@p2+1)  
Set @p4 = Len(@Data)+1 
Set @DocuId=convert(numeric(38),SUBSTRING(@Data,1,@p1-1))
Set @DocuNumero=SUBSTRING(@Data,@p1+1,@p2-@p1-1) 
Set @DocuEmision=convert(date,SUBSTRING(@Data,@p2+1,@p3-@p2-1))
Set @DocuUsuario=SUBSTRING(@Data,@p3+1,@p4-@p3-1)
update DocumentoVenta
set DocuNumero=@DocuNumero,DocuEmision=@DocuEmision,
DocuUsuario=@DocuUsuario
where DocuId=@DocuId
select 'true'
end
GO
/****** Object:  StoredProcedure [dbo].[uspEditaDocu]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspEditaDocu]
@Data varchar(max)
as
begin
Declare @pos1 int
Declare @pos2 int
declare @pos3 int
declare @pos4 int
Declare @DocuId numeric(38),
@Numero varchar(40),
@DocuEmision date,
@Usuario varchar(40)
Set @Data = LTRIM(RTrim(@Data))
Set @pos1 = CharIndex('|',@Data,0)
Set @DocuId=convert(numeric(38),SUBSTRING(@Data,1,@pos1-1))
Set @pos2 = CharIndex('|',@Data,@pos1+1)
Set @Numero=SUBSTRING(@Data,@pos1+1,@pos2-@pos1-1)
Set @pos3= CharIndex('|',@Data,@pos2+1)
Set @DocuEmision=convert(date,SUBSTRING(@Data,@pos2+1,@pos3-@pos2-1))
Set @pos4= Len(@Data)+1
Set @Usuario=SUBSTRING(@Data,@pos3+1,@pos4-@pos3-1)
update DocumentoVenta
set DocuNumero=@Numero,DocuEmision=@DocuEmision,DocuUsuario=@Usuario
where DocuId=@DocuId
select 'true'
end
GO
/****** Object:  StoredProcedure [dbo].[uspEditaGuiaInterna]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspEditaGuiaInterna]    
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
        @pos5 int,@pos6 int,@pos7 int ,@pos8 int,@pos9 int   
Declare @GuiaId numeric(38),@FechaRegistro datetime,  
  @Concepto nvarchar(1),@Motivo varchar(300),@Origen varchar(300),  
  @Destino varchar(300),@Observacion varchar(max),@Usuario varchar(80),  
  @KardexDocu varchar(300),@UsuarioId int
      
Set @pos1 = CharIndex('|',@orden,0)    
Set @pos2 = CharIndex('|',@orden,@pos1+1)    
Set @pos3 = CharIndex('|',@orden,@pos2+1)    
Set @pos4 = CharIndex('|',@orden,@pos3+1)    
Set @pos5 = CharIndex('|',@orden,@pos4+1)    
Set @pos6= CharIndex('|',@orden,@pos5+1)  
Set @pos7= CharIndex('|',@orden,@pos6+1)
Set @pos8= CharIndex('|',@orden,@pos7+1)   
Set @pos9= Len(@orden)+1    
  
Set @FechaRegistro=convert(datetime,SUBSTRING(@orden,1,@pos1-1))    
Set @Concepto=SUBSTRING(@orden,@pos1+1,@pos2-@pos1-1)    
Set @Motivo=SUBSTRING(@orden,@pos2+1,@pos3-@pos2-1)    
Set @Origen=SUBSTRING(@orden,@pos3+1,@pos4-@pos3-1)    
Set @Destino=SUBSTRING(@orden,@pos4+1,@pos5-@pos4-1)    
Set @Observacion=SUBSTRING(@orden,@pos5+1,@pos6-@pos5-1)    
Set @Usuario=SUBSTRING(@orden,@pos6+1,@pos7-@pos6-1)  
Set @UsuarioId=SUBSTRING(@orden,@pos7+1,@pos8-@pos7-1)
Set @GuiaId=convert(numeric(38),SUBSTRING(@orden,@pos8+1,@pos9-@pos8-1))         
  
Begin Transaction
    
update GuiaInterna 
set FechaRegistro=@FechaRegistro,  
	Concepto=@Concepto,Motivo=@Motivo,Origen=@Origen,  
	Destino=@Destino,Observacion=@Observacion,Usuario=@Usuario
where GuiaId=@GuiaId	
    
if(@Concepto='I')set @KardexDocu='Guia De ING Interno'  
else set @KardexDocu='Guia De SAL Interno'  
  
Declare Tabla Cursor For Select * From fnSplitString(@detalle,';')     
Open Tabla    
  
Declare @Columna varchar(max),    
  @IdProducto numeric(20),    
  @Cantidad decimal(18,2),   
  @UM varchar(80),   
  @Descripcion varchar(max),  
  @Costo decimal(18,4),    
    
  @StockInicial decimal(18,2),    
  @StockFinal decimal(18,2),@CantidadIng decimal(18,2)  
      
Declare @p1 int,@p2 int,@p3 int,@p4 int,    
        @p5 int  
          
Fetch Next From Tabla INTO @Columna    
 While @@FETCH_STATUS = 0    
 Begin    
Set @p1 = CharIndex('|',@Columna,0)    
Set @p2 = CharIndex('|',@Columna,@p1+1)    
Set @p3 = CharIndex('|',@Columna,@p2+1)    
Set @p4 = CharIndex('|',@Columna,@p3+1)       
Set @p5= Len(@Columna)+1  
   
Set @IdProducto=Convert(numeric(20),SUBSTRING(@Columna,1,@p1-1))    
Set @Cantidad=convert(decimal(18,2),SUBSTRING(@Columna,@p1+1,@p2-(@p1+1)))    
Set @UM=SUBSTRING(@Columna,@p2+1,@p3-(@p2+1))    
Set @Descripcion=SUBSTRING(@Columna,@p3+1,@p4-(@p3+1))    
Set @Costo=Convert(decimal(18,4),SUBSTRING(@Columna,@p4+1,@p5-(@p4+1)))    
  
update DetalleGuiaInterna 
set Estado='E'
where GuiaId=@GuiaId    

if(@Concepto='I')
begin
  
set @StockInicial=(select top 1 ProductoCantidad from Producto(nolock)     
where IdProducto=@IdProducto)    
  
set @CantidadIng=(@Cantidad*1)    
set @StockFinal=@StockInicial+@CantidadIng    
  
update Producto    
set ProductoCantidad=ProductoCantidad+@CantidadIng    
where IdProducto=@IdProducto  
   
insert into Kardex values(@IdProducto,@FechaRegistro,@KardexDocu,convert(varchar,@GuiaId),@StockInicial,    
@CantidadIng,0,@Costo,@StockFinal,'INGRESO',@Usuario) 

end
else
begin

set @StockInicial=(select top 1 ProductoCantidad from Producto(nolock)     
where IdProducto=@IdProducto)    
  
set @CantidadIng=(@Cantidad*1)    
set @StockFinal=@StockInicial-@CantidadIng    
  
update Producto    
set ProductoCantidad=ProductoCantidad-@CantidadIng    
where IdProducto=@IdProducto  
   
insert into Kardex values(@IdProducto,@FechaRegistro,@KardexDocu,convert(varchar,@GuiaId),@StockInicial,    
0,@CantidadIng,@Costo,@StockFinal,'SALIDA',@Usuario) 

end 
    
Fetch Next From Tabla INTO @Columna    
end    
 Close Tabla;    
 Deallocate Tabla;     
 Commit Transaction;    
 SELECT 'true'    
end
GO
/****** Object:  StoredProcedure [dbo].[uspEditaHash]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspEditaHash]      
@orden varchar(Max)      
as      
begin      
Declare @pos1 int,@pos2 int  
  
Declare @NotaId numeric(38),  
        @DocuHASH varchar(max),@DocuId numeric(38)  
              
Set @pos1 = CharIndex('|',@orden,0)    
Set @pos2 = Len(@orden)+1  
  
Set @NotaId=convert(int,SUBSTRING(@orden,1,@pos1-1))  
set @DocuHASH=SUBSTRING(@orden,@pos1+1,@pos2-@pos1-1)  
  
set @DocuId=(select top 1 d.DocuId   
from DocumentoVenta d  
where NotaId=@NotaId and EstadoSunat='PENDIENTE'  
order by d.DocuId desc)  
     
update DocumentoVenta  
set DocuHash=@DocuHASH  
Where DocuId=@DocuId 

update  DetallePedido
set DetalleEstado='PENDIENTEB'
where NotaId=@NotaId
  
select 'true'      
  
end
GO
/****** Object:  StoredProcedure [dbo].[uspEditarNotaPedido]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[uspEditarNotaPedido]  
@Data VARCHAR(MAX)  
AS  
BEGIN  
  
SET NOCOUNT ON;  
  
BEGIN TRY  
BEGIN TRAN  
  
DECLARE   
@pos1 INT,  
@pos2 INT,  
@Cabecera VARCHAR(MAX),  
@Detalle VARCHAR(MAX),  
@NotaId INT  
  
SET @pos1 = CHARINDEX('[',@Data,0)  
SET @pos2 = LEN(@Data)+1  
  
SET @Cabecera = SUBSTRING(@Data,1,@pos1-1)  
SET @Detalle  = SUBSTRING(@Data,@pos1+1,@pos2-@pos1-1)  
  
DECLARE  
@p1 INT,@p2 INT,@p3 INT,@p4 INT,@p5 INT,@p6 INT,@p7 INT  
  
SET @p1 = CHARINDEX('|',@Cabecera,0)  
SET @p2 = CHARINDEX('|',@Cabecera,@p1+1)  
SET @p3 = CHARINDEX('|',@Cabecera,@p2+1)  
SET @p4 = CHARINDEX('|',@Cabecera,@p3+1)  
SET @p5 = CHARINDEX('|',@Cabecera,@p4+1)  
SET @p6 = CHARINDEX('|',@Cabecera,@p5+1)  
SET @p7 = LEN(@Cabecera)+1  
  
SET @NotaId = CONVERT(INT,SUBSTRING(@Cabecera,1,@p1-1))  
  
/* DEVOLVER STOCK ANTERIOR */  
  
UPDATE p  
SET p.ProductoCantidad = p.ProductoCantidad + (d.DetalleCantidad * ISNULL(NULLIF(d.ValorUM,0),1))  
FROM Producto p  
INNER JOIN DetallePedido d   
ON p.IdProducto = d.IdProducto  
WHERE d.NotaId = @NotaId  
  
  
/* ACTUALIZA CABECERA */  
  
UPDATE NotaPedido  
SET  
NotaDocu = SUBSTRING(@Cabecera,@p1+1,@p2-@p1-1),  
ClienteId = CONVERT(INT,SUBSTRING(@Cabecera,@p2+1,@p3-@p2-1)),  
NotaFecha = CONVERT(DATETIME,SUBSTRING(@Cabecera,@p3+1,@p4-@p3-1)),  
NotaUsuario = SUBSTRING(@Cabecera,@p4+1,@p5-@p4-1),  
NotaFormaPago = SUBSTRING(@Cabecera,@p5+1,@p6-@p5-1),  
NotaCondicion = SUBSTRING(@Cabecera,@p6+1,@p7-@p6-1)  
WHERE NotaId=@NotaId  
  
  
/* ELIMINA DETALLE ANTERIOR */  
  
DELETE FROM DetallePedido  
WHERE NotaId=@NotaId  
  
  
/* INSERTA NUEVO DETALLE */  
  
DECLARE  
@fila VARCHAR(MAX),  
@c1 INT,@c2 INT,@c3 INT,@c4 INT,@c5 INT,@c6 INT,@c7 INT,@c8 INT,@c9 INT,  
@IdProducto NUMERIC(20),  
@Cantidad DECIMAL(18,2),  
@ValorUM DECIMAL(18,6),  
@ValorUMSegment VARCHAR(60)  
  
WHILE LEN(@Detalle)>0  
BEGIN  
  
SET @c1 = CHARINDEX(';',@Detalle)  
  
IF @c1=0  
BEGIN  
 SET @fila=@Detalle  
 SET @Detalle=''  
END  
ELSE  
BEGIN  
 SET @fila=SUBSTRING(@Detalle,1,@c1-1)  
 SET @Detalle=SUBSTRING(@Detalle,@c1+1,LEN(@Detalle))  
END  
  
  
SET @c1 = CHARINDEX('|',@fila,0)  
SET @c2 = CHARINDEX('|',@fila,@c1+1)  
SET @c3 = CHARINDEX('|',@fila,@c2+1)  
SET @c4 = CHARINDEX('|',@fila,@c3+1)  
SET @c5 = CHARINDEX('|',@fila,@c4+1)  
SET @c6 = CHARINDEX('|',@fila,@c5+1)  
SET @c7 = CHARINDEX('|',@fila,@c6+1)  
SET @c8 = CHARINDEX('|',@fila,@c7+1)  
IF @c8=0 SET @c8 = LEN(@fila)+1  
SET @c9 = CHARINDEX('|',@fila,@c8+1)  
IF @c9=0 SET @c9 = LEN(@fila)+1  
  
  
SET @IdProducto = CONVERT(NUMERIC,SUBSTRING(@fila,1,@c1-1))  
SET @Cantidad   = CONVERT(DECIMAL,SUBSTRING(@fila,@c1+1,@c2-@c1-1))  
SET @ValorUM = 1  
SET @ValorUMSegment = ''  
IF @c8 < LEN(@fila)  
BEGIN  
 SET @ValorUMSegment = LTRIM(RTRIM(SUBSTRING(@fila,@c8+1,@c9-@c8-1)))  
END  
IF ISNUMERIC(REPLACE(@ValorUMSegment,',','.')) = 1  
BEGIN  
 SET @ValorUM = CONVERT(DECIMAL(18,6), REPLACE(@ValorUMSegment,',','.'))  
 IF @ValorUM<=0 SET @ValorUM=1  
END  
  
  
INSERT INTO DetallePedido  
(  
NotaId,  
IdProducto,  
DetalleCantidad,  
DetalleUm,  
DetalleDescripcion,  
DetalleCosto,  
DetallePrecio,  
DetalleImporte,  
DetalleEstado,  
ValorUM  
)  
VALUES  
(  
@NotaId,  
@IdProducto,  
@Cantidad,  
SUBSTRING(@fila,@c2+1,@c3-@c2-1),  
SUBSTRING(@fila,@c3+1,@c4-@c3-1),  
CONVERT(DECIMAL,SUBSTRING(@fila,@c4+1,@c5-@c4-1)),  
CONVERT(DECIMAL,SUBSTRING(@fila,@c5+1,@c6-@c5-1)),  
CONVERT(DECIMAL,SUBSTRING(@fila,@c6+1,@c7-@c6-1)),  
SUBSTRING(@fila,@c7+1,@c8-@c7-1),  
@ValorUM  
)  
  
  
/* DESCONTAR STOCK NUEVO */  
  
UPDATE Producto  
SET ProductoCantidad = ProductoCantidad - (@Cantidad * @ValorUM)  
WHERE IdProducto = @IdProducto  
  
  
END  
  
  
COMMIT TRAN  
  
SELECT 'UPDATED'  
  
END TRY  
BEGIN CATCH  
  
ROLLBACK TRAN  
  
SELECT ERROR_MESSAGE() AS Error  
  
END CATCH  
  
END  
GO
/****** Object:  StoredProcedure [dbo].[uspEditarRB]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- 2) Actualizar uspEditarRB
CREATE procedure [dbo].[uspEditarRB]
@Data varchar(max)
as
begin
    declare @p1 int,@p2 int,@p3 int,@p4 int,@p5 int;
    declare @ResumenId numeric(38),
            @CodigoSunat varchar(80),
            @MensajeSunat varchar(max),
            @HASHCDR varchar(max),
            @CDRBase64 varchar(max);

    set @Data = ltrim(rtrim(@Data));
    set @p1 = charindex('|', @Data, 0);
    set @p2 = charindex('|', @Data, @p1 + 1);
    set @p3 = charindex('|', @Data, @p2 + 1);
    set @p4 = charindex('|', @Data, @p3 + 1);
    set @p5 = len(@Data) + 1;

    if (@p4 = 0) set @p4 = @p5;

    set @ResumenId    = convert(numeric(38), substring(@Data, 1, @p1 - 1));
    set @CodigoSunat  = substring(@Data, @p1 + 1, @p2 - @p1 - 1);
    set @MensajeSunat = substring(@Data, @p2 + 1, @p3 - @p2 - 1);
    set @HASHCDR      = substring(@Data, @p3 + 1, @p4 - @p3 - 1);
    set @CDRBase64    = case when @p4 < @p5 then substring(@Data, @p4 + 1, @p5 - @p4 - 1) else '' end;

    update ResumenBoletas
       set CodigoSunat = @CodigoSunat,
           MensajeSunat = @MensajeSunat,
           HASHCDR = @HASHCDR,
           CDRBase64 = case when isnull(@CDRBase64,'')='' then CDRBase64 else @CDRBase64 end
     where ResumenId = @ResumenId;

    select 'true';
end
GO
/****** Object:  StoredProcedure [dbo].[uspEditarTemporal]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspEditarTemporal]
@Data varchar(max)
as
begin
Declare @pos1 int
Declare @pos2 int
Declare @pos3 int
Declare @pos4 int
Declare @pos5 int
Declare @pos6 int
declare @Id numeric(38),
@cantidad decimal(18,2),
@precioCosto decimal(18,4),
@Descuento decimal(18,4),
@importe decimal(18,2),
@UsuarioID int
Set @Data = LTRIM(RTrim(@Data))
Set @pos1 = CharIndex('|',@Data,0)
Set @pos2 = CharIndex('|',@Data,@pos1+1)
Set @pos3 = CharIndex('|',@Data,@pos2+1)
Set @pos4 = CharIndex('|',@Data,@pos3+1)
Set @pos5= CharIndex('|',@Data,@pos4+1)
Set @pos6 =Len(@Data)+1
Set @Id =convert(numeric(38),SUBSTRING(@Data,1,@pos1-1))
Set @cantidad=convert(decimal(18,2),SUBSTRING(@Data,@pos1+1,@pos2-@pos1-1))
Set @precioCosto=convert(decimal(18,4),SUBSTRING(@Data,@pos2+1,@pos3-@pos2-1))
Set @Descuento=convert(decimal(18,4),SUBSTRING(@Data,@pos3+1,@pos4-@pos3-1))
Set @importe=convert(decimal(18,4),SUBSTRING(@Data,@pos4+1,@pos5-@pos4-1))
Set @UsuarioID=convert(int,SUBSTRING(@Data,@pos5+1,@pos6-@pos5-1))
update TemporalCompra
set DetalleCantidad=@cantidad,PrecioCosto=@precioCosto,
DetalleDescuento=@Descuento,DetalleImporte=@importe
where TemporalId=@Id
select isnull((select STUFF ((select '¬'+convert(varchar,u.IdUm)+'|'+convert(varchar,u.IdProducto)+'|'+
u.UMDescripcion+'|'+CONVERT(VarChar(50), cast(u.ValorUM as money ), 1)+'|'+
convert(varchar,t.PrecioCosto)
from UnidadMedida u
inner join TemporalCompra t
on t.IdProducto=u.IdProducto
where t.UsuarioID=@UsuarioID
order by u.ValorUM asc
for xml path('')),1,1,'')),'true')
end
GO
/****** Object:  StoredProcedure [dbo].[uspEditarUM]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspEditarUM]
@detalle varchar(max)
as
Begin
Begin Transaction  
Declare Tabla Cursor For Select * From fnSplitString(@detalle,';')   
Open Tabla  
Declare @Columna varchar(max),  
        @Id numeric(38),
        @Costo decimal(18,4)  
Declare @p1 int,@p2 int  
Fetch Next From Tabla INTO @Columna  
 While @@FETCH_STATUS = 0  
 Begin  
Set @p1=CharIndex('|',@Columna,0)  
Set @p2=Len(@Columna)+1  
Set @Id=Convert(numeric(38),SUBSTRING(@Columna,1,@p1-1))
Set @Costo=Convert(decimal(18,4),SUBSTRING(@Columna,@p1+1,@p2-@p1-1))

update UnidadMedida 
set PrecioCosto=(@Costo * ValorUM) 
where IdUm=@Id
  
Fetch Next From Tabla INTO @Columna  
end  
 Close Tabla;  
 Deallocate Tabla;  
 Commit Transaction;  
SELECT 'true'
End
GO
/****** Object:  StoredProcedure [dbo].[uspEditaSubLinea]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspEditaSubLinea]
@detalle varchar(max)
as
Begin
Begin Transaction
Declare Tabla Cursor For Select * From fnSplitString(@detalle,';')	
Open Tabla
Declare @Columna varchar(max),
        @Id numeric(20),
        @Categoria varchar(300),
        @CodigoSunat varchar(40)
        
Declare @p1 int,@p2 int,@p3 int
Fetch Next From Tabla INTO @Columna
	While @@FETCH_STATUS = 0
	Begin
Set @p1 = CharIndex('|',@Columna,0)
Set @p2 = CharIndex('|',@Columna,@p1+1)	
Set @p3 = Len(@Columna)+1

Set @Id=Convert(numeric(20),SUBSTRING(@Columna,1,@p1-1))
Set @Categoria=SUBSTRING(@Columna,@p1+1,@p2-@p1-1)
Set @CodigoSunat=SUBSTRING(@Columna,@p2+1,@p3-@p2-1)

update Sublinea
set NombreSublinea=@Categoria,CodigoSunat=@CodigoSunat
where IdSubLinea=@Id

Fetch Next From Tabla INTO @Columna
end
	Close Tabla;
	Deallocate Tabla;
	Commit Transaction;
Select 'true'
end
GO
/****** Object:  StoredProcedure [dbo].[uspEditaTemporalVenta]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspEditaTemporalVenta]  
@Data varchar(max)  
as  
begin  
Declare @p1 int,@p2 int,  
        @p3 int,@p4 int,  
        @p5 int,@p6 int,
        @p7 int  
Declare @Id numeric(38),  
        @IdProducto numeric(20),  
        @Cantidad decimal(18,2),  
        @Unidad varchar(40),  
        @PrecioVenta decimal(18,2),  
        @Importe decimal(18,2),
        @Descripcion varchar(max)   
Set @Data = LTRIM(RTrim(@Data))  
Set @p1 = CharIndex('|',@Data,0)  
Set @p2 = CharIndex('|',@Data,@p1+1)  
Set @p3 = CharIndex('|',@Data,@p2+1)  
Set @p4 = CharIndex('|',@Data,@p3+1)  
Set @p5 = CharIndex('|',@Data,@p4+1)
Set @p6 = CharIndex('|',@Data,@p5+1)    
Set @p7= Len(@Data)+1  
Set @Id=convert(numeric(38),SUBSTRING(@Data,1,@p1-1))  
Set @IdProducto=convert(numeric(20),SUBSTRING(@Data,@p1+1,@p2-@p1-1))  
Set @Cantidad=convert(decimal(18,2),SUBSTRING(@Data,@p2+1,@p3-@p2-1))  
Set @Unidad=SUBSTRING(@Data,@p3+1,@p4-@p3-1)  
Set @PrecioVenta=convert(decimal(18,2),SUBSTRING(@Data,@p4+1,@p5-@p4-1))  
Set @Importe=convert(decimal(18,2),SUBSTRING(@Data,@p5+1,@p6-@p5-1))
Set @Descripcion=SUBSTRING(@Data,@p6+1,@p7-@p6-1)    
update TemporalVenta   
set cantidad=@Cantidad,precioventa=@PrecioVenta,  
importe=@Importe,Descripcion=@Descripcion  
where temporalId=@Id  
select 'true'  
end
GO
/****** Object:  StoredProcedure [dbo].[uspEliminaDetaCompra]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[uspEliminaDetaCompra]
@Columna varchar(max)
as 
begin
Declare @DetalleId numeric(38),    
        @IdProducto numeric(20),    
        @Documento varchar(255),        
        @Cantidad decimal(18,2),    
        @Costo  decimal(18,4), 
        @Estado varchar(60),
        @Motivo varchar(80),
        @Usuario varchar(80),
        @Concepto varchar(80)  
        Declare @p1 int,@p2 int,@p3 int,@p4 int,    
        @p5 int,@p6 int,@p7 int,@p8 int,@p9 int
        
Set @p1 = CharIndex('|',@Columna,0)      
Set @p2 = CharIndex('|',@Columna,@p1+1)      
Set @p3 = CharIndex('|',@Columna,@p2+1)      
Set @p4 = CharIndex('|',@Columna,@p3+1)      
Set @p5 = CharIndex('|',@Columna,@p4+1)
Set @p6 = CharIndex('|',@Columna,@p5+1)
Set @p7 = CharIndex('|',@Columna,@p6+1)
Set @p8 = CharIndex('|',@Columna,@p7+1)                
Set @p9=Len(@Columna)+1    
    
set @DetalleId=Convert(numeric(38),SUBSTRING(@Columna,1,@p1-1))        
set @IdProducto=Convert(numeric(20),SUBSTRING(@Columna,@p1+1,@p2-(@p1+1)))           
Set @Documento=SUBSTRING(@Columna,@p2+1,@p3-(@p2+1))          
Set @Cantidad=convert(decimal(18,2),SUBSTRING(@Columna,@p3+1,@p4-(@p3+1)))      
Set @Costo=convert(decimal(18,4),SUBSTRING(@Columna,@p4+1,@p5-(@p4+1)))           
Set @Estado=SUBSTRING(@Columna,@p5+1,@p6-(@p5+1))
Set @Motivo=SUBSTRING(@Columna,@p6+1,@p7-(@p6+1))
Set @Usuario=SUBSTRING(@Columna,@p7+1,@p8-(@p7+1))
Set @Concepto=SUBSTRING(@Columna,@p8+1,@p9-(@p8+1))        
        
Begin Transaction

delete from DetalleCompra 
where DetalleId=@DetalleId

if(@Concepto='MERCADERIA')
begin
declare @IniciaStock decimal(18,2),@stockFinal decimal(18,2)    
set @IniciaStock=(select top 1 p.ProductoCantidad 
from Producto p where p.IdProducto=@IdProducto)    

set @stockFinal=@IniciaStock-@Cantidad 

if(@Estado<>'PENDIENTE')
begin

update Producto     
set ProductoCantidad=ProductoCantidad-@Cantidad    
where IdProducto=@IdProducto 

insert into Kardex values(@IdProducto,DATEADD(HOUR, 1, GETDATE()),@Motivo,@Documento,@IniciaStock,      
0,@Cantidad,@Costo,@StockFinal,'INGRESO',@Usuario)   

END
END

Commit Transaction;     
select 'true'   
           
end
GO
/****** Object:  StoredProcedure [dbo].[uspEliminaDetalleGI]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspEliminaDetalleGI]  
@Columna varchar(max)    
as  
begin  
Declare  
  @DetalleId numeric(38),   
  @IdProducto numeric(20),    
  @Cantidad decimal(18,2),    
  @Costo decimal(18,4),    
  @Estado nvarchar(1)  
Declare @GuiaId numeric(38),@Usuario varchar(80),  
        @Concepto nvarchar(1)  
   
Declare @IniciaStock decimal(18,2),    
        @StockFinal decimal(18,2)    
  
Declare @pos1 int,@pos2 int,@pos3 int,  
        @pos4 int,@pos5 int,@pos6 int,  
        @pos7 int,@pos8 int  
  
Set @pos1 = CharIndex('|',@Columna,0)    
Set @pos2 = CharIndex('|',@Columna,@pos1+1)  
Set @pos3 = CharIndex('|',@Columna,@pos2+1)     
Set @pos4 = CharIndex('|',@Columna,@pos3+1)  
Set @pos5 = CharIndex('|',@Columna,@pos4+1)  
Set @pos6 = CharIndex('|',@Columna,@pos5+1)
Set @pos7 = CharIndex('|',@Columna,@pos6+1)            
Set @pos8 = Len(@Columna)+1  
  
Set @DetalleId=Convert(numeric(38),SUBSTRING(@Columna,1,@pos1-1))    
Set @IdProducto=Convert(numeric(20),SUBSTRING(@Columna,@pos1+1,@pos2-(@pos1+1)))    
Set @Cantidad=Convert(decimal(18,2),SUBSTRING(@Columna,@pos2+1,@pos3-(@pos2+1)))  
Set @Costo=Convert(decimal(18,4),SUBSTRING(@Columna,@pos3+1,@pos4-(@pos3+1)))  
  
  
Set @GuiaId=Convert(numeric(20),SUBSTRING(@Columna,@pos4+1,@pos5-(@pos4+1)))   
Set @Usuario=SUBSTRING(@Columna,@pos5+1,@pos6-(@pos5+1))    
Set @Concepto=SUBSTRING(@Columna,@pos6+1,@pos7-(@pos6+1)) 

Set @Estado=SUBSTRING(@Columna,@pos7+1,@pos8-(@pos7+1))      
  
Begin Transaction

if(@Estado='E')
begin   
 if(@Concepto='S')  
 begin  
   
 set @IniciaStock=(select top 1 ProductoCantidad from Producto where IdProducto=@IdProducto)    
 set @StockFinal=@IniciaStock-@Cantidad  
     
 insert into Kardex values(@IdProducto,DATEADD(HOUR, 1, GETDATE()),'Salida Por Anulacion Guia Interna',convert(varchar,@GuiaId),@IniciaStock,    
 0,@Cantidad,@Costo,@StockFinal,'SALIDA',@Usuario)   
     
 update producto     
 set  ProductoCantidad =ProductoCantidad - @Cantidad    
 where IDProducto=@IdProducto  
     
 end  
 else  
 begin  
   
 set @IniciaStock=(select top 1 ProductoCantidad from Producto where IdProducto=@IdProducto)    
 set @StockFinal=@IniciaStock+@Cantidad  
     
 insert into Kardex values(@IdProducto,DATEADD(HOUR, 1, GETDATE()),'Ingreso Por Anulacion Guia Interna',convert(varchar,@GuiaId),@IniciaStock,    
 @Cantidad,0,@Costo,@StockFinal,'INGRESO',@Usuario)   
     
 update producto     
 set  ProductoCantidad =ProductoCantidad + @Cantidad    
 where IDProducto=@IdProducto   
   
end
end
   
 delete from DetalleGuiaInterna  
 where DetalleId=@DetalleId  
   
 Commit Transaction;    
 select 'true'   
  
end
GO
/****** Object:  StoredProcedure [dbo].[uspEliminaGasto]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspEliminaGasto]  
@Data varchar(max)  
as  
begin  
declare @GastoId int  
Set @GastoId=convert(int,@Data)  
begin
  
 delete from GastosFijos   
 where GastoId=@GastoId
   
	Select isnull((select STUFF((select '¬'+ CONVERT(varchar,g.GastoId)+'|'+convert(varchar,g.GastoFecha,103)+'|'+  
	g.GsstoDesc+'|'+CONVERT(VarChar(50), cast(g.GstoMonto as money ), 1)+'|'+  
	(IsNull(convert(varchar,g.GastoReg,103),'')+' '+ IsNull(SUBSTRING(convert(varchar,g.GastoReg,114),1,8),''))+'|'+  
	g.GastoUsuario+'|'+g.Estado  
	from GastosFijos g    
	where month(g.GastoFecha)=month(GETDATE())and year(g.GastoFecha)=year(GETDATE())  
	order by g.GastoFecha asc,g.GastoId asc  
	FOR XML PATH('')), 1, 1, '')),'~')  

end  
end
GO
/****** Object:  StoredProcedure [dbo].[uspEliminaliquiVenta]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[uspEliminaliquiVenta]  
@ListaOrden varchar(Max)  
as  
begin  
Declare @pos1 int,@pos2 int  
Declare @orden varchar(max),  
        @detalle varchar(max)  
Set @pos1 = CharIndex('[',@ListaOrden,0)  
Set @pos2 =Len(@ListaOrden)+1  
Set @orden = SUBSTRING(@ListaOrden,1,@pos1-1)  
Set @detalle = SUBSTRING(@ListaOrden,@pos1+1,@pos2-@pos1-1)  
Declare @LiquidacionId numeric(38)  
set @LiquidacionId=@orden  
Begin Transaction  
Declare Tabla Cursor For Select * From fnSplitString(@detalle,';')   
Open Tabla  
Declare @Columna varchar(max),  
        @DetalleId nvarchar(38),  
        @NotaId nvarchar(38),  
        @Acuenta decimal(18,2)  
Declare @p1 int,@p2 int,@p3 int  
Fetch Next From Tabla INTO @Columna  
 While @@FETCH_STATUS = 0  
 Begin  
Set @p1 = CharIndex('|',@Columna,0)  
Set @p2 = CharIndex('|',@Columna,@p1+1)  
Set @p3 =Len(@Columna)+1  
set @DetalleId=SUBSTRING(@Columna,1,@p1-1)  
Set @Acuenta=Convert(decimal(18,2),SUBSTRING(@Columna,@p1+1,@p2-(@p1+1)))  
Set @NotaId=SUBSTRING(@Columna,@p2+1,@p3-(@p2+1))  
delete from CajaDetalle  
where LiquidaId=@DetalleId  
update NotaPedido  
set NotaSaldo=NotaSaldo + @Acuenta,NotaEstado='EMITIDO'  
where NotaId=@NotaId  
Fetch Next From Tabla INTO @Columna  
end  
 Close Tabla;  
 Deallocate Tabla;  
 delete from DetaLiquidaVenta  
 where LiquidacionId=@LiquidacionId  
 delete from LiquidacionVenta  
 where LiquidacionId=@LiquidacionId  
 Commit Transaction;  
 Select 'true'  
end
GO
/****** Object:  StoredProcedure [dbo].[uspEliminarArea]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspEliminarArea]
@Id int
as

BEGIN TRY
  DELETE FROM Area WHERE AreaId = @Id
END TRY
BEGIN CATCH

    DECLARE @ErrorNum INT = ERROR_NUMBER();
    --DECLARE @ErrorMsg NVARCHAR(200) = ERROR_MESSAGE();
    DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
    --DECLARE @ErrorState INT = ERROR_STATE();
	--PRINT 'Se encontró un error: ' + @ErrorMsg + ' (Número: ' + CAST(@ErrorNum AS VARCHAR) + ', Severidad: ' + CAST(@ErrorSeverity AS VARCHAR) + ')';

	 if(@ErrorNum=547 and @ErrorSeverity=16)
		begin 
			PRINT 'No se pudo eliminar, ya que tiene relacion con otros modulos'
		end 

END CATCH
GO
/****** Object:  StoredProcedure [dbo].[uspEliminarCaja]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspEliminarCaja]  
@CajaId varchar(38)  
as  
begin  
--Declare @CajaId nvarchar(40),@CajaIdB nvarchar(40),  
--  @Justificacion varchar(max),@Monto decimal(18,2),  
--  @Cajero varchar(80),@Autoriza varchar(80)  
--Declare @p1 int,@p2 int,@p3 int,@p4 int,  
--        @p5 int,@p6 int  
--Set @Data =LTRIM(RTrim(@Data))  
--Set @p1 =CharIndex('|',@Data,0)  
----Set @p2 =CharIndex('|',@Data,@p1+1)  
----Set @p3 =CharIndex('|',@Data,@p2+1)  
----Set @p4 =CharIndex('|',@Data,@p3+1)  
----Set @p5 =CharIndex('|',@Data,@p4+1)  
----Set @p6 =Len(@Data)+1   
--Set @CajaId=convert(int,SUBSTRING(@Data,1,@p1-1))  
--Set @CajaIdB=convert(numeric(38),SUBSTRING(@Data,@p1+1,@p2-@p1-1))  
--Set @Justificacion=SUBSTRING(@Data,@p2+1,@p3-@p2-1)  
--Set @Monto=convert(decimal(18,2),SUBSTRING(@Data,@p3+1,@p4-@p3-1))  
--Set @Cajero=SUBSTRING(@Data,@p4+1,@p5-@p4-1)  
--Set @Autoriza=SUBSTRING(@Data,@p5+1,@p6-@p5-1)  
Begin Transaction  
delete from CajaDetalle where CajaId=@CajaId  
delete from CajaPincipal where CajaId=@CajaId  
delete from Monedas where CajaId=@CajaId  
delete from Caja where CajaId=@CajaId  
--delete from MAYOLICA.dbo.CajaDetalle where CajaId=@CajaIdB  
--delete from MAYOLICA.dbo.CajaPincipal where CajaId=@CajaIdB  
--delete from MAYOLICA.dbo.Caja where CajaId=@CajaIdB  
--insert into logCaja values(GETDATE(),convert(varchar,@CajaId),'ELIMINA',  
--'ELIMINA CAJA PRINCIPAL',@Justificacion+' NRO-'+convert(varchar,@CajaId),@Monto,@Cajero,@Autoriza)   
Commit Transaction;  
select 'true'  
end
GO
/****** Object:  StoredProcedure [dbo].[uspEliminarCategoria]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspEliminarCategoria]
@Id int
as

BEGIN TRY
  DELETE FROM Sublinea WHERE IdSubLinea = @Id
END TRY
BEGIN CATCH

    DECLARE @ErrorNum INT = ERROR_NUMBER();
    --DECLARE @ErrorMsg NVARCHAR(200) = ERROR_MESSAGE();
    DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
    --DECLARE @ErrorState INT = ERROR_STATE();
	--PRINT 'Se encontró un error: ' + @ErrorMsg + ' (Número: ' + CAST(@ErrorNum AS VARCHAR) + ', Severidad: ' + CAST(@ErrorSeverity AS VARCHAR) + ')';

	 if(@ErrorNum=547 and @ErrorSeverity=16)
		begin 
			PRINT 'No se pudo eliminar, ya que tiene relacion con otros modulos'
		end 

END CATCH
GO
/****** Object:  StoredProcedure [dbo].[uspEliminarCliente]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspEliminarCliente]
@Id int
as

BEGIN TRY
  DELETE FROM Cliente WHERE ClienteId = @Id
END TRY
BEGIN CATCH

    DECLARE @ErrorNum INT = ERROR_NUMBER();
    --DECLARE @ErrorMsg NVARCHAR(200) = ERROR_MESSAGE();
    DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
    --DECLARE @ErrorState INT = ERROR_STATE();
	--PRINT 'Se encontró un error: ' + @ErrorMsg + ' (Número: ' + CAST(@ErrorNum AS VARCHAR) + ', Severidad: ' + CAST(@ErrorSeverity AS VARCHAR) + ')';

	 if(@ErrorNum=547 and @ErrorSeverity=16)
		begin 
			PRINT 'No se pudo eliminar, ya que tiene relacion con otros modulos'
		end 

END CATCH
GO
/****** Object:  StoredProcedure [dbo].[uspEliminarCompraB]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspEliminarCompraB]    
@ListaOrden varchar(Max)    
as    
begin    
Declare @pos int    
Declare @CompraId varchar(max)    
Declare @detalle varchar(max)    
Set @pos = CharIndex('[',@ListaOrden,0)    
Set @CompraId= SUBSTRING(@ListaOrden,1,@pos-1)    
Set @detalle = SUBSTRING(@ListaOrden,@pos+1,len(@ListaOrden)-@pos)    
    
Begin Transaction    
Declare Tabla Cursor For Select * From fnSplitString(@detalle,';')     
Open Tabla    
Declare @Columna varchar(max)    
Declare @c1 int,@c2 int,@c3 int,
        @c4 int,@c5 int,@c6 int,
        @c7 int
Declare @IdProducto numeric(20),    
  @KardexMotivo  varchar(60),    
  @KardexDocumento varchar(60),    
  @CantidadSalida decimal(18, 2),    
  @PrecioCosto decimal(18,4),    
  @Usuario varchar(60),
  @AplicaINV nvarchar(1)    
Fetch Next From Tabla INTO @Columna    
 While @@FETCH_STATUS = 0    
 Begin    
Set @c1=CharIndex('|',@Columna,0)    
Set @c2=CharIndex('|',@Columna,@c1+1)    
Set @c3=CharIndex('|',@Columna,@c2+1)    
Set @c4=CharIndex('|',@Columna,@c3+1)    
Set @c5=CharIndex('|',@Columna,@c4+1)
Set @c6=CharIndex('|',@Columna,@c5+1)    
Set @c7=Len(@Columna)+1    
Set @IdProducto=convert(numeric(20),SUBSTRING(@Columna,1,@c1-1))    
Set @KardexMotivo=SUBSTRING(@Columna,@c1+1,@c2-@c1-1)    
Set @KardexDocumento=SUBSTRING(@Columna,@c2+1,@c3-@c2-1)    
set @CantidadSalida=convert(decimal(18,2),SUBSTRING(@Columna,@c3+1,@c4-@c3-1))    
set @PrecioCosto=convert(decimal(18,2),SUBSTRING(@Columna,@c4+1,@c5-@c4-1))    
set @Usuario=SUBSTRING(@Columna,@c5+1,@c6-@c5-1)
set @AplicaINV=SUBSTRING(@Columna,@c6+1,@c7-@c6-1)      

if(@AplicaINV='S')
begin    

declare @IniciaStock decimal(18,2),@StockFinal decimal(18,2),@Concepto varchar(40)    
set @IniciaStock=(select top 1 ProductoCantidad from Producto where IdProducto=@IdProducto)    
    
set @StockFinal=@IniciaStock-@CantidadSalida    
    
insert into Kardex values(@IdProducto,GETDATE(),@KardexMotivo,@KardexDocumento,@IniciaStock,    
0,@CantidadSalida,@PrecioCosto,@StockFinal,'SALIDA',@Usuario)    
    
update producto     
set    ProductoCantidad =ProductoCantidad - @CantidadSalida  
where  IDProducto=@IdProducto

End  
  
Fetch Next From Tabla INTO @Columna    
end    
 Close Tabla;    
 Deallocate Tabla;    
    delete from DetalleCompra     
    where CompraId=@CompraId    
    delete from Compras    
    where CompraId=@CompraId    
 Commit Transaction;    
 select 'true'     
end
GO
/****** Object:  StoredProcedure [dbo].[uspEliminarFeriado]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[uspEliminarFeriado]
    @Id int
AS
BEGIN
    DELETE FROM dbo.Feriados WHERE IdFeriado = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[uspEliminarPersonal]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspEliminarPersonal]
@Id numeric(20)
as

BEGIN TRY
  DELETE FROM Personal WHERE PersonalId =@Id
END TRY
BEGIN CATCH

    DECLARE @ErrorNum INT = ERROR_NUMBER();
    --DECLARE @ErrorMsg NVARCHAR(200) = ERROR_MESSAGE();
    DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
    --DECLARE @ErrorState INT = ERROR_STATE();
	--PRINT 'Se encontró un error: ' + @ErrorMsg + ' (Número: ' + CAST(@ErrorNum AS VARCHAR) + ', Severidad: ' + CAST(@ErrorSeverity AS VARCHAR) + ')';

	 if(@ErrorNum=547 and @ErrorSeverity=16)
		begin 
			PRINT 'No se pudo eliminar, ya que tiene relacion con otros modulos'
		end 

END CATCH
GO
/****** Object:  StoredProcedure [dbo].[uspEliminarProducto]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[uspEliminarProducto]  
@Id int  
as  
BEGIN TRY

  DELETE FROM Producto WHERE IdProducto = @Id
  DELETE FROM Kardex   WHERE IdProducto = @Id
  DELETE FROM UnidadMedida WHERE IdProducto = @Id

END TRY  
BEGIN CATCH  

    DECLARE @ErrorNum INT = ERROR_NUMBER();  
    DECLARE @ErrorSeverity INT = ERROR_SEVERITY();  
  
  if(@ErrorNum=547 and @ErrorSeverity=16)  
  begin   
   PRINT 'No se pudo eliminar, ya que tiene relacion con otros modulos'  
  end   
  
END CATCH  
GO
/****** Object:  StoredProcedure [dbo].[uspEliminarProveedor]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspEliminarProveedor]  
@Id int  
as  
BEGIN TRY

  DELETE FROM Proveedor WHERE ProveedorId = @Id

END TRY  
BEGIN CATCH  

    DECLARE @ErrorNum INT = ERROR_NUMBER();  
    DECLARE @ErrorSeverity INT = ERROR_SEVERITY();  
  
  if(@ErrorNum=547 and @ErrorSeverity=16)  
  begin   
   PRINT 'No se pudo eliminar, ya que tiene relacion con otros modulos'  
  end   
  
END CATCH 

GO
/****** Object:  StoredProcedure [dbo].[uspEliminarPuntos]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspEliminarPuntos]
@Data varchar(40)
as
begin
Begin Transaction
delete from DetallePuntos
where IdPuntos=@Data
delete from PasarPuntos
where IdPuntos=@Data
Commit Transaction;
select 'true'
end
GO
/****** Object:  StoredProcedure [dbo].[uspEliminarUsuario]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspEliminarUsuario] 
@Id int  
as  
BEGIN TRY

  DELETE FROM Usuarios WHERE UsuarioID = @Id

END TRY  
BEGIN CATCH  

    DECLARE @ErrorNum INT = ERROR_NUMBER();  
    DECLARE @ErrorSeverity INT = ERROR_SEVERITY();  
  
  if(@ErrorNum=547 and @ErrorSeverity=16)  
  begin   
   PRINT 'No se pudo eliminar, ya que tiene relacion con otros modulos'  
  end   
  
END CATCH 

GO
/****** Object:  StoredProcedure [dbo].[uspGasto]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspGasto]    
as    
begin    
select    
'Id|Fecha|Descripcion|Monto|FechaRe|Usuario|Estado¬90|120|415|125|100|90|90¬String|String|String|String|String|String|String¬'+    
isnull((select STUFF((select '¬'+ CONVERT(varchar,g.GastoId)+'|'+convert(varchar,g.GastoFecha,103)+'|'+    
g.GsstoDesc+'|'+CONVERT(VarChar(50), cast(g.GstoMonto as money ), 1)+'|'+    
(IsNull(convert(varchar,g.GastoReg,103),'')+' '+ IsNull(SUBSTRING(convert(varchar,g.GastoReg,114),1,8),''))+'|'+    
g.GastoUsuario+'|'+g.Estado    
from GastosFijos g     
where month(g.GastoFecha)=month(GETDATE())and year(g.GastoFecha)=year(GETDATE())    
order by g.GastoId desc    
FOR XML PATH('')), 1, 1, '')),'~')    
end  
GO
/****** Object:  StoredProcedure [dbo].[uspGastoFecha]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspGastoFecha]  
@fechainicio date,  
@fechafin date  
as  
begin
select  
'Id|Fecha|Descripcion|Monto|FechaRe|Usuario|Estado¬90|120|415|125|100|90|90¬String|String|String|String|String|String|String¬'+  
isnull((select STUFF((select '¬'+ CONVERT(varchar,g.GastoId)+'|'+convert(varchar,g.GastoFecha,103)+'|'+  
g.GsstoDesc+'|'+CONVERT(VarChar(50), cast(g.GstoMonto as money ), 1)+'|'+  
(IsNull(convert(varchar,g.GastoReg,103),'')+' '+ IsNull(SUBSTRING(convert(varchar,g.GastoReg,114),1,8),''))+'|'+  
g.GastoUsuario+'|'+g.Estado  
from GastosFijos g  
where (Convert(char(10),g.GastoFecha,101) BETWEEN @fechainicio AND @fechafin)  
order by g.GastoFecha asc,g.GastoId asc  
FOR XML PATH('')), 1, 1, '')),'~')  
end
GO
/****** Object:  StoredProcedure [dbo].[uspGuardarCredencialesSunat]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[uspGuardarCredencialesSunat]
    @CompaniaId INT,
    @UsuarioSOL VARCHAR(100),
    @ClaveSOL VARCHAR(100),
    @CertificadoBase64 VARCHAR(MAX),
    @ClaveCertificado VARCHAR(100),
    @Entorno INT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM Compania WHERE CompaniaId = @CompaniaId)
    BEGIN
        UPDATE Compania
        SET 
            CompaniaUserSecun = @UsuarioSOL,
            ComapaniaPWD      = @ClaveSOL,
            CompaniaPFX       = @CertificadoBase64,
            CompaniaClave     = @ClaveCertificado,
            TIPO_PROCESO      = @Entorno
        WHERE CompaniaId = @CompaniaId;
    END
    ELSE
    BEGIN
        INSERT INTO Compania (
            CompaniaId,
            CompaniaUserSecun,
            ComapaniaPWD,
            CompaniaPFX,
            CompaniaClave,
            TIPO_PROCESO
        )
        VALUES (
            @CompaniaId,
            @UsuarioSOL,
            @ClaveSOL,
            @CertificadoBase64,
            @ClaveCertificado,
            @Entorno
        );
    END
END

GO
/****** Object:  StoredProcedure [dbo].[uspGuardarUnidadMedidaProducto]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[uspGuardarUnidadMedidaProducto]    
(    
    @IdProducto NUMERIC(20),    
    @UMDescripcion VARCHAR(100),    
    @ValorUM DECIMAL(18,2),    
    @PrecioVenta DECIMAL(18,2),    
    @PrecioVentaB DECIMAL(18,2),    
    @PrecioCosto DECIMAL(18,2),    
    @UnidadImagen VARCHAR(MAX) = NULL    
)    
AS    
BEGIN    
    SET NOCOUNT ON;    
    
    DECLARE @IdUm INT;    
     
    SET @UMDescripcion = LTRIM(RTRIM(@UMDescripcion));    
    SET @UnidadImagen = NULLIF(LTRIM(RTRIM(ISNULL(@UnidadImagen, ''))), '');    
    
    SELECT TOP 1 @IdUm = IdUm    
    FROM UnidadMedida    
    WHERE IdProducto = @IdProducto    
    AND UPPER(LTRIM(RTRIM(UMDescripcion))) = UPPER(@UMDescripcion);    
    
    IF @IdUm IS NOT NULL    
    BEGIN    
        UPDATE UnidadMedida    
        SET     
            ValorUM = @ValorUM,    
            PrecioVenta = @PrecioVenta,    
            PrecioVentaB = @PrecioVentaB,    
            PrecioCosto = @PrecioCosto,    
            unidadImagen = @UnidadImagen    
        WHERE IdUm = @IdUm;    
    
        SELECT @IdUm AS IdUm;    
        RETURN;    
    END    
    
    INSERT INTO UnidadMedida    
    (    
        IdProducto,    
        UMDescripcion,    
        ValorUM,    
        PrecioVenta,    
        PrecioVentaB,    
        PrecioCosto,    
        unidadImagen    
    )    
    VALUES    
    (    
        @IdProducto,    
        @UMDescripcion,    
        @ValorUM,    
        @PrecioVenta,    
        @PrecioVentaB,    
        @PrecioCosto,    
        @UnidadImagen    
    );    
    
    SELECT SCOPE_IDENTITY() AS IdUm;    
END  
GO
/****** Object:  StoredProcedure [dbo].[uspHistoria]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspHistoria]
@ClienteId numeric(20),
@IdProducto numeric(20)
as
begin
select
'FechaVenta|PrecioUni|Cantidad|UM|Vendedor¬140|100|100|80|150¬String|String|String|String|String¬'+
isnull((select stuff((select '¬'+(IsNull(convert(varchar,n.NotaFecha,103),'')+' '+ IsNull(SUBSTRING(convert(varchar,n.NotaFecha,114),1,8),''))+'|'+
CONVERT(VarChar(50),cast(d.DetallePrecio as money ), 1)+'|'+
CONVERT(VarChar(50),cast(d.DetalleCantidad as money ), 1)+'|'+d.DetalleUm+'|'+
n.NotaUsuario
from DetallePedido d 
inner join NotaPedido n 
on n.NotaId=d.NotaId
where n.ClienteId=@ClienteId and (d.IdProducto=@IdProducto and n.NotaEstado<>'PENDIENTE') 
order by n.NotaFecha desc
for xml path('')),1,1,'')),'~')
end
GO
/****** Object:  StoredProcedure [dbo].[uspIngresarFeriado]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[uspIngresarFeriado]
@Data varchar(max) -- Formato: Id|MM-dd-yyyy|Motivo
AS
BEGIN

DECLARE @Id int, @Fecha date, @Motivo varchar(300);
Declare @pos1 int, @pos2 int,@pos3 int;

Set @Data= LTRIM(RTrim(@Data))  
Set @pos1=CharIndex('|',@Data,0)  
Set @pos2=CharIndex('|',@Data,@pos1+1)  
Set @pos3=Len(@Data)+1

Set @Id=convert(int,SUBSTRING(@Data,1,@pos1-1))  
Set @Fecha=convert(date,SUBSTRING(@Data,@pos1+1,@pos2-@pos1-1))  
Set @Motivo=SUBSTRING(@Data,@pos2+1,@pos3-@pos2-1)  

IF ISNULL(@Id, 0) = 0
    BEGIN
	 IF EXISTS(select top 1 f.Fecha  
			 from Feriados f where f.Fecha=@Fecha)   
	 begin  
	   select 'existe feriado'   
	 End
	 Else
	   Begin

        INSERT INTO Feriados(Fecha, Motivo)
        VALUES (@Fecha, @Motivo);

        SELECT @@identity AS IdFeriado;
       
	   END
	END
    ELSE
    BEGIN
	IF EXISTS(select top 1 f.Fecha  
			 from Feriados f where f.Fecha=@Fecha AND IdFeriado <> @Id)   
	 begin  
	   select 'existe feriado'   
	 End
	 Else
	 Begin
        UPDATE Feriados
        SET Fecha = @Fecha,
            Motivo = @Motivo
        WHERE IdFeriado = @Id;

        SELECT @Id AS IdFeriado;
     End
	END
END
GO
/****** Object:  StoredProcedure [dbo].[uspIngresarPersonal]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspIngresarPersonal]
@Data varchar(max)
as
Begin

Declare @Id numeric(20),@PersonalNombres varchar(140),    
		@PersonalApellidos varchar(140),@AreaId numeric(20),    
		@PersonalCodigo varchar (80),@PersonalNacimiento date,    
		@PersonalIngreso date,@PersonalDNI varchar(20),    
		@PersonalDireccion varchar(140),@PersonalTelefono varchar(40),     
		@PersonalEmail varchar(100),@PersonalEstado varchar(60),    
		@PersonalImagen varchar(max),@CompaniaId int
		
Declare @pos1 int, @pos2 int,@pos3 int,  
        @pos4 int, @pos5 int,@pos6 int,
		@pos7 int, @pos8 int,@pos9 int,
		@pos10 int,@pos11 int,@pos12 int,
		@pos13 int,@pos14 int

Set @Data= LTRIM(RTrim(@Data))  
Set @pos1=CharIndex('|',@Data,0)  
Set @pos2=CharIndex('|',@Data,@pos1+1)  
Set @pos3=CharIndex('|',@Data,@pos2+1)  
Set @pos4=CharIndex('|',@Data,@pos3+1)  
Set @pos5=CharIndex('|',@Data,@pos4+1)
Set @pos6=CharIndex('|',@Data,@pos5+1)  
Set @pos7=CharIndex('|',@Data,@pos6+1)  
Set @pos8=CharIndex('|',@Data,@pos7+1) 
Set @pos9=CharIndex('|',@Data,@pos8+1)  
Set @pos10=CharIndex('|',@Data,@pos9+1)  
Set @pos11=CharIndex('|',@Data,@pos10+1)
Set @pos12=CharIndex('|',@Data,@pos11+1)
Set @pos13=CharIndex('|',@Data,@pos12+1)
Set @pos14=Len(@Data)+1  

Set @Id=convert(int,SUBSTRING(@Data,1,@pos1-1))  
Set @PersonalNombres=SUBSTRING(@Data,@pos1+1,@pos2-@pos1-1)  
Set @PersonalApellidos=SUBSTRING(@Data,@pos2+1,@pos3-@pos2-1)  
Set @AreaId=SUBSTRING(@Data,@pos3+1,@pos4-@pos3-1)  
Set @PersonalCodigo=SUBSTRING(@Data,@pos4+1,@pos5-@pos4-1)  
Set @PersonalNacimiento=convert(date,SUBSTRING(@Data,@pos5+1,@pos6-@pos5-1))
Set @PersonalIngreso=convert(date,SUBSTRING(@Data,@pos6+1,@pos7-@pos6-1))
Set @PersonalDNI=SUBSTRING(@Data,@pos7+1,@pos8-@pos7-1)  
Set @PersonalDireccion=SUBSTRING(@Data,@pos8+1,@pos9-@pos8-1)  
Set @PersonalTelefono=SUBSTRING(@Data,@pos9+1,@pos10-@pos9-1)  
Set @PersonalEmail=SUBSTRING(@Data,@pos10+1,@pos11-@pos10-1)
Set @PersonalEstado=SUBSTRING(@Data,@pos11+1,@pos12-@pos11-1)
Set @PersonalImagen=SUBSTRING(@Data,@pos12+1,@pos13-@pos12-1)
Set @CompaniaId=convert(int,SUBSTRING(@Data,@pos13+1,@pos14-@pos13-1)) 

if(@Id=0)
Begin

 IF EXISTS(select top 1 p.PersonalDNI  
      from Personal p where p.PersonalDNI=@PersonalDNI and p.PersonalDNI<>'')   
 begin  
   select 'existe DNI'   
 end
 Else
  Begin
	insert into Personal values    
		(@PersonalNombres,@PersonalApellidos,@AreaId,@PersonalCodigo,    
		 @PersonalNacimiento,@PersonalIngreso,@PersonalDNI,@PersonalDireccion,    
		 @PersonalTelefono,@PersonalEmail,'ACTIVO',    
		 @PersonalImagen,@CompaniaId)
	
	select 'true'
  end
End
Else
Begin

IF EXISTS(select top 1 p.PersonalDNI  
      from Personal p where p.PersonalDNI=@PersonalDNI and p.PersonalId<>@Id and p.PersonalDNI<>'')   
 begin  
   select 'existe DNI'   
 end
Else
Begin
	update Personal  
    set PersonalNombres=@PersonalNombres,PersonalApellidos=@PersonalApellidos,
			AreaId=@AreaId,PersonalCodigo=@PersonalCodigo,PersonalNacimiento=@PersonalNacimiento,  
			PersonalIngreso=@PersonalIngreso,PersonalDNI=@PersonalDNI,
			PersonalDireccion=@PersonalDireccion,PersonalTelefono=@PersonalTelefono,  
			PersonalEmail=@PersonalEmail,PersonalEstado=@PersonalEstado,
			PersonalImagen=@PersonalImagen,CompaniaId=@CompaniaId  
	where PersonalId=@Id
	
	select 'true'
END
END
End 
GO
/****** Object:  StoredProcedure [dbo].[uspIngresarProducto]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[uspIngresarProducto]      
 @Data VARCHAR(MAX)      
AS      
BEGIN      
    SET NOCOUNT ON;    
    
    DECLARE @Id NUMERIC(20),    
            @IdSubLinea NUMERIC(20),                  
            @ProductoCodigo VARCHAR(300),                  
            @ProductoNombre VARCHAR(300),                         
            @ProductoUM VARCHAR(60),                  
            @ProductoCosto DECIMAL(18,4),                  
            @ProductoVenta DECIMAL(18,2),                  
            @ProductoVentaB DECIMAL(18,2),                  
            @ProductoCantidad DECIMAL(18,2),                  
            @ProductoEstado VARCHAR(60),                  
            @ProductoUsuario VARCHAR(60),                  
            @ProductoImagen VARCHAR(MAX),                  
            @ValorCritico DECIMAL(18,2),              
            @AplicaINV NVARCHAR(1)    
    
    DECLARE @posOpen INT = 0,     
            @posClose INT = 0    
    
    DECLARE @cabecera VARCHAR(MAX),     
            @detalleUM VARCHAR(MAX)    
    
    IF CHARINDEX('[', @Data) > 0 AND CHARINDEX(']', @Data) > 0    
    BEGIN    
        SET @posOpen = CHARINDEX('[', @Data)    
        SET @posClose = CHARINDEX(']', @Data)    
    
        SET @cabecera = SUBSTRING(@Data, 1, @posOpen - 1)    
        SET @detalleUM = SUBSTRING(@Data, @posOpen + 1, @posClose - @posOpen - 1)    
    END    
    ELSE    
    BEGIN    
        SET @cabecera = @Data    
        SET @detalleUM = ''    
    END    
    
    DECLARE @pos1 INT,@pos2 INT,@pos3 INT,@pos4 INT,@pos5 INT,@pos6 INT,    
            @pos7 INT,@pos8 INT,@pos9 INT,@pos10 INT,@pos11 INT,    
            @pos12 INT,@pos13 INT,@pos14 INT      
    
    SET @cabecera = LTRIM(RTRIM(@cabecera))    
    
    SET @pos1=CHARINDEX('|',@cabecera,0)        
    SET @pos2=CHARINDEX('|',@cabecera,@pos1+1)        
    SET @pos3=CHARINDEX('|',@cabecera,@pos2+1)        
    SET @pos4=CHARINDEX('|',@cabecera,@pos3+1)        
    SET @pos5=CHARINDEX('|',@cabecera,@pos4+1)      
    SET @pos6=CHARINDEX('|',@cabecera,@pos5+1)        
    SET @pos7=CHARINDEX('|',@cabecera,@pos6+1)        
    SET @pos8=CHARINDEX('|',@cabecera,@pos7+1)       
    SET @pos9=CHARINDEX('|',@cabecera,@pos8+1)        
    SET @pos10=CHARINDEX('|',@cabecera,@pos9+1)        
    SET @pos11=CHARINDEX('|',@cabecera,@pos10+1)      
    SET @pos12=CHARINDEX('|',@cabecera,@pos11+1)      
    SET @pos13=CHARINDEX('|',@cabecera,@pos12+1)      
    SET @pos14=LEN(@cabecera)+1        
    
    SET @Id=CONVERT(NUMERIC(20),SUBSTRING(@cabecera,1,@pos1-1))        
    SET @IdSubLinea=CONVERT(NUMERIC(20),SUBSTRING(@cabecera,@pos1+1,@pos2-@pos1-1))        
    SET @ProductoCodigo=SUBSTRING(@cabecera,@pos2+1,@pos3-@pos2-1)        
    SET @ProductoNombre=SUBSTRING(@cabecera,@pos3+1,@pos4-@pos3-1)        
    SET @ProductoUM=SUBSTRING(@cabecera,@pos4+1,@pos5-@pos4-1)        
    SET @ProductoCosto=CONVERT(DECIMAL(18,4),SUBSTRING(@cabecera,@pos5+1,@pos6-@pos5-1))      
    SET @ProductoVenta=CONVERT(DECIMAL(18,2),SUBSTRING(@cabecera,@pos6+1,@pos7-@pos6-1))      
    SET @ProductoVentaB=CONVERT(DECIMAL(18,2),SUBSTRING(@cabecera,@pos7+1,@pos8-@pos7-1))        
    SET @ProductoCantidad=CONVERT(DECIMAL(18,2),SUBSTRING(@cabecera,@pos8+1,@pos9-@pos8-1))        
    SET @ProductoEstado=SUBSTRING(@cabecera,@pos9+1,@pos10-@pos9-1)        
    SET @ProductoUsuario=SUBSTRING(@cabecera,@pos10+1,@pos11-@pos10-1)      
    SET @ProductoImagen=SUBSTRING(@cabecera,@pos11+1,@pos12-@pos11-1)      
    SET @ValorCritico=CONVERT(DECIMAL(18,2),SUBSTRING(@cabecera,@pos12+1,@pos13-@pos12-1))      
    SET @AplicaINV=SUBSTRING(@cabecera,@pos13+1,@pos14-@pos13-1)    
    
    DECLARE @IdProductoFinal NUMERIC(20)    
    
    IF(@Id=0)      
    BEGIN      
        IF EXISTS(SELECT 1 FROM Producto WHERE ProductoCodigo=@ProductoCodigo)         
        BEGIN        
            SELECT 'existe Codigo'         
            RETURN    
        END      
    
        INSERT INTO Producto    
        VALUES(@IdSubLinea,@ProductoCodigo,@ProductoNombre,    
               @ProductoUM,@ProductoCosto,@ProductoVenta,@ProductoVentaB,    
               @ProductoCantidad,'ACTIVO',@ProductoUsuario,GETDATE(),    
               @ProductoImagen,@ValorCritico,@AplicaINV)                  
    
        SET @IdProductoFinal = SCOPE_IDENTITY()    
    
        IF(@AplicaINV='S')          
        BEGIN                  
            INSERT INTO Kardex     
            VALUES(@IdProductoFinal,GETDATE(),    
            'Nuevo Registro','Nuevo Registro',0,    
            @ProductoCantidad,0,@ProductoCosto,@ProductoCantidad,    
            'INGRESO',@ProductoUsuario)                
        END      
  
        IF LTRIM(RTRIM(@detalleUM)) <> ''    
        BEGIN    
            DECLARE @item VARCHAR(MAX), @pos INT    
  
            WHILE LEN(@detalleUM) > 0    
            BEGIN    
                SET @pos = CHARINDEX(';', @detalleUM)    
  
                IF @pos > 0    
                BEGIN    
                    SET @item = SUBSTRING(@detalleUM, 1, @pos - 1)    
                    SET @detalleUM = SUBSTRING(@detalleUM, @pos + 1, LEN(@detalleUM))    
                END    
                ELSE    
                BEGIN    
                    SET @item = @detalleUM    
                    SET @detalleUM = ''    
                END    
  
                IF LTRIM(RTRIM(@item)) <> ''    
                BEGIN    
                    DECLARE @u1 INT,@u2 INT,@u3 INT,@u4 INT,@u5 INT    
                    DECLARE @PrecioCostoUM VARCHAR(60), @UnidadImagenUM VARCHAR(MAX)    
  
                    SET @u1 = CHARINDEX('|', @item)    
                    SET @u2 = CHARINDEX('|', @item, @u1 + 1)    
                    SET @u3 = CHARINDEX('|', @item, @u2 + 1)    
                    SET @u4 = CHARINDEX('|', @item, @u3 + 1)    
                    SET @u5 = CHARINDEX('|', @item, @u4 + 1)    
  
                    IF @u1 > 0 AND @u2 > 0 AND @u3 > 0 AND @u4 > 0    
                    BEGIN    
                        IF @u5 > 0    
                        BEGIN    
                            SET @PrecioCostoUM = SUBSTRING(@item, @u4 + 1, @u5 - @u4 - 1)    
                            SET @UnidadImagenUM = SUBSTRING(@item, @u5 + 1, LEN(@item))    
                        END    
                        ELSE    
                        BEGIN    
                            SET @PrecioCostoUM = SUBSTRING(@item, @u4 + 1, LEN(@item))    
                            SET @UnidadImagenUM = ''    
                        END    
    
                        INSERT INTO UnidadMedida    
                        (IdProducto, UMDescripcion, ValorUM, PrecioVenta, PrecioVentaB, PrecioCosto, unidadImagen)    
                        VALUES    
                        (    
                            @IdProductoFinal,    
                            SUBSTRING(@item, 1, @u1 - 1),    
                            CONVERT(DECIMAL(18,2), SUBSTRING(@item, @u1 + 1, @u2 - @u1 - 1)),    
                            CONVERT(DECIMAL(18,2), SUBSTRING(@item, @u2 + 1, @u3 - @u2 - 1)),    
                            CONVERT(DECIMAL(18,2), SUBSTRING(@item, @u3 + 1, @u4 - @u3 - 1)),    
                            CONVERT(DECIMAL(18,2), @PrecioCostoUM),    
                            NULLIF(LTRIM(RTRIM(ISNULL(@UnidadImagenUM, ''))), '')    
                        )    
                    END    
                END    
            END    
        END    
    END      
  
    ELSE      
    BEGIN      
        IF EXISTS(SELECT 1 FROM Producto     
                  WHERE ProductoCodigo=@ProductoCodigo     
                  AND IdProducto<>@Id)         
        BEGIN        
            SELECT 'existe Codigo'         
            RETURN    
        END      
    
        DECLARE @CantAnt DECIMAL(18,4)    
        SET @CantAnt=(SELECT ProductoCantidad FROM Producto WHERE IdProducto=@Id)    
    
        UPDATE Producto SET      
            IdSubLinea = @IdSubLinea,      
            ProductoCodigo = @ProductoCodigo,      
            ProductoNombre = @ProductoNombre,      
            ProductoUM = @ProductoUM,      
            ProductoCosto = @ProductoCosto,      
            ProductoVenta = @ProductoVenta,      
            ProductoVentaB = @ProductoVentaB,      
            ProductoCantidad = @ProductoCantidad,      
            ProductoEstado = @ProductoEstado,      
            ProductoUsuario = @ProductoUsuario,      
            ProductoFecha = GETDATE(),      
            ProductoImagen = @ProductoImagen,      
            ValorCritico = @ValorCritico,      
            AplicaINV = @AplicaINV      
        WHERE IdProducto = @Id      
    
        SET @IdProductoFinal = @Id    
    
        IF(@AplicaINV='S')          
        BEGIN                  
            INSERT INTO Kardex     
            VALUES(@Id,GETDATE(),    
            'Edita Producto','Edita Producto',    
          @CantAnt,0,0,@ProductoCosto,@ProductoCantidad,    
            'INGRESO',@ProductoUsuario)                  
        END      
    END    
    
    SELECT @IdProductoFinal    
END
GO
/****** Object:  StoredProcedure [dbo].[uspInsertaActivacion]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[uspInsertaActivacion]
@Data varchar(max)
as
begin
Declare @pos1 int,@pos2 int,
		@pos3 int,@pos4 int		
Declare  @CodigoDXN varchar(80),@Afiliado varchar(250),
		 @Serial varchar(250),@FechaActivacion datetime
Set @pos1 = CharIndex('|',@Data,0)
Set @pos2 = CharIndex('|',@Data,@pos1+1)
Set @pos3 = CharIndex('|',@Data,@pos2+1)
Set @pos4 = Len(@Data)+1

Set @CodigoDXN=SUBSTRING(@Data,1,@pos1-1)
Set @Afiliado=SUBSTRING(@Data,@pos1+1,@pos2-@pos1-1)
Set @Serial=SUBSTRING(@Data,@pos2+1,@pos3-@pos2-1)
Set @FechaActivacion=convert(datetime,SUBSTRING(@Data,@pos3+1,@pos4-@pos3-1))
IF EXISTS(select a.CodigoDXN
from Activaciones a
where a.CodigoDXN=@CodigoDXN)
begin
select 'CODIGO'
end
else IF EXISTS(select a.Serial
from Activaciones a
where a.Serial=@Serial)
begin
select 'SERIAL'
end
else
begin
insert into Activaciones values(@CodigoDXN,@Afiliado,@Serial,'D',@FechaActivacion)
select 'true'
end
end
GO
/****** Object:  StoredProcedure [dbo].[uspInsertaAutorizacion]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[uspInsertaAutorizacion]
@Data varchar(max)
as
begin
Declare @p1 int,@p2 int,@p3 int,
        @p4 int,@p5 int,@p6 int,
        @p7 int,@p8 int
Declare @IdAuto numeric(38),
        @IdPersonal int,@IdAdmin int,
        @HoraInicio datetime,@Tiempo int,
        @HoraFin datetime,@Observaciones varchar(max),
        @fechaRegistro datetime
Set @Data = LTRIM(RTrim(@Data))
Set @p1 = CharIndex('|',@Data,0)
Set @p2 = CharIndex('|',@Data,@p1+1)
Set @p3 = CharIndex('|',@Data,@p2+1)
Set @p4 = CharIndex('|',@Data,@p3+1)
Set @p5 = CharIndex('|',@Data,@p4+1)
Set @p6 = CharIndex('|',@Data,@p5+1)
Set @p7 = CharIndex('|',@Data,@p6+1)
Set @p8= Len(@Data)+1
Set @IdAuto=convert(numeric(38),SUBSTRING(@Data,1,@p1-1))
Set @IdPersonal=convert(int,SUBSTRING(@Data,@p1+1,@p2-@p1-1))
Set @IdAdmin=convert(int,SUBSTRING(@Data,@p2+1,@p3-@p2-1))
Set @HoraInicio=convert(datetime,SUBSTRING(@Data,@p3+1,@p4-@p3-1))
Set @Tiempo=convert(int,SUBSTRING(@Data,@p4+1,@p5-@p4-1))
Set @HoraFin=convert(datetime,SUBSTRING(@Data,@p5+1,@p6-@p5-1))
Set @Observaciones=SUBSTRING(@Data,@p6+1,@p7-@p6-1)
Set @fechaRegistro=convert(date,SUBSTRING(@Data,@p7+1,@p8-@p7-1))
if(@IdAuto=0)
begin
IF EXISTS(select a.IdAuto 
from AutorizaEdicion a 
where convert(date,a.HoraInicio)=
convert(date,@HoraInicio) and a.IdPersonal=@IdPersonal)
begin
select 'existe' as 'A'
end
else
begin
insert into AutorizaEdicion values(@IdPersonal,@IdAdmin,
@HoraInicio,@Tiempo,@HoraFin,@Observaciones,@fechaRegistro)
select 'true' as 'A'
end
end
else
begin
update AutorizaEdicion
set HoraInicio=@HoraInicio,Tiempo=@Tiempo,HoraFin=@HoraFin,
Observaciones=@Observaciones,FechaRegistro=@fechaRegistro
where IdAuto=@IdAuto
select 'true' as 'A'
end
end
GO
/****** Object:  StoredProcedure [dbo].[uspinsertaFactura]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspinsertaFactura]              
@ListaOrden varchar(Max)              
as              
begin              
Declare @posA1 int,@posA2 int,@posA3 int    
Declare @orden varchar(max),    
        @detalle varchar(max),    
        @Guia varchar(max)    
Set @posA1 = CharIndex('[',@ListaOrden,0)    
Set @posA2 = CharIndex('[',@ListaOrden,@posA1+1)    
Set @posA3 =Len(@ListaOrden)+1    
Set @orden = SUBSTRING(@ListaOrden,1,@posA1-1)    
Set @detalle = SUBSTRING(@ListaOrden,@posA1+1,@posA2-@posA1-1)    
Set @Guia=SUBSTRING(@ListaOrden,@posA2+1,@posA3-@posA2-1)  
              
Declare @pos1 int,@pos2 int,@pos3 int,@pos4 int,              
        @pos5 int,@pos6 int,@pos7 int,@pos8 int,              
        @pos9 int,@pos10 int,@pos11 int,@pos12 int,              
        @pos13 int,@pos14 int,@pos15 int,@pos16 int,              
        @pos17 int,@pos18 int,@pos19 int,@pos20 int,              
        @pos21 int,@pos22 int,@pos23 int,@pos24 int,              
        @pos25 int,@pos26 int,@pos27 int,@pos28 int,              
        @pos29 int,@pos30 int,@pos31 int,@pos32 int,              
        @pos33 int,@pos34 int            
Declare @CompaniaId int,@NotaId numeric(38),@DocuDocumento varchar(60),              
        @DocuNumero varchar(60),@ClienteId numeric(20),@DocuEmision date,              
        @DocuSubTotal decimal(18,2),@DocuIgv decimal(18,2),@DocuTotal decimal(18,2),              
        @DocuUsuario varchar(60),@DocuSerie varchar(4),@TipoCodigo char(20),              
        @DocuAdicional decimal(18,2),@DocuAsociado varchar(80),@DocuConcepto varchar(80),              
        @DocuHASH varchar(250),@EstadoSunat varchar(80),@Letras varchar(60),              
        @DocuId numeric(38),@ICBPER decimal(18,2),@CodigoSunat VARCHAR(80),@MensajeSunat varchar(max),              
        @DocuGravada decimal(18,2),@DocuDescuento decimal(18,2),@CajaId nvarchar(40),              
        @UsuarioId int,@NotaFormaPago varchar(60),@Movimiento varchar(40),        
        @NotaCondicion varchar(60),@EntidadBancaria varchar(80),@NroOperacion varchar(80),      
        @Efectivo decimal(18,2),@Deposito decimal(18,2),@ClienteRazon varchar(140),  
        @ClienteRuc varchar(40),@ClienteDni varchar(40),@DireccionFiscal varchar(max)          
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
Set @pos20= CharIndex('|',@orden,@pos19+1)              
Set @pos21= CharIndex('|',@orden,@pos20+1)              
Set @pos22= CharIndex('|',@orden,@pos21+1)              
Set @pos23= CharIndex('|',@orden,@pos22+1)              
Set @pos24= CharIndex('|',@orden,@pos23+1)        
Set @pos25= CharIndex('|',@orden,@pos24+1)      
Set @pos26= CharIndex('|',@orden,@pos25+1)              
Set @pos27= CharIndex('|',@orden,@pos26+1)              
Set @pos28= CharIndex('|',@orden,@pos27+1)        
Set @pos29= CharIndex('|',@orden,@pos28+1)    
    
Set @pos30= CharIndex('|',@orden,@pos29+1)              
Set @pos31= CharIndex('|',@orden,@pos30+1)              
Set @pos32= CharIndex('|',@orden,@pos31+1)        
Set @pos33= CharIndex('|',@orden,@pos32+1)    
             
Set @pos34= Len(@orden)+1      
              
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
set @ICBPER=convert(decimal(18,2),SUBSTRING(@orden,@pos18+1,@pos19-@pos18-1))              
set @CodigoSunat=SUBSTRING(@orden,@pos19+1,@pos20-@pos19-1)              
set @MensajeSunat=SUBSTRING(@orden,@pos20+1,@pos21-@pos20-1)              
set @DocuGravada=convert(decimal(18,2),SUBSTRING(@orden,@pos21+1,@pos22-@pos21-1))              
set @DocuDescuento=convert(decimal(18,2),SUBSTRING(@orden,@pos22+1,@pos23-@pos22-1))              
set @NotaFormaPago=SUBSTRING(@orden,@pos23+1,@pos24-@pos23-1)              
set @UsuarioId=convert(int,SUBSTRING(@orden,@pos24+1,@pos25-@pos24-1))        
set @NotaCondicion=SUBSTRING(@orden,@pos25+1,@pos26-@pos25-1)      
      
set @EntidadBancaria=SUBSTRING(@orden,@pos26+1,@pos27-@pos26-1)              
set @NroOperacion=SUBSTRING(@orden,@pos27+1,@pos28-@pos27-1)              
set @Efectivo=convert(decimal(18,2),SUBSTRING(@orden,@pos28+1,@pos29-@pos28-1))              
set @Deposito=convert(decimal(18,2),SUBSTRING(@orden,@pos29+1,@pos30-@pos29-1))      
    
set @ClienteRazon=SUBSTRING(@orden,@pos30+1,@pos31-@pos30-1)              
set @ClienteRuc=SUBSTRING(@orden,@pos31+1,@pos32-@pos31-1)     
set @ClienteDni=SUBSTRING(@orden,@pos32+1,@pos33-@pos32-1)              
set @DireccionFiscal=SUBSTRING(@orden,@pos33+1,@pos34-@pos33-1)       
             
set @CajaId=isnull((select top 1 convert(nvarchar,CajaId)from Caja               
where CajaEstado='ACTIVO' order by 1 desc),'0')        
              
if(@CajaId=0)              
begin              
select 'false'              
end              
else              
begin              
Begin Transaction      
              
insert into DocumentoVenta values(@CompaniaId,@NotaId,@DocuDocumento,@DocuNumero,              
@ClienteId,GETDATE(),@DocuEmision,'ALCONTADO',@Letras,@DocuSubTotal,@DocuIgv,@DocuTotal,0,              
@DocuUsuario,'EMITIDO',@DocuSerie,@TipoCodigo,@DocuAdicional,@DocuAsociado,              
@DocuConcepto,'',@DocuHASH,@EstadoSunat,@ICBPER,@CodigoSunat,@MensajeSunat,              
@DocuGravada,@DocuDescuento,'',@NotaFormaPago,    
@EntidadBancaria,@NroOperacion,@Efectivo,@Deposito,    
@ClienteRazon,@ClienteRuc,@ClienteDni,@DireccionFiscal)            
            
Set @DocuId= @@identity        
              
/*        
EFECTIVO        
DEPOSITO        
TARJETA        
YAPE        
EFECTIVO/DEPOSITO        
TARJETA/EFECTIVO        
YAPE/EFECTIVO        
YAPE/DEPOSITO        
TARJETA/DEPOSITO        
*/        
        
Declare @pZ1 int=0        
        
if(@NotaFormaPago='YAPE/DEPOSITO' or @NotaFormaPago='TARJETA/DEPOSITO')        
begin        
set @Movimiento='DEPOSITO'         
end        
else        
begin        
Declare @pZ2 int        
Declare @FormaA varchar(max),                      
        @FormaB varchar(max),        
        @MovimientoB varchar(40)        
                         
Set @pZ1 = CharIndex('/',@NotaFormaPago,0)        
        
if(@pZ1>0)        
begin        
        
Set @pZ2 =Len(@NotaFormaPago)+1        
Set @FormaA = SUBSTRING(@NotaFormaPago,1,@pZ1-1)        
Set @FormaB = SUBSTRING(@NotaFormaPago,@pZ1+1,@pZ2-@pZ1-1)        
        
if(@FormaA='EFECTIVO')set @Movimiento='INGRESO'                      
else if(@FormaA='DEPOSITO')set @Movimiento='DEPOSITO'                      
else if(@FormaA='YAPE' or @FormaA='PLIN' or @FormaA='PEDIDOSYA')set @Movimiento='DEPOSITO'                      
else set @Movimiento='TARJETA'         
        
if(@FormaB='EFECTIVO')set @MovimientoB='INGRESO'                      
else if(@FormaB='DEPOSITO')set @MovimientoB='DEPOSITO'                      
else if(@FormaB='YAPE' or @FormaB='PLIN' or @FormaB='PEDIDOSYA')set @MovimientoB='DEPOSITO'                      
else set @MovimientoB='TARJETA'         
        
END        
Else        
begin        
        
if(@NotaFormaPago='EFECTIVO')set @Movimiento='INGRESO'                      
else if(@NotaFormaPago='DEPOSITO')set @Movimiento='DEPOSITO'                      
else if(@NotaFormaPago='YAPE' OR @NotaFormaPago='PLIN' or @NotaFormaPago='PEDIDOSYA')set @Movimiento='DEPOSITO'                      
else set @Movimiento='TARJETA'         
        
End        
END      
              
   if(@NotaCondicion='CREDITO')        
   BEGIN          
   update NotaPedido               
   set CompaniaId=@CompaniaId,NotaSerie=@DocuSerie,              
   NotaNumero=@DocuNumero,NotaEstado='EMITIDO',NotaSaldo=@DocuTotal,NotaAcuenta=0,CajaId=@CajaId              
   where NotaId=@NotaId         
   end        
   Else        
   begin        
   if(@pZ1>0)        
begin        
    if(@Movimiento='INGRESO')        
    BEGIN        
    insert into CajaDetalle values(@CajaId,GETDATE(),@NotaId,@Movimiento,'',                      
    'Transacción con '+@NotaFormaPago,@Efectivo,@Efectivo,0,'','T','',@DocuUsuario,'','')                      
    END        
    else        
    begin        
    insert into CajaDetalle values(@CajaId,GETDATE(),@NotaId,@Movimiento,'',                      
    'Transacción con '+@NotaFormaPago,@Deposito,@Deposito,0,'','T','',@DocuUsuario,'','')         
    end        
    if(@MovimientoB='INGRESO')        
    BEGIN        
    insert into CajaDetalle values(@CajaId,GETDATE(),@NotaId,@MovimientoB,'',                      
    'Transacción con '+@NotaFormaPago,@Efectivo,@Efectivo,0,'','T','',@DocuUsuario,'','')         
    END        
    ELSE        
    BEGIN        
    insert into CajaDetalle values(@CajaId,GETDATE(),@NotaId,@MovimientoB,'',                      
    'Transacción con '+@NotaFormaPago,@Deposito,@Deposito,0,'','T','',@DocuUsuario,'','')         
    END                
END      
ELSE         
BEGIN       
    insert into CajaDetalle values(@CajaId,GETDATE(),@NotaId,@Movimiento,'',                      
    'Transacción con '+@NotaFormaPago,@DocuTotal,@DocuTotal,0,'','T','',@DocuUsuario,'','')              
END      
           
   update NotaPedido               
   set CompaniaId=@CompaniaId,NotaSerie=@DocuSerie,              
   NotaNumero=@DocuNumero,NotaEstado='CANCELADO',NotaSaldo=0,NotaAcuenta=@DocuTotal,CajaId=@CajaId              
   where NotaId=@NotaId           
         
END      
        
Declare Tabla Cursor For Select * From fnSplitString(@detalle,';')               
Open Tabla              
Declare @Columna varchar(max),              
  @IdProducto numeric(20),              
  @Cantidad decimal(18,2),              
  @Costo decimal(18,4),              
  @Precio decimal(18,2),              
  @Importe decimal(18,2),              
  @DetalleNotaId numeric(38),              
  @UM varchar(80),              
  @ValorUM decimal(18,4),            
  @Descripcion varchar(300),
  @AplicaINV nvarchar(1),            
  @IniciaStock decimal(18,2),@StockFinal decimal(18,2)              
Declare @p1 int,@p2 int,@p3 int,@p4 int,              
        @p5 int,@p6 int,@p7 int,@p8 int,
        @p9 int,@p10 int             
Fetch Next From Tabla INTO @Columna              
 While @@FETCH_STATUS = 0              
 Begin              
            
Set @p1 = CharIndex('|',@Columna,0)              
Set @p2 = CharIndex('|',@Columna,@p1+1)              
Set @p3 = CharIndex('|',@Columna,@p2+1)              
Set @p4 = CharIndex('|',@Columna,@p3+1)              
Set @p5 = CharIndex('|',@Columna,@p4+1)              
Set @p6= CharIndex('|',@Columna,@p5+1)              
Set @p7= CharIndex('|',@Columna,@p6+1)            
Set @p8= CharIndex('|',@Columna,@p7+1)
Set @p9= CharIndex('|',@Columna,@p8+1)                
Set @p10= Len(@Columna)+1          
              
Set @DetalleNotaId=Convert(numeric(38),SUBSTRING(@Columna,1,@p1-1))              
Set @IdProducto=Convert(numeric(20),SUBSTRING(@Columna,@p1+1,@p2-(@p1+1)))              
Set @Cantidad=Convert(decimal(18,2),SUBSTRING(@Columna,@p2+1,@p3-(@p2+1)))              
Set @UM=SUBSTRING(@Columna,@p3+1,@p4-(@p3+1))              
Set @Costo=Convert(decimal(18,4),SUBSTRING(@Columna,@p4+1,@p5-(@p4+1)))              
Set @Precio=Convert(decimal(18,2),SUBSTRING(@Columna,@p5+1,@p6-(@p5+1)))              
Set @Importe=Convert(decimal(18,2),SUBSTRING(@Columna,@p6+1,@p7-(@p6+1)))              
Set @ValorUM=Convert(decimal(18,4),SUBSTRING(@Columna,@p7+1,@p8-(@p7+1)))            
Set @Descripcion=SUBSTRING(@Columna,@p8+1,@p9-(@p8+1))
Set @AplicaINV=SUBSTRING(@Columna,@p9+1,@p10-(@p9+1))            
               
Declare @CantidadSal decimal(18,2)          
                    
insert into DetalleDocumento               
values(@DocuId,@IdProducto,@Cantidad,@Precio,@Importe,            
@DetalleNotaId,@UM,@ValorUM,@Descripcion)              

if(@AplicaINV='S')          
  BEGIN            
   set @CantidadSal=@Cantidad * @ValorUM           
          
   set @IniciaStock=(select top 1 ProductoCantidad from Producto where IdProducto=@IdProducto)              
   set @StockFinal=@IniciaStock-@CantidadSal              
            
   insert into Kardex values(@IdProducto,GETDATE(),'Salida por Venta',@DocuSerie+'-'+@DocuNumero,@IniciaStock,              
   0,@CantidadSal,@Costo,@StockFinal,'SALIDA',@DocuUsuario)              
             
   update producto               
   set  ProductoCantidad =ProductoCantidad - @CantidadSal             
   where IDProducto=@IdProducto              
  End           
Fetch Next From Tabla INTO @Columna              
end              
 Close Tabla;              
 Deallocate Tabla;              
 Declare @EstadoDetalle varchar(80)              
 if(@EstadoSunat='PENDIENTE')set @EstadoDetalle='PENDIENTEB'              
 else set @EstadoDetalle='EMITIDO'              
 update DetallePedido              
 set DetalleEstado=@EstadoDetalle              
 where NotaId=@NotaId              
 --Commit Transaction;              
 --select 'true'  
if(len(@Guia)>0)    
begin    
Declare TablaB Cursor For Select * From fnSplitString(@Guia,';')     
Open TablaB    
Declare @ColumnaB varchar(max)  
Declare @g1 int,@g2 int,  
        @g3 int,@g4 int,@g5 int  
  
Declare @CantidadA decimal(18,2),   
        @IdProductoU numeric(20),                   
        @CantidadU decimal(18,2),                      
        @UmU varchar(40),                                                     
        @ValorUMU decimal(18,4)  
  
Declare @IniciaStockB decimal(18,2),  
        @StockFinalB decimal(18,2)  
            
Fetch Next From TablaB INTO @ColumnaB    
 While @@FETCH_STATUS = 0    
 Begin    
Set @g1 = CharIndex('|',@ColumnaB,0)                     
Set @g2 = CharIndex('|',@ColumnaB,@g1+1)                      
Set @g3 = CharIndex('|',@ColumnaB,@g2+1)                      
Set @g4 = CharIndex('|',@ColumnaB,@g3+1)                      
Set @g5=Len(@ColumnaB)+1     
   
set @CantidadA=Convert(decimal(18,2),SUBSTRING(@ColumnaB,1,@g1-1))  
Set @IdProductoU=Convert(numeric(20),SUBSTRING(@ColumnaB,@g1+1,@g2-(@g1+1)))  
Set @CantidadU=Convert(decimal(18,2),SUBSTRING(@ColumnaB,@g2+1,@g3-(@g2+1)))    
Set @UmU=SUBSTRING(@ColumnaB,@g3+1,@g4-(@g3+1))    
Set @ValorUMU=Convert(decimal(18,4),SUBSTRING(@ColumnaB,@g4+1,@g5-(@g4+1)))        
  
 Declare @CantidadSalB decimal(18,2)   
  
 set @CantidadSalB=(@CantidadA * @CantidadU)* @ValorUMU              
                  
 set @IniciaStockB=(select top 1 p.ProductoCantidad   
 from Producto p where p.IdProducto=@IdProductoU)                      
   
 set @StockFinalB=@IniciaStockB-@CantidadSalB                     
               
 insert into Kardex values(@IdProductoU,GETDATE(),'Salida por Venta',@DocuSerie+'-'+@DocuNumero,@IniciaStockB,                      
 0,@CantidadSalB,0,@StockFinalB,'SALIDA',@DocuUsuario)                      
                 
 update producto                       
 set  ProductoCantidad =ProductoCantidad - @CantidadSalB                     
 where IDProducto=@IdProductoU      
  
Fetch Next From TablaB INTO @ColumnaB    
end    
    Close TablaB;    
    Deallocate TablaB;    
    Commit Transaction;    
    select 'true'  
end    
else    
begin    
    Commit Transaction;    
    select 'true'  
end  
                
end              
end
GO
/****** Object:  StoredProcedure [dbo].[uspinsertaGasto]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspinsertaGasto]    
@Data varchar(max)    
as    
begin    
Declare @p1 int    
Declare @p2 int    
Declare @p3 int    
Declare @p4 int    
Declare @p5 int    
declare    
@GastoId int,    
@GastoFecha date,    
@GastoDesc varchar(max),    
@GastoMonto decimal(18,2),    
@GastoUsuario varchar(80)    
Set @Data = LTRIM(RTrim(@Data))  
Set @p1 = CharIndex('|',@Data,0)  
Set @p2 = CharIndex('|',@Data,@p1+1)  
Set @p3 = CharIndex('|',@Data,@p2+1)  
Set @p4 = CharIndex('|',@Data,@p3+1)  
Set @p5= Len(@Data)+1  
    
Set @GastoId=convert(int,SUBSTRING(@Data,1,@p1-1))    
Set @GastoFecha=convert(date,SUBSTRING(@Data,@p1+1,@p2-@p1-1))      
Set @GastoDesc=SUBSTRING(@Data,@p2+1,@p3-@p2-1)    
Set @GastoMonto=convert(decimal(18,2),SUBSTRING(@Data,@p3+1,@p4-@p3-1))    
Set @GastoUsuario=SUBSTRING(@Data,@p4+1,@p5-@p4-1)    
  
if (@GastoId=0)  
begin    
 IF EXISTS(select * from GastosFijos g   
 where g.GsstoDesc=@GastoDesc and    
 (Month(g.GastoFecha)=MONTH(@GastoFecha) and year(g.GastoFecha)=YEAR(@GastoFecha)))    
 begin  
 select 'existe'  
 end    
else    
begin    
 insert into GastosFijos values(@GastoFecha,@GastoDesc,@GastoMonto,GETDATE(),@GastoUsuario,'')  
     
 select isnull((select STUFF((select '¬'+ CONVERT(varchar,g.GastoId)+'|'+convert(varchar,g.GastoFecha,103)+'|'+    
 g.GsstoDesc+'|'+CONVERT(VarChar(50), cast(g.GstoMonto as money ), 1)+'|'+    
 (IsNull(convert(varchar,g.GastoReg,103),'')+' '+ IsNull(SUBSTRING(convert(varchar,g.GastoReg,114),1,8),''))+'|'+    
 g.GastoUsuario+'|'+g.Estado    
 from GastosFijos g     
 where month(g.GastoFecha)=month(GETDATE())and year(g.GastoFecha)=year(GETDATE())    
 order by g.GastoFecha asc,g.GastoId asc    
 FOR XML PATH('')), 1, 1, '')),'~')  
      
end    
end
else
begin

update GastosFijos
set GastoFecha=@GastoFecha,GsstoDesc=@GastoDesc,
GstoMonto=@GastoMonto,GastoReg=getdate(),GastoUsuario=@GastoUsuario
where GastoId=@GastoId

 select isnull((select STUFF((select '¬'+ CONVERT(varchar,g.GastoId)+'|'+convert(varchar,g.GastoFecha,103)+'|'+    
 g.GsstoDesc+'|'+CONVERT(VarChar(50), cast(g.GstoMonto as money ), 1)+'|'+    
 (IsNull(convert(varchar,g.GastoReg,103),'')+' '+ IsNull(SUBSTRING(convert(varchar,g.GastoReg,114),1,8),''))+'|'+    
 g.GastoUsuario+'|'+g.Estado    
 from GastosFijos g     
 where month(g.GastoFecha)=month(GETDATE())and year(g.GastoFecha)=year(GETDATE())    
 order by g.GastoFecha asc,g.GastoId asc    
 FOR XML PATH('')), 1, 1, '')),'~')  

end 
end
GO
/****** Object:  StoredProcedure [dbo].[uspinsertaGuiaInterna]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspinsertaGuiaInterna]                          
@ListaOrden varchar(Max)                          
as                          
begin  
                          
Declare @posA1 int,@posA2 int,@posA3 int      
Declare @orden varchar(max),      
        @detalle varchar(max),      
        @Guia varchar(max)  
             
Set @posA1 = CharIndex('[',@ListaOrden,0)      
Set @posA2 = CharIndex('[',@ListaOrden,@posA1+1)      
Set @posA3 =Len(@ListaOrden)+1      
Set @orden = SUBSTRING(@ListaOrden,1,@posA1-1)      
Set @detalle = SUBSTRING(@ListaOrden,@posA1+1,@posA2-@posA1-1)      
Set @Guia=SUBSTRING(@ListaOrden,@posA2+1,@posA3-@posA2-1)  
  
Declare @pos1 int,@pos2 int,@pos3 int,@pos4 int,                          
        @pos5 int,@pos6 int,@pos7 int ,@pos8 int,              
        @pos9 int,@pos10 int,@pos11 int,@pos12 int       
                                                  
Declare @GuiaId numeric(38),@FechaRegistro datetime,                        
  @Concepto nvarchar(1),@Motivo varchar(300),        
  @Origen varchar(300),@Destino varchar(300),        
  @Observacion varchar(max),@Usuario varchar(80),                        
  @KardexDocu varchar(300),@UsuarioId int,        
  @Estado nvarchar(1),@Serie nvarchar(4),        
  @ClienteId varchar(20),@Total decimal(18,2)            
                    
Set @pos1 = CharIndex('|',@orden,0)                          
Set @pos2 = CharIndex('|',@orden,@pos1+1)                          
Set @pos3 = CharIndex('|',@orden,@pos2+1)                          
Set @pos4 = CharIndex('|',@orden,@pos3+1)                          
Set @pos5 = CharIndex('|',@orden,@pos4+1)                          
Set @pos6= CharIndex('|',@orden,@pos5+1)                        
Set @pos7= CharIndex('|',@orden,@pos6+1)                
Set @pos8= CharIndex('|',@orden,@pos7+1)              
Set @pos9= CharIndex('|',@orden,@pos8+1)                          
Set @pos10 = CharIndex('|',@orden,@pos9+1)                          
Set @pos11= CharIndex('|',@orden,@pos10+1)                                        
Set @pos12= Len(@orden)+1                          
                        
Set @FechaRegistro=convert(datetime,SUBSTRING(@orden,1,@pos1-1))                          
Set @Concepto=SUBSTRING(@orden,@pos1+1,@pos2-@pos1-1)              
Set @Serie=SUBSTRING(@orden,@pos2+1,@pos3-@pos2-1)                          
Set @Motivo=SUBSTRING(@orden,@pos3+1,@pos4-@pos3-1)                          
Set @Origen=SUBSTRING(@orden,@pos4+1,@pos5-@pos4-1)                          
Set @Destino=SUBSTRING(@orden,@pos5+1,@pos6-@pos5-1)                          
Set @ClienteId=SUBSTRING(@orden,@pos6+1,@pos7-@pos6-1)             
Set @Observacion=SUBSTRING(@orden,@pos7+1,@pos8-@pos7-1)                          
Set @Total=convert(decimal(18,2),SUBSTRING(@orden,@pos8+1,@pos9-@pos8-1))            
Set @Usuario=SUBSTRING(@orden,@pos9+1,@pos10-@pos9-1)                          
Set @UsuarioId=SUBSTRING(@orden,@pos10+1,@pos11-@pos10-1)                
Set @Estado=SUBSTRING(@orden,@pos11+1,@pos12-@pos11-1)                          
                        
Begin Transaction              
              
declare @cod varchar(13)              
SET @cod=isnull((select TOP 1 dbo.genenerarNroGuiaSI(@Serie,@Concepto) AS ID               
FROM GuiaInternaSI),'00000001')              
                          
insert into GuiaInternaSI values(@FechaRegistro,@Concepto,              
@Serie,@cod,@Motivo,@Origen,@Destino,@ClienteId,@Observacion,        
@Total,@Usuario,@Estado)                
                          
Set @GuiaId= @@identity                        
                        
if(@Concepto='I')set @KardexDocu='Guia De ING Interno'                        
else set @KardexDocu='Guia De SAL Interno'                        
                        
Declare Tabla Cursor For Select * From fnSplitString(@detalle,';')                           
Open Tabla                          
                        
Declare @Columna varchar(max),                          
  @IdProducto numeric(20),                          
  @Cantidad decimal(18,2),                         
  @UM varchar(80),                         
  @Descripcion varchar(max),                  
  @Costo decimal(18,4),                
  @PrecioVenta decimal(18,2),                
  @Importe decimal(18,2),    
  @ValorUM decimal(18,4),
  @AplicaINV nvarchar(1),                        
                          
  @StockInicial decimal(18,2),                          
  @StockFinal decimal(18,2),@CantidadIng decimal(18,2)                        
                            
Declare @p1 int,@p2 int,@p3 int,@p4 int,                          
        @p5 int,@p6 int,@p7 int,@p8 int,
        @p9 int                        
                                
Fetch Next From Tabla INTO @Columna                          
 While @@FETCH_STATUS = 0                          
 Begin                          
Set @p1=CharIndex('|',@Columna,0)                          
Set @p2=CharIndex('|',@Columna,@p1+1)                          
Set @p3=CharIndex('|',@Columna,@p2+1)                          
Set @p4=CharIndex('|',@Columna,@p3+1)                
Set @p5=CharIndex('|',@Columna,@p4+1)                          
Set @p6=CharIndex('|',@Columna,@p5+1)    
Set @p7=CharIndex('|',@Columna,@p6+1)
Set @p8=CharIndex('|',@Columna,@p7+1)                                
Set @p9=Len(@Columna)+1                        
                         
Set @IdProducto=Convert(numeric(20),SUBSTRING(@Columna,1,@p1-1))                
Set @Cantidad=convert(decimal(18,2),SUBSTRING(@Columna,@p1+1,@p2-(@p1+1)))                          
Set @UM=SUBSTRING(@Columna,@p2+1,@p3-(@p2+1))                 
Set @Descripcion=SUBSTRING(@Columna,@p3+1,@p4-(@p3+1))                
Set @Costo=Convert(decimal(18,4),SUBSTRING(@Columna,@p4+1,@p5-(@p4+1)))                
Set @PrecioVenta=Convert(decimal(18,2),SUBSTRING(@Columna,@p5+1,@p6-(@p5+1)))                   
Set @Importe=Convert(decimal(18,2),SUBSTRING(@Columna,@p6+1,@p7-(@p6+1)))    
Set @ValorUM=Convert(decimal(18,4),SUBSTRING(@Columna,@p7+1,@p8-(@p7+1)))
Set @AplicaINV=SUBSTRING(@Columna,@p8+1,@p9-(@p8+1))                           
                        
insert into DetalleGuiaInterna values(@GuiaId,@IdProducto,@Cantidad,@UM,@Descripcion,                
@Costo,@PrecioVenta,@Importe,'E',@ValorUM)                          

if(@AplicaINV='S')
BEGIN
                      
if(@Concepto='I')                      
begin
                        
set @StockInicial=(select top 1 ProductoCantidad     
from Producto(nolock)                           
where IdProducto=@IdProducto)                          
                        
set @CantidadIng=(@Cantidad*@ValorUM)                          
set @StockFinal=@StockInicial+@CantidadIng                          
                        
update Producto                          
set ProductoCantidad=ProductoCantidad+@CantidadIng                          
where IdProducto=@IdProducto                        
                         
insert into Kardex values(@IdProducto,@FechaRegistro,@KardexDocu,@Serie+'-'+@cod,@StockInicial,                          
@CantidadIng,0,@Costo,@StockFinal,'INGRESO',@Usuario)                  
                  
END                     
else                      
begin                      
                      
set @StockInicial=(select top 1 ProductoCantidad     
from Producto(nolock)                           
where IdProducto=@IdProducto)                          
                        
set @CantidadIng=(@Cantidad*@ValorUM)                          
set @StockFinal=@StockInicial-@CantidadIng                          
                        
update Producto                          
set ProductoCantidad=ProductoCantidad-@CantidadIng                          
where IdProducto=@IdProducto                        
                         
insert into Kardex values(@IdProducto,@FechaRegistro,@KardexDocu,@Serie+'-'+@cod,@StockInicial,                          
0,@CantidadIng,@Costo,@StockFinal,'SALIDA',@Usuario)                        
                      
END
                          
END                      
Fetch Next From Tabla INTO @Columna                          
end           
 Close Tabla;                          
 Deallocate Tabla;        
                          
 delete from TemporalGuiaB                         
 where UsuarioID=@UsuarioId and Concepto=@Concepto                      
                         
 --Commit Transaction;                          
 --select convert(varchar,@GuiaId)+'¬'+@cod  
   
 if(len(@Guia)>0)      
begin      
Declare TablaB Cursor For Select * From fnSplitString(@Guia,';')       
Open TablaB      
Declare @ColumnaB varchar(max)    
Declare @g1 int,@g2 int,    
        @g3 int,@g4 int,@g5 int    
    
Declare @CantidadA decimal(18,2),     
        @IdProductoU numeric(20),                     
        @CantidadU decimal(18,2),                        
        @UmU varchar(40),                                                       
        @ValorUMU decimal(18,4)    
    
Declare @IniciaStockB decimal(18,2),    
        @StockFinalB decimal(18,2)    
              
Fetch Next From TablaB INTO @ColumnaB      
 While @@FETCH_STATUS = 0      
 Begin      
Set @g1 = CharIndex('|',@ColumnaB,0)                       
Set @g2 = CharIndex('|',@ColumnaB,@g1+1)                        
Set @g3 = CharIndex('|',@ColumnaB,@g2+1)                        
Set @g4 = CharIndex('|',@ColumnaB,@g3+1)                        
Set @g5=Len(@ColumnaB)+1       
     
set @CantidadA=Convert(decimal(18,2),SUBSTRING(@ColumnaB,1,@g1-1))    
Set @IdProductoU=Convert(numeric(20),SUBSTRING(@ColumnaB,@g1+1,@g2-(@g1+1)))    
Set @CantidadU=Convert(decimal(18,2),SUBSTRING(@ColumnaB,@g2+1,@g3-(@g2+1)))      
Set @UmU=SUBSTRING(@ColumnaB,@g3+1,@g4-(@g3+1))      
Set @ValorUMU=Convert(decimal(18,4),SUBSTRING(@ColumnaB,@g4+1,@g5-(@g4+1)))          
    
 Declare @CantidadSalB decimal(18,2)     
    
 set @CantidadSalB=(@CantidadA * @CantidadU)* @ValorUMU                
                    
 set @IniciaStockB=(select top 1 p.ProductoCantidad     
 from Producto p where p.IdProducto=@IdProductoU)  
   
                         
 if(@Concepto='I')  
 begin   
     
 set @StockFinalB=@IniciaStockB + @CantidadSalB                                      
    
 insert into Kardex values(@IdProductoU,@FechaRegistro,@KardexDocu,@Serie+'-'+@cod,@IniciaStockB,                          
 @CantidadSalB,0,0,@StockFinalB,'INGRESO',@Usuario)   
                                           
 update producto                         
 set  ProductoCantidad =ProductoCantidad + @CantidadSalB                       
 where IDProducto=@IdProductoU   
        
 end  
 else  
 begin  
   
 set @StockFinalB=@IniciaStockB - @CantidadSalB                                      
   
 insert into Kardex values(@IdProductoU,@FechaRegistro,@KardexDocu,@Serie+'-'+@cod,@IniciaStockB,                          
 0,@CantidadSalB,0,@StockFinalB,'SALIDA',@Usuario)   
                                         
 update producto                         
 set  ProductoCantidad =ProductoCantidad - @CantidadSalB                       
 where IDProducto=@IdProductoU    
   
 end  
  
Fetch Next From TablaB INTO @ColumnaB      
end      
    Close TablaB;      
    Deallocate TablaB;      
    Commit Transaction;      
    select convert(varchar,@GuiaId)+'¬'+@cod  
end      
else      
begin      
    Commit Transaction;      
    select convert(varchar,@GuiaId)+'¬'+@cod  
end  
                  
end
GO
/****** Object:  StoredProcedure [dbo].[uspinsertaLiquidaVenta]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[uspinsertaLiquidaVenta]      
@ListaOrden varchar(Max)      
as      
begin      
Declare @pos1 int,@pos2 int      
Declare @orden varchar(max),      
        @detalle varchar(max)      
Set @pos1 = CharIndex('[',@ListaOrden,0)      
Set @pos2 =Len(@ListaOrden)+1      
Set @orden = SUBSTRING(@ListaOrden,1,@pos1-1)      
Set @detalle = SUBSTRING(@ListaOrden,@pos1+1,@pos2-@pos1-1)      
Declare @c1 int,@c2 int,@c3 int,@c4 int,      
        @c5 int,@c6 int,@c7 int,@c8 int,      
        @c9 int,@c10 int,@c11 int      
Declare      
@LiquidacionNumero varchar(80),      
@LiquidacionFecha date,      
@LiquidacionDescripcion varchar(250),      
@LiquidacionCambio decimal(18,3),      
@LiquidaEfectivoSol decimal(18,2),      
@LiquidaDepositoSol decimal(18,2),      
@LiquidaTotalSol decimal(18,2),      
@LiquidaEfectivoDol decimal(18,2),      
@LiquidaDepositoDol decimal(18,2),      
@LiquidaTotalDol decimal(18,2),      
@LiquidaUsuario varchar(60)  
     
Set @c1 = CharIndex('|',@orden,0)      
Set @c2 = CharIndex('|',@orden,@c1+1)      
Set @c3 = CharIndex('|',@orden,@c2+1)      
Set @c4 = CharIndex('|',@orden,@c3+1)      
Set @c5 = CharIndex('|',@orden,@c4+1)      
Set @c6= CharIndex('|',@orden,@c5+1)      
Set @c7 = CharIndex('|',@orden,@c6+1)      
Set @c8 = CharIndex('|',@orden,@c7+1)      
Set @c9 = CharIndex('|',@orden,@c8+1)      
Set @c10= CharIndex('|',@orden,@c9+1)      
Set @c11= Len(@orden)+1       
  
set @LiquidacionNumero=SUBSTRING(@orden,1,@c1-1)      
set @LiquidacionFecha=convert(date,SUBSTRING(@orden,@c1+1,@c2-@c1-1))      
set @LiquidacionDescripcion=SUBSTRING(@orden,@c2+1,@c3-@c2-1)      
set @LiquidacionCambio=Convert(decimal(18,3),SUBSTRING(@orden,@c3+1,@c4-@c3-1))      
set @LiquidaEfectivoSol=convert(decimal(18,2),SUBSTRING(@orden,@c4+1,@c5-@c4-1))      
set @LiquidaDepositoSol=convert(decimal(18,2),SUBSTRING(@orden,@c5+1,@c6-@c5-1))      
set @LiquidaTotalSol=convert(decimal(18,2),SUBSTRING(@orden,@c6+1,@c7-@c6-1))      
set @LiquidaEfectivoDol=convert(decimal(18,2),SUBSTRING(@orden,@c7+1,@c8-@c7-1))      
set @LiquidaDepositoDol=convert(decimal(18,2),SUBSTRING(@orden,@c8+1,@c9-@c8-1))      
set @LiquidaTotalDol=convert(decimal(18,2),SUBSTRING(@orden,@c9+1,@c10-@c9-1))      
set @LiquidaUsuario=SUBSTRING(@orden,@c10+1,@c11-@c10-1)      
     
Declare @LiquidacionId numeric(38)  
  
Declare @CajaIdB numeric(38)  
set @CajaIdB=(select isnull((select stuff((select '¬'+ convert(varchar,c.CajaId)  
from Caja c where c.CajaEstado='ACTIVO' order by c.CajaId desc for xml path('')),1,1,'')),'0'))  
  
if(@CajaIdB='0')  
begin  
select 'existe'  
end  
else  
begin      
Begin Transaction    
declare @cod varchar(13)              
SET @cod=ISNULL((select TOP 1 dbo.geneneraIdLiVenta('0001') AS ID FROM LiquidacionVenta),'0001-00000001')        
insert into LiquidacionVenta values(@cod,      
GETDATE(),@LiquidacionFecha,@LiquidacionDescripcion,      
@LiquidacionCambio,@LiquidaEfectivoSol,@LiquidaDepositoSol,      
@LiquidaTotalSol,@LiquidaEfectivoDol,@LiquidaDepositoDol,      
@LiquidaTotalDol,@LiquidaUsuario)      
set @LiquidacionId=(select @@identity)    
      
Declare Tabla Cursor For Select * From fnSplitString(@detalle,';')       
Open Tabla      
Declare @Columna varchar(max),      
  @DocuId numeric(38),      
  @NotaId numeric(38),      
  @SaldoDocu decimal(18,2),      
  @EfectivoSoles decimal(18, 2),      
  @EfectivoDolar decimal(18, 2),      
  @DepositoSoles decimal(18, 2),      
  @DepositoDolar decimal(18, 2),      
  @TipoCambio decimal(18, 3),      
  @EntidadBanco varchar(80),      
  @NroOperacion varchar(80),      
  @AcuentaGeneral decimal(18, 2),      
  @SaldoActual decimal(18, 2),      
  @FechaPago varchar(60),      
  @DocuEstado varchar(60),      
  @NumeroDoc varchar(80),      
  @DetalleId numeric(38),
  @Cliente varchar(350)      
Declare @p1 int,@p2 int,@p3 int,@p4 int,      
        @p5 int,@p6 int,@p7 int,@p8 int,      
        @p9 int,@p10 int,@p11 int,      
        @p12 int,@p13 int,@p14 int,@p15 int      
Fetch Next From Tabla INTO @Columna      
 While @@FETCH_STATUS = 0      
 Begin      
Set @p1 = CharIndex('|',@Columna,0)      
Set @p2 = CharIndex('|',@Columna,@p1+1)      
Set @p3 = CharIndex('|',@Columna,@p2+1)      
Set @p4 = CharIndex('|',@Columna,@p3+1)      
Set @p5 = CharIndex('|',@Columna,@p4+1)      
Set @p6= CharIndex('|',@Columna,@p5+1)      
Set @p7= CharIndex('|',@Columna,@p6+1)      
Set @p8 = CharIndex('|',@Columna,@p7+1)      
Set @p9 = CharIndex('|',@Columna,@p8+1)      
Set @p10 = CharIndex('|',@Columna,@p9+1)      
Set @p11= CharIndex('|',@Columna,@p10+1)      
Set @p12= CharIndex('|',@Columna,@p11+1)      
Set @p13= CharIndex('|',@Columna,@p12+1)
Set @p14= CharIndex('|',@Columna,@p13+1)      
Set @p15=Len(@Columna)+1      
set @DocuId=Convert(numeric(38),SUBSTRING(@Columna,1,@p1-1))      
Set @NumeroDoc=SUBSTRING(@Columna,@p1+1,@p2-(@p1+1))      
Set @SaldoDocu=convert(decimal(18,2),SUBSTRING(@Columna,@p2+1,@p3-(@p2+1)))      
Set @TipoCambio=convert(decimal(18,3),SUBSTRING(@Columna,@p3+1,@p4-(@p3+1)))      
Set @EfectivoSoles=convert(decimal(18,2),SUBSTRING(@Columna,@p4+1,@p5-(@p4+1)))      
Set @EfectivoDolar=convert(decimal(18,2),SUBSTRING(@Columna,@p5+1,@p6-(@p5+1)))      
Set @DepositoSoles=convert(decimal(18,2),SUBSTRING(@Columna,@p6+1,@p7-(@p6+1)))      
Set @DepositoDolar=convert(decimal(18,2),SUBSTRING(@Columna,@p7+1,@p8-(@p7+1)))      
Set @EntidadBanco=SUBSTRING(@Columna,@p8+1,@p9-(@p8+1))      
Set @NroOperacion=SUBSTRING(@Columna,@p9+1,@p10-(@p9+1))      
Set @AcuentaGeneral=convert(decimal(18,2),SUBSTRING(@Columna,@p10+1,@p11-(@p10+1)))      
Set @FechaPago=SUBSTRING(@Columna,@p11+1,@p12-(@p11+1))      
Set @SaldoActual=convert(decimal(18,2),SUBSTRING(@Columna,@p12+1,@p13-(@p12+1)))      
Set @NotaId=convert(numeric(38),SUBSTRING(@Columna,@p13+1,@p14-(@p13+1)))
Set @Cliente=SUBSTRING(@Columna,@p14+1,@p15-(@p14+1))
      
if (@SaldoActual <= 0) set @DocuEstado='CANCELADO'      
else set @DocuEstado='EMITIDO'      
insert into DetaLiquidaVenta values(      
@LiquidacionId,@DocuId,@NotaId,@SaldoDocu,@EfectivoSoles,      
@EfectivoDolar,@DepositoSoles,@DepositoDolar,      
@TipoCambio,@EntidadBanco,@NroOperacion,      
@AcuentaGeneral,@SaldoActual,@FechaPago,@NumeroDoc,@Cliente)      
set @DetalleId=(select @@IDENTITY)      
if(@DepositoSoles>0)      
begin      
insert into CajaDetalle values(@CajaIdB,GETDATE(),'0','DEPOSITO','LIQUIDACION',      
'LIQUIDACION DEL DOC NRO '+@NumeroDoc,@DepositoSoles,      
@DepositoSoles,0,'','T','',@LiquidaUsuario,'',CONVERT(varchar,@DetalleId))      
end      
if(@EfectivoSoles>0)      
BEGIN      
insert into CajaDetalle values(@CajaIdB,GETDATE(),'0','INGRESO','LIQUIDACION',      
'LIQUIDACION DEL DOC NRO '+@NumeroDoc,@EfectivoSoles,      
@EfectivoSoles,0,'','T','',@LiquidaUsuario,'',CONVERT(varchar,@DetalleId))      
END      
update NotaPedido      
set NotaSaldo=NotaSaldo-@AcuentaGeneral,NotaEstado=@DocuEstado      
where NotaId=@NotaId      
Fetch Next From Tabla INTO @Columna      
end      
 Close Tabla;      
 Deallocate Tabla;      
 Commit Transaction;      
 SELECT 'true'      
end  
end
GO
/****** Object:  StoredProcedure [dbo].[uspInsertarArea]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspInsertarArea] 
@Data varchar(max)  
as  
begin  
Declare @pos1 int, @pos2 int
Declare @Id int, @Nombre varchar (80)

Set @Data = LTRIM(RTrim(@Data))  
Set @pos1 = CharIndex('|',@Data,0)  
Set @pos2 = Len(@Data)+1  
  
Set @Id=convert(int,SUBSTRING(@Data,1,@pos1-1))  
Set @Nombre=SUBSTRING(@Data,@pos1+1,@pos2-@pos1-1)  

If(@Id=0)  
Begin  
 IF EXISTS(select top 1 a.AreaNombre   
      from Area a where a.AreaNombre=@Nombre)   
 begin  
  select 'existe'   
 end   
 else   
 begin   
  INSERT INTO Area (AreaNombre) VALUES (@Nombre)  
  select 'true'   
 end  
end  
Else  
Begin
  IF EXISTS(select top 1 a.AreaNombre   
      from Area a where a.AreaNombre=@Nombre and AreaId<>@Id)   
 begin  
  select 'existe'   
 end
 Else
 Begin
	 UPDATE Area 
	 SET AreaNombre = @Nombre 
	 WHERE AreaId = @Id
 End
End  
End
GO
/****** Object:  StoredProcedure [dbo].[uspInsertarCategoria]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[uspInsertarCategoria]    
@Data VARCHAR(MAX)    
AS    
BEGIN    

DECLARE @pos1 INT, @pos2 INT, @pos3 INT    
DECLARE @Id INT, @Nombre VARCHAR(300), @Codigo VARCHAR(40)    
DECLARE @NewId INT    

SET @Data = LTRIM(RTRIM(@Data))    

SET @pos1 = CHARINDEX('|',@Data,0)    
SET @pos2 = CHARINDEX('|',@Data,@pos1+1)    
SET @pos3 = LEN(@Data)+1    

SET @Id = CONVERT(INT,SUBSTRING(@Data,1,@pos1-1))    
SET @Nombre = SUBSTRING(@Data,@pos1+1,@pos2-@pos1-1)    
SET @Codigo = SUBSTRING(@Data,@pos2+1,@pos3-@pos2-1)    

IF(@Id=0)    
BEGIN    

    IF EXISTS(SELECT TOP 1 s.NombreSubLinea     
              FROM Sublinea s 
              WHERE s.NombreSubLinea=@Nombre)     
    BEGIN    
        SELECT 'existe'     
    END     

    ELSE     
    BEGIN     
        INSERT INTO Sublinea (NombreSublinea, CodigoSunat) 
        VALUES (@Nombre, @Codigo)    

        SET @NewId = SCOPE_IDENTITY()

        SELECT 
        CAST(@NewId AS VARCHAR) + '|' + @Nombre
    END    

END    

ELSE    

BEGIN  

    IF EXISTS(SELECT TOP 1 s.NombreSubLinea     
              FROM Sublinea s 
              WHERE s.NombreSubLinea=@Nombre 
              AND IdSubLinea<>@Id)     

    BEGIN    
        SELECT 'existe'     
    END  

    ELSE  

    BEGIN  

        UPDATE Sublinea     
        SET NombreSublinea = @Nombre, 
            CodigoSunat = @Codigo     
        WHERE IdSubLinea = @Id  

        SELECT 
        CAST(@Id AS VARCHAR) + '|' + @Nombre

    END  

END    

END
GO
/****** Object:  StoredProcedure [dbo].[uspInsertarCliente]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[uspInsertarCliente]
@Data varchar(max)
AS
BEGIN

Declare @Id numeric(20),
        @ClienteRazon varchar(140),
        @ClienteRuc varchar(40),
        @ClienteDni varchar(40),
        @ClienteDireccion varchar(max),
        @ClienteTelefono varchar(80),
        @ClienteCorreo varchar(80),
        @ClienteEstado varchar(40),
        @ClienteDespacho varchar(max),
        @ClienteUsuario varchar(40)

Declare @pos1 int,@pos2 int,@pos3 int,
        @pos4 int,@pos5 int,@pos6 int,
        @pos7 int,@pos8 int,@pos9 int,
        @pos10 int

Set @Data=LTRIM(RTRIM(@Data))

Set @pos1=CharIndex('|',@Data,0)
Set @pos2=CharIndex('|',@Data,@pos1+1)
Set @pos3=CharIndex('|',@Data,@pos2+1)
Set @pos4=CharIndex('|',@Data,@pos3+1)
Set @pos5=CharIndex('|',@Data,@pos4+1)
Set @pos6=CharIndex('|',@Data,@pos5+1)
Set @pos7=CharIndex('|',@Data,@pos6+1)
Set @pos8=CharIndex('|',@Data,@pos7+1)
Set @pos9=CharIndex('|',@Data,@pos8+1)
Set @pos10=Len(@Data)+1

Set @Id=convert(numeric(20),SUBSTRING(@Data,1,@pos1-1))
Set @ClienteRazon=SUBSTRING(@Data,@pos1+1,@pos2-@pos1-1)
Set @ClienteRuc=SUBSTRING(@Data,@pos2+1,@pos3-@pos2-1)
Set @ClienteDni=SUBSTRING(@Data,@pos3+1,@pos4-@pos3-1)
Set @ClienteDireccion=SUBSTRING(@Data,@pos4+1,@pos5-@pos4-1)
Set @ClienteTelefono=SUBSTRING(@Data,@pos5+1,@pos6-@pos5-1)
Set @ClienteCorreo=SUBSTRING(@Data,@pos6+1,@pos7-@pos6-1)
Set @ClienteEstado=SUBSTRING(@Data,@pos7+1,@pos8-@pos7-1)
Set @ClienteDespacho=SUBSTRING(@Data,@pos8+1,@pos9-@pos8-1)
Set @ClienteUsuario=SUBSTRING(@Data,@pos9+1,@pos10-@pos9-1)


IF(@Id=0)
BEGIN

    IF EXISTS(select top 1 c.ClienteDni
              from Cliente c where c.ClienteDni=@ClienteDni and ClienteDni<>'')
    BEGIN
        SELECT 'existe DNI'
    END

    ELSE IF EXISTS(select top 1 c.ClienteRuc
                   from Cliente c where c.ClienteRuc=@ClienteRuc and ClienteRuc<>'')
    BEGIN
        SELECT 'existe RUC'
    END

    ELSE
    BEGIN

        INSERT INTO Cliente
        (ClienteRazon,ClienteRuc,ClienteDni,
         ClienteDireccion,ClienteTelefono,ClienteCorreo,
         ClienteEstado,ClienteDespacho,ClienteUsuario,ClienteFecha)
        VALUES
        (@ClienteRazon,@ClienteRuc,@ClienteDni,@ClienteDireccion,
         @ClienteTelefono,@ClienteCorreo,@ClienteEstado,
         @ClienteDespacho,@ClienteUsuario,GETDATE())

        DECLARE @NuevoId INT
        SET @NuevoId = SCOPE_IDENTITY()

        SELECT CONVERT(varchar,@NuevoId)+'|'+@ClienteRazon

    END

END

ELSE
BEGIN

    IF EXISTS(select top 1 c.ClienteDni
              from Cliente c where c.ClienteDni=@ClienteDni
              and (ClienteDni<>'' and ClienteId<>@Id))
    BEGIN
        SELECT 'existe DNI'
    END

    ELSE IF EXISTS(select top 1 c.ClienteRuc
                   from Cliente c where c.ClienteRuc=@ClienteRuc
                   and (ClienteRuc<>'' and ClienteId<>@Id))
    BEGIN
        SELECT 'existe RUC'
    END

    ELSE
    BEGIN

        UPDATE Cliente SET
        ClienteRazon = @ClienteRazon,
        ClienteRuc = @ClienteRuc,
        ClienteDni = @ClienteDni,
        ClienteDireccion = @ClienteDireccion,
        ClienteTelefono = @ClienteTelefono,
        ClienteCorreo = @ClienteCorreo,
        ClienteEstado = @ClienteEstado,
        ClienteDespacho = @ClienteDespacho,
        ClienteUsuario = @ClienteUsuario,
        ClienteFecha = GETDATE()
        WHERE ClienteId = @Id

        SELECT CONVERT(varchar,@Id)+'|'+@ClienteRazon

    END

END

END


GO
/****** Object:  StoredProcedure [dbo].[uspInsertarCuenta]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspInsertarCuenta]
@Data varchar(max)
as
begin
Declare @pos1 int
Declare @pos2 int
Declare @pos3 int
Declare @pos4 int
Declare @pos5 int
Declare @pos6 int
declare @CuentaId numeric(38),
@ProveedorId numeric(38),
@Entidad varchar(80),
@TipoCuenta varchar(80),
@Moneda varchar(80),
@NroCuenta varchar(80)
Set @Data = LTRIM(RTrim(@Data))
Set @pos1 = CharIndex('|',@Data,0)
Set @CuentaId=convert(numeric(38),SUBSTRING(@Data,1,@pos1-1))
Set @pos2 = CharIndex('|',@Data,@pos1+1)
Set @ProveedorId=convert(numeric(38),SUBSTRING(@Data,@pos1+1,@pos2-@pos1-1))
Set @pos3 = CharIndex('|',@Data,@pos2+1)
Set @Entidad=SUBSTRING(@Data,@pos2+1,@pos3-@pos2-1)
Set @pos4 = CharIndex('|',@Data,@pos3+1)
Set @TipoCuenta=SUBSTRING(@Data,@pos3+1,@pos4-@pos3-1)
Set @pos5 = CharIndex('|',@Data,@pos4+1)
Set @Moneda=SUBSTRING(@Data,@pos4+1,@pos5-@pos4-1)
Set @pos6 = Len(@Data)+1
Set @NroCuenta=SUBSTRING(@Data,@pos5+1,@pos6-@pos5-1)
if(@CuentaId=0)
begin
insert into CuentaProveedor values(@ProveedorId,@Entidad,@TipoCuenta,@Moneda,@NroCuenta)
select isnull((select STUFF ((select '¬'+ CONVERT(varchar,c.CuentaId)+'|'+c.Entidad+'|'+
c.TipoCuenta+'|'+c.Moneda+'|'+c.NroCuenta
from CuentaProveedor c
where c.ProveedorId=@ProveedorId
order by c.CuentaId desc
for xml path('')),1,1,'')),'~')
end
else
begin
update CuentaProveedor
set TipoCuenta=@TipoCuenta,NroCuenta=@NroCuenta
where CuentaId=@CuentaId
select 'true'
end
end
GO
/****** Object:  StoredProcedure [dbo].[uspInsertarCuentaProveedor]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspInsertarCuentaProveedor]
@Data varchar(max)
as
begin

Declare	@CuentaId int,
	    @ProveedorId int,
	    @Entidad varchar(80),
	    @TipoCuenta varchar(80),
	    @Moneda varchar(80),
	    @NroCuenta varchar(80)

Declare @pos1 int, @pos2 int,@pos3 int,  
		@pos4 int, @pos5 int,@pos6 int

Set @Data= LTRIM(RTrim(@Data))  
Set @pos1=CharIndex('|',@Data,0)  
Set @pos2=CharIndex('|',@Data,@pos1+1)  
Set @pos3=CharIndex('|',@Data,@pos2+1)  
Set @pos4=CharIndex('|',@Data,@pos3+1)  
Set @pos5=CharIndex('|',@Data,@pos4+1)
Set @pos6=Len(@Data)+1

Set @CuentaId=convert(int,SUBSTRING(@Data,1,@pos1-1))  
Set @ProveedorId=convert(int,SUBSTRING(@Data,@pos1+1,@pos2-@pos1-1))  
Set @Entidad=SUBSTRING(@Data,@pos2+1,@pos3-@pos2-1)  
Set @TipoCuenta=SUBSTRING(@Data,@pos3+1,@pos4-@pos3-1)  
Set @Moneda=SUBSTRING(@Data,@pos4+1,@pos5-@pos4-1)  
Set @NroCuenta=SUBSTRING(@Data,@pos5+1,@pos6-@pos5-1)

if(@CuentaId=0)
Begin
    
	IF EXISTS(select top 1 c.NroCuenta  
			 from CuentaProveedor c where c.NroCuenta=@NroCuenta)   
	 begin  
	   select 'existe Cuenta'   
	 End
    Else
	Begin
		
		INSERT INTO CuentaProveedor (ProveedorId, Entidad, TipoCuenta, Moneda, NroCuenta)
		VALUES (@ProveedorId, @Entidad, @TipoCuenta, @Moneda, @NroCuenta);

	    Set @CuentaId=@@IDENTITY

	    SELECT @CuentaId
	End
End
Else
Begin
   IF EXISTS(select top 1 c.NroCuenta  
			 from CuentaProveedor c where c.NroCuenta=@NroCuenta and CuentaId<>@CuentaId)   
	 begin  
	   select 'existe Cuenta'   
	 End
	 Begin

		UPDATE CuentaProveedor
		SET Entidad = @Entidad,
			TipoCuenta = @TipoCuenta,
			Moneda = @Moneda,
			NroCuenta = @NroCuenta
		WHERE CuentaId = @CuentaId AND ProveedorId = @ProveedorId;

		select 'true'
      
	  End
End
end
GO
/****** Object:  StoredProcedure [dbo].[uspInsertarDesbloqueo]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[uspInsertarDesbloqueo]
@Data varchar(max)
as
begin
Declare @pos1 int,@pos2 int,
		@pos3 int,@pos4 int
declare @Fecha datetime,
		@IdAdmin int,@IdPersonal int,
        @Descripcion varchar(max)
Set @Data = LTRIM(RTrim(@Data))
Set @pos1 = CharIndex('|',@Data,0)
Set @pos2 = CharIndex('|',@Data,@pos1+1)
Set @pos3 = CharIndex('|',@Data,@pos2+1)
Set @pos4 = Len(@Data)+1
Set @Fecha=convert(date,SUBSTRING(@Data,1,@pos1-1))
Set @IdAdmin=convert(int,SUBSTRING(@Data,@pos1+1,@pos2-@pos1-1))
Set @IdPersonal=convert(int,SUBSTRING(@Data,@pos2+1,@pos3-@pos2-1))
Set @Descripcion=SUBSTRING(@Data,@pos3+1,@pos4-@pos3-1)
declare @permisos int
set @permisos=isnull((select top 1 MaxDesbloqueo from AdministraBL a
where a.IdAdmin=@IdAdmin),0)
Declare @Cantidad int
set @Cantidad=(select count(d.IdPersonal) from Desbloqueo d
inner join PersonalBL p
on p.IdPersonal=d.IdPersonal and p.Area=0
where d.IdPersonal=@IdPersonal and
(month(d.Fecha)=MONTH(GETDATE()) and 
YEAR(d.Fecha)=YEAR(GETDATE())))
if(@Cantidad>=@permisos)
begin
select 'limite' as 'A'
end
else
begin
IF EXISTS(select top 1 d.IdPersonal from Desbloqueo d
where d.IdPersonal=@IdPersonal and convert(date,d.Fecha)=convert(date,@Fecha))
begin
select 'existe' as 'A'
end
else
begin
insert into Desbloqueo values(@Fecha,@IdAdmin,@IdPersonal,@Descripcion)
select 'true' as 'A'
end 
end
end
GO
/****** Object:  StoredProcedure [dbo].[uspinsertaRechazo]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
Declare  @pos1 int,@pos2 int,@pos3 int,@pos4 int,      
         @pos5 int,@pos6 int,@pos7 int,@pos8 int,      
         @pos9 int,@pos10 int,@pos11 int,@pos12 int,      
         @pos13 int,@pos14 int,@pos15 int,@pos16 int,      
         @pos17 int,@pos18 int,@pos19 int,@pos20 int,      
         @pos21 int,@pos22 int,@pos23 int,@pos24 int,      
         @pos25 int      
 Declare @CompaniaId int,@NotaId numeric(38),@DocuDocumento varchar(60),      
         @DocuNumero varchar(60),@ClienteId numeric(20),@DocuEmision date,      
         @DocuSubTotal decimal(18,2),@DocuIgv decimal(18,2),@DocuTotal decimal(18,2),      
         @DocuUsuario varchar(60),@DocuSerie char(4),@TipoCodigo char(20),      
         @DocuAdicional decimal(18,2),@DocuAsociado varchar(80),@DocuConcepto varchar(80),      
         @DocuHASH varchar(250),@EstadoSunat varchar(80),@Letras varchar(60),      
         @DocuId numeric(38),@TraeEstado varchar(80),@NotaEstado varchar(80),      
         @ICBPER decimal(18,2),@CodigoSunat VARCHAR(80),@MensajeSunat varchar(max),
         @ClienteRazon varchar(140),@ClienteRuc varchar(40),
         @ClienteDni varchar(40),@DireccionFiscal varchar(max)      
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
Set @pos20= CharIndex('|',@orden,@pos19+1)
Set @pos21= CharIndex('|',@orden,@pos20+1)      
Set @pos22= CharIndex('|',@orden,@pos21+1)      
Set @pos23= CharIndex('|',@orden,@pos22+1)      
Set @pos24= CharIndex('|',@orden,@pos23+1)      
Set @pos25= Len(@orden)+1
      
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
set @ICBPER=convert(decimal(18,2),SUBSTRING(@orden,@pos18+1,@pos19-@pos18-1))      
set @CodigoSunat=SUBSTRING(@orden,@pos19+1,@pos20-@pos19-1)      
set @MensajeSunat=SUBSTRING(@orden,@pos20+1,@pos21-@pos20-1)      

set @ClienteRazon=SUBSTRING(@orden,@pos21+1,@pos22-@pos21-1)          
set @ClienteRuc=SUBSTRING(@orden,@pos22+1,@pos23-@pos22-1) 
set @ClienteDni=SUBSTRING(@orden,@pos23+1,@pos24-@pos23-1)          
set @DireccionFiscal=SUBSTRING(@orden,@pos24+1,@pos25-@pos24-1)   

set @TraeEstado=(select top 1 n.NotaEstado from NotaPedido n where n.NotaId=@NotaId)      
if(@TraeEstado='PENDIENTE')set @NotaEstado='EMITIDO'      
else set @NotaEstado=@TraeEstado      

Begin Transaction      
insert into DocumentoVenta values(@CompaniaId,@NotaId,@DocuDocumento,@DocuNumero,      
@ClienteId,GETDATE(),@DocuEmision,'ALCONTADO','CERO CON 00/100 SOLES',0,0,0,0,      
@DocuUsuario,'RECHAZADO',@DocuSerie,@TipoCodigo,0,@DocuAsociado,      
@DocuConcepto,'',@DocuHASH,'RECHAZADO',0,@CodigoSunat,@MensajeSunat,0,0,'','EFECTIVO','','',0,0,
@ClienteRazon,@ClienteRuc,@ClienteDni,@DireccionFiscal)

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
  @ValorUM decimal(18,4),    
  @Descripcion varchar(300)     
Declare @p1 int,@p2 int,@p3 int,@p4 int,      
        @p5 int,@p6 int,@p7 int,@p8 int     
Fetch Next From Tabla INTO @Columna      
 While @@FETCH_STATUS = 0      
 Begin      
Set @p1 = CharIndex('|',@Columna,0)      
Set @p2 = CharIndex('|',@Columna,@p1+1)      
Set @p3 = CharIndex('|',@Columna,@p2+1)      
Set @p4 = CharIndex('|',@Columna,@p3+1)      
Set @p5 = CharIndex('|',@Columna,@p4+1)      
Set @p6= CharIndex('|',@Columna,@p5+1)    
Set @p7= CharIndex('|',@Columna,@p6+1)      
Set @p8 = Len(@Columna)+1      
Set @DetalleNotaId=Convert(numeric(38),SUBSTRING(@Columna,1,@p1-1))      
Set @IdProducto=Convert(numeric(20),SUBSTRING(@Columna,@p1+1,@p2-(@p1+1)))      
Set @Cantidad=Convert(decimal(18,2),SUBSTRING(@Columna,@p2+1,@p3-(@p2+1)))      
Set @UM=SUBSTRING(@Columna,@p3+1,@p4-(@p3+1))      
Set @Precio=Convert(decimal(18,2),SUBSTRING(@Columna,@p4+1,@p5-(@p4+1)))      
Set @Importe=Convert(decimal(18,2),SUBSTRING(@Columna,@p5+1,@p6-(@p5+1)))      
Set @ValorUM=Convert(decimal(18,4),SUBSTRING(@Columna,@p6+1,@p7-(@p6+1)))    
Set @Descripcion=SUBSTRING(@Columna,@p7+1,@p8-(@p7+1))    
      
insert into DetalleDocumento       
values(@DocuId,@IdProducto,@Cantidad,@Precio,@Importe,@DetalleNotaId,@UM,@ValorUM,@Descripcion)      
    
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
GO
/****** Object:  StoredProcedure [dbo].[uspInsertarMaquina]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspInsertarMaquina]
@Data varchar(max)
as
begin
Declare @Id int,@Maquina varchar(140), 
		@SerieFactura nvarchar(4),
		@SerieNC nvarchar(4),
		@SerieBoleta nvarchar(4),
		@Tiketera varchar(300)

Declare @pos1 int, @pos2 int,@pos3 int,
        @pos4 int, @pos5 int,@pos6 int

Set @Data = LTRIM(RTrim(@Data))
Set @pos1 = CharIndex('|',@Data,0)
Set @pos2 = CharIndex('|',@Data,@pos1+1)
Set @pos3 = CharIndex('|',@Data,@pos2+1)
Set @pos4 = CharIndex('|',@Data,@pos3+1)
Set @pos5 = CharIndex('|',@Data,@pos4+1)
Set @pos6 = Len(@Data)+1

Set @Id=convert(int,SUBSTRING(@Data,1,@pos1-1))
Set @Maquina=SUBSTRING(@Data,@pos1+1,@pos2-@pos1-1)
Set @SerieFactura=SUBSTRING(@Data,@pos2+1,@pos3-@pos2-1)
Set @SerieNC=SUBSTRING(@Data,@pos3+1,@pos4-@pos3-1)
Set @SerieBoleta=SUBSTRING(@Data,@pos4+1,@pos5-@pos4-1)
Set @Tiketera=SUBSTRING(@Data,@pos5+1,@pos6-@pos5-1)

if(@Id=0)
begin
	IF EXISTS(select top 1 m.Maquina
			   from MAQUINAS m where m.Maquina=@Maquina) 
	begin
		select 'existe' 
	end
	else
	begin
		INSERT INTO MAQUINAS (Maquina, Registro, SerieFactura, SerieNC, SerieBoleta, Tiketera)
                  VALUES (@Maquina,GETDATE(),@SerieFactura, @SerieNC, @SerieBoleta, @Tiketera)
		select 'true'
	end
End
Else
Begin

  	IF EXISTS(select top 1 m.Maquina
			   from MAQUINAS m where m.Maquina=@Maquina and IdMaquina<>@Id) 
	begin
		select 'existe' 
	end
	Else
	begin

		UPDATE MAQUINAS
		SET Maquina = @Maquina,Registro = getdate(),
		SerieFactura = @SerieFactura,
		SerieNC = @SerieNC,
		SerieBoleta = @SerieBoleta,
		Tiketera = @Tiketera
		WHERE IdMaquina = @Id

		select 'true'
	end
End

end
GO
/****** Object:  StoredProcedure [dbo].[uspinsertarNC]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspinsertarNC]                
@ListaOrden varchar(Max)                
as                
begin                
  
Declare @posA1 int,@posA2 int,@posA3 int      
Declare @orden varchar(max),      
        @detalle varchar(max),      
        @Guia varchar(max)      
Set @posA1 = CharIndex('[',@ListaOrden,0)      
Set @posA2 = CharIndex('[',@ListaOrden,@posA1+1)      
Set @posA3 =Len(@ListaOrden)+1      
Set @orden = SUBSTRING(@ListaOrden,1,@posA1-1)      
Set @detalle = SUBSTRING(@ListaOrden,@posA1+1,@posA2-@posA1-1)      
Set @Guia=SUBSTRING(@ListaOrden,@posA2+1,@posA3-@posA2-1)   
               
Declare @pos1 int,@pos2 int,@pos3 int,@pos4 int,                
        @pos5 int,@pos6 int,@pos7 int,@pos8 int,                
        @pos9 int,@pos10 int,@pos11 int,@pos12 int,                
        @pos13 int,@pos14 int,@pos15 int,@pos16 int,                
        @pos17 int,@pos18 int,@pos19 int,@pos20 int,                
        @pos21 int,@pos22 int,@pos23 int,@pos24 int,        
        @pos25 int,@pos26 int,@pos27 int,@pos28 int,    
        @pos29 int,@pos30 int,@pos31 int,@pos32 int,@pos33 int              
Declare @CompaniaId int,@NotaId numeric(38),@DocuDocumento varchar(60),                
        @DocuNumero varchar(60),@ClienteId numeric(20),@DocuEmision date,                
        @DocuSubTotal decimal(18,2),@DocuIgv decimal(18,2),@DocuTotal decimal(18,2),                
        @DocuUsuario varchar(60),@DocuSerie char(4),@TipoCodigo varchar(10),                
        @DocuAdicional decimal(18,2),@DocuAsociado varchar(80),@DocuConcepto varchar(80),                
        @DocuHASH varchar(250),@EstadoSunat varchar(80),@Letras varchar(60),@NroReferencia varchar(80),                
        @DocuId numeric(38),@KardexDocu varchar(80),@ICBPER decimal(18,2),                
        @CodigoSunat VARCHAR(80),@MensajeSunat varchar(max),                
        @DocuGravada decimal(18,2),@DocuDescuento decimal(18,2),        
        @Efectivo decimal(18,2),@Deposito decimal(18,2),@ClienteRazon varchar(140),    
        @ClienteRuc varchar(40),@ClienteDni varchar(40),@DireccionFiscal varchar(max),      
        @FormaPago varchar(60),@EntidadBancaria varchar(80),@NroOperacion varchar(80)             
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
Set @pos20= CharIndex('|',@orden,@pos19+1)                
Set @pos21= CharIndex('|',@orden,@pos20+1)                
Set @pos22= CharIndex('|',@orden,@pos21+1)                
Set @pos23= CharIndex('|',@orden,@pos22+1)         
Set @pos24= CharIndex('|',@orden,@pos23+1)                
Set @pos25= CharIndex('|',@orden,@pos24+1)         
Set @pos26= CharIndex('|',@orden,@pos25+1)                
Set @pos27= CharIndex('|',@orden,@pos26+1)                
Set @pos28= CharIndex('|',@orden,@pos27+1)         
Set @pos29= CharIndex('|',@orden,@pos28+1)                
Set @pos30= CharIndex('|',@orden,@pos29+1)    
Set @pos31= CharIndex('|',@orden,@pos30+1)                
Set @pos32= CharIndex('|',@orden,@pos31+1)                      
Set @pos33= Len(@orden)+1                
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
set @NroReferencia=SUBSTRING(@orden,@pos18+1,@pos19-@pos18-1)                
set @ICBPER=convert(decimal(18,2),SUBSTRING(@orden,@pos19+1,@pos20-@pos19-1))                
set @CodigoSunat=SUBSTRING(@orden,@pos20+1,@pos21-@pos20-1)                
set @MensajeSunat=SUBSTRING(@orden,@pos21+1,@pos22-@pos21-1)                
set @DocuGravada=convert(decimal(18,2),SUBSTRING(@orden,@pos22+1,@pos23-@pos22-1))                
set @DocuDescuento=convert(decimal(18,2),SUBSTRING(@orden,@pos23+1,@pos24-@pos23-1))        
set @Efectivo=convert(decimal(18,2),SUBSTRING(@orden,@pos24+1,@pos25-@pos24-1))                
set @Deposito=convert(decimal(18,2),SUBSTRING(@orden,@pos25+1,@pos26-@pos25-1))      
set @ClienteRazon=SUBSTRING(@orden,@pos26+1,@pos27-@pos26-1)                
set @ClienteRuc=SUBSTRING(@orden,@pos27+1,@pos28-@pos27-1)       
set @ClienteDni=SUBSTRING(@orden,@pos28+1,@pos29-@pos28-1)                
set @DireccionFiscal=SUBSTRING(@orden,@pos29+1,@pos30-@pos29-1)      
set @FormaPago=SUBSTRING(@orden,@pos30+1,@pos31-@pos30-1)    
set @EntidadBancaria=SUBSTRING(@orden,@pos31+1,@pos32-@pos31-1)      
set @NroOperacion=SUBSTRING(@orden,@pos32+1,@pos33-@pos32-1)            
               
Begin Transaction                
 insert into DocumentoVenta values(@CompaniaId,@NotaId,@DocuDocumento,@DocuNumero,                
 @ClienteId,GETDATE(),@DocuEmision,'ALCONTADO',@Letras,@DocuSubTotal,@DocuIgv,@DocuTotal,0,                
 @DocuUsuario,'EMITIDO',@DocuSerie,@TipoCodigo,@DocuAdicional,@DocuAsociado,                
 @DocuConcepto,@NroReferencia,@DocuHASH,@EstadoSunat,@ICBPER,@CodigoSunat,@MensajeSunat,                
 @DocuGravada,@DocuDescuento,'',@FormaPago,@EntidadBancaria,@NroOperacion,@Efectivo,@Deposito,      
 @ClienteRazon,@ClienteRuc,@ClienteDni,@DireccionFiscal)             
                
 Set @DocuId= @@identity                
 set @KardexDocu=@DocuSerie+'-'+@DocuNumero                
               
 Update DocumentoVenta                
 set DocuAsociado=@DocuId                
 where DocuId=@DocuAsociado                
               
 update NotaPedido                
 set NotaEstado='ANULADO',NotaSaldo=@DocuTotal,NotaAcuenta=0                
 where NotaId=@NotaId      
                
Declare Tabla Cursor For Select * From fnSplitString(@detalle,';')                 
Open Tabla                
Declare @Columna varchar(max),                
  @IdProducto numeric(20),                
  @Cantidad decimal(18,2),    
  @Costo decimal(18,4),                
  @Precio decimal(18,2),                
  @Importe decimal(18,2),                
  @DetalleNotaId numeric(38),                
  @UM varchar(80),                
  @ValorUM decimal(18,4),              
  @Descripcion varchar(300),  
  @AplicaINV nvarchar(1),             
  @StockInicial decimal(18,2),                
  @StockFinal decimal(18,2),@CantidadIng decimal(18,2)                
Declare @p1 int,@p2 int,@p3 int,@p4 int,                
        @p5 int,@p6 int,@p7 int,@p8 int,
        @p9 int,@p10 int              
                        
Fetch Next From Tabla INTO @Columna                
 While @@FETCH_STATUS = 0                
 Begin                
Set @p1 = CharIndex('|',@Columna,0)                
Set @p2 = CharIndex('|',@Columna,@p1+1)                
Set @p3 = CharIndex('|',@Columna,@p2+1)                
Set @p4 = CharIndex('|',@Columna,@p3+1)                
Set @p5 = CharIndex('|',@Columna,@p4+1)                
Set @p6= CharIndex('|',@Columna,@p5+1)                
Set @p7= CharIndex('|',@Columna,@p6+1)              
Set @p8= CharIndex('|',@Columna,@p7+1)
Set @p9= CharIndex('|',@Columna,@p8+1)                 
Set @p10= Len(@Columna)+1                
              
Set @Cantidad=Convert(decimal(18,2),SUBSTRING(@Columna,1,@p1-1))                
Set @UM=SUBSTRING(@Columna,@p1+1,@p2-(@p1+1))            
Set @Descripcion=SUBSTRING(@Columna,@p2+1,@p3-(@p2+1))                 
Set @Precio=Convert(decimal(18,2),SUBSTRING(@Columna,@p3+1,@p4-(@p3+1)))                
Set @Importe=Convert(decimal(18,2),SUBSTRING(@Columna,@p4+1,@p5-(@p4+1)))                
Set @DetalleNotaId=Convert(numeric(38),SUBSTRING(@Columna,@p5+1,@p6-(@p5+1)))                
Set @IdProducto=Convert(numeric(20),SUBSTRING(@Columna,@p6+1,@p7-(@p6+1)))                
Set @ValorUM=Convert(decimal(18,4),SUBSTRING(@Columna,@p7+1,@p8-(@p7+1)))                
Set @Costo=Convert(decimal(18,4),SUBSTRING(@Columna,@p8+1,@p9-(@p8+1)))
Set @AplicaINV=SUBSTRING(@Columna,@p9+1,@p10-(@p9+1))               
                         
insert into DetalleDocumento                 
values(@DocuId,@IdProducto,@Cantidad,@Precio,@Importe,              
@DetalleNotaId,@UM,@ValorUM,@Descripcion)              

if(@AplicaINV='S')
BEGIN               
set @StockInicial=(select top 1 ProductoCantidad from Producto(nolock)                 
where IdProducto=@IdProducto)              
               
set @CantidadIng=(@Cantidad*@ValorUM)         
set @StockFinal=@StockInicial+@CantidadIng                
              
update Producto                
set ProductoCantidad=ProductoCantidad+@CantidadIng                
where IdProducto=@IdProducto              
                 
insert into Kardex                
values(@IdProducto,GETDATE(),'Ingreso por N-Credito',@KardexDocu,@StockInicial,                
@CantidadIng,0,@Costo,@StockFinal,'INGRESO',@DocuUsuario)
End             
                
Fetch Next From Tabla INTO @Columna                
end                
 Close Tabla;                
 Deallocate Tabla;                
 delete from CajaDetalle                
 where NotaId=@NotaId                
 --Commit Transaction;                
 --SELECT 'true'    
if(len(@Guia)>0)      
begin      
Declare TablaB Cursor For Select * From fnSplitString(@Guia,';')       
Open TablaB      
Declare @ColumnaB varchar(max)    
Declare @g1 int,@g2 int,    
        @g3 int,@g4 int,@g5 int    
    
Declare @CantidadA decimal(18,2),     
        @IdProductoU numeric(20),                     
        @CantidadU decimal(18,2),                        
        @UmU varchar(40),                                                       
        @ValorUMU decimal(18,4)    
    
Declare @IniciaStockB decimal(18,2),    
        @StockFinalB decimal(18,2)    
              
Fetch Next From TablaB INTO @ColumnaB      
 While @@FETCH_STATUS = 0      
 Begin      
Set @g1 = CharIndex('|',@ColumnaB,0)                       
Set @g2 = CharIndex('|',@ColumnaB,@g1+1)                        
Set @g3 = CharIndex('|',@ColumnaB,@g2+1)                        
Set @g4 = CharIndex('|',@ColumnaB,@g3+1)                        
Set @g5=Len(@ColumnaB)+1       
     
set @CantidadA=Convert(decimal(18,2),SUBSTRING(@ColumnaB,1,@g1-1))    
Set @IdProductoU=Convert(numeric(20),SUBSTRING(@ColumnaB,@g1+1,@g2-(@g1+1)))    
Set @CantidadU=Convert(decimal(18,2),SUBSTRING(@ColumnaB,@g2+1,@g3-(@g2+1)))      
Set @UmU=SUBSTRING(@ColumnaB,@g3+1,@g4-(@g3+1))      
Set @ValorUMU=Convert(decimal(18,4),SUBSTRING(@ColumnaB,@g4+1,@g5-(@g4+1)))          
    
 Declare @CantidadSalB decimal(18,2)     
    
 set @CantidadSalB=(@CantidadA * @CantidadU)* @ValorUMU                
                    
 set @IniciaStockB=(select top 1 p.ProductoCantidad     
 from Producto p where p.IdProducto=@IdProductoU)                        
     
 set @StockFinalB=@IniciaStockB + @CantidadSalB                       
                 
 insert into Kardex values(@IdProductoU,GETDATE(),'Ingreso por N-Credito',@KardexDocu,@IniciaStockB,              
 @CantidadSalB,0,0,@StockFinalB,'INGRESO',@DocuUsuario)                          
                   
 update producto                         
 set  ProductoCantidad =ProductoCantidad + @CantidadSalB                       
 where IDProducto=@IdProductoU        
    
Fetch Next From TablaB INTO @ColumnaB      
end      
    Close TablaB;      
    Deallocate TablaB;      
    Commit Transaction;      
    select 'true'    
end      
else      
begin      
    Commit Transaction;      
    select 'true'    
end   
                
end
GO
/****** Object:  StoredProcedure [dbo].[uspinsertarNotaB]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[uspinsertarNotaB]
@ListaOrden varchar(Max)
AS
BEGIN
Declare @pos1 int,@pos2 int,@pos3 int
Declare @orden varchar(max),
        @detalle varchar(max),
        @Guia varchar(max)
Set @pos1 = CharIndex('[',@ListaOrden,0)
Set @pos2 = CharIndex('[',@ListaOrden,@pos1+1)
Set @pos3 = Len(@ListaOrden)+1
Set @orden = SUBSTRING(@ListaOrden,1,@pos1-1)
Set @detalle = SUBSTRING(@ListaOrden,@pos1+1,@pos2-@pos1-1)
Set @Guia = SUBSTRING(@ListaOrden,@pos2+1,@pos3-@pos2-1)

Declare @c1 int,@c2 int,@c3 int,@c4 int,
        @c5 int,@c6 int,@c7 int,@c8 int,
        @c9 int,@c10 int,@c11 int,@c12 int,
        @c13 int,@c14 int,@c15 int,@c16 int,
        @c17 int,@c18 int,@c19 int,@c20 int,
        @c21 int,@c22 int,@c23 int,@c24 int,
        @c25 int,@c26 int,@c27 int,@c28 int,
        @c29 int,@c30 int,@c31 int,@c32 int,
        @c33 int,@c34 int,@c35 int,@c36 int,
        @c37 int,@c38 int,@c39 int,@c40 int,
        @c41 int,@c42 int

Declare
  @NotaDocu varchar(60),@ClienteId numeric(20),
  @NotaUsuario varchar(60),@NotaFormaPago varchar(60),
  @NotaCondicion varchar(60),@NotaDireccion varchar(max),
  @NotaTelefono varchar(60),@NotaSubtotal decimal(18,2),
  @NotaMovilidad decimal(18,2),@NotaDescuento decimal(18,2),
  @NotaTotal decimal(18,2),@NotaAcuenta decimal(18,2),
  @NotaSaldo decimal(18,2),@NotaAdicional decimal(18,2),
  @NotaTarjeta decimal(18,2),@NotaPagar decimal(18,2),
  @NotaEstado varchar(60),@CompaniaId int,
  @NotaEntrega varchar(40),@NotaConcepto varchar(60),
  @Serie varchar(20),@Numero varchar(60),
  @NotaGanancia decimal(18,2),@Letra varchar(max),
  @DocuAdicional decimal(18,2),@DocuHash varchar(250),
  @EstadoSunat varchar(80),@DocuSubtotal decimal(18,2),
  @DocuIGV decimal(18,2),@UsuarioId int,@ICBPER decimal(18,2),
  @DocuGravada decimal(18,2),@DocuDescuento decimal(18,2),
  @CajaId varchar(38),@Movimiento varchar(40),@KARDEX varchar(1),
  @NotaIdBR varchar(38),@EntidadBancaria varchar(80),
  @NroOperacion varchar(80),@Efectivo decimal(18,2),
  @Deposito decimal(18,2),@ClienteRazon varchar(140),
  @ClienteRuc varchar(40),@ClienteDni varchar(40),
  @DireccionFiscal varchar(max)

Set @c1 = CharIndex('|',@orden,0)
Set @c2 = CharIndex('|',@orden,@c1+1)
Set @c3 = CharIndex('|',@orden,@c2+1)
Set @c4 = CharIndex('|',@orden,@c3+1)
Set @c5 = CharIndex('|',@orden,@c4+1)
Set @c6 = CharIndex('|',@orden,@c5+1)
Set @c7 = CharIndex('|',@orden,@c6+1)
Set @c8 = CharIndex('|',@orden,@c7+1)
Set @c9 = CharIndex('|',@orden,@c8+1)
Set @c10 = CharIndex('|',@orden,@c9+1)
Set @c11 = CharIndex('|',@orden,@c10+1)
Set @c12 = CharIndex('|',@orden,@c11+1)
Set @c13 = CharIndex('|',@orden,@c12+1)
Set @c14 = CharIndex('|',@orden,@c13+1)
Set @c15 = CharIndex('|',@orden,@c14+1)
Set @c16 = CharIndex('|',@orden,@c15+1)
Set @c17 = CharIndex('|',@orden,@c16+1)
Set @c18 = CharIndex('|',@orden,@c17+1)
Set @c19 = CharIndex('|',@orden,@c18+1)
Set @c20 = CharIndex('|',@orden,@c19+1)
Set @c21 = CharIndex('|',@orden,@c20+1)
Set @c22 = CharIndex('|',@orden,@c21+1)
Set @c23 = CharIndex('|',@orden,@c22+1)
Set @c24 = CharIndex('|',@orden,@c23+1)
Set @c25 = CharIndex('|',@orden,@c24+1)
Set @c26 = CharIndex('|',@orden,@c25+1)
Set @c27 = CharIndex('|',@orden,@c26+1)
Set @c28 = CharIndex('|',@orden,@c27+1)
Set @c29 = CharIndex('|',@orden,@c28+1)
Set @c30 = CharIndex('|',@orden,@c29+1)
Set @c31 = CharIndex('|',@orden,@c30+1)
Set @c32 = CharIndex('|',@orden,@c31+1)
Set @c33 = CharIndex('|',@orden,@c32+1)
Set @c34 = CharIndex('|',@orden,@c33+1)
Set @c35 = CharIndex('|',@orden,@c34+1)
Set @c36 = CharIndex('|',@orden,@c35+1)
Set @c37 = CharIndex('|',@orden,@c36+1)
Set @c38 = CharIndex('|',@orden,@c37+1)
Set @c39 = CharIndex('|',@orden,@c38+1)
Set @c40 = CharIndex('|',@orden,@c39+1)
Set @c41 = CharIndex('|',@orden,@c40+1)
Set @c42 = Len(@orden)+1

set @NotaDocu = SUBSTRING(@orden,1,@c1-1)
set @ClienteId = convert(numeric(20),SUBSTRING(@orden,@c1+1,@c2-@c1-1))
set @NotaUsuario = SUBSTRING(@orden,@c2+1,@c3-@c2-1)
set @NotaFormaPago = SUBSTRING(@orden,@c3+1,@c4-@c3-1)
set @NotaCondicion = SUBSTRING(@orden,@c4+1,@c5-@c4-1)
set @NotaDireccion = SUBSTRING(@orden,@c5+1,@c6-@c5-1)
set @NotaTelefono = SUBSTRING(@orden,@c6+1,@c7-@c6-1)
set @NotaSubtotal = convert(decimal(18,2),SUBSTRING(@orden,@c7+1,@c8-@c7-1))
set @NotaMovilidad = convert(decimal(18,2),SUBSTRING(@orden,@c8+1,@c9-@c8-1))
set @NotaDescuento = convert(decimal(18,2),SUBSTRING(@orden,@c9+1,@c10-@c9-1))
set @NotaTotal = convert(decimal(18,2),SUBSTRING(@orden,@c10+1,@c11-@c10-1))
set @NotaAcuenta = convert(decimal(18,2),SUBSTRING(@orden,@c11+1,@c12-@c11-1))
set @NotaSaldo = convert(decimal(18,2),SUBSTRING(@orden,@c12+1,@c13-@c12-1))
set @NotaAdicional = convert(decimal(18,2),SUBSTRING(@orden,@c13+1,@c14-@c13-1))
set @NotaTarjeta = convert(decimal(18,2),SUBSTRING(@orden,@c14+1,@c15-@c14-1))
set @NotaPagar = convert(decimal(18,2),SUBSTRING(@orden,@c15+1,@c16-@c15-1))
set @NotaEstado = SUBSTRING(@orden,@c16+1,@c17-@c16-1)
set @CompaniaId = convert(int,SUBSTRING(@orden,@c17+1,@c18-@c17-1))
set @NotaEntrega = SUBSTRING(@orden,@c18+1,@c19-@c18-1)
set @NotaConcepto = SUBSTRING(@orden,@c19+1,@c20-@c19-1)
set @Serie = SUBSTRING(@orden,@c20+1,@c21-@c20-1)
set @Numero = SUBSTRING(@orden,@c21+1,@c22-@c21-1)
set @NotaGanancia = convert(decimal(18,2),SUBSTRING(@orden,@c22+1,@c23-@c22-1))
set @Letra = SUBSTRING(@orden,@c23+1,@c24-@c23-1)
set @DocuAdicional = convert(decimal(18,2),SUBSTRING(@orden,@c24+1,@c25-@c24-1))
set @DocuHash = SUBSTRING(@orden,@c25+1,@c26-@c25-1)
set @EstadoSunat = SUBSTRING(@orden,@c26+1,@c27-@c26-1)
set @DocuSubtotal = convert(decimal(18,2),SUBSTRING(@orden,@c27+1,@c28-@c27-1))
set @DocuIGV = convert(decimal(18,2),SUBSTRING(@orden,@c28+1,@c29-@c28-1))
set @UsuarioId = convert(int,SUBSTRING(@orden,@c29+1,@c30-@c29-1))
set @ICBPER = convert(decimal(18,2),SUBSTRING(@orden,@c30+1,@c31-@c30-1))
set @DocuGravada = convert(decimal(18,2),SUBSTRING(@orden,@c31+1,@c32-@c31-1))
set @DocuDescuento = convert(decimal(18,2),SUBSTRING(@orden,@c32+1,@c33-@c32-1))
set @NotaIdBR = SUBSTRING(@orden,@c33+1,@c34-@c33-1)
set @EntidadBancaria = SUBSTRING(@orden,@c34+1,@c35-@c34-1)
set @NroOperacion = SUBSTRING(@orden,@c35+1,@c36-@c35-1)
set @Efectivo = convert(decimal(18,2),SUBSTRING(@orden,@c36+1,@c37-@c36-1))
set @Deposito = convert(decimal(18,2),SUBSTRING(@orden,@c37+1,@c38-@c37-1))
set @ClienteRazon = SUBSTRING(@orden,@c38+1,@c39-@c38-1)
set @ClienteRuc = SUBSTRING(@orden,@c39+1,@c40-@c39-1)
set @ClienteDni = SUBSTRING(@orden,@c40+1,@c41-@c40-1)
set @DireccionFiscal = SUBSTRING(@orden,@c41+1,@c42-@c41-1)

if(@NotaIdBR='') set @NotaIdBR = 0

IF EXISTS(
    select top 1 n.NotaId
    from NotaPedido n
    where n.NotaId = @NotaIdBR
)
begin
    select 'EXISTE'
end
else
begin
    if(@NotaDocu='FACTURA')
    begin
        -- FACTURA debe comportarse igual que BOLETA:
        -- - Si es CREDITO => EMITIDO con saldo
        -- - Si es ALCONTADO => CANCELADO con saldo 0
        if(@NotaCondicion='CREDITO')
        begin
            set @NotaEstado='EMITIDO'
            set @NotaSaldo=@NotaPagar
            set @NotaAcuenta=0
        end
        else
        begin
            set @NotaEstado='CANCELADO'
            set @NotaSaldo=0
            set @NotaAcuenta=@NotaPagar
        end
    end
    else if(@NotaDocu='PROFORMA')
        set @NotaEstado='PENDIENTE'
    else
    begin
        if(@NotaCondicion='CREDITO')
        begin
            set @NotaEstado='EMITIDO'
            set @NotaSaldo=@NotaPagar
            set @NotaAcuenta=0
        end
        else
        begin
            set @NotaEstado='CANCELADO'
            set @NotaSaldo=0
            set @NotaAcuenta=@NotaPagar
        end
    end

    Declare @pZ1 int = 0

    if(@NotaFormaPago='YAPE/DEPOSITO' or @NotaFormaPago='TARJETA/DEPOSITO')
    begin
        set @Movimiento='DEPOSITO'
    end
    else
    begin
        Declare @pZ2 int
        Declare @FormaA varchar(max),
                @FormaB varchar(max),
                @MovimientoB varchar(40)

        Set @pZ1 = CharIndex('/',@NotaFormaPago,0)

        if(@pZ1>0)
        begin
            Set @pZ2 = Len(@NotaFormaPago)+1
            Set @FormaA = SUBSTRING(@NotaFormaPago,1,@pZ1-1)
            Set @FormaB = SUBSTRING(@NotaFormaPago,@pZ1+1,@pZ2-@pZ1-1)

            if(@FormaA='EFECTIVO') set @Movimiento='INGRESO'
            else if(@FormaA='DEPOSITO') set @Movimiento='DEPOSITO'
            else if(@FormaA='YAPE' OR @FormaA='PLIN') set @Movimiento='DEPOSITO'
            else set @Movimiento='TARJETA'

            if(@FormaB='EFECTIVO') set @MovimientoB='INGRESO'
            else if(@FormaB='DEPOSITO') set @MovimientoB='DEPOSITO'
            else if(@FormaB='YAPE' OR @FormaB='PLIN') set @MovimientoB='DEPOSITO'
            else set @MovimientoB='TARJETA'
        end
        else
        begin
            if(@NotaFormaPago='EFECTIVO') set @Movimiento='INGRESO'
            else if(@NotaFormaPago='DEPOSITO') set @Movimiento='DEPOSITO'
            else if(@NotaFormaPago='TRANSFERENCIA') set @Movimiento='DEPOSITO'
            else if(@NotaFormaPago='YAPE' OR @NotaFormaPago='PLIN') set @Movimiento='DEPOSITO'
            else set @Movimiento='TARJETA'
        end
    end

    declare @NotaId numeric(38),
            @DocuId numeric(38) = 0

    Begin Transaction

    update Cliente
    set ClienteDespacho=@NotaDireccion,ClienteTelefono=@NotaTelefono
    where ClienteId=@ClienteId

    delete from TemporalVenta
    where UsuarioID=@UsuarioId

    declare @cod varchar(13)

    SET @cod = ISNULL((
        select top 1 dbo.genenerarNroFactura(@Serie,@CompaniaId,@NotaDocu) AS ID
        FROM DocumentoVenta
    ),'00000001')

    insert into NotaPedido (
        NotaDocu,
        ClienteId,
        NotaFecha,
        NotaUsuario,
        NotaFormaPago,
        NotaCondicion,
        NotaFechaPago,
        NotaDireccion,
        NotaTelefono,
        NotaSubtotal,
        NotaMovilidad,
        NotaDescuento,
        NotaTotal,
        NotaAcuenta,
        NotaSaldo,
        NotaAdicional,
        NotaTarjeta,
        NotaPagar,
        NotaEstado,
        CompaniaId,
        NotaEntrega,
        ModificadoPor,
        FechaEdita,
        NotaConcepto,
        NotaSerie,
        NotaNumero,
        NotaGanancia,
        ICBPER,
        CajaId,
        EntidadBancaria,
        NroOperacion,
        Efectivo,
        Deposito
    )
    VALUES (
        @NotaDocu,
        @ClienteId,
        GETDATE(),
        @NotaUsuario,
        @NotaFormaPago,
        @NotaCondicion,
        GETDATE(),
        @NotaDireccion,
        @NotaTelefono,
        @NotaSubtotal,
        @NotaMovilidad,
        @NotaDescuento,
        @NotaTotal,
        @NotaAcuenta,
        @NotaSaldo,
        @NotaAdicional,
        @NotaTarjeta,
        @NotaPagar,
        @NotaEstado,
        @CompaniaId,
        @NotaEntrega,
        '',
        NULL,
        @NotaConcepto,
        @Serie,
        @cod,
        @NotaGanancia,
        @ICBPER,
        @CajaId,
        @EntidadBancaria,
        @NroOperacion,
        @Efectivo,
        @Deposito
    )

    set @NotaId = (select @@IDENTITY)

    if (@NotaDocu='PROFORMA V')
    begin
        insert into DocumentoVenta values
        (@CompaniaId,@NotaId,'PROFORMA V',@cod,@ClienteId,GETDATE(),
         GETDATE(),@NotaCondicion,@Letra,@DocuSubtotal,
         @DocuIGV,@NotaPagar,0,@NotaUsuario,'EMITIDO',@Serie,'00',@NotaMovilidad,'','VENTA','',
         @DocuHash,'ENVIADO',
         @ICBPER,'','',@DocuGravada,@DocuDescuento,'',@NotaFormaPago,@EntidadBancaria,
         @NroOperacion,@Efectivo,@Deposito,@ClienteRazon,@ClienteRuc,@ClienteDni,@DireccionFiscal)

        set @DocuId = (select @@IDENTITY)

        if(@NotaCondicion<>'CREDITO')
        begin
            if(@pZ1>0)
            begin
                set @KARDEX='S'
            end
            else
            begin
                set @KARDEX='S'
            end
        end
    end
    else if(@NotaDocu='FACTURA')
    begin
        insert into DocumentoVenta values
        (@CompaniaId,@NotaId,'FACTURA',@cod,@ClienteId,GETDATE(),
         GETDATE(),@NotaCondicion,@Letra,@DocuSubtotal,
         @DocuIGV,@NotaPagar,0,@NotaUsuario,'EMITIDO',@Serie,'01',@DocuAdicional,'','VENTA','',
         @DocuHash,@EstadoSunat,
         @ICBPER,'','',@DocuGravada,@DocuDescuento,'',
         @NotaFormaPago,@EntidadBancaria,@NroOperacion,@Efectivo,@Deposito,
         @ClienteRazon,@ClienteRuc,@ClienteDni,@DireccionFiscal)

        set @DocuId = (select @@IDENTITY)

        if(@NotaConcepto='MERCADERIA')
        begin
            if(@NotaCondicion<>'CREDITO')
            begin
                if(@pZ1>0)
                begin
                    set @KARDEX='S'
                end
                else
                begin
                    set @KARDEX='S'
                end
            end
        end
    end
    else if(@NotaDocu='BOLETA')
    begin
        insert into DocumentoVenta values
        (@CompaniaId,@NotaId,'BOLETA',@cod,@ClienteId,GETDATE(),
         GETDATE(),@NotaCondicion,@Letra,@DocuSubtotal,
         @DocuIGV,@NotaPagar,0,@NotaUsuario,'EMITIDO',@Serie,'03',@NotaMovilidad,'','VENTA','',
         @DocuHash,@EstadoSunat,
         @ICBPER,'','',@DocuGravada,@DocuDescuento,'',
         @NotaFormaPago,@EntidadBancaria,@NroOperacion,@Efectivo,@Deposito,
         @ClienteRazon,@ClienteRuc,@ClienteDni,@DireccionFiscal)

        set @DocuId = (select @@IDENTITY)

        if(@NotaConcepto='MERCADERIA')
        begin
            if(@NotaCondicion<>'CREDITO')
            begin
                if(@pZ1>0)
                begin
                    set @KARDEX='S'
                end
                else
                begin
                    set @KARDEX='S'
                end
            end
        end
    end

    Declare Tabla Cursor For Select * From fnSplitString(@detalle,';')
    Open Tabla

    Declare @Columna varchar(max),
      @IdProducto numeric(20),
      @CodigoPro varchar(200),
      @DetalleCantidad decimal(18,2),
      @DetalleUm varchar(40),
      @Descripcion varchar(140),
      @DetalleCosto decimal(18,4),
      @DetallePrecio decimal(18,2),
      @DetalleImporte decimal(18,2),
      @DetalleEstado varchar(60),
      @AplicaINV nvarchar(1),
      @ValorUM decimal(18,4),@CantidadSaldo decimal(18,2),
      @IniciaStock decimal(18,2),@StockFinal decimal(18,2)

    Declare @p1 int,@p2 int,@p3 int,@p4 int,
            @p5 int,@p6 int,@p7 int,@p8 int,
            @p9 int,@p10 int,@p11 int

    Fetch Next From Tabla INTO @Columna
    While @@FETCH_STATUS = 0
    Begin
        Set @p1 = CharIndex('|',@Columna,0)
        Set @p2 = CharIndex('|',@Columna,@p1+1)
        Set @p3 = CharIndex('|',@Columna,@p2+1)
        Set @p4 = CharIndex('|',@Columna,@p3+1)
        Set @p5 = CharIndex('|',@Columna,@p4+1)
        Set @p6 = CharIndex('|',@Columna,@p5+1)
        Set @p7 = CharIndex('|',@Columna,@p6+1)
        Set @p8 = CharIndex('|',@Columna,@p7+1)
        Set @p9 = CharIndex('|',@Columna,@p8+1)
        Set @p10 = CharIndex('|',@Columna,@p9+1)
        Set @p11 = Len(@Columna)+1

        set @IdProducto = Convert(numeric(20),SUBSTRING(@Columna,1,@p1-1))
        Set @CodigoPro = SUBSTRING(@Columna,@p1+1,@p2-(@p1+1))
        Set @DetalleCantidad = convert(decimal(18,2),SUBSTRING(@Columna,@p2+1,@p3-(@p2+1)))
        Set @DetalleUm = SUBSTRING(@Columna,@p3+1,@p4-(@p3+1))
        Set @Descripcion = SUBSTRING(@Columna,@p4+1,@p5-(@p4+1))
        Set @DetalleCosto = convert(decimal(18,4),SUBSTRING(@Columna,@p5+1,@p6-(@p5+1)))
        Set @DetallePrecio = convert(decimal(18,2),SUBSTRING(@Columna,@p6+1,@p7-(@p6+1)))
        Set @DetalleImporte = convert(decimal(18,2),SUBSTRING(@Columna,@p7+1,@p8-(@p7+1)))
        Set @DetalleEstado = SUBSTRING(@Columna,@p8+1,@p9-(@p8+1))
        set @ValorUM = convert(decimal(18,4),SUBSTRING(@Columna,@p9+1,@p10-(@p9+1)))
        set @AplicaINV = SUBSTRING(@Columna,@p10+1,@p11-(@p10+1))

        Declare @CantidadSal decimal(18,2)

        if(@NotaEntrega='INMEDIATA')
            Set @CantidadSaldo=0
        else
            Set @CantidadSaldo=@DetalleCantidad

        insert into DetallePedido values(@NotaId,@IdProducto,@DetalleCantidad,
        @DetalleUm,@Descripcion,@DetalleCosto,@DetallePrecio,
        @DetalleImporte,@DetalleEstado,@CantidadSaldo,@ValorUM)

        if(@DocuId<>0)
        begin
            insert into DetalleDocumento values
            (@DocuId,@IdProducto,@DetalleCantidad,@DetallePrecio,@DetalleImporte,
             @NotaId,@DetalleUm,@ValorUM,@Descripcion)
        end

        if(@KARDEX='S')
        BEGIN
            if(@AplicaINV='S')
            BEGIN
                set @CantidadSal = @DetalleCantidad * @ValorUM
                set @IniciaStock = (select top 1 ProductoCantidad from Producto where IdProducto=@IdProducto)
                set @StockFinal = @IniciaStock-@CantidadSal

                insert into Kardex values(@IdProducto,GETDATE(),'Salida por Venta',@Serie+'-'+@cod,@IniciaStock,
                 0,@CantidadSal,@DetalleCosto,@StockFinal,'SALIDA',@NotaUsuario)

                update producto
                set ProductoCantidad = ProductoCantidad - @CantidadSal
                where IDProducto=@IdProducto
            End
        END

        Fetch Next From Tabla INTO @Columna
    end

    Close Tabla;
    Deallocate Tabla;

    if(len(@Guia)>0 AND @NotaEstado<>'PENDIENTE')
    begin
        Declare TablaB Cursor For Select * From fnSplitString(@Guia,';')
        Open TablaB

        Declare @ColumnaB varchar(max)
        Declare @g1 int,@g2 int,@g3 int,@g4 int,@g5 int

        Declare @CantidadA decimal(18,2),
                @IdProductoU numeric(20),
                @CantidadU decimal(18,2),
                @Um varchar(40),
                @ValorUMU decimal(18,4)

        Declare @IniciaStockB decimal(18,2),
                @StockFinalB decimal(18,2)

        Fetch Next From TablaB INTO @ColumnaB
        While @@FETCH_STATUS = 0
        Begin
            Set @g1 = CharIndex('|',@ColumnaB,0)
            Set @g2 = CharIndex('|',@ColumnaB,@g1+1)
            Set @g3 = CharIndex('|',@ColumnaB,@g2+1)
            Set @g4 = CharIndex('|',@ColumnaB,@g3+1)
            Set @g5 = Len(@ColumnaB)+1

            set @CantidadA = Convert(decimal(18,2),SUBSTRING(@ColumnaB,1,@g1-1))
            Set @IdProductoU = Convert(numeric(20),SUBSTRING(@ColumnaB,@g1+1,@g2-(@g1+1)))
            Set @CantidadU = Convert(decimal(18,2),SUBSTRING(@ColumnaB,@g2+1,@g3-(@g2+1)))
            Set @Um = SUBSTRING(@ColumnaB,@g3+1,@g4-(@g3+1))
            Set @ValorUMU = Convert(decimal(18,4),SUBSTRING(@ColumnaB,@g4+1,@g5-(@g4+1)))

            Declare @CantidadSalB decimal(18,2)

            set @CantidadSalB = (@CantidadA * @CantidadU) * @ValorUMU
            set @IniciaStockB = (select top 1 p.ProductoCantidad from Producto p where p.IdProducto=@IdProductoU)
            set @StockFinalB = @IniciaStockB-@CantidadSalB

            insert into Kardex values(@IdProductoU,GETDATE(),'Salida por Venta',@Serie+'-'+@cod,@IniciaStockB,
             0,@CantidadSalB,0,@StockFinalB,'SALIDA',@NotaUsuario)

            update producto
            set ProductoCantidad = ProductoCantidad - @CantidadSalB
            where IDProducto=@IdProductoU

            Fetch Next From TablaB INTO @ColumnaB
        end

        Close TablaB;
        Deallocate TablaB;
        Commit Transaction;
        select convert(varchar,@NotaId) + CHAR(172) + @cod
    end
    else
    begin
        Commit Transaction;
        select convert(varchar,@NotaId) + CHAR(172) + @cod
    end
END
END
GO
/****** Object:  StoredProcedure [dbo].[uspInsertarProducto]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspInsertarProducto]
@Data varchar(max)
as
Begin

Declare @Id int, 
        @ProveedorRazon varchar(250),
        @ProveedorRuc varchar (20),
        @ProveedorContacto varchar(140),
        @ProveedorCelular varchar(140),
        @ProveedorTelefono varchar(140),
        @ProveedorCorreo varchar(140),
        @ProveedorDireccion varchar(250),
        @ProveedorEstado varchar(20)


 Declare @pos1 int, @pos2 int,@pos3 int,  
		 @pos4 int, @pos5 int,@pos6 int,
		 @pos7 int, @pos8 int,@pos9 int

Set @Data= LTRIM(RTrim(@Data))  
Set @pos1=CharIndex('|',@Data,0)  
Set @pos2=CharIndex('|',@Data,@pos1+1)  
Set @pos3=CharIndex('|',@Data,@pos2+1)  
Set @pos4=CharIndex('|',@Data,@pos3+1)  
Set @pos5=CharIndex('|',@Data,@pos4+1)
Set @pos6=CharIndex('|',@Data,@pos5+1)  
Set @pos7=CharIndex('|',@Data,@pos6+1)  
Set @pos8=CharIndex('|',@Data,@pos7+1)  
Set @pos9=Len(@Data)+1 


Set @Id=convert(int,SUBSTRING(@Data,1,@pos1-1))  
Set @ProveedorRazon=SUBSTRING(@Data,@pos1+1,@pos2-@pos1-1)  
Set @ProveedorRuc=SUBSTRING(@Data,@pos2+1,@pos3-@pos2-1)  
Set @ProveedorContacto=SUBSTRING(@Data,@pos3+1,@pos4-@pos3-1)  
Set @ProveedorCelular=SUBSTRING(@Data,@pos4+1,@pos5-@pos4-1)  
Set @ProveedorTelefono=SUBSTRING(@Data,@pos5+1,@pos6-@pos5-1)
Set @ProveedorCorreo=SUBSTRING(@Data,@pos6+1,@pos7-@pos6-1)
Set @ProveedorDireccion=SUBSTRING(@Data,@pos7+1,@pos8-@pos7-1)  
Set @ProveedorEstado=SUBSTRING(@Data,@pos8+1,@pos9-@pos8-1)  

if(@Id=0)  
 Begin

 IF EXISTS(select top 1  p.ProveedorRuc 
			 from Proveedor p where p.ProveedorRuc=@ProveedorRuc and ProveedorRuc<>'')   
	 begin  
	   select 'existe RUC'   
	 end
 Else
 Begin
 
 INSERT INTO Proveedor (ProveedorRazon,ProveedorRuc,ProveedorContacto,
    ProveedorCelular,ProveedorTelefono,ProveedorCorreo,
    ProveedorDireccion,ProveedorEstado) 
  VALUES (@ProveedorRazon,@ProveedorRuc,@ProveedorContacto,
    @ProveedorCelular,@ProveedorTelefono,@ProveedorCorreo,
    @ProveedorDireccion,@ProveedorEstado)

 select 'true'

 End

End
Else
Begin
 IF EXISTS(select top 1  p.ProveedorRuc 
			 from Proveedor p where p.ProveedorRuc=@ProveedorRuc and (ProveedorRuc<>'' and ProveedorId<>@Id))   
	 begin  
	   select 'existe RUC'   
	 end
 Else
 Begin

	  UPDATE Proveedor SET
		ProveedorRazon = @ProveedorRazon,
		ProveedorRuc = @ProveedorRuc,
		ProveedorContacto = @ProveedorContacto,
		ProveedorCelular = @ProveedorCelular,
		ProveedorTelefono = @ProveedorTelefono,
		ProveedorCorreo = @ProveedorCorreo,
		ProveedorDireccion = @ProveedorDireccion,
		ProveedorEstado = @ProveedorEstado
	 WHERE ProveedorId = @Id

   select 'select true'   

 End

End
End
GO
/****** Object:  StoredProcedure [dbo].[uspInsertarProveedor]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspInsertarProveedor] 
@Data varchar(max)  
as  
Begin  
  
Declare @Id int,   
        @ProveedorRazon varchar(250),  
        @ProveedorRuc varchar (20),  
        @ProveedorContacto varchar(140),  
        @ProveedorCelular varchar(140),  
        @ProveedorTelefono varchar(140),  
        @ProveedorCorreo varchar(140),  
        @ProveedorDireccion varchar(250),  
        @ProveedorEstado varchar(20)  
  
  
 Declare @pos1 int, @pos2 int,@pos3 int,    
   @pos4 int, @pos5 int,@pos6 int,  
   @pos7 int, @pos8 int,@pos9 int  
  
Set @Data= LTRIM(RTrim(@Data))    
Set @pos1=CharIndex('|',@Data,0)    
Set @pos2=CharIndex('|',@Data,@pos1+1)    
Set @pos3=CharIndex('|',@Data,@pos2+1)    
Set @pos4=CharIndex('|',@Data,@pos3+1)    
Set @pos5=CharIndex('|',@Data,@pos4+1)  
Set @pos6=CharIndex('|',@Data,@pos5+1)    
Set @pos7=CharIndex('|',@Data,@pos6+1)    
Set @pos8=CharIndex('|',@Data,@pos7+1)    
Set @pos9=Len(@Data)+1   
  
  
Set @Id=convert(int,SUBSTRING(@Data,1,@pos1-1))    
Set @ProveedorRazon=SUBSTRING(@Data,@pos1+1,@pos2-@pos1-1)    
Set @ProveedorRuc=SUBSTRING(@Data,@pos2+1,@pos3-@pos2-1)    
Set @ProveedorContacto=SUBSTRING(@Data,@pos3+1,@pos4-@pos3-1)    
Set @ProveedorCelular=SUBSTRING(@Data,@pos4+1,@pos5-@pos4-1)    
Set @ProveedorTelefono=SUBSTRING(@Data,@pos5+1,@pos6-@pos5-1)  
Set @ProveedorCorreo=SUBSTRING(@Data,@pos6+1,@pos7-@pos6-1)  
Set @ProveedorDireccion=SUBSTRING(@Data,@pos7+1,@pos8-@pos7-1)    
Set @ProveedorEstado=SUBSTRING(@Data,@pos8+1,@pos9-@pos8-1)    
  
if(@Id=0)    
 Begin  
  
 IF EXISTS(select top 1  p.ProveedorRuc   
    from Proveedor p where p.ProveedorRuc=@ProveedorRuc and p.ProveedorRuc<>'')     
  begin    
    select 'existe RUC'     
  end  
 Else  
 Begin  
   
 INSERT INTO Proveedor (ProveedorRazon,ProveedorRuc,ProveedorContacto,  
    ProveedorCelular,ProveedorTelefono,ProveedorCorreo,  
    ProveedorDireccion,ProveedorEstado)   
  VALUES (@ProveedorRazon,@ProveedorRuc,@ProveedorContacto,  
    @ProveedorCelular,@ProveedorTelefono,@ProveedorCorreo,  
    @ProveedorDireccion,@ProveedorEstado)  
  
 select 'true'  
  
 End  
  
End  
Else  
Begin  
 IF EXISTS(select top 1  p.ProveedorRuc   
    from Proveedor p where p.ProveedorRuc=@ProveedorRuc and (p.ProveedorRuc<>'' and p.ProveedorId<>@Id))     
  begin    
    select 'existe RUC'     
  end  
 Else  
 Begin  
  
   UPDATE Proveedor SET  
  ProveedorRazon = @ProveedorRazon,  
  ProveedorRuc = @ProveedorRuc,  
  ProveedorContacto = @ProveedorContacto,  
  ProveedorCelular = @ProveedorCelular,  
  ProveedorTelefono = @ProveedorTelefono,  
  ProveedorCorreo = @ProveedorCorreo,  
  ProveedorDireccion = @ProveedorDireccion,  
  ProveedorEstado = @ProveedorEstado  
  WHERE ProveedorId = @Id  
  
   select 'select true'     
  
 End  
  
End  
End
GO
/****** Object:  StoredProcedure [dbo].[uspinsertarPuntos]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspinsertarPuntos]
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
        @pos17 int,@pos18 int,@pos19 int,@pos20 int,
        @pos21 int,@pos22 int
        
 Declare @IdPuntos numeric(38),@CentroSV nvarchar(60),
@NotaId numeric(38),@FechaEmision date,@FechaRegistro datetime,
@Condicion varchar(60),@Documento varchar(60),@Numero varchar(60),
@CodigoDXN varchar(60),@Cliente varchar(300),@RUC varchar(20),
@DNI varchar(20),@CodigoRes varchar(60),@Responsable varchar(300),
@Usuario varchar(80),@TotalPV decimal(18,2),@Subtotal decimal(18,2),
@Descuento decimal(18,2),@OperacionGV decimal(18,2),@IGV decimal(18,2),
@ICBPER decimal(18,2),@Total decimal(18,2)
 
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
Set @pos20= CharIndex('|',@orden,@pos19+1)
Set @pos21= CharIndex('|',@orden,@pos20+1)
Set @pos22= Len(@orden)+1

Set @IdPuntos=convert(numeric(38),SUBSTRING(@orden,1,@pos1-1))
Set @CentroSV=SUBSTRING(@orden,@pos1+1,@pos2-@pos1-1)
Set @NotaId=convert(numeric(38),SUBSTRING(@orden,@pos2+1,@pos3-@pos2-1))
Set @FechaEmision=convert(date,SUBSTRING(@orden,@pos3+1,@pos4-@pos3-1))
Set @FechaRegistro=convert(datetime,SUBSTRING(@orden,@pos4+1,@pos5-@pos4-1))
Set @Condicion=SUBSTRING(@orden,@pos5+1,@pos6-@pos5-1)
Set @Documento=SUBSTRING(@orden,@pos6+1,@pos7-@pos6-1)
Set @Numero=SUBSTRING(@orden,@pos7+1,@pos8-@pos7-1)
Set @CodigoDXN=SUBSTRING(@orden,@pos8+1,@pos9-@pos8-1)
Set @Cliente=SUBSTRING(@orden,@pos9+1,@pos10-@pos9-1)
Set @RUC=SUBSTRING(@orden,@pos10+1,@pos11-@pos10-1)
Set @DNI=SUBSTRING(@orden,@pos11+1,@pos12-@pos11-1)
set @CodigoRes=SUBSTRING(@orden,@pos12+1,@pos13-@pos12-1)
set @Responsable=SUBSTRING(@orden,@pos13+1,@pos14-@pos13-1)
set @Usuario=SUBSTRING(@orden,@pos14+1,@pos15-@pos14-1)
set @TotalPV=convert(decimal(18,2),SUBSTRING(@orden,@pos15+1,@pos16-@pos15-1))
set @Subtotal=convert(decimal(18,2),SUBSTRING(@orden,@pos16+1,@pos17-@pos16-1))
set @Descuento=convert(decimal(18,2),SUBSTRING(@orden,@pos17+1,@pos18-@pos17-1))
set @OperacionGV=convert(decimal(18,2),SUBSTRING(@orden,@pos18+1,@pos19-@pos18-1))
set @IGV=convert(decimal(18,2),SUBSTRING(@orden,@pos19+1,@pos20-@pos19-1))
set @ICBPER=convert(decimal(18,2),SUBSTRING(@orden,@pos20+1,@pos21-@pos20-1))
set @Total=convert(decimal(18,2),SUBSTRING(@orden,@pos21+1,@pos22-@pos21-1))

Begin Transaction

insert into PasarPuntos values(@CentroSV,@NotaId,convert(date,GETDATE()),GETDATE(),
@Condicion,@Documento,@Numero,@CodigoDXN,@Cliente,@RUC,
@DNI,@CodigoRes,@Responsable,@Usuario,@TotalPV,@Subtotal,
@Descuento,@OperacionGV,@IGV,@ICBPER,@Total,'','','PENDIENTE')
Set @IdPuntos=@@identity

Declare Tabla Cursor For Select * From fnSplitString(@detalle,';')	
Open Tabla
Declare @Columna varchar(max),
		@IdProducto numeric(20),
		@Codigo varchar(80),
		@Cantidad decimal(18,2),
		@Unidad varchar(40),
		@Descripcion varchar(300),
		@PrecioUni decimal(18,2),
		@PV decimal(18,2),
		@SV decimal(18,2),
		@Importe decimal(18,2)
		
Declare @p1 int,@p2 int,@p3 int,@p4 int,
        @p5 int,@p6 int,@p7 int,@p8 int,@p9 int
        
Fetch Next From Tabla INTO @Columna
	While @@FETCH_STATUS = 0
	Begin
Set @p1 = CharIndex('|',@Columna,0)
Set @p2 = CharIndex('|',@Columna,@p1+1)
Set @p3 = CharIndex('|',@Columna,@p2+1)
Set @p4 = CharIndex('|',@Columna,@p3+1)
Set @p5 = CharIndex('|',@Columna,@p4+1)
Set @p6= CharIndex('|',@Columna,@p5+1)
Set @p7= CharIndex('|',@Columna,@p6+1)
Set @p8= CharIndex('|',@Columna,@p7+1)
Set @p9= Len(@Columna)+1

Set @IdProducto=Convert(numeric(20),SUBSTRING(@Columna,1,@p1-1))
Set @Codigo=SUBSTRING(@Columna,@p1+1,@p2-(@p1+1))
Set @Cantidad=Convert(decimal(18,2),SUBSTRING(@Columna,@p2+1,@p3-(@p2+1)))
Set @Unidad=SUBSTRING(@Columna,@p3+1,@p4-(@p3+1))
Set @Descripcion=SUBSTRING(@Columna,@p4+1,@p5-(@p4+1))
Set @PrecioUni=Convert(decimal(18,2),SUBSTRING(@Columna,@p5+1,@p6-(@p5+1)))
Set @PV=Convert(decimal(18,2),SUBSTRING(@Columna,@p6+1,@p7-(@p6+1)))
Set @SV=Convert(decimal(18,2),SUBSTRING(@Columna,@p7+1,@p8-(@p7+1)))
Set @Importe=Convert(decimal(18,2),SUBSTRING(@Columna,@p8+1,@p9-(@p8+1)))

insert into DetallePuntos values(@IdPuntos,@IdProducto,@Codigo,@Cantidad,
@Unidad,@Descripcion,@PrecioUni,@PV,@SV,@Importe)

Fetch Next From Tabla INTO @Columna
end
	Close Tabla;
	Deallocate Tabla;
	Commit Transaction;
select 'true'
end
GO
/****** Object:  StoredProcedure [dbo].[uspinsertarRB]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[uspinsertarRB]
    @ListaOrden varchar(Max)
AS
BEGIN
    DECLARE @pos int
    DECLARE @orden varchar(max)
    DECLARE @detalle varchar(max)

    SET @pos = CharIndex('[', @ListaOrden, 0)
    SET @orden = SUBSTRING(@ListaOrden, 1, @pos - 1)
    SET @detalle = SUBSTRING(@ListaOrden, @pos + 1, len(@ListaOrden) - @pos)

    DECLARE @c1 int, @c2 int, @c3 int, @c4 int,
            @c5 int, @c6 int, @c7 int, @c8 int,
            @c9 int, @c10 int, @c11 int, @c12 int,
            @c13 int, @c14 INT

    DECLARE @CompaniaId int, @ResumenSerie varchar(250),
            @Secuencia numeric(38), @FechaReferencia date,
            @SubTotal decimal(18,2), @IGV decimal(18,2),
            @Total decimal(18,2), @ResumenTiket varchar(250),
            @CodigoSunat varchar(80), @HASHCDR varchar(max),
            @Usuario varchar(80), @Status int, @Estado char(1),
            @RangoNumero varchar(80), @ICBPER decimal(18,2)

    SET @c1 = CharIndex('|', @orden, 0)
    SET @c2 = CharIndex('|', @orden, @c1 + 1)
    SET @c3 = CharIndex('|', @orden, @c2 + 1)
    SET @c4 = CharIndex('|', @orden, @c3 + 1)
    SET @c5 = CharIndex('|', @orden, @c4 + 1)
    SET @c6 = CharIndex('|', @orden, @c5 + 1)
    SET @c7 = CharIndex('|', @orden, @c6 + 1)
    SET @c8 = CharIndex('|', @orden, @c7 + 1)
    SET @c9 = CharIndex('|', @orden, @c8 + 1)
    SET @c10 = CharIndex('|', @orden, @c9 + 1)
    SET @c11 = CharIndex('|', @orden, @c10 + 1)
    SET @c12 = CharIndex('|', @orden, @c11 + 1)
    SET @c13 = CharIndex('|', @orden, @c12 + 1)
    SET @c14 = Len(@orden) + 1

    SET @CompaniaId = CONVERT(int, SUBSTRING(@orden, 1, @c1 - 1))
    SET @ResumenSerie = SUBSTRING(@orden, @c1 + 1, @c2 - @c1 - 1)
    SET @Secuencia = CONVERT(int, SUBSTRING(@orden, @c2 + 1, @c3 - @c2 - 1))
    SET @FechaReferencia = CONVERT(date, SUBSTRING(@orden, @c3 + 1, @c4 - @c3 - 1))
    SET @SubTotal = CONVERT(decimal(18,2), SUBSTRING(@orden, @c4 + 1, @c5 - @c4 - 1))
    SET @IGV = CONVERT(decimal(18,2), SUBSTRING(@orden, @c5 + 1, @c6 - @c5 - 1))
    SET @Total = CONVERT(decimal(18,2), SUBSTRING(@orden, @c6 + 1, @c7 - @c6 - 1))
    SET @ResumenTiket = SUBSTRING(@orden, @c7 + 1, @c8 - @c7 - 1)
    SET @CodigoSunat = SUBSTRING(@orden, @c8 + 1, @c9 - @c8 - 1)
    SET @HASHCDR = SUBSTRING(@orden, @c9 + 1, @c10 - @c9 - 1)
    SET @Usuario = SUBSTRING(@orden, @c10 + 1, @c11 - @c10 - 1)
    SET @Status = SUBSTRING(@orden, @c11 + 1, @c12 - @c11 - 1)
    SET @RangoNumero = SUBSTRING(@orden, @c12 + 1, @c13 - @c12 - 1)
    SET @ICBPER = SUBSTRING(@orden, @c13 + 1, @c14 - @c13 - 1)

    IF (@Status = 3)
    BEGIN
        SET @SubTotal = 0 - @SubTotal
        SET @IGV = 0 - @IGV
        SET @ICBPER = 0 - @ICBPER
        SET @Total = 0 - @Total
        SET @Estado = 'B' -- BAJA
    END
    ELSE
    BEGIN
        SET @Estado = 'E' -- ENVIADO
    END

    BEGIN TRANSACTION

    INSERT INTO dbo.ResumenBoletas
    (
        CompaniaId, ResumenSerie, Secuencia, FechaReferencia, FechaEnvio,
        SubTotal, IGV, Total, ResumenTiket, CodigoSunat, HASHCDR, MensajeSunat,
        Usuario, ESTADO, RangoNumero, ICBPER, CDRBase64
    )
    VALUES
    (
        @CompaniaId, @ResumenSerie, @Secuencia, @FechaReferencia, GETDATE(),
        @SubTotal, @IGV, @Total, @ResumenTiket, @CodigoSunat, @HASHCDR, '',
        @Usuario, @Estado, @RangoNumero, @ICBPER, ''
    )

    DECLARE Tabla CURSOR FOR
        SELECT * FROM fnSplitString(@detalle, ';')

    OPEN Tabla

    DECLARE @Columna varchar(max),
            @DocuId numeric(38)
    DECLARE @p1 int

    FETCH NEXT FROM Tabla INTO @Columna
    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @p1 = Len(@Columna) + 1
        SET @DocuId = CONVERT(numeric(38), SUBSTRING(@Columna, 1, @p1 - 1))

        IF (@Status = 1) -- Declarar (3 = Anular)
        BEGIN
            UPDATE DocumentoVenta
            SET DocuHash = @HASHCDR, EstadoSunat = 'ENVIADO'
            WHERE DocuId = @DocuId
        END
        ELSE
        BEGIN
            UPDATE DocumentoVenta
            SET DocuHash = @HASHCDR,
                DocuEstado = 'BAJA',
                EstadoSunat = 'ENVIADO',
                DocuSubTotal = 0,
                DocuIgv = 0,
                DocuTotal = 0,
                ICBPER = 0
            WHERE DocuId = @DocuId
        END

        FETCH NEXT FROM Tabla INTO @Columna
    END

    CLOSE Tabla
    DEALLOCATE Tabla

    COMMIT TRANSACTION

    SELECT
        ISNULL((
            SELECT STUFF((
                SELECT '¬' + CONVERT(varchar, r.ResumenId) + '|' + CONVERT(varchar, r.CompaniaId) + '|' +
                       (ISNULL(CONVERT(varchar, r.FechaReferencia, 103), '')) + '|' +
                       (ISNULL(CONVERT(varchar, r.FechaEnvio, 103), '') + ' ' + ISNULL(SUBSTRING(CONVERT(varchar, r.FechaEnvio, 114), 1, 8), '')) + '|' +
                       r.ResumenSerie + '-' + CONVERT(varchar, r.Secuencia) + '|' + r.RangoNumero + '|' +
                       CONVERT(varchar(50), CAST(r.SubTotal as money), 1) + '|' +
                       CONVERT(varchar(50), CAST(r.IGV as money), 1) + '|' +
                       CONVERT(varchar(50), CAST(r.ICBPER as money), 1) + '|' +
                       CONVERT(varchar(50), CAST(r.Total as money), 1) + '|' +
                       r.ResumenTiket + '|' + r.CodigoSunat + '|' + r.HASHCDR + '|' + r.MensajeSunat + '|' +
                       r.Usuario + '|' + c.CompaniaRUC + '|' +
                       c.CompaniaUserSecun + '|' + c.ComapaniaPWD + '|' + r.Estado + '||' + c.TokenApi + '|' + ClienIdToken
                FROM ResumenBoletas r
                INNER JOIN Compania c ON c.CompaniaId = r.CompaniaId
                WHERE Month(r.FechaReferencia) = MONTH(GETDATE())
                  AND YEAR(r.FechaReferencia) = YEAR(GETDATE())
                ORDER BY r.CompaniaId, r.FechaEnvio ASC
                FOR XML PATH('')
            ), 1, 1, '')
        ), '~')
END
GO
/****** Object:  StoredProcedure [dbo].[uspInsertarUsuario]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[uspInsertarUsuario]  
@Data VARCHAR(MAX)  
AS  
BEGIN  
    SET NOCOUNT ON;  
  
    DECLARE  
        @UsuarioID INT,  
        @PersonalId INT,  
        @UsuarioAlias VARCHAR(80),  
        @UsuarioClave VARCHAR(50),  
        @UsuarioEstado VARCHAR(20)  
  
    DECLARE  
        @pos1 INT, @pos2 INT, @pos3 INT, @pos4 INT, @pos5 INT  
  
    SET @Data = LTRIM(RTRIM(@Data))  
  
    SET @pos1 = CHARINDEX('|', @Data)  
    SET @pos2 = CHARINDEX('|', @Data, @pos1 + 1)  
    SET @pos3 = CHARINDEX('|', @Data, @pos2 + 1)  
    SET @pos4 = CHARINDEX('|', @Data, @pos3 + 1)  
    SET @pos5 = LEN(@Data) + 1  
  
    SET @UsuarioID     = CONVERT(INT, SUBSTRING(@Data, 1, @pos1 - 1))  
    SET @PersonalId    = CONVERT(INT, SUBSTRING(@Data, @pos1 + 1, @pos2 - @pos1 - 1))  
    SET @UsuarioAlias  = SUBSTRING(@Data, @pos2 + 1, @pos3 - @pos2 - 1)  
    SET @UsuarioClave  = SUBSTRING(@Data, @pos3 + 1, @pos4 - @pos3 - 1)  
    SET @UsuarioEstado = SUBSTRING(@Data, @pos4 + 1, @pos5 - @pos4 - 1)  
  
  
    IF (@UsuarioID = 0)  
    BEGIN  
        IF EXISTS (  
            SELECT 1   
            FROM Usuarios   
            WHERE UsuarioAlias = @UsuarioAlias  
        )  
        BEGIN  
            SELECT 'EXISTE_USUARIO'  
            RETURN  
        END  
  
       INSERT INTO Usuarios  
(  
    PersonalId,  
    UsuarioAlias,  
    UsuarioClave,  
    UsuarioEstado,  
    UsuarioFechaReg,
    FechaVencimientoClave
)  
VALUES  
(  
    @PersonalId,  
    @UsuarioAlias,  
    dbo.encriptar(@UsuarioClave),  
    @UsuarioEstado,  
    GETDATE(),
    DATEADD(MONTH,6,GETDATE())
)
  
        SELECT SCOPE_IDENTITY() AS UsuarioID  
        RETURN  
    END  
  
    ELSE  
    BEGIN  
  
      IF EXISTS (  
            SELECT 1   
            FROM Usuarios   
            WHERE UsuarioAlias = @UsuarioAlias and UsuarioID<>@UsuarioID  
        )  
        BEGIN  
  
            SELECT 'EXISTE_USUARIO'  
            RETURN  
          
  END  
       UPDATE Usuarios  
SET  
    UsuarioAlias = @UsuarioAlias,  
    UsuarioClave = CASE   
                      WHEN @UsuarioClave <> ''   
                      THEN dbo.encriptar(@UsuarioClave)  
                      ELSE UsuarioClave  
                   END,
    FechaVencimientoClave = CASE
                               WHEN @UsuarioClave <> ''
                               THEN DATEADD(MONTH,6,GETDATE())
                               ELSE FechaVencimientoClave
                            END,
    UsuarioEstado = @UsuarioEstado  
WHERE UsuarioID = @UsuarioID  
AND PersonalId = @PersonalId
  
        SELECT 'UPDATED'  
    END  
END  
GO
/****** Object:  StoredProcedure [dbo].[uspInsertaUnion]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspInsertaUnion]  
@Data varchar(max)  
as  
begin  
Declare @pos1 int,@pos2 int,  
        @pos3 int,@pos4 int, 
		@pos5 int,@pos6 int, 
		@pos7 int
Declare @Id int,
        @IdProducto numeric(20),
        @IdProductoB numeric(20),
		@Cantidad decimal(18,2),
		@UM varchar(80),
		@Precio decimal(18,2),
		@ValorUM decimal(18,4)

Set @Data = LTRIM(RTrim(@Data))  
Set @pos1 = CharIndex('|',@Data,0)
Set @pos2 = CharIndex('|',@Data,@pos1+1)
Set @pos3 = CharIndex('|',@Data,@pos2+1)
Set @pos4 = CharIndex('|',@Data,@pos3+1)
Set @pos5 = CharIndex('|',@Data,@pos4+1)
Set @pos6 = CharIndex('|',@Data,@pos5+1)
Set @pos7 = Len(@Data)+1    

Set @Id =convert(int,SUBSTRING(@Data,1,@pos1-1))  
Set @IdProducto=convert(numeric,SUBSTRING(@Data,@pos1+1,@pos2-@pos1-1))
Set @IdProductoB=convert(numeric,SUBSTRING(@Data,@pos2+1,@pos3-@pos2-1)) 
Set @Cantidad=convert(decimal(18,2),SUBSTRING(@Data,@pos3+1,@pos4-@pos3-1))  
Set @UM=SUBSTRING(@Data,@pos4+1,@pos5-@pos4-1)
Set @Precio=convert(decimal(18,2),SUBSTRING(@Data,@pos5+1,@pos6-@pos5-1))  
Set @ValorUM=convert(decimal(18,4),SUBSTRING(@Data,@pos6+1,@pos7-@pos6-1))
    
if (@Id=0) 
begin

insert into ProductoUnion values(@IdProducto,@IdProductoB,@Cantidad,@UM,@Precio,@ValorUM,'P')
select 'true'

end 
else  
begin  

update ProductoUnion 
set Cantidad=@Cantidad,Estado='P' 
where Id=@Id  
select 'true'  

end  
end
GO
/****** Object:  StoredProcedure [dbo].[uspListaActivosB]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[uspListaActivosB]
as
begin
select
isnull((select STUFF ((select '¬'+p.Nombres+'_'+a.Nombres+'_'+d.Descripcion
from Desbloqueo d
inner join AdministraBL a
on a.IdAdmin=d.IdAdmin
inner join PersonalBL p
on p.IdPersonal=d.IdPersonal
where convert(date,Fecha)=CONVERT(date,DATEADD(HOUR, 1,GETDATE()))
order by d.IdDetalle desc
for xml path('')),1,1,'')),'~')
end
GO
/****** Object:  StoredProcedure [dbo].[uspListaBajas]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[uspListaBajas]      
@Data varchar(max)      
as      
begin      
Declare @p1 int      
Declare @CompaniaId int      
Set @Data = LTRIM(RTrim(@Data))      
set @CompaniaId=@Data      
select         
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
/****** Object:  StoredProcedure [dbo].[uspListaClientes]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[uspListaClientes]    
as    
begin    
select    
isnull((select STUFF ((select '¬'+c.Nombre+'_'+c.Codigo+'_'+    
CONVERT(VarChar(max), cast(c.Monto as money ), 1)+'_'+  
--isnull(convert(varchar,c.DiasPlazo),'')+'_'+  
convert(varchar,c.ClienteId)    
from ClienteCredito c    
order by c.ClienteId desc    
for xml path('')),1,1,'')),'~')    
end 
GO
/****** Object:  StoredProcedure [dbo].[uspListaDetaServicioCom]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspListaDetaServicioCom]
@CompraId varchar(20)
as
select
'TemId|UsuarioId|Descripcion|UM|Cantidad|PrecioCosto|Descuento|Importe|Estado¬100|100|100|100|100|100|100|100|100¬String|String|String|String|String|String|String|String|String¬'+  
isnull((select stuff((select '¬'+convert(varchar,d.DetalleId)+'|'+
convert(varchar,d.CompraId)+'|'+
d.Descripcion+'|'+
DetalleUM+'|'+
convert(varchar,d.DetalleCantidad)+'|'+
convert(varchar,d.PrecioCosto)+'|'+
convert(varchar,d.DetalleDescuento)+'|'+
convert(varchar,d.DetalleImporte)+'|'+
d.DetalleEstado
from DetalleCompra d 
where d.CompraId=@CompraId 
order by d.CompraId asc
FOR XML path ('')),1,1,'')),'~')
GO
/****** Object:  StoredProcedure [dbo].[uspListaDocumentos]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[uspListaDocumentos]
    @Data varchar(max)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CompaniaId int;
    DECLARE @fechaReferencia date;
    DECLARE @detalle varchar(max);

    SET @Data = LTRIM(RTRIM(@Data));
    SET @CompaniaId = TRY_CONVERT(int, @Data);

    IF @CompaniaId IS NULL OR @CompaniaId <= 0
    BEGIN
        SELECT '~' AS Resultado;
        RETURN;
    END;

    -- Fecha de referencia: primera fecha pendiente (anterior a hoy)
    SELECT TOP (1)
        @fechaReferencia = d.DocuEmision
    FROM DocumentoVenta d
    WHERE d.TipoCodigo = '03'
      AND d.CompaniaId = @CompaniaId
      AND d.EstadoSunat = 'PENDIENTE'
      AND d.DocuEmision <= CONVERT(date, GETDATE())
    ORDER BY d.DocuEmision DESC

    IF @fechaReferencia IS NULL
    BEGIN
        SELECT '~' AS Resultado;
        RETURN;
    END;

    -- Detalle del lote de la fecha de referencia
    SELECT
        @detalle = STUFF((
            SELECT TOP (450)
                '¬'
                + CONVERT(varchar(20), ISNULL(d.DocuId, 0)) + '|'
                + CONVERT(varchar(20), ISNULL(d.CompaniaId, 0)) + '|'
                + CONVERT(varchar(20), ISNULL(d.NotaId, 0)) + '|'
                + CONVERT(char(10), d.DocuEmision, 103) + '|'
                + ISNULL(d.DocuDocumento, '') + '|'
                + ISNULL(d.DocuSerie, '') + '-' + ISNULL(CONVERT(varchar(20), d.DocuNumero), '') + '|'
                + ISNULL(c.ClienteRazon, '') + '|'
                + ISNULL(NULLIF(c.ClienteDni, ''), '00000000') + '|'
                + ISNULL(CONVERT(varchar(50), CAST(d.DocuSubTotal AS money), -1), '0.00') + '|'
                + ISNULL(CONVERT(varchar(50), CAST(d.DocuIgv AS money), -1), '0.00') + '|'
                + ISNULL(CONVERT(varchar(50), CAST(d.ICBPER AS money), -1), '0.00') + '|'
                + ISNULL(CONVERT(varchar(50), CAST(d.DocuTotal AS money), -1), '0.00') + '|'
                + ISNULL(d.DocuUsuario, '') + '|'
                + ISNULL(d.EstadoSunat, '')
            FROM DocumentoVenta d
            INNER JOIN Cliente c ON c.ClienteId = d.ClienteId
            WHERE d.TipoCodigo = '03'
              AND d.CompaniaId = @CompaniaId
              AND d.EstadoSunat = 'PENDIENTE'
              AND d.DocuEmision = @fechaReferencia
            ORDER BY d.DocuSerie, d.DocuNumero
            FOR XML PATH(''), TYPE
        ).value('.', 'varchar(max)'), 1, 1, '');

    SELECT
        CONVERT(varchar(10), @fechaReferencia, 103)
        + '§'
        + ISNULL(NULLIF(@detalle, ''), '~') AS Resultado;
END
GO
/****** Object:  StoredProcedure [dbo].[uspListaFacturaPendiente]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[uspListaFacturaPendiente]        
as        
begin        
select         
'DocuID|NotaId|FechaEmision|Documento|Numero|Cliente|RUC|Descuento|SubTotal|IGV|ICBPER|Total|Usuario|Compania|Movilidad|Adicional|TipoCodigo|Serie|Nro|Forma|Condicion¬90|100|100|100|130|350|100|100|110|110|90|110|150|90|90|90|90|90|90|90|90¬String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String¬'+        
isnull((select STUFF ((select '¬'+convert(varchar,d.DocuId)+'|'+    
convert(varchar,d.NotaId)+'|'+        
Convert(char(10),d.DocuEmision,103)+'|'+    
d.DocuDocumento+'|'+        
d.DocuSerie+'-'+d.DocuNumero+'|'+    
cl.ClienteRazon+'|'+    
cl.ClienteRuc+'|'+        
CONVERT(VarChar(50), cast(n.NotaDescuento as money ), 1)+'|'+        
CONVERT(VarChar(50), cast(d.DocuSubTotal as money ), 1)+'|'+        
CONVERT(VarChar(50), cast(d.DocuIgv as money ), 1)+'|'+        
CONVERT(VarChar(50), cast(d.ICBPER as money ), 1)+'|'+        
CONVERT(VarChar(50), cast(d.DocuTotal as money ), 1)+'|'+    
d.DocuUsuario+'|'+    
c.CompaniaRazonSocial+'|'+        
CONVERT(VarChar(50), cast(n.NotaMovilidad as money ), 1)+'|'+        
CONVERT(VarChar(50), cast(n.NotaAdicional as money ), 1)+'|'+        
LTRIM(RTrim(d.TipoCodigo))+    
'|'+d.DocuSerie+'|'+    
d.DocuNumero+'|'+        
n.NotaFormaPago+'|'+    
n.NotaCondicion        
from DocumentoVenta d        
inner join NotaPedido n        
on n.NotaId=d.NotaId        
inner join Cliente cl        
on cl.ClienteId=d.ClienteId        
inner join Compania c        
on c.CompaniaId=d.CompaniaId        
--where d.EstadoSunat='PENDIENTE' and (d.TipoCodigo='01' or d.TipoCodigo='07' or TipoCodigo='03')
where d.EstadoSunat='PENDIENTE' and (d.TipoCodigo='01' or d.TipoCodigo='07') 
order by d.CompaniaId,d.DocuEmision asc        
for xml path('')),1,1,'')),'~')        
end 
GO
/****** Object:  StoredProcedure [dbo].[uspListaGuiaInterna]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspListaGuiaInterna]              
@Id nvarchar(1),              
@fechainicio date,                
@fechafin date               
as              
begin              
select              
'Id|NroGuia|FechaRegistro|Motivo|Origen|RazonSocial|Observacion|Total|Usuario|Estado|ClienteId|Numero¬90|90|90|90|90|90|90|90|90|90|90|90¬String|String|String|String|String|String|String|String|String|String|String|String¬'+                
isnull((select STUFF ((select '¬'+convert(varchar,g.GuiaId)+'|'+        
g.Serie+'-'+g.Numero+'|'+              
(IsNull(convert(varchar,g.FechaRegistro,103),'')+' '+ IsNull(SUBSTRING(convert(varchar,g.FechaRegistro,114),1,8),''))+'|'+                
g.Motivo+'|'+g.Origen+'|'+g.Destino+'|'+g.Observacion+'|'+        
CONVERT(VarChar(50), cast(g.Total as money ), 1)+'|'+        
g.Usuario+'|'+g.Estado+'|'+g.ClienteId+'|'+g.Numero             
from GuiaInternaSI g              
where g.Concepto=@Id and (Convert(char(10),g.FechaRegistro,101) BETWEEN @fechainicio AND @fechafin)                
order by g.GuiaId desc              
for xml path('')),1,1,'')),'~')        
end
GO
/****** Object:  StoredProcedure [dbo].[uspListaPersonalED]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspListaPersonalED]
as
begin
select
isnull((select STUFF ((select '¬'+p.Nombres+'_'+
'HT: '+ IsNull(SUBSTRING(convert(varchar,a.HoraFin,114),1,8),'')+'_'+
'ATZ: '+SUBSTRING(c.Nombres, 1, CHARINDEX(' ',c.Nombres, 1) - 1)+'_'+
convert(varchar,a.IdAuto)+'_'+
IsNull(convert(varchar,a.HoraFin,103),'')+' ; '+ 
IsNull(SUBSTRING(convert(varchar,a.HoraFin,114),1,8),'')+'_'+
CONVERT(varchar,a.Tiempo)+'_'+
a.Observaciones
from AutorizaEdicion a
inner join PersonalBL p
on p.IdPersonal=a.IdPersonal
inner join AdministraBL c
on c.IdAdmin=a.IdAdmin
where convert(date,a.FechaRegistro)=convert(date,getdate())
order by a.IdAuto desc
for xml path('')),1,1,'')),'~')
end
GO
/****** Object:  StoredProcedure [dbo].[uspListarArchivos]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[uspListarArchivos]
@Data varchar(max)
as
begin
Declare @p1 int,@p2 int
Declare @fechainicio date,
        @fechafin date
Set @Data = LTRIM(RTrim(@Data))
Set @p1 = CharIndex('|',@Data,0)
Set @p2= Len(@Data)+1
Set @fechainicio=convert(date,SUBSTRING(@Data,1,@p1-1))
Set @fechafin=convert(date,SUBSTRING(@Data,@p1+1,@p2-@p1-1))
SELECT
'ID|FechaHora|Descripcion|Importe|Encargado|Ruta¬90|140|400|110|155|90¬String|String|String|String|String|String¬'+
isnull((select STUFF((select '¬'+convert(varchar,d.DetalleId)+'|'+
(IsNull(convert(varchar,d.DetalleFecha,103),'')+' '+ IsNull(SUBSTRING(convert(varchar,d.DetalleFecha,114),1,8),''))+'|'+
d.DetalleConcepto+'|'+CONVERT(varChar(max),cast(d.DetalleMonto as money ), 1)+'|'+
c.CajaEncargado+'|'+d.RutaImagen
from CajaDetalle d
inner join Caja c
on c.CajaId=d.CajaId
where (Convert(char(10),d.DetalleFecha,101) BETWEEN @fechainicio AND @fechafin)and(d.NotaId=0 and d.DetalleMovimiento='SALIDA')
order by d.DetalleId desc
FOR XML PATH('')), 1, 1, '')),'~')+'['+
'ID|FechaHora|Descripcion|Importe|Encargado|Ruta¬90|140|400|110|155|90¬String|String|String|String|String|String¬'+
isnull((select STUFF((select '¬'+convert(varchar,d.DetalleId)+'|'+
(IsNull(convert(varchar,d.DetalleFecha,103),'')+' '+ IsNull(SUBSTRING(convert(varchar,d.DetalleFecha,114),1,8),''))+'|'+
d.DetalleConcepto+'|'+CONVERT(varChar(max),cast(d.DetalleMonto as money ), 1)+'|'+
c.CajaEncargado+'|'+d.RutaImagen
from CajaDetalle d
inner join Caja c
on c.CajaId=d.CajaId
where (Convert(char(10),d.DetalleFecha,101) BETWEEN @fechainicio AND @fechafin) and(d.NotaId=0 and d.DetalleMovimiento='INGRESO' and d.Vista='')
order by d.DetalleId desc
FOR XML PATH('')), 1, 1, '')),'~')
end
GO
/****** Object:  StoredProcedure [dbo].[uspListarClientes]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[uspListarClientes]
    @Estado varchar(20) = 'ACTIVO' 
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ClienteId,
        ClienteRazon,
        ClienteRuc,
        ClienteDni,
        ClienteDireccion,
        ClienteTelefono,
        ClienteCorreo,
        ClienteEstado,
        ClienteDespacho,
        ClienteUsuario,
        ISNULL(CONVERT(varchar, ClienteFecha, 103), '') + ' ' +
        ISNULL(SUBSTRING(CONVERT(varchar, ClienteFecha, 114), 1, 8), '') AS ClienteFecha
    FROM Cliente WITH (NOLOCK)
    WHERE (@Estado IS NULL) OR (ClienteEstado = @Estado)
    ORDER BY ClienteId DESC;
END
GO
/****** Object:  StoredProcedure [dbo].[usplistarDetaCaja]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[usplistarDetaCaja]  
@CajaId varchar(38)  
as  
begin  
select  
'Id|Fecha|Descripcion|Importe|Ruta|GastoId|Referencia|LQID¬80|150|420|115|100|100|100|90¬String|String|String|String|String|String|String|String¬'+  
isnull((select stuff((select '¬'+convert(varchar,d.DetalleId)+'|'+  
(IsNull(convert(varchar,d.DetalleFecha,103),'')+' '+ IsNull(SUBSTRING(convert(varchar,d.DetalleFecha,114),1,8),''))+'|'+  
d.DetalleConcepto+'|'+  
CONVERT(VarChar(50), cast(d.DetalleMonto as money ), 1)+'|'+d.RutaImagen+'|'+d.GastoId+'|'+  
d.DetalleReferencia+'|'+d.LiquidaId  
from CajaDetalle d  
where d.CajaId=@CajaId and (d.NotaId=0 and d.DetalleMovimiento='INGRESO' and d.Vista='')  
order by d.DetalleId desc  
for xml path('')),1,1,'')),'~')+'['+  
'Id|Fecha|Descripcion|Importe|Ruta|GastoId|Referencia|LQID¬80|150|420|115|100|100|100|90¬String|String|String|String|String|String|String|String¬'+  
isnull((select stuff((select '¬'+convert(varchar,d.DetalleId)+'|'+  
(IsNull(convert(varchar,d.DetalleFecha,103),'')+' '+ IsNull(SUBSTRING(convert(varchar,d.DetalleFecha,114),1,8),''))+'|'+  
d.DetalleConcepto+'|'+  
CONVERT(VarChar(50), cast(d.DetalleMonto as money ), 1)+'|'+d.RutaImagen+'|'+d.GastoId+'|'+  
d.DetalleReferencia+'|'+d.LiquidaId  
from CajaDetalle d  
where d.CajaId=@CajaId and (d.NotaId=0 and d.DetalleMovimiento='SALIDA' and d.Vista='')  
order by d.DetalleId desc  
for xml path('')),1,1,'')),'~')  
end
GO
/****** Object:  StoredProcedure [dbo].[usplistaResumen]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[usplistaResumen]  
@Data varchar(max)  
as  
begin  
Declare @p1 int,@p2 int  
Declare @MES INT,@ANNO INT  
Set @Data = LTRIM(RTrim(@Data))  
Set @p1 = CharIndex('|',@Data,0)  
Set @p2= Len(@Data)+1  
Set @MES=convert(int,SUBSTRING(@Data,1,@p1-1))  
Set @ANNO=convert(int,SUBSTRING(@Data,@p1+1,@p2-@p1-1))  
SELECT  
'Id|Compania|FechaEmision|FechaEnvio|Serie|RangoNumeros|SubTotal|IGV|ICBPER|Total|Ticket|CDSunat|HASHCDR|Mensaje|Usuario|RUC|UserSol|ClaveSol|ESTADO|Intentos|TokenApi|IdToken¬100|100|100|100|100|100|100|100|110|110|110|100|100|100|100|100|100|100|100|100|100|100¬String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String¬'+   
isnull((select STUFF ((select '¬'+convert(varchar,r.ResumenId)+'|'+convert(varchar,r.CompaniaId)+'|'+  
(IsNull(convert(varchar,r.FechaReferencia,103),''))+'|'+  
(IsNull(convert(varchar,r.FechaEnvio,103),'')+' '+ IsNull(SUBSTRING(convert(varchar,r.FechaEnvio,114),1,8),''))+'|'+  
r.ResumenSerie+'-'+convert(varchar,r.Secuencia)+'|'+r.RangoNumero+'|'+  
CONVERT(VarChar(50),cast(r.SubTotal as money ), 1)+'|'+  
CONVERT(VarChar(50),cast( r.IGV as money ), 1)+'|'+  
CONVERT(VarChar(50),cast( r.ICBPER as money ), 1)+'|'+  
CONVERT(VarChar(50),cast(r.Total as money ), 1)+'|'+  
r.ResumenTiket+'|'+r.CodigoSunat+'|'+r.HASHCDR+'|'+r.MensajeSunat+'|'+  
r.Usuario+'|'+c.CompaniaRUC+'|'+  
c.CompaniaUserSecun+'|'+c.ComapaniaPWD+'|'+r.Estado+'||'+c.TokenApi+'|'+ClienIdToken  
FROM ResumenBoletas r  
inner join Compania c  
on c.CompaniaId=r.CompaniaId  
where Month(r.FechaReferencia)=@MES and YEAR(r.FechaReferencia)=@ANNO  
order by r.CompaniaId,r.FechaEnvio asc  
for xml path('')),1,1,'')),'~')  
end
GO
/****** Object:  StoredProcedure [dbo].[uspListarFeriados]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[uspListarFeriados]
AS
BEGIN
    SELECT IdFeriado, Fecha, Motivo
    FROM Feriados
    ORDER BY IdFeriado desc;
END
GO
/****** Object:  StoredProcedure [dbo].[uspListarMaquinas]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspListarMaquinas]
as
Begin
SELECT IdMaquina, Maquina,
convert(varchar,Registro,103)+' '+SUBSTRING(convert(varchar,Registro,114),1,8) as Registro,
SerieFactura,SerieNC, SerieBoleta, Tiketera
FROM MAQUINAS 
order by 1 asc
End
GO
/****** Object:  StoredProcedure [dbo].[usplistarNC]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[usplistarNC]
as
begin
select
'DocuId|Compania|NroNota|FechaEmision|Documento|Numero|RazonSocial|RUC|Referencia|Nro|Serie|SubTotal|IGV|ICBPER|Total|Usuario|Estado|Direccion|Asociado|CompaniaRazon|CompaniaRUC|Concepto|Gravada|Descuento|Adcional¬100|80|100|110|115|120|340|105|120|100|100|115|115|90|115|150|130|100|100|100|100|220|100|100|100¬String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String¬'+
isnull((select STUFF ((select '¬'+convert(varchar,d.DocuId)+'|'+convert(varchar,d.CompaniaId)+'|'+
convert(varchar,d.NotaId)+'|'+(Convert(char(10),d.DocuEmision,103))+'|'+
d.DocuDocumento+'|'+d.docuSerie+'-'+d.DocuNumero+'|'+c.ClienteRazon+'|'+c.ClienteRuc+'|'+
d.DocuNroGuia+'|'+d.DocuNumero+'|'+d.DocuSerie+'|'+
(convert(varchar(50), CAST(d.DocuSubTotal as money),1))+'|'+
(convert(varchar(50), CAST(d.DocuIgv as money),1))+'|'+
(convert(varchar(50), CAST(d.ICBPER as money),1))+'|'+
(convert(varchar(50), CAST(d.DocuTotal as money),1))+'|'+
d.DocuUsuario+'|'+d.DocuEstado+'|'+c.ClienteDireccion+'|'+d.DocuAsociado+'|'+
co.CompaniaRazonSocial+'|'+co.CompaniaRUC+'|'+d.DocuConcepto+'|'+
(convert(varchar(50), CAST(d.DocuGravada as money),1))+'|'+
(convert(varchar(50), CAST(d.DocuDescuento as money),1))+'|'+
(convert(varchar(50), CAST(d.DocuAdicional as money),1))
from DocumentoVenta d
inner join Cliente c
on c.ClienteId=d.ClienteId
inner join Compania co
on co.CompaniaId=d.CompaniaId
where d.TipoCodigo='07'and (Month(d.DocuEmision)=Month(GETDATE())and year(d.DocuEmision)=YEAR(Getdate()))
order by d.DocuId desc
for xml path('')),1,1,'')),'~')
end
GO
/****** Object:  StoredProcedure [dbo].[usplistarNCFecha]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[usplistarNCFecha]  
@fechainicio date,  
@fechafin date  
as  
begin  
select  
'DocuId|Compania|NroNota|FechaEmision|Documento|Numero|RazonSocial|RUC|Referencia|Nro|Serie|SubTotal|IGV|ICBPER|Total|Usuario|Estado|Direccion|Asociado|CompaniaRazon|CompaniaRUC|Concepto|Gravada|Descuento|Adcional¬100|80|100|110|115|120|340|105|120|100|100|115|115|90|115|150|130|100|100|100|100|220|100|100|100¬String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String¬'+  
isnull((select STUFF ((select '¬'+convert(varchar,d.DocuId)+'|'+convert(varchar,d.CompaniaId)+'|'+  
convert(varchar,d.NotaId)+'|'+(Convert(char(10),d.DocuEmision,103))+'|'+  
d.DocuDocumento+'|'+d.docuSerie+'-'+d.DocuNumero+'|'+c.ClienteRazon+'|'+c.ClienteRuc+'|'+  
d.DocuNroGuia+'|'+d.DocuNumero+'|'+d.DocuSerie+'|'+  
(convert(varchar(50), CAST(d.DocuSubTotal as money),1))+'|'+  
(convert(varchar(50), CAST(d.DocuIgv as money),1))+'|'+  
(convert(varchar(50), CAST(d.ICBPER as money),1))+'|'+  
(convert(varchar(50), CAST(d.DocuTotal as money),1))+'|'+  
d.DocuUsuario+'|'+d.DocuEstado+'|'+c.ClienteDireccion+'|'+d.DocuAsociado+'|'+  
co.CompaniaRazonSocial+'|'+co.CompaniaRUC+'|'+d.DocuConcepto+'|'+  
(convert(varchar(50), CAST(d.DocuGravada as money),1))+'|'+  
(convert(varchar(50), CAST(d.DocuDescuento as money),1))+'|'+  
(convert(varchar(50), CAST(d.DocuAdicional as money),1))  
from DocumentoVenta d  
inner join Cliente c  
on c.ClienteId=d.ClienteId  
inner join Compania co  
on co.CompaniaId=d.CompaniaId
where d.TipoCodigo='07' and(Convert(char(10),d.DocuEmision,101) BETWEEN @fechainicio AND @fechafin)  
order by d.DocuId desc  
for xml path('')),1,1,'')),'~')  
end
GO
/****** Object:  StoredProcedure [dbo].[uspListarProducto]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[uspListarProducto]  
  @Estado VARCHAR(20) = NULL  
AS  
BEGIN  
  SET NOCOUNT ON;  
  
  ;WITH filas AS  
  (  
    -- BASE  
    SELECT  
      p.IdProducto AS SortId,  
      1 AS SortTipo,  
      '¬' +  
      ISNULL(CONVERT(VARCHAR(20), p.IdProducto), '0') + '|' +  
      ISNULL(CONVERT(VARCHAR(20), p.IdSubLinea), '0') + '|' +  
      ISNULL(p.ProductoCodigo, '') + '|' +  
      ISNULL(p.ProductoNombre, '') + '|' +  
      ISNULL(p.ProductoUM, '') + '|' +  
      ISNULL(CONVERT(VARCHAR(50), p.ProductoCosto), '0') + '|' +  
      ISNULL(CONVERT(VARCHAR(50), p.ProductoVenta), '0') + '|' +  
      ISNULL(CONVERT(VARCHAR(50), p.ProductoVentaB), '0') + '|' +  
      ISNULL(CONVERT(VARCHAR(50), p.ProductoCantidad), '0') + '|' +  
      ISNULL(p.ProductoEstado, '') + '|' +  
      ISNULL(p.ProductoUsuario, '') + '|' +  
      ISNULL(CONVERT(VARCHAR(10), p.ProductoFecha, 23), '') + '|' +  
      ISNULL(p.ProductoImagen, '') + '|' +  
      ISNULL(CONVERT(VARCHAR(50), p.ValorCritico), '0') + '|' +  
      ISNULL(p.AplicaINV, '') AS RowText  
    FROM Producto p  
    WHERE (@Estado IS NULL OR p.ProductoEstado = @Estado)  
  
    UNION ALL  
  
    -- UNIDAD ALTERNA  
    SELECT  
      p.IdProducto AS SortId,  
      2 AS SortTipo,  
      '¬' +  
      ISNULL(CONVERT(VARCHAR(20), p.IdProducto), '0') + '|' +  
      ISNULL(CONVERT(VARCHAR(20), p.IdSubLinea), '0') + '|' +  
      ISNULL(p.ProductoCodigo, '') + '|' +  
      ISNULL(p.ProductoNombre, '') + '|' +  
      ISNULL(u.UMDescripcion, '') + '|' +  
      ISNULL(CONVERT(VARCHAR(50), u.PrecioCosto), '0') + '|' +  
      ISNULL(CONVERT(VARCHAR(50), u.PrecioVenta), '0') + '|' +  
      ISNULL(CONVERT(VARCHAR(50), u.PrecioVentaB), '0') + '|' +  
      ISNULL(
        CASE
          WHEN ISNULL(u.ValorUM, 0) = 0 THEN '0'
          ELSE CONVERT(VARCHAR, CAST((p.ProductoCantidad / u.ValorUM) AS MONEY), 1)
        END,
      '0') + '|' +  
      ISNULL(p.ProductoEstado, '') + '|' +  
      ISNULL(p.ProductoUsuario, '') + '|' +  
      ISNULL(CONVERT(VARCHAR(10), p.ProductoFecha, 23), '') + '|' +  
      ISNULL(u.unidadImagen, ISNULL(p.ProductoImagen, '')) + '|' +  
      ISNULL(CONVERT(VARCHAR(50), p.ValorCritico), '0') + '|' +  
      ISNULL(p.AplicaINV, '') AS RowText  
    FROM UnidadMedida u  
    INNER JOIN Producto p ON p.IdProducto = u.IdProducto  
    WHERE (@Estado IS NULL OR p.ProductoEstado = @Estado)  
  )  
  SELECT ISNULL(  
    STUFF(  
      (  
        SELECT f.RowText  
        FROM filas f  
        ORDER BY f.SortId DESC, f.SortTipo ASC  
        FOR XML PATH(''), TYPE  
      ).value('.', 'VARCHAR(MAX)'),  
      1, 1, ''  
    ),  
    '~'  
  ) AS Data;  
END  
GO
/****** Object:  StoredProcedure [dbo].[uspListarProveedor]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[uspListarProveedor]
    @Estado VARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        p.ProveedorId,
        p.ProveedorRazon,
        p.ProveedorRuc,
        p.ProveedorContacto,
        p.ProveedorCelular,
        p.ProveedorTelefono,
        p.ProveedorCorreo,
        p.ProveedorDireccion,
        p.ProveedorEstado
    FROM Proveedor p WITH (NOLOCK)
    WHERE (@Estado IS NULL OR p.ProveedorEstado = @Estado)
    ORDER BY p.ProveedorId DESC;
END;
GO
/****** Object:  StoredProcedure [dbo].[uspListaTemServicioCom]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspListaTemServicioCom]
@UsuarioId varchar(20)
as
begin
select
'TemId|UsuarioId|Descripcion|UM|Cantidad|PrecioCosto|Descuento|Importe|Estado¬100|100|100|100|100|100|100|100|100¬String|String|String|String|String|String|String|String|String¬'+ 
isnull((select stuff((select '¬'+convert(varchar,t.TemporalId)+'|'+convert(varchar,t.UsuarioId)+'|'+
t.TemporalDetalle+'|'+t.TemporalUm+'|'+
convert(varchar,t.TemporalCantidad)+'|'+
convert(varchar,t.TemporalCosto)+'|'+
convert(varchar,t.TemporalDescuento)+'|'+
convert(varchar,t.TemporalImporte)+'|'+
t.TemporalEstado 
from TemporalServicio t 
where t.UsuarioId=@UsuarioId
order by t.TemporalId asc
FOR XML path ('')),1,1,'')),'~')
end
GO
/****** Object:  StoredProcedure [dbo].[usplistaUnionPro]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[usplistaUnionPro]   
@IdProducto nvarchar(20)  
as  
begin  
select  
'Id|IdProducto|Cantidad|Unidad|Descripcion|PreVenta|ValorUM|Importe|Estado¬80|80|80|80|80|80|80|80|80¬String|String|String|String|String|String|String|String|String¬'+  
isnull((select STUFF ((select '¬'+convert(varchar,u.Id)+'|'+
CONVERT(varchar,u.IdProductoB)+'|'+convert(varchar,u.Cantidad)+'|'+
u.UM+'|'+p.ProductoNombre+'|'+
CONVERT(VarChar(50),cast(u.Precio as money ), 1)+'|'+
convert(varchar,u.ValorUM)+'|'+ 
CONVERT(VarChar(50),cast((u.Cantidad*u.Precio) as money ), 1)+'|'+  
u.Estado
from ProductoUnion u (nolock)
inner join Producto P (nolock)
on p.IdProducto=u.IdProductoB
where u.IdProducto=@IdProducto  
order by u.Id asc  
for xml path('')),1,1,'')),'~')  
end
GO
/****** Object:  StoredProcedure [dbo].[uspObtenerCredencialesSunat]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   PROCEDURE [dbo].[uspObtenerCredencialesSunat]  
    @CompaniaId INT  
AS  
BEGIN  
    SET NOCOUNT ON;  
  
    SELECT   
        CompaniaUserSecun   AS UsuarioSOL,  
        ComapaniaPWD        AS ClaveSOL,  
        CompaniaPFX         AS CertificadoPFX,  
        CompaniaClave       AS ClaveCertificado,  
        TIPO_PROCESO        AS Entorno  
    FROM Compania  
    WHERE CompaniaId = @CompaniaId;  
END
GO
/****** Object:  StoredProcedure [dbo].[uspObtenerFeriadoPorId]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[uspObtenerFeriadoPorId]
    @Id int
AS
BEGIN
    SELECT IdFeriado,Fecha, Motivo
    FROM Feriados
    WHERE IdFeriado = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[uspObtenerNotaPedido]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[uspObtenerNotaPedido]
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
GO
/****** Object:  StoredProcedure [dbo].[uspObtenerProductoPorId]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspObtenerProductoPorId]
@Id numeric(20)
as
begin
SELECT
    top 1
    p.IdProducto,
    p.IdSubLinea,
    p.ProductoCodigo,
    p.ProductoNombre,
    p.ProductoUM,
    CONVERT(VarChar,cast(p.ProductoCosto as money ), 1) as ProductoCosto,
    CONVERT(VarChar,cast(p.ProductoVenta as money ), 1) as ProductoVenta,
    CONVERT(VarChar,cast(p.ProductoVentaB as money ), 1) as ProductoVentaB,
    CONVERT(VarChar,cast(p.ProductoCantidad as money ), 1) as ProductoCantidad,
    p.ProductoEstado,
    p.ProductoUsuario,
    IsNull(convert(varchar,p.ProductoFecha,103),'')+' '+ IsNull(SUBSTRING(convert(varchar,p.ProductoFecha,114),1,8),'') as ProductoFecha,
    p.ProductoImagen,
    p.ValorCritico,
    p.AplicaINV
FROM Producto p WITH (NOLOCK)
WHERE p.IdProducto = @Id
End
GO
/****** Object:  StoredProcedure [dbo].[uspObtenerProveedorPorId]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspObtenerProveedorPorId]
@Id int
as
Begin
SELECT TOP 1
    p.ProveedorId,
    p.ProveedorRazon,
    p.ProveedorRuc,
    p.ProveedorContacto,
    p.ProveedorCelular,
    p.ProveedorTelefono,
    p.ProveedorCorreo,
    p.ProveedorDireccion,
    p.ProveedorEstado
FROM Proveedor p WITH (NOLOCK)
WHERE p.ProveedorId = @Id
End
GO
/****** Object:  StoredProcedure [dbo].[uspPasarPuntosListaCsv]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create procedure [dbo].[uspPasarPuntosListaCsv]
@Id varchar(20),
@fechainicio date,
@fechafin date
as
Begin
select 
'IdPuntos|NotaId|FechaEmision|FechaPase|Condicion|Documento|Numero|CodigoDXN|Cliente|RUC|DNI|CodigoRes|Responsable|Usuario|PVs|Subtotal|Descuento|OperacionGV|IGV|ICBPER|Total|NroTransaccion|Estado¬100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100¬String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String¬'+
isnull((select STUFF((select '¬'+Convert(varchar,p.IdPuntos)+'|'+
Convert(varchar,p.NotaId)+'|'+convert(varchar,p.FechaEmision,103)+'|'+p.FechaPase+'|'+
p.Condicion+'|'+p.Documento+'|'+p.Numero+'|'+p.CodigoDXN+'|'+p.Cliente+'|'+p.RUC+'|'+p.DNI+'|'+
p.CodigoRes+'|'+p.Responsable+'|'+p.Usuario+'|'+CONVERT(VarChar(50), cast(p.TotalPV as money ), 1)+'|'+
CONVERT(VarChar(50), cast(p.Subtotal as money ), 1)+'|'+CONVERT(VarChar(50), cast(p.Descuento as money ), 1)+'|'+
CONVERT(VarChar(50), cast(p.OperacionGV as money ), 1)+'|'+CONVERT(VarChar(50), cast(p.IGV as money ), 1)+'|'+
CONVERT(VarChar(50), cast(p.ICBPER as money ), 1)+'|'+CONVERT(VarChar(50), cast(p.Total as money ), 1)+'|'+
p.NroTransaccion+'|'+p.Estado
from PasarPuntos p
where CentroSV=@Id and (Convert(char(10),p.FechaEmision,101) BETWEEN @fechainicio AND @fechafin)
order by p.IdPuntos desc
for XMl path('')),1,1,'')),'~')
End
GO
/****** Object:  StoredProcedure [dbo].[uspPasarPuntosListaCsvA]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspPasarPuntosListaCsvA]
@Id varchar(20)
as
Begin
select 
'IdPuntos|NotaId|FechaEmision|FechaPase|Condicion|Documento|Numero|CodigoDXN|Cliente|RUC|DNI|CodigoRes|Responsable|Usuario|PVs|Subtotal|Descuento|OperacionGV|IGV|ICBPER|Total|NroTransaccion|Estado¬100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100|100¬String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String¬'+
isnull((select STUFF((select '¬'+Convert(varchar,p.IdPuntos)+'|'+
Convert(varchar,p.NotaId)+'|'+convert(varchar,p.FechaEmision,103)+'|'+p.FechaPase+'|'+
p.Condicion+'|'+p.Documento+'|'+p.Numero+'|'+p.CodigoDXN+'|'+p.Cliente+'|'+p.RUC+'|'+p.DNI+'|'+
p.CodigoRes+'|'+p.Responsable+'|'+p.Usuario+'|'+CONVERT(VarChar(50), cast(p.TotalPV as money ), 1)+'|'+
CONVERT(VarChar(50), cast(p.Subtotal as money ), 1)+'|'+CONVERT(VarChar(50), cast(p.Descuento as money ), 1)+'|'+
CONVERT(VarChar(50), cast(p.OperacionGV as money ), 1)+'|'+CONVERT(VarChar(50), cast(p.IGV as money ), 1)+'|'+
CONVERT(VarChar(50), cast(p.ICBPER as money ), 1)+'|'+CONVERT(VarChar(50), cast(p.Total as money ), 1)+'|'+
p.NroTransaccion+'|'+p.Estado
from PasarPuntos p
where CentroSV=@Id and p.Estado='PENDIENTE'
order by p.IdPuntos desc
for XMl path('')),1,1,'')),'~')
End
GO
/****** Object:  StoredProcedure [dbo].[uspRechazoBoleta]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspRechazoBoleta]        
@ListaOrden varchar(Max)        
as        
begin        
Declare @pos int        
Declare @orden varchar(max)        
Declare @detalle varchar(max)        
Set @pos = CharIndex('[',@ListaOrden,0)        
Set @orden = SUBSTRING(@ListaOrden,1,@pos-1)        
Set @detalle = SUBSTRING(@ListaOrden,@pos+1,len(@ListaOrden)-@pos)        
Declare  @pos1 int,@pos2 int,    
         @pos3 int,@pos4 int    
 Declare @NotaId numeric(38),@CodigoSunat VARCHAR(80),    
         @MensajeSunat varchar(max),@DocuHASH varchar(max),    
         @DocuId numeric(38)    
                
Set @pos1 = CharIndex('|',@orden,0)    
Set @pos2 = CharIndex('|',@orden,@pos1+1)        
Set @pos3 = CharIndex('|',@orden,@pos2+1)      
Set @pos4 = Len(@orden)+1    
    
Set @NotaId=convert(int,SUBSTRING(@orden,1,@pos1-1))        
Set @CodigoSunat=convert(numeric(38),SUBSTRING(@orden,@pos1+1,@pos2-@pos1-1))        
Set @MensajeSunat=SUBSTRING(@orden,@pos2+1,@pos3-@pos2-1)    
set @DocuHASH=SUBSTRING(@orden,@pos3+1,@pos4-@pos3-1)    
    
set @DocuId=(select top 1 d.DocuId     
from DocumentoVenta d    
where NotaId=@NotaId and EstadoSunat='PENDIENTE'    
order by d.DocuId desc)    
       
Begin Transaction        
    
update DocumentoVenta    
set DocuLetras='CERO CON 00/100 SOLES',DocuSubTotal=0,DocuIgv=0,    
DocuTotal=0,DocuSaldo=0,DocuAdicional=0,DocuEstado='RECHAZADO',    
DocuHash=@DocuHASH,EstadoSunat='RECHAZADO',ICBPER=0,CodigoSunat=@CodigoSunat,    
MensajeSunat=@MensajeSunat,DocuGravada=0,DocuDescuento=0,EnvioCorreo='',  
EntidadBancaria='',  
NroOperacion='',  
Efectivo=0,  
Deposito=0   
Where DocuId=@DocuId    
    
update NotaPedido         
set    NotaEstado='PENDIENTE'        
where  NotaId=@NotaId    
      
Declare Tabla Cursor For Select * From fnSplitString(@detalle,';')         
Open Tabla        
Declare @Columna varchar(max),    
        @IdProducto numeric(20),    
        @Cantidad decimal(18,2)    
        --@Costo decimal(18,4),@UM varchar(80),    
        --@ValorUM decimal(18,4)        
Declare @p1 int,@p2 int    
        --@p3 int,@p4 int,        
        --@p5 int    
               
Fetch Next From Tabla INTO @Columna        
 While @@FETCH_STATUS = 0        
 Begin        
    
Set @p1 = CharIndex('|',@Columna,0)        
--Set @p2 = CharIndex('|',@Columna,@p1+1)        
--Set @p3 = CharIndex('|',@Columna,@p2+1)        
--Set @p4 = CharIndex('|',@Columna,@p3+1)       
Set @p2 = Len(@Columna)+1    
        
Set @IdProducto=Convert(numeric(20),SUBSTRING(@Columna,1,@p1-1))        
Set @Cantidad=Convert(decimal(18,2),SUBSTRING(@Columna,@p1+1,@p2-(@p1+1)))      
--Set @Costo=Convert(decimal(18,4),SUBSTRING(@Columna,@p2+1,@p3-(@p2+1)))        
--Set @UM=SUBSTRING(@Columna,@p3+1,@p4-(@p3+1))        
--Set @ValorUM=Convert(decimal(18,4),SUBSTRING(@Columna,@p4+1,@p5-(@p4+1)))    
    
update Producto    
set ProductoCantidad=ProductoCantidad + @Cantidad    
where IdProducto=@IdProducto    
      
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
GO
/****** Object:  StoredProcedure [dbo].[uspReEnviarFactura]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspReEnviarFactura]    
@Data varchar(max)    
as    
begin    
Declare @p1 int,@p2 int,    
        @p3 int,@p4 int 
DECLARE @NotaId numeric(38),@CodigoSunat VARCHAR(80),    
        @MensajeSunat varchar(max),@DocuHASH varchar(max)
            
Set @p1 = CharIndex('|',@Data,0)    
Set @p2 = CharIndex('|',@Data,@p1+1)
Set @p3 = CharIndex('|',@Data,@p2+1)   
Set @p4 =Len(@Data)+1
  
Set @NotaId=convert(numeric(38),SUBSTRING(@Data,1,@p1-1))    
Set @CodigoSunat=convert(numeric(38),SUBSTRING(@Data,@p1+1,@p2-@p1-1))    
Set @MensajeSunat=SUBSTRING(@Data,@p2+1,@p3-@p2-1)
Set @DocuHASH=SUBSTRING(@Data,@p3+1,@p4-@p3-1)   
  
update DocumentoVenta    
set EstadoSunat='ENVIADO',CodigoSunat=@CodigoSunat,    
MensajeSunat=@MensajeSunat,DocuHash=@DocuHASH   
where NotaId=@NotaId and ((TipoCodigo='01'or TipoCodigo='03') and EstadoSunat='PENDIENTE')    
  
update DetallePedido    
set DetalleEstado='EMITIDO'    
where NotaId=@NotaId    
select 'true'     
  
end
GO
/****** Object:  StoredProcedure [dbo].[uspReEnviarNotaCredito]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspReEnviarNotaCredito]   
@Data varchar(max)   
as  
begin   
Declare @p1 int,@p2 int,@p3 int   
Declare @DocuId numeric(38),
        @CodigoSunat VARCHAR(80),  
        @MensajeSunat varchar(max)   
Set @p1 = CharIndex('|',@Data,0)   
Set @p2 = CharIndex('|',@Data,@p1+1)   
Set @p3 =Len(@Data)+1   
Set @DocuId=convert(numeric(38),SUBSTRING(@Data,1,@p1-1))  
Set @CodigoSunat=SUBSTRING(@Data,@p1+1,@p2-@p1-1)   
Set @MensajeSunat=SUBSTRING(@Data,@p2+1,@p3-@p2-1)  
update DocumentoVenta   
set EstadoSunat='ENVIADO',CodigoSunat=@CodigoSunat,MensajeSunat=@MensajeSunat   
where DocuId=@DocuId and (TipoCodigo='07' and EstadoSunat='PENDIENTE')  
select 'true'  
end
GO
/****** Object:  StoredProcedure [dbo].[uspReporteAnual]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspReporteAnual]
@CompaniaId int,
@ANNO int
AS
begin
SELECT
--isnull(b.NroMes,s.NroMes) as NroMes,
--isnull(b.Mes,S.Mes) as Mes, 
isnull(b.NroMes,isnull(S.NroMes,isnull(d.NroMes,isnull(x.NroMes,z.NroMes)))) as NroMes,
isnull(b.Mes,isnull(S.Mes,isnull(d.Mes,isnull(x.Mes,z.Mes)))) as Mes,
convert(varchar(50),cast((ISNULL(b.Monto,0))as money),1) as Ventas,
convert(varchar(50),cast((ISNULL(s.Monto,0)+ISNULL(d.Monto,0))-(ISNULL(x.Monto,0)+ISNULL(z.Monto,0))as money),1) as Compras,
convert(varchar(50),cast((ISNULL(b.Monto,0)-(ISNULL(s.Monto,0)+ISNULL(d.Monto,0))-(ISNULL(x.Monto,0)+ISNULL(z.Monto,0)))as money),1) as Ganancia
FROM(
select month(d.DocuEmision) as NroMes,Datename(MONTH,d.DocuEmision)as Mes,sum(d.DocuTotal)as Monto
from DocumentoVenta d with(nolock)
where (CompaniaId=@CompaniaId and year(d.DocuEmision)=@ANNO)and(D.DocuDocumento<>'PROFORMA V')
group by month(d.DocuEmision),Datename(MONTH,d.DocuEmision)) b
full join
(
    select month(c.CompraComputo) as NroMes,Datename(MONTH,c.CompraComputo)as Mes,SUM(c.CompraTotaL)as Monto
	from Compras c with(nolock)--FACTURAS EN SOLES
	where (c.CompaniaId=@CompaniaId AND year(c.CompraComputo)=@ANNO)and(c.TipoCodigo='01' and c.CompraMoneda='SOLES')
	group by month(c.CompraComputo),Datename(MONTH,c.CompraComputo)
)s on s.NroMes=b.NroMes
full join(
	select month(c.CompraComputo) as NroMes,Datename(MONTH,c.CompraComputo)as Mes,cast(sum(c.CompraTotal*c.CompraTipoSunat)as decimal(18,2)) as Monto
	from Compras c with(nolock)--FACTURAS EN DOLARES
	where (c.CompaniaId=@CompaniaId AND year(c.CompraComputo)=@ANNO) and (c.TipoCodigo='01' and c.CompraMoneda='DOLARES')
	group by month(c.CompraComputo),Datename(MONTH,c.CompraComputo)
)d on d.NroMes=b.NroMes
full join (
	select month(c.CompraComputo) as NroMes,Datename(MONTH,c.CompraComputo)as Mes,sum(c.CompraTotal) as Monto
	from Compras c with(nolock)--nota de credito en soles
	where (c.CompaniaId=@CompaniaId AND year(c.CompraComputo)=@ANNO) AND(c.TipoCodigo='07' and c.CompraMoneda='SOLES')
	group by month(c.CompraComputo),Datename(MONTH,c.CompraComputo)
)x on x.NroMes=b.NroMes
full join(
	select month(c.CompraComputo) as NroMes,Datename(MONTH,c.CompraComputo)as Mes,cast(sum(c.CompraTotal*c.CompraTipoSunat)as decimal(18,2)) as Monto
	from Compras c with(nolock)--credito EN DOLARES
	where c.CompaniaId=@CompaniaId AND year(c.CompraComputo)=@ANNO and (c.TipoCodigo='07' and c.CompraMoneda='DOLARES')
	group by month(c.CompraComputo),Datename(MONTH,c.CompraComputo)
)z on z.NroMes=b.NroMes
order by 1 asc
end
GO
/****** Object:  StoredProcedure [dbo].[uspReporteMozoCaja]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[uspReporteMozoCaja]  
@fechainicio date,  
@fechafin date
as  
begin  
select 
'Id|Cliente|Descripcion|Cantidad|UM|Importe¬0|220|450|100|100|115¬String|String|String|String|String|String¬'+
isnull((select STUFF ((select '¬'+
convert(varchar,c.ClienteId)+'|'+
c.ClienteRazon+'|'+ --as Cliente,
p.ProductoNombre+'|'+ --as Descripcion,  
CONVERT(VarChar(50), cast(SUM(d.DetalleCantidad) as money ), 1)+'|'+ --as Cantidad,
p.ProductoUM+'|'+ 
CONVERT(VarChar(50), cast(SUM(d.DetalleImporte) as money ), 1)--as Importe
from NotaPedido t  
inner join DetallePedido d
on d.NotaId=t.NotaId
inner join Cliente c
on c.ClienteId=t.ClienteId
inner join Producto p  
on p.IdProducto=d.IdProducto  
where (Convert(char(10),t.NotaFecha,101) BETWEEN @fechainicio AND @fechafin) 
and(t.NotaEstado='CANCELADO' and t.NotaConcepto='MERCADERIA') and c.ClienteId<>1
group by c.ClienteId,c.ClienteRazon,p.ProductoNombre,p.ProductoUM
order by c.ClienteRazon asc
for xml path('')),1,1,'')),'~')  
end  
GO
/****** Object:  StoredProcedure [dbo].[uspResumenDetalle]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspResumenDetalle]     
@fechainicio date,    
@fechafin date    
as    
begin    
select     
'ID|Descripcion|Cantidad|UM|Importe¬90|400|110|100|115¬String|String|String|String|String¬'+    
isnull((select STUFF((select '¬'+convert(varchar,d.IdProducto)+'|'+    
d.DetalleDescripcion+'|'+    
CONVERT(VarChar(50), cast(SUM(d.DetalleCantidad) as money ), 1)+'|'+d.DetalleUm+'|'+    
CONVERT(VarChar(50), cast(SUM(d.DetalleImporte) as money ), 1)    
from NotaPedido n    
inner join DetallePedido d    
on d.NotaId=n.NotaId    
where N.NotaConcepto='MERCADERIA' and (n.NotaEstado<>'ANULADO' and (Convert(char(10),n.NotaFecha,101) BETWEEN @fechainicio AND @fechafin))    
group by d.IdProducto,d.DetalleDescripcion,d.DetalleUm    
order by d.DetalleDescripcion asc    
for xml path('')),1,1,'')),'~')+'['+ 
'ID|Descripcion|Cantidad¬90|400|110¬String|String|String¬'+ 
isnull((select STUFF((select top 8 '¬'+convert(varchar,d.IdProducto)+'|'+      
d.DetalleDescripcion+' '+d.DetalleUm+'|'+      
CONVERT(VarChar(50), cast(SUM(d.DetalleCantidad) as money ), 1)  
from NotaPedido n      
inner join DetallePedido d      
on d.NotaId=n.NotaId      
where (Convert(char(10),n.NotaFecha,101) BETWEEN @fechainicio AND @fechafin) and   
(n.NotaConcepto='MERCADERIA' and d.IdProducto<>350 and   
n.NotaEstado<>'ANULADO')      
group by d.IdProducto,d.DetalleDescripcion,d.DetalleUm      
order by SUM(d.DetalleCantidad) desc      
for xml path('')),1,1,'')),'~')   
end  
GO
/****** Object:  StoredProcedure [dbo].[uspResumenDetalleZ]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[uspResumenDetalleZ]  
@Valores varchar(max)       
as          
begin  
Declare @Mes int,          
        @Anno int,  
        @IdSubLinea int  
Declare @a1 int,@a2 int,@a3 int  
  
Set @Valores= LTRIM(RTrim(@Valores))  
Set @a1=CharIndex('|',@Valores,0)  
Set @a2=CharIndex('|',@Valores,@a1+1)  
Set @a3=Len(@Valores)+1  
set @Mes=SUBSTRING(@Valores,1,@a1-1)  
set @Anno=SUBSTRING(@Valores,@a1+1,@a2-@a1-1)  
set @IdSubLinea=SUBSTRING(@Valores,@a2+1,@a3-@a2-1)         
select           
'ID|Descripcion|Cantidad¬90|400|110¬String|String|String¬'+          
isnull((select STUFF((select top 5 '¬'+convert(varchar,d.IdProducto)+'|'+          
d.DetalleDescripcion+' '+d.DetalleUm+'|'+          
CONVERT(VarChar(50), cast(SUM(d.DetalleCantidad) as money ), 1)      
from NotaPedido n          
inner join DetallePedido d          
on d.NotaId=n.NotaId  
inner join Producto p  
on p.IdProducto=d.IdProducto          
where p.IdSubLinea=@IdSubLinea and (month(n.NotaFecha)=@Mes and year(n.NotaFecha)=@Anno) and       
(n.NotaConcepto='MERCADERIA' and n.NotaEstado='CANCELADO')        
group by d.IdProducto,d.DetalleDescripcion,d.DetalleUm          
order by SUM(d.DetalleCantidad) desc          
for xml path('')),1,1,'')),'~')          
end
GO
/****** Object:  StoredProcedure [dbo].[uspResumenFecha]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- 4) Actualizar uspResumenFecha (incluye TieneCDR y sanitiza '|')
CREATE procedure [dbo].[uspResumenFecha]
@Data varchar(max)
as
begin
    declare @p1 int,@p2 int;
    declare @fechainicio date,@fechafin date;
    declare @sep char(1);
    set @sep = char(172);

    set @Data = ltrim(rtrim(@Data));
    set @p1 = charindex('|',@Data,0);
    set @p2 = len(@Data)+1;
    set @fechainicio = convert(date,substring(@Data,1,@p1-1));
    set @fechafin = convert(date,substring(@Data,@p1+1,@p2-@p1-1));

    select
    'Id|Compania|FechaEmision|FechaEnvio|Serie|RangoNumeros|SubTotal|IGV|ICBPER|Total|Ticket|CDSunat|HASHCDR|Mensaje|Usuario|RUC|UserSol|ClaveSol|ESTADO|Intentos|TokenApi|IdToken|TieneCDR|CDRBase64'
    + @sep +
    '100|100|100|100|100|100|110|110|110|100|100|100|100|100|100|100|100|100|100|100|100|100|80|300'
    + @sep +
    'String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String|String'
    + @sep +
    isnull((select stuff((select @sep + convert(varchar,r.ResumenId) + '|' + convert(varchar,r.CompaniaId) + '|' +
    isnull(convert(varchar,r.FechaReferencia,103),'') + '|' +
    (isnull(convert(varchar,r.FechaEnvio,103),'') + ' ' + isnull(substring(convert(varchar,r.FechaEnvio,114),1,8),'')) + '|' +
    r.ResumenSerie + '-' + convert(varchar,r.Secuencia) + '|' + isnull(r.RangoNumero,'') + '|' +
    convert(varchar(50),cast(r.SubTotal as money),1) + '|' +
    convert(varchar(50),cast(r.IGV as money),1) + '|' +
    convert(varchar(50),cast(r.ICBPER as money),1) + '|' +
    convert(varchar(50),cast(r.Total as money),1) + '|' +
    isnull(r.ResumenTiket,'') + '|' +
    replace(isnull(r.CodigoSunat,''),'|',' ') + '|' +
    replace(isnull(r.HASHCDR,''),'|',' ') + '|' +
    replace(isnull(r.MensajeSunat,''),'|',' ') + '|' +
    replace(isnull(r.Usuario,''),'|',' ') + '|' +
    isnull(c.CompaniaRUC,'') + '|' +
    isnull(c.CompaniaUserSecun,'') + '|' +
    isnull(c.ComapaniaPWD,'') + '|' +
    isnull(r.Estado,'') + '||' +
    isnull(c.TokenApi,'') + '|' +
    isnull(c.ClienIdToken,'') + '|' +
    case when isnull(r.CDRBase64,'')='' then 'NO' else 'SI' end + '|' +
    replace(isnull(r.CDRBase64,''),'|',' ')
    from ResumenBoletas r
    inner join Compania c on c.CompaniaId=r.CompaniaId
    where r.FechaReferencia between @fechainicio and @fechafin
    order by r.CompaniaId,r.FechaEnvio asc
    for xml path('')),1,1,'')),'~');
end
GO
/****** Object:  StoredProcedure [dbo].[uspResumenSubLinea]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspResumenSubLinea]    
@Data varchar(max)    
as    
Declare @IdSubLinea numeric(20),    
        @fechainicio date,    
        @fechafin date    
Declare @p1 int,@p2 int,@p3 int    
Set @p1 = CharIndex('|',@Data,0)    
Set @p2 = CharIndex('|',@Data,@p1+1)    
Set @p3 = Len(@Data)+1    
Set @IdSubLinea=convert(numeric(20),SUBSTRING(@Data,1,@p1-1))    
Set @fechainicio=convert(date,SUBSTRING(@Data,@p1+1,@p2-@p1-1))    
Set @fechafin=SUBSTRING(@Data,@p2+1,@p3-@p2-1)    
begin    
select     
'ID|Descripcion|Cantidad|UM|Importe¬90|400|110|100|115¬String|String|String|String|String¬'+    
isnull((select STUFF((select '¬'+convert(varchar,d.IdProducto)+'|'+    
d.DetalleDescripcion+'|'+    
CONVERT(VarChar(50), cast(SUM(d.DetalleCantidad) as money ), 1)+'|'+d.DetalleUm+'|'+    
CONVERT(VarChar(50), cast(SUM(d.DetalleImporte) as money ), 1)    
from NotaPedido n    
inner join DetallePedido d    
on d.NotaId=n.NotaId    
inner join Producto p    
on p.IdProducto=d.IdProducto    
where p.IdSubLinea=@IdSubLinea and n.NotaConcepto='MERCADERIA' and
(n.NotaEstado<>'ANULADO' and (Convert(char(10),n.NotaFecha,101) BETWEEN @fechainicio AND @fechafin))    
group by d.IdProducto,d.DetalleDescripcion,d.DetalleUm    
order by SUM(d.DetalleCantidad) desc    
for xml path('')),1,1,'')),'~')+'['+
'ID|Descripcion|Cantidad¬90|400|110¬String|String|String¬'+ 
isnull((select STUFF((select top 8 '¬'+convert(varchar,d.IdProducto)+'|'+      
d.DetalleDescripcion+' '+d.DetalleUm+'|'+      
CONVERT(VarChar(50), cast(SUM(d.DetalleCantidad) as money ), 1)  
from NotaPedido n      
inner join DetallePedido d      
on d.NotaId=n.NotaId
inner join Producto p
on p.IdProducto=d.IdProducto
where p.IdSubLinea=@IdSubLinea and n.NotaConcepto='MERCADERIA' and 
(Convert(char(10),n.NotaFecha,101) BETWEEN @fechainicio AND @fechafin) and   
(n.NotaConcepto='MERCADERIA' and d.IdProducto<>350 and   
n.NotaEstado<>'ANULADO')      
group by d.IdProducto,d.DetalleDescripcion,d.DetalleUm      
order by SUM(d.DetalleCantidad) desc      
for xml path('')),1,1,'')),'~')   
end  
GO
/****** Object:  StoredProcedure [dbo].[uspRetornaBoletaPorTicket]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- 3) Actualizar uspRetornaBoletaPorTicket
CREATE procedure [dbo].[uspRetornaBoletaPorTicket]
@ResumenId varchar(80)
as
begin
    declare @FechaEmision date;
    declare @Dia int,@Mes int,@ANNO int;

    set @FechaEmision = (select top 1 r.FechaReferencia from ResumenBoletas r where r.ResumenId=@ResumenId);
    set @Dia = day(@FechaEmision);
    set @Mes = month(@FechaEmision);
    set @ANNO = year(@FechaEmision);

    update ResumenBoletas
       set MensajeSunat='NO SE GENERO EL TICKET DE RESPUESTA DE SUNAT',
           HASHCDR='',
           CDRBase64=''
     where ResumenId=@ResumenId;

    update DocumentoVenta
       set EstadoSunat='PENDIENTE'
     where (day(DocuEmision)=@Dia and month(DocuEmision)=@Mes and year(DocuEmision)=@ANNO)
       and TipoCodigo='03';

    select 'true';
end
GO
/****** Object:  StoredProcedure [dbo].[uspRetornarBoletas]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspRetornarBoletas]  
@ResumenId numeric(38)  
as  
begin  
declare @FechaEmision date  
declare @Dia int,@Mes int,@ANNO int  
set @FechaEmision=(select top 1 r.FechaReferencia from ResumenBoletas r where r.ResumenId=@ResumenId)  
set @Dia=DAY(@FechaEmision)  
set @Mes=MONTH(@FechaEmision)  
set @ANNO=YEAR(@FechaEmision)  
update DocumentoVenta  
set EstadoSunat='PENDIENTE'  
WHERE (DAY(DocuEmision)=@Dia AND MONTH(DocuEmision)=@Mes and YEAR(DocuEmision)=@ANNO) and TipoCodigo='03'  
select 'true'  
end
GO
/****** Object:  StoredProcedure [dbo].[uspTop8Clientes]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspTop8Clientes]       
@Mes int,      
@Anno int      
as      
begin      
select       
'ID|Cliente|Venta¬90|400|110¬String|String|String¬'+      
isnull((select STUFF((select top 8 '¬'+convert(varchar,n.ClienteId)+'|'+      
c.ClienteRazon+'|'+      
CONVERT(VarChar(50),cast(SUM(n.NotaPagar) as money ), 1)  
from NotaPedido n
inner join Cliente c
on c.ClienteId=n.ClienteId
where (month(n.NotaFecha)=@Mes and year(n.NotaFecha)=@Anno) and   
(n.NotaConcepto='MERCADERIA' and n.ClienteId<>1 and n.NotaEstado<>'ANULADO')      
group by n.ClienteId,c.ClienteRazon     
order by SUM(n.NotaPagar) desc      
for xml path('')),1,1,'')),'~')
end    
GO
/****** Object:  StoredProcedure [dbo].[uspTop8ClientesCount]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspTop8ClientesCount]       
@Mes int,      
@Anno int      
as      
begin      
select       
'ID|Cliente|Venta¬90|400|110¬String|String|String¬'+      
isnull((select STUFF((select top 8 '¬'+convert(varchar,n.ClienteId)+'|'+      
c.ClienteRazon+'|'+      
CONVERT(VarChar(50),Count(n.ClienteId))  
from NotaPedido n
inner join Cliente c
on c.ClienteId=n.ClienteId
where (month(n.NotaFecha)=@Mes and year(n.NotaFecha)=@Anno) and   
(n.NotaConcepto='MERCADERIA' and n.ClienteId<>1 and n.NotaEstado<>'ANULADO')      
group by n.ClienteId,c.ClienteRazon     
order by Count(n.ClienteId) desc      
for xml path('')),1,1,'')),'~')
end    
GO
/****** Object:  StoredProcedure [dbo].[usptraeNewCodePro]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[usptraeNewCodePro]
as
begin
select top 1 convert(numeric(38),substring(p.ProductoCodigo,4,LEN(p.ProductoCodigo)))+1 as Codigo 
from producto p 
where ProductoCodigo like'%MR00%' 
order by convert(numeric(38),substring(p.ProductoCodigo,4,LEN(p.ProductoCodigo))) desc
end
GO
/****** Object:  StoredProcedure [dbo].[usptraerCaja]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[usptraerCaja]
@UsuarioId int
as
begin
select
isnull((select stuff((select '¬'+
convert(varchar,c.CajaId)+'|'+
CONVERT(VarChar(50), cast(c.MontoIniSOl as money ), 1)+'|'+
c.CajaEncargado
from Caja c 
where c.CajaEstado='ACTIVO' and UsuarioId=@UsuarioId
order by c.CajaId desc
for xml path('')),1,1,'')),'0')
end
GO
/****** Object:  StoredProcedure [dbo].[uspTraerFeriados]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[uspTraerFeriados] AS 
BEGIN SET NOCOUNT ON; 
SELECT ISNULL((SELECT STUFF((SELECT '¬' + CONVERT(VARCHAR, f.IdFeriado) + '|' +
CONVERT(VARCHAR(20), f.Fecha, 23) + '|' + 
ISNULL(f.Motivo, '') 
FROM dbo.Feriados f 
ORDER BY f.Fecha ASC 
FOR XML PATH('')), 1, 1, '')), '~'); END
GO
/****** Object:  StoredProcedure [dbo].[uspTraerGastosA]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[uspTraerGastosA]
@CajaId varchar(40)
as
begin
Select
isnull((select STUFF((select '¬'+ c.DetalleConcepto+'|'+
case when c.DetalleMonto<=0 then
''
else CONVERT(VarChar(max),cast(c.DetalleMonto as money ), 1) end +'|'+
c.Estado+'|'+CONVERT(varchar,c.DetalleId)+'|S'
from CajaDetalle c
where (CajaId=@CajaId and NotaId=0) and c.DetalleMovimiento='SALIDA'
order by c.DetalleId asc
FOR XML path ('')),1,1,'')),'~')+'['+
isnull((select STUFF((select '¬'+ c.DetalleConcepto+'|'+
case when c.DetalleMonto<=0 then
''
else CONVERT(VarChar(max),cast(c.DetalleMonto as money ), 1) end +'|'+
c.Estado+'|'+CONVERT(varchar,c.DetalleId)+'|I'
from CajaDetalle c
where (CajaId=@CajaId and NotaId=0)and c.DetalleMovimiento='INGRESO'
order by c.DetalleId asc
FOR XML path ('')),1,1,'')),'~')
end
GO
/****** Object:  StoredProcedure [dbo].[uspTraerPFX]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspTraerPFX]  
@CompaniaId int  
as  
begin  
SELECT   
isnull((select STUFF ((select top 1'¬'+convert(varchar,c.CompaniaId)+'|'+c.CompaniaRazonSocial+'|'+  
c.CompaniaComercial+'|'+c.CompaniaRUC+'|'+c.CompaniaUserSecun+'|'+c.ComapaniaPWD+'|'+c.CompaniaPFX+'|'+c.CompaniaClave+'|'+  
convert(varchar,dbo.genenerarNroFactura('F001',@CompaniaId,'FACTURA'))+'|'+c.CompaniaEmail+'|'+c.CompaniaDireccion+'|'+  
c.CompaniaTelefono+'|'+CompaniaNomUBG+'|'+CompaniaCodigoUBG+'|'+CompaniaDistrito+'|'+CompaniaDirecSunat  
FROM Compania c  
where c.CompaniaId=@CompaniaId  
for xml path('')),1,1,'')),'~')  
end
GO
/****** Object:  StoredProcedure [dbo].[usptraerSecuenciaResumen]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[usptraerSecuenciaResumen]  
@CompaniaId varchar(20)  
as  
begin  
Declare @COUNT INT  
set @COUNT=(select COUNT(*) from ResumenBoletas)  
if(@COUNT=0)  
begin  
select '1'  
end  
else  
begin  
select top 1 convert(varchar,Secuencia+1)  
from ResumenBoletas where CompaniaId =@CompaniaId  
order by Secuencia desc  
end  
end
GO
/****** Object:  StoredProcedure [dbo].[uspUtilitario]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[uspUtilitario]
as
begin
select 
'TABLAS|TABLE['+
isnull((select STUFF((select '¬'+s.name+'|'+s.name
from sys.tables s
order by s.name asc
for XMl path('')),1,1,'')),'~')+'['+
'TYPO|COLUMN_NAME|DATA_TYPE|TAMANO¬0|220|150|115¬'+
isnull((select STUFF((select '¬'+ I.DATA_TYPE+'|'+I.COLUMN_NAME+'|'+I.DATA_TYPE+'|'+
       isnull(convert(varchar,case when CHARACTER_MAXIMUM_LENGTH is null then
       NUMERIC_PRECISION
       else CHARACTER_MAXIMUM_LENGTH end),'0')+','+isnull(convert(varchar,NUMERIC_SCALE),'0')+'|'+
       I.TABLE_NAME
FROM   INFORMATION_SCHEMA.COLUMNS I
order by TABLE_NAME asc
for XMl path('')),1,1,'')),'~')
end
GO
/****** Object:  StoredProcedure [dbo].[uspValidaPuntosPendientes]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspValidaPuntosPendientes]  
@centro varchar(20)  
as  
begin  
Declare @catidad int  
set @catidad=isnull((select convert(varchar,COUNT(*)) from PasarPuntos p  
where p.CentroSV =@centro  
AND p.Estado = 'PENDIENTE'),0)  
select convert(varchar,@catidad) 
--select '0' 
end
GO
/****** Object:  StoredProcedure [dbo].[uspValidarApertura]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[uspValidarApertura]
as
begin
--Declare @BoletaPen int
--Declare @ConsultaPen int 
--Declare @AnuladosPen int
--Declare @ConsultaError int
--set @BoletaPen=(select top 1 count(DocuId) from DocumentoVenta
--where TipoCodigo='03'and((CompaniaId=1 and EstadoSunat='PENDIENTE')
--and DocuEmision<convert(date,GETDATE())))
--set @ConsultaPen=(select COUNT(ResumenId) from ResumenBoletas
--where CodigoSunat='')
--set @AnuladosPen=(select COUNT(d.DocuId) from DocumentoVenta d
--where d.TipoCodigo='03'and((d.CompaniaId=1 and DocuEstado='ANULADO' and d.EstadoSunat='ENVIADO')))
--set @ConsultaError=(select COUNT(ResumenId) from ResumenBoletas
--where CodigoSunat='env:Server' or CodigoSunat='env:Client')
--if(@BoletaPen>0)
--begin
--select 'BOLETA'
--END
--else if(@AnuladosPen>0)
--begin
--select 'ANULADOS'
--end
--else if(@ConsultaPen>0)
--begin
--select 'CONSULTA'
--end
--else if(@ConsultaError>0)
--begin
--select 'ERROR'
--end
--else
--begin
select 'true'
--end
end
GO
/****** Object:  StoredProcedure [dbo].[uspValidaUsuario]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[uspValidaUsuario]
@Data varchar(max)
AS
BEGIN
    DECLARE @p1 INT, @p2 INT

    DECLARE @Usuario VARCHAR(150),
            @Clave VARCHAR(150)

    SET @Data = LTRIM(RTRIM(@Data))
    SET @p1 = CHARINDEX('|', @Data, 0)
    SET @p2 = LEN(@Data) + 1

    SET @Usuario = SUBSTRING(@Data, 1, @p1 - 1)
    SET @Clave   = SUBSTRING(@Data, @p1 + 1, @p2 - @p1 - 1)

    SELECT
    ISNULL((
        SELECT STUFF((
            SELECT TOP 1
                '¬' + CONVERT(VARCHAR, U.UsuarioID) + '|' +
                CONVERT(VARCHAR, p.PersonalId) + '|' +
                a.AreaNombre + '|' +
                (
                    (SUBSTRING(p.PersonalNombres + ' ', 1, CHARINDEX(' ', p.PersonalNombres + ' ') - 1)) + ' ' +
                    (SUBSTRING(p.PersonalApellidos + ' ', 1, CHARINDEX(' ', p.PersonalApellidos + ' ') - 1))
                ) + '|' +
                CONVERT(VARCHAR, p.CompaniaId) + '|' +
                c.CompaniaRazonSocial + '|' +
                ISNULL(CONVERT(VARCHAR(10), U.FechaVencimientoClave, 23), '') + '|' +
                ISNULL(CONVERT(VARCHAR(20), c.DescuentoMax), '0') + '|' +
                ISNULL(c.CompaniaRUC, '') + '|' +
                ISNULL(c.CompaniaNomUBG, '') + '|' +
                ISNULL(c.CompaniaComercial, '') + '|' +
                ISNULL(c.CompaniaDirecSunat, '') + '|' +
                ISNULL(c.CompaniaUserSecun, '') + '|' +
                ISNULL(c.ComapaniaPWD, '') + '|' +
                ISNULL(c.CompaniaPFX, '') + '|' +
                ISNULL(c.CompaniaClave, '') + '|' +
                ISNULL(CONVERT(VARCHAR, c.TIPO_PROCESO), '3') + '|' +
                ISNULL(c.CompaniaTelefono, '') + '|' +
                ISNULL(CONVERT(VARCHAR, c.BoletaPorLote), '1')
            FROM Usuarios U
            INNER JOIN Personal p ON p.PersonalId = U.PersonalId
            INNER JOIN Area a ON a.AreaId = p.AreaId
            INNER JOIN Compania c ON c.CompaniaId = p.CompaniaId
            WHERE U.UsuarioAlias = @Usuario
              AND dbo.desincrectar(U.UsuarioClave) = @Clave
              AND UsuarioEstado = 'ACTIVO'
              AND p.PersonalEstado = 'ACTIVO'
            FOR XML PATH('')
        ), 1, 1, '')
    ), '~')
END
GO
/****** Object:  StoredProcedure [dbo].[validarDatos]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[validarDatos]    
@NotaId varchar(38)   
as    
begin    
select
isnull((select STUFF((select '¬'+ 
convert(varchar,a.NotaId)+'|'+
a.NotaEstado+'|'+
convert(varchar,a.Cantidad)+'|'+
convert(varchar,isnull(b.Documento,0))+'|'+
convert(varchar,isnull(b.Acuenta,0)) 
from     
(select top 1 
n.NotaId,
n.NotaEstado,    
COUNT(IdDetalle) as Cantidad     
from DetalleGuia g     
inner join DetallePedido d     
on d.DetalleId=g.IdDetalle     
right join NotaPedido n    
on n.NotaId=d.NotaId    
where n.NotaId=@NotaId   
group by n.NotaId,n.NotaEstado) a     
full join     
(select top 1 
d.NotaId as NotaId,
COUNT(d.NotaId) as Documento,
COUNT(l.NotaId) as Acuenta     
from DocumentoVenta d    
left join DetaLiquidaVenta l    
on l.NotaId=d.NotaId     
where d.NotaId=@NotaId and d.DocuEstado<>'RECHAZADO'    
group by d.NotaId) b     
on a.NotaId=b.NotaId
FOR XML path ('')),1,1,'')),'~')   
end
GO
/****** Object:  StoredProcedure [dbo].[ventanaDeudas]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[ventanaDeudas]    
as    
begin     
select  
'DocuId|Cliente|Ruc|FechaEmision|FechaPago|Documento|SaldoDoc|Tarjeta|Total|NotaId¬90|90|90|90|90|90|90|90|90|90¬String|String|String|String|String|String|String|String|String|String¬'+  
isnull((select STUFF ((select '¬'+convert(varchar,n.NotaId)+'|'+c.ClienteRazon+'|'+  
c.ClienteRuc+'|'+  
(IsNull(convert(varchar,n.NotaFecha,103),'')+' '+ IsNull(SUBSTRING(convert(varchar,n.NotaFecha,114),1,8),''))+'|'+   
(Convert(char(10),n.NotaFechaPago,103))+'|'+   
n.NotaSerie+'-'+n.NotaNumero+'|'+  
CONVERT(VarChar(50),cast(n.NotaSaldo as money ), 1)+'|'+  
CONVERT(VarChar(50),cast(n.NotaTarjeta as money ), 1)+'|'+    
CONVERT(VarChar(50),cast(n.NotaPagar as money ), 1)+'|'+  
convert(varchar,n.NotaId)    
from NotaPedido n(nolock)   
inner join Cliente c(nolock)    
on  c.ClienteId=n.ClienteId    
where (n.NotaCondicion='CREDITO' and (n.NotaEstado<>'CANCELADO' and n.NotaEstado<>'ANULADO'))and n.NotaSaldo > 0    
order by n.NotaId desc  
for xml path('')),1,1,'')),'~')    
end
GO
/****** Object:  StoredProcedure [dbo].[ventanaFacturas]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create procedure [dbo].[ventanaFacturas]
@TipoCodigo varchar(40)  
as  
begin  
select
'Id|Proveedor|FechaEmision|Documento|Moneda|TipoCambio|SaldoDocu|MontoDocu|Docu¬90|90|90|90|90|90|90|90|90¬String|String|String|String|String|String|String|String|String¬'+ 
isnull((select STUFF ((select '¬'+convert(varchar,c.CompraId)+'|'+
p.ProveedorRazon+'|'+
(Convert(char(10),c.CompraEmision,103))+'|'+
substring(t.TipoDescripcion,1,1)+'C '+c.CompraSerie+'-'+c.CompraNumero+'|'+
c.CompraMoneda+'|'+
convert(varchar,c.CompraTipoCambio)+'|'+
CONVERT(VarChar(50),cast(c.CompraSaldo as money), 1)+'|'+
CONVERT(VarChar(50),cast(c.CompraTotal as money ), 1)+'|'+  
t.TipoDescripcion 
from Compras c  
inner join Proveedor p  
on  c.ProveedorId=p.ProveedorId  
inner join TipoComprobante t  
on t.TipoCodigo=c.TipoCodigo  
where t.TipoCodigo=@TipoCodigo and c.CompraEstado='PENDIENTE DE PAGO'  
order by c.CompraId desc
for xml path('')),1,1,'')),'~')   
end
GO
/****** Object:  StoredProcedure [dbo].[ventanaLetras]    Script Date: 20/04/2026 16:11:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[ventanaLetras]  
as  
begin  
select
'Id|IdLetra|Proveedor|F_Vencimiento|DocumentoFechaGiro|Moneda|SaldoDocu|MontoDocu¬90|90|90|90|90|90|90|90|90¬String|String|String|String|String|String|String|String|String¬'+ 
isnull((select STUFF ((select '¬'+convert(varchar,d.DetalleId)+'|'+
convert(varchar,l.LetraId)+'|'+
p.ProveedorRazon+'|'+
(Convert(char(10),d.LetraVencimiento,103))+'|'+
'LT '+d.LetraCanje+'|'+  
(Convert(char(10),l.LetraFechaGiro,103))+'|'+
l.LetraMoneda,CONVERT(VarChar(50),cast(d.DetalleSaldo as money ), 1)+'|'+ 
CONVERT(VarChar(50),cast(d.DetalleMonto as money ), 1)  
from DetalleLetra d  
inner join Letra l  
on l.LetraId=d.LetraId  
inner join Proveedor p  
on p.ProveedorId=l.ProveedorId  
where d.DetalleEstado<>'TOTALMENTE PAGADO'  
order by d.LetraVencimiento asc
for xml path('')),1,1,'')),'~') 
end
GO
USE [master]
GO
ALTER DATABASE [BD_SGOV] SET  READ_WRITE 
GO
