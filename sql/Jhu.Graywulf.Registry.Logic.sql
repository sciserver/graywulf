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
	-- Determine order number
	SELECT @Number = MAX(Number) + 1
	FROM Entity WITH (UPDLOCK)
	WHERE ParentGuid = @ParentGuid AND EntityType = @EntityType AND Deleted = 0;
	
	IF (@Number IS NULL)
	BEGIN
		SET @Number = 0;
	END;
	
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
	DECLARE @Number int, @ParentGuid uniqueidentifier, @EntityType int

	SELECT @ConcurrencyVersion = ConcurrencyVersion + 1,
		   @Number = Number,
		   @ParentGuid = ParentGuid,
		   @EntityType = EntityType
	FROM Entity
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


CREATE PROC [dbo].[spCheckEntityDuplicate]
	@UserGuid uniqueidentifier,
	@EntityType int,
	@Guid uniqueidentifier,
	@ParentGuid uniqueidentifier,
	@Name nvarchar(128)
AS
	DECLARE @count int;

	SELECT @count = COUNT(*)
	FROM Entity
	WHERE
		-- Entity.EntityType = @EntityType AND
		(@Guid IS NULL OR Guid <> @Guid)
		AND Entity.ParentGuid = @ParentGuid
		AND Entity.Name = @Name
		AND Deleted = 0;
		
	RETURN ISNULL(@count, 0);
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
	-- Reset lock and increase concurrency version
	
	UPDATE Entity
	SET LockOwner = NULL
	WHERE Guid = @Guid
	
	SET @ConcurrencyVersion = @@DBTS;
	
	RETURN 0
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


CREATE PROC [dbo].[spFindJobInstance_Next]
	@UserGuid uniqueidentifier,
	@QueueInstanceGuid uniqueidentifier,
	@JobInstanceGuid uniqueidentifier OUTPUT
AS
	-- Default return value is null
	SET @JobInstanceGuid = NULL

	DECLARE @cnt int;

	-- Resumed (previously suspended) or suspended but timed out workflow
	SELECT TOP 1 @JobInstanceGuid = Entity.Guid
	FROM Entity
	INNER JOIN JobInstance ON JobInstance.EntityGuid = Entity.Guid
	WHERE Entity.ParentGuid = @QueueInstanceGuid
		AND
			((JobExecutionStatus & dbo.JobExecutionState::Persisted) != 0
			 OR (JobExecutionStatus & dbo.JobExecutionState::Suspended) != 0
			 AND SuspendTimeout < GETDATE())
		AND Deleted = 0;

	IF (@JobInstanceGuid IS NOT NULL) RETURN;

	-- Job scheduled at a given time
	SELECT TOP 1 @JobInstanceGuid = Entity.Guid
	FROM Entity
	INNER JOIN JobInstance ON JobInstance.EntityGuid = Entity.Guid
	WHERE Entity.ParentGuid = @QueueInstanceGuid
		AND (JobExecutionStatus & dbo.JobExecutionState::Scheduled != 0)
		AND ScheduleType = dbo.ScheduleType::Timed
		AND ScheduleTime <= GETDATE()		-- Earlier or now
		AND Deleted = 0;

	IF (@JobInstanceGuid IS NOT NULL) RETURN;

	
	-- Queued job
	SELECT TOP 1 @JobInstanceGuid = Entity.Guid
	FROM Entity
	INNER JOIN JobInstance ON JobInstance.EntityGuid = Entity.Guid
	WHERE Entity.ParentGuid = @QueueInstanceGuid
		AND (JobExecutionStatus & dbo.JobExecutionState::Scheduled) != 0
		AND ScheduleType = dbo.ScheduleType::Queued 
		AND Entity.Deleted = 0
	ORDER BY Entity.Number;				-- The next one in the queue
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
	DELETE JobInstanceCheckpoint
	DELETE JobInstanceParameter
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
	DELETE UserRoleMembership
	
	DELETE Entity
	DELETE EntityReference
GO

