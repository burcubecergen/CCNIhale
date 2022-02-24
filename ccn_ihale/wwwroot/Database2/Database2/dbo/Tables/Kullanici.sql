CREATE TABLE [dbo].[Kullanici] (
    [ID]         INT            IDENTITY (1, 1) NOT NULL,
    [LoginName]  NVARCHAR (50)  NOT NULL,
    [Sifre]      NVARCHAR (250) NOT NULL,
    [MailAdres]  NVARCHAR (500) NOT NULL,
    [AdSoyad]    NVARCHAR (250) NOT NULL,
    [AdminYetki] BIT            NOT NULL,
    [Aktif]      BIT            NULL,
    CONSTRAINT [PK_Kullanici] PRIMARY KEY CLUSTERED ([ID] ASC) WITH (FILLFACTOR = 90)
);

