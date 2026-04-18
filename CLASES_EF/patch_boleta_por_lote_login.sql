SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF COL_LENGTH('dbo.Compania', 'BoletaPorLote') IS NULL
BEGIN
    ALTER TABLE dbo.Compania
    ADD BoletaPorLote bit NOT NULL
        CONSTRAINT DF_Compania_BoletaPorLote DEFAULT ((1));
END
ELSE
BEGIN
    IF OBJECT_ID('DF_Compania_BoletaPorLote', 'D') IS NULL
    BEGIN
        ALTER TABLE dbo.Compania
        ADD CONSTRAINT DF_Compania_BoletaPorLote DEFAULT ((1)) FOR BoletaPorLote;
    END
END
GO

ALTER PROCEDURE [dbo].[uspValidaUsuario]
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
