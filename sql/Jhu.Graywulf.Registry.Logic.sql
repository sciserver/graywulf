-- ENABLE CLR

-- sp_configure 'show advanced options', 1;
-- GO
-- RECONFIGURE;
-- GO
-- sp_configure 'clr enabled', 1;
-- GO
-- RECONFIGURE;
-- GO

-- USER DEFINE TYPES --

CREATE TYPE [dbo].[DeploymentState]
EXTERNAL NAME [Jhu.Graywulf.Registry.Enum].[Jhu.Graywulf.Registry.Sql.DeploymentState]

GO


CREATE TYPE [dbo].[JobExecutionState]
EXTERNAL NAME [Jhu.Graywulf.Registry.Enum].[Jhu.Graywulf.Registry.Sql.JobExecutionState]

GO


CREATE TYPE [dbo].[RunningState]
EXTERNAL NAME [Jhu.Graywulf.Registry.Enum].[Jhu.Graywulf.Registry.Sql.RunningState]

GO


CREATE TYPE [dbo].[ScheduleType]
EXTERNAL NAME [Jhu.Graywulf.Registry.Enum].[Jhu.Graywulf.Registry.Sql.ScheduleType]

GO


-- ENTITY FUNCTIONS --

IF OBJECT_ID('[dbo].[spCheckEntityConcurrency]') IS NOT NULL
DROP PROC [dbo].[spCheckEntityConcurrency]

GO

CREATE PROC [dbo].[spCheckEntityConcurrency]
	@UserGuid uniqueidentifier,
	@Guid uniqueidentifier,
	@LockOwner uniqueidentifier,
	@ConcurrencyVersion binary(8)
AS

	DECLARE @v int, @l uniqueidentifier

	SELECT @v = ConcurrencyVersion, @l = LockOwner
	FROM Entity
	WHERE GUID = @GUID

	IF (@v <> @ConcurrencyVersion)
	BEGIN
		RETURN -1
	END
	
	IF (@l IS NOT NULL AND @l <> @LockOwner)
	BEGIN
		RETURN -2
	END
	
	RETURN 0
GO


IF OBJECT_ID('[dbo].[spCheckEntityDuplicate]') IS NOT NULL
DROP PROC [dbo].[spCheckEntityDuplicate]

GO

CREATE PROC [dbo].[spCheckEntityDuplicate]
	@UserGuid uniqueidentifier,
	@Guid uniqueidentifier,
	@ParentGuid uniqueidentifier,
	@Name nvarchar(128)
AS
	DECLARE @count int;

	SELECT @count = COUNT(*)
	FROM Entity
	WHERE
		(@Guid IS NULL OR Guid <> @Guid)
		AND Entity.ParentGuid = @ParentGuid
		AND Entity.Name = @Name
		AND Deleted = 0;
		
	RETURN ISNULL(@count, 0);
GO


IF OBJECT_ID('[dbo].[spCreateEntity]') IS NOT NULL
DROP PROC [dbo].[spCreateEntity]

GO

CREATE PROC [dbo].[spCreateEntity]
	@UserGuid uniqueidentifier,
	
	@ConcurrencyVersion binary(8) OUTPUT,
	@Number int OUTPUT,
	
	@Guid uniqueidentifier,
	@ParentGuid uniqueidentifier,
	@EntityType int,
	@Name nvarchar(128),
	@Version nvarchar(25),
	@RunningState int,
	@AlertState int,
	@DeploymentState int,
	@DateCreated datetime,
	@DateModified datetime,
	@Settings nvarchar(max),
	@Comments nvarchar(max),
	@EntityReferences EntityReferenceList READONLY
AS

	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

	-- Check entity duplicate
	DECLARE @duplicate int

	SELECT @duplicate = ISNULL(COUNT(*), 0)
	FROM Entity WITH (XLOCK)
	WHERE Entity.ParentGuid = @ParentGuid
		AND Entity.Name = @Name
		AND (@Guid IS NULL OR Guid <> @Guid)
		AND Deleted = 0;

	IF @duplicate > 0
	BEGIN
		THROW 51000, 'Duplicate entity name', 0;
	END;

	-- Determine order number
	SELECT @Number = ISNULL(MAX(Number), 0) + 1
	FROM Entity WITH (XLOCK)
	WHERE ParentGuid = @ParentGuid AND EntityType = @EntityType AND
		  Deleted = 0

	-- Insert new entity
	INSERT Entity
		(Guid, ParentGuid, EntityType, 
		 Number, Name, Version,
		 System, Hidden, ReadOnly, [Primary], Deleted, LockOwner,
		 RunningState, AlertState, DeploymentState, UserGuidOwner, DateCreated, UserGuidCreated,
		 DateModified, UserGuidModified, DateDeleted, UserGuidDeleted, Settings, Comments)
	VALUES
		(@Guid, @ParentGuid, @EntityType,  
		 @Number, @Name, @Version,
		 0, 0, 0, 0, 0, NULL,
		 @RunningState, @AlertState, @DeploymentState, @UserGuid, @DateCreated, @UserGuid,
		 @DateModified, @UserGuid, NULL, NULL, @Settings, @Comments);
		 
	-- Current time stamp		
	SET @ConcurrencyVersion = @@DBTS;
		 
	-- Create entity references
	INSERT EntityReference
		(EntityGuid, ReferenceType, ReferencedEntityGuid)
	SELECT @Guid, ReferenceType, ReferencedEntityGuid
	FROM @EntityReferences

GO


IF OBJECT_ID('[dbo].[spModifyEntity]') IS NOT NULL
DROP PROC [dbo].[spModifyEntity]

GO

CREATE PROC [dbo].[spModifyEntity]
	@UserGuid uniqueidentifier,
	@Guid uniqueidentifier,
	@ConcurrencyVersion binary(8) OUTPUT,
	
	@Name nvarchar(128),
	@Version nvarchar(25),
	@RunningState int,
	@AlertState int,
	@DeploymentState int,
	@DateCreated datetime,
	@DateModified datetime,	
	@Settings nvarchar(max),
	@Comments nvarchar(max),
	@EntityReferences EntityReferenceList READONLY
AS	
	
	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

	-- Check if name has changed, if so, verify duplicates

	DECLARE @parentGuid uniqueidentifier;
	DECLARE @oldName nvarchar(128);

	SELECT @parentGuid = ParentGuid, @oldName = Name
	FROM Entity WITH (XLOCK)
	WHERE Guid = @Guid

	IF (@Name <> @oldName)
	BEGIN
		-- Check entity duplicate
		DECLARE @duplicate int

		SELECT @duplicate = ISNULL(COUNT(*), 0)
		FROM Entity WITH (XLOCK)
		WHERE Entity.ParentGuid = @ParentGuid
			AND Entity.Name = @Name
			AND (@Guid IS NULL OR Guid <> @Guid)
			AND Deleted = 0;

		IF @duplicate > 0
		BEGIN
			THROW 51000, 'Duplicate entity name', 0;
		END;
	END;

	UPDATE Entity
	SET Name = @Name,
		Version = @Version,
		RunningState = @RunningState,
		AlertState = @AlertState,
		DeploymentState = @DeploymentState,
		Settings = @Settings,
		Comments = @Comments,
		UserGuidModified = @UserGuid,
		DateModified = @DateModified
	WHERE GUID = @GUID
	
	SET @ConcurrencyVersion = @@DBTS

	MERGE EntityReference AS er
	USING @EntityReferences AS ner
		ON ner.EntityGuid = er.EntityGuid AND ner.ReferenceType = er.ReferenceType
	WHEN MATCHED THEN 
		UPDATE SET ReferencedEntityGuid = ner.ReferencedEntityGuid
	WHEN NOT MATCHED THEN
		INSERT (EntityGuid, ReferenceType, ReferencedEntityGuid)
		VALUES (ner.EntityGuid, ner.ReferenceType, ner.ReferencedEntityGuid);

GO


CREATE PROC [dbo].[spDeleteEntity]
	@UserGuid uniqueidentifier,
	@Guid uniqueidentifier,
	@ConcurrencyVersion binary(8) OUTPUT
AS

	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

	DECLARE @Number int, @ParentGuid uniqueidentifier, @EntityType int

	SELECT @Number = Number,
		   @ParentGuid = ParentGuid,
		   @EntityType = EntityType
	FROM Entity WITH(XLOCK)
	WHERE GUID = @GUID

	UPDATE Entity
	SET Deleted = 1,
		UserGuidDeleted = @UserGuid,
		DateDeleted = GETDATE()
	WHERE GUID = @GUID
	
	SET @ConcurrencyVersion = @@DBTS;
	
	-- Reorder siblings
	UPDATE Entity
	SET	Number = Number - 1
	WHERE ParentGuid = @ParentGuid AND
		  EntityType = @EntityType AND
		  Number > @Number AND
		  Deleted = 0
GO


CREATE PROC [dbo].[spHideEntity]
	@UserGuid uniqueidentifier,
	@Guid uniqueidentifier,
	@ConcurrencyVersion binary(8) OUTPUT
AS
	SELECT @ConcurrencyVersion = ConcurrencyVersion + 1
	FROM Entity
	WHERE GUID = @GUID
	
	UPDATE Entity
	SET Hidden = 1
	WHERE GUID = @GUID
	
	SET @ConcurrencyVersion = @@DBTS;
GO


CREATE PROC [dbo].[spShowEntity]
	@UserGuid uniqueidentifier,
	@Guid uniqueidentifier,
	@ConcurrencyVersion binary(8) OUTPUT
AS
	SELECT @ConcurrencyVersion = ConcurrencyVersion + 1
	FROM Entity
	WHERE GUID = @GUID
	
	UPDATE Entity
	SET Hidden = 0
	WHERE GUID = @GUID
	
	SET @ConcurrencyVersion = @@DBTS;
GO


CREATE PROC [dbo].[spMoveDownEntity]
	@UserGuid uniqueidentifier,
	@Guid uniqueidentifier
AS
	-- Renumber entity assuming values are consecutive
	DECLARE @Number int, @ParentGuid uniqueidentifier, @EntityType int, @Max int
	
	SELECT @Number = Number, @ParentGuid = ParentGuid, @EntityType = EntityType
	FROM Entity
	WHERE Guid = @Guid AND Deleted = 0

	-- No update if entity not found or deleted
	IF (@Number IS NULL) RETURN -1;
	
	SELECT @Max = MAX(Number)
	FROM Entity
	WHERE ParentGuid = @ParentGuid AND EntityType = @EntityType AND Deleted = 0
	
	-- No update neccessary if already the last one
	IF (@Number = @Max) RETURN @Max
	
	-- Modify previous entity
	UPDATE Entity
	SET Number = Number - 1
	WHERE ParentGuid = @ParentGuid AND EntityType = @EntityType AND Deleted = 0
		AND Number = @Number + 1
		
	-- Modify entity being moved
	UPDATE Entity
	SET Number = Number + 1
	WHERE Guid = @Guid
	
	RETURN @Number - 1;
GO


CREATE PROC [dbo].[spMoveUpEntity]
	@UserGuid uniqueidentifier,
	@Guid uniqueidentifier
AS
	-- Renumber entity assuming values are consecutive
	DECLARE @Number int, @ParentGuid uniqueidentifier, @EntityType int;
	
	SELECT @Number = Number, @ParentGuid = ParentGuid, @EntityType = EntityType
	FROM Entity
	WHERE Guid = @Guid AND Deleted = 0
	
	-- No update if entity not found or deleted
	IF (@Number IS NULL) RETURN -1;
	
	-- No update neccessary if already the first
	IF (@Number = 0) RETURN 0
	
	-- Modify previous entity
	UPDATE Entity
	SET Number = Number + 1
	WHERE ParentGuid = @ParentGuid AND EntityType = @EntityType AND Deleted = 0
		AND Number = @Number - 1
		
	-- Modify entity being moved
	UPDATE Entity
	SET Number = Number - 1
	WHERE Guid = @Guid
	
	RETURN @Number - 1;
GO


CREATE PROC [dbo].[spObtainEntityLock]
	@UserGuid uniqueidentifier,
	@Guid uniqueidentifier,
	@LockOwner uniqueidentifier,
	@ConcurrencyVersion binary(8) OUTPUT
AS
	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

	-- Set lock and increase concurrency version
	
	UPDATE Entity
	SET LockOwner = @LockOwner
	WHERE Guid = @Guid
	
	SET @ConcurrencyVersion = @@DBTS;
	
	RETURN 0
GO


CREATE PROC [dbo].[spReleaseEntityLock]
	@UserGuid uniqueidentifier,
	@Guid uniqueidentifier,
	@LockOwner uniqueidentifier,
	@ConcurrencyVersion binary(8) OUTPUT
AS
	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

	-- Reset lock and increase concurrency version
	
	UPDATE Entity
	SET LockOwner = NULL
	WHERE Guid = @Guid
	
	SET @ConcurrencyVersion = @@DBTS;
	
	RETURN 0
GO


CREATE PROC [dbo].[spFindEntityAscendants]
(
	@Guid uniqueidentifier
)
AS

	WITH ascendants AS
	(
		SELECT e.Guid, e.EntityType, e.ParentGuid, e.Name, 0 AS level
		FROM Entity e
		WHERE Guid = @guid
	
		UNION ALL

		SELECT e.Guid, e.EntityType, e.ParentGuid, e.Name, a.level + 1 AS level
		FROM Entity e
		INNER JOIN ascendants a
			ON e.Guid = a.ParentGuid
	)
	SELECT guid, entityType, name
	FROM ascendants
	ORDER BY level DESC

GO


CREATE FUNCTION [dbo].[fGetEntityGuid_byName]
(
	@EntityType int,
	@NameParts NamePartList READONLY
)
RETURNS uniqueidentifier
AS
BEGIN
	-- Declare the return variable here
	DECLARE @namecount int, @count int;
	DECLARE @name nvarchar(128);
	DECLARE @guid uniqueidentifier = '00000000-0000-0000-0000-000000000000';
	DECLARE @nguid uniqueidentifier = NULL;

	SELECT @namecount = COUNT(*) FROM @NameParts

	DECLARE parts CURSOR FORWARD_ONLY FAST_FORWARD READ_ONLY
	FOR SELECT Name FROM @NameParts ORDER BY ID;
	
	OPEN parts;
	
	FETCH NEXT FROM parts INTO @name;
	
	SET @count = 1;
	WHILE (@@FETCH_STATUS = 0)
	BEGIN
		SET @nguid  = NULL;
	
		SELECT @nguid = Guid
		FROM Entity
		WHERE
			(@count < @namecount OR @EntityType IS NULL OR Entity.EntityType = @EntityType)
			AND Entity.Deleted = 0
			AND Entity.ParentGuid = @guid
			AND Entity.Name = @name;
		
		IF (@nguid IS NULL)
		BEGIN
			RETURN NULL;
		END
		
		SET @guid = @nguid;
	
		FETCH NEXT FROM parts INTO @name;
		SET @count = @count + 1
	END 
	
	CLOSE parts;
	DEALLOCATE parts;
	
	-- Return the result of the function
	RETURN @guid

END
GO


CREATE PROC spFindEntity_byName
	@UserGuid uniqueidentifier,
	@EntityType int,
	@NameParts NamePartList READONLY
AS
    DECLARE @guid uniqueidentifier;

    SELECT @guid = dbo.fGetEntityGuid_byName(@EntityType, @NameParts);
    
    IF (@guid IS NULL)
    BEGIN
            RAISERROR ('Entity not found', 1, 1);
    END


    SELECT Entity.*
    FROM Entity
    WHERE
        Entity.Guid = @guid
        AND Deleted = 0;

GO
	


CREATE PROC [dbo].[spFindEntityReference]
	@UserGuid uniqueidentifier,
	@Guid uniqueidentifier
AS
	SELECT EntityReference.*
	FROM EntityReference
	WHERE EntityGuid = @Guid
GO


CREATE PROC [dbo].[spFindJobInstance_byDetails]
	@UserGuid uniqueidentifier,
	@ShowHidden bit,
	@ShowDeleted bit,
	@From int,
	@Max int,
	@RowCount int OUTPUT,
	@JobUserGuid uniqueidentifier,
	@QueueInstanceGuids GuidList READONLY,
	@JobDefinitionGuids GuidList READONLY,
	@JobExecutionStatus int
AS
	SET NOCOUNT ON;
	
	DECLARE @qicount int;
	SELECT @qicount = COUNT(*) FROM @QueueInstanceGuids;
	
	DECLARE @jdcount int;
	SELECT @jdcount = COUNT(*) FROM @JobDefinitionGuids;

	SET NOCOUNT OFF;

	WITH q AS
	(
		SELECT Entity.*, JobInstance.*, ROW_NUMBER () OVER ( ORDER BY DateCreated DESC ) AS rn
		FROM Entity
		INNER JOIN JobInstance ON JobInstance.EntityGuid = Entity.Guid
		INNER JOIN EntityReference JobDefinition ON JobDefinition.ReferenceType = 1 AND JobDefinition.EntityGuid = Entity.Guid
		WHERE 
			(@ShowHidden = 1 OR Entity.Hidden = 0) AND
			(@ShowDeleted = 1 OR Entity.Deleted = 0) AND
			
			(@JobUserGUID = Entity.UserGuidCreated OR @JobUserGuid IS NULL) AND
			(Entity.ParentGuid IN (SELECT Guid FROM @QueueInstanceGuids) OR @qicount = 0) AND
			(JobDefinition.ReferencedEntityGuid IN (SELECT Guid FROM @JobDefinitionGuids) OR @jdcount = 0) AND
			((@JobExecutionStatus & JobInstance.JobExecutionStatus) != 0 OR @JobExecutionStatus IS NULL)
	)
	SELECT q.* FROM q
	WHERE rn BETWEEN @From + 1 AND @From + @Max OR @From IS NULL OR @Max IS NULL
	ORDER BY rn

	SET @RowCount = @@ROWCOUNT
GO


IF OBJECT_ID('[dbo].[spFindJobInstance_Next]') IS NOT NULL
DROP PROC [dbo].[spFindJobInstance_Next]

GO

CREATE PROC [dbo].[spFindJobInstance_Next]
	@UserGuid uniqueidentifier,					-- Job poller context (scheduler)
	@QueueInstanceGuid uniqueidentifier,		-- Queue instance being polled
	@LastUserGuid uniqueidentifier,				-- User of last job scheduler (for round-robin)
	@MaxJobs int								-- Maximum number of jobs to return
AS
	SET NOCOUNT ON
	
	-- Jobs will be collected in a temp table
	CREATE TABLE ##jobs
	(
		JobInstanceGuid uniqueidentifier
	)

	-- To make sure jobs are picked up in a round-robin manner first we try
	-- UserGuid > LastUserGuid then UserGuid <= LastUserGuid
	DECLARE @attempt int = 0;

	WHILE (@attempt < 2)
	BEGIN

		-- Resumed (previously suspended) or suspended but timed out workflow

		WITH q AS
		(
			SELECT Entity.Guid AS JobInstanceGuid, ROW_NUMBER() OVER (ORDER BY Number) AS rn
			FROM Entity
			INNER JOIN JobInstance ON JobInstance.EntityGuid = Entity.Guid
			WHERE Entity.ParentGuid = @QueueInstanceGuid
				AND
					((JobExecutionStatus & dbo.JobExecutionState::Persisted) != 0
					 OR (JobExecutionStatus & dbo.JobExecutionState::Suspended) != 0
					 AND SuspendTimeout < GETDATE())
				AND Deleted = 0
				AND (@attempt = 0 AND Entity.UserGuidOwner > @LastUserGuid OR
					 @attempt = 1 AND Entity.UserGuidOwner <= @LastUserGuid)
		)
		INSERT ##jobs
		SELECT JobInstanceGuid FROM q WHERE rn <= @MaxJobs;

		SELECT @MaxJobs = @MaxJobs - COUNT(*) FROM ##jobs;

		IF (@MaxJobs <= 0) BREAK;

		-- Job scheduled at a given time

		WITH q AS
		(
			SELECT Entity.Guid AS JobInstanceGuid, ROW_NUMBER() OVER (ORDER BY Number) AS rn
			FROM Entity
			INNER JOIN JobInstance ON JobInstance.EntityGuid = Entity.Guid
			WHERE Entity.ParentGuid = @QueueInstanceGuid
				AND (JobExecutionStatus & dbo.JobExecutionState::Scheduled != 0)
				AND ScheduleType = dbo.ScheduleType::Timed
				AND ScheduleTime <= GETDATE()		-- Earlier or now
				AND Deleted = 0
				AND (@attempt = 0 AND Entity.UserGuidOwner > @LastUserGuid OR
					 @attempt = 1 AND Entity.UserGuidOwner <= @LastUserGuid)
		)
		INSERT ##jobs
		SELECT JobInstanceGuid FROM q WHERE rn <= @MaxJobs;

		SELECT @MaxJobs = @MaxJobs - COUNT(*) FROM ##jobs;

		IF (@MaxJobs <= 0) BREAK;
	
		-- Queued job

		WITH q AS
		(
			SELECT Entity.Guid AS JobInstanceGuid, ROW_NUMBER() OVER (ORDER BY Number) AS rn
			FROM Entity
			INNER JOIN JobInstance ON JobInstance.EntityGuid = Entity.Guid
			WHERE Entity.ParentGuid = @QueueInstanceGuid
				AND (JobExecutionStatus & dbo.JobExecutionState::Scheduled) != 0
				AND ScheduleType = dbo.ScheduleType::Queued 
				AND Entity.Deleted = 0
				AND (@attempt = 0 AND Entity.UserGuidOwner > @LastUserGuid OR
					 @attempt = 1 AND Entity.UserGuidOwner <= @LastUserGuid)
		)
		INSERT ##jobs
		SELECT JobInstanceGuid FROM q WHERE rn <= @MaxJobs;

		SELECT @MaxJobs = @MaxJobs - COUNT(*) FROM ##jobs;

		IF (@MaxJobs <= 0) BREAK;

		-- If not enough jobs found, proceed to second attempt which allows
		-- for picking up jobs from the last user

		SET @attempt = @attempt + 1;
	END

	SET NOCOUNT OFF

	SELECT * FROM Entity
	INNER JOIN JobInstance ON JobInstance.EntityGuid = Entity.Guid
	WHERE Guid IN (SELECT JobInstanceGuid FROM ##jobs)

	DROP TABLE ##jobs
GO


CREATE PROC [dbo].[spFindReferencingEntity]
	@UserGuid uniqueidentifier,
	@ShowHidden bit,
	@ShowDeleted bit,
	@Guid uniqueidentifier
AS
	SELECT Entity.*
	FROM Entity
	INNER JOIN EntityReference ON EntityReference.EntityGuid = Entity.Guid
	WHERE EntityReference.ReferencedEntityGuid = @Guid AND
		(@ShowHidden = 1 OR Entity.Hidden = 0) AND
		(@ShowDeleted = 1 OR Entity.Deleted = 0)
GO


-- USER MANAGEMENT --


CREATE PROC [dbo].[spFindUser_byDomainActivationCode]
	@DomainGuid uniqueidentifier,
	@ActivationCode nvarchar(50)
AS
	SELECT Entity.*, [User].*
	FROM Entity
	INNER JOIN [User] ON [User].EntityGuid = Entity.Guid
	WHERE
		Entity.Deleted = 0 AND
		Entity.ParentGuid = @DomainGuid AND
		ActivationCode = @ActivationCode
GO


CREATE PROC [dbo].[spFindUser_byDomainEmail]
	@DomainGuid uniqueidentifier,
	@Email nvarchar(128)
AS
	SELECT Entity.*, [User].*
	FROM Entity
	INNER JOIN [User] ON [User].EntityGuid = Entity.Guid
	WHERE
		Entity.Deleted = 0 AND
		Entity.ParentGuid = @DomainGuid AND
		Email = @Email
GO


CREATE PROC [dbo].[spFindUser_byIdentity]
	@DomainGuid uniqueidentifier,
	@Protocol nvarchar(25),
	@Authority nvarchar(250),
	@Identifier nvarchar(250)
AS
	SELECT Entity.*, [User].*
	FROM Entity
	INNER JOIN [User] ON [User].EntityGuid = Entity.Guid
	INNER JOIN [Entity] ue ON ue.ParentGuid = [User].EntityGuid
	INNER JOIN [UserIdentity] ON ue.Guid = [UserIdentity].EntityGuid
	WHERE
		ue.Deleted = 0 AND
		Entity.ParentGuid = @DomainGuid AND
		[Protocol] = @Protocol AND
		[Authority] = @Authority AND
		[Identifier] = @Identifier
		
GO


CREATE PROC [dbo].[spLoginUser]
	@ParentGuid uniqueidentifier,
	@NameOrEmail nvarchar(128)
AS
	SELECT Entity.*, [User].*
	FROM Entity
	INNER JOIN [User] ON [User].EntityGuid = Entity.Guid
	WHERE
		Entity.Deleted = 0 AND
		Entity.ParentGuid = @ParentGuid AND
		(Email = @NameOrEmail OR Entity.Name = @NameOrEmail)
GO


CREATE PROC [dbo].[spLoginUserNtlm]
	@NtlmUser nvarchar(128)
AS
	SELECT Entity.*, [User].*
	FROM Entity
	INNER JOIN [User] ON [User].EntityGuid = Entity.Guid
	WHERE 
		Entity.Deleted = 0 AND
		NtlmUser = @NtlmUser AND Integrated = 1
GO


CREATE PROC [dbo].[spCheckUserEmailDuplicate]
	@ParentGuid uniqueidentifier,
	@Email nvarchar(128)
AS
	DECLARE @count int;

	SELECT @count = COUNT(*)
	FROM Entity
	INNER JOIN [User] ON [User].EntityGuid = Entity.Guid
	WHERE
		Entity.ParentGuid = @ParentGuid
		AND Deleted = 0
		AND Email = @Email;
		
		
	RETURN ISNULL(@count, 0);
	
GO

	
-- DEVELOPER TOOLS --

CREATE SCHEMA dev
GO

CREATE PROC [dev].[spCleanupEverything]
AS
	DELETE Cluster
	DELETE DatabaseDefinition
	DELETE DatabaseInstance
	DELETE DatabaseInstanceFile
	DELETE DatabaseInstanceFileGroup
	DELETE DatabaseVersion
	DELETE DeploymentPackage
	DELETE DiskVolume
	DELETE Domain
	DELETE Federation
	DELETE FileGroup
	DELETE JobDefinition
	DELETE JobInstance
	DELETE Machine
	DELETE MachineRole
	DELETE Partition
	DELETE QueueDefinition
	DELETE QueueInstance
	DELETE RemoteDatabase
	DELETE ServerInstance
	DELETE ServerVersion
	DELETE Slice
	DELETE [User]
	DELETE UserDatabaseInstance
	DELETE UserGroup
	DELETE UserRole
	DELETE UserGroupMembership
	
	DELETE Entity
	DELETE EntityReference
GO

