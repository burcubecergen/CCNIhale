CREATE TABLE [dbo].[Yetkiler] (
    [ID]           INT IDENTITY (1, 1) NOT NULL,
    [UserID]       INT NOT NULL,
    [IhalePaketID] INT NOT NULL,
    [CevapYetkisi] BIT NOT NULL
);

