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
        -- 🔄 UPDATE
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
        -- ➕ INSERT (mínimo necesario)
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
