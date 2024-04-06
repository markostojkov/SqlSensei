CREATE TABLE [dbo].[MonitoringJobIndexUsageLog]
(
    [Id]                    BIGINT IDENTITY(1,1)    NOT NULL,
    [CompanyFk]             BIGINT                  NOT NULL,
    [DatabaseName]          NVARCHAR(128)           NOT NULL,
    [IsClusteredIndex]      BIT                     NOT NULL,
    [IndexName]             NVARCHAR(128)           NOT NULL,
    [TableName]             NVARCHAR(128)           NOT NULL,
    [IndexDetails]          NVARCHAR(MAX)           NOT NULL,
    [Usage]                 NVARCHAR(MAX)           NOT NULL,
    [ReadsUsage]            BIGINT                  NOT NULL,
    [WriteUsage]            BIGINT                  NOT NULL,
    CONSTRAINT [PK_MonitoringJobIndexUsageLog] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_MonitoringJobIndexUsageLog_Company] FOREIGN KEY ([CompanyFk]) REFERENCES [dbo].[Company]([Id])
);
