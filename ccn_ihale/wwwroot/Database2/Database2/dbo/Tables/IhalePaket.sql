CREATE TABLE [dbo].[IhalePaket] (
    [ID]             INT            IDENTITY (1, 1) NOT NULL,
    [ProjectID]      INT            NOT NULL,
    [IhalePaketName] NVARCHAR (250) NOT NULL,
    [AcilanTarih]    DATETIME       NOT NULL,
    [FTPDosyalar]    NVARCHAR (MAX) NOT NULL
);

