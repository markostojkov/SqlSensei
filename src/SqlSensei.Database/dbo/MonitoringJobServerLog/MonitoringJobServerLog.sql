CREATE TABLE [dbo].[MonitoringJobServerLog]
(
    [Id]                        BIGINT IDENTITY(1,1)    NOT NULL,
    [CompanyFk]                 BIGINT                  NOT NULL,
    [JobFk]                     BIGINT                  NOT NULL,
    [DatabaseName]              NVARCHAR(128)           NULL,
    [Priority]                  TINYINT                 NULL,
    [CheckId]                   INT                     NOT NULL,
    [Details]                   NVARCHAR(MAX)           NULL,
    CONSTRAINT [PK_MonitoringJobServerLog] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_MonitoringJobServerLog_JobExecution] FOREIGN KEY ([JobFk]) REFERENCES [dbo].[JobExecution]([Id]),
    CONSTRAINT [FK_MonitoringJobServerLog_Company] FOREIGN KEY ([CompanyFk]) REFERENCES [dbo].[Company]([Id])
);
