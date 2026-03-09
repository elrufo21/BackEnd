IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Compras_CompraEstado_CompraId' AND object_id = OBJECT_ID('dbo.Compras'))
BEGIN
    CREATE INDEX IX_Compras_CompraEstado_CompraId
        ON dbo.Compras (CompraEstado ASC, CompraId DESC);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_DetalleCompra_CompraId_DetalleId' AND object_id = OBJECT_ID('dbo.DetalleCompra'))
BEGIN
    CREATE INDEX IX_DetalleCompra_CompraId_DetalleId
        ON dbo.DetalleCompra (CompraId ASC, DetalleId ASC);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_NotaPedido_NotaEstado_NotaId' AND object_id = OBJECT_ID('dbo.NotaPedido'))
BEGIN
    CREATE INDEX IX_NotaPedido_NotaEstado_NotaId
        ON dbo.NotaPedido (NotaEstado ASC, NotaId DESC);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_DetallePedido_NotaId_DetalleId' AND object_id = OBJECT_ID('dbo.DetallePedido'))
BEGIN
    CREATE INDEX IX_DetallePedido_NotaId_DetalleId
        ON dbo.DetallePedido (NotaId ASC, DetalleId ASC);
END
GO
