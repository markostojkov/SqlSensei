CREATE TABLE [dbo].[JobExecution]
(
	[Id]			BIGINT				IDENTITY (1, 1) NOT NULL,
	[CompanyFk]		BIGINT				NOT NULL,
	[ServerFk]		BIGINT				NOT NULL,
	[Type]			SMALLINT			NOT NULL,
	[Status]		SMALLINT			NOT NULL,
	[CreatedOn]		SMALLDATETIME		NOT NULL,
	[CompletedOn]	SMALLDATETIME		NULL,
	CONSTRAINT [PK_JobExecution] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (FILLFACTOR = 100),
	CONSTRAINT [FK_JobExecution_Company] FOREIGN KEY ([CompanyFk]) REFERENCES [dbo].[Company]([Id]),
	CONSTRAINT [FK_JobExecution_Server] FOREIGN KEY ([ServerFk]) REFERENCES [dbo].[Server]([Id])
)
