IF OBJECT_ID('[dbo].[spFindJobInstance_Next]') IS NOT NULL
DROP PROC [dbo].[spFindJobInstance_Next]

GO

CREATE PROC [dbo].[spFindJobInstance_Next]
	@UserGuid uniqueidentifier,					-- Job poller context (scheduler)
	@LockOwner uniqueidentifier,
	@QueueInstanceGuid uniqueidentifier,		-- Queue instance being polled
	@LastUserGuid uniqueidentifier,				-- User of last job scheduler (for round-robin)
	@MaxJobs int								-- Maximum number of jobs to return
AS
	SET NOCOUNT ON
	
	-- Jobs will be collected in a temp table
	CREATE TABLE ##jobs
	(
		JobInstanceGuid uniqueidentifier PRIMARY KEY
	)

	-- To make sure jobs are picked up in a round-robin manner first we try
	-- UserGuid > LastUserGuid then UserGuid <= LastUserGuid
	DECLARE @count int = 0;
	DECLARE @attempt int = 0;

	WHILE (@attempt < 2)
	BEGIN
		WITH q AS
		(
			SELECT Entity.Guid AS JobInstanceGuid, ROW_NUMBER() OVER (ORDER BY Number) AS rn
			FROM Entity WITH(ROWLOCK, UPDLOCK)
			INNER JOIN JobInstance WITH(ROWLOCK, UPDLOCK)
				ON JobInstance.EntityGuid = Entity.Guid
			WHERE Entity.ParentGuid = @QueueInstanceGuid
				AND Deleted = 0
				AND (LockOwner IS NULL)
				AND (@attempt = 0 AND Entity.UserGuidOwner > @LastUserGuid OR
					 @attempt = 1 AND Entity.UserGuidOwner <= @LastUserGuid)
				AND (
					-- Resumed (previously persisted)
					((JobExecutionStatus & dbo.JobExecutionState::Persisted) != 0)
					  
					 OR 
					 
					 -- Suspended but timed out workflows
					((JobExecutionStatus & dbo.JobExecutionState::Suspended) != 0
					  AND SuspendTimeout < GETDATE())

					 OR

					 -- Job scheduled for a given time
					((JobExecutionStatus & dbo.JobExecutionState::Scheduled != 0)
					  AND ScheduleType = dbo.ScheduleType::Timed
					  AND ScheduleTime <= GETDATE())		-- Earlier or now

					OR

					-- Queued job
					((JobExecutionStatus & dbo.JobExecutionState::Scheduled) != 0
					  AND ScheduleType = dbo.ScheduleType::Queued)

					)
		)
		INSERT ##jobs
		SELECT JobInstanceGuid FROM q WHERE rn <= @MaxJobs;

		SELECT @count = COUNT(*) FROM ##jobs;
		SET @MaxJobs = @MaxJobs - @count;

		IF (@MaxJobs <= 0) BREAK;

		-- If not enough jobs have found, proceed to second attempt which allows
		-- for picking up jobs from the last user as well

		SET @attempt = @attempt + 1;
	END

	SET NOCOUNT OFF

	UPDATE Entity
	SET LockOwner = @LockOwner
	FROM Entity
	INNER JOIN ##jobs ON Entity.Guid = ##jobs.JobInstanceGuid

	SELECT * FROM Entity
	INNER JOIN JobInstance ON JobInstance.EntityGuid = Entity.Guid
	WHERE Guid IN (SELECT JobInstanceGuid FROM ##jobs)

	DROP TABLE ##jobs
GO

----------------------------------------------------------------

IF OBJECT_ID('[dbo].[spGetJobInstance]') IS NOT NULL
DROP PROC [dbo].[spGetJobInstance]

GO

CREATE PROC [dbo].[spGetJobInstance]
	@UserGuid uniqueidentifier,					-- Job poller context (scheduler)
	@LockOwner uniqueidentifier,
	@JobInstanceGuid uniqueidentifier
AS
	SET NOCOUNT ON

	UPDATE Entity
	SET LockOwner = @LockOwner
	FROM Entity
	WHERE Guid = @JobInstanceGuid AND (LockOwner IS NULL OR LockOwner = @LockOwner)

	SET NOCOUNT OFF

	SELECT * FROM Entity
	INNER JOIN JobInstance ON JobInstance.EntityGuid = Entity.Guid
	WHERE Guid = @JobInstanceGuid AND (LockOwner IS NULL OR LockOwner = @LockOwner)