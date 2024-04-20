CREATE TABLE [dbo].[User]
(
	[Identifier]			NVARCHAR(256)		NOT NULL,
	[CompanyFk]				BIGINT				NOT NULL,
	[AuthProvider]			INT					NOT NULL DEFAULT 0,
	CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([Identifier] ASC) WITH (FILLFACTOR = 100),
	CONSTRAINT [FK_User_Company] FOREIGN KEY ([CompanyFk]) REFERENCES [dbo].[Company]([Id])
)
