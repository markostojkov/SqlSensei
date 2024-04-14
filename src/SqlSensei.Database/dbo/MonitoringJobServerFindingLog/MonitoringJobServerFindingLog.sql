CREATE TABLE [dbo].[MonitoringJobServerFindingLog]
(
    [Id]                        BIGINT IDENTITY(1,1)    NOT NULL,
    [CompanyFk]                 BIGINT                  NOT NULL,
    [JobFk]                     BIGINT                  NOT NULL,
    [Priority]                  TINYINT                 NULL,
    [CheckId]                   INT                     NOT NULL,
    [Details]                   NVARCHAR(MAX)           NULL,
    CONSTRAINT [PK_MonitoringJobServerFindingLog] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_MonitoringJobServerFindingLog_JobExecution] FOREIGN KEY ([JobFk]) REFERENCES [dbo].[JobExecution]([Id]),
    CONSTRAINT [FK_MonitoringJobServerFindingLog_Company] FOREIGN KEY ([CompanyFk]) REFERENCES [dbo].[Company]([Id])
);
