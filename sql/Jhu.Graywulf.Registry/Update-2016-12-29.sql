-- Add DisplayName column to Entities table

/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
CREATE TABLE dbo.Tmp_Entity
	(
	Guid uniqueidentifier NOT NULL,
	ConcurrencyVersion timestamp NOT NULL,
	ParentGuid uniqueidentifier NOT NULL,
	EntityType int NOT NULL,
	Number int NOT NULL,
	Name nvarchar(128) NOT NULL,
	DisplayName nvarchar(128) NOT NULL,
	Version nvarchar(25) NOT NULL,
	System bit NOT NULL,
	Hidden bit NOT NULL,
	ReadOnly bit NOT NULL,
	[Primary] bit NOT NULL,
	Deleted bit NOT NULL,
	LockOwner uniqueidentifier NULL,
	RunningState int NOT NULL,
	AlertState int NOT NULL,
	DeploymentState int NOT NULL,
	UserGuidOwner uniqueidentifier NOT NULL,
	DateCreated datetime NOT NULL,
	UserGuidCreated uniqueidentifier NOT NULL,
	DateModified datetime NOT NULL,
	UserGuidModified uniqueidentifier NOT NULL,
	DateDeleted datetime NULL,
	UserGuidDeleted uniqueidentifier NULL,
	Settings xml NULL,
	Comments nvarchar(MAX) NOT NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_Entity SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.Tmp_Entity ADD CONSTRAINT
	DF_Entity_DisplayName DEFAULT '' FOR DisplayName
GO
IF EXISTS(SELECT * FROM dbo.Entity)
	 EXEC('INSERT INTO dbo.Tmp_Entity (Guid, ParentGuid, EntityType, Number, Name, Version, System, Hidden, ReadOnly, [Primary], Deleted, LockOwner, RunningState, AlertState, DeploymentState, UserGuidOwner, DateCreated, UserGuidCreated, DateModified, UserGuidModified, DateDeleted, UserGuidDeleted, Settings, Comments)
		SELECT Guid, ParentGuid, EntityType, Number, Name, Version, System, Hidden, ReadOnly, [Primary], Deleted, LockOwner, RunningState, AlertState, DeploymentState, UserGuidOwner, DateCreated, UserGuidCreated, DateModified, UserGuidModified, DateDeleted, UserGuidDeleted, Settings, Comments FROM dbo.Entity WITH (HOLDLOCK TABLOCKX)')
GO
DROP TABLE dbo.Entity
GO
EXECUTE sp_rename N'dbo.Tmp_Entity', N'Entity', 'OBJECT' 
GO
ALTER TABLE dbo.Entity ADD CONSTRAINT
	PK_Entity PRIMARY KEY CLUSTERED 
	(
	ParentGuid,
	EntityType,
	Number,
	Guid
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
CREATE UNIQUE NONCLUSTERED INDEX IX_Entity_Guid ON dbo.Entity
	(
	Guid
	) WITH( PAD_INDEX = OFF, FILLFACTOR = 60, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
COMMIT


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
	@DisplayName nvarchar(128),
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
		 Number, Name, DisplayName, Version,
		 System, Hidden, ReadOnly, [Primary], Deleted, LockOwner,
		 RunningState, AlertState, DeploymentState, UserGuidOwner, DateCreated, UserGuidCreated,
		 DateModified, UserGuidModified, DateDeleted, UserGuidDeleted, Settings, Comments)
	VALUES
		(@Guid, @ParentGuid, @EntityType,  
		 @Number, @Name, @DisplayName, @Version,
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
	@DisplayName nvarchar(128),
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
		DisplayName = @DisplayName,
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

-------------------------------------------------------
DECLARE @federationGuid uniqueidentifier

SELECT @federationGuid = Guid
FROM Entity e
INNER JOIN Federation f ON e.Guid = f.EntityGuid
WHERE Name = 'SkyQuery'

-- Delete old schema source server references

DELETE EntityReference
WHERE EntityGuid = @federationGuid AND ReferenceType = 2

-- Drop unused column from database definition

ALTER TABLE dbo.DatabaseDefinition
DROP COLUMN SchemaSourceDatabaseName

