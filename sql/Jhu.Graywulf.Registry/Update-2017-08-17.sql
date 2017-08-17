-- Drop JobExecutionStatus column and use RunningState instead

UPDATE Entity
SET RunningState = JobExecutionStatus
FROM Entity
INNER JOIN JobInstance ON Guid = EntityGuid

GO

BEGIN TRANSACTION

ALTER TABLE dbo.JobInstance
	DROP COLUMN JobExecutionStatus

COMMIT

GO

-- Create index to find jobs quickly

IF (SELECT COUNT(*) FROM sys.indexes WHERE name = 'IX_Entity_Jobs') > 0
DROP INDEX IX_Entity_Jobs ON Entity

CREATE UNIQUE INDEX IX_Entity_Jobs ON Entity
(
	UserGuidOwner,
	DateCreated,
	Guid
)
INCLUDE
(
	ParentGuid,
	Number,
	Version,
	System,
	Hidden,
	ReadOnly,
	[Primary],
	Deleted,
	LockOwner,
	RunningState,
	AlertState,
	DeploymentState
)
WHERE EntityType = 0x00200400

GO

----------------------------------------------------------------
