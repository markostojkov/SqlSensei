CREATE TABLE [dbo].[MaintenanceLog]
(
    [Id]                    BIGINT IDENTITY(1,1)        NOT NULL,
    [CompanyFk]             BIGINT                      NOT NULL,
    [DatabaseName]          NVARCHAR(128)               NOT NULL,
    [Index]                 NVARCHAR(128)               NULL,
    [Statistic]             NVARCHAR(MAX)               NULL,
    [IsError]               BIT                         NOT NULL,
    [ErrorMessage]          NVARCHAR(MAX)               NULL,
    CONSTRAINT [PK_MaintenanceLog] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_MaintenanceLog_Company] FOREIGN KEY ([CompanyFk]) REFERENCES [dbo].[Company]([Id])
)
