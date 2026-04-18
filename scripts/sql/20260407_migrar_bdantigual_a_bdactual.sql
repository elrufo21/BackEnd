/*******************************************************************************
 Script: 20260407_migrar_bdantigual_a_bdactual.sql
 Objetivo: Alinear esquema/programabilidad de bdantigual con bdactual
 Generado automaticamente desde scripts/sql/bdactual.sql
 Fecha: 2026-04-07
*******************************************************************************/

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- 1) Crear tablas faltantes
IF OBJECT_ID(N'[dbo].[__EFMigrationsHistory]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[__EFMigrationsHistory](
    	[MigrationId] [nvarchar](150) NOT NULL,
    	[ProductVersion] [nvarchar](32) NOT NULL,
     CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
    (
    	[MigrationId] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY]
END
GO

IF OBJECT_ID(N'[dbo].[Addresses]', N'U') IS NULL
BEGIN
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
END
GO

IF OBJECT_ID(N'[dbo].[Countries]', N'U') IS NULL
BEGIN
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
END
GO

IF OBJECT_ID(N'[dbo].[Feriados]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Feriados](
    	[IdFeriado] [int] IDENTITY(1,1) NOT NULL,
    	[Fecha] [date] NULL,
    	[Motivo] [varchar](300) NULL,
    PRIMARY KEY CLUSTERED 
    (
    	[IdFeriado] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY]
END
GO

IF OBJECT_ID(N'[dbo].[Images]', N'U') IS NULL
BEGIN
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
END
GO

IF OBJECT_ID(N'[dbo].[Reviews]', N'U') IS NULL
BEGIN
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
END
GO

IF OBJECT_ID(N'[dbo].[ShoppingCartItems]', N'U') IS NULL
BEGIN
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
END
GO

IF OBJECT_ID(N'[dbo].[ShoppingCarts]', N'U') IS NULL
BEGIN
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
END
GO

-- 2) Agregar columnas faltantes
IF OBJECT_ID(N'[dbo].[Compania]', N'U') IS NOT NULL AND COL_LENGTH(N'dbo.Compania', N'TIPO_PROCESO') IS NULL
BEGIN
    ALTER TABLE [dbo].[Compania] ADD [TIPO_PROCESO] [int] NULL
END
GO

IF OBJECT_ID(N'[dbo].[MAQUINAS]', N'U') IS NOT NULL AND COL_LENGTH(N'dbo.MAQUINAS', N'Registro') IS NULL
BEGIN
    ALTER TABLE [dbo].[MAQUINAS] ADD [Registro] [datetime] NULL
END
GO

IF OBJECT_ID(N'[dbo].[MAQUINAS]', N'U') IS NOT NULL AND COL_LENGTH(N'dbo.MAQUINAS', N'SerieNC') IS NULL
BEGIN
    ALTER TABLE [dbo].[MAQUINAS] ADD [SerieNC] [nvarchar](4) NULL
END
GO

IF OBJECT_ID(N'[dbo].[MAQUINAS]', N'U') IS NOT NULL AND COL_LENGTH(N'dbo.MAQUINAS', N'Tiketera') IS NULL
BEGIN
    ALTER TABLE [dbo].[MAQUINAS] ADD [Tiketera] [varchar](300) NULL
END
GO

IF OBJECT_ID(N'[dbo].[ResumenBoletas]', N'U') IS NOT NULL AND COL_LENGTH(N'dbo.ResumenBoletas', N'CDRBase64') IS NULL
BEGIN
    ALTER TABLE [dbo].[ResumenBoletas] ADD [CDRBase64] [varchar](max) NULL
END
GO

IF OBJECT_ID(N'[dbo].[UnidadMedida]', N'U') IS NOT NULL AND COL_LENGTH(N'dbo.UnidadMedida', N'unidadImagen') IS NULL
BEGIN
    ALTER TABLE [dbo].[UnidadMedida] ADD [unidadImagen] [varchar](255) NULL
END
GO

IF OBJECT_ID(N'[dbo].[Usuarios]', N'U') IS NOT NULL AND COL_LENGTH(N'dbo.Usuarios', N'FechaVencimientoClave') IS NULL
BEGIN
    ALTER TABLE [dbo].[Usuarios] ADD [FechaVencimientoClave] [date] NULL
END
GO

-- 3) Ajustar tipos/definiciones de columnas distintas
IF OBJECT_ID(N'[dbo].[Compania]', N'U') IS NOT NULL AND COL_LENGTH(N'dbo.Compania', N'CompaniaPFX') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[Compania] ALTER COLUMN [CompaniaPFX] [varchar](max) NULL
END
GO

IF OBJECT_ID(N'[dbo].[Personal]', N'U') IS NOT NULL AND COL_LENGTH(N'dbo.Personal', N'AreaId') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[Personal] ALTER COLUMN [AreaId] [int] NULL
END
GO

IF OBJECT_ID(N'[dbo].[Usuarios]', N'U') IS NOT NULL AND COL_LENGTH(N'dbo.Usuarios', N'PersonalId') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[Usuarios] ALTER COLUMN [PersonalId] [int] NULL
END
GO

-- 4) FK faltante en tablas nuevas de carrito
IF OBJECT_ID(N'[dbo].[ShoppingCartItems]', N'U') IS NOT NULL
   AND OBJECT_ID(N'[dbo].[ShoppingCarts]', N'U') IS NOT NULL
   AND NOT EXISTS (
       SELECT 1
       FROM sys.foreign_keys
       WHERE name = N'FK_ShoppingCartItems_ShoppingCarts_ShoppingCartId'
         AND parent_object_id = OBJECT_ID(N'[dbo].[ShoppingCartItems]')
   )
BEGIN
    ALTER TABLE [dbo].[ShoppingCartItems]  WITH CHECK ADD  CONSTRAINT [FK_ShoppingCartItems_ShoppingCarts_ShoppingCartId] FOREIGN KEY([ShoppingCartId])
    REFERENCES [dbo].[ShoppingCarts] ([Id])
    ON DELETE CASCADE

    ALTER TABLE [dbo].[ShoppingCartItems] CHECK CONSTRAINT [FK_ShoppingCartItems_ShoppingCarts_ShoppingCartId]
END
GO

-- 5) Crear/actualizar stored procedures de bdactual
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[listaNotaPedido]
@FechaInicio DATE,
@FechaFin DATE
AS
BEGIN
SET NOCOUNT ON;

SELECT
ISNULL((
SELECT STUFF((
SELECT
'Â¬'+
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
CONVERT(VARCHAR,n.NotaFecha,103)+'|'+
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

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[uspEliminarArea]
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
	--PRINT 'Se encontrÃ³ un error: ' + @ErrorMsg + ' (NÃºmero: ' + CAST(@ErrorNum AS VARCHAR) + ', Severidad: ' + CAST(@ErrorSeverity AS VARCHAR) + ')';

	 if(@ErrorNum=547 and @ErrorSeverity=16)
		begin 
			PRINT 'No se pudo eliminar, ya que tiene relacion con otros modulos'
		end 

END CATCH
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[uspEliminarCategoria]
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
	--PRINT 'Se encontrÃ³ un error: ' + @ErrorMsg + ' (NÃºmero: ' + CAST(@ErrorNum AS VARCHAR) + ', Severidad: ' + CAST(@ErrorSeverity AS VARCHAR) + ')';

	 if(@ErrorNum=547 and @ErrorSeverity=16)
		begin 
			PRINT 'No se pudo eliminar, ya que tiene relacion con otros modulos'
		end 

END CATCH
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[uspEliminarCliente]
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
	--PRINT 'Se encontrÃ³ un error: ' + @ErrorMsg + ' (NÃºmero: ' + CAST(@ErrorNum AS VARCHAR) + ', Severidad: ' + CAST(@ErrorSeverity AS VARCHAR) + ')';

	 if(@ErrorNum=547 and @ErrorSeverity=16)
		begin 
			PRINT 'No se pudo eliminar, ya que tiene relacion con otros modulos'
		end 

END CATCH
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[uspEliminarFeriado]
    @Id int
AS
BEGIN
    DELETE FROM dbo.Feriados WHERE IdFeriado = @Id
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[uspEliminarPersonal]
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
	--PRINT 'Se encontrÃ³ un error: ' + @ErrorMsg + ' (NÃºmero: ' + CAST(@ErrorNum AS VARCHAR) + ', Severidad: ' + CAST(@ErrorSeverity AS VARCHAR) + ')';

	 if(@ErrorNum=547 and @ErrorSeverity=16)
		begin 
			PRINT 'No se pudo eliminar, ya que tiene relacion con otros modulos'
		end 

END CATCH
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[uspEliminarProducto]  
@Id int  
as  
BEGIN TRY

  DELETE FROM Producto WHERE IdProducto = @Id
  DELETE FROM Kardex   WHERE IdProducto = @Id

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

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[uspEliminarProveedor]  
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

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[uspEliminarUsuario] 
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

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[uspGuardarUnidadMedidaProducto]  
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

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[uspIngresarFeriado]
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

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[uspIngresarPersonal]
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

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[uspInsertarArea] 
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

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[uspInsertarCategoria]    
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

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[uspInsertarCuentaProveedor]
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

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[uspInsertarMaquina]
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

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[uspInsertarProducto]
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

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[uspInsertarProveedor] 
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

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[uspInsertarUsuario]  
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

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[uspListarFeriados]
AS
BEGIN
    SELECT IdFeriado, Fecha, Motivo
    FROM Feriados
    ORDER BY IdFeriado desc;
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[uspListarMaquinas]
as
Begin
SELECT IdMaquina, Maquina,
convert(varchar,Registro,103)+' '+SUBSTRING(convert(varchar,Registro,114),1,8) as Registro,
SerieFactura,SerieNC, SerieBoleta, Tiketera
FROM MAQUINAS 
order by 1 asc
End
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[uspListarProducto]  
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
      'Â¬' +  
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
      'Â¬' +  
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

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[uspObtenerFeriadoPorId]
    @Id int
AS
BEGIN
    SELECT IdFeriado,Fecha, Motivo
    FROM Feriados
    WHERE IdFeriado = @Id;
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[uspObtenerProductoPorId]
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

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[uspObtenerProveedorPorId]
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

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[uspResumenFecha]
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

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[uspTraerFeriados] AS 
BEGIN SET NOCOUNT ON; 
SELECT ISNULL((SELECT STUFF((SELECT 'Â¬' + CONVERT(VARCHAR, f.IdFeriado) + '|' +
CONVERT(VARCHAR(20), f.Fecha, 23) + '|' + 
ISNULL(f.Motivo, '') 
FROM dbo.Feriados f 
ORDER BY f.Fecha ASC 
FOR XML PATH('')), 1, 1, '')), '~'); END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[editarPersonal]
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

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[listarPersonal]
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

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[ListarUsuario]  
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

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[uspEditarRB]
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

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[uspinsertarNotaB]                                 
@ListaOrden varchar(Max)                                  
as                                  
begin                                  
Declare @pos1 int,@pos2 int,@pos3 int                
Declare @orden varchar(max),                
        @detalle varchar(max),                
        @Guia varchar(max)                
Set @pos1 = CharIndex('[',@ListaOrden,0)                
Set @pos2 = CharIndex('[',@ListaOrden,@pos1+1)                
Set @pos3 =Len(@ListaOrden)+1                
Set @orden = SUBSTRING(@ListaOrden,1,@pos1-1)                
Set @detalle = SUBSTRING(@ListaOrden,@pos1+1,@pos2-@pos1-1)                
Set @Guia=SUBSTRING(@ListaOrden,@pos2+1,@pos3-@pos2-1)                                
Declare @c1 int,@c2 int,@c3 int,@c4 int,                                  
        @c5 int,@c6 int,@c7 int,@c8 int,                                  
        @c9 int,@c10 int,@c11 int,@c12 int,                                  
        @c13 int,@c14 int,@c15 int,@c16 int,                                  
        @c17 int,@c18 int,@c19 int,@c20 int,                                  
        @c21 int,@c22 int,@c23 int,@c24 int,                                  
        @c25 int,@c26 int,@c27 int,@c28 int,                                  
        @c29 int,@c30 int,@c31 int,@c32 int,                                  
        @c33 int,@C34 int,@c35 int,@C36 int,                    
        @c37 int,@C38 int,@c39 int,@C40 int,                    
        @c41 int,@C42 int                                
Declare                                   
  @NotaDocu varchar(60),@ClienteId numeric(20),                                  
  @NotaUsuario varchar(60),@NotaFormaPago varchar(60),                                  
  @NotaCondicion varchar(60),@NotaDireccion varchar(max),                                  
  @NotaTelefono varchar(60),@NotaSubtotal decimal (18,2),                                  
  @NotaMovilidad decimal(18,2),@NotaDescuento decimal (18, 2),                                  
  @NotaTotal decimal (18,2),@NotaAcuenta decimal(18,2),                                  
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
  @CajaId varchar(38),@Movimiento varchar(40),@KARDEX VARCHAR(1),                            
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
Set @c28= CharIndex('|',@orden,@c27+1)                                  
Set @c29= CharIndex('|',@orden,@c28+1)                                  
Set @c30= CharIndex('|',@orden,@c29+1)                                  
Set @c31= CharIndex('|',@orden,@c30+1)                                  
Set @c32= CharIndex('|',@orden,@c31+1)                            
Set @c33= CharIndex('|',@orden,@c32+1)                    
Set @c34= CharIndex('|',@orden,@c33+1)                                  
Set @c35= CharIndex('|',@orden,@c34+1)                                  
Set @c36= CharIndex('|',@orden,@c35+1)                            
Set @c37= CharIndex('|',@orden,@c36+1)                
                
Set @c38= CharIndex('|',@orden,@c37+1)                                  
Set @c39= CharIndex('|',@orden,@c38+1)                                  
Set @c40= CharIndex('|',@orden,@c39+1)                            
Set @c41= CharIndex('|',@orden,@c40+1)                                   
Set @c42= Len(@orden)+1                
                                  
set @NotaDocu=SUBSTRING(@orden,1,@c1-1)                                  
set @ClienteId=convert(numeric(20),SUBSTRING(@orden,@c1+1,@c2-@c1-1))                                  
set @NotaUsuario=SUBSTRING(@orden,@c2+1,@c3-@c2-1)                                  
set @NotaFormaPago=SUBSTRING(@orden,@c3+1,@c4-@c3-1)                                  
set @NotaCondicion=SUBSTRING(@orden,@c4+1,@c5-@c4-1)                                  
set @NotaDireccion=SUBSTRING(@orden,@c5+1,@c6-@c5-1)                            
set @NotaTelefono=SUBSTRING(@orden,@c6+1,@c7-@c6-1)                                  
set @NotaSubtotal=convert(decimal(18,2),SUBSTRING(@orden,@c7+1,@c8-@c7-1))                                  
set @NotaMovilidad=convert(decimal(18,2),SUBSTRING(@orden,@c8+1,@c9-@c8-1))                                  
set @NotaDescuento=convert(decimal(18,2),SUBSTRING(@orden,@c9+1,@c10-@c9-1))                                  
set @NotaTotal=convert(decimal(18,2),SUBSTRING(@orden,@c10+1,@c11-@c10-1))                                  
set @NotaAcuenta=convert(decimal(18,2),SUBSTRING(@orden,@c11+1,@c12-@c11-1))                                  
set @NotaSaldo=convert(decimal(18,2),SUBSTRING(@orden,@c12+1,@c13-@c12-1))                                  
set @NotaAdicional=convert(decimal(18,2),SUBSTRING(@orden,@c13+1,@c14-@c13-1))                                  
set @NotaTarjeta=convert(decimal(18,2),SUBSTRING(@orden,@c14+1,@c15-@c14-1))                                  
set @NotaPagar=convert(decimal(18,2),SUBSTRING(@orden,@c15+1,@c16-@c15-1))                                  
set @NotaEstado=SUBSTRING(@orden,@c16+1,@c17-@c16-1)                                  
set @CompaniaId=convert(int,SUBSTRING(@orden,@c17+1,@c18-@c17-1))                                  
set @NotaEntrega=SUBSTRING(@orden,@c18+1,@c19-@c18-1)                           
set @NotaConcepto=SUBSTRING(@orden,@c19+1,@c20-@c19-1)                                  
set @Serie=SUBSTRING(@orden,@c20+1,@c21-@c20-1)                                  
set @Numero=SUBSTRING(@orden,@c21+1,@c22-@c21-1)                                  
set @NotaGanancia=convert(decimal(18,2),SUBSTRING(@orden,@c22+1,@c23-@c22-1))                                  
set @Letra=SUBSTRING(@orden,@c23+1,@c24-@c23-1)             
set @DocuAdicional=convert(decimal(18,2),SUBSTRING(@orden,@c24+1,@c25-@c24-1))                                  
set @DocuHash=SUBSTRING(@orden,@c25+1,@c26-@c25-1)                                  
set @EstadoSunat=SUBSTRING(@orden,@c26+1,@c27-@c26-1)                                  
set @DocuSubtotal=convert(decimal(18,2),SUBSTRING(@orden,@c27+1,@c28-@c27-1))                                  
set @DocuIGV=convert(decimal(18,2),SUBSTRING(@orden,@c28+1,@c29-@c28-1))             
set @UsuarioId=convert(int,SUBSTRING(@orden,@c29+1,@c30-@c29-1))                                  
set @ICBPER=convert(decimal(18,2),SUBSTRING(@orden,@c30+1,@c31-@c30-1))                                  
set @DocuGravada=convert(decimal(18,2),SUBSTRING(@orden,@c31+1,@c32-@c31-1))                                  
set @DocuDescuento=convert(decimal(18,2),SUBSTRING(@orden,@c32+1,@c33-@c32-1))                            
set @NotaIdBR=SUBSTRING(@orden,@c33+1,@c34-@c33-1)                    
                    
set @EntidadBancaria=SUBSTRING(@orden,@c34+1,@c35-@c34-1)                          
set @NroOperacion=SUBSTRING(@orden,@c35+1,@c36-@c35-1)                          
set @Efectivo=convert(decimal(18,2),SUBSTRING(@orden,@c36+1,@c37-@c36-1))                          
set @Deposito=convert(decimal(18,2),SUBSTRING(@orden,@c37+1,@c38-@c37-1))                
                
set @ClienteRazon=SUBSTRING(@orden,@c38+1,@c39-@c38-1)                          
set @ClienteRuc=SUBSTRING(@orden,@c39+1,@c40-@c39-1)                 
set @ClienteDni=SUBSTRING(@orden,@c40+1,@c41-@c40-1)                          
set @DireccionFiscal=SUBSTRING(@orden,@c41+1,@c42-@c41-1)                              
                          
if(@NotaIdBR='')set @NotaIdBR=0                          
                               
IF EXISTS(select top 1 n.NotaId                             
from NotaPedido n                             
where n.NotaId =@NotaIdBR)                            
begin                              
select 'EXISTE'                                 
end                            
else                            
begin             

-- =============================================
-- CAJA TEMPORALMENTE DESHABILITADA
-- =============================================
-- IF NOT EXISTS (SELECT 1 FROM Caja WHERE CajaId = @CajaId)
-- BEGIN
--     SET @CajaId = NULL
-- END
-- if(@CajaId=0)                                  
-- begin                                  
--     select 'No Aperturo Caja'                                  
-- END
-- else
-- begin
-- =============================================

if(@NotaDocu='FACTURA')                                  
begin                                  
   -- FACTURA debe comportarse igual que BOLETA:
   -- Si es CREDITO => EMITIDO con saldo
   -- Si es ALCONTADO => CANCELADO con saldo 0
   if(@NotaCondicion='CREDITO')                        
   BEGIN                        
   set @NotaEstado='EMITIDO'                                  
   set @NotaSaldo=@NotaPagar                                  
   set @NotaAcuenta=0                        
   END                        
   ELSE                        
   BEGIN                                 
   set @NotaEstado='CANCELADO'                                  
   set @NotaSaldo=0                                 
   set @NotaAcuenta=@NotaPagar                        
   END                                 
end
else if(@NotaDocu='PROFORMA')set @NotaEstado='PENDIENTE'                                  
else                                  
begin                            
   if(@NotaCondicion='CREDITO')                        
   BEGIN                        
   set @NotaEstado='EMITIDO'                                  
   set @NotaSaldo=@NotaPagar                                  
   set @NotaAcuenta=0                        
   END                        
   ELSE                        
   BEGIN                                 
   set @NotaEstado='CANCELADO'                                  
   set @NotaSaldo=0                                 
   set @NotaAcuenta=@NotaPagar                        
   END                                 
END                       
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
else if(@FormaA='YAPE' OR @FormaA='PLIN')set @Movimiento='DEPOSITO'                                  
else set @Movimiento='TARJETA'                     
                    
if(@FormaB='EFECTIVO')set @MovimientoB='INGRESO'                                  
else if(@FormaB='DEPOSITO')set @MovimientoB='DEPOSITO'                                  
else if(@FormaB='YAPE' OR @FormaB='PLIN')set @MovimientoB='DEPOSITO'                                  
else set @MovimientoB='TARJETA'                     
                    
END                    
Else                    
begin                    
                    
if(@NotaFormaPago='EFECTIVO')set @Movimiento='INGRESO'                                  
else if(@NotaFormaPago='DEPOSITO')set @Movimiento='DEPOSITO'    
else if(@NotaFormaPago='TRANSFERENCIA')set @Movimiento='DEPOSITO'    
else if(@NotaFormaPago='YAPE' OR @NotaFormaPago='PLIN')set @Movimiento='DEPOSITO'                                  
else set @Movimiento='TARJETA'                     
                    
End                    
End                   
                                 
declare @NotaId numeric(38),                                  
        @DocuId numeric(38)=0                    
                                                      
Begin Transaction                  
                                
update Cliente                                  
set ClienteDespacho=@NotaDireccion,ClienteTelefono=@NotaTelefono                                  
where ClienteId=@ClienteId                                  
delete from TemporalVenta                                   
where UsuarioID=@UsuarioId                            
                                  
declare @cod varchar(13)                            
                                  
SET @cod=ISNULL((select TOP 1                             
dbo.genenerarNroFactura(@Serie,@CompaniaId,@NotaDocu) AS ID                             
FROM DocumentoVenta),'00000001')                           
                                 
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
  
                                  
set @NotaId=(select @@IDENTITY)                    
                                  
if (@NotaDocu='PROFORMA V')                                  
Begin                                  
 insert into DocumentoVenta values                                  
 (@CompaniaId,@NotaId,'PROFORMA V',@cod,@ClienteId,GETDATE(),             
 GETDATE(),@NotaCondicion,@Letra,@DocuSubtotal,                                  
 @DocuIGV,@NotaPagar,0,@NotaUsuario,'EMITIDO',@Serie,'00',@NotaMovilidad,'','VENTA','',                                  
 @DocuHash,'ENVIADO',                                  
 @ICBPER,'','',@DocuGravada,@DocuDescuento,'',@NotaFormaPago,@EntidadBancaria,                
 @NroOperacion,@Efectivo,@Deposito,@ClienteRazon,@ClienteRuc,@ClienteDni,@DireccionFiscal)                
                
 set @DocuId=(select @@IDENTITY)                        
                         
if(@NotaCondicion<>'CREDITO')                        
BEGIN                         
if(@pZ1>0)                    
begin                    
    -- CAJA DESHABILITADA TEMPORALMENTE
    -- if(@Movimiento='INGRESO')                    
    -- BEGIN                    
    -- insert into CajaDetalle values(@CajaId,GETDATE(),@NotaId,@Movimiento,'',                                  
    -- 'TransacciÃ³n con '+@NotaFormaPago,@Efectivo,@Efectivo,0,'','T','',@NotaUsuario,'','')                                  
    -- END                    
    -- else                  
    -- begin                    
    -- insert into CajaDetalle values(@CajaId,GETDATE(),@NotaId,@Movimiento,'',                                  
    -- 'TransacciÃ³n con '+@NotaFormaPago,@Deposito,@Deposito,0,'','T','',@NotaUsuario,'','')                     
    -- end                    
    -- if(@MovimientoB='INGRESO')                    
    -- BEGIN                    
    -- insert into CajaDetalle values(@CajaId,GETDATE(),@NotaId,@MovimientoB,'',                                  
    -- 'TransacciÃ³n con '+@NotaFormaPago,@Efectivo,@Efectivo,0,'','T','',@NotaUsuario,'','')                     
    -- END                    
    -- ELSE                
    -- BEGIN                    
    -- insert into CajaDetalle values(@CajaId,GETDATE(),@NotaId,@MovimientoB,'',                                  
    -- 'TransacciÃ³n con '+@NotaFormaPago,@Deposito,@Deposito,0,'','T','',@NotaUsuario,'','')                     
    -- END                    
    SET @KARDEX='S'  -- se mantiene activo                              
END                  
ELSE                     
BEGIN                    
    -- CAJA DESHABILITADA TEMPORALMENTE
    -- insert into CajaDetalle values(@CajaId,GETDATE(),@NotaId,@Movimiento,'',                                  
    -- 'TransacciÃ³n con '+@NotaFormaPago,@NotaPagar,@NotaPagar,0,'','T','',@NotaUsuario,'','')                         
    SET @KARDEX='S'  -- se mantiene activo                     
END                    
END                                
END                                
Else if(@NotaDocu='BOLETA')                                  
Begin                    
                                     
 insert into DocumentoVenta values                                  
 (@CompaniaId,@NotaId,'BOLETA',@cod,@ClienteId,GETDATE(),                                  
 GETDATE(),@NotaCondicion,@Letra,@DocuSubtotal,                                  
 @DocuIGV,@NotaPagar,0,@NotaUsuario,'EMITIDO',@Serie,'03',@NotaMovilidad,'','VENTA','',                                  
 @DocuHash,@EstadoSunat,                                  
 @ICBPER,'','',@DocuGravada,@DocuDescuento,'',                
 @NotaFormaPago,@EntidadBancaria,@NroOperacion,@Efectivo,@Deposito,                
 @ClienteRazon,@ClienteRuc,@ClienteDni,@DireccionFiscal)                                  
 set @DocuId=(select @@IDENTITY)                        
                    
if(@NotaConcepto='MERCADERIA')                                      
begin                        
if(@NotaCondicion<>'CREDITO')                        
BEGIN                    
if(@pZ1>0)                    
begin                    
    -- CAJA DESHABILITADA TEMPORALMENTE
    -- if(@Movimiento='INGRESO')                    
    -- BEGIN                    
    -- insert into CajaDetalle values(@CajaId,GETDATE(),@NotaId,@Movimiento,'',                                  
    -- 'TransacciÃ³n con '+@NotaFormaPago,@Efectivo,@Efectivo,0,'','T','',@NotaUsuario,'','')                                  
    -- END                    
    -- else                    
    -- begin                    
    -- insert into CajaDetalle values(@CajaId,GETDATE(),@NotaId,@Movimiento,'',                                  
    -- 'TransacciÃ³n con '+@NotaFormaPago,@Deposito,@Deposito,0,'','T','',@NotaUsuario,'','')            
    -- end                    
    -- if(@MovimientoB='INGRESO')                    
    -- BEGIN                    
    -- insert into CajaDetalle values(@CajaId,GETDATE(),@NotaId,@MovimientoB,'',                                  
    -- 'TransacciÃ³n con '+@NotaFormaPago,@Efectivo,@Efectivo,0,'','T','',@NotaUsuario,'','')                     
    -- END                    
    -- ELSE                    
    -- BEGIN                    
    -- insert into CajaDetalle values(@CajaId,GETDATE(),@NotaId,@MovimientoB,'',                                  
    -- 'TransacciÃ³n con '+@NotaFormaPago,@Deposito,@Deposito,0,'','T','',@NotaUsuario,'','')                   
    -- END                    
    SET @KARDEX='S'                       
END                    
ELSE                     
BEGIN                    
                       
    SET @KARDEX='S'               
END                    
END                            
END                  
END                       
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
Set @CodigoPro=SUBSTRING(@Columna,@p1+1,@p2-(@p1+1))                                      
Set @DetalleCantidad=convert(decimal(18,2),SUBSTRING(@Columna,@p2+1,@p3-(@p2+1)))                                  
Set @DetalleUm=SUBSTRING(@Columna,@p3+1,@p4-(@p3+1))                                  
Set @Descripcion=SUBSTRING(@Columna,@p4+1,@p5-(@p4+1))                                  
Set @DetalleCosto=convert(decimal(18,4),SUBSTRING(@Columna,@p5+1,@p6-(@p5+1)))                                  
Set @DetallePrecio=convert(decimal(18,2),SUBSTRING(@Columna,@p6+1,@p7-(@p6+1)))                                  
Set @DetalleImporte=convert(decimal(18,2),SUBSTRING(@Columna,@p7+1,@p8-(@p7+1)))            
Set @DetalleEstado=SUBSTRING(@Columna,@p8+1,@p9-(@p8+1))                                
set @ValorUM=convert(decimal(18,4),SUBSTRING(@Columna,@p9+1,@p10-(@p9+1)))        
set @AplicaINV=SUBSTRING(@Columna,@p10+1,@p11-(@p10+1))                          
                          
Declare @CantidadSal decimal(18,2)                          
                             
if(@NotaEntrega='INMEDIATA')Set @CantidadSaldo=0                                  
else Set @CantidadSaldo=@DetalleCantidad                                
                                  
insert into DetallePedido values(@NotaId,@IdProducto,@DetalleCantidad,                                  
@DetalleUm,@Descripcion,@DetalleCosto, @DetallePrecio,                                  
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
                             
 set @CantidadSal=@DetalleCantidad * @ValorUM                            
                                
 set @IniciaStock=(select top 1 ProductoCantidad from Producto where IdProducto=@IdProducto)                                    
 set @StockFinal=@IniciaStock-@CantidadSal                                   
                             
 insert into Kardex values(@IdProducto,GETDATE(),'Salida por Venta',@Serie+'-'+@cod,@IniciaStock,                                    
 0,@CantidadSal,@DetalleCosto,@StockFinal,'SALIDA',@NotaUsuario)                                    
                               
 update producto                                     
 set  ProductoCantidad =ProductoCantidad - @CantidadSal                                   
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
Declare @g1 int,@g2 int,              
        @g3 int,@g4 int,@g5 int              
              
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
Set @g5=Len(@ColumnaB)+1                 
               
set @CantidadA=Convert(decimal(18,2),SUBSTRING(@ColumnaB,1,@g1-1))              
Set @IdProductoU=Convert(numeric(20),SUBSTRING(@ColumnaB,@g1+1,@g2-(@g1+1)))              
Set @CantidadU=Convert(decimal(18,2),SUBSTRING(@ColumnaB,@g2+1,@g3-(@g2+1)))                
Set @Um=SUBSTRING(@ColumnaB,@g3+1,@g4-(@g3+1))                
Set @ValorUMU=Convert(decimal(18,4),SUBSTRING(@ColumnaB,@g4+1,@g5-(@g4+1)))                    
              
 Declare @CantidadSalB decimal(18,2)               
              
 set @CantidadSalB=(@CantidadA * @CantidadU)* @ValorUMU                          
                              
 set @IniciaStockB=(select top 1 p.ProductoCantidad               
 from Producto p where p.IdProducto=@IdProductoU)                                  
               
 set @StockFinalB=@IniciaStockB-@CantidadSalB                                 
                           
 insert into Kardex values(@IdProductoU,GETDATE(),'Salida por Venta',@Serie+'-'+@cod,@IniciaStockB,                                  
 0,@CantidadSalB,0,@StockFinalB,'SALIDA',@NotaUsuario)                                  
                             
 update producto                            
 set  ProductoCantidad =ProductoCantidad - @CantidadSalB                                
 where IDProducto=@IdProductoU                  
              
Fetch Next From TablaB INTO @ColumnaB                
end                
    Close TablaB;                
    Deallocate TablaB;                
    Commit Transaction;                
    select convert(varchar,@NotaId)+'Â¬'+@cod                
end                
else                
begin                
    Commit Transaction;                
    select convert(varchar,@NotaId)+'Â¬'+@cod                
end                
                                

END 
END  
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[uspListaBajas]  
@Data varchar(max)  
as  
begin  
Declare @p1 int  
Declare @CompaniaId int  
Set @Data = LTRIM(RTrim(@Data))  
set @CompaniaId=@Data  
select  
'DocuId|Compania|NotaId|FechaEmision|Documento|Numero|RazonSocial|DNI|SubTotal|IGV|ICBPER|Total|Usuario|EstadoÂ¬100|80|100|115|95|130|350|90|115|115|100|115|160|125Â¬String|String|String|String|String|String|String|String|String|String|String|String|String|String|StringÂ¬'+  
isnull((select STUFF((select 'Â¬'+convert(varchar,d.DocuId)+'|'+convert(varchar,d.CompaniaId)+'|'+convert(varchar,d.NotaId)+'|'+  
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

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[uspListaDocumentos]
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
                'Â¬'
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
        + 'Â§'
        + ISNULL(NULLIF(@detalle, ''), '~') AS Resultado;
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[uspRetornaBoletaPorTicket]
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

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[uspValidaUsuario]                        
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
                'Â¬' + CONVERT(VARCHAR, U.UsuarioID) + '|' +                    
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
  
                ISNULL(c.CompaniaUserSecun, '') + '|' +   -- Usuario SOL  
                ISNULL(c.ComapaniaPWD, '') + '|' +        -- Clave SOL  
                ISNULL(c.CompaniaPFX, '') + '|' +         -- Certificado Base64  
                ISNULL(c.CompaniaClave, '') + '|' +       -- Clave Certificado  
                ISNULL(CONVERT(VARCHAR, c.TIPO_PROCESO), '3')+'|'+ -- Entorno  
                ISNULL(c.CompaniaTelefono,'')

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

