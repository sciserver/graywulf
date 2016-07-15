
CREATE TABLE dbo.Tmp_JobInstance
	(
	EntityGuid uniqueidentifier NOT NULL,
	WorkflowTypeName nvarchar(1024) NULL,
	DateStarted datetime NULL,
	DateFinished datetime NULL,
	JobExecutionStatus int NOT NULL,
	JobTimeout bigint NULL,
	SuspendTimeout datetime NULL,
	ScheduleType int NOT NULL,
	ScheduleTime datetime NULL,
	RecurringPeriod int NULL,
	RecurringInterval int NULL,
	RecurringMask bigint NULL,
	WorkflowInstanceId uniqueidentifier NULL,
	AdminRequestTime datetime NULL,
	AdminRequestData nvarchar(MAX) NULL,
	AdminRequestResult int NULL,
	ExceptionMessage nvarchar(MAX) NULL,
	Parameters xml NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO

INSERT INTO dbo.Tmp_JobInstance (EntityGuid, WorkflowTypeName, DateStarted, DateFinished, JobExecutionStatus, SuspendTimeout, ScheduleType, ScheduleTime, RecurringPeriod, RecurringInterval, RecurringMask, WorkflowInstanceId, AdminRequestTime, AdminRequestData, AdminRequestResult, ExceptionMessage, Parameters)
SELECT EntityGuid, WorkflowTypeName, DateStarted, DateFinished, JobExecutionStatus, SuspendTimeout, ScheduleType, ScheduleTime, RecurringPeriod, RecurringInterval, RecurringMask, WorkflowInstanceId, AdminRequestTime, AdminRequestData, AdminRequestResult, ExceptionMessage, Parameters FROM dbo.JobInstance WITH (HOLDLOCK TABLOCKX)
GO

DROP TABLE dbo.JobInstance
GO

EXECUTE sp_rename N'dbo.Tmp_JobInstance', N'JobInstance', 'OBJECT' 

ALTER TABLE dbo.JobInstance ADD CONSTRAINT
	PK_JobInstance PRIMARY KEY CLUSTERED 
	(
	EntityGuid
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

