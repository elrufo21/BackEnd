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
