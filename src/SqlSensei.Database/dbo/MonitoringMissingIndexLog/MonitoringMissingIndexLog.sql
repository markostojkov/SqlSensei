CREATE TABLE [dbo].[MonitoringJobIndexMissingLog]
(
    [Id]                        BIGINT IDENTITY(1,1)    NOT NULL,
    [CompanyFk]                 BIGINT                  NOT NULL,
    [DatabaseName]              NVARCHAR(128)           NOT NULL,
    [TableName]                 NVARCHAR(128)           NOT NULL,
    [MagicBenefitNumber]        BIGINT                  NOT NULL,
    [Impact]                    NVARCHAR(MAX)           NOT NULL,
    [IndexDetails]              NVARCHAR(MAX)           NOT NULL,
    CONSTRAINT [PK_MonitoringJobIndexMissingLog] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_MonitoringJobIndexMissingLog_Company] FOREIGN KEY ([CompanyFk]) REFERENCES [dbo].[Company]([Id])
);
