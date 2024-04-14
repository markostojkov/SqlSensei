CREATE TABLE [dbo].[MonitoringJobServerWaitStatLog]
(
    [Id]                        BIGINT IDENTITY(1,1)    NOT NULL,
    [CompanyFk]                 BIGINT                  NOT NULL,
    [JobFk]                     BIGINT                  NOT NULL,
    [TimeInMs]                  BIGINT                  NOT NULL,
    [Type]                      NVARCHAR(60)            NOT NULL,
    CONSTRAINT [PK_MonitoringJobServerWaitStatLog] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_MonitoringJobServerWaitStatLog_JobExecution] FOREIGN KEY ([JobFk]) REFERENCES [dbo].[JobExecution]([Id]),
    CONSTRAINT [FK_MonitoringJobServerWaitStatLog_Company] FOREIGN KEY ([CompanyFk]) REFERENCES [dbo].[Company]([Id])
);
