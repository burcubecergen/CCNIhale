CREATE TABLE [dbo].[AdminYetki] (
    [ID]        INT           IDENTITY (1, 1) NOT NULL,
    [ProjectID] INT           NOT NULL,
    [UserName]  NVARCHAR (50) NOT NULL
);

