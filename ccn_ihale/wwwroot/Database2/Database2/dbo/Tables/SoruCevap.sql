CREATE TABLE [dbo].[SoruCevap] (
    [ID]           INT            IDENTITY (1, 1) NOT NULL,
    [IhalePaketID] INT            NOT NULL,
    [Soru]         NVARCHAR (MAX) NOT NULL,
    [Cevap]        NVARCHAR (MAX) NOT NULL,
    [SoruZamani]   DATETIME       NULL,
    [CevapZamani]  DATETIME       NULL,
    [Aktif]        BIT            NULL,
    [SoruNo]       INT            NULL
);

